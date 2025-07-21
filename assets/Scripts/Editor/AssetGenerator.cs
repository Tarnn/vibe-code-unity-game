using UnityEngine;
using UnityEditor;
using System.IO;
using FrostRealm.Data;

namespace FrostRealm.Editor
{
    /// <summary>
    /// Generates all unit data, hero data, and ability assets for FrostRealm Chronicles.
    /// Creates complete Warcraft III-style data following the GDD specifications.
    /// </summary>
    public class AssetGenerator
    {
        [MenuItem("FrostRealm/Generate All Assets")]
        public static void GenerateAllAssets()
        {
            Debug.Log("=== Generating FrostRealm Chronicles Assets ===");
            
            CreateDirectories();
            GenerateHeroData();
            GenerateUnitData();
            GenerateAbilityData();
            GenerateUpgradeData();
            
            AssetDatabase.Refresh();
            Debug.Log("✅ All assets generated successfully!");
        }
        
        private static void CreateDirectories()
        {
            string[] directories = {
                "assets/Data/Heroes",
                "assets/Data/Units/Human",
                "assets/Data/Units/Orc",
                "assets/Data/Units/Undead",
                "assets/Data/Units/NightElf",
                "assets/Data/Abilities",
                "assets/Data/Upgrades"
            };
            
            foreach (string dir in directories)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }
        
        #region Hero Generation
        
        private static void GenerateHeroData()
        {
            Debug.Log("Generating Hero Data...");
            
            // Human Heroes
            CreatePaladinHero();
            CreateArchmageHero();
            
            // Orc Heroes  
            CreateBlademasterHero();
            CreateFarSeerHero();
            
            // Undead Heroes
            CreateDeathKnightHero();
            CreateLichHero();
            
            // Night Elf Heroes
            CreateDemonHunterHero();
            CreateKeeperOfTheGroveHero();
        }
        
        private static void CreatePaladinHero()
        {
            var paladin = ScriptableObject.CreateInstance<HeroData>();
            paladin.heroName = "Paladin";
            paladin.title = "Knight of the Silver Hand";
            paladin.description = "Holy warrior dedicated to justice and protection.";
            paladin.race = Race.Human;
            paladin.primaryAttribute = HeroAttribute.Strength;
            
            // Base attributes (level 1)
            paladin.baseStrength = 22;
            paladin.baseAgility = 13;
            paladin.baseIntelligence = 17;
            paladin.strengthGain = 2.6f;
            paladin.agilityGain = 1.1f;
            paladin.intelligenceGain = 1.8f;
            
            // Stats
            paladin.baseMovementSpeed = 300f;
            
            AssetDatabase.CreateAsset(paladin, "assets/Data/Heroes/PaladinHero.asset");
        }
        
        private static void CreateArchmageHero()
        {
            var archmage = ScriptableObject.CreateInstance<HeroData>();
            archmage.heroName = "Archmage";
            archmage.title = "Master of the Arcane";
            archmage.description = "Powerful spellcaster wielding elemental magic.";
            archmage.race = Race.Human;
            archmage.primaryAttribute = HeroAttribute.Intelligence;
            
            archmage.baseStrength = 16;
            archmage.baseAgility = 14;
            archmage.baseIntelligence = 20;
            archmage.strengthGain = 1.8f;
            archmage.agilityGain = 1.2f;
            archmage.intelligenceGain = 2.7f;
            
            archmage.baseMovementSpeed = 270f;
            
            AssetDatabase.CreateAsset(archmage, "assets/Data/Heroes/ArchmageHero.asset");
        }
        
