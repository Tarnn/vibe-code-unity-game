# AI Code Review Checklist for FrostRealm Chronicles

## Overview
This document provides AI-assisted code review workflows for Unity RTS development, focusing on code quality, performance, TFT mechanics accuracy, and maintainability. Use these prompts and checklists to ensure consistent, high-quality code generation and review.

## AI Code Review Prompts

### Comprehensive Code Review
```
Review this Unity C# code for FrostRealm Chronicles RTS game:

Code:
```csharp
[PASTE CODE HERE]
```

Review Criteria:
1. TFT Mechanics Accuracy: Validate formulas against specifications
2. Unity 6.1 Best Practices: ECS/DOTS usage, performance patterns
3. Code Quality: Architecture, naming, documentation
4. Performance: Memory allocation, frame rate impact
5. Maintainability: SOLID principles, testability
6. Security: No exposed secrets, safe input handling

Context: [Brief description of what the code does]
Performance Target: 60 FPS with 500+ units

Output: Detailed review with specific improvement suggestions and corrected code where needed.
```

### Mechanical Accuracy Review
```
Validate TFT mechanics implementation accuracy:

System: [e.g., "Combat damage calculation"]
Code:
```csharp
[PASTE IMPLEMENTATION]
```

TFT Reference: [Specific formula or mechanic from GDD/TechSpecs]
Expected Behavior: [What should happen in TFT]

Check for:
- Formula correctness (±0.1 tolerance for floats)
- Edge case handling (0 values, maximum values)
- Rounding behavior consistency
- Type effectiveness tables accuracy

Output: Validation report with any discrepancies highlighted
```

### Performance Review
```
Review Unity ECS/DOTS code for RTS performance optimization:

Target: Process 500+ entities at 60 FPS (<16ms frame time)
Code:
```csharp
[PASTE SYSTEM CODE]
```

Performance Analysis:
- Job system usage and parallelization opportunities
- Memory allocation patterns and GC pressure
- Burst compilation eligibility
- Query efficiency and filtering
- Component access patterns

Provide optimized version with performance improvements
```

## Code Quality Standards

### Naming Conventions
```csharp
// Approved patterns for FrostRealm Chronicles

// Namespaces
namespace FrostRealm.Core.Combat
namespace FrostRealm.UI.HUD
namespace FrostRealm.Data.Units

// Classes
public class CombatSystem : SystemBase
public class UnitData : ScriptableObject
public class HealthComponent : IComponentData

// Methods
public void CalculateDamage(float baseDamage, ArmorType armorType)
private bool ValidateAttackRange(float distance, float maxRange)

// Variables
public float maxHealthPoints;
private int currentFoodSupply;
public const float DAMAGE_REDUCTION_MULTIPLIER = 0.06f;

// Events
public static event System.Action<float> OnHealthChanged;
public static event System.Action<GameObject, DamageInfo> OnUnitTakeDamage;
```

### Documentation Standards
```csharp
/// <summary>
/// Calculates effective damage based on TFT damage reduction formula.
/// Formula: Reduction = (armor * 0.06) / (1 + armor * 0.06)
/// </summary>
/// <param name="baseDamage">Raw damage before armor reduction</param>
/// <param name="armorValue">Target's armor value</param>
/// <param name="damageType">Type of damage being dealt</param>
/// <param name="armorType">Type of armor on target</param>
/// <returns>Final damage after all reductions and bonuses</returns>
/// <remarks>
/// Implements exact TFT mechanics with type effectiveness multipliers.
/// Reference: TechSpecs.md Combat System section.
/// </remarks>
public float CalculateEffectiveDamage(float baseDamage, int armorValue, 
    DamageType damageType, ArmorType armorType)
{
    // Implementation with clear formula comments
}
```

