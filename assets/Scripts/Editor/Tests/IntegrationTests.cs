using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using FrostRealm.Core;
using FrostRealm.Data;

namespace FrostRealm.Tests
{
    /// <summary>
    /// Integration tests for FrostRealm Chronicles.
    /// Tests that all systems work together correctly and the build process functions.
    /// </summary>
    public class IntegrationTests
    {
        [Test]
        public void BuildSettings_AllScenesAreValid()
        {
            var buildScenes = EditorBuildSettings.scenes;
            Assert.Greater(buildScenes.Length, 0, "Build settings should contain scenes");
            
            foreach (var scene in buildScenes)
            {
                if (scene.enabled)
                {
                    Assert.IsTrue(System.IO.File.Exists(scene.path), $"Scene file should exist: {scene.path}");
                    
                    // Validate GUID
                    var assetGUID = AssetDatabase.AssetPathToGUID(scene.path);
                    Assert.IsFalse(string.IsNullOrEmpty(assetGUID), $"Scene should have valid GUID: {scene.path}");
                }
            }
        }
        
        [Test]
        public void HeroRegistry_LoadsFromResources()
        {
            var heroRegistry = Resources.Load<HeroRegistry>("HeroRegistry");
            Assert.IsNotNull(heroRegistry, "HeroRegistry should be loadable from Resources");
            Assert.Greater(heroRegistry.HeroCount, 0, "HeroRegistry should contain heroes");
        }
        
        [Test]
        public void HeroRegistry_AllHeroAssetsExist()
        {
            var heroRegistry = Resources.Load<HeroRegistry>("HeroRegistry");
            Assume.That(heroRegistry, Is.Not.Null);
            
            for (int i = 0; i < heroRegistry.HeroCount; i++)
            {
                var hero = heroRegistry.GetHero(i);
                Assert.IsNotNull(hero, $"Hero at index {i} should not be null");
                Assert.IsFalse(string.IsNullOrEmpty(hero.HeroName), $"Hero at index {i} should have a name");
            }
        }
        
        [Test]
        public void InputActions_AssetExists()
        {
            var inputActionsAsset = AssetDatabase.FindAssets("FrostRealmInputActions t:InputActionAsset").FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(inputActionsAsset), "FrostRealmInputActions asset should exist");
            
            var path = AssetDatabase.GUIDToAssetPath(inputActionsAsset);
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(path);
            Assert.IsNotNull(asset, "InputActionAsset should be loadable");
        }
        
        [Test]
        public void AssemblyDefinitions_AreValid()
        {
            // Check main assembly
            var mainAssembly = AssetDatabase.FindAssets("FrostRealm t:AssemblyDefinitionAsset").FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(mainAssembly), "Main FrostRealm assembly definition should exist");
            
            // Check test assembly
            var testAssembly = AssetDatabase.FindAssets("FrostRealm.Tests t:AssemblyDefinitionAsset").FirstOrDefault();
            Assert.IsFalse(string.IsNullOrEmpty(testAssembly), "FrostRealm.Tests assembly definition should exist");
        }
        
        [Test]
        public void CoreScripts_HaveNoCompilationErrors()
        {
            // This test will fail if there are compilation errors in the project
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var frostRealmAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == "FrostRealm");
            
            Assert.IsNotNull(frostRealmAssembly, "FrostRealm assembly should be compiled successfully");
            
            // Check that key types can be loaded
            Assert.DoesNotThrow(() => frostRealmAssembly.GetType("FrostRealm.Core.GameManager"), 
                "GameManager type should be loadable");
            Assert.DoesNotThrow(() => frostRealmAssembly.GetType("FrostRealm.Core.ResourceManager"), 
                "ResourceManager type should be loadable");
            Assert.DoesNotThrow(() => frostRealmAssembly.GetType("FrostRealm.Data.HeroData"), 
                "HeroData type should be loadable");
        }
        
        [UnityTest]
        public IEnumerator GameManager_InitializesWithoutErrors()
        {
            // Create a test scene with GameManager
            var testScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            var gameManagerGO = new GameObject("GameManager");
            var gameManager = gameManagerGO.AddComponent<GameManager>();
            
            // Wait a frame for initialization
            yield return null;
            
            Assert.IsNotNull(GameManager.Instance, "GameManager.Instance should be accessible");
            Assert.AreEqual(gameManager, GameManager.Instance, "GameManager.Instance should point to created instance");
            
            // Cleanup
            Object.DestroyImmediate(gameManagerGO);
        }
        
        [UnityTest]
        public IEnumerator ResourceManager_InitializesWithoutErrors()
        {
            var testScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            var resourceManagerGO = new GameObject("ResourceManager");
            var resourceManager = resourceManagerGO.AddComponent<ResourceManager>();
            
            // Wait a frame for initialization
            yield return null;
            
            Assert.IsNotNull(ResourceManager.Instance, "ResourceManager.Instance should be accessible");
            Assert.GreaterOrEqual(resourceManager.Gold, 0, "Initial gold should be valid");
            Assert.GreaterOrEqual(resourceManager.Lumber, 0, "Initial lumber should be valid");
            Assert.GreaterOrEqual(resourceManager.Food, 0, "Initial food should be valid");
            
            // Cleanup
            Object.DestroyImmediate(resourceManagerGO);
        }
        
        [Test]
        public void AllScenes_HaveValidNames()
        {
            var buildScenes = EditorBuildSettings.scenes;
            var expectedScenes = new[] { "MainMenu", "CharacterSelection", "Gameplay" };
            
            foreach (var expectedScene in expectedScenes)
            {
                bool found = buildScenes.Any(scene => scene.path.Contains(expectedScene));
                Assert.IsTrue(found, $"Scene containing '{expectedScene}' should be in build settings");
            }
        }
        
        [Test]
        public void ProjectSettings_AreConfigured()
        {
            // Check that key project settings are properly configured
            Assert.IsNotNull(UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline, 
                "Render pipeline should be set (expecting HDRP)");
            
            // Check that required packages are available
            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.InputSystem, Unity.InputSystem");
            Assert.IsNotNull(inputSystemType, "Input System package should be available");
        }
        
        [Test]
        public void AudioManager_HasValidConfiguration()
        {
            // Check if audio folders exist
            var audioFolderPath = "Assets/Audio";
            Assert.IsTrue(AssetDatabase.IsValidFolder(audioFolderPath), "Audio folder should exist");
            
            var musicFolderPath = "Assets/Audio/Music";
            Assert.IsTrue(AssetDatabase.IsValidFolder(musicFolderPath), "Music folder should exist");
            
            var sfxFolderPath = "Assets/Audio/SFX";
            Assert.IsTrue(AssetDatabase.IsValidFolder(sfxFolderPath), "SFX folder should exist");
            
            var voiceFolderPath = "Assets/Audio/Voice";
            Assert.IsTrue(AssetDatabase.IsValidFolder(voiceFolderPath), "Voice folder should exist");
        }
        
        [Test]
        public void UIAssets_AreValid()
        {
            // Check if UI assets exist
            var uiFiles = AssetDatabase.FindAssets("t:VisualTreeAsset", new[] { "Assets/UI" });
            Assert.Greater(uiFiles.Length, 0, "Should have UI visual tree assets");
            
            var styleFiles = AssetDatabase.FindAssets("t:StyleSheet", new[] { "Assets/UI" });
            Assert.Greater(styleFiles.Length, 0, "Should have UI style sheet assets");
        }
    }
}