        private static void CreateBlademasterHero()
        {
            var blademaster = ScriptableObject.CreateInstance<HeroData>();
            blademaster.heroName = "Blademaster";
            blademaster.title = "Swift Warrior";
            blademaster.description = "Agile swordsman with deadly precision.";
            blademaster.race = Race.Orc;
            blademaster.primaryAttribute = HeroAttribute.Agility;
            
            blademaster.baseStrength = 18;
            blademaster.baseAgility = 22;
            blademaster.baseIntelligence = 16;
            blademaster.strengthGain = 2.3f;
            blademaster.agilityGain = 2.6f;
            blademaster.intelligenceGain = 1.4f;
            
            blademaster.baseMovementSpeed = 320f;
            
            AssetDatabase.CreateAsset(blademaster, "assets/Data/Heroes/BlademasterHero.asset");
        }
        
        private static void CreateFarSeerHero()
        {
            var farseer = ScriptableObject.CreateInstance<HeroData>();
            farseer.heroName = "Far Seer";
            farseer.title = "Shaman Prophet";
            farseer.description = "Wise shaman with command over the elements.";
            farseer.race = Race.Orc;
            farseer.primaryAttribute = HeroAttribute.Intelligence;
            
            farseer.baseStrength = 19;
            farseer.baseAgility = 15;
            farseer.baseIntelligence = 21;
            farseer.strengthGain = 2.2f;
            farseer.agilityGain = 1.3f;
            farseer.intelligenceGain = 2.8f;
            
            farseer.baseMovementSpeed = 300f;
            
            AssetDatabase.CreateAsset(farseer, "assets/Data/Heroes/FarSeerHero.asset");
        }
        
        private static void CreateDeathKnightHero()
        {
            var deathknight = ScriptableObject.CreateInstance<HeroData>();
            deathknight.heroName = "Death Knight";
            deathknight.title = "Fallen Paladin";
            deathknight.description = "Corrupted warrior wielding necromantic power.";
            deathknight.race = Race.Undead;
            deathknight.primaryAttribute = HeroAttribute.Strength;
            
            deathknight.baseStrength = 24;
            deathknight.baseAgility = 13;
            deathknight.baseIntelligence = 19;
            deathknight.strengthGain = 2.7f;
            deathknight.agilityGain = 1.1f;
            deathknight.intelligenceGain = 2.1f;
            
            deathknight.baseMovementSpeed = 300f;
            
            AssetDatabase.CreateAsset(deathknight, "assets/Data/Heroes/DeathKnightHero.asset");
        }
        
        private static void CreateLichHero()
        {
            var lich = ScriptableObject.CreateInstance<HeroData>();
            lich.heroName = "Lich";
            lich.title = "Undead Sorcerer";
            lich.description = "Master of death magic and frost spells.";
            lich.race = Race.Undead;
            lich.primaryAttribute = HeroAttribute.Intelligence;
            
            lich.baseStrength = 15;
            lich.baseAgility = 11;
            lich.baseIntelligence = 23;
            lich.strengthGain = 1.6f;
            lich.agilityGain = 1.0f;
            lich.intelligenceGain = 3.0f;
            
            lich.baseMovementSpeed = 270f;
            
            AssetDatabase.CreateAsset(lich, "assets/Data/Heroes/LichHero.asset");
        }
        
        private static void CreateDemonHunterHero()
        {
            var demonhunter = ScriptableObject.CreateInstance<HeroData>();
            demonhunter.heroName = "Demon Hunter";
            demonhunter.title = "Illidari Outcast";
            demonhunter.description = "Blind warrior infused with demonic power.";
            demonhunter.race = Race.NightElf;
            demonhunter.primaryAttribute = HeroAttribute.Agility;
            
            demonhunter.baseStrength = 20;
            demonhunter.baseAgility = 23;
            demonhunter.baseIntelligence = 16;
            demonhunter.strengthGain = 2.4f;
            demonhunter.agilityGain = 2.8f;
            demonhunter.intelligenceGain = 1.5f;
            
            demonhunter.baseMovementSpeed = 320f;
            
            AssetDatabase.CreateAsset(demonhunter, "assets/Data/Heroes/DemonHunterHero.asset");
        }
        
