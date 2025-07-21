using UnityEngine;
using System.Collections.Generic;

namespace FrostRealm.Data
{
    /// <summary>
    /// ScriptableObject for unit data following Warcraft III unit stats.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUnit", menuName = "FrostRealm/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Basic Information")]
        public string unitName = "Unit";
        public string description = "";
        public Race race = Race.Human;
        public UnitType unitType = UnitType.Melee;
        
        [Header("Model and Animation")]
        public GameObject modelPrefab;
        public RuntimeAnimatorController animatorController;
        public float modelScale = 1f;
        
        [Header("Stats")]
        [Range(50, 5000)] public int hitPoints = 100;
        [Range(0, 500)] public int manaPoints = 0;
        [Range(1, 100)] public int damage = 10;
        [Range(1, 50)] public int damageRandomBonus = 2; // Damage will be damage to damage+bonus
        public DamageType damageType = DamageType.Normal;
        [Range(0, 50)] public int armor = 0;
        public ArmorType armorType = ArmorType.Medium;
        
        [Header("Movement")]
        [Range(0, 522)] public float moveSpeed = 270f; // Warcraft III units range from 0-522
        public MovementType movementType = MovementType.Ground;
        [Range(0.5f, 3f)] public float turnRate = 1.5f;
        
        [Header("Combat")]
        [Range(0.1f, 3f)] public float attackSpeed = 1.5f; // Time between attacks
        [Range(50, 700)] public float attackRange = 128f; // Melee ~128, Ranged 400-700
        public float attackPoint = 0.3f; // Animation time before damage
        public float attackBackswing = 0.5f; // Animation time after damage
        
        [Header("Vision")]
        [Range(200, 1800)] public float sightRangeDay = 800f;
        [Range(200, 1200)] public float sightRangeNight = 600f;
        public bool hasDetection = false;
        [Range(0, 900)] public float detectionRange = 0f;
        
        [Header("Cost and Build")]
        public int goldCost = 100;
        public int lumberCost = 0;
        public int foodCost = 1;
        public float buildTime = 30f;
        public UnitData[] requirements; // Required buildings/upgrades
        
        [Header("Abilities")]
        public List<AbilityData> abilities = new List<AbilityData>();
        
        [Header("Upgrades")]
        public List<UpgradeData> availableUpgrades = new List<UpgradeData>();
        
        [Header("Formation")]
        public float formationSpacing = 1.5f;
        public int formationPriority = 5; // Lower = front
        
        [Header("AI Behavior")]
        public float acquisitionRange = 600f; // Auto-attack range
        public AIPriority aiPriority = AIPriority.Medium;
        public bool canFlee = true;
        public float fleeHealthPercent = 0.15f;
        
        [Header("Sound")]
        public AudioClip[] selectionSounds;
        public AudioClip[] moveSounds;
        public AudioClip[] attackSounds;
        public AudioClip[] deathSounds;
        
        // Calculated stats
        public float GetDPS()
        {
            float avgDamage = damage + (damageRandomBonus * 0.5f);
            return avgDamage / attackSpeed;
        }
        
        public float GetEffectiveHP(DamageType incomingDamageType)
        {
            float armorReduction = CalculateArmorReduction(armor);
            float typeMultiplier = DamageCalculator.GetDamageMultiplier(incomingDamageType, armorType);
            return hitPoints / (typeMultiplier * (1f - armorReduction));
        }
        
        private float CalculateArmorReduction(float armorValue)
        {
            // Warcraft III formula
            return (armorValue * 0.06f) / (1f + armorValue * 0.06f);
        }
    }
    
    // Enums matching Warcraft III
    public enum Race
    {
        Human,
        Orc, 
        Undead,
        NightElf,
        Neutral
    }
    
    public enum UnitType
    {
        Worker,
        Melee,
        Ranged,
        Caster,
        Siege,
        Flying,
        Hero,
        Building
    }
    
    public enum DamageType
    {
        Normal,
        Piercing,
        Siege,
        Magic,
        Chaos,
        Hero
    }
    
    public enum ArmorType
    {
        Unarmored,
        Light,
        Medium,
        Heavy,
        Fortified,
        Hero,
        Divine
    }
    
    public enum MovementType
    {
        Ground,
        Flying,
        Amphibious,
        Hover
    }
    
    public enum AIPriority
    {
        Low,
        Medium,
        High,
        Hero
    }
    
    /// <summary>
    /// Damage calculation following Warcraft III damage/armor table.
    /// </summary>
    public static class DamageCalculator
    {
        // Damage type vs Armor type effectiveness table (percentages)
        private static readonly float[,] damageTable = new float[,]
        {
            // Unarmored, Light, Medium, Heavy, Fortified, Hero, Divine
            { 1.00f, 1.00f, 1.00f, 1.00f, 0.70f, 1.00f, 0.05f }, // Normal
            { 1.50f, 2.00f, 0.75f, 0.50f, 0.35f, 0.50f, 0.05f }, // Piercing
            { 1.00f, 1.00f, 0.50f, 1.00f, 1.50f, 0.50f, 0.05f }, // Siege
            { 1.25f, 1.25f, 1.00f, 1.00f, 0.35f, 0.50f, 0.05f }, // Magic
            { 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 0.05f }, // Chaos
            { 1.00f, 1.00f, 1.00f, 1.00f, 0.50f, 1.00f, 0.05f }  // Hero
        };
        
        public static float GetDamageMultiplier(DamageType damageType, ArmorType armorType)
        {
            return damageTable[(int)damageType, (int)armorType];
        }
        
        public static float CalculateDamage(float baseDamage, float armor, DamageType damageType, ArmorType armorType)
        {
            float armorReduction = (armor * 0.06f) / (1f + armor * 0.06f);
            float typeMultiplier = GetDamageMultiplier(damageType, armorType);
            return baseDamage * typeMultiplier * (1f - armorReduction);
        }
    }
}