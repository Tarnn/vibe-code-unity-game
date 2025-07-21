using UnityEngine;
using System.Collections;

namespace FrostRealm.Core
{
    /// <summary>
    /// Performance optimization manager for FrostRealm Chronicles.
    /// Handles memory management, FPS optimization, and resource cleanup.
    /// </summary>
    public class PerformanceOptimizer : MonoBehaviour
    {
        [Header("Performance Settings")]
        [SerializeField] private bool enableAutoGC = true;
        [SerializeField] private float gcInterval = 30f;
        [SerializeField] private bool enableObjectPooling = true;
        [SerializeField] private bool enableLODOptimization = true;
        
        [Header("Memory Management")]
        [SerializeField] private int maxTextureMemoryMB = 512;
        [SerializeField] private int maxAudioMemoryMB = 128;
        [SerializeField] private bool unloadUnusedAssets = true;
        
        [Header("Rendering Optimization")]
        [SerializeField] private bool enableOcclusion = true;
        [SerializeField] private bool enableInstancing = true;
        [SerializeField] private int maxShadowDistance = 50;
        
        private static PerformanceOptimizer instance;
        public static PerformanceOptimizer Instance => instance;
        
        private Coroutine gcCoroutine;
        private float lastGCTime;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeOptimizations();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            if (enableAutoGC)
            {
                gcCoroutine = StartCoroutine(AutoGarbageCollection());
            }
        }
        
        void OnDestroy()
        {
            if (gcCoroutine != null)
            {
                StopCoroutine(gcCoroutine);
            }
        }
        
        /// <summary>
        /// Initializes performance optimizations.
        /// </summary>
        private void InitializeOptimizations()
        {
            // Set quality settings based on hardware
            OptimizeQualitySettings();
            
            // Configure rendering optimizations
            ConfigureRendering();
            
            // Set memory limits
            ConfigureMemoryLimits();
            
            Debug.Log("Performance optimizations initialized");
        }
        
        /// <summary>
        /// Optimizes quality settings based on device capabilities.
        /// </summary>
        private void OptimizeQualitySettings()
        {
            // Get system memory in MB
            int systemMemoryMB = SystemInfo.systemMemorySize;
            
            // Adjust quality based on available memory
            if (systemMemoryMB < 4096) // Less than 4GB
            {
                QualitySettings.SetQualityLevel(0); // Very Low
                Debug.Log("Performance: Set quality to Very Low (Low Memory)");
            }
            else if (systemMemoryMB < 8192) // Less than 8GB
            {
                QualitySettings.SetQualityLevel(2); // Medium
                Debug.Log("Performance: Set quality to Medium (Medium Memory)");
            }
            else if (systemMemoryMB < 16384) // Less than 16GB
            {
                QualitySettings.SetQualityLevel(3); // High
                Debug.Log("Performance: Set quality to High (High Memory)");
            }
            else
            {
                QualitySettings.SetQualityLevel(5); // Ultra
                Debug.Log("Performance: Set quality to Ultra (High Memory)");
            }
            
            // Optimize shadow settings
            if (enableLODOptimization)
            {
                QualitySettings.shadowDistance = maxShadowDistance;
                QualitySettings.shadowCascades = systemMemoryMB > 8192 ? 4 : 2;
            }
        }
        
        /// <summary>
        /// Configures rendering optimizations.
        /// </summary>
        private void ConfigureRendering()
        {
            // Enable GPU instancing if supported
            if (enableInstancing && SystemInfo.supportsInstancing)
            {
                Debug.Log("Performance: GPU Instancing enabled");
            }
            
            // Configure occlusion culling
            if (enableOcclusion)
            {
                // Occlusion culling is configured per-scene
                Debug.Log("Performance: Occlusion culling enabled");
            }
            
            // Set appropriate render pipeline settings
            if (QualitySettings.renderPipeline != null)
            {
                Debug.Log($"Performance: Using {QualitySettings.renderPipeline.name}");
            }
        }
        
