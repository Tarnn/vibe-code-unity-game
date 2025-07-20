using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.IO;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Build script for automated builds and development workflow.
    /// Provides methods for building the game from command line and editor.
    /// </summary>
    public class BuildScript
    {
        private static readonly string[] SCENES = {
            "assets/Scenes/MainMenu/MainMenu.unity",
            "assets/Scenes/CharacterSelection.unity",
            "assets/Scenes/Gameplay/Gameplay.unity",
        };
        
        private static readonly string BUILD_PATH = "Build/FrostRealmChronicles.exe";
        
        /// <summary>
        /// Builds the game for development (called from command line).
        /// </summary>
        public static void BuildGame()
        {
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = SCENES,
                locationPathName = BUILD_PATH,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            
            PerformBuild(buildOptions, "Development Build");
        }
        
        /// <summary>
        /// Builds the game for release.
        /// </summary>
        [MenuItem("FrostRealm/Build/Release Build")]
        public static void BuildRelease()
        {
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = SCENES,
                locationPathName = "Build/Release/FrostRealmChronicles.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            
            PerformBuild(buildOptions, "Release Build");
        }
        
        /// <summary>
        /// Builds the game for development with auto-run.
        /// </summary>
        [MenuItem("FrostRealm/Build/Development Build & Run")]
        public static void BuildAndRun()
        {
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = SCENES,
                locationPathName = BUILD_PATH,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AutoRunPlayer
            };
            
            PerformBuild(buildOptions, "Development Build & Run");
        }
        
        /// <summary>
        /// Quick build for iteration (minimal settings).
        /// </summary>
        [MenuItem("FrostRealm/Build/Quick Build")]
        public static void QuickBuild()
        {
            // Use only the character selection scene for quick testing
            string[] quickScenes = { "assets/Scenes/CharacterSelection.unity" };
            
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = quickScenes,
                locationPathName = "Build/Quick/FrostRealmQuick.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };
            
            PerformBuild(buildOptions, "Quick Build");
        }
        
        /// <summary>
        /// Performs the actual build with error handling and logging.
        /// </summary>
        private static void PerformBuild(BuildPlayerOptions buildOptions, string buildType)
        {
            Debug.Log($"Starting {buildType}...");
            Debug.Log($"Target: {buildOptions.target}");
            Debug.Log($"Path: {buildOptions.locationPathName}");
            Debug.Log($"Scenes: {string.Join(", ", buildOptions.scenes)}");
            
            // Ensure build directory exists
            string buildDir = Path.GetDirectoryName(buildOptions.locationPathName);
            if (!Directory.Exists(buildDir))
            {
                Directory.CreateDirectory(buildDir);
                Debug.Log($"Created build directory: {buildDir}");
            }
            
            // Perform pre-build validation
            if (!ValidateBuildSettings())
            {
                Debug.LogError("Build validation failed!");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
                return;
            }
            
            // Start build
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            stopwatch.Stop();
            
            // Handle build result
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"{buildType} succeeded!");
                Debug.Log($"Build time: {stopwatch.Elapsed:mm\\:ss}");
                Debug.Log($"Size: {FormatBytes(summary.totalSize)}");
                Debug.Log($"Output: {summary.outputPath}");
                
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }
            else
            {
                Debug.LogError($"{buildType} failed!");
                Debug.LogError($"Result: {summary.result}");
                Debug.LogError($"Total errors: {summary.totalErrors}");
                Debug.LogError($"Total warnings: {summary.totalWarnings}");
                
                // Log detailed errors
                foreach (BuildStep step in report.steps)
                {
                    foreach (BuildStepMessage message in step.messages)
                    {
                        if (message.type == LogType.Error || message.type == LogType.Exception)
                        {
                            Debug.LogError($"Build Error: {message.content}");
                        }
                        else if (message.type == LogType.Warning)
                        {
                            Debug.LogWarning($"Build Warning: {message.content}");
                        }
                    }
                }
                
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
            }
        }
        
        /// <summary>
        /// Validates build settings and project state.
        /// </summary>
        private static bool ValidateBuildSettings()
        {
            bool isValid = true;
            
            // Check if all scenes exist
            foreach (string scenePath in SCENES)
            {
                if (!File.Exists(scenePath))
                {
                    Debug.LogError($"Scene not found: {scenePath}");
                    isValid = false;
                }
            }
            
            // Check if HeroRegistry exists
            var heroRegistry = Resources.Load<FrostRealm.Core.HeroRegistry>("HeroRegistry");
            if (heroRegistry == null)
            {
                Debug.LogWarning("HeroRegistry not found in Resources folder. Creating placeholder...");
                // Could create a basic registry here if needed
            }
            
            // Validate player settings
            if (string.IsNullOrEmpty(PlayerSettings.companyName))
            {
                PlayerSettings.companyName = "FrostRealm Studios";
                Debug.Log("Set company name to: FrostRealm Studios");
            }
            
            if (string.IsNullOrEmpty(PlayerSettings.productName))
            {
                PlayerSettings.productName = "FrostRealm Chronicles";
                Debug.Log("Set product name to: FrostRealm Chronicles");
            }
            
            return isValid;
        }
        
        /// <summary>
        /// Formats byte count into human readable string.
        /// </summary>
        private static string FormatBytes(ulong bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = bytes;
            
            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }
            
            return $"{size:F2} {suffixes[suffixIndex]}";
        }
        
        /// <summary>
        /// Cleans the build directory.
        /// </summary>
        [MenuItem("FrostRealm/Build/Clean Build Directory")]
        public static void CleanBuildDirectory()
        {
            if (Directory.Exists("Build"))
            {
                Directory.Delete("Build", true);
                Debug.Log("Build directory cleaned");
            }
            
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// Opens the build directory in file explorer.
        /// </summary>
        [MenuItem("FrostRealm/Build/Open Build Directory")]
        public static void OpenBuildDirectory()
        {
            string buildPath = Path.GetFullPath("Build");
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            
            EditorUtility.RevealInFinder(buildPath);
        }
    }
}