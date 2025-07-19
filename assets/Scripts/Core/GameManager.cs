using UnityEngine;
using UnityEngine.SceneManagement;
using FrostRealm.Data;

namespace FrostRealm.Core
{
    /// <summary>
    /// Central game manager handling game state, scene transitions, and persistent data.
    /// Implements singleton pattern for global access.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private GameSettings gameSettings;
        
        [Header("Scene References")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string characterSelectionSceneName = "CharacterSelection";
        [SerializeField] private string gameplaySceneName = "Gameplay";
        
        // Singleton instance
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        // Game state
        [Header("Current State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        [SerializeField] private HeroData selectedHero;
        
        // Events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<HeroData> OnHeroSelected;
        
        // Properties
        public GameState CurrentState => currentState;
        public HeroData SelectedHero => selectedHero;
        public bool DebugMode => debugMode;
        public GameSettings Settings => gameSettings;
        
        void Awake()
        {
            // Ensure singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            // Auto-transition to character selection if we're starting from a gameplay scene
            if (SceneManager.GetActiveScene().name == characterSelectionSceneName)
            {
                SetGameState(GameState.CharacterSelection);
            }
        }
        
        /// <summary>
        /// Initializes core game systems.
        /// </summary>
        private void InitializeGame()
        {
            // Initialize input system
            if (gameSettings != null)
            {
                Application.targetFrameRate = gameSettings.targetFrameRate;
                QualitySettings.vSyncCount = gameSettings.enableVSync ? 1 : 0;
            }
            
            // Validate hero registry
            if (HeroRegistry.Instance != null)
            {
                bool isValid = HeroRegistry.Instance.ValidateAllHeroes();
                if (debugMode)
                {
                    Debug.Log($"Hero registry validation: {(isValid ? "PASSED" : "FAILED")}");
                }
            }
            
            Debug.Log("FrostRealm Chronicles initialized successfully!");
        }
        
        /// <summary>
        /// Changes the current game state and notifies listeners.
        /// </summary>
        /// <param name="newState">The new game state</param>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState)
                return;
                
            GameState previousState = currentState;
            currentState = newState;
            
            if (debugMode)
            {
                Debug.Log($"Game state changed: {previousState} -> {newState}");
            }
            
            OnGameStateChanged?.Invoke(newState);
        }
        
        /// <summary>
        /// Sets the selected hero and notifies listeners.
        /// </summary>
        /// <param name="hero">The selected hero data</param>
        public void SelectHero(HeroData hero)
        {
            if (selectedHero == hero)
                return;
                
            selectedHero = hero;
            
            if (debugMode && hero != null)
            {
                Debug.Log($"Hero selected: {hero.HeroName} ({hero.HeroClass})");
            }
            
            OnHeroSelected?.Invoke(hero);
        }
        
        /// <summary>
        /// Loads the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            SetGameState(GameState.Loading);
            SceneManager.LoadScene(mainMenuSceneName);
            SetGameState(GameState.MainMenu);
        }
        
        /// <summary>
        /// Loads the character selection scene.
        /// </summary>
        public void LoadCharacterSelection()
        {
            SetGameState(GameState.Loading);
            SceneManager.LoadScene(characterSelectionSceneName);
            SetGameState(GameState.CharacterSelection);
        }
        
        /// <summary>
        /// Loads the gameplay scene with the selected hero.
        /// </summary>
        public void LoadGameplay()
        {
            if (selectedHero == null)
            {
                Debug.LogWarning("No hero selected! Using default hero.");
                selectedHero = HeroRegistry.Instance.DefaultHero;
            }
            
            SetGameState(GameState.Loading);
            SceneManager.LoadScene(gameplaySceneName);
            SetGameState(GameState.InGame);
        }
        
        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitGame()
        {
            if (debugMode)
            {
                Debug.Log("Quit game requested");
            }
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Pauses or unpauses the game.
        /// </summary>
        /// <param name="paused">True to pause, false to unpause</param>
        public void SetPaused(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
            SetGameState(paused ? GameState.Paused : GameState.InGame);
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == GameState.InGame)
            {
                SetPaused(true);
            }
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && currentState == GameState.InGame)
            {
                SetPaused(true);
            }
        }
    }
    
    /// <summary>
    /// Defines the possible game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        CharacterSelection,
        Loading,
        InGame,
        Paused,
        GameOver,
        Settings
    }
    
    /// <summary>
    /// Game settings configuration.
    /// </summary>
    [System.Serializable]
    public struct GameSettings
    {
        [Header("Performance")]
        public int targetFrameRate;
        public bool enableVSync;
        
        [Header("Audio")]
        [Range(0f, 1f)] public float masterVolume;
        [Range(0f, 1f)] public float musicVolume;
        [Range(0f, 1f)] public float sfxVolume;
        [Range(0f, 1f)] public float voiceVolume;
        
        [Header("Graphics")]
        public int qualityLevel;
        public bool enableRaytracing;
        public bool enablePostProcessing;
        
        public static GameSettings Default => new GameSettings
        {
            targetFrameRate = 60,
            enableVSync = true,
            masterVolume = 1f,
            musicVolume = 0.7f,
            sfxVolume = 0.8f,
            voiceVolume = 0.9f,
            qualityLevel = 2,
            enableRaytracing = false,
            enablePostProcessing = true
        };
    }
}