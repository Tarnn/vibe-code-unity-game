using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FrostRealm.Data;

namespace FrostRealm.Core
{
    /// <summary>
    /// Central registry for managing all available heroes in the game.
    /// Provides access to hero data and manages hero selection state.
    /// </summary>
    [CreateAssetMenu(fileName = "Hero Registry", menuName = "FrostRealm/Hero Registry")]
    public class HeroRegistry : ScriptableObject
    {
        [Header("Available Heroes")]
        [SerializeField] private HeroData[] availableHeroes;
        
        [Header("Default Selection")]
        [SerializeField] private HeroData defaultHero;
        
        private static HeroRegistry instance;
        
        /// <summary>
        /// Singleton instance of the hero registry.
        /// </summary>
        public static HeroRegistry Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<HeroRegistry>("HeroRegistry");
                    if (instance == null)
                    {
                        Debug.LogError("HeroRegistry not found in Resources folder! Please create one.");
                    }
                }
                return instance;
            }
        }
        
        /// <summary>
        /// Gets all available heroes.
        /// </summary>
        public HeroData[] AvailableHeroes => availableHeroes;
        
        /// <summary>
        /// Gets the default hero for fallback selection.
        /// </summary>
        public HeroData DefaultHero => defaultHero;
        
        /// <summary>
        /// Gets the total number of available heroes.
        /// </summary>
        public int HeroCount => availableHeroes?.Length ?? 0;
        
        /// <summary>
        /// Gets a hero by index.
        /// </summary>
        /// <param name="index">The index of the hero</param>
        /// <returns>Hero data or null if index is invalid</returns>
        public HeroData GetHero(int index)
        {
            if (availableHeroes == null || index < 0 || index >= availableHeroes.Length)
            {
                Debug.LogWarning($"Invalid hero index: {index}. Returning default hero.");
                return defaultHero;
            }
            
            return availableHeroes[index];
        }
        
        /// <summary>
        /// Gets a hero by name.
        /// </summary>
        /// <param name="heroName">The name of the hero</param>
        /// <returns>Hero data or null if not found</returns>
        public HeroData GetHeroByName(string heroName)
        {
            if (availableHeroes == null || string.IsNullOrEmpty(heroName))
                return null;
                
            return availableHeroes.FirstOrDefault(hero => 
                hero != null && hero.HeroName.Equals(heroName, System.StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Gets heroes by faction.
        /// </summary>
        /// <param name="faction">The faction to filter by</param>
        /// <returns>Array of heroes belonging to the faction</returns>
        public HeroData[] GetHeroesByFaction(Faction faction)
        {
            if (availableHeroes == null)
                return new HeroData[0];
                
            return availableHeroes.Where(hero => hero != null && hero.Faction == faction).ToArray();
        }
        
        /// <summary>
        /// Gets heroes by class.
        /// </summary>
        /// <param name="heroClass">The hero class to filter by</param>
        /// <returns>Array of heroes of the specified class</returns>
        public HeroData[] GetHeroesByClass(HeroClass heroClass)
        {
            if (availableHeroes == null)
                return new HeroData[0];
                
            return availableHeroes.Where(hero => hero != null && hero.HeroClass == heroClass).ToArray();
        }
        
        /// <summary>
        /// Validates all hero data in the registry.
        /// </summary>
        /// <returns>True if all heroes are valid, false otherwise</returns>
        public bool ValidateAllHeroes()
        {
            if (availableHeroes == null || availableHeroes.Length == 0)
            {
                Debug.LogError("Hero registry is empty!");
                return false;
            }
            
            bool allValid = true;
            
            for (int i = 0; i < availableHeroes.Length; i++)
            {
                var hero = availableHeroes[i];
                if (hero == null)
                {
                    Debug.LogError($"Hero at index {i} is null!");
                    allValid = false;
                    continue;
                }
                
                if (!hero.IsValid())
                {
                    Debug.LogError($"Hero '{hero.HeroName}' at index {i} is invalid!");
                    allValid = false;
                }
            }
            
            return allValid;
        }
        
        /// <summary>
        /// Gets a random hero from the registry.
        /// </summary>
        /// <returns>Random hero data</returns>
        public HeroData GetRandomHero()
        {
            if (availableHeroes == null || availableHeroes.Length == 0)
                return defaultHero;
                
            int randomIndex = Random.Range(0, availableHeroes.Length);
            return availableHeroes[randomIndex];
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method to refresh the hero list from project assets.
        /// </summary>
        [ContextMenu("Refresh Hero List")]
        private void RefreshHeroList()
        {
            var heroGuids = UnityEditor.AssetDatabase.FindAssets("t:HeroData");
            var heroList = new List<HeroData>();
            
            foreach (string guid in heroGuids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var hero = UnityEditor.AssetDatabase.LoadAssetAtPath<HeroData>(path);
                if (hero != null)
                {
                    heroList.Add(hero);
                }
            }
            
            availableHeroes = heroList.ToArray();
            UnityEditor.EditorUtility.SetDirty(this);
            
            Debug.Log($"Refreshed hero list. Found {availableHeroes.Length} heroes.");
        }
        
        /// <summary>
        /// Editor-only validation method.
        /// </summary>
        [ContextMenu("Validate All Heroes")]
        private void ValidateAllHeroesEditor()
        {
            bool isValid = ValidateAllHeroes();
            Debug.Log(isValid ? "All heroes are valid!" : "Some heroes have validation errors. Check console for details.");
        }
        #endif
    }
}