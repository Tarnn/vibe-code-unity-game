using UnityEngine;

namespace FrostRealm.Data
{
    /// <summary>
    /// ScriptableObject for upgrade data following Warcraft III upgrade system.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "FrostRealm/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("Basic Information")]
        public string upgradeName = "Upgrade";
        public string description = "";
        public Sprite icon;
        public Race race = Race.Human;
        
        [Header("Upgrade Levels")]
        public int maxLevel = 3;
        public UpgradeLevel[] levels = new UpgradeLevel[3];
        
        [Header("Requirements")]
        public BuildingType requiredBuilding = BuildingType.None;
        public TechTier requiredTier = TechTier.Tier1;
        public UpgradeData[] prerequisiteUpgrades;
        
        [Header("Effects")]
        public UpgradeEffect effectType = UpgradeEffect.Attack;
        public UnitType[] affectedUnitTypes; // Which units benefit
        public bool affectsHeroes = false;
        
        [Header("Sound")]
        public AudioClip researchSound;
        public AudioClip completeSound;
    }
    
    [System.Serializable]
    public class UpgradeLevel
    {
        public int goldCost = 100;
        public int lumberCost = 50;
        public float researchTime = 60f;
        
        // Effect values
        public int attackBonus = 0;
        public int armorBonus = 0;
        public int hitPointBonus = 0;
        public int manaBonus = 0;
        public float moveSpeedBonus = 0f;
        public float attackSpeedBonus = 0f;
        
        // Special effects
        public AbilityData unlockedAbility = null;
        public string customEffect = "";
    }
    
    public enum UpgradeEffect
    {
        Attack,
        Armor,
        HitPoints,
        Mana,
        MoveSpeed,
        AttackSpeed,
        UnlockAbility,
        Custom
    }
    
    public enum BuildingType
    {
        None,
        // Human
        Barracks,
        BlackSmith,
        ArcaneSanctum,
        Workshop,
        GryphonAviary,
        // Orc
        WarMill,
        SpiritLodge,
        Beastiary,
        TaurenTotem,
        // Undead
        Graveyard,
        TempleOfTheDamned,
        Slaughterhouse,
        BoneYard,
        // Night Elf
        HuntersHall,
        TreeOfLife,
        ChimaeraRoost,
        AncientOfWar
    }
    
    public enum TechTier
    {
        Tier1,
        Tier2,
        Tier3
    }
    
    /// <summary>
    /// Example upgrades from Warcraft III
    /// </summary>
    public static class UpgradeExamples
    {
        // Human - Iron Forged Swords (3 levels)
        public static UpgradeLevel[] IronForgedSwords = new UpgradeLevel[]
        {
            new UpgradeLevel { goldCost = 100, lumberCost = 50, researchTime = 60f, attackBonus = 1 },
            new UpgradeLevel { goldCost = 150, lumberCost = 75, researchTime = 75f, attackBonus = 2 },
            new UpgradeLevel { goldCost = 200, lumberCost = 100, researchTime = 90f, attackBonus = 3 }
        };
        
        // Orc - Reinforced Armor (3 levels)
        public static UpgradeLevel[] ReinforcedArmor = new UpgradeLevel[]
        {
            new UpgradeLevel { goldCost = 150, lumberCost = 75, researchTime = 60f, armorBonus = 1 },
            new UpgradeLevel { goldCost = 225, lumberCost = 112, researchTime = 75f, armorBonus = 2 },
            new UpgradeLevel { goldCost = 300, lumberCost = 150, researchTime = 90f, armorBonus = 3 }
        };
    }
}