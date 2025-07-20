using UnityEngine;
using UnityEngine.UIElements;
using FrostRealm.Core;

namespace FrostRealm.UI
{
    /// <summary>
    /// Main menu controller handling UI interactions and navigation.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument uiDocument;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip buttonHoverSound;
        
        // UI Elements
        private VisualElement rootElement;
        private Button playButton;
        private Button settingsButton;
        private Button creditsButton;
        private Button quitButton;
        
        void Start()
        {
            InitializeUI();
            SetupButtonHandlers();
        }
        
        void OnEnable()
        {
            // Subscribe to input events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnSubmit += HandleSubmit;
                InputManager.Instance.OnCancel += HandleCancel;
            }
        }
        
        void OnDisable()
        {
            // Unsubscribe from input events
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnSubmit -= HandleSubmit;
                InputManager.Instance.OnCancel -= HandleCancel;
            }
        }
        
        /// <summary>
        /// Initializes UI elements and references.
        /// </summary>
        private void InitializeUI()
        {
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }
            
            if (uiDocument != null)
            {
                rootElement = uiDocument.rootVisualElement;
                
                // Get button references
                playButton = rootElement.Q<Button>("play-button");
                settingsButton = rootElement.Q<Button>("settings-button");
                creditsButton = rootElement.Q<Button>("credits-button");
                quitButton = rootElement.Q<Button>("quit-button");
            }
            else
            {
                Debug.LogWarning("MainMenuController: UIDocument not found, creating fallback UI");
                CreateFallbackUI();
            }
        }
        
        /// <summary>
        /// Creates a simple fallback UI if UIDocument is not available.
        /// </summary>
        private void CreateFallbackUI()
        {
            Debug.Log("Creating fallback main menu UI - Use keyboard shortcuts:");
            Debug.Log("P - Play Game");
            Debug.Log("Q - Quit Game");
        }
        
        /// <summary>
        /// Sets up button click handlers.
        /// </summary>
        private void SetupButtonHandlers()
        {
            if (playButton != null)
            {
                playButton.clicked += OnPlayClicked;
            }
            
            if (settingsButton != null)
            {
                settingsButton.clicked += OnSettingsClicked;
            }
            
            if (creditsButton != null)
            {
                creditsButton.clicked += OnCreditsClicked;
            }
            
            if (quitButton != null)
            {
                quitButton.clicked += OnQuitClicked;
            }
        }
        
        /// <summary>
        /// Handles the play button click.
        /// </summary>
        private void OnPlayClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("Play button clicked - Loading character selection");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadCharacterSelection();
            }
        }
        
        /// <summary>
        /// Handles the settings button click.
        /// </summary>
        private void OnSettingsClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("Settings button clicked - Settings not implemented yet");
            
            // TODO: Implement settings menu
        }
        
        /// <summary>
        /// Handles the credits button click.
        /// </summary>
        private void OnCreditsClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("Credits button clicked - Credits not implemented yet");
            
            // TODO: Implement credits screen
        }
        
        /// <summary>
        /// Handles the quit button click.
        /// </summary>
        private void OnQuitClicked()
        {
            PlaySound(buttonClickSound);
            Debug.Log("Quit button clicked - Exiting game");
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }
        
        /// <summary>
        /// Handles submit input (Enter/Space).
        /// </summary>
        private void HandleSubmit()
        {
            // Default action is to start the game
            OnPlayClicked();
        }
        
        /// <summary>
        /// Handles cancel input (Escape).
        /// </summary>
        private void HandleCancel()
        {
            OnQuitClicked();
        }
        
        /// <summary>
        /// Handles keyboard shortcuts for fallback UI.
        /// </summary>
        void Update()
        {
            if (uiDocument == null)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    OnPlayClicked();
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    OnQuitClicked();
                }
            }
        }
        
        /// <summary>
        /// Plays a sound effect if audio source is available.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}