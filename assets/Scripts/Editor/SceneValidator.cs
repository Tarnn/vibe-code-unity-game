using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using FrostRealm.Core;
using FrostRealm.UI;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Scene validation tool for FrostRealm Chronicles.
    /// Validates that all scenes have the required components and are properly configured.
    /// </summary>
    public class SceneValidator : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool showPassedChecks = false;
        private List<ValidationResult> lastValidationResults = new List<ValidationResult>();
        
        [MenuItem("FrostRealm/Validate Scenes")]
        public static void ShowWindow()
        {
            GetWindow<SceneValidator>("Scene Validator");
        }
        
        void OnGUI()
        {
            EditorGUILayout.LabelField("FrostRealm Chronicles - Scene Validator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            showPassedChecks = EditorGUILayout.Toggle("Show Passed Checks", showPassedChecks);
            
            if (GUILayout.Button("Validate All Scenes", GUILayout.Height(40)))
            {
                ValidateAllScenes();
            }
            
            EditorGUILayout.Space();
            
            if (lastValidationResults.Count > 0)
            {
                DisplayValidationResults();
            }
        }
        
        private void ValidateAllScenes()
        {
            lastValidationResults.Clear();
            
            // Get all scenes from build settings
            var buildScenes = EditorBuildSettings.scenes;
            
            if (buildScenes.Length == 0)
            {
                lastValidationResults.Add(new ValidationResult
                {
                    sceneName = "Build Settings",
                    category = "Configuration",
                    checkName = "Build Scenes",
                    passed = false,
                    message = "No scenes found in build settings!"
                });
                return;
            }
            
            foreach (var buildScene in buildScenes)
            {
                if (buildScene.enabled)
                {
                    ValidateScene(buildScene.path);
                }
            }
            
            // Validate project-wide settings
            ValidateProjectSettings();
            
            // Display summary
            int passedCount = lastValidationResults.Count(r => r.passed);
            int totalCount = lastValidationResults.Count;
            
            Debug.Log($"Scene Validation Complete: {passedCount}/{totalCount} checks passed");
            
            if (passedCount == totalCount)
            {
                Debug.Log("✅ All scenes are properly configured!");
            }
            else
            {
                Debug.LogWarning($"⚠️ {totalCount - passedCount} validation issues found. Check Scene Validator window for details.");
            }
        }
        
        private void ValidateScene(string scenePath)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            // Save current scene if dirty
            bool savedCurrentScene = false;
            if (EditorSceneManager.GetActiveScene().isDirty)
            {
                if (EditorUtility.DisplayDialog("Save Scene", "Current scene has unsaved changes. Save before validation?", "Save", "Don't Save"))
                {
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
                savedCurrentScene = true;
            }
            
            // Load the scene for validation
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            if (sceneName.Contains("MainMenu"))
            {
                ValidateMainMenuScene(sceneName);
            }
            else if (sceneName.Contains("CharacterSelection"))
            {
                ValidateCharacterSelectionScene(sceneName);
            }
            else if (sceneName.Contains("Gameplay"))
            {
                ValidateGameplayScene(sceneName);
            }
            
            // Common validations for all scenes
            ValidateCommonSceneElements(sceneName);
        }
        
        private void ValidateMainMenuScene(string sceneName)
        {
            // Check for GameManager
            CheckForComponent<GameManager>(sceneName, "Core", "GameManager", true);
            
            // Check for AudioManager
            CheckForComponent<AudioManager>(sceneName, "Core", "AudioManager", true);
            
            // Check for main camera
            Camera mainCamera = Camera.main;
            AddValidationResult(sceneName, "Camera", "Main Camera", mainCamera != null, 
                mainCamera != null ? "Main camera found" : "Main camera missing");
            
            // Check for UI Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            AddValidationResult(sceneName, "UI", "Canvas", canvas != null,
                canvas != null ? "UI Canvas found" : "UI Canvas missing");
            
            // Check for Event System
            UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            AddValidationResult(sceneName, "UI", "Event System", eventSystem != null,
                eventSystem != null ? "Event System found" : "Event System missing");
        }
        
        private void ValidateCharacterSelectionScene(string sceneName)
        {
            // Check for GameManager
            CheckForComponent<GameManager>(sceneName, "Core", "GameManager", true);
            
            // Check for AudioManager
            CheckForComponent<AudioManager>(sceneName, "Core", "AudioManager", true);
            
            // Check for HeroSelectionManager
            CheckForComponent<HeroSelectionManager>(sceneName, "UI", "HeroSelectionManager", true);
            
            // Check for CharacterSelectionController
            CheckForComponent<CharacterSelectionController>(sceneName, "UI", "CharacterSelectionController", false);
            
            // Check for main camera
            Camera mainCamera = Camera.main;
            AddValidationResult(sceneName, "Camera", "Main Camera", mainCamera != null,
                mainCamera != null ? "Main camera found" : "Main camera missing");
            
            // Check for UI Document
            UnityEngine.UIElements.UIDocument uiDocument = FindObjectOfType<UnityEngine.UIElements.UIDocument>();
            AddValidationResult(sceneName, "UI", "UI Document", uiDocument != null,
                uiDocument != null ? "UI Document found" : "UI Document missing - Character selection UI won't work");
        }
        
        private void ValidateGameplayScene(string sceneName)
        {
            // Check for GameManager
            CheckForComponent<GameManager>(sceneName, "Core", "GameManager", true);
            
            // Check for AudioManager
            CheckForComponent<AudioManager>(sceneName, "Core", "AudioManager", true);
            
            // Check for ResourceManager
            CheckForComponent<ResourceManager>(sceneName, "Core", "ResourceManager", true);
            
            // Check for SelectionManager
            CheckForComponent<SelectionManager>(sceneName, "Core", "SelectionManager", true);
            
            // Check for RTS Camera
            RTSCameraController rtsCamera = FindObjectOfType<RTSCameraController>();
            AddValidationResult(sceneName, "Camera", "RTS Camera Controller", rtsCamera != null,
                rtsCamera != null ? "RTS Camera Controller found" : "RTS Camera Controller missing - RTS gameplay won't work properly");
            
            // Check for main camera
            Camera mainCamera = Camera.main;
            AddValidationResult(sceneName, "Camera", "Main Camera", mainCamera != null,
                mainCamera != null ? "Main camera found" : "Main camera missing");
            
            if (mainCamera != null && rtsCamera == null)
            {
                AddValidationResult(sceneName, "Camera", "Camera Setup", false,
                    "Main camera exists but doesn't have RTSCameraController component");
            }
        }
        
        private void ValidateCommonSceneElements(string sceneName)
        {
            // Check for proper lighting setup
            Light[] lights = FindObjectsOfType<Light>();
            AddValidationResult(sceneName, "Lighting", "Scene Lighting", lights.Length > 0,
                lights.Length > 0 ? $"{lights.Length} light(s) found" : "No lights in scene - may appear dark");
            
            // Check for proper audio listener
            AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
            AddValidationResult(sceneName, "Audio", "Audio Listener", audioListeners.Length == 1,
                audioListeners.Length == 1 ? "Audio Listener found" : 
                audioListeners.Length == 0 ? "No Audio Listener found" : "Multiple Audio Listeners found (can cause issues)");
        }
        
        private void ValidateProjectSettings()
        {
            // Check Input System
            AddValidationResult("Project", "Input", "Input System Package", 
                System.Type.GetType("UnityEngine.InputSystem.InputSystem, Unity.InputSystem") != null,
                "Input System package verification");
            
            // Check HDRP
            AddValidationResult("Project", "Rendering", "HDRP Package",
                UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null,
                "HDRP render pipeline check");
            
            // Check Hero Registry
            HeroRegistry heroRegistry = Resources.Load<HeroRegistry>("HeroRegistry");
            AddValidationResult("Project", "Data", "Hero Registry", heroRegistry != null,
                heroRegistry != null ? $"Hero Registry found with {heroRegistry.HeroCount} heroes" : "Hero Registry missing from Resources folder");
            
            if (heroRegistry != null)
            {
                bool heroesValid = heroRegistry.ValidateAllHeroes();
                AddValidationResult("Project", "Data", "Hero Data Validation", heroesValid,
                    heroesValid ? "All hero data is valid" : "Some hero data validation failed");
            }
        }
        
        private void CheckForComponent<T>(string sceneName, string category, string componentName, bool required) where T : Component
        {
            T component = FindObjectOfType<T>();
            bool found = component != null;
            string message = found ? $"{componentName} found" : $"{componentName} missing";
            
            if (!found && required)
            {
                message += " (REQUIRED)";
            }
            
            AddValidationResult(sceneName, category, componentName, found || !required, message);
        }
        
        private void AddValidationResult(string sceneName, string category, string checkName, bool passed, string message)
        {
            lastValidationResults.Add(new ValidationResult
            {
                sceneName = sceneName,
                category = category,
                checkName = checkName,
                passed = passed,
                message = message
            });
        }
        
        private void DisplayValidationResults()
        {
            EditorGUILayout.LabelField("Validation Results:", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            var groupedResults = lastValidationResults.GroupBy(r => r.sceneName);
            
            foreach (var sceneGroup in groupedResults)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"Scene: {sceneGroup.Key}", EditorStyles.largeLabel);
                
                var categoryGroups = sceneGroup.GroupBy(r => r.category);
                
                foreach (var categoryGroup in categoryGroups)
                {
                    EditorGUILayout.LabelField($"  {categoryGroup.Key}:", EditorStyles.boldLabel);
                    
                    foreach (var result in categoryGroup)
                    {
                        if (!showPassedChecks && result.passed)
                            continue;
                        
                        GUIStyle style = result.passed ? EditorStyles.label : EditorStyles.helpBox;
                        Color originalColor = GUI.color;
                        
                        if (!result.passed)
                        {
                            GUI.color = Color.red;
                        }
                        else if (showPassedChecks)
                        {
                            GUI.color = Color.green;
                        }
                        
                        string statusIcon = result.passed ? "✅" : "❌";
                        EditorGUILayout.LabelField($"    {statusIcon} {result.checkName}: {result.message}", style);
                        
                        GUI.color = originalColor;
                    }
                }
            }
            
            EditorGUILayout.EndScrollView();
            
            // Summary
            EditorGUILayout.Space();
            int passedCount = lastValidationResults.Count(r => r.passed);
            int totalCount = lastValidationResults.Count;
            
            GUIStyle summaryStyle = passedCount == totalCount ? EditorStyles.helpBox : EditorStyles.helpBox;
            Color originalSummaryColor = GUI.color;
            GUI.color = passedCount == totalCount ? Color.green : Color.yellow;
            
            EditorGUILayout.LabelField($"Summary: {passedCount}/{totalCount} checks passed", summaryStyle);
            GUI.color = originalSummaryColor;
        }
        
        private struct ValidationResult
        {
            public string sceneName;
            public string category;
            public string checkName;
            public bool passed;
            public string message;
        }
    }
}