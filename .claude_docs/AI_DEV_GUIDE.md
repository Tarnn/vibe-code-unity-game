# AI Development Guide for FrostRealm Chronicles

## Overview
This guide provides AI-powered development workflows specifically for FrostRealm Chronicles RTS development using Claude/Cursor. It includes optimized prompts, best practices, and common patterns for rapid iteration.

## Quick Reference Prompts

### System Implementation
```
Generate a Unity 6.1 ECS/DOTS system for [SYSTEM_NAME] in FrostRealm Chronicles.

Context from GDD/TechSpecs:
- [Paste relevant mechanics, e.g., "Resource system: Gold 8-12g/trip, upkeep penalties at 51+ food"]
- Integration: [e.g., "Tie to UI events, use ScriptableObjects"]
- Performance: [e.g., "Target 500+ units at 60 FPS"]

Requirements:
- Follow TFT mechanics exactly
- Use Burst compilation where applicable
- Include error handling and validation
- Add comprehensive XML documentation

Output: Complete C# implementation with namespace FrostRealm.Core
```

### Bug Fixing & Debugging
```
Debug this Unity RTS code for FrostRealm Chronicles:

Issue: [Specific problem, e.g., "Units not pathfinding correctly in formation"]
Expected: [What should happen]
Actual: [What's happening]

Code:
```csharp
[PASTE CODE HERE]
```

Context from TechSpecs: [Relevant section about pathfinding/movement]

Fix and explain changes with // AI: comments
```

### Performance Optimization
```
Optimize this RTS code for Unity 6.1 ECS performance (target 60 FPS with 500+ units):

Current Code:
```csharp
[PASTE CODE]
```

Requirements:
- Convert to ECS/DOTS if not already
- Use Burst compilation
- Implement job parallelization
- Add profiler markers

Output: Optimized version with performance notes
```

### UI Implementation
```
Create Unity UI Toolkit interface for [UI_ELEMENT] in FrostRealm Chronicles.

Requirements from GDD:
- [Paste UI specs, e.g., "Resource display: Gold/Lumber/Food counters"]
- Style: TFT-inspired with modern polish
- Responsive to game events
- Accessibility features (colorblind support)

Output: UXML, USS, and C# binding scripts
```

## Development Workflows

### 1. Feature Implementation Pipeline
```
1. Reference GDD/TechSpecs for exact requirements
2. Generate base implementation with Claude
3. Test in Unity Editor
4. Debug and refine with AI assistance
5. Optimize for performance
6. Add unit tests
7. Update documentation
```

### 2. Asset Integration Workflow
```
1. Generate AI prompts from ASSET_PIPELINE.md
2. Create assets with AI tools (Rosebud, Layer AI, etc.)
3. Import to Unity with auto-LOD
4. Generate integration scripts with Claude
5. Test in-game and iterate
```

> **Directory convention**: Place all hero and playable character assets under `assets/hero/` (e.g., `assets/hero/Paladin/Paladin_Base.fbx`). Unity import automation and post-processor scripts target this folder by default.

### 3. Debugging Workflow
```
1. Collect error logs and context
2. Use AI_DEBUGGING_GUIDE.md prompts
3. Generate fixes with Claude
4. Test and validate
5. Add preventive measures
```

## Code Patterns

### ScriptableObject Data Pattern
```csharp
// Use this pattern for all game data (units, buildings, abilities)
[CreateAssetMenu(fileName = "New Unit Data", menuName = "FrostRealm/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("TFT Stats")]
    public float baseHP = 420f; // Footman default
    public DamageInfo damage;
    public ArmorInfo armor;
    public ResourceCost cost;
    
    [Header("Abilities")]
    public AbilityData[] abilities;
}
```

### ECS Component Pattern
```csharp
// Standard ECS component for game entities
[System.Serializable]
public struct HealthComponent : IComponentData
{
    public float currentHP;
    public float maxHP;
    public float regenRate;
}
```

### Event System Pattern
```csharp
// For loose coupling between systems
public static class GameEvents
{
    public static event System.Action<float, float, float> OnResourcesChanged;
    public static event System.Action<GameObject> OnUnitDied;
}
```

## AI-Assisted Testing

