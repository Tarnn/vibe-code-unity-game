using UnityEngine;
using System.Collections.Generic;

namespace FrostRealm.Data
{
    /// <summary>
    /// ScriptableObject for ability data following Warcraft III ability system.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbility", menuName = "FrostRealm/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        [Header("Basic Information")]
        public string abilityName = "Ability";
        public string description = "";
        public Sprite icon;
        public AbilityType abilityType = AbilityType.Active;
        
        [Header("Targeting")]
        public TargetType targetType = TargetType.None;
        public TargetFilter targetFilter = TargetFilter.All;
        [Range(0, 2000)] public float castRange = 500f;
        [Range(0, 1000)] public float areaOfEffect = 0f;
        
        [Header("Cost and Cooldown")]
        public int[] manaCost = { 50, 75, 100 }; // Per level
        public float[] cooldown = { 10f, 9f, 8f }; // Per level
        public int maxLevel = 3;
        public int requiredHeroLevel = 1;
        public bool isUltimate = false;
        
        [Header("Cast Time")]
        public float castPoint = 0.3f; // Time before effect
        public float castBackswing = 0.5f; // Recovery time
        public bool channeling = false;
        public float channelTime = 0f;
        
        [Header("Effects per Level")]
        public AbilityEffect[] effects = new AbilityEffect[3];
        
        [Header("Visual Effects")]
        public GameObject castEffectPrefab;
        public GameObject impactEffectPrefab;
        public GameObject projectilePrefab;
        public float projectileSpeed = 900f;
        
        [Header("Sound")]
        public AudioClip castSound;
        public AudioClip impactSound;
        
        [Header("AI Usage")]
        public float aiCastPriority = 5f; // 1-10, higher = more likely
        public float aiMinimumManaPercent = 0.3f;
        public bool aiCastOnAllies = false;
        public bool aiCastOnEnemies = true;
    }
    
    [System.Serializable]
    public class AbilityEffect
    {
        public EffectType effectType = EffectType.Damage;
        public float value = 100f;
        public float duration = 0f; // For buffs/debuffs
        public DamageType damageType = DamageType.Magic; // If damage effect
        public string buffName = ""; // For buff/debuff identification
        
        // Additional effect parameters
        public float slowPercent = 0f;
        public float stunDuration = 0f;
        public float healAmount = 0f;
        public float summonDuration = 0f;
        public UnitData summonUnit = null;
    }
    
    public enum AbilityType
    {
        Active,
        Passive,
        Aura,
        Autocast
    }
    
    public enum TargetType
    {
        None, // Instant cast
        Unit,
        Point,
        Area,
        Direction
    }
    
    public enum TargetFilter
    {
        All,
        Enemy,
        Ally,
        Self,
        Ground,
        Air,
        Organic,
        Mechanical,
        Hero,
        NonHero
    }
    
    public enum EffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        Summon,
        Stun,
        Slow,
        Silence,
        Dispel,
        Teleport
    }
    
    /// <summary>
    /// Example hero abilities from Warcraft III
    /// </summary>
    public static class AbilityExamples
    {
        // Paladin - Holy Light
        public static AbilityEffect[] HolyLightEffects = new AbilityEffect[]
        {
            new AbilityEffect { effectType = EffectType.Heal, healAmount = 200f },
            new AbilityEffect { effectType = EffectType.Heal, healAmount = 400f },
            new AbilityEffect { effectType = EffectType.Heal, healAmount = 600f }
        };
        
        // Blademaster - Critical Strike (Passive)
        public static AbilityEffect[] CriticalStrikeEffects = new AbilityEffect[]
        {
            new AbilityEffect { effectType = EffectType.Damage, value = 2f }, // 2x damage
            new AbilityEffect { effectType = EffectType.Damage, value = 3f }, // 3x damage
            new AbilityEffect { effectType = EffectType.Damage, value = 4f }  // 4x damage
        };
        
        // Death Knight - Death Coil
        public static AbilityEffect[] DeathCoilEffects = new AbilityEffect[]
        {
            new AbilityEffect { effectType = EffectType.Damage, value = 100f, damageType = DamageType.Magic },
            new AbilityEffect { effectType = EffectType.Damage, value = 200f, damageType = DamageType.Magic },
            new AbilityEffect { effectType = EffectType.Damage, value = 300f, damageType = DamageType.Magic }
        };
    }
}