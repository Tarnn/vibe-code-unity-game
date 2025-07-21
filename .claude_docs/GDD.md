# Game Design Document (GDD) for "FrostRealm Chronicles"

## 1. Title Page
- **Game Title**: FrostRealm Chronicles
- **Genre**: Real-Time Strategy (RTS)
- **Platform**: PC (Windows/macOS primary; Steam Deck compatible)
- **Target Audience**: Fans of classic RTS games like Warcraft III: The Frozen Throne, aged 12+; Strategy enthusiasts seeking deep mechanics and modern visuals.
- **Development Team**: Solo developer (lead engineer/video game designer) using AI tools (Cursor/Claude for code; AI generators for assets).
- **Engine**: Unity 6.1 LTS (July 2025 release)
- **Version**: 1.0 (Initial Prototype)
- **Date**: July 19, 2025
- **Inspiration**: Direct emulation of Warcraft III: The Frozen Throne (TFT), with mechanics like hero leveling, asymmetric races, creep farming, and upkeep systems. Graphics target Warcraft III: Reforged quality (high-poly models, 4K textures, ray-traced effects).

## 2. Executive Summary
FrostRealm Chronicles is an RTS game that recreates the strategic depth and fantasy lore of Warcraft III: The Frozen Throne, featuring four asymmetric races (Human Alliance, Orc Horde, Undead Scourge, Night Elf Sentinels), hero-driven combat, resource management, and expansive campaigns. Players build bases, gather resources (gold, lumber, food), train units, level heroes, and engage in real-time battles with rock-paper-scissors unit counters and spellcasting.

**High-Concept Pitch**: A hero-centric RTS where asymmetric factions clash in a frozen fantasy world, blending base-building strategy with RPG hero progression—like Warcraft III meets modern ray-traced visuals.

**Target Rating**: T for Teen (ESRB/PEGI equivalent: fantasy violence, mild blood).

**Comparable Titles**: Warcraft III: Reforged (core mechanics and visuals), StarCraft II (multiplayer balance and e-sports features), Age of Empires IV (modern RTS polish with historical asymmetry adapted to fantasy).

Key Features:
- Single-player campaigns mirroring TFT's narrative arcs (e.g., betrayal, alliances).
- Multiplayer skirmishes (1v1 to 4v4) with custom maps.
- Modern enhancements: Ray-traced graphics via Unity HDRP for immersive visuals (e.g., dynamic shadows in day/night cycles).
- Development Focus: Free tools; AI-generated assets at Reforged fidelity.

Core Loop: Gather resources → Build/tech up → Scout/creep farm → Engage/harass → Destroy enemy bases.

Unique Selling Points: Faithful TFT mechanics (e.g., exact damage formulas, hero attributes) with Unity 6 optimizations for large battles (500+ units at 60 FPS).

## 3. Story and Setting
### Lore Overview
In the frozen realms of Azeroth-inspired world "FrostRealm," ancient evils awaken after a cataclysmic war. Four factions vie for control:
- **Human Alliance**: Noble survivors rebuilding kingdoms, focusing on defense and arcane magic.
- **Orc Horde**: Brutal warriors seeking honor in conquest, emphasizing aggression and shamanism.
- **Undead Scourge**: Necrotic legions spreading plague, reliant on swarms and resurrection.
- **Night Elf Sentinels**: Ancient guardians of nature, using mobility and stealth.

**Themes and Tone**: Redemption amid chaos; Epic fantasy with dark undertones, balanced by heroic moments and humorous unit voice lines for levity.

