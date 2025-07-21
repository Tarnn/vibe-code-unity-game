# AI Asset Pipeline Guide for FrostRealm Chronicles

## Overview
This guide provides comprehensive workflows for generating Warcraft III: Reforged quality assets using free AI tools. It covers 3D models, textures, animations, UI elements, and audio generation with consistency guidelines for the RTS game FrostRealm Chronicles.

### Project Asset Directory
All hero and playable character assets (models, textures, animations, icons) **must** be stored in the repository at `assets/hero/`. Import automation and build scripts assume this location when generating LODs, assigning materials, and wiring up `HeroData` ScriptableObjects.

Example structure:
```
assets/hero/Paladin/Paladin_Base.fbx
assets/hero/Paladin/Paladin_Base_Albedo.png
assets/hero/Paladin/Paladin_Base_Normal.png
assets/hero/DemonHunter/DemonHunter_Base.fbx
```
Keep non-hero unit assets in their respective future folders (e.g., `assets/units/`).

## Art Style Guidelines

### Visual Target: Warcraft III Reforged Fidelity
- **Polygon Count**: 5k-10k tris for hero models, 2k-5k for units, 1k-3k for buildings
- **Texture Resolution**: 4K primary, 2K secondary, 1K for minor details
- **Style**: Stylized realism with exaggerated proportions
- **Lighting**: PBR materials with baked ambient occlusion
- **Effects**: Ray-traced compatible with volumetric elements

### Color Palette Standards
```
Human Alliance: 
- Primary: Royal Blue (#1E3A8A), Gold (#F59E0B)
- Accent: Silver (#9CA3AF), Red (#DC2626)

Orc Horde:
- Primary: Dark Red (#991B1B), Brown (#92400E) 
- Accent: Bone White (#F3F4F6), Orange (#EA580C)

Undead Scourge:
- Primary: Purple (#7C3AED), Dark Green (#166534)
- Accent: Bone White (#F3F4F6), Cyan (#0891B2)

Night Elf Sentinels:
- Primary: Forest Green (#166534), Silver (#6B7280)
- Accent: Purple (#7C3AED), Teal (#0D9488)
```

## AI Tool Configuration

### Primary Tools (Free Tier)
1. **Rosebud AI** - Primary 3D model generation
2. **Layer AI** - Texture and sprite generation  
3. **Leonardo.ai** - Concept art and icons
4. **Stable Diffusion (Local)** - High-quality texture work
5. **Scenario.ai** - Environment assets
6. **Meshy** - Alternative 3D generation
7. **ElevenLabs (Free)** - Voice generation
8. **AIVA.ai** - Music composition

### Quality Control Workflow
```
1. Generate → 2. Validate → 3. Optimize → 4. Integrate → 5. Test
```

## 3D Model Generation

### Unit Model Prompts

#### Human Footman
```
Primary Prompt (Rosebud AI):
"Generate high-quality 3D human footman model in Warcraft 3 Reforged style. Heavily armored medieval soldier with tabard, sword and shield. High-poly sculpted details (8k triangles), realistic proportions with slight stylization. Include variants: base, armored upgrade (+golden trim), gender-swap (female variant). 4K PBR textures with normal maps, metallic workflow. Isometric-optimized for RTS camera angle. Include LOD mesh at 2k triangles."

Secondary Refinement:
"Enhance model with: battle scars on armor, cloth physics simulation for tabard, proper UV unwrapping for texture painting. Add equipment variants: iron sword (base), steel sword (+1 upgrade), mithril sword (+2 upgrade). Ensure clean topology for animation rigging."
```

#### Orc Grunt  
```
Primary Prompt (Scenario.ai):
"Create 3D orc grunt warrior model matching Warcraft 3 Reforged quality. Brutish muscular orc with tribal armor, battle axe, tusks, and scars. 6k triangle count with detailed normal maps. Green skin with darker war paint, brown leather armor with bone/metal accents. Include upgrade variants: bone armor (base), steel armor (+spikes), champion armor (skull details). Female variant with similar proportions. Ready for Unity HDRP with PBR workflow."
```

#### Undead Ghoul
```
Primary Prompt (Meshy):
"Generate undead ghoul 3D model in Reforged style. Emaciated humanoid with exposed ribs, tattered flesh, glowing eyes. Hunched posture for quadrupedal movement capability. 4k triangles base, 8k for hero version. Decay variations: fresh corpse, rotting, skeletal. Purple/green necromantic glow effects. Include claws, torn clothing, chains. PBR textures with emissive materials for magical effects."
```

#### Night Elf Archer
```
Primary Prompt (Rosebud AI):
"Create night elf archer model in Warcraft 3 Reforged style. Elegant elven features with long ears, flowing hair, nature-themed armor made of leaves and leather. Carrying ornate bow and quiver. 5k triangles with detailed facial features. Purple/silver skin tone, silver hair, green/brown armor. Include variants: base leather armor, moonweave armor (+magical glow), ancient guardian armor (+nature effects). Both male and female versions with different hairstyles."
```

### Building Model Prompts

