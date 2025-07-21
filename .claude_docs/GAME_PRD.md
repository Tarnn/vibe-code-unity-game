# Expanded Product Requirements Document (PRD) and Technical Plan for "FrostRealm Chronicles" - An RTS Game Inspired by Warcraft III: The Frozen Throne

## Executive Summary
"FrostRealm Chronicles" is a real-time strategy (RTS) game that deeply emulates Warcraft III: The Frozen Throne (TFT), incorporating its expanded mechanics from Reign of Chaos, such as new heroes per race, neutral heroes via Taverns, rotating item Marketplaces, naval/amphibious elements in campaigns, and refined resource costs with higher lumber demands for high-tech units. Built in Unity 6.1 LTS (latest as of July 2025), it uses HDRP for ray-traced graphics matching the quality of Warcraft III: Reforged—featuring 4K textures, high-poly models with baked details, unit variants (e.g., gender-swapped heroes), varied idle animations, and realistic lighting effects like dynamic shadows and volumetric fog. As a solo developer using AI, all tools are free: Unity free tier, open-source libraries, and AI generators for assets at Reforged-level fidelity (e.g., high-resolution sprites/models with stylized yet realistic textures).

This PRD expands previous versions with TFT-specific details: exact damage reduction formulas (e.g., physical damage reduced by ~6% per armor point), hero attribute scaling (e.g., +1 damage per primary stat point), unit stats tables, ability breakdowns, tech tree paths, creep camp rules (e.g., level-based XP/gold drops), and upkeep penalties (70% income at 51-80 food, 40% at 81-100). Graphics pipeline replicates Reforged's upgrades: remodeled units from concept to sculpted high-poly, then optimized for isometric RTS view. Asset generation uses 2025 AI tools trained on Reforged-style prompts for consistency. Structured for AI coding prompts.