Story Arc (Inspired by TFT):
- Campaigns: 4 interconnected stories (8-12 missions each).
  - Human: Reclaim lost territories amid betrayals (e.g., blood elf alliances like TFT's Curse of the Blood Elves).
  - Orc: Reform the horde against demonic influences (similar to Founding of Durotar bonus campaign).
  - Undead: Expand the scourge's icy grip (Legacy of the Damned).
  - Night Elf: Protect sacred groves from invasion (Terror of the Tides).
- Themes: Betrayal, redemption, alliance fragility; Neutral elements like Naga and Pandaren add depth.

### Character Bios
- **Paladin (Human Hero)**: A holy warrior sworn to protect the innocent, haunted by past failures in the cataclysmic war; embodies justice but struggles with rigid faith.
- **Blademaster (Orc Hero)**: A cunning swordsman from a fallen clan, seeking personal glory while uniting the horde; quick-witted with a code of honor.
- **Death Knight (Undead Hero)**: A fallen paladin resurrected as a harbinger of death, torn between loyalty to the scourge and lingering humanity.
- **Demon Hunter (Night Elf Hero)**: A blind outcast who sacrificed sight for demonic power, driven by vengeance against ancient evils threatening the forests.

### World Building
- **Lore Timeline**:
  - Pre-Cataclysm (Ancient Era): Peaceful realms where factions coexisted; Night Elves guard sacred groves.
  - Cataclysm (Year 0): Ancient evils shatter the world, awakening undead and forcing orcs/human migrations.
  - Post-Cataclysm (Current Era): Factions war for resources; Neutral races like Naga emerge from seas.
- Maps: Procedurally generated or hand-crafted with terrain types (forests for lumber, gold mines, creep camps).
- Day/Night Cycle: 4-min day/2-min night affecting visibility and abilities (e.g., Night Elf bonuses).
- Neutral Elements: Creep camps (levels 1-10, drop XP/items); Buildings like Tavern (hire neutral heroes), Marketplace (rotating items).

## 4. Gameplay Overview
### Game Modes
- **Single-Player Campaigns**: Narrative-driven missions with objectives (e.g., base defense, hero quests, escorts). Difficulty: Easy (AI passive, reduced enemy waves), Normal (balanced aggression), Hard (aggressive AI with adaptive counters, increased creep difficulty).
- **Skirmish/Multiplayer**: Melee maps (2-8 players); Free-for-All, Teams; Custom maps with editor support.
- **Tutorial**: Interactive missions teaching basics (resource gathering, unit control).
- **Win Conditions**: Destroy all enemy buildings/units; Achieve objectives (e.g., hold points).

**Game Length**: Skirmish sessions: 20-45 minutes; Campaign missions: 30-60 minutes; Full campaign: 20-30 hours.

### Player Perspective
- Isometric top-down view (45-degree angle, zoomable).
- Controls: Mouse for selection/commands; Keyboard hotkeys (TFT-style: A=Attack, M=Move, QWER=Abilities).

### Core Pillars
- **Strategy Depth**: Asymmetric races with unique tech trees and counters.
- **Hero Management**: Level heroes to turn battles.
- **Resource Economy**: Balance gathering with upkeep penalties.
- **Tactical Combat**: Micro abilities, macro expansions.

**Player Flow Diagram** (Text-Based):
- Main Menu → [New Game/Load/Options/Quit] → Race Select → Map Load → Tutorial Popups (if first play) → Core Loop (Gather/Build/Scout/Combat) → Victory/Defeat Screen → Post-Match Stats/Replay → Return to Menu.

## 5. Core Mechanics
### Races and Factions
Four races with TFT-inspired asymmetry:
- **Human Alliance**: Versatile, defensive. Strengths: Fast expansion (Militia creeps), strong towers. Weaknesses: Fragile early units.
- **Orc Horde**: Aggressive, durable. Strengths: High-damage melee, burrows for regen. Weaknesses: Slow tech, lumber-heavy.
- **Undead Scourge**: Swarm/regenerative. Strengths: Fast build (summon), corpse mechanics. Weaknesses: Blight dependency, weak vs magic.
- **Night Elf Sentinels**: Mobile, nature-based. Strengths: Uproot buildings, stealth. Weaknesses: Fragile workers, mana-reliant.

Neutral Heroes (via Tavern): 8 options (e.g., Naga Sea Witch: Forked Lightning chain dmg 85/150/225).

### Resource Management
- **Gold**: Mined from mines (8-12g/trip; max 5 workers; holds 5k-30k).
- **Lumber**: Harvested from trees (10/chop; 200-400/tree; regrow 60-120s).
- **Food**: Population cap (max 100); Provided by farms/burrows (e.g., Farm: +10 food).
- **Upkeep**: 0-50: 100% income; 51-80: 70%; 81-100: 40%. Formula: Income = base * multiplier.

**Economy Model Breakdown**: Early game focuses on gold for worker production and basic units; Mid-game introduces lumber bottlenecks for Tier 2 tech and upgrades; Late-game requires expansions to new mines/forests to offset upkeep penalties and support large armies. Worker efficiency: Auto-return resources, but interruptions (e.g., attacks) drop partial loads.

### Base Building
- Tech Tree: Tier 1 (basic), Tier 2 (advanced), Tier 3 (elite).
- Placement: Grid-based; Validity checks (no overlap, terrain-specific e.g., blight for Undead).
- Construction: Timers (e.g., Town Hall 60s); Workers repair (50% speed).

### Units
12-15 per race; Stats tables (base values; upgrades +1-3):

| Race | Unit | HP | Damage (Type) | Armor (Type) | Cost (G/L/F) | Abilities/Notes |
|------|------|----|---------------|--------------|--------------|-----------------|
| Human | Footman | 420 | 12-13 (Normal) | 2 (Heavy) | 135/0/2 | Defend (+armor vs piercing) |
| Human | Rifleman | 505 | 18-24 (Piercing) | 0 (Medium) | 205/30/3 | Long range |
| Orc | Grunt | 700 | 19-21 (Normal) | 1 (Medium) | 200/0/3 | Pillage (gain resources on attack) |
| Orc | Troll Headhunter | 350 | 21-26 (Piercing) | 0 (Light) | 135/20/2 | Berserk (+attack speed) |
| Undead | Ghoul | 340 | 12-13 (Normal) | 0 (Medium) | 120/0/2 | Cannibalize (eat corpses for HP) |
| Undead | Crypt Fiend | 550 | 26-32 (Piercing) | 0 (Unarmored) | 215/40/3 | Web (ground air units) |
| Night Elf | Archer | 310 | 15-17 (Piercing) | 0 (Light) | 130/10/2 | Shadowmeld (invis at night) |
| Night Elf | Huntress | 600 | 16-18 (Normal) | 1 (Medium) | 195/20/3 | Sentinel (owl scout) |

- Roles: Worker, Melee, Ranged, Caster, Siege, Air, Detector.
- Upgrades: Attack/Armor +1-3 at forges.

**Unit AI and Behaviors**: Finite State Machine (FSM): Idle (standby animations), Move (pathfinding with formations), Attack (auto-target in range, prioritize counters), Harvest (workers auto-queue and return resources, flee at <20% HP). Edge cases: Stuck detection (repath every 2s); Group commands limit 12 units.

### Heroes
4 per race (3 base +1 TFT addition); Max level 10.
- Attributes: Strength (+19 HP, +1 dmg melee), Agility (+1 AS/20 pts, +1 dmg ranged), Intelligence (+15 mana, +1 dmg spells).
- Leveling: XP from kills (shared 600u radius); Curve: Lvl2=200, quadratic to Lvl10=~18k.
- Abilities: 3 levels each (unlock 1/3/5), Ultimate (Lvl6, 1 level).

Example Hero Breakdown:
- **Paladin (Human, Strength)**: Starting stats: Str 22, Agi 13, Int 17.
  - Holy Light (Lvl1/3/5): Heal 200/400/600 HP or dmg Undead; Mana 65, CD 5s.
  - Devotion Aura: +1/2/3 armor aura.
  - Divine Shield: Invuln 12/15/18s; Mana 25, CD 35s.
  - Resurrection (Ult): Revive 6 units at 100/60/30 HP; Mana 200, CD 240s.

Neutral Heroes: E.g., Beastmaster: Summons (Quillbeast, Bear, Hawk); Stampede Ult.

### Combat Mechanics
- **Damage Types**: Normal, Piercing, Siege, Magic, Chaos, Hero.
- **Armor Types**: Unarmored (100%), Light (150% Piercing), Medium (100%), Heavy (50% Piercing), Fortified (150% Siege), Hero (varies).
- **Formula**: Reduction = (armor * 0.06) / (1 + armor * 0.06); Effective = base * type_mult * (1 - red).
- **Pathfinding**: NavMesh; Formations to avoid clumping.
- **Day/Night**: Reduced sight 50% at night; Night Elf regen +50%.
- **Fog of War and Scouting**: Unexplored areas black; Explored grayed; Unit sight radii (ground: 800u, air: 1200u); Dynamic reveal with detectors for invis.
- **Environmental Interactions**: High ground +50% ranged dmg (TFT-style); Forests provide cover (+25% evasion); Destructible trees update pathing.

### Items and Neutrals
- **Items**: Consumables (potions), Permanent (Claws +3/6/9 atk), Charged (wands). Drops: 20% from creeps; Shops: Rotating at Marketplace (refresh 420s).
- **Creeps**: Levels 1-10; XP 80-800; Gold 10-50; Respawn 120-300s.
- **Neutrals**: Tavern hires; Merc camps; Fountains for regen.

## 6. Progression and Levels
### Campaigns
- Structure: Linear with branches (e.g., choice affects allies).
- Mission Types: Base-building, Hero RPG, Puzzle (e.g., activate runes).
- Progression: Unlock races/heroes; Carryover hero levels/items.

### Multiplayer Maps
- Sizes: Small (1v1), Large (4v4); Resources balanced (e.g., 12k gold mines).

**Level Design Principles**: Symmetric layouts for fairness; Choke points for defensive strategies; Creep camps scaled by distance (easy near start, hard for expansions); Procedural elements for replayability (e.g., random resource nodes).

**Replay System**: Save and replay battles with timeline scrubber, camera controls, and stats overlay for analysis.

**Achievements/Unlocks**: In-game rewards (e.g., "Creep Slayer: Farm 100 creeps in one match" unlocks hero skins; Campaign completion unlocks neutral heroes for skirmish).

## 7. UI and Controls
- **HUD**: Top: Resources/time; Bottom: Command card (abilities/hotkeys); Minimap (clickable).
- **Controls**:
  - Mouse: Select (drag-box), Command (right-click).
  - Hotkeys: Build (B then sub, e.g., B-F Farm); Groups (Ctrl+1-0).
  - Accessibility: Remappable; Colorblind modes.
- **Tooltips**: Detailed (e.g., "Holy Light: Heals 200 HP, damages Undead").

**Wireframes/Mockups** (Text Descriptions):
- Main Menu: Centered buttons for New Game, Load, Options, Quit; Background art of frozen battlefield.
- In-Game HUD: Top bar (gold/lumber/food icons with counters); Bottom panel (unit portraits, ability grid); Minimap in corner with ping functionality.

**Feedback Systems**: Visual cues (e.g., red flash on low HP units); Audio pings for minimap alerts; Haptic simulation via sound for ability casts.

## 8. Art and Audio
### Art Style
- Visuals: Isometric hybrid 2D/3D; Reforged fidelity (high-poly units 5k-10k tris, 4K textures with normals/specular).
- Assets: AI-generated (Rosebud AI, Layer AI, Stable Diffusion with Reforged LoRAs).
- Effects: Ray-traced spells (VFX Graph); Volumetric fog for night.
- **Mood Board References**: Unit designs: Stylized realism like Reforged orcs (detailed scars, metallic armor); Color palettes: Cool blues/grays for Undead, earthy greens/browns for Night Elves; Inspirations: Frozen Throne cinematics for epic scale.

**Animation States**: Per unit/hero: Idle (breathing loop, occasional fidgets); Walk (directional blends); Attack (wind-up, impact particles); Death (ragdoll with decay timer); Special (e.g., uproot for Ancients with root-pulling VFX).

### Audio
- SFX: Spatial (FMOD); Unit responses (AI-generated voices).
- Music: Orchestral (day epic, night ambient); AI-composed (AIVA.ai).
- **Soundtrack Tracklist Examples**: "FrostRealm Main Theme" (epic orchestral with choir); "Battle Fury" (intense percussion for combat); "Night Whisper" (ambient flutes for stealth); Default volumes: SFX 70%, Music 50%, Voice 80%.

## 9. Technical Specifications
- **Engine**: Unity 6.1 LTS; HDRP for raytracing.
- **Performance**: ECS/DOTS for entities; LOD auto-generation.
- **Networking**: Netcode for multiplayer.
- **Modding**: JSON maps; Exposed APIs.

**Hardware Requirements**:
- Minimum: GTX 1650, 8GB RAM, Intel Core i5 (for 1080p/30 FPS without raytracing).
- Recommended: RTX 3060, 16GB RAM, Intel Core i7 (for 4K/60 FPS with full raytracing).

**Optimization Strategies**: RTS-specific batching for units; Dynamic resolution scaling; Profiler-guided (e.g., <16ms/frame for entity updates); Hybrid ECS/GameObject for non-critical elements.

## 10. Balancing and Testing
- **Balance**: Mirror TFT meta (e.g., Human fast expand viable); Data-driven ScriptableObjects.
- **Testing**: Unit tests for formulas; Playtesting for APM balance.

**Balance Metrics**: Win rates per race <55%; Average game time 30 min; Hero kills decide 70% of outcomes; Economy curves tracked (e.g., gold/minute peaks at mid-game).

**QA Plan**: Alpha (mechanics testing via unit tests); Beta (playtesting with AI simulations for balance); Final (multiplayer stress tests for desyncs).

**Risks and Mitigations**:
- Risk: Imbalanced races; Mitigation: Iterative playtesting with data logs.
- Risk: Performance dips in large battles; Mitigation: ECS profiling and hardware tiers.
- Risk: Narrative bugs in branching campaigns; Mitigation: Automated trigger tests.

## 11. Marketing and Community
- **Target Release**: Steam/Itch.io for PC distribution.
- **Marketing Overview**: Teasers on X (Twitter) and Reddit showcasing gameplay clips; Discord server for community feedback; Trailers emphasizing TFT fidelity with modern twists.
- **Community Engagement**: Modding tools to encourage user-generated maps; Beta invites for playtesters.

## 12. Localization
- **Plan**: English primary; String tables for easy addition of languages (e.g., French, German, Spanish); AI-assisted translation via free tools like DeepL for initial drafts.

## 13. Post-Launch Roadmap
- **Phase 1 (Month 1-3)**: Bug fixes, balance patches based on player data.
- **Phase 2 (Month 4-6)**: DLC additions (e.g., new neutral hero or campaign extension).
- **Phase 3 (Ongoing)**: Community events, mod support updates.

## 14. Team and Workflow
- **As Solo Dev**: Use Cursor/Claude for code generation/iteration; AI tools for assets; Git for version control.
- **Workflow**: Prototype mechanics (e.g., resources first); Weekly milestones (e.g., Week 1: Setup Unity project); Daily Claude prompts referencing GDD/TechSpecs.

## 15. Budget and Timeline
- **Budget**: Zero-cost (free Unity tier, open-source libs, AI generators).
- **Timeline**: Prototype: 3 months (core mechanics); Full release: 12 months (campaigns, polish); Milestones: Month 1: Resource/Unit systems; Month 3: First playable mission.

## 16. Concept Art and Prototypes
- **Concept Art**: AI-generated placeholders (e.g., hero portraits via Leonardo.ai; Unit mockups via Stable Diffusion).
- **Prototypes**: Early builds focus on core loop; Link to tech doc for asset prompts.

## Appendices
- **Appendix A: Full Unit/Hero Tables**: [Expand all 12-15 units per race with full stats, abilities; E.g., Human Knight: HP 800, Damage 24-30 Normal, Armor 5 Heavy, Cost 245/60/4, Charge ability.]
- **Appendix B: Ability Details**: All spells with mana costs, cooldowns, ranges (e.g., Holy Light: Range 800u, Targets friendly/Undead).
- **Glossary**: TFT terms (e.g., Blight: Undead build area; Upkeep: Income penalty at high population).