        private static void CreateKeeperOfTheGroveHero()
        {
            var keeper = ScriptableObject.CreateInstance<HeroData>();
            keeper.heroName = "Keeper of the Grove";
            keeper.title = "Ancient Protector";
            keeper.description = "Guardian of nature with healing powers.";
            keeper.race = Race.NightElf;
            keeper.primaryAttribute = HeroAttribute.Intelligence;
            
            keeper.baseStrength = 17;
            keeper.baseAgility = 14;
            keeper.baseIntelligence = 22;
            keeper.strengthGain = 2.0f;
            keeper.agilityGain = 1.2f;
            keeper.intelligenceGain = 2.9f;
            
            keeper.baseMovementSpeed = 290f;
            
            AssetDatabase.CreateAsset(keeper, "assets/Data/Heroes/KeeperOfTheGroveHero.asset");
        }
        
        #endregion
        
        #region Unit Generation
        
        private static void GenerateUnitData()
        {
            Debug.Log("Generating Unit Data...");
            
            // Human Units
            CreateHumanFootman();
            CreateHumanRifleman();
            CreateHumanKnight();
            CreateHumanPriest();
            
            // Orc Units
            CreateOrcGrunt();
            CreateOrcTrollHeadhunter();
            CreateOrcTaurenWarrior();
            CreateOrcShamanUnit();
            
            // Add more units as needed...
        }
        
        private static void CreateHumanFootman()
        {
            var footman = ScriptableObject.CreateInstance<UnitData>();
            footman.unitName = "Footman";
            footman.description = "Basic human infantry unit.";
            footman.race = Race.Human;
            footman.unitType = UnitType.Melee;
            
            footman.hitPoints = 420;
            footman.damage = 12;
            footman.damageRandomBonus = 1;
            footman.damageType = DamageType.Normal;
            footman.armor = 2;
            footman.armorType = ArmorType.Heavy;
            
            footman.moveSpeed = 270f;
            footman.attackSpeed = 1.35f;
            footman.attackRange = 128f;
            
            footman.goldCost = 135;
            footman.lumberCost = 0;
            footman.foodCost = 2;
            footman.buildTime = 30f;
            
            AssetDatabase.CreateAsset(footman, "assets/Data/Units/Human/Footman.asset");
        }
        
        private static void CreateHumanRifleman()
        {
            var rifleman = ScriptableObject.CreateInstance<UnitData>();
            rifleman.unitName = "Rifleman";
            rifleman.description = "Ranged human gunner.";
            rifleman.race = Race.Human;
            rifleman.unitType = UnitType.Ranged;
            
            rifleman.hitPoints = 505;
            rifleman.damage = 18;
            rifleman.damageRandomBonus = 6;
            rifleman.damageType = DamageType.Piercing;
            rifleman.armor = 0;
            rifleman.armorType = ArmorType.Medium;
            
            rifleman.moveSpeed = 220f;
            rifleman.attackSpeed = 1.5f;
            rifleman.attackRange = 500f;
            
            rifleman.goldCost = 205;
            rifleman.lumberCost = 30;
            rifleman.foodCost = 3;
            rifleman.buildTime = 35f;
            
            AssetDatabase.CreateAsset(rifleman, "assets/Data/Units/Human/Rifleman.asset");
        }
        
        private static void CreateHumanKnight()
        {
            var knight = ScriptableObject.CreateInstance<UnitData>();
            knight.unitName = "Knight";
            knight.description = "Heavy cavalry unit.";
            knight.race = Race.Human;
            knight.unitType = UnitType.Melee;
            
            knight.hitPoints = 800;
            knight.damage = 34;
            knight.damageRandomBonus = 9;
            knight.damageType = DamageType.Normal;
            knight.armor = 5;
            knight.armorType = ArmorType.Heavy;
            
            knight.moveSpeed = 400f;
            knight.attackSpeed = 1.4f;
            knight.attackRange = 128f;
            
            knight.goldCost = 245;
            knight.lumberCost = 60;
            knight.foodCost = 4;
            knight.buildTime = 45f;
            
            AssetDatabase.CreateAsset(knight, "assets/Data/Units/Human/Knight.asset");
        }
        
