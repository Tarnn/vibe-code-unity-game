using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using FrostRealm.Core;
using FrostRealm.Data;

namespace FrostRealm.UI
{
    /// <summary>
    /// Manages hero selection state, preview animations, and selection validation.
    /// Works in conjunction with CharacterSelectionController for enhanced UX.
    /// </summary>
    public class HeroSelectionManager : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float selectionAnimationDuration = 0.3f;
        [SerializeField] private float previewFadeDelay = 0.1f;
        [SerializeField] private AnimationCurve selectionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Preview Settings")]
        [SerializeField] private float heroRotationSpeed = 30f;
        [SerializeField] private bool enableHeroPreview3D = true;
        [SerializeField] private Transform heroPreviewContainer;
        
        [Header("Validation")]
        [SerializeField] private bool requireHeroSelection = true;
        [SerializeField] private bool allowRandomSelection = true;
        
        // State management
        private HeroData currentSelectedHero;
        private GameObject currentPreviewModel;
        private Coroutine previewAnimationCoroutine;
        private bool isAnimating = false;
        
        // Events
        public System.Action<HeroData> OnHeroSelectionChanged;
        public System.Action<HeroData> OnHeroConfirmed;
        public System.Action OnSelectionCancelled;
        
        // Properties
        public HeroData SelectedHero => currentSelectedHero;
        public bool HasValidSelection => currentSelectedHero != null;
        public bool IsAnimating => isAnimating;
        