## Product Vision and Scope
- **Genre and Inspiration**: RTS with TFT's hero-focused asymmetry—e.g., heroes gain +1 damage per primary attribute point (Strength: melee heroes get 2d6 base dice; Agility: 2d12; Intelligence: 2d4 ranged), level up via shared XP in radius (curve: level 2 needs 200 XP, scaling quadratically to level 10 at ~18,000 XP). Differentiate with Reforged-quality visuals: 4K support, ray-traced effects for spells (e.g., glowing Val'kyr in resurrection animations).
- **Target Platforms**: PC primary; Optimize for RTX hardware to enable raytracing. Free development ensures no costs.
- **Core Differentiators**: TFT fidelity in mechanics (e.g., chaos damage at 100% vs all armors); Reforged graphics via AI (high-poly variants, baked normals for detail retention).
- **Out of Scope**: Paid features; Full naval in multiplayer (limit to campaign amphibious like TFT's select missions).
- **Success Metrics**: Match TFT balance (e.g., unit counters: piercing vs light armor at 150%); 60 FPS with raytracing on RTX 3060.

## Key Features
### Gameplay Features
1. **Races and Factions**:
   - **Human Alliance**: Tech tree: Town Hall → Keep (Tier 2, unlocks advanced) → Castle (Tier 3). Units stats (base/max after upgrades): Footman (HP 420/585, Damage 12-13/15-16 Normal, Armor 2/5 Heavy, Cost 135g/2f). Heroes: Paladin (Strength primary: +2.5 HP, +1 dmg per point; Abilities: Holy Light heals 200/400/600 HP, costs 65 mana; Devotion Aura +1/2/3 armor; Divine Shield invuln 12/15/18s; Resurrection revives 6 units at 100/60/30 HP).
   - **Orc Horde**: Great Hall → Stronghold → Fortress. Grunt (HP 700/805, Damage 19-21/22-24 Normal, Armor 1/4 Medium, Cost 200g/3f). Heroes: Blademaster (Agility: +1 attack speed/20 points; Wind Walk invis +40/60/80% speed, dmg bonus on exit; Mirror Image 3 illusions at 0/15/30% dmg; Critical Strike 15% chance 2/3/4x dmg; Bladestorm 40/50/60 DPS whirl).
   - **Undead Scourge**: Necropolis → Halls of the Dead → Black Citadel. Ghoul (HP 340/400, Damage 12-13/15-16 Normal, Armor 0/3 Medium, Cost 120g/2f, harvests lumber). Heroes: Death Knight (Strength; Death Coil 100/200/300 dmg/heal; Death Pact kill ally for 150/300/450 HP; Unholy Aura +10/20/30% speed; Animate Dead raise 6 invincible units 40s).
   - **Night Elf Sentinels**: Tree of Life → Ages → Eternity. Archer (HP 310/370, Damage 15-17/18-20 Piercing, Armor 0/3 Light, Cost 130g/10l/2f). Heroes: Demon Hunter (Agility; Immolation 10/20/30 DPS aura; Mana Burn 50/100/150 mana dmg; Evasion 10/20/30% dodge; Metamorphosis demon form +500 HP, chaos dmg).
   - **Neutral/New**: Naga (campaign/custom: Sea Witch hero with Forked Lightning 85/150/225 dmg chain); Neutral heroes via Tavern (e.g., Dark Ranger: Silence 140/210/280 mana drain AoE; Black Arrow +20/40/60 dmg curse).
   - Variants: Like Reforged, include gender-swaps (e.g., female Footman) for diversity.

2. **Resource Management**:
   - Gold: Mined at 8-12g per trip (diminishing after 5 workers); Mines hold 5000-30000g.
   - Lumber: 10 per chop; Trees hold 200-400, regrow in 60-120s.
   - Food: Max 100; Upkeep: 0-50: 100% income; 51-80: 70%; 81-100: 40% (formula: income = base * (1 - penalty), penalty=0.3/0.6).
   - TFT Additions: Higher lumber for Tier 3 (e.g., Gryphon Aviary 150g/200l); Non-hero inventory via Orb items (carry 1-3).

3. **Base Building and Units**:
   - Placement: Blight for Undead (spreads from buildings); Uproot for Night Elf Ancients (move/fight, HP 900-1200).
   - Upgrades: +1/2/3 attack (e.g., Iron Forged Swords +1 Normal dmg); Armor +1/2/3.
   - Counters: Normal 100% vs Medium, 150% vs Unarmored; Piercing 150% vs Light, 50% vs Heavy; Siege 150% vs Fortified, 50% vs Unarmored.

4. **Combat and Mechanics**:
   - **Damage Formulas**: Physical reduction = (armor * 0.06) / (1 + armor * 0.06) (e.g., 5 armor ~23% reduction); Magic/Chaos ignore partial; Hero dmg: base + attribute + dice (e.g., Strength hero: 2d6 + str).
   - Attack Types: As TFT (e.g., Gargoyle: Piercing ground, Normal air).
   - Day/Night: 4min day/2min night; Sight reduced 50% night; Night Elf +50% regen, Shadowmeld.
   - Creeps: Levels 1-10; Drops: Gold 10-50, items 20% chance (e.g., Potion of Healing); Respawn 120-300s; XP shared in 600 range.

5. **Hero System**:
   - Attributes: +1 dmg primary; Strength +19 HP; Agility +1 AS/20; Intelligence +15 mana.
   - Leveling: Max 10; Abilities 3 levels each (unlock 1/3/5), Ultimate 1 level (6).
   - TFT: New heroes (e.g., Human Blood Mage: Flame Strike 35/50/65 DPS pillar); Neutral (e.g., Pandaren Brewmaster: Breath of Fire 65/130/195 cone).
   - Inventory: 6 slots; Drops on death (recoverable); Revival: Timer = 7*level s, cost 400+10*level g.

6. **Single-Player Campaigns**:
   - Mirror TFT: 4 arcs with branching (e.g., Illidan's betrayal choices); Missions blend base/RPG (e.g., naval in Terror of the Tides).

7. **Multiplayer**:
   - TFT Maps: Island with Goblin Shipyard for transports (cost 200g/100l).

8. **UI/UX**:
   - TFT-Style: Command card with ability levels; Tooltips show formulas (e.g., "Reduces dmg by ~6% per armor").

### Additional Features
9. **Neutral Elements Depth**: Marketplace: 9 slots, refresh 420s, items like Claws +3/6/9/12 atk (costs 150-400g).
10. **Audio/Effects**: Reforged-like: Varied unit voices (AI-generated variants); Ray-traced spell VFX (e.g., reflective frost in Lich's Nova).

## Gameplay Mechanics Breakdown
### Core Loop
- TFT Fidelity: Early creep farm for hero XP (e.g., level 1 creep: 80 XP); Mid-game expansions with upkeep management.

### Detailed Mechanics
- **Combat Calc**: Effective dmg = base * type_multiplier * (1 - reduction); E.g., 20 Piercing vs 5 Light armor (150% type): 30 * (1-0.23) = 23.1.
- **Edge Cases**: Mana burn on 0 mana: No dmg; Illusion dmg 0-30% based on level.

## Technical Architecture
- **Graphics**: HDRP with raytracing for Reforged effects (e.g., adaptive probes for GI in battles); Models: High-poly sculpt → bake to low-poly (1000-5000 tris/unit).

### Asset Pipeline
- **Style**: Reforged emulation: Stylized realistic (e.g., detailed orc skin textures, variant murlocs); 4K textures, high-poly bases with normals/height maps.
- **Creation Recommendations** (Free 2025 Tools):
  - **Recraft AI**: Primary for 3D models/textures (Prompt: "Generate high-quality 3D orc grunt model like Warcraft 3 Reforged: high-poly with baked details, 4K textures, isometric optimized, variants including armored upgrade and gender-swap").
  - **Layer AI**: Free for 2D/3D assets (Prompt: "Create spritesheet for human footman in Reforged style: 64x64 pixels, animations (idle 8 frames, attack 6, walk 8 directions), 4K upscaled textures").
  - **Stable Diffusion (Local/Civitai LoRAs)**: Free with "warcraft reforged LoRA" (Prompt: "sdxl, warcraft 3 reforged night elf archer model, high-poly sculpted, baked normals, ray-trace ready").
  - **Tripo AI / ModelsLab 3DVerse**: For 3D generation (free tiers; Prompt: "Text-to-3D undead ghoul asset, Reforged quality with variants, UV unwrapped").
  - **Process**: AI generate high-poly → Auto-bake in Unity (Mesh LOD); Import as FBX/PNG; ECS animation for variants.
  - Consistency: Train on Reforged screenshots (public datasets); Manual tweaks in Blender (free).

## Risks and Mitigations
- **Mechanics Accuracy**: Validate vs TFT guides; AI code with formulas.
- **Asset Quality**: Iterate prompts; Compare to Reforged via free viewers.

This PRD enables full AI implementation with TFT depth and Reforged visuals.<grok:render card_id="74e9d9" card_type="citation_card" type="render_inline_citation">
<argument name="citation_id">0</argument>
</grok:render><grok:render card_id="cf454d" card_type="citation_card" type="render_inline_citation">
<argument name="citation_id">1</argument>
</grok:render><grok:render card_id="81fb40" card_type="citation_card" type="render_inline_citation">
<argument name="citation_id">2</argument>
</grok:render><grok:render card_id="0a273d" card_type="citation_card" type="render_inline_citation">
<argument name="citation_id">3</argument>
</grok:render><grok:render card_id="2cb6e3" card_type="citation_card" type="render_inline_citation">
<argument name="citation_id">4</argument>
</grok:render>