        private static void CreateHumanPriest()
        {
            var priest = ScriptableObject.CreateInstance<UnitData>();
            priest.unitName = "Priest";
            priest.description = "Human spellcaster with healing abilities.";
            priest.race = Race.Human;
            priest.unitType = UnitType.Caster;
            
            priest.hitPoints = 350;
            priest.manaPoints = 300;
            priest.damage = 12;
            priest.damageRandomBonus = 3;
            priest.damageType = DamageType.Magic;
            priest.armor = 0;
            priest.armorType = ArmorType.Unarmored;
            
            priest.moveSpeed = 270f;
            priest.attackSpeed = 1.5f;
            priest.attackRange = 450f;
            
            priest.goldCost = 135;
            priest.lumberCost = 10;
            priest.foodCost = 2;
            priest.buildTime = 40f;
            
            AssetDatabase.CreateAsset(priest, "assets/Data/Units/Human/Priest.asset");
        }
        
        // Orc Units
        private static void CreateOrcGrunt()
        {
            var grunt = ScriptableObject.CreateInstance<UnitData>();
            grunt.unitName = "Grunt";
            grunt.description = "Basic orc infantry.";
            grunt.race = Race.Orc;
            grunt.unitType = UnitType.Melee;
            
            grunt.hitPoints = 700;
            grunt.damage = 19;
            grunt.damageRandomBonus = 2;
            grunt.damageType = DamageType.Normal;
            grunt.armor = 1;
            grunt.armorType = ArmorType.Medium;
            
            grunt.moveSpeed = 270f;
            grunt.attackSpeed = 1.5f;
            grunt.attackRange = 128f;
            
            grunt.goldCost = 200;
            grunt.lumberCost = 0;
            grunt.foodCost = 3;
            grunt.buildTime = 30f;
            
            AssetDatabase.CreateAsset(grunt, "assets/Data/Units/Orc/Grunt.asset");
        }
        
        private static void CreateOrcTrollHeadhunter()
        {
            var troll = ScriptableObject.CreateInstance<UnitData>();
            troll.unitName = "Troll Headhunter";
            troll.description = "Ranged troll warrior.";
            troll.race = Race.Orc;
            troll.unitType = UnitType.Ranged;
            
            troll.hitPoints = 350;
            troll.damage = 21;
            troll.damageRandomBonus = 5;
            troll.damageType = DamageType.Piercing;
            troll.armor = 0;
            troll.armorType = ArmorType.Light;
            
            troll.moveSpeed = 270f;
            troll.attackSpeed = 1.5f;
            troll.attackRange = 450f;
            
            troll.goldCost = 135;
            troll.lumberCost = 20;
            troll.foodCost = 2;
            troll.buildTime = 25f;
            
            AssetDatabase.CreateAsset(troll, "assets/Data/Units/Orc/TrollHeadhunter.asset");
        }
        
        private static void CreateOrcTaurenWarrior()
        {
            var tauren = ScriptableObject.CreateInstance<UnitData>();
            tauren.unitName = "Tauren Warrior";
            tauren.description = "Massive minotaur warrior.";
            tauren.race = Race.Orc;
            tauren.unitType = UnitType.Melee;
            
            tauren.hitPoints = 1200;
            tauren.damage = 45;
            tauren.damageRandomBonus = 5;
            tauren.damageType = DamageType.Normal;
            tauren.armor = 2;
            tauren.armorType = ArmorType.Heavy;
            
            tauren.moveSpeed = 270f;
            tauren.attackSpeed = 1.6f;
            tauren.attackRange = 128f;
            
            tauren.goldCost = 280;
            tauren.lumberCost = 80;
            tauren.foodCost = 5;
            tauren.buildTime = 60f;
            
            AssetDatabase.CreateAsset(tauren, "assets/Data/Units/Orc/TaurenWarrior.asset");
        }
        