#### Human Town Hall
```
Primary Prompt (Scenario.ai):
"Generate Human Town Hall building for Warcraft 3 Reforged style RTS. Large medieval castle structure with blue roof tiles, stone walls, multiple towers. 3k triangles optimized for isometric view. Include upgrade variants: Town Hall (basic), Keep (additional walls/towers), Castle (massive fortress with flags). 4K texture resolution with weathering details, moss, battle damage. Modular design for easy variant creation."
```

### Hero Model Prompts

#### Paladin
```
Primary Prompt (Rosebud AI):
"Create epic Paladin hero model in Warcraft 3 Reforged style. Noble human warrior in gleaming plate armor with holy symbols, wielding blessed hammer/sword. 10k triangles for hero detail level. Include: base armor (silver/blue), blessed armor (+golden glow), avatar armor (+divine light effects). Cape physics, detailed facial features, customizable gender. Holy light particle attachment points. PBR materials with emissive holy energy."
```

## Texture Generation

### Material Creation Workflow

#### Base Texture Prompt
```
Primary Prompt (Layer AI):
"Generate 4K PBR texture set for [MATERIAL_TYPE] in Warcraft 3 Reforged style:
- Albedo: [Color description] with hand-painted details
- Normal: Surface detail enhancement, [specific features]
- Roughness: [Surface properties] variation
- Metallic: [Metallic areas] definition
- AO: Contact shadows and crevice darkening

Style: Stylized realism, high contrast, readable at distance. Include edge wear, dirt accumulation, [faction-specific weathering]."
```

#### Human Metal Armor
```
"Generate 4K PBR armor texture set for Human Alliance in Reforged style:
- Albedo: Polished steel base with royal blue accents, gold trim details
- Normal: Hammered metal surface, rivet details, battle damage
- Roughness: Polished metal (low) to cloth areas (high)
- Metallic: Full metallic on armor plates, non-metallic on leather straps
- AO: Deep shadows in armor joints and decorative engravings
Include variants: clean (new), battle-worn, ceremonial (+extra gold)"
```

### Environment Texture Generation

#### Terrain Textures
```
Primary Prompt (Stable Diffusion + Reforged LoRA):
"Generate tileable 2K terrain texture for RTS game in Warcraft style:
Type: [Grass/Stone/Snow/Blight]
Features: [Specific details like rocks, vegetation, damage]
Tiling: Seamless edges, organic randomness to avoid repetition
Variants: Base, damaged (battle aftermath), magical (affected by spells)
PBR workflow with subtle normal mapping for depth perception"
```

## Animation and Rigging

### Animation State Prompts

#### Unit Animation Set
```
Generate animation description for Unity Mechanim setup:

Unit: [Unit Name]
Required Animations:
- Idle: Breathing cycle (3s loop), occasional fidgets every 10s
- Walk: 8-directional movement, [specific gait for unit type]
- Attack: Wind-up, strike, recovery with weapon-specific timing
- Death: [Dramatic collapse/dissolution based on unit type]
- Special: [Unit-specific abilities, e.g., orc pillage, elf shadowmeld]

Technical: 30 FPS keyframes, root motion for movement, additive layers for breathing
Timing: Attack speed scales with TFT values (base 1.35s for most units)
```

#### Hero Animation Set
```
Extended animation set for heroes:

Basic: Idle, Walk, Attack, Death (as above)
Combat: Critical strike (+flourish), spell casting (channeling/instant)
Abilities: [Hero-specific animations per ability, e.g., Paladin holy light casting]
Social: Victory pose, taunt, retreat
Movement: Run (faster than walk), charge (for melee heroes)

Facial: Emotion states for dialogue/cutscenes
Props: Equipment interaction (weapon swapping, item usage)
```

## UI Asset Generation

### Interface Elements

#### HUD Icon Generation
```
Primary Prompt (Leonardo.ai):
"Generate UI icon for [ABILITY/ITEM] in Warcraft 3 Reforged style:
Size: 64x64 base, scalable to 256x256
Style: Hand-painted fantasy with strong silhouette
Colors: [Faction colors] with magical glow effects
Border: Ornate frame matching [faction aesthetic]
Background: Subtle texture, not distracting from main element
Variants: Normal, disabled (grayed), highlighted (+glow)

Examples:
- Holy Light: Golden hand with radiant energy
- Fireball: Orange sphere with flame wisps
- Health Potion: Red bottle with cork and label"
```

#### Resource Icons
```
"Generate resource counter icons for RTS HUD:
Set: Gold coin, Lumber log, Food/Population symbol
Style: Warcraft 3 Reforged hand-painted look
Size: 32x32 for HUD, 64x64 for menus
Features: Clear silhouette, faction-neutral colors
Gold: Detailed coin with fantasy markings
Lumber: Cross-section wood log with growth rings
Food: Stylized house/family symbol for population"
```

### Menu and UI Backgrounds

#### Main Menu Background
```
Primary Prompt (Scenario.ai):
"Create main menu background for FrostRealm Chronicles:
Scene: Epic fantasy battlefield with all four factions represented
Composition: Hero figures in foreground, armies clashing in background
Atmosphere: Dramatic lighting with volumetric fog, frozen landscape theme
Quality: 4K resolution for various screen sizes
Elements: Flying units overhead, magical effects, environmental storytelling
Style: Warcraft 3 cinematic quality, painted illustration style"
```

