# Technical Specifications for "FrostRealm Chronicles" - RTS Game in Unity

## Overview
This document provides comprehensive technical details for developing "FrostRealm Chronicles," a real-time strategy (RTS) game inspired by Warcraft III: The Frozen Throne (TFT). It is designed for reference in AI tools like Claude and Cursor, enabling code generation, asset creation, and implementation. All specifications align with TFT mechanics (e.g., damage formulas, hero scaling, upkeep systems) and target graphics quality equivalent to Warcraft III: Reforged (high-poly models with 4K textures, baked normals, ray-traced effects).

The game uses Unity 6.1 LTS (released July 2, 2025), leveraging new features like in-editor LOD generation for optimized models, swappable physics backends (e.g., PhysX or Havok for large battles), integrated generative AI tools for asset prototyping, enhanced UI Toolkit for dynamic HUDs, and improved ECS/DOTS for handling 500+ entities at 60 FPS. Development is free-tier focused: Unity Personal Edition, open-source libraries, and free AI tools for assets.

Store this document in your project root (e.g., as `TechSpecs.md`) for Claude/Cursor prompts, e.g., "Implement [module] in Unity C# using these specs: [paste section]."

## Engine and Setup
- **Unity Version**: 6.1 LTS (July 2025 release). Key new features:
  - In-editor LOD generation: Auto-create LOD meshes for units/buildings to optimize distant views.
  - Swappable physics backends: Use PhysX for default; Switch to Havok for high-entity collisions in RTS battles.
  - Generative AI integration: Built-in tools for texture/mesh prototyping (e.g., text-to-texture via Unity AI hub).
  - Enhanced ECS/DOTS: Entities 1.2+ with better animation support for unit states.
  - UI Toolkit milestones (Feb 2025): Streamlined customization for RTS HUD (e.g., dynamic command cards).
  - Roadmap previews: Unity 6.1 includes shading/rendering updates (e.g., better volumetric fog for day/night), animation tools, and world-building procedural generation for maps.
- **Render Pipeline**: High Definition Render Pipeline (HDRP) for Reforged-quality visuals: Raytracing for dynamic shadows/reflections (e.g., hero auras), volumetric lighting for spells, adaptive probe volumes for global illumination in battles.
  - Fallback: Universal Render Pipeline (URP) for mid-range hardware via graphics tiers.
- **Project Setup**:
  - New Project: HDRP template.
  - Packages (via Package Manager, free): Entities (for ECS), Netcode for GameObjects/Entities, Input System, UI Toolkit, Visual Scripting (for prototyping), VFX Graph, Shader Graph.
  - Open-Source: A* Pathfinding Project (free tier for NavMesh enhancements).
  - Performance Targets: 60 FPS on RTX 3060/16GB RAM with raytracing; Scale to GTX 1650 via LOD and reduced effects.
  - Build Settings: PC standalone (Windows/macOS); IL2CPP for faster runtime.
- **Hardware Requirements**:
  - Minimum: GTX 1650, 8GB RAM, Intel Core i5 (1080p/30 FPS without raytracing).
  - Recommended: RTX 3060, 16GB RAM, Intel Core i7 (4K/60 FPS with full raytracing).

**Optimization Strategies**: RTS-specific: GPU instancing for units; Dynamic resolution scaling; Profiler-guided tweaks (target <16ms/frame for entity updates); Hybrid ECS/GameObject approach (ECS for performance-critical like combat, GameObjects for static UI).

## Core Systems Implementation
### Input System
- Use Unity Input System package.
- Actions: Mouse (left: select/drag-box; right: command/move/attack); Keyboard hotkeys mirroring TFT (e.g., A: Attack, M: Move, B: Build menu, QWER: Abilities).
- Raycasting: From Camera.main (orthographic isometric view, 45-degree angle) for clicks; Layer masks for terrain/units/buildings.
- Edge Cases: Invalid commands (e.g., build on occupied tile: Play error SFX, show tooltip "Cannot build here").

**Gamepad Support**: Map analog sticks for camera pan; Buttons for abilities (e.g., A/B/X/Y for QWER).

### Pathfinding and Movement
- NavMeshAgent for units; Enhanced with A* for custom formations (e.g., line/box to avoid clumping like TFT).
- Unity 6.1 Features: Dynamic NavMesh baking for destructible environments (e.g., chopped trees update paths).
- Group Movement: Select up to 12 units (TFT limit); Command queueing (Shift for multiples).
- Collision: Sizes (small: 0.5u, large: 2u); Physics backend swap if bottlenecks occur.

**Environmental Pathing**: High ground bonuses (+50% ranged dmg); Forest cover (obstacle layers for evasion paths).

### Entity Management (ECS/DOTS)
- Use Entities package for performance: Convert GameObjects to entities for units/heroes.
- Components: Health (float HP, regen rate), Movement (NavMeshAgent ref, speed), Attack (damage, type, range, cooldown).
- Systems: UpdateJob for parallel processing (e.g., damage calculations in bursts).
- New in 6.1: ECS-based animation for states (idle, walk, attack) with better blending.

**Hybrid Integration**: Authoring components for conversion (e.g., MonoBehaviour to IComponentData); Burst-compiled jobs for math-heavy ops like damage calcs.

### Resource System
- Singleton Manager (ScriptableObject): Track gold, lumber, food.
- Formulas (TFT-exact):
  - Gold Mining: 8-12g per worker trip; Diminishing after 5 workers/mine.
  - Lumber Harvest: 10 per chop; Trees: 200-400 capacity.
  - Upkeep: Income multiplier = 1 (0-50 food), 0.7 (51-80), 0.4 (81-100). Max food: 100.
- Events: OnResourceChanged for UI updates.