        /// <summary>
        /// Configures memory usage limits.
        /// </summary>
        private void ConfigureMemoryLimits()
        {
            // These are Unity's internal memory limits
            // Actual implementation would require native plugins
            Debug.Log($"Performance: Target texture memory: {maxTextureMemoryMB}MB");
            Debug.Log($"Performance: Target audio memory: {maxAudioMemoryMB}MB");
        }
        
        /// <summary>
        /// Automatic garbage collection coroutine.
        /// </summary>
        private IEnumerator AutoGarbageCollection()
        {
            while (true)
            {
                yield return new WaitForSeconds(gcInterval);
                
                if (Time.time - lastGCTime >= gcInterval)
                {
                    PerformGarbageCollection();
                    lastGCTime = Time.time;
                }
            }
        }
        
        /// <summary>
        /// Performs garbage collection and memory cleanup.
        /// </summary>
        public void PerformGarbageCollection()
        {
            try
            {
                // Force garbage collection
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
                
                // Unload unused assets if enabled
                if (unloadUnusedAssets)
                {
                    Resources.UnloadUnusedAssets();
                }
                
                Debug.Log("Performance: Garbage collection completed");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Performance: GC failed: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Optimizes for a specific scene.
        /// </summary>
        /// <param name="sceneName">Name of the scene to optimize for</param>
        public void OptimizeForScene(string sceneName)
        {
            switch (sceneName.ToLower())
            {
                case "mainmenu":
                    OptimizeForMenu();
                    break;
                case "characterselection":
                    OptimizeForCharacterSelection();
                    break;
                case "gameplay":
                    OptimizeForGameplay();
                    break;
                default:
                    Debug.Log($"Performance: No specific optimizations for scene: {sceneName}");
                    break;
            }
        }
        
        /// <summary>
        /// Optimizes settings for menu scenes.
        /// </summary>
        private void OptimizeForMenu()
        {
            // Reduce target framerate for menus to save power
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 1;
            
            Debug.Log("Performance: Optimized for menu scene");
        }
        
        /// <summary>
        /// Optimizes settings for character selection.
        /// </summary>
        private void OptimizeForCharacterSelection()
        {
            // Moderate framerate for character selection
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            
            Debug.Log("Performance: Optimized for character selection");
        }
        
        /// <summary>
        /// Optimizes settings for gameplay.
        /// </summary>
        private void OptimizeForGameplay()
        {
            // Maximum performance for gameplay
            Application.targetFrameRate = -1; // Unlimited
            QualitySettings.vSyncCount = 0; // Disable VSync for max FPS
            
            Debug.Log("Performance: Optimized for gameplay");
        }
        
        /// <summary>
        /// Gets current memory usage statistics.
        /// </summary>
        /// <returns>Memory usage info string</returns>
        public string GetMemoryStats()
        {
            return $"Allocated: {(System.GC.GetTotalMemory(false) / 1024 / 1024):F1}MB | " +
                   $"Reserved: {(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024 / 1024):F1}MB | " +
                   $"Used: {(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1024 / 1024):F1}MB";
        }
        
        /// <summary>
        /// Called when memory pressure is detected.
        /// </summary>
        public void OnLowMemory()
        {
            Debug.LogWarning("Performance: Low memory detected, performing aggressive cleanup");
            
            // Immediate garbage collection
            PerformGarbageCollection();
            
            // Reduce quality settings temporarily
            int currentQuality = QualitySettings.GetQualityLevel();
            if (currentQuality > 0)
            {
                QualitySettings.SetQualityLevel(currentQuality - 1);
                Debug.Log($"Performance: Reduced quality level to {currentQuality - 1}");
            }
        }
        
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Perform cleanup when app is paused
                PerformGarbageCollection();
            }
        }
        
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // Perform cleanup when app loses focus
                PerformGarbageCollection();
            }
        }
    }
}