        void Start()
        {
            InitializePreviewContainer();
            
            // Subscribe to game manager events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHeroSelected += OnHeroSelectedByGameManager;
            }
        }
        
        void OnDestroy()
        {
            // Unsubscribe from events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnHeroSelected -= OnHeroSelectedByGameManager;
            }
            
            // Clean up preview model
            CleanupPreviewModel();
        }
        
        /// <summary>
        /// Initializes the 3D hero preview container.
        /// </summary>
        private void InitializePreviewContainer()
        {
            if (heroPreviewContainer == null && enableHeroPreview3D)
            {
                var previewGO = new GameObject("HeroPreviewContainer");
                previewGO.transform.SetParent(transform);
                heroPreviewContainer = previewGO.transform;
                
                // Position the container off-screen for 3D preview
                heroPreviewContainer.position = new Vector3(1000, 0, 0);
            }
        }
        
        /// <summary>
        /// Selects a hero with animation and validation.
        /// </summary>
        /// <param name="hero">The hero to select</param>
        /// <param name="playAnimation">Whether to play selection animation</param>
        public void SelectHero(HeroData hero, bool playAnimation = true)
        {
            if (hero == null)
            {
                Debug.LogWarning("Attempted to select null hero!");
                return;
            }
            
            if (isAnimating && playAnimation)
            {
                Debug.Log("Selection animation in progress, queuing selection...");
                StartCoroutine(QueuedSelection(hero));
                return;
            }
            
            var previousHero = currentSelectedHero;
            currentSelectedHero = hero;
            
            // Validate selection
            if (!ValidateHeroSelection(hero))
            {
                currentSelectedHero = previousHero;
                return;
            }
            
            // Update 3D preview
            if (enableHeroPreview3D)
            {
                UpdateHeroPreview(hero, playAnimation);
            }
            
            // Notify listeners
            OnHeroSelectionChanged?.Invoke(hero);
            
            // Update game manager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SelectHero(hero);
            }
            
            if (playAnimation)
            {
                StartSelectionAnimation();
            }
            
            Debug.Log($"Hero selected: {hero.HeroName} ({hero.HeroClass})");
        }
        
        /// <summary>
        /// Confirms the current hero selection and proceeds to gameplay.
        /// </summary>
        public void ConfirmSelection()
        {
            if (!HasValidSelection)
            {
                if (allowRandomSelection && HeroRegistry.Instance != null)
                {
                    var randomHero = HeroRegistry.Instance.GetRandomHero();
                    if (randomHero != null)
                    {
                        SelectHero(randomHero, false);
                    }
                }
                
                if (!HasValidSelection)
                {
                    Debug.LogWarning("Cannot confirm selection: No valid hero selected!");
                    return;
                }
            }
            
            // Final validation
            if (!ValidateHeroSelection(currentSelectedHero))
            {
                Debug.LogError("Cannot confirm selection: Hero validation failed!");
                return;
            }
            
            OnHeroConfirmed?.Invoke(currentSelectedHero);
            
            Debug.Log($"Hero selection confirmed: {currentSelectedHero.HeroName}");
        }
        
        /// <summary>
        /// Cancels the current selection and returns to previous state.
        /// </summary>
        public void CancelSelection()
        {
            CleanupPreviewModel();
            currentSelectedHero = null;
            OnSelectionCancelled?.Invoke();
            
            Debug.Log("Hero selection cancelled");
        }
        
        /// <summary>
        /// Gets a random hero from the available options.
        /// </summary>
        public HeroData GetRandomHero()
        {
            if (HeroRegistry.Instance == null)
                return null;
                
            return HeroRegistry.Instance.GetRandomHero();
        }
        
        /// <summary>
        /// Validates if a hero can be selected.
        /// </summary>
        private bool ValidateHeroSelection(HeroData hero)
        {
            if (hero == null)
                return false;
                
            if (!hero.IsValid())
            {
                Debug.LogWarning($"Hero {hero.HeroName} failed validation check!");
                return false;
            }
            
            // Additional validation logic can be added here
            // e.g., check if hero is unlocked, available in current game mode, etc.
            
            return true;
        }
        
        /// <summary>
        /// Updates the 3D hero preview model.
        /// </summary>
        private void UpdateHeroPreview(HeroData hero, bool animate = true)
        {
            if (!enableHeroPreview3D || heroPreviewContainer == null)
                return;
                
            // Clean up previous model
            CleanupPreviewModel();
            
            // Instantiate new preview model
            if (hero.ModelPrefab != null)
            {
                currentPreviewModel = Instantiate(hero.ModelPrefab, heroPreviewContainer);
                
                // Set up preview model (disable unnecessary components, set layer, etc.)
                SetupPreviewModel(currentPreviewModel);
                
                if (animate)
                {
                    StartPreviewAnimation();
                }
            }
        }
        
        /// <summary>
        /// Sets up the preview model for display.
        /// </summary>
        private void SetupPreviewModel(GameObject model)
        {
            if (model == null) return;
            
            // Disable colliders for preview
            var colliders = model.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            
            // Set to UI layer or preview layer
            SetLayerRecursively(model, LayerMask.NameToLayer("UI"));
            
            // Add rotation animation
            var rotator = model.GetComponent<HeroPreviewRotator>();
            if (rotator == null)
            {
                rotator = model.AddComponent<HeroPreviewRotator>();
            }
            rotator.rotationSpeed = heroRotationSpeed;
        }
        
        /// <summary>
        /// Recursively sets the layer for a game object and its children.
        /// </summary>
        private void SetLayerRecursively(GameObject obj, int layer)
        {
            if (obj == null) return;
            
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }
        
        /// <summary>
        /// Cleans up the current preview model.
        /// </summary>
        private void CleanupPreviewModel()
        {
            if (currentPreviewModel != null)
            {
                Destroy(currentPreviewModel);
                currentPreviewModel = null;
            }
            
            if (previewAnimationCoroutine != null)
            {
                StopCoroutine(previewAnimationCoroutine);
                previewAnimationCoroutine = null;
            }
        }
        
        /// <summary>
        /// Starts the selection animation.
        /// </summary>
        private void StartSelectionAnimation()
        {
            if (previewAnimationCoroutine != null)
            {
                StopCoroutine(previewAnimationCoroutine);
            }
            
            previewAnimationCoroutine = StartCoroutine(SelectionAnimationCoroutine());
        }
        
        /// <summary>
        /// Starts the preview entrance animation.
        /// </summary>
        private void StartPreviewAnimation()
        {
            if (currentPreviewModel != null)
            {
                StartCoroutine(PreviewEntranceAnimation());
            }
        }
        
        /// <summary>
        /// Coroutine for selection animation effects.
        /// </summary>
        private IEnumerator SelectionAnimationCoroutine()
        {
            isAnimating = true;
            
            float elapsedTime = 0f;
            while (elapsedTime < selectionAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = selectionCurve.Evaluate(elapsedTime / selectionAnimationDuration);
                
                // Apply animation effects here (scaling, glowing, etc.)
                if (currentPreviewModel != null)
                {
                    float scale = Mathf.Lerp(0.8f, 1f, progress);
                    currentPreviewModel.transform.localScale = Vector3.one * scale;
                }
                
                yield return null;
            }
            
            // Ensure final state
            if (currentPreviewModel != null)
            {
                currentPreviewModel.transform.localScale = Vector3.one;
            }
            
            isAnimating = false;
        }
        
        /// <summary>
        /// Coroutine for preview model entrance animation.
        /// </summary>
        private IEnumerator PreviewEntranceAnimation()
        {
            if (currentPreviewModel == null) yield break;
            
            // Start with model hidden
            currentPreviewModel.transform.localScale = Vector3.zero;
            currentPreviewModel.SetActive(true);
            
            yield return new WaitForSeconds(previewFadeDelay);
            
            float elapsedTime = 0f;
            while (elapsedTime < selectionAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = selectionCurve.Evaluate(elapsedTime / selectionAnimationDuration);
                
                if (currentPreviewModel != null)
                {
                    currentPreviewModel.transform.localScale = Vector3.one * progress;
                }
                
                yield return null;
            }
            
            // Ensure final state
            if (currentPreviewModel != null)
            {
                currentPreviewModel.transform.localScale = Vector3.one;
            }
        }
        
        /// <summary>
        /// Coroutine for queued selection when animation is in progress.
        /// </summary>
        private IEnumerator QueuedSelection(HeroData hero)
        {
            yield return new WaitUntil(() => !isAnimating);
            SelectHero(hero, true);
        }
        
        /// <summary>
        /// Handles hero selection from GameManager.
        /// </summary>
        private void OnHeroSelectedByGameManager(HeroData hero)
        {
            if (hero != currentSelectedHero)
            {
                SelectHero(hero, false);
            }
        }
    }
    
    /// <summary>
    /// Simple component to rotate hero preview models.
    /// </summary>
    public class HeroPreviewRotator : MonoBehaviour
    {
        public float rotationSpeed = 30f;
        
        void Update()
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}