**Economy Integration**: Tie to building costs (e.g., refund 75% on cancel); Worker AI for auto-harvest (FSM states: Gather, Return, Idle).

### Building System
- Placement: Grid snap (1x1 to 4x4 tiles); Collision check (Physics.OverlapBox); Ghost preview with material shader (green/red validity).
- Construction: Coroutine timer (e.g., 60s for Town Hall); Worker assignment (repair at 50% speed).
- TFT Mechanics: Undead blight spread (procedural texture overlay); Night Elf uproot (switch to mobile entity).

**Tech Tree Logic**: Prerequisite checks (e.g., Tier 2 requires Keep upgrade); Visual morphs via mesh swaps.

### Unit and Hero System
- Data: ScriptableObjects for stats (e.g., Footman: HP 420 base, Damage 12-13 Normal, Armor 2 Heavy).
- AI: Finite State Machine (FSM) via ECS (Idle → Move → Attack → Harvest).
- Heroes: Attribute scaling (Strength: +19 HP, +1 dmg; Agility: +1 AS/20 pts, +1 dmg; Intelligence: +15 mana, +1 dmg).
  - Leveling: XP curve quadratic (200 for lvl2, ~18k for lvl10); Shared in 600u radius.
  - Inventory: 6 slots (UI grid); Items (e.g., Claws +3 atk).
- Damage Calc: Effective = base * type_mult * (1 - red); Red = armor * 0.06 / (1 + armor * 0.06).

**Unit Variants**: Gender-swaps via alternate meshes; Upgrades apply material changes (e.g., +armor glow).

### Combat System
- Attack: Raycast/overlap for range; Projectile pooling (Rigidbody for arcs).
- Types/Mults: Normal (100% Med, 150% Unarm), etc., per TFT.
- Abilities: ScriptableObjects (e.g., Holy Light: Target heal 200/400/600, mana 65, CD 5s).
- VFX: VFX Graph for particles (ray-traced glows); Shader Graph for auras.

**Fog of War**: Compute Shader for dynamic reveal; Sight radii integrated with ECS queries.

### UI System
- UI Toolkit: Runtime UI for HUD (resources top, minimap bottom-left, command card bottom-right).
- Minimap: RenderTexture with ray-traced previews; Click navigation.
- Tooltips: Dynamic panels showing formulas (e.g., "Piercing: 150% vs Light").

**Wireframes**: HUD scalable; Options menu with sliders for volume/graphics.

### Networking
- Netcode for Entities: Client-side prediction for movement; Server-authoritative damage.
- New in 6.1: Host Migration and Distributed Authority for multiplayer stability.
- Modes: LAN/Online (Vivox free SDK for voice).

**Desync Handling**: Reconciliation for positions; RPCs for ability sync.

### Audio System
- FMOD integration (free tier): Spatial SFX (unit attacks directional); Dynamic BGM (day/night shifts).
- Voice Lines: AI-generated variants for units (e.g., "Ready to work!" for peasants).

**Audio Mixing**: Layers for SFX/Music/Voice; Adaptive music based on game state (e.g., tension build-up).

## Asset Pipeline and Generation
- **Style**: Reforged emulation: High-poly models (5k-10k tris base, LOD to 1k); 4K textures (albedo, normal, specular); Baked details for cloth/fur.
- **Free AI Tools (2025 Best Practices)**:
  - **Rosebud AI**: All-in-one for 2D/3D assets (Prompt: "Generate Reforged-style orc grunt 3D model: high-poly with baked normals, 4K textures, animations (idle, walk, attack), isometric optimized").
  - **Layer AI**: Style-consistent generation (free starter); For environments/units (Prompt: "Warcraft Reforged human footman spritesheet: 64x64 upscaled to 4K, animations 8 frames each, variants").
  - **Scenario.ai**: Customizable high-quality assets (Prompt: "3D night elf archer model, Reforged fidelity, UV unwrapped for HDRP").
  - **3DFY.AI**: Text-to-3D (Prompt: "Undead ghoul asset like Reforged: high-poly sculpted, baked details").
  - **Stable Diffusion (Local/Civitai LoRAs)**: Free with "warcraft reforged LoRA" (Prompt: "sdxl, warcraft 3 reforged paladin hero portrait, 4K detailed").
  - **Leonardo.ai**: Free credits for icons/portraits (Prompt: "Fantasy ability icon: holy light, Reforged style").
  - **Meshy**: 3D from text (Prompt: "Generate building model: human town hall, Reforged quality").
  - **Playground AI / Adobe Firefly**: Quick 2D tests (free tiers).
  - Unity Integration: Use 6.1's generative AI hub to refine (e.g., text-to-texture on imported meshes).
- **Process**:
  1. AI Generate: High-poly model/texture via prompt.
  2. Import: FBX/PNG to Unity; Auto-LOD via in-editor tool.
  3. Optimize: Bake normals; ECS rig for animations.
  4. Variants: AI batch for upgrades/genders.

**Consistency Guidelines**: Unified style guide (e.g., color palette from Reforged); Batch processing for race-specific themes.

## Testing and Optimization
- Unity Test Framework: Unit tests for formulas (e.g., damage calc assertions).
- Profiler: Target <16ms/frame; Use 6.1's large-scale physics for 500+ units.
- Error Handling: Logs with Unity Cloud Diagnostics (free); Runtime validations.

**QA Plan**: Unit tests for systems; Integration tests for mechanics (e.g., resource+combat); Playtesting for balance; Stress tests for multiplayer.

**Risks and Mitigations**:
- Risk: ECS learning curve; Mitigation: Claude prompts for conversions.
- Risk: Asset import issues; Mitigation: Automated baking scripts.

This spec ensures TFT fidelity with modern Unity enhancements. Reference sections in prompts for targeted implementation.