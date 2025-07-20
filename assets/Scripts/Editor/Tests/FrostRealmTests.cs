using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using FrostRealm.Core;
using FrostRealm.Data;

namespace FrostRealm.Tests
{
    /// <summary>
    /// Comprehensive test suite for FrostRealm Chronicles core systems.
    /// Tests critical functionality, edge cases, and error handling.
    /// </summary>
    public class FrostRealmTests
    {
        private GameObject testGameObject;
        
        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestObject");
        }
        
        [TearDown]
        public void TearDown()
        {
            if (testGameObject != null)
                Object.DestroyImmediate(testGameObject);
                
            // Clean up any singleton instances
            ClearSingletonInstances();
        }
        
        private void ClearSingletonInstances()
        {
            // Force clear singleton instances to prevent test interference
            var gameManagerField = typeof(GameManager).GetField("instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (gameManagerField != null) gameManagerField.SetValue(null, null);
            
            var resourceManagerField = typeof(ResourceManager).GetField("instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (resourceManagerField != null) resourceManagerField.SetValue(null, null);
            
            var selectionManagerField = typeof(SelectionManager).GetField("instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (selectionManagerField != null) selectionManagerField.SetValue(null, null);
            
            var audioManagerField = typeof(AudioManager).GetField("instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (audioManagerField != null) audioManagerField.SetValue(null, null);
        }
        
        #region GameManager Tests
        
        [Test]
        public void GameManager_Singleton_CreatesInstance()
        {
            var instance = GameManager.Instance;
            Assert.IsNotNull(instance, "GameManager.Instance should create an instance if none exists");
        }
        
        [Test]
        public void GameManager_Singleton_ReturnsSameInstance()
        {
            var instance1 = GameManager.Instance;
            var instance2 = GameManager.Instance;
            Assert.AreSame(instance1, instance2, "GameManager.Instance should return the same instance");
        }
        
        [Test]
        public void GameManager_SetGameState_NotifiesListeners()
        {
            var gameManager = testGameObject.AddComponent<GameManager>();
            bool eventFired = false;
            GameState receivedState = GameState.MainMenu;
            
            gameManager.OnGameStateChanged += (state) => {
                eventFired = true;
                receivedState = state;
            };
            
            gameManager.SetGameState(GameState.InGame);
            
            Assert.IsTrue(eventFired, "OnGameStateChanged event should fire");
            Assert.AreEqual(GameState.InGame, receivedState, "Event should pass correct state");
        }
        
        [Test]
        public void GameManager_SetGameState_IgnoresSameState()
        {
            var gameManager = testGameObject.AddComponent<GameManager>();
            int eventCount = 0;
            
            gameManager.OnGameStateChanged += (state) => eventCount++;
            
            gameManager.SetGameState(GameState.MainMenu); // Default state
            gameManager.SetGameState(GameState.MainMenu); // Same state again
            
            Assert.AreEqual(0, eventCount, "Event should not fire when setting same state");
        }
        
        [Test]
        public void GameManager_SelectHero_HandlesNullHero()
        {
            var gameManager = testGameObject.AddComponent<GameManager>();
            
            Assert.DoesNotThrow(() => gameManager.SelectHero(null), 
                "SelectHero should handle null hero gracefully");
        }
        
        #endregion
        
        #region ResourceManager Tests
        
        [Test]
        public void ResourceManager_Singleton_CreatesInstance()
        {
            var instance = ResourceManager.Instance;
            Assert.IsNotNull(instance, "ResourceManager.Instance should create an instance if none exists");
        }
        
        [Test]
        public void ResourceManager_InitialResources_AreValid()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            
            Assert.GreaterOrEqual(resourceManager.Gold, 0, "Initial gold should be non-negative");
            Assert.GreaterOrEqual(resourceManager.Lumber, 0, "Initial lumber should be non-negative");
            Assert.GreaterOrEqual(resourceManager.Food, 0, "Initial food should be non-negative");
            Assert.AreEqual(0, resourceManager.UsedFood, "Initial used food should be 0");
        }
        
        [Test]
        public void ResourceManager_AddGold_WorksCorrectly()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            int initialGold = resourceManager.Gold;
            
            bool result = resourceManager.AddGold(100);
            
            Assert.IsTrue(result, "AddGold should return true for valid amount");
            Assert.AreEqual(initialGold + 100, resourceManager.Gold, "Gold should increase by added amount");
        }
        
        [Test]
        public void ResourceManager_AddGold_RejectsNegativeAmount()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            int initialGold = resourceManager.Gold;
            
            bool result = resourceManager.AddGold(-50);
            
            Assert.IsFalse(result, "AddGold should return false for negative amount");
            Assert.AreEqual(initialGold, resourceManager.Gold, "Gold should not change for negative amount");
        }
        
        [Test]
        public void ResourceManager_SpendGold_WorksCorrectly()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            resourceManager.AddGold(1000); // Ensure we have enough gold
            int goldBeforeSpending = resourceManager.Gold;
            
            bool result = resourceManager.SpendGold(100);
            
            Assert.IsTrue(result, "SpendGold should return true when sufficient gold available");
            Assert.AreEqual(goldBeforeSpending - 100, resourceManager.Gold, "Gold should decrease by spent amount");
        }
        
