using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Comprehensive project validation tool for FrostRealm Chronicles.
    /// Validates all systems, configurations, and build readiness.
    /// </summary>
    public static class ProjectValidator
    {
        [MenuItem("FrostRealm/Validate/Full Project Validation")]
        public static void ValidateFullProject()
        {
            var results = new List<ValidationResult>();
            
            Debug.Log("=== FrostRealm Chronicles - Full Project Validation ===");
            
            // Run all validation checks
            ValidateProjectStructure(results);
            ValidateAssets(results);
            ValidateScripts(results);
            ValidateBuildSettings(results);
            ValidateInputSystem(results);
            ValidateAudioSystem(results);
            ValidateHeroSystem(results);
            ValidateUISystem(results);
            
            // Display results
            DisplayValidationResults(results);
            
            // Summary
            int passed = results.Count(r => r.passed);
            int total = results.Count;
            
            if (passed == total)
            {
                Debug.Log($"<color=green>✅ ALL VALIDATION CHECKS PASSED ({passed}/{total})</color>");
                Debug.Log("🎮 Project is ready for build and deployment!");
            }
            else
            {
                Debug.LogWarning($"<color=orange>⚠️ VALIDATION ISSUES FOUND ({total - passed}/{total} failures)</color>");
                Debug.LogWarning("Please address the issues above before building.");
            }
        }
        
        [MenuItem("FrostRealm/Validate/Quick Build Validation")]
        public static void ValidateBuildReadiness()
        {
            var results = new List<ValidationResult>();
            
            Debug.Log("=== Quick Build Validation ===");
            
            ValidateBuildSettings(results);
            ValidateScripts(results);
            ValidateHeroSystem(results);
            
            // Display results
            DisplayValidationResults(results);
            
            int passed = results.Count(r => r.passed);
            int total = results.Count;
            
            if (passed == total)
            {
                Debug.Log("<color=green>✅ Build validation passed - Ready to build!</color>");
            }
            else
            {
                Debug.LogError("<color=red>❌ Build validation failed - Fix issues before building</color>");
            }
        }
        
        private static void ValidateProjectStructure(List<ValidationResult> results)
        {
            Debug.Log("Validating project structure...");
            
            // Check key folders
            var requiredFolders = new[]
            {
                "assets/Scripts",
                "assets/Scripts/Core",
                "assets/Scripts/Data", 
                "assets/Scripts/UI",
                "assets/Scripts/Editor",
                "assets/Data",
                "assets/Data/Heroes",
                "assets/Resources",
                "assets/Scenes",
                "assets/Audio",
                "assets/UI"
            };
            
            foreach (var folder in requiredFolders)
            {
                bool exists = AssetDatabase.IsValidFolder(folder);
                results.Add(new ValidationResult
                {
                    category = "Project Structure",
                    test = $"Folder: {folder}",
                    passed = exists,
                    message = exists ? "Exists" : "Missing"
                });
            }
        }
        
        private static void ValidateAssets(List<ValidationResult> results)
        {
            Debug.Log("Validating assets...");
            
            // Check assembly definitions
            var asmdefGuids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");
            bool hasMainAsmdef = asmdefGuids.Any(guid => 
                AssetDatabase.GUIDToAssetPath(guid).Contains("FrostRealm.asmdef"));
            bool hasTestAsmdef = asmdefGuids.Any(guid => 
                AssetDatabase.GUIDToAssetPath(guid).Contains("FrostRealm.Tests.asmdef"));
            
            results.Add(new ValidationResult
            {
                category = "Assets",
                test = "Main Assembly Definition",
                passed = hasMainAsmdef,
                message = hasMainAsmdef ? "Found" : "Missing FrostRealm.asmdef"
            });
            
            results.Add(new ValidationResult
            {
                category = "Assets",
                test = "Test Assembly Definition", 
                passed = hasTestAsmdef,
                message = hasTestAsmdef ? "Found" : "Missing FrostRealm.Tests.asmdef"
            });
            
            // Check input actions
            var inputActionGuids = AssetDatabase.FindAssets("FrostRealmInputActions t:InputActionAsset");
            bool hasInputActions = inputActionGuids.Length > 0;
            
            results.Add(new ValidationResult
            {
                category = "Assets",
                test = "Input Action Asset",
                passed = hasInputActions,
                message = hasInputActions ? "Found" : "Missing FrostRealmInputActions"
            });
        }
        
        private static void ValidateScripts(List<ValidationResult> results)
        {
            Debug.Log("Validating scripts...");
            
            // Check core script files exist
            var coreScripts = new[]
            {
                "assets/Scripts/Core/GameManager.cs",
                "assets/Scripts/Core/ResourceManager.cs",
                "assets/Scripts/Core/AudioManager.cs",
                "assets/Scripts/Core/InputManager.cs",
                "assets/Scripts/Core/SelectionManager.cs",
                "assets/Scripts/Core/RTSCameraController.cs",
                "assets/Scripts/Core/HeroRegistry.cs",
                "assets/Scripts/Data/HeroData.cs"
            };
            
            foreach (var script in coreScripts)
            {
                bool exists = File.Exists(script);
                results.Add(new ValidationResult
                {
                    category = "Scripts",
                    test = Path.GetFileName(script),
                    passed = exists,
                    message = exists ? "Exists" : "Missing"
                });
            }
            
            // Check for compilation errors
            bool hasCompilationErrors = EditorUtility.scriptCompilationFailed;
            results.Add(new ValidationResult
            {
                category = "Scripts",
                test = "Compilation",
                passed = !hasCompilationErrors,
                message = hasCompilationErrors ? "Has compilation errors" : "Clean compilation"
            });
        }
        
        private static void ValidateBuildSettings(List<ValidationResult> results)
        {
            Debug.Log("Validating build settings...");
            
            var buildScenes = EditorBuildSettings.scenes;
            bool hasBuildScenes = buildScenes.Length > 0;
            
            results.Add(new ValidationResult
            {
                category = "Build Settings",
                test = "Scenes in Build",
                passed = hasBuildScenes,
                message = hasBuildScenes ? $"{buildScenes.Length} scenes" : "No scenes in build"
            });
            
            // Check specific scenes
            var requiredScenes = new[] { "MainMenu", "CharacterSelection", "Gameplay" };
            foreach (var sceneName in requiredScenes)
            {
                bool found = buildScenes.Any(scene => scene.path.Contains(sceneName));
                results.Add(new ValidationResult
                {
                    category = "Build Settings",
                    test = $"Scene: {sceneName}",
                    passed = found,
                    message = found ? "In build" : "Missing from build"
                });
            }
            
            // Check player settings
            bool hasCompanyName = !string.IsNullOrEmpty(PlayerSettings.companyName);
            bool hasProductName = !string.IsNullOrEmpty(PlayerSettings.productName);
            
            results.Add(new ValidationResult
            {
                category = "Build Settings",
                test = "Company Name",
                passed = hasCompanyName,
                message = hasCompanyName ? PlayerSettings.companyName : "Not set"
            });
            
            results.Add(new ValidationResult
            {
                category = "Build Settings",
                test = "Product Name",
                passed = hasProductName,
                message = hasProductName ? PlayerSettings.productName : "Not set"
            });
        }
        
        private static void ValidateInputSystem(List<ValidationResult> results)
        {
            Debug.Log("Validating input system...");
            
            // Check if Input System package is available
            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.InputSystem, Unity.InputSystem");
            bool hasInputSystem = inputSystemType != null;
            
            results.Add(new ValidationResult
            {
                category = "Input System",
                test = "Package Available",
                passed = hasInputSystem,
                message = hasInputSystem ? "Installed" : "Missing Unity Input System package"
            });
            
            if (hasInputSystem)
            {
                // Check InputManager script
                bool hasInputManager = File.Exists("assets/Scripts/Core/InputManager.cs");
                results.Add(new ValidationResult
                {
                    category = "Input System",
                    test = "InputManager Script",
                    passed = hasInputManager,
                    message = hasInputManager ? "Exists" : "Missing"
                });
            }
        }
        
        private static void ValidateAudioSystem(List<ValidationResult> results)
        {
            Debug.Log("Validating audio system...");
            
            // Check AudioManager script
            bool hasAudioManager = File.Exists("assets/Scripts/Core/AudioManager.cs");
            results.Add(new ValidationResult
            {
                category = "Audio System",
                test = "AudioManager Script",
                passed = hasAudioManager,
                message = hasAudioManager ? "Exists" : "Missing"
            });
            
            // Check audio folders
            var audioFolders = new[] { "assets/Audio/Music", "assets/Audio/SFX", "assets/Audio/Voice" };
            foreach (var folder in audioFolders)
            {
                bool exists = AssetDatabase.IsValidFolder(folder);
                results.Add(new ValidationResult
                {
                    category = "Audio System",
                    test = $"Folder: {Path.GetFileName(folder)}",
                    passed = exists,
                    message = exists ? "Exists" : "Missing"
                });
            }
        }
        
        private static void ValidateHeroSystem(List<ValidationResult> results)
        {
            Debug.Log("Validating hero system...");
            
            // Check HeroRegistry in Resources
            var heroRegistry = Resources.Load<FrostRealm.Core.HeroRegistry>("HeroRegistry");
            bool hasHeroRegistry = heroRegistry != null;
            
            results.Add(new ValidationResult
            {
                category = "Hero System",
                test = "HeroRegistry Asset",
                passed = hasHeroRegistry,
                message = hasHeroRegistry ? "Found in Resources" : "Missing from Resources"
            });
            
            if (hasHeroRegistry)
            {
                int heroCount = heroRegistry.HeroCount;
                bool hasHeroes = heroCount > 0;
                
                results.Add(new ValidationResult
                {
                    category = "Hero System",
                    test = "Hero Count",
                    passed = hasHeroes,
                    message = hasHeroes ? $"{heroCount} heroes" : "No heroes in registry"
                });
                
                if (hasHeroes)
                {
                    bool allValid = heroRegistry.ValidateAllHeroes();
                    results.Add(new ValidationResult
                    {
                        category = "Hero System",
                        test = "Hero Data Validation",
                        passed = allValid,
                        message = allValid ? "All heroes valid" : "Some heroes invalid"
                    });
                }
            }
            
            // Check for hero data assets
            var heroDataGuids = AssetDatabase.FindAssets("t:HeroData", new[] { "assets/Data/Heroes" });
            int heroDataCount = heroDataGuids.Length;
            
            results.Add(new ValidationResult
            {
                category = "Hero System",
                test = "Hero Data Assets",
                passed = heroDataCount > 0,
                message = heroDataCount > 0 ? $"{heroDataCount} hero assets" : "No hero data assets"
            });
        }
        
        private static void ValidateUISystem(List<ValidationResult> results)
        {
            Debug.Log("Validating UI system...");
            
            // Check UI assets
            var uixmlGuids = AssetDatabase.FindAssets("t:VisualTreeAsset", new[] { "assets/UI" });
            var ussGuids = AssetDatabase.FindAssets("t:StyleSheet", new[] { "assets/UI" });
            
            results.Add(new ValidationResult
            {
                category = "UI System",
                test = "UXML Files",
                passed = uixmlGuids.Length > 0,
                message = uixmlGuids.Length > 0 ? $"{uixmlGuids.Length} files" : "No UXML files"
            });
            
            results.Add(new ValidationResult
            {
                category = "UI System",
                test = "USS Files",
                passed = ussGuids.Length > 0,
                message = ussGuids.Length > 0 ? $"{ussGuids.Length} files" : "No USS files"
            });
            
            // Check UI scripts
            var uiScripts = new[]
            {
                "assets/Scripts/UI/HeroSelectionManager.cs",
                "assets/Scripts/UI/CharacterSelectionController.cs"
            };
            
            foreach (var script in uiScripts)
            {
                bool exists = File.Exists(script);
                results.Add(new ValidationResult
                {
                    category = "UI System",
                    test = Path.GetFileName(script),
                    passed = exists,
                    message = exists ? "Exists" : "Missing"
                });
            }
        }
        
        private static void DisplayValidationResults(List<ValidationResult> results)
        {
            var groupedResults = results.GroupBy(r => r.category);
            
            foreach (var group in groupedResults)
            {
                Debug.Log($"\n--- {group.Key} ---");
                
                foreach (var result in group)
                {
                    string icon = result.passed ? "✅" : "❌";
                    string color = result.passed ? "green" : "red";
                    Debug.Log($"<color={color}>{icon} {result.test}: {result.message}</color>");
                }
            }
        }
        
        private struct ValidationResult
        {
            public string category;
            public string test;
            public bool passed;
            public string message;
        }
    }
}