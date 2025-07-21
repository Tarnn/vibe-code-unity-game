# AI Debugging Guide for FrostRealm Chronicles

## Overview
This guide provides AI-assisted debugging strategies for Unity RTS development, covering common Unity issues, ECS/DOTS problems, multiplayer synchronization, performance bottlenecks, and game-specific mechanics debugging. Use these prompts and workflows to rapidly identify and resolve issues.

## AI Debugging Prompts

### General Bug Analysis
```
Debug this Unity issue in FrostRealm Chronicles:

Problem Description: [Specific issue, e.g., "Units not responding to move commands"]
Expected Behavior: [What should happen]
Actual Behavior: [What is happening]
Reproduction Steps: [How to trigger the bug]

Error Messages/Logs:
```
[PASTE LOGS HERE]
```

Code Context:
```csharp
[PASTE RELEVANT CODE]
```

System Info: Unity 6.1 LTS, Windows/macOS, [hardware specs if relevant]

Provide: Root cause analysis, fix implementation, and prevention strategies
```

### Performance Debugging
```
Analyze performance issue in FrostRealm Chronicles:

Symptoms: [e.g., "Frame rate drops during large battles"]
Profiler Data: [FPS, memory usage, specific bottlenecks]
Scene Context: [Entity count, active systems, complexity]

Code Under Investigation:
```csharp
[PASTE PERFORMANCE-CRITICAL CODE]
```

Target Performance: 60 FPS with 500+ units
Current Performance: [Actual measurements]

Requirements:
- Identify bottlenecks using Unity Profiler data
- Suggest ECS/DOTS optimizations
- Provide optimized implementation
- Include profiler markers for monitoring
```

### ECS/DOTS Debugging
```
Debug Unity ECS system issue:

System: [System name and purpose]
Issue: [Specific ECS problem, e.g., "Entities not being processed", "Job compilation errors"]

Error Details:
```
[PASTE ECS ERRORS]
```

System Implementation:
```csharp
[PASTE ECS SYSTEM CODE]
```

Component Definitions:
```csharp
[PASTE COMPONENT CODE]
```

Archetype Info: [If available from Entity Debugger]

Analyze for: Component access patterns, query correctness, job scheduling issues, Burst compilation problems
```

### TFT Mechanics Debugging
```
Debug game mechanics accuracy against TFT reference:

Mechanic: [e.g., "Damage calculation", "Hero leveling", "Resource upkeep"]
Expected TFT Behavior: [Reference from GDD/TechSpecs]
Observed Behavior: [What's happening in game]

Implementation:
```csharp
[PASTE FORMULA IMPLEMENTATION]
```

Test Cases:
- [Known values that should work]
- [Edge cases that are failing]

Validate against TFT formulas and provide corrected implementation
```

## Common Unity Issues

### Null Reference Exceptions
```csharp
// Common patterns and solutions

// ❌ Problem: Unprotected component access
void Update()
{
    transform.position = target.position; // NullReferenceException if target is null
}

// ✅ Solution: Defensive programming
void Update()
{
    if (target != null)
    {
        transform.position = target.position;
    }
    else
    {
        Debug.LogWarning($"[{gameObject.name}] Target is null, stopping movement");
        enabled = false; // Disable script to prevent spam
    }
}

// ✅ Better: Try-get pattern for components
if (TryGetComponent<Health>(out var health))
{
    health.TakeDamage(damage);
}
```

### Performance Issues
```csharp
// Common RTS performance problems

// ❌ Problem: Inefficient collision detection
void Update()
{
    var enemies = GameObject.FindObjectsOfType<Enemy>(); // Expensive every frame
    foreach (var enemy in enemies)
    {
        if (Vector3.Distance(transform.position, enemy.transform.position) < attackRange)
        {
            Attack(enemy);
        }
    }
}

// ✅ Solution: Cached queries with spatial partitioning
private List<Enemy> cachedEnemies = new List<Enemy>();
private float lastCacheUpdate = 0f;
private const float CACHE_UPDATE_INTERVAL = 0.1f; // Update 10x per second

void Update()
{
    if (Time.time - lastCacheUpdate > CACHE_UPDATE_INTERVAL)
    {
        UpdateEnemyCache();
        lastCacheUpdate = Time.time;
    }
    
    foreach (var enemy in cachedEnemies)
    {
        if (Vector3.SqrMagnitude(transform.position - enemy.transform.position) < attackRange * attackRange)
        {
            Attack(enemy);
            break; // Only attack one target per frame
        }
    }
}
```