        [Test]
        public void ResourceManager_SpendGold_FailsWhenInsufficient()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            int initialGold = resourceManager.Gold;
            
            bool result = resourceManager.SpendGold(initialGold + 1000);
            
            Assert.IsFalse(result, "SpendGold should return false when insufficient gold");
            Assert.AreEqual(initialGold, resourceManager.Gold, "Gold should not change when spending fails");
        }
        
        [Test]
        public void ResourceManager_UpkeepSystem_WorksCorrectly()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            
            // Test no upkeep (0-50 population)
            resourceManager.UseFood(25);
            Assert.AreEqual(UpkeepLevel.None, resourceManager.CurrentUpkeep, "Should have no upkeep at low population");
            Assert.AreEqual(1.0f, resourceManager.UpkeepMultiplier, "Upkeep multiplier should be 1.0 at low population");
            
            // Test medium upkeep (51-80 population)
            resourceManager.UseFood(35); // Total 60
            Assert.AreEqual(UpkeepLevel.Medium, resourceManager.CurrentUpkeep, "Should have medium upkeep at mid population");
            Assert.AreEqual(0.7f, resourceManager.UpkeepMultiplier, "Upkeep multiplier should be 0.7 at mid population");
            
            // Test high upkeep (81+ population)
            resourceManager.UseFood(25); // Total 85
            Assert.AreEqual(UpkeepLevel.High, resourceManager.CurrentUpkeep, "Should have high upkeep at high population");
            Assert.AreEqual(0.4f, resourceManager.UpkeepMultiplier, "Upkeep multiplier should be 0.4 at high population");
        }
        
        [Test]
        public void ResourceManager_UseFood_FailsWhenExceedsCapacity()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            int maxFood = resourceManager.Food;
            
            bool result = resourceManager.UseFood(maxFood + 1);
            
            Assert.IsFalse(result, "UseFood should fail when exceeding capacity");
            Assert.AreEqual(0, resourceManager.UsedFood, "Used food should not change when use fails");
        }
        
        [Test]
        public void ResourceManager_CanAfford_WorksCorrectly()
        {
            var resourceManager = testGameObject.AddComponent<ResourceManager>();
            resourceManager.AddGold(500);
            resourceManager.AddLumber(200);
            
            Assert.IsTrue(resourceManager.CanAfford(100, 50), "Should be able to afford when resources sufficient");
            Assert.IsFalse(resourceManager.CanAfford(1000, 50), "Should not be able to afford when gold insufficient");
            Assert.IsFalse(resourceManager.CanAfford(100, 500), "Should not be able to afford when lumber insufficient");
        }
        
        #endregion
        
        #region HeroData Tests
        
        [Test]
        public void HeroData_IsValid_WorksCorrectly()
        {
            var heroData = ScriptableObject.CreateInstance<HeroData>();
            
            // Test invalid hero (empty data) - since HeroData fields are private and serialized,
            // a newly created ScriptableObject will have default values and be invalid
            Assert.IsFalse(heroData.IsValid(), "Empty hero data should be invalid");
            
            // Note: HeroData uses [SerializeField] private fields, so we can't set them in tests
            // This test validates that the IsValid() method correctly identifies invalid data
            
            Object.DestroyImmediate(heroData);
        }
        
        [Test]
        public void HeroData_PropertiesAccess_WorksCorrectly()
        {
            var heroData = ScriptableObject.CreateInstance<HeroData>();
            
            // Test that all properties can be accessed without errors
            Assert.DoesNotThrow(() => {
                var name = heroData.HeroName;
                var description = heroData.Description;
                var portrait = heroData.Portrait;
                var model = heroData.ModelPrefab;
                var heroClass = heroData.HeroClass;
                var faction = heroData.Faction;
                var heroType = heroData.HeroType;
                var stats = heroData.BaseStats;
                var abilities = heroData.Abilities;
                var voiceLines = heroData.VoiceLines;
            }, "All HeroData properties should be accessible");
            
            Object.DestroyImmediate(heroData);
        }
        
        #endregion
        
        #region HeroRegistry Tests
        
        [Test]
        public void HeroRegistry_ValidateAllHeroes_HandlesEmptyRegistry()
        {
            var heroRegistry = ScriptableObject.CreateInstance<HeroRegistry>();
            
            // Empty registry should be invalid
            Assert.IsFalse(heroRegistry.ValidateAllHeroes(), "Empty hero registry should be invalid");
            
            Object.DestroyImmediate(heroRegistry);
        }
        
        [Test]
        public void HeroRegistry_GetHero_HandlesInvalidIndex()
        {
            var heroRegistry = ScriptableObject.CreateInstance<HeroRegistry>();
            
            var result = heroRegistry.GetHero(-1);
            Assert.IsNull(result, "GetHero should return null for negative index");
            
            result = heroRegistry.GetHero(1000);
            Assert.IsNull(result, "GetHero should return null for out-of-bounds index");
            
            Object.DestroyImmediate(heroRegistry);
        }
        
        [Test]
        public void HeroRegistry_GetHeroByName_HandlesNullAndEmpty()
        {
            var heroRegistry = ScriptableObject.CreateInstance<HeroRegistry>();
            
            var result = heroRegistry.GetHeroByName(null);
            Assert.IsNull(result, "GetHeroByName should return null for null name");
            
            result = heroRegistry.GetHeroByName("");
            Assert.IsNull(result, "GetHeroByName should return null for empty name");
            
            Object.DestroyImmediate(heroRegistry);
        }
        
        #endregion
        
        #region RTSCameraController Tests
        
        [Test]
        public void RTSCameraController_RequiresCamera()
        {
            var controller = testGameObject.AddComponent<RTSCameraController>();
            
            // Controller should automatically add a camera if none exists
            var camera = testGameObject.GetComponent<Camera>();
            Assert.IsNotNull(camera, "RTSCameraController should require a Camera component");
        }
        
        [Test]
        public void RTSCameraController_SetPosition_WorksCorrectly()
        {
            var camera = testGameObject.AddComponent<Camera>();
            var controller = testGameObject.AddComponent<RTSCameraController>();
            
            Vector3 targetPosition = new Vector3(10, 5, 10);
            controller.SetPosition(targetPosition);
            
            Assert.AreEqual(targetPosition, controller.Position, "SetPosition should update camera position");
        }
        
        [Test]
        public void RTSCameraController_SetZoom_ClampsToLimits()
        {
            var camera = testGameObject.AddComponent<Camera>();
            var controller = testGameObject.AddComponent<RTSCameraController>();
            
            // Test setting zoom beyond limits
            controller.SetZoom(-10f); // Below minimum
            Assert.GreaterOrEqual(controller.CurrentZoom, 5f, "Zoom should be clamped to minimum");
            
            controller.SetZoom(1000f); // Above maximum
            Assert.LessOrEqual(controller.CurrentZoom, 25f, "Zoom should be clamped to maximum");
        }
        
        #endregion
        
        #region SelectionManager Tests
        
        [Test]
        public void SelectionManager_ClearSelection_WorksCorrectly()
        {
            var selectionManager = testGameObject.AddComponent<SelectionManager>();
            
            // Simulate having selected objects
            var testSelectable = new MockSelectable();
            var selectablesList = new List<ISelectable> { testSelectable };
            selectionManager.SelectMultiple(selectablesList);
            
            Assert.AreEqual(1, selectionManager.SelectionCount, "Should have one selected object");
            
            selectionManager.ClearSelection();
            
            Assert.AreEqual(0, selectionManager.SelectionCount, "Selection should be cleared");
            Assert.IsFalse(selectionManager.HasSelection, "Should not have selection after clearing");
        }
        
        [Test]
        public void SelectionManager_SelectMultiple_RespectsMaxSelectionLimit()
        {
            var selectionManager = testGameObject.AddComponent<SelectionManager>();
            var selectables = new List<ISelectable>();
            
            // Create more selectables than the limit (12)
            for (int i = 0; i < 20; i++)
            {
                selectables.Add(new MockSelectable());
            }
            
            selectionManager.SelectMultiple(selectables);
            
            Assert.LessOrEqual(selectionManager.SelectionCount, 12, "Selection should respect maximum limit");
        }
        
        [Test]
        public void SelectionManager_ToggleSelection_WorksCorrectly()
        {
            var selectionManager = testGameObject.AddComponent<SelectionManager>();
            var testSelectable = new MockSelectable();
            
            // First toggle should select
            selectionManager.ToggleSelection(testSelectable);
            Assert.IsTrue(selectionManager.IsSelected(testSelectable), "First toggle should select object");
            
            // Second toggle should deselect
            selectionManager.ToggleSelection(testSelectable);
            Assert.IsFalse(selectionManager.IsSelected(testSelectable), "Second toggle should deselect object");
        }
        
        #endregion
        
        #region Error Handling Tests
        
        [Test]
        public void Systems_HandleNullReferences_Gracefully()
        {
            // Test that systems don't crash with null references
            Assert.DoesNotThrow(() => {
                var gameManager = testGameObject.AddComponent<GameManager>();
                gameManager.SelectHero(null);
            }, "GameManager should handle null hero selection");
            
            Assert.DoesNotThrow(() => {
                var resourceManager = testGameObject.AddComponent<ResourceManager>();
                resourceManager.SpendGold(0);
                resourceManager.SpendLumber(0);
            }, "ResourceManager should handle zero spending");
        }
        
        [Test]
        public void Systems_HandleEdgeCases_Gracefully()
        {
            Assert.DoesNotThrow(() => {
                var resourceManager = testGameObject.AddComponent<ResourceManager>();
                resourceManager.AddGold(int.MaxValue);
                resourceManager.AddLumber(int.MaxValue);
            }, "ResourceManager should handle maximum values");
        }
        
        #endregion
        
        #region Mock Classes for Testing
        
        private class MockSelectable : ISelectable
        {
            public Vector3 Position => Vector3.zero;
            public float SelectionSize => 1f;
            public bool IsSelectable => true;
            private bool selected = false;
            
            public void OnSelected()
            {
                selected = true;
            }
            
            public void OnDeselected()
            {
                selected = false;
            }
            
            public bool IsSelected => selected;
        }
        
        #endregion
    }
}