using UnityEngine;

namespace FrostRealm.Data
{
    /// <summary>
    /// ScriptableObject that defines a hero's core properties and capabilities.
    /// Based on Warcraft III: The Frozen Throne hero design patterns.
    /// </summary>
    [CreateAssetMenu(fileName = "New Hero", menuName = "FrostRealm/Hero Data")]
    public class HeroData : ScriptableObject
    {
        [Header("Basic Information")]
        [SerializeField] private string heroName;
        [SerializeField] [TextArea(3, 5)] private string description;
        [SerializeField] private Sprite portrait;
        [SerializeField] private GameObject modelPrefab;
        
        [Header("Classification")]
        [SerializeField] private HeroClass heroClass;
        [SerializeField] private Faction faction;
        [SerializeField] private HeroType heroType;
        
        [Header("Base Stats")]
        [SerializeField] private HeroStats baseStats;
        
        [Header("Abilities")]
        [SerializeField] private AbilityData[] abilities;
        
        [Header("Audio")]
        [SerializeField] private HeroVoiceLines voiceLines;
        
        // Public properties for external access
        public string HeroName => heroName;
        public string Description => description;
        public Sprite Portrait => portrait;
        public GameObject ModelPrefab => modelPrefab;
        public HeroClass HeroClass => heroClass;
        public Faction Faction => faction;
        public HeroType HeroType => heroType;
        public HeroStats BaseStats => baseStats;
        public AbilityData[] Abilities => abilities;
        public HeroVoiceLines VoiceLines => voiceLines;
        
        /// <summary>
        /// Validates the hero data for completeness and correctness.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(heroName) && 
                   portrait != null && 
                   modelPrefab != null && 
                   baseStats.IsValid();
        }
    }
    
    /// <summary>
    /// Defines the hero's class/archetype following TFT patterns.
    /// </summary>
    public enum HeroClass
    {
        Paladin,        // Human - Strength, Holy abilities
        Archmage,       // Human - Intelligence, Arcane magic
        MountainKing,   // Human - Strength, Dwarven abilities
        BloodMage,      // Human - Intelligence, Fire magic
        
        Blademaster,    // Orc - Agility, Martial arts
        FarSeer,        // Orc - Intelligence, Shamanic magic
        TaurenChieftain,// Orc - Strength, Earth abilities
        ShadowHunter,   // Orc - Intelligence, Voodoo magic
        
        DeathKnight,    // Undead - Strength, Death magic
        Lich,           // Undead - Intelligence, Frost magic
        Dreadlord,      // Undead - Strength, Demonic abilities
        CryptLord,      // Undead - Strength, Insect abilities
        
        DemonHunter,    // Night Elf - Agility, Demonic power
        KeeperOfTheGrove, // Night Elf - Intelligence, Nature magic
        PriestessOfTheMoon, // Night Elf - Agility, Lunar magic
        Warden,         // Night Elf - Agility, Shadow abilities
        
        // Neutral heroes
        Beastmaster,    // Neutral - Strength, Animal control
        DarkRanger,     // Neutral - Agility, Dark magic
        PandarenBrewmaster, // Neutral - Strength, Elemental magic
        SeaWitch        // Neutral - Intelligence, Water magic
    }
    
    /// <summary>
    /// Faction alignment for heroes.
    /// </summary>
    public enum Faction
    {
        Human,
        Orc,
        Undead,
        NightElf,
        Neutral
    }
    
    /// <summary>
    /// Hero type classification for gameplay balance.
    /// </summary>
    public enum HeroType
    {
        Melee,      // Close combat specialist
        Ranged,     // Ranged combat specialist
        Caster,     // Magic-focused hero
        Support,    // Support and utility focused
        Hybrid      // Mix of combat and magic
    }
    
    /// <summary>
    /// Hero statistics following TFT attribute system.
    /// </summary>
    [System.Serializable]
    public struct HeroStats
    {
        [Header("Primary Attributes")]
        public int strength;        // Affects HP and melee damage
        public int agility;         // Affects attack speed and armor
        public int intelligence;    // Affects mana and spell damage
        
        [Header("Attribute Growth")]
        public float strengthGrowth;    // Per level
        public float agilityGrowth;     // Per level
        public float intelligenceGrowth; // Per level
        
        [Header("Combat Stats")]
        public int baseHealth;
        public int baseMana;
        public float baseMovementSpeed;
        public float baseAttackSpeed;
        public int baseDamageMin;
        public int baseDamageMax;
        public int baseArmor;
        
        /// <summary>
        /// Validates the stats for reasonable values.
        /// </summary>
        public bool IsValid()
        {
            return strength > 0 && agility > 0 && intelligence > 0 &&
                   baseHealth > 0 && baseMana >= 0 && baseMovementSpeed > 0;
        }
    }
    
    
    /// <summary>
    /// Hero voice lines for immersive audio experience.
    /// </summary>
    [System.Serializable]
    public struct HeroVoiceLines
    {
        [Header("Selection Lines")]
        public AudioClip[] selectionLines;
        
        [Header("Movement Lines")]
        public AudioClip[] movementLines;
        
        [Header("Attack Lines")]
        public AudioClip[] attackLines;
        
        [Header("Ability Lines")]
        public AudioClip[] abilityLines;
    }
}