### Memory Leaks
```csharp
// Common memory leak patterns

// ❌ Problem: Event subscription without cleanup
void Start()
{
    GameEvents.OnUnitDied += HandleUnitDeath; // Never unsubscribed
}

// ✅ Solution: Proper event lifecycle
void OnEnable()
{
    GameEvents.OnUnitDied += HandleUnitDeath;
}

void OnDisable()
{
    GameEvents.OnUnitDied -= HandleUnitDeath;
}

// ❌ Problem: Uncleaned collections
private List<Projectile> activeProjectiles = new List<Projectile>();

void FireProjectile()
{
    activeProjectiles.Add(new Projectile()); // Never removed
}

// ✅ Solution: Proper cleanup
void Update()
{
    for (int i = activeProjectiles.Count - 1; i >= 0; i--)
    {
        if (activeProjectiles[i].IsDestroyed)
        {
            activeProjectiles.RemoveAt(i);
        }
    }
}
```

## ECS/DOTS Specific Issues

### Component Access Errors
```csharp
// Common ECS pitfalls

// ❌ Problem: Incorrect component access
protected override void OnUpdate()
{
    Entities.ForEach((ref Translation translation, in Velocity velocity) =>
    {
        translation.Value += velocity.Value * Time.DeltaTime; // Wrong Time access
    }).Schedule();
}

// ✅ Solution: Use SystemAPI for frame data
protected override void OnUpdate()
{
    float deltaTime = SystemAPI.Time.DeltaTime;
    
    foreach (var (translation, velocity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Velocity>>())
    {
        translation.ValueRW.Position += velocity.ValueRO.Value * deltaTime;
    }
}

// ❌ Problem: Structural changes during iteration
protected override void OnUpdate()
{
    Entities.ForEach((Entity entity, in HealthComponent health) =>
    {
        if (health.CurrentHP <= 0)
        {
            EntityManager.DestroyEntity(entity); // Structural change during iteration
        }
    }).Run();
}

// ✅ Solution: Use ECB for deferred structural changes
protected override void OnUpdate()
{
    var ecb = new EntityCommandBuffer(Allocator.TempJob);
    
    foreach (var (health, entity) in SystemAPI.Query<RefRO<HealthComponent>>().WithEntityAccess())
    {
        if (health.ValueRO.CurrentHP <= 0)
        {
            ecb.DestroyEntity(entity);
        }
    }
    
    ecb.Playback(EntityManager);
    ecb.Dispose();
}
```

### Burst Compilation Issues
```csharp
// Common Burst problems

// ❌ Problem: Managed objects in Burst jobs
[BurstCompile]
struct DamageJob : IJobForEach<HealthComponent, DamageComponent>
{
    public void Execute(ref HealthComponent health, ref DamageComponent damage)
    {
        Debug.Log($"Dealing {damage.Value} damage"); // Managed call in Burst
        health.CurrentHP -= damage.Value;
    }
}

// ✅ Solution: Remove managed calls or use [BurstDiscard]
[BurstCompile]
struct DamageJob : IJobForEach<HealthComponent, DamageComponent>
{
    public void Execute(ref HealthComponent health, ref DamageComponent damage)
    {
        LogDamage(damage.Value); // Burst-compatible logging
        health.CurrentHP -= damage.Value;
    }
    
    [BurstDiscard]
    private static void LogDamage(float damageValue)
    {
        Debug.Log($"Dealing {damageValue} damage");
    }
}
```

## Multiplayer Debugging

### Synchronization Issues
```csharp
// Common networking problems

// ❌ Problem: Client-side authority on critical data
[ClientRpc]
void DealDamageClientRpc(float damage)
{
    health -= damage; // Client modifies authoritative state
}

// ✅ Solution: Server authority with client prediction
[ServerRpc(RequireOwnership = false)]
void DealDamageServerRpc(float damage, ServerRpcParams serverRpcParams = default)
{
    if (ValidateDamageSource(serverRpcParams.Receive.SenderClientId))
    {
        health -= damage;
        UpdateHealthClientRpc(health); // Sync to all clients
    }
}

[ClientRpc]
void UpdateHealthClientRpc(float newHealth)
{
    health = newHealth; // Server-authoritative update
}
```