        private static void CreateOrcShamanUnit()
        {
            var shaman = ScriptableObject.CreateInstance<UnitData>();
            shaman.unitName = "Shaman";
            shaman.description = "Orc spellcaster with elemental magic.";
            shaman.race = Race.Orc;
            shaman.unitType = UnitType.Caster;
            
            shaman.hitPoints = 350;
            shaman.manaPoints = 300;
            shaman.damage = 14;
            shaman.damageRandomBonus = 4;
            shaman.damageType = DamageType.Magic;
            shaman.armor = 0;
            shaman.armorType = ArmorType.Unarmored;
            
            shaman.moveSpeed = 270f;
            shaman.attackSpeed = 1.5f;
            shaman.attackRange = 400f;
            
            shaman.goldCost = 130;
            shaman.lumberCost = 20;
            shaman.foodCost = 2;
            shaman.buildTime = 40f;
            
            AssetDatabase.CreateAsset(shaman, "assets/Data/Units/Orc/Shaman.asset");
        }
        
        #endregion
        
        #region Ability Generation
        
        private static void GenerateAbilityData()
        {
            Debug.Log("Generating Ability Data...");
            
            CreateHolyLightAbility();
            CreateBlizzardAbility();
            CreateCriticalStrikeAbility();
            CreateDeathCoilAbility();
        }
        
        private static void CreateHolyLightAbility()
        {
            var holyLight = ScriptableObject.CreateInstance<AbilityData>();
            holyLight.abilityName = "Holy Light";
            holyLight.description = "Heals a target or damages undead units.";
            holyLight.abilityType = AbilityType.Active;
            holyLight.targetType = TargetType.Unit;
            holyLight.targetFilter = TargetFilter.All;
            
            holyLight.castRange = 500f;
            holyLight.manaCost = new int[] { 65, 65, 65 };
            holyLight.cooldown = new float[] { 5f, 5f, 5f };
            holyLight.maxLevel = 3;
            holyLight.requiredHeroLevel = 1;
            
            holyLight.effects = new AbilityEffect[]
            {
                new AbilityEffect { effectType = EffectType.Heal, healAmount = 200f },
                new AbilityEffect { effectType = EffectType.Heal, healAmount = 400f },
                new AbilityEffect { effectType = EffectType.Heal, healAmount = 600f }
            };
            
            AssetDatabase.CreateAsset(holyLight, "assets/Data/Abilities/HolyLight.asset");
        }
        
        private static void CreateBlizzardAbility()
        {
            var blizzard = ScriptableObject.CreateInstance<AbilityData>();
            blizzard.abilityName = "Blizzard";
            blizzard.description = "Calls down freezing shards over an area.";
            blizzard.abilityType = AbilityType.Active;
            blizzard.targetType = TargetType.Area;
            blizzard.targetFilter = TargetFilter.Enemy;
            
            blizzard.castRange = 800f;
            blizzard.areaOfEffect = 200f;
            blizzard.manaCost = new int[] { 75, 125, 200 };
            blizzard.cooldown = new float[] { 8f, 7f, 6f };
            blizzard.maxLevel = 3;
            blizzard.requiredHeroLevel = 1;
            blizzard.isUltimate = true;
            
            blizzard.channeling = true;
            blizzard.channelTime = 6f;
            
            blizzard.effects = new AbilityEffect[]
            {
                new AbilityEffect { effectType = EffectType.Damage, value = 100f, damageType = DamageType.Magic },
                new AbilityEffect { effectType = EffectType.Damage, value = 150f, damageType = DamageType.Magic },
                new AbilityEffect { effectType = EffectType.Damage, value = 200f, damageType = DamageType.Magic }
            };
            
            AssetDatabase.CreateAsset(blizzard, "assets/Data/Abilities/Blizzard.asset");
        }
        
