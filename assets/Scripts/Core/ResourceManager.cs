using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;

namespace FrostRealm.Core
{
    /// <summary>
    /// Central resource management system for FrostRealm Chronicles.
    /// Handles gold, lumber, food/population with Warcraft III: The Frozen Throne mechanics.
    /// Includes upkeep system that reduces income at higher population levels.
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("Starting Resources")]
        [SerializeField] private int startingGold = 500;
        [SerializeField] private int startingLumber = 150;
        [SerializeField] private int maxFood = 100;
        
        [Header("Resource Limits")]
        [SerializeField] private int maxGold = 999999;
        [SerializeField] private int maxLumber = 999999;
        
        [Header("Upkeep System (Warcraft III style)")]
        [SerializeField] private int lowUpkeepThreshold = 50;   // 0-50: No upkeep penalty
        [SerializeField] private int highUpkeepThreshold = 80;  // 51-80: Medium upkeep penalty
        [SerializeField] private float mediumUpkeepMultiplier = 0.7f;  // 70% income
        [SerializeField] private float highUpkeepMultiplier = 0.4f;    // 40% income
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Current resource amounts
        private int currentGold;
        private int currentLumber;
        private int currentFood;
        private int usedFood;
        
        // Resource income tracking
        private float goldIncomeRate = 0f;
        private float lumberIncomeRate = 0f;
        private float lastIncomeUpdate;
        
        // Upkeep tracking
        private UpkeepLevel currentUpkeepLevel = UpkeepLevel.None;
        private float currentUpkeepMultiplier = 1.0f;
        
        // Events for UI updates
        public static event Action<int> OnGoldChanged;
        public static event Action<int> OnLumberChanged;
        public static event Action<int, int> OnFoodChanged; // used, max
        public static event Action<UpkeepLevel, float> OnUpkeepChanged;
        public static event Action<ResourceType, int> OnResourceGained;
        public static event Action<ResourceType, int> OnResourceSpent;
        public static event Action<ResourceType> OnResourceInsufficient;
        
        // Singleton pattern
        private static ResourceManager instance;
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ResourceManager>();
                    if (instance == null)
                    {
                        var go = new GameObject("Resource Manager");
                        instance = go.AddComponent<ResourceManager>();
                    }
                }
                return instance;
            }
        }
        
        // Properties
        public int Gold => currentGold;
        public int Lumber => currentLumber;
        public int Food => currentFood;
        public int UsedFood => usedFood;
        public int AvailableFood => currentFood - usedFood;
        public float GoldIncomeRate => goldIncomeRate * currentUpkeepMultiplier;
        public float LumberIncomeRate => lumberIncomeRate;
        public UpkeepLevel CurrentUpkeep => currentUpkeepLevel;
        public float UpkeepMultiplier => currentUpkeepMultiplier;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                if (transform.parent == null)
                    DontDestroyOnLoad(gameObject);
                InitializeResources();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        void Start()
        {
            lastIncomeUpdate = Time.time;
        }
        
        void Update()
        {
            UpdateResourceIncome();
            UpdateUpkeepLevel();
        }
        
        /// <summary>
        /// Initializes starting resource amounts.
        /// </summary>
        private void InitializeResources()
        {
            currentGold = startingGold;
            currentLumber = startingLumber;
            currentFood = maxFood;
            usedFood = 0;
            
            UpdateUpkeepLevel();
            
            // Notify initial values
            OnGoldChanged?.Invoke(currentGold);
            OnLumberChanged?.Invoke(currentLumber);
            OnFoodChanged?.Invoke(usedFood, currentFood);
            
            if (showDebugInfo)
            {
                Debug.Log($"Resources initialized - Gold: {currentGold}, Lumber: {currentLumber}, Food: {usedFood}/{currentFood}");
            }
        }
        
        /// <summary>
        /// Updates resource income over time.
        /// </summary>
        private void UpdateResourceIncome()
        {
            float currentTime = Time.time;
            float deltaTime = currentTime - lastIncomeUpdate;
            
            if (deltaTime >= 1.0f) // Update income every second
            {
                // Apply gold income with upkeep modifier
                if (goldIncomeRate > 0)
                {
                    int goldGain = Mathf.FloorToInt(goldIncomeRate * currentUpkeepMultiplier * deltaTime);
                    if (goldGain > 0)
                    {
                        AddGold(goldGain, false); // Don't trigger resource gained event for income
                    }
                }
                
                // Apply lumber income (not affected by upkeep)
                if (lumberIncomeRate > 0)
                {
                    int lumberGain = Mathf.FloorToInt(lumberIncomeRate * deltaTime);
                    if (lumberGain > 0)
                    {
                        AddLumber(lumberGain, false);
                    }
                }
                
                lastIncomeUpdate = currentTime;
            }
        }
        
        /// <summary>
        /// Updates the upkeep level based on current population.
        /// </summary>
        private void UpdateUpkeepLevel()
        {
            UpkeepLevel newUpkeepLevel;
            float newMultiplier;
            
            if (usedFood <= lowUpkeepThreshold)
            {
                newUpkeepLevel = UpkeepLevel.None;
                newMultiplier = 1.0f;
            }
            else if (usedFood <= highUpkeepThreshold)
            {
                newUpkeepLevel = UpkeepLevel.Medium;
                newMultiplier = mediumUpkeepMultiplier;
            }
            else
            {
                newUpkeepLevel = UpkeepLevel.High;
                newMultiplier = highUpkeepMultiplier;
            }
            
            if (newUpkeepLevel != currentUpkeepLevel || !Mathf.Approximately(newMultiplier, currentUpkeepMultiplier))
            {
                currentUpkeepLevel = newUpkeepLevel;
                currentUpkeepMultiplier = newMultiplier;
                OnUpkeepChanged?.Invoke(currentUpkeepLevel, currentUpkeepMultiplier);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Upkeep level changed to {currentUpkeepLevel} (multiplier: {currentUpkeepMultiplier:F1}x)");
                }
            }
        }
        
        /// <summary>
        /// Adds gold to the current amount.
        /// </summary>
        public bool AddGold(int amount, bool triggerEvent = true)
        {
            if (amount <= 0) return false;
            
            int oldGold = currentGold;
            currentGold = Mathf.Min(currentGold + amount, maxGold);
            
            if (currentGold != oldGold)
            {
                OnGoldChanged?.Invoke(currentGold);
                if (triggerEvent)
                {
                    OnResourceGained?.Invoke(ResourceType.Gold, amount);
                }
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Adds lumber to the current amount.
        /// </summary>
        public bool AddLumber(int amount, bool triggerEvent = true)
        {
            if (amount <= 0) return false;
            
            int oldLumber = currentLumber;
            currentLumber = Mathf.Min(currentLumber + amount, maxLumber);
            
            if (currentLumber != oldLumber)
            {
                OnLumberChanged?.Invoke(currentLumber);
                if (triggerEvent)
                {
                    OnResourceGained?.Invoke(ResourceType.Lumber, amount);
                }
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Spends gold if available.
        /// </summary>
        public bool SpendGold(int amount)
        {
            if (amount <= 0) return true;
            if (currentGold < amount)
            {
                OnResourceInsufficient?.Invoke(ResourceType.Gold);
                return false;
            }
            
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            OnResourceSpent?.Invoke(ResourceType.Gold, amount);
            return true;
        }
        
        /// <summary>
        /// Spends lumber if available.
        /// </summary>
        public bool SpendLumber(int amount)
        {
            if (amount <= 0) return true;
            if (currentLumber < amount)
            {
                OnResourceInsufficient?.Invoke(ResourceType.Lumber);
                return false;
            }
            
            currentLumber -= amount;
            OnLumberChanged?.Invoke(currentLumber);
            OnResourceSpent?.Invoke(ResourceType.Lumber, amount);
            return true;
        }
        
        /// <summary>
        /// Spends both gold and lumber in a single transaction.
        /// </summary>
        public bool SpendResources(int goldCost, int lumberCost)
        {
            // Check if we have enough resources
            if (currentGold < goldCost)
            {
                OnResourceInsufficient?.Invoke(ResourceType.Gold);
                return false;
            }
            
            if (currentLumber < lumberCost)
            {
                OnResourceInsufficient?.Invoke(ResourceType.Lumber);
                return false;
            }
            
            // Spend resources
            bool success = true;
            if (goldCost > 0)
                success &= SpendGold(goldCost);
            if (lumberCost > 0)
                success &= SpendLumber(lumberCost);
                
            return success;
        }
        
        /// <summary>
        /// Checks if we have enough resources for a cost.
        /// </summary>
        public bool CanAfford(int goldCost, int lumberCost = 0)
        {
            return currentGold >= goldCost && currentLumber >= lumberCost;
        }
        
        /// <summary>
        /// Uses food (increases population).
        /// </summary>
        public bool UseFood(int amount)
        {
            if (amount <= 0) return true;
            if (usedFood + amount > currentFood)
                return false;
                
            usedFood += amount;
            OnFoodChanged?.Invoke(usedFood, currentFood);
            return true;
        }
        
        /// <summary>
        /// Frees food (decreases population).
        /// </summary>
        public bool FreeFood(int amount)
        {
            if (amount <= 0) return true;
            
            usedFood = Mathf.Max(0, usedFood - amount);
            OnFoodChanged?.Invoke(usedFood, currentFood);
            return true;
        }
        
        /// <summary>
        /// Increases the maximum food capacity.
        /// </summary>
        public void IncreaseMaxFood(int amount)
        {
            if (amount <= 0) return;
            
            currentFood += amount;
            OnFoodChanged?.Invoke(usedFood, currentFood);
            
            if (showDebugInfo)
            {
                Debug.Log($"Max food increased by {amount}. New capacity: {currentFood}");
            }
        }
        
        /// <summary>
        /// Decreases the maximum food capacity.
        /// </summary>
        public void DecreaseMaxFood(int amount)
        {
            if (amount <= 0) return;
            
            currentFood = Mathf.Max(usedFood, currentFood - amount);
            OnFoodChanged?.Invoke(usedFood, currentFood);
            
            if (showDebugInfo)
            {
                Debug.Log($"Max food decreased by {amount}. New capacity: {currentFood}");
            }
        }
        
        /// <summary>
        /// Sets the gold income rate (gold per second).
        /// </summary>
        public void SetGoldIncomeRate(float ratePerSecond)
        {
            if (float.IsNaN(ratePerSecond) || float.IsInfinity(ratePerSecond))
            {
                Debug.LogError($"Invalid gold income rate: {ratePerSecond}. Setting to 0.");
                goldIncomeRate = 0f;
                return;
            }
            
            goldIncomeRate = Mathf.Max(0, ratePerSecond);
            
            if (showDebugInfo)
            {
                Debug.Log($"Gold income rate set to {goldIncomeRate}/sec (effective: {GoldIncomeRate}/sec with upkeep)");
            }
        }
        
        /// <summary>
        /// Sets the lumber income rate (lumber per second).
        /// </summary>
        public void SetLumberIncomeRate(float ratePerSecond)
        {
            if (float.IsNaN(ratePerSecond) || float.IsInfinity(ratePerSecond))
            {
                Debug.LogError($"Invalid lumber income rate: {ratePerSecond}. Setting to 0.");
                lumberIncomeRate = 0f;
                return;
            }
            
            lumberIncomeRate = Mathf.Max(0, ratePerSecond);
            
            if (showDebugInfo)
            {
                Debug.Log($"Lumber income rate set to {lumberIncomeRate}/sec");
            }
        }
        
        /// <summary>
        /// Modifies the gold income rate by a certain amount.
        /// </summary>
        public void ModifyGoldIncomeRate(float deltaRate)
        {
            SetGoldIncomeRate(goldIncomeRate + deltaRate);
        }
        
        /// <summary>
        /// Modifies the lumber income rate by a certain amount.
        /// </summary>
        public void ModifyLumberIncomeRate(float deltaRate)
        {
            SetLumberIncomeRate(lumberIncomeRate + deltaRate);
        }
        
        /// <summary>
        /// Gets current resource information for debugging.
        /// </summary>
        public string GetResourceDebugInfo()
        {
            return $"Gold: {currentGold} (+{GoldIncomeRate:F1}/s), " +
                   $"Lumber: {currentLumber} (+{lumberIncomeRate:F1}/s), " +
                   $"Food: {usedFood}/{currentFood}, " +
                   $"Upkeep: {currentUpkeepLevel} ({currentUpkeepMultiplier:F1}x)";
        }
        
        /// <summary>
        /// Resets all resources to starting values.
        /// </summary>
        [ContextMenu("Reset Resources")]
        public void ResetResources()
        {
            InitializeResources();
            goldIncomeRate = 0f;
            lumberIncomeRate = 0f;
            
            if (showDebugInfo)
            {
                Debug.Log("Resources reset to starting values");
            }
        }
        
        void OnGUI()
        {
            if (showDebugInfo && Application.isPlaying)
            {
                GUILayout.BeginArea(new Rect(10, 10, 400, 120));
                GUILayout.Label("=== Resource Manager Debug ===");
                GUILayout.Label(GetResourceDebugInfo());
                
                if (GUILayout.Button("Add 100 Gold"))
                    AddGold(100);
                if (GUILayout.Button("Add 50 Lumber"))
                    AddLumber(50);
                if (GUILayout.Button("Reset Resources"))
                    ResetResources();
                    
                GUILayout.EndArea();
            }
        }
    }
    
    /// <summary>
    /// Defines the types of resources in the game.
    /// </summary>
    public enum ResourceType
    {
        Gold,
        Lumber,
        Food
    }
    
    /// <summary>
    /// Defines the upkeep levels that affect income.
    /// </summary>
    public enum UpkeepLevel
    {
        None,    // 0-50 population: No penalty
        Medium,  // 51-80 population: 70% income
        High     // 81+ population: 40% income
    }
}