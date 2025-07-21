using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace FrostRealm.Core
{
    /// <summary>
    /// Handles scene loading with automatic transitions and proper initialization.
    /// Ensures smooth gameplay flow without manual scene management.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene Configuration")]
        [SerializeField] private bool autoStartGame = true;
        [SerializeField] private float splashScreenDuration = 2.0f;
        
        // Scene name constants
        public const string MAIN_MENU_SCENE = "MainMenu";
        public const string CHARACTER_SELECTION_SCENE = "CharacterSelection";
        public const string GAMEPLAY_SCENE = "Gameplay";
        
        private static SceneLoader instance;
        public static SceneLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<SceneLoader>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SceneLoader");
                        instance = go.AddComponent<SceneLoader>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // Loading state
        private bool isLoading = false;
        public bool IsLoading => isLoading;
        
        // Events
        public System.Action<string> OnSceneLoadStarted;
        public System.Action<string> OnSceneLoadCompleted;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (autoStartGame)
                {
                    StartCoroutine(AutoStartGameSequence());
                }
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private IEnumerator AutoStartGameSequence()
        {
            // Wait for splash screen
            yield return new WaitForSeconds(splashScreenDuration);
            
            // Check current scene and decide what to do
            string currentScene = SceneManager.GetActiveScene().name;
            
            if (currentScene != MAIN_MENU_SCENE && 
                currentScene != CHARACTER_SELECTION_SCENE && 
                currentScene != GAMEPLAY_SCENE)
            {
                // We're in an unknown scene, go to main menu
                yield return LoadSceneAsync(MAIN_MENU_SCENE);
            }
        }
        
        public void LoadMainMenu()
        {
            StartCoroutine(LoadSceneAsync(MAIN_MENU_SCENE));
        }
        
        public void LoadCharacterSelection()
        {
            StartCoroutine(LoadSceneAsync(CHARACTER_SELECTION_SCENE));
        }
        
        public void LoadGameplay()
        {
            StartCoroutine(LoadSceneAsync(GAMEPLAY_SCENE));
        }
        
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (isLoading)
            {
                Debug.LogWarning($"Already loading a scene. Ignoring request to load {sceneName}");
                yield break;
            }
            
            isLoading = true;
            
            Debug.Log($"Loading scene: {sceneName}");
            OnSceneLoadStarted?.Invoke(sceneName);
            
            // Show loading screen if available
            ShowLoadingScreen(true);
            
            // Load the scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            // Wait until the scene is fully loaded
            while (!asyncLoad.isDone)
            {
                // You could update a loading bar here
                yield return null;
            }
            
            // Scene loaded, perform post-load setup
            yield return new WaitForEndOfFrame();
            
            // Initialize scene-specific systems
            InitializeSceneSystems(sceneName);
            
            // Hide loading screen
            ShowLoadingScreen(false);
            
            isLoading = false;
            
            Debug.Log($"Scene loaded successfully: {sceneName}");
            OnSceneLoadCompleted?.Invoke(sceneName);
        }
        
        private void InitializeSceneSystems(string sceneName)
        {
            switch (sceneName)
            {
                case MAIN_MENU_SCENE:
                    InitializeMainMenuSystems();
                    break;
                    
                case CHARACTER_SELECTION_SCENE:
                    InitializeCharacterSelectionSystems();
                    break;
                    
                case GAMEPLAY_SCENE:
                    InitializeGameplaySystems();
                    break;
            }
        }
        
        private void InitializeMainMenuSystems()
        {
            Debug.Log("Initializing Main Menu systems...");
            
            // Set game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.MainMenu);
            }
            
            // Configure camera
            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.Skybox;
            }
        }
        
        private void InitializeCharacterSelectionSystems()
        {
            Debug.Log("Initializing Character Selection systems...");
            
            // Set game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.CharacterSelection);
            }
            
            // Initialize hero selection UI
            var heroSelectionManager = FindFirstObjectByType<FrostRealm.UI.HeroSelectionManager>();
            if (heroSelectionManager != null)
            {
                // HeroSelectionManager initializes automatically in Start()
                Debug.Log("Found HeroSelectionManager - will auto-initialize");
            }
        }
        
        private void InitializeGameplaySystems()
        {
            Debug.Log("Initializing Gameplay systems...");
            
            // Set game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGameState(GameState.InGame);
            }
            
            // Initialize RTS camera
            var rtsCamera = FindFirstObjectByType<SimpleRTSCamera>();
            if (rtsCamera == null)
            {
                var cameraGO = Camera.main?.gameObject ?? new GameObject("RTS Camera");
                if (cameraGO.GetComponent<Camera>() == null)
                {
                    var cam = cameraGO.AddComponent<Camera>();
                    cam.tag = "MainCamera";
                }
                cameraGO.AddComponent<SimpleRTSCamera>();
            }
            
            // Initialize selection manager
            var selectionManager = FindFirstObjectByType<SimpleSelection>();
            if (selectionManager == null)
            {
                var selectionGO = new GameObject("SelectionManager");
                selectionGO.AddComponent<SimpleSelection>();
            }
            
            // Initialize resource manager UI
            var resourceManager = SimpleResourceManager.Instance;
            if (resourceManager != null)
            {
                resourceManager.ResetResources();
            }
        }
        
        private void ShowLoadingScreen(bool show)
        {
            // You could implement a loading screen UI here
            // For now, just log the state
            if (show)
            {
                Debug.Log("🔄 Loading...");
            }
            else
            {
                Debug.Log("✅ Loading complete");
            }
        }
        
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("FrostRealm/Scenes/Load Main Menu")]
        public static void LoadMainMenuEditor()
        {
            if (Application.isPlaying)
            {
                Instance.LoadMainMenu();
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("assets/Scenes/MainMenu/MainMenu.unity");
            }
        }
        
        [UnityEditor.MenuItem("FrostRealm/Scenes/Load Character Selection")]
        public static void LoadCharacterSelectionEditor()
        {
            if (Application.isPlaying)
            {
                Instance.LoadCharacterSelection();
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("assets/Scenes/CharacterSelection.unity");
            }
        }
        
        [UnityEditor.MenuItem("FrostRealm/Scenes/Load Gameplay")]
        public static void LoadGameplayEditor()
        {
            if (Application.isPlaying)
            {
                Instance.LoadGameplay();
            }
            else
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("assets/Scenes/Gameplay/Gameplay.unity");
            }
        }
        #endif
    }
}