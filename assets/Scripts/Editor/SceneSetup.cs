using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using FrostRealm.Core;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Automatically sets up scenes with required components and systems.
    /// Ensures scenes work without manual configuration in Unity Editor.
    /// </summary>
    public class SceneSetup
    {
        [MenuItem("FrostRealm/Setup/Setup All Scenes")]
        public static void SetupAllScenes()
        {
            Debug.Log("=== Setting up all scenes for FrostRealm Chronicles ===");
            
            SetupMainMenuScene();
            SetupCharacterSelectionScene();
            SetupGameplayScene();
            
            Debug.Log("✅ All scenes setup complete!");
        }
        
        [MenuItem("FrostRealm/Setup/Setup Main Menu Scene")]
        public static void SetupMainMenuScene()
        {
            string scenePath = "assets/Scenes/MainMenu/MainMenu.unity";
            Debug.Log($"Setting up Main Menu scene: {scenePath}");
            
            var scene = EditorSceneManager.OpenScene(scenePath);
            
            // Create main menu components
            SetupMainMenuComponents();
            
            // Save the scene
            EditorSceneManager.SaveScene(scene);
            Debug.Log("✅ Main Menu scene setup complete");
        }
        
        [MenuItem("FrostRealm/Setup/Setup Character Selection Scene")]
        public static void SetupCharacterSelectionScene()
        {
            string scenePath = "assets/Scenes/CharacterSelection.unity";
            Debug.Log($"Setting up Character Selection scene: {scenePath}");
            
            var scene = EditorSceneManager.OpenScene(scenePath);
            
            // Create character selection components
            SetupCharacterSelectionComponents();
            
            // Save the scene
            EditorSceneManager.SaveScene(scene);
            Debug.Log("✅ Character Selection scene setup complete");
        }
        
        [MenuItem("FrostRealm/Setup/Setup Gameplay Scene")]
        public static void SetupGameplayScene()
        {
            string scenePath = "assets/Scenes/Gameplay/Gameplay.unity";
            Debug.Log($"Setting up Gameplay scene: {scenePath}");
            
            var scene = EditorSceneManager.OpenScene(scenePath);
            
            // Create gameplay components
            SetupGameplayComponents();
            
            // Save the scene
            EditorSceneManager.SaveScene(scene);
            Debug.Log("✅ Gameplay scene setup complete");
        }
        
        private static void SetupMainMenuComponents()
        {
            // Ensure AutoSetup exists
            EnsureAutoSetup();
            
            // Ensure SceneLoader exists
            EnsureSceneLoader();
            
            // Add RuntimeMainMenu
            var mainMenu = Object.FindFirstObjectByType<FrostRealm.UI.RuntimeMainMenu>();
            if (mainMenu == null)
            {
                var menuGO = new GameObject("Main Menu Manager");
                menuGO.AddComponent<FrostRealm.UI.RuntimeMainMenu>();
            }
        }
        
        private static void SetupCharacterSelectionComponents()
        {
            // Ensure AutoSetup exists
            EnsureAutoSetup();
            
            // Ensure SceneLoader exists
            EnsureSceneLoader();
            
            // Create UI Canvas if it doesn't exist
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("UI Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // Create EventSystem if it doesn't exist
            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            // Add HeroSelectionManager if it doesn't exist
            var heroSelectionManager = Object.FindFirstObjectByType<UI.HeroSelectionManager>();
            if (heroSelectionManager == null)
            {
                var hsm = new GameObject("HeroSelectionManager");
                hsm.AddComponent<UI.HeroSelectionManager>();
            }
            
            // Add CharacterSelectionController if it doesn't exist
            var charController = Object.FindFirstObjectByType<UI.CharacterSelectionController>();
            if (charController == null)
            {
                var csc = new GameObject("CharacterSelectionController");
                csc.AddComponent<UI.CharacterSelectionController>();
            }
        }
        
        private static void SetupGameplayComponents()
        {
            // Ensure AutoSetup exists
            EnsureAutoSetup();
            
            // Ensure SceneLoader exists
            EnsureSceneLoader();
            
            // Ensure GameManager exists
            EnsureGameManager();
            
            // Generate terrain
            var terrainGen = Object.FindFirstObjectByType<FrostRealm.Core.TerrainGenerator>();
            if (terrainGen == null)
            {
                var terrainGO = new GameObject("Terrain Generator");
                terrainGen = terrainGO.AddComponent<FrostRealm.Core.TerrainGenerator>();
            }
            
            // Setup RTS Camera
            var rtsCamera = Object.FindFirstObjectByType<FrostRealm.Core.SimpleRTSCamera>();
            if (rtsCamera == null)
            {
                var cameraGO = Camera.main?.gameObject;
                if (cameraGO == null)
                {
                    cameraGO = new GameObject("RTS Camera");
                    var cam = cameraGO.AddComponent<Camera>();
                    cam.tag = "MainCamera";
                    // Position camera for RTS view
                    cameraGO.transform.position = new Vector3(64, 20, 40);
                    cameraGO.transform.rotation = Quaternion.Euler(45, 0, 0);
                }
                cameraGO.AddComponent<FrostRealm.Core.SimpleRTSCamera>();
            }
            
            // Setup Selection Manager
            var selectionManager = Object.FindFirstObjectByType<FrostRealm.Core.SimpleSelection>();
            if (selectionManager == null)
            {
                var sm = new GameObject("SelectionManager");
                sm.AddComponent<FrostRealm.Core.SimpleSelection>();
            }
            
            // Setup Resource Manager
            var resourceManager = Object.FindFirstObjectByType<FrostRealm.Core.SimpleResourceManager>();
            if (resourceManager == null)
            {
                var rm = new GameObject("ResourceManager");
                rm.AddComponent<FrostRealm.Core.SimpleResourceManager>();
            }
            
            // Setup Audio Manager
            var audioManager = Object.FindFirstObjectByType<FrostRealm.Core.SimpleAudio>();
            if (audioManager == null)
            {
                var am = new GameObject("AudioManager");
                am.AddComponent<FrostRealm.Core.SimpleAudio>();
            }
            
            // Setup Input Manager
            var inputManager = Object.FindFirstObjectByType<InputManager>();
            if (inputManager == null)
            {
                var im = new GameObject("InputManager");
                im.AddComponent<InputManager>();
            }
            
            // Create sample units for testing
            CreateSampleUnits();
            
            // Create UI Canvas for gameplay UI
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("Gameplay UI Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                var canvasScaler = canvasGO.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // Create EventSystem if it doesn't exist
            var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                var eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }
        
        private static void CreateSampleUnits()
        {
            // Create some sample units for testing
            for (int i = 0; i < 5; i++)
            {
                var unitGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                unitGO.name = $"Test Unit {i + 1}";
                unitGO.transform.position = new Vector3(60 + i * 3, 1, 60);
                
                // Add NavMeshAgent
                var agent = unitGO.AddComponent<UnityEngine.AI.NavMeshAgent>();
                agent.speed = 3.5f;
                
                // Add unit controller
                var unitController = unitGO.AddComponent<FrostRealm.Units.UnitController>();
                
                // Color units
                var renderer = unitGO.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("HDRP/Lit"));
                    mat.color = i % 2 == 0 ? Color.blue : Color.red;
                    renderer.material = mat;
                }
            }
        }
        
        private static void EnsureAutoSetup()
        {
            var autoSetup = Object.FindFirstObjectByType<AutoSetup>();
            if (autoSetup == null)
            {
                var autoSetupGO = new GameObject("AutoSetup");
                autoSetupGO.AddComponent<AutoSetup>();
            }
        }
        
        private static void EnsureSceneLoader()
        {
            var sceneLoader = Object.FindFirstObjectByType<SceneLoader>();
            if (sceneLoader == null)
            {
                var sceneLoaderGO = new GameObject("SceneLoader");
                sceneLoaderGO.AddComponent<SceneLoader>();
            }
        }
        
        private static void EnsureGameManager()
        {
            var gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                var gameManagerGO = new GameObject("GameManager");
                gameManagerGO.AddComponent<GameManager>();
            }
        }
        
        [MenuItem("FrostRealm/Setup/Create Startup Scene")]
        public static void CreateStartupScene()
        {
            string scenePath = "assets/Scenes/Startup.unity";
            
            // Create new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Add AutoSetup component
            var autoSetupGO = new GameObject("AutoSetup");
            autoSetupGO.AddComponent<AutoSetup>();
            
            // Add SceneLoader component
            var sceneLoaderGO = new GameObject("SceneLoader");
            sceneLoaderGO.AddComponent<SceneLoader>();
            
            // Add GameManager component
            var gameManagerGO = new GameObject("GameManager");
            gameManagerGO.AddComponent<GameManager>();
            
            // Save the scene
            EditorSceneManager.SaveScene(scene, scenePath);
            
            Debug.Log($"✅ Startup scene created at: {scenePath}");
            Debug.Log("This scene will automatically initialize all systems and transition to the main menu.");
        }
        
        [MenuItem("FrostRealm/Setup/Validate Scene Setup")]
        public static void ValidateSceneSetup()
        {
            Debug.Log("=== Validating Scene Setup ===");
            
            string[] sceneNames = { "MainMenu", "CharacterSelection", "Gameplay" };
            bool allValid = true;
            
            foreach (string sceneName in sceneNames)
            {
                string scenePath = $"assets/Scenes/{sceneName}/{sceneName}.unity";
                if (sceneName == "CharacterSelection")
                {
                    scenePath = "assets/Scenes/CharacterSelection.unity";
                }
                
                if (System.IO.File.Exists(scenePath))
                {
                    Debug.Log($"✅ Scene found: {sceneName}");
                }
                else
                {
                    Debug.LogError($"❌ Scene missing: {sceneName} at {scenePath}");
                    allValid = false;
                }
            }
            
            if (allValid)
            {
                Debug.Log("✅ All scenes are properly set up!");
            }
            else
            {
                Debug.LogError("❌ Some scenes are missing. Run 'Setup All Scenes' to fix.");
            }
        }
    }
}