### Error Handling Patterns
```csharp
// Validated patterns for robust RTS code

// Input validation
public bool TryExecuteAbility(AbilityData ability, GameObject target)
{
    if (ability == null)
    {
        Debug.LogError($"[CombatSystem] Null ability data provided");
        return false;
    }
    
    if (target == null)
    {
        Debug.LogWarning($"[CombatSystem] No target for ability {ability.name}");
        return false;
    }
    
    // Continue with execution
}

// Resource validation
public bool TrySpendResources(ResourceCost cost)
{
    if (!CanAfford(cost))
    {
        GameEvents.OnInsufficientResources?.Invoke(cost);
        return false;
    }
    
    SpendResourcesInternal(cost);
    return true;
}

// Component safety
public bool TryGetHealthComponent(Entity entity, out HealthComponent health)
{
    return EntityManager.TryGetComponent(entity, out health);
}
```

## Review Checklists

### Pre-Commit Checklist
```
□ TFT Mechanics Validation
  □ Formulas match specifications exactly
  □ Edge cases handled (0 values, max values)
  □ Type effectiveness tables correct
  □ Hero attribute scaling accurate

□ Unity 6.1 Best Practices
  □ ECS components are value types
  □ Systems use Burst compilation where possible
  □ No memory allocations in hot paths
  □ Proper component access patterns

□ Code Quality
  □ Follows naming conventions
  □ Comprehensive XML documentation
  □ SOLID principles applied
  □ Unit tests provided

□ Performance
  □ Target frame rate maintained
  □ Memory allocations minimized
  □ Profiler markers added
  □ LOD systems utilized

□ Integration
  □ Event system usage correct
  □ Dependency injection patterns
  □ Proper error handling
  □ Logging for debugging
```

### Architecture Review
```
□ System Responsibilities
  □ Single responsibility principle
  □ Clear system boundaries
  □ Minimal coupling between systems
  □ Event-driven communication

□ Data Flow
  □ Components contain only data
  □ Systems contain only logic
  □ No circular dependencies
  □ Clear ownership patterns

□ Scalability
  □ Handles increasing entity counts
  □ Memory usage scales linearly
  □ Performance degrades gracefully
  □ Easy to add new features
```

### Security Review
```
□ Input Validation
  □ All user inputs validated
  □ Network messages sanitized
  □ File paths restricted to safe directories
  □ No SQL injection vectors

□ Secrets Management
  □ No hardcoded API keys
  □ Configuration externalized
  □ Sensitive data encrypted
  □ Debug information sanitized

□ Access Control
  □ Proper permission checks
  □ Resource access validated
  □ Network security implemented
  □ Cheating prevention measures
```

## AI Review Workflows

### 1. Automated Review Pipeline
```
Step 1: Static Analysis
Prompt: "Analyze this code for potential issues without executing it"

Step 2: Mechanical Validation
Prompt: "Validate TFT mechanics accuracy against specifications"

Step 3: Performance Analysis
Prompt: "Review for Unity ECS performance optimization opportunities"

Step 4: Security Audit
Prompt: "Check for security vulnerabilities and improper data handling"

Step 5: Integration Review
Prompt: "Validate integration with existing FrostRealm Chronicles systems"
```

### 2. Peer Review Simulation
```
Simulate experienced Unity developer reviewing code:

Reviewer Personas:
- Senior Unity Developer: Focus on engine best practices
- RTS Specialist: Validate game mechanics and balance
- Performance Engineer: Identify optimization opportunities
- Security Expert: Find potential vulnerabilities

Generate feedback from multiple perspectives for comprehensive review
```

### 3. Regression Prevention
```
Compare new code against existing patterns:

Prompt: "Compare this implementation with existing FrostRealm Chronicles code patterns. Identify any inconsistencies or deviations from established conventions."

Include:
- Naming convention adherence
- Architecture pattern consistency
- Error handling approaches
- Performance characteristic alignment
```

## Common Issues and Patterns

### Performance Anti-Patterns
```csharp
// ❌ BAD: Memory allocation in hot path
public void UpdateDamage()
{
    var targets = new List<GameObject>(); // Allocates every frame
    // Process targets
}

// ✅ GOOD: Reuse collections
private List<GameObject> targetsCache = new List<GameObject>();
public void UpdateDamage()
{
    targetsCache.Clear();
    // Process targets
}

// ❌ BAD: GameObject operations in ECS system
foreach (var entity in query)
{
    var go = GetComponent<Transform>(entity).gameObject; // Slow bridge
}

// ✅ GOOD: Pure ECS operations
foreach (var (transform, health) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthComponent>>())
{
    // Direct component access
}
```

### TFT Mechanics Common Errors
```csharp
// ❌ BAD: Incorrect damage reduction formula
float reduction = armor * 0.06f; // Missing denominator

// ✅ GOOD: Correct TFT formula
float reduction = (armor * 0.06f) / (1f + armor * 0.06f);

// ❌ BAD: Wrong hero attribute scaling
float heroDamage = baseDamage + strength; // Missing dice and scaling

// ✅ GOOD: Correct hero damage
float heroDamage = rollDice(2, 6) + strength + (isPrimary ? strength : 0);
```

### Architecture Issues
```csharp
// ❌ BAD: Tight coupling
public class CombatSystem : MonoBehaviour
{
    public UIManager uiManager; // Direct dependency
    
    void DealDamage()
    {
        uiManager.UpdateHealthBar(); // Tight coupling
    }
}

// ✅ GOOD: Event-driven loose coupling
public class CombatSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Deal damage logic
        GameEvents.OnHealthChanged?.Invoke(newHealth); // Loose coupling
    }
}
```

## AI-Assisted Refactoring

### Legacy Code Modernization
```
Modernize this MonoBehaviour to Unity 6.1 ECS:

Legacy Code:
```csharp
[PASTE LEGACY CODE]
```

Requirements:
- Convert to ECS/DOTS architecture
- Maintain exact behavior
- Add Burst compilation
- Improve performance
- Follow FrostRealm Chronicles patterns

Output: Modernized implementation with migration notes
```

### Performance Optimization
```
Optimize this code for RTS performance requirements:

Current Implementation:
```csharp
[PASTE CODE]
```

Constraints:
- Must handle 500+ entities
- Target <2ms execution time
- Maintain TFT accuracy
- Preserve functionality

Provide: Optimized version with performance analysis
```

### Code Organization
```
Reorganize this code following SOLID principles:

Code:
```csharp
[PASTE MONOLITHIC CODE]
```

Goals:
- Single responsibility per class
- Open/closed principle adherence
- Dependency inversion
- Interface segregation
- Liskov substitution compliance

Output: Refactored architecture with clear separations
```

## Quality Metrics

### Code Coverage Standards
- **Unit Tests**: 90%+ for core systems
- **Integration Tests**: 80%+ for system interactions
- **TFT Mechanics**: 100% formula coverage
- **Performance Tests**: All ECS systems benchmarked

### Performance Benchmarks
- **Frame Time**: <16ms for 500+ units
- **Memory Usage**: <2GB for full battle scenarios
- **Load Times**: <5s for map loading
- **Network Latency**: <100ms for multiplayer

### Review Metrics
```
Track review effectiveness:

Defect Detection Rate: [Pre-release bugs found / Total bugs]
Review Coverage: [Lines reviewed / Total lines changed]
Review Time: [Average time per review]
Fix Rate: [Issues fixed / Issues identified]
```

## Continuous Improvement

### Review Template Evolution
```
Regularly update review prompts based on:
- Common issues discovered
- New Unity features
- Performance learnings
- TFT mechanics clarifications
- Team feedback
```

### Knowledge Base Maintenance
```
Maintain searchable database of:
- Common review findings
- Best practice examples
- Anti-pattern catalogs
- Performance optimization techniques
- TFT mechanics reference implementations
```

This comprehensive review framework ensures consistent code quality while leveraging AI for efficient and thorough reviews.