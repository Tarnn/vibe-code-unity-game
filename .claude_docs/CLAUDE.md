# Claude.md - Guidelines for Using Claude in Cursor for FrostRealm Chronicles Development

This document serves as a reference for leveraging Claude (via Cursor's AI integration) in the development of "FrostRealm Chronicles," a real-time strategy (RTS) game built in Unity 6.1 LTS, inspired by Warcraft III: The Frozen Throne. As a solo engineer using AI tools, you'll use Claude for code generation, debugging, asset prompt creation, and iterative refinement.

Cursor integrates Claude Sonnet 3.5 (or Opus if available) for context-aware coding. Copy-paste prompts from this file into Cursor's chat or inline editor (e.g., Ctrl+L for chat), referencing project files like `GDD.md`, `TechSpecs.md`, `PRD.md`. Always include relevant sections from those docs in your prompts for accuracy.

## Project Overview
- **Game Summary**: RTS with four asymmetric races (Human, Orc, Undead, Night Elf), hero leveling, resource management (gold/lumber/food with upkeep), base-building, and campaigns/multiplayer. Mechanics mirror Frozen Throne (e.g., damage formulas: reduction = armor * 0.06 / (1 + armor * 0.06)).
- **Tech Stack**: Unity 6.1 LTS, HDRP for ray-traced graphics (Reforged-quality assets), ECS/DOTS for performance, Netcode for multiplayer.
- **Development Goals**: Free tools only; AI-generated assets (e.g., Rosebud AI for models); Prototype core mechanics first (resources, units, heroes).
- **Key Files in Project**:
  - `GDD.md`: Game Design Document (mechanics, units, heroes, balancing).
  - `TechSpecs.md`: Technical details (systems, asset pipeline, optimization).
  - `PRD.md`: Product Requirements (features, scope, risks).
- **Best Practices**:
  - Provide context: Always paste relevant GDD/TechSpecs sections in prompts.
  - Iterate: Generate code, test in Unity, then prompt for fixes (e.g., "Debug this script: [paste error]").
  - Specificity: Use exact TFT formulas/mechanics for fidelity.
  - File Structure: Organize scripts in folders (e.g., `Assets/Scripts/Core/ResourceSystem.cs`).
  - Version Control: Use Git; Commit after each Claude-generated module.

## Prompt Templates
Use these as bases. Replace [placeholders] with specifics. Aim for prompts under 2000 tokens for efficiency.

### 1. Generating a New Script/Module
**Template**:
```
You are Claude, assisting in Unity development for FrostRealm Chronicles, an RTS inspired by Warcraft III: The Frozen Throne.

Project Context: [Paste brief overview from GDD/TechSpecs, e.g., "Use ECS/DOTS for entities; HDRP for visuals."]

Specific Task: Implement [module name, e.g., "Resource Management System"] in C# for Unity 6.1 LTS.

Requirements from GDD/TechSpecs:
- [Paste relevant sections, e.g., "Resources: Gold (mined 8-12g/trip), Lumber (10/chop), Food (upkeep penalties: 70% at 51-80). Singleton manager with events."]
- Formulas: [e.g., "Upkeep multiplier = 1 (0-50), 0.7 (51-80), 0.4 (81-100)."]
- Integration: [e.g., "Tie to UI HUD; Use ScriptableObjects for data."]

Output: Complete C# script(s) with comments, namespaces, and error handling. No explanations outside code unless asked.
```

**Example Use**: For resource system, paste into Cursor chat.

### 2. Refining/Debugging Existing Code
**Template**:
```
Debug and refine this Unity C# code for FrostRealm Chronicles.

Context: [Paste GDD/TechSpecs excerpt, e.g., "Pathfinding uses NavMeshAgent with A* enhancements for formations."]

Current Code:
```csharp
[Paste code here]
```

Issues/Improvements: [e.g., "Fix NullReferenceException on NavMesh; Optimize for 500+ units using ECS; Add TFT-style collision avoidance."]

Output: Revised full script with changes highlighted in comments (e.g., // Claude: Added ECS conversion).
```

**Tip**: Use Cursor's "Apply" button to insert fixes directly.

### 3. Generating AI Asset Prompts
**Template**:
```
Create prompts for AI asset generation tools (e.g., Rosebud AI, Stable Diffusion) for FrostRealm Chronicles assets, matching Warcraft III Reforged quality.

Asset Type: [e.g., "3D model for Orc Grunt unit."]

Specs from GDD/TechSpecs:
- Style: High-poly (5k-10k tris), 4K textures (albedo/normal/specular), isometric optimized.
- Details: [e.g., "Animations: Idle (breathing), Walk (8 directions), Attack (melee swing). Variants: Base, armored upgrade, gender-swap."]
- Tool-Specific: [e.g., "For Rosebud AI: Text-to-3D prompt; Include Reforged LoRA if applicable."]

Output: 3-5 variant prompts, ready to copy-paste into the tool.
```

**Example Output Prompt**: "Generate Reforged-style orc grunt 3D model: high-poly with baked normals, 4K textures, animations (idle, walk, attack), isometric view."

### 4. Creating Unit/Hero Data (ScriptableObjects)
**Template**:
```
Generate C# ScriptableObject classes and sample assets for [e.g., "Human units and heroes"] in Unity for FrostRealm Chronicles.

From GDD:
- Units: [Paste table excerpt, e.g., "Footman: HP 420, Damage 12-13 Normal, Armor 2 Heavy."]
- Heroes: [e.g., "Paladin: Strength primary; Abilities: Holy Light (heal 200/400/600), etc."]
- Structure: Base class UnitData : ScriptableObject { float hp; string damageType; etc. }

Output: C# code for classes, plus JSON-like sample data for instantiation.
```

### 5. Implementing Gameplay Features (e.g., Combat, UI)
**Template**:
```
Implement [feature, e.g., "Combat Damage Calculation"] in Unity C# for FrostRealm Chronicles.

Context from TechSpecs/GDD:
- Formula: Effective damage = base * type_multiplier * (1 - reduction); Reduction = armor * 0.06 / (1 + armor * 0.06).
- Types: [List: Normal 100% vs Medium, 150% vs Unarmored, etc.]
- Integration: ECS component for entities; VFX for hits.

Output: Complete method/script, with unit test snippets.
```

### 6. Optimizing Performance
**Template**:
```
Optimize this Unity code for RTS performance in FrostRealm Chronicles (target 60 FPS with 500+ units).

Code:
```csharp
[Paste code]
```

Guidelines from TechSpecs: Use ECS/DOTS jobs; LOD auto-generation; Batching; Profiler targets <16ms/frame.

Output: Optimized version with explanations in comments.
```

### 7. Generating Campaign/Mission Scripts
**Template**:
```
Script a campaign mission for FrostRealm Chronicles using Unity Timeline and C# triggers.

Mission Details from GDD: [e.g., "Human Campaign Mission 1: Base defense against waves; Objectives: Survive 10 min, build 5 farms."]
- Events: Timed enemy spawns, cutscenes, win/lose conditions.

Output: C# MissionManager script; Timeline setup description.
```

### 8. Multiplayer/Netcode Implementation
**Template**:
```
Add multiplayer features to this code using Unity Netcode for FrostRealm Chronicles.

Context: Client-side prediction for movement; Server-authoritative damage.

Code:
```csharp
[Paste base script]
```

Output: Updated script with NetworkBehaviour, RPCs, and Host Migration (Unity 6.1 feature).
```

## Advanced Tips for Cursor + Claude
- **Context Window Management**: Cursor handles large contexts; Prefix prompts with "Refer to attached files: GDD.md, TechSpecs.md" if uploaded.
- **Inline Editing**: Select code in editor, hit Ctrl+K, paste prompt like "Refactor this for ECS compatibility."
- **Testing**: After generation, prompt "Write Unity Test Framework tests for this script."
- **Versioning Prompts**: Save successful prompts here for reuse.
- **Limitations**: Claude can't run code; Use Unity Editor for testing. If stuck, prompt "Suggest debugging steps for [error]."
- **Ethical/Style Notes**: Ensure code is clean, commented, follows C# conventions. Avoid paid assets; Stick to free Unity packages.

Update this file as development progresses (e.g., add new templates for modding). Happy coding!