### Unit Test Generation
```
Generate Unity Test Framework tests for this FrostRealm Chronicles system:

System: [Name and brief description]
Code:
```csharp
[PASTE IMPLEMENTATION]
```

Generate tests for:
- Core functionality
- Edge cases (null refs, invalid inputs)
- TFT formula accuracy (if applicable)
- Performance benchmarks

Use AAA pattern (Arrange, Act, Assert)
```

### Integration Test Pattern
```
Create integration test for [SYSTEM_A] + [SYSTEM_B] interaction in FrostRealm Chronicles:

Scenario: [e.g., "Unit takes damage and updates UI"]
Expected Flow: [Step by step]

Generate NUnit test with proper setup/teardown
```

## AI Prompt Optimization

### Context Management
- Always reference relevant GDD/TechSpecs sections
- Include exact TFT formulas when applicable
- Specify Unity 6.1 features to leverage
- Mention performance targets (60 FPS, 500+ units)

### Response Formatting
- Request namespace organization
- Ask for XML documentation
- Specify error handling requirements
- Request performance markers for profiling

### Iteration Strategy
- Start with working implementation
- Optimize in separate prompts
- Add features incrementally
- Test each iteration thoroughly

## Common AI Workflows

### New System Development
1. **Planning**: "Break down [SYSTEM] into components based on TechSpecs"
2. **Core**: "Implement basic [SYSTEM] functionality"
3. **Integration**: "Connect [SYSTEM] to existing code"
4. **Polish**: "Add error handling and optimization"
5. **Testing**: "Generate comprehensive tests"

### Refactoring
1. **Analysis**: "Analyze this code for improvement opportunities"
2. **Plan**: "Create refactoring plan maintaining TFT compatibility"
3. **Execute**: "Refactor while preserving functionality"
4. **Validate**: "Generate tests to verify behavior unchanged"

### Debug Session
1. **Reproduce**: "Help reproduce this bug consistently"
2. **Analyze**: "Identify root cause of [ISSUE]"
3. **Fix**: "Generate fix maintaining game balance"
4. **Prevent**: "Add safeguards against similar issues"

## Quality Checklist

Use this before committing AI-generated code:

- [ ] Follows TFT mechanics exactly
- [ ] Uses proper namespace (FrostRealm.*)
- [ ] Includes XML documentation
- [ ] Has error handling
- [ ] Performance optimized (ECS/Burst where appropriate)
- [ ] Follows Unity 6.1 best practices
- [ ] Integrates with existing systems
- [ ] Maintains code style consistency
- [ ] Includes relevant profiler markers

## Advanced Techniques

### Multi-System Prompts
```
Implement interacting systems for FrostRealm Chronicles:

Systems: [A, B, C]
Interactions: [How they connect]
Shared Data: [Common components/events]

Generate all systems with proper communication patterns
```

### Architecture Guidance
```
Design system architecture for [FEATURE] in Unity 6.1 ECS:

Requirements: [From GDD]
Performance: [Targets]
Integration: [Existing systems]

Provide:
- Component design
- System responsibilities
- Data flow diagrams (text-based)
- Interface definitions
```

### Legacy Migration
```
Convert this MonoBehaviour to Unity 6.1 ECS for FrostRealm Chronicles:

Legacy Code:
```csharp
[PASTE OLD CODE]
```

Requirements:
- Maintain exact behavior
- Improve performance
- Follow ECS best practices
- Add Burst compilation where possible
```

## AI Tool Integration

### With Cursor
- Use Ctrl+L for chat mode with these prompts
- Use Ctrl+K for inline code generation
- Reference this guide in chat for context

### With Claude
- Always include relevant docs sections
- Use code blocks for better parsing
- Iterate on generated code systematically

### Version Control
- Commit AI-generated code separately
- Tag commits with "AI: [description]"
- Include prompt used in commit message

## Troubleshooting

### Common AI Issues
1. **Hallucinated APIs**: Always verify Unity API exists
2. **Outdated Patterns**: Specify Unity 6.1 explicitly
3. **Missing Context**: Include more GDD/TechSpecs
4. **Performance Issues**: Request ECS/DOTS specifically

### Quality Assurance
- Test all AI-generated code in Unity
- Verify TFT formula accuracy with references
- Check performance with Unity Profiler
- Validate against existing codebase patterns

This guide should be updated as development progresses and new patterns emerge.