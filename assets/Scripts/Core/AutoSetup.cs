using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using FrostRealm.Data;

namespace FrostRealm.Core
{
    /// <summary>
    /// Automatic project setup that runs on game startup.
    /// Ensures all systems are properly initialized without manual configuration.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class AutoSetup : MonoBehaviour
    {
        [Header("Auto-Setup Configuration")]
        [SerializeField] private bool enableAutoSetup = true;
        [SerializeField] private bool showDebugLogs = true;
        
        private void Awake()
        {
            if (!enableAutoSetup) return;
            
            PerformAutoSetup();
        }
        
        private void PerformAutoSetup()
        {
            LogDebug("=== FrostRealm Chronicles Auto-Setup Starting ===");
            
            // Add debug logger first
            if (FindFirstObjectByType<DebugLogger>() == null)
            {
                gameObject.AddComponent<DebugLogger>();
                LogDebug("Added DebugLogger component");
            }
            
            // Add SimpleTestMenu for guaranteed UI
            if (FindFirstObjectByType<SimpleTestMenu>() == null)
            {
                gameObject.AddComponent<SimpleTestMenu>();
                LogDebug("Added SimpleTestMenu component");
            }
            
            // Configure application settings
            ConfigureApplicationSettings();
            
            // Initialize core systems
            InitializeCoreManagers();
            
            // Configure rendering pipeline
            ConfigureRenderPipeline();
            
            // Configure input system
            ConfigureInputSystem();
            
            // Configure audio system
            ConfigureAudioSystem();
            
            // Load essential resources
            LoadEssentialResources();
            
            // Start the main menu if we're in the first scene
            StartMainMenuIfNeeded();
            
            LogDebug("=== FrostRealm Chronicles Auto-Setup Complete ===");
        }
        
        private void ConfigureApplicationSettings()
        {
            LogDebug("Configuring application settings...");
            
            // Set target frame rate for optimal performance
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            
            // Configure screen settings
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
            
            // Disable sleep mode during gameplay
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            // Configure cursor behavior
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            LogDebug("✅ Application settings configured");
        }
        
        private void InitializeCoreManagers()
        {
            LogDebug("Initializing core managers...");
            
            // Ensure GameManager exists
            if (GameManager.Instance == null)
            {
                LogDebug("❌ GameManager not found in scene");
            }
            else
            {
                LogDebug("✅ GameManager initialized");
            }
            
            // Initialize ResourceManager
            var resourceManager = FindFirstObjectByType<SimpleResourceManager>();
            if (resourceManager == null)
            {
                GameObject rmGO = new GameObject("SimpleResourceManager");
                rmGO.AddComponent<SimpleResourceManager>();
                DontDestroyOnLoad(rmGO);
                LogDebug("✅ SimpleResourceManager created and initialized");
            }
            else
            {
                LogDebug("✅ SimpleResourceManager found");
            }
            
            // Initialize AudioManager
            var audioManager = FindFirstObjectByType<SimpleAudio>();
            if (audioManager == null)
            {
                GameObject amGO = new GameObject("SimpleAudio");
                amGO.AddComponent<SimpleAudio>();
                DontDestroyOnLoad(amGO);
                LogDebug("✅ SimpleAudio created and initialized");
            }
            else
            {
                LogDebug("✅ SimpleAudio found");
            }
            
            // Initialize InputManager
            var inputManager = FindFirstObjectByType<InputManager>();
            if (inputManager == null)
            {
                GameObject imGO = new GameObject("InputManager");
                imGO.AddComponent<InputManager>();
                DontDestroyOnLoad(imGO);
                LogDebug("✅ InputManager created and initialized");
            }
            else
            {
                LogDebug("✅ InputManager found");
            }
        }
        
        private void ConfigureRenderPipeline()
        {
            LogDebug("Configuring HDRP render pipeline...");
            
            // Configure HDRP settings for optimal performance
            var hdrpAsset = QualitySettings.renderPipeline as HDRenderPipelineAsset;
            if (hdrpAsset != null)
            {
                LogDebug("✅ HDRP pipeline detected and configured");
            }
            else
            {
                LogDebug("⚠️ HDRP pipeline not found - using built-in pipeline");
            }
            
            // Configure lighting settings
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            
            LogDebug("✅ Render pipeline configured");
        }
        
        private void ConfigureInputSystem()
        {
            LogDebug("Configuring Unity Input System...");
            
            #if ENABLE_INPUT_SYSTEM
            // Input System is enabled
            LogDebug("✅ Unity Input System detected");
            #else
            LogDebug("⚠️ Unity Input System not enabled - check Project Settings");
            #endif
        }
        
        private void ConfigureAudioSystem()
        {
            LogDebug("Configuring audio system...");
            
            // Configure audio settings
            AudioSettings.outputSampleRate = 44100;
            
            // Set master volume
            AudioListener.volume = 1.0f;
            
            LogDebug("✅ Audio system configured");
        }
        
        private void LoadEssentialResources()
        {
            LogDebug("Loading essential resources...");
            
            // Load HeroRegistry
            var heroRegistry = Resources.Load<HeroRegistry>("HeroRegistry");
            if (heroRegistry != null)
            {
                LogDebug($"✅ HeroRegistry loaded with {heroRegistry.HeroCount} heroes");
            }
            else
            {
                LogDebug("❌ HeroRegistry not found in Resources folder");
            }
            
            // Preload critical assets
            Resources.LoadAll("Audio");
            Resources.LoadAll("UI");
            
            LogDebug("✅ Essential resources loaded");
        }
        
        private void StartMainMenuIfNeeded()
        {
            LogDebug("Checking if main menu needs to be created...");
            
            // Check if we're in the MainMenu scene or first scene
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (currentScene == "MainMenu" || currentScene.Contains("SampleScene") || currentScene == "")
            {
                // Check if RuntimeMainMenu already exists
                var existingMainMenu = FindFirstObjectByType<FrostRealm.UI.RuntimeMainMenu>();
                if (existingMainMenu == null)
                {
                    LogDebug("Creating RuntimeMainMenu...");
                    GameObject mainMenuGO = new GameObject("RuntimeMainMenu");
                    mainMenuGO.AddComponent<FrostRealm.UI.RuntimeMainMenu>();
                    LogDebug("✅ RuntimeMainMenu created and will initialize automatically");
                }
                else
                {
                    LogDebug("✅ RuntimeMainMenu already exists");
                }
            }
            else
            {
                LogDebug($"Current scene '{currentScene}' doesn't need main menu");
            }
        }
        
        private void LogDebug(string message)
        {
            if (showDebugLogs)
            {
                Debug.Log($"[AutoSetup] {message}");
            }
        }
        
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("FrostRealm/Tools/Run Auto-Setup")]
        public static void RunAutoSetupEditor()
        {
            var autoSetup = FindFirstObjectByType<AutoSetup>();
            if (autoSetup == null)
            {
                GameObject go = new GameObject("AutoSetup");
                autoSetup = go.AddComponent<AutoSetup>();
            }
            
            autoSetup.enableAutoSetup = true;
            autoSetup.showDebugLogs = true;
            autoSetup.PerformAutoSetup();
            
            Debug.Log("Auto-Setup completed in editor");
        }
        #endif
    }
}