        private static void CreateCriticalStrikeAbility()
        {
            var critStrike = ScriptableObject.CreateInstance<AbilityData>();
            critStrike.abilityName = "Critical Strike";
            critStrike.description = "Chance to deal increased damage.";
            critStrike.abilityType = AbilityType.Passive;
            
            critStrike.maxLevel = 3;
            critStrike.requiredHeroLevel = 1;
            
            critStrike.effects = new AbilityEffect[]
            {
                new AbilityEffect { effectType = EffectType.Damage, value = 2f }, // 2x damage
                new AbilityEffect { effectType = EffectType.Damage, value = 3f }, // 3x damage
                new AbilityEffect { effectType = EffectType.Damage, value = 4f }  // 4x damage
            };
            
            AssetDatabase.CreateAsset(critStrike, "assets/Data/Abilities/CriticalStrike.asset");
        }
        
        private static void CreateDeathCoilAbility()
        {
            var deathCoil = ScriptableObject.CreateInstance<AbilityData>();
            deathCoil.abilityName = "Death Coil";
            deathCoil.description = "Damages enemies or heals undead units.";
            deathCoil.abilityType = AbilityType.Active;
            deathCoil.targetType = TargetType.Unit;
            deathCoil.targetFilter = TargetFilter.All;
            
            deathCoil.castRange = 500f;
            deathCoil.manaCost = new int[] { 75, 75, 75 };
            deathCoil.cooldown = new float[] { 8f, 8f, 8f };
            deathCoil.maxLevel = 3;
            deathCoil.requiredHeroLevel = 1;
            
            deathCoil.effects = new AbilityEffect[]
            {
                new AbilityEffect { effectType = EffectType.Damage, value = 100f, damageType = DamageType.Magic },
                new AbilityEffect { effectType = EffectType.Damage, value = 200f, damageType = DamageType.Magic },
                new AbilityEffect { effectType = EffectType.Damage, value = 300f, damageType = DamageType.Magic }
            };
            
            AssetDatabase.CreateAsset(deathCoil, "assets/Data/Abilities/DeathCoil.asset");
        }
        
        #endregion
        
        #region Upgrade Generation
        
        private static void GenerateUpgradeData()
        {
            Debug.Log("Generating Upgrade Data...");
            
            CreateIronForgedSwordsUpgrade();
            CreateReinforcedArmorUpgrade();
        }
        
        private static void CreateIronForgedSwordsUpgrade()
        {
            var ironSwords = ScriptableObject.CreateInstance<UpgradeData>();
            ironSwords.upgradeName = "Iron Forged Swords";
            ironSwords.description = "Increases attack damage of melee units.";
            ironSwords.race = Race.Human;
            ironSwords.effectType = UpgradeEffect.Attack;
            ironSwords.maxLevel = 3;
            
            ironSwords.levels = new UpgradeLevel[]
            {
                new UpgradeLevel { goldCost = 100, lumberCost = 50, researchTime = 60f, attackBonus = 1 },
                new UpgradeLevel { goldCost = 150, lumberCost = 75, researchTime = 75f, attackBonus = 2 },
                new UpgradeLevel { goldCost = 200, lumberCost = 100, researchTime = 90f, attackBonus = 3 }
            };
            
            AssetDatabase.CreateAsset(ironSwords, "assets/Data/Upgrades/IronForgedSwords.asset");
        }
        
        private static void CreateReinforcedArmorUpgrade()
        {
            var armor = ScriptableObject.CreateInstance<UpgradeData>();
            armor.upgradeName = "Reinforced Armor";
            armor.description = "Increases armor of units.";
            armor.race = Race.Human;
            armor.effectType = UpgradeEffect.Armor;
            armor.maxLevel = 3;
            
            armor.levels = new UpgradeLevel[]
            {
                new UpgradeLevel { goldCost = 150, lumberCost = 75, researchTime = 60f, armorBonus = 1 },
                new UpgradeLevel { goldCost = 225, lumberCost = 112, researchTime = 75f, armorBonus = 2 },
                new UpgradeLevel { goldCost = 300, lumberCost = 150, researchTime = 90f, armorBonus = 3 }
            };
            
            AssetDatabase.CreateAsset(armor, "assets/Data/Upgrades/ReinforcedArmor.asset");
        }
        
        #endregion
    }
}