## Audio Asset Generation

### Voice Line Generation

#### Unit Voice Prompts
```
Primary Prompt (ElevenLabs):
"Generate voice lines for [UNIT_TYPE] in [FACTION]:

Personality: [Unit-specific traits, e.g., "Noble and dutiful" for Human Footman]
Voice Quality: [Age, accent, tone, e.g., "Young adult, slight British accent, confident"]

Required Lines:
- Selection: 3 variants ("Yes, milord?", "Ready for duty!", "At your command!")
- Movement: 3 variants ("Moving out!", "On my way!", "Right away!")
- Attack: 3 variants ("For the Alliance!", "Charge!", "Victory or death!")
- Death: 1-2 dramatic final words
- Building: 2 variants ("Work, work!", "Something need doing?")

Style: Match Warcraft 3 tone, memorable and quotable
Length: 1-3 seconds per line, clear pronunciation"
```

### Music Generation

#### Background Music Prompts
```
Primary Prompt (AIVA.ai):
"Compose orchestral music for RTS game in Warcraft style:

Track Type: [Menu/Battle/Exploration/Victory]
Mood: [Epic/Tense/Peaceful/Triumphant]
Duration: [2-5 minutes with loop points]
Instruments: Full orchestra with choir for epic moments
Style: Fantasy orchestral, Hans Zimmer influence for battles
Faction Theme: [Incorporate faction-specific instruments if needed]

Menu: Majestic and welcoming, building anticipation
Battle: Intense percussion, urgent strings, heroic brass
Exploration: Ambient with nature sounds, mysterious undertones
Victory: Triumphant fanfare with full orchestral crescendo"
```

## Quality Assurance Checklist

### Pre-Integration Validation
```
3D Models:
□ Polygon count within target range
□ UV unwrapping clean and efficient
□ Texture resolution appropriate for use case
□ LOD versions generated
□ Pivot point correctly positioned
□ Naming convention followed

Textures:
□ Resolution matches quality tier
□ PBR workflow complete (Albedo/Normal/Roughness/Metallic)
□ Tiling seamless where required
□ Color palette adherence
□ Performance optimization applied

Audio:
□ File format appropriate (OGG/WAV)
□ Audio levels normalized
□ Length within specifications
□ Quality sufficient for use case
□ Compression settings optimized
```

### Unity Integration Process

#### Import Settings Automation
```csharp
// AI-generated asset postprocessor
public class FrostRealmAssetPostprocessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        // AI: Configure import settings based on asset type
        // LOD generation, material assignment, etc.
    }
    
    void OnPreprocessTexture()
    {
        TextureImporter importer = assetImporter as TextureImporter;
        // AI: Set texture import settings for PBR workflow
        // Compression, filtering, format selection
    }
}
```

## Batch Processing Workflows

### Mass Asset Generation
```
Batch Generation Prompt:
"Generate complete asset set for [FACTION] in FrostRealm Chronicles:

Units: [List from GDD - e.g., 12 unique units]
Buildings: [List from GDD - e.g., 8 structures]
Heroes: [3 heroes per faction]
Upgrades: [Visual variants for +1/+2/+3 upgrades]

Consistency Requirements:
- Unified color palette per faction
- Matching art style and proportions
- Technical specifications maintained
- Performance targets met

Output each asset with naming convention: [Faction]_[Type]_[Name]_[Variant]
Include technical specifications and integration notes"
```

### Version Control for Assets
```
Git LFS Integration:
- Store large assets (>100MB) in Git LFS
- Tag asset versions with creation method
- Include AI prompt in commit message
- Maintain asset dependency tracking

Naming Convention:
Models: [Faction]_[Type]_[Name]_v[Version].fbx
Textures: [Faction]_[Type]_[Name]_[Map]_v[Version].png
Audio: [Faction]_[Type]_[Name]_v[Version].ogg
```

## Troubleshooting Common Issues

### AI Generation Problems
```
Issue: Inconsistent style between generations
Solution: Use consistent prompt templates, reference images, style guides

Issue: Poor topology for game use
Solution: Specify "game-ready topology" in prompts, use retopology tools

Issue: Incorrect proportions for RTS camera
Solution: Include "isometric optimized" and "readable at distance" in prompts

Issue: Texture seams on models
Solution: Request "properly UV unwrapped" and validate in Unity

Issue: Animation timing doesn't match gameplay
Solution: Specify exact timing requirements based on TFT values
```

### Performance Optimization
```
Automatic LOD Generation:
- Use Unity 6.1's built-in LOD tools
- Generate 4 LOD levels: 100%, 50%, 25%, 10% complexity
- Test rendering performance with target scene complexity

Texture Compression:
- Apply appropriate compression per platform
- Use texture streaming for large environments
- Validate quality vs performance trade-offs
```

This pipeline ensures consistent, high-quality assets while maximizing the use of free AI generation tools.