### Desync Detection
```csharp
// Desync debugging tools

public class DesyncDetector : NetworkBehaviour
{
    [SerializeField] private float checksumUpdateRate = 1f;
    private float lastChecksumTime;
    
    void Update()
    {
        if (IsServer && Time.time - lastChecksumTime > checksumUpdateRate)
        {
            var checksum = CalculateGameStateChecksum();
            ValidateChecksumClientRpc(checksum);
            lastChecksumTime = Time.time;
        }
    }
    
    [ClientRpc]
    void ValidateChecksumClientRpc(uint serverChecksum)
    {
        var clientChecksum = CalculateGameStateChecksum();
        if (clientChecksum != serverChecksum)
        {
            Debug.LogError($"Desync detected! Server: {serverChecksum}, Client: {clientChecksum}");
            RequestResyncServerRpc();
        }
    }
    
    private uint CalculateGameStateChecksum()
    {
        // Calculate deterministic checksum of critical game state
        uint checksum = 0;
        foreach (var unit in FindObjectsOfType<Unit>())
        {
            checksum ^= (uint)unit.transform.position.GetHashCode();
            checksum ^= (uint)unit.Health.GetHashCode();
        }
        return checksum;
    }
}
```

## Game-Specific Debugging

### Resource System Issues
```csharp
// TFT-specific debugging patterns

public class ResourceSystemDebugger : MonoBehaviour
{
    [SerializeField] private bool enableDebugLogging = true;
    
    void OnEnable()
    {
        if (enableDebugLogging)
        {
            ResourceSystem.OnResourceChanged += LogResourceChange;
            ResourceSystem.OnUpkeepCalculated += LogUpkeepCalculation;
        }
    }
    
    void LogResourceChange(ResourceType type, float oldValue, float newValue, string reason)
    {
        Debug.Log($"[Resource] {type}: {oldValue} -> {newValue} ({reason})");
        
        // Validate against TFT rules
        if (type == ResourceType.Gold && newValue < 0)
        {
            Debug.LogError("[Resource] Gold went negative! Check spending validation.");
        }
    }
    
    void LogUpkeepCalculation(int foodUsed, float incomeMultiplier)
    {
        // Validate TFT upkeep formula
        float expectedMultiplier = CalculateExpectedUpkeep(foodUsed);
        if (Mathf.Abs(incomeMultiplier - expectedMultiplier) > 0.01f)
        {
            Debug.LogError($"[Upkeep] Incorrect calculation! Expected: {expectedMultiplier}, Got: {incomeMultiplier}");
        }
    }
    
    float CalculateExpectedUpkeep(int food)
    {
        if (food <= 50) return 1.0f;
        if (food <= 80) return 0.7f;
        return 0.4f;
    }
}
```

### Combat System Debugging
```csharp
public class CombatDebugger : MonoBehaviour
{
    [SerializeField] private bool visualizeAttackRanges = true;
    [SerializeField] private bool logDamageCalculations = true;
    
    void OnDrawGizmos()
    {
        if (!visualizeAttackRanges) return;
        
        foreach (var unit in FindObjectsOfType<Unit>())
        {
            Gizmos.color = unit.IsSelected ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(unit.transform.position, unit.AttackRange);
        }
    }
    
    public static void LogDamageCalculation(DamageInfo damage, ArmorInfo armor, float finalDamage)
    {
        if (!instance.logDamageCalculations) return;
        
        float expectedDamage = CombatFormulas.CalculateExpectedDamage(damage, armor);
        
        Debug.Log($"[Combat] {damage.Type} {damage.Value} vs {armor.Type} {armor.Value} = {finalDamage} " +
                  $"(Expected: {expectedDamage})");
        
        if (Mathf.Abs(finalDamage - expectedDamage) > 0.1f)
        {
            Debug.LogError($"[Combat] Damage calculation mismatch! Formula may be incorrect.");
        }
    }
}
```

## AI Debugging Workflows

### 1. Systematic Issue Resolution
```
Step 1: Problem Identification
Prompt: "Analyze symptoms and logs to identify the root cause"

Step 2: Hypothesis Generation
Prompt: "Generate 3-5 potential causes ranked by likelihood"

Step 3: Validation Strategy
Prompt: "Create tests to validate each hypothesis"

Step 4: Solution Implementation
Prompt: "Implement fix for confirmed root cause"

Step 5: Prevention Measures
Prompt: "Add safeguards to prevent similar issues"
```

### 2. Performance Optimization
```
Performance Debugging Workflow:

Step 1: Profiler Analysis
"Analyze Unity Profiler data and identify bottlenecks"

Step 2: Code Review
"Review performance-critical code sections"

Step 3: Optimization Strategy
"Develop optimization plan maintaining functionality"

Step 4: Implementation
"Apply optimizations with before/after measurements"

Step 5: Validation
"Verify performance targets met without regressions"
```

### 3. Regression Investigation
```
When something breaks after changes:

Step 1: Identify Change Set
"Review recent commits that could cause the issue"

Step 2: Binary Search
"Use git bisect approach to isolate problematic change"

Step 3: Impact Analysis
"Analyze how the change affects related systems"

Step 4: Minimal Fix
"Implement smallest fix that resolves the issue"

Step 5: Test Coverage
"Add tests to prevent future regressions"
```

## Debug Tools and Utilities

### Custom Debug Console
```csharp
public class FrostRealmDebugConsole : MonoBehaviour
{
    [Header("Debug Commands")]
    [SerializeField] private bool enableDebugConsole = true;
    
    void Update()
    {
        if (!enableDebugConsole) return;
        
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleDebugMode();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            LogGameState();
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ValidateAllFormulas();
        }
    }
    
    [ContextMenu("Validate TFT Formulas")]
    void ValidateAllFormulas()
    {
        var validator = new TFTFormulaValidator();
        validator.ValidateAllFormulas();
    }
    
    [ContextMenu("Log Current Game State")]
    void LogGameState()
    {
        Debug.Log($"Units: {FindObjectsOfType<Unit>().Length}");
        Debug.Log($"Resources: G:{ResourceSystem.Gold} L:{ResourceSystem.Lumber} F:{ResourceSystem.Food}");
        Debug.Log($"Performance: {1f/Time.deltaTime:F1} FPS");
    }
}
```

### Automated Testing for Debugging
```csharp
[TestFixture]
public class DebugValidationTests
{
    [Test]
    public void ValidateAllTFTFormulas()
    {
        var testCases = LoadTFTTestCases();
        
        foreach (var testCase in testCases)
        {
            var result = CombatFormulas.CalculateDamage(testCase.damage, testCase.armor);
            Assert.AreEqual(testCase.expected, result, 0.1f, 
                $"Formula failed for {testCase.description}");
        }
    }
    
    [Test]
    public void ValidateResourceIntegrity()
    {
        var initialGold = ResourceSystem.Gold;
        
        // Perform various resource operations
        ResourceSystem.SpendGold(100);
        ResourceSystem.AddGold(50);
        
        var expectedGold = initialGold - 50;
        Assert.AreEqual(expectedGold, ResourceSystem.Gold, 
            "Resource tracking integrity failed");
    }
}
```

## Prevention Strategies

### Defensive Programming
```csharp
// Always validate inputs
public void TakeDamage(float damage, DamageType type)
{
    if (damage < 0)
    {
        Debug.LogError($"[Health] Negative damage received: {damage}");
        return;
    }
    
    if (currentHealth <= 0)
    {
        Debug.LogWarning($"[Health] Damage dealt to dead unit");
        return;
    }
    
    // Process damage...
}

// Use assertions for development
public void SetPosition(Vector3 position)
{
    Debug.Assert(!float.IsNaN(position.x), "Position contains NaN values");
    Debug.Assert(position.y >= 0, "Units cannot be below ground level");
    
    transform.position = position;
}
```

### Monitoring and Alerts
```csharp
public class PerformanceMonitor : MonoBehaviour
{
    private const float WARNING_FRAME_TIME = 20f; // ms
    private const float CRITICAL_FRAME_TIME = 33f; // ms
    
    void Update()
    {
        float frameTime = Time.deltaTime * 1000f;
        
        if (frameTime > CRITICAL_FRAME_TIME)
        {
            Debug.LogError($"Critical frame time: {frameTime:F1}ms");
            LogPerformanceSnapshot();
        }
        else if (frameTime > WARNING_FRAME_TIME)
        {
            Debug.LogWarning($"Frame time warning: {frameTime:F1}ms");
        }
    }
    
    void LogPerformanceSnapshot()
    {
        Debug.Log($"Entity count: {EntityManager.GetAllEntities().Length}");
        Debug.Log($"Memory usage: {System.GC.GetTotalMemory(false) / 1024 / 1024}MB");
    }
}
```

This guide provides comprehensive debugging strategies to rapidly identify and resolve issues in your RTS development.