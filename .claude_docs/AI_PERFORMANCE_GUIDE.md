# AI Performance Optimization Guide for FrostRealm Chronicles

## Overview
This guide provides AI-assisted performance optimization strategies for Unity 6.1 ECS/DOTS RTS development. It covers system optimization, memory management, rendering performance, and achieving 60 FPS with 500+ units using modern Unity features and AI-generated solutions.

## Performance Targets

### Target Specifications
- **Frame Rate**: 60 FPS (16.67ms budget)
- **Entity Count**: 500+ units simultaneously
- **Memory Usage**: <2GB total allocation
- **Load Times**: <5 seconds for map loading
- **Network Latency**: <100ms for multiplayer

### Hardware Targets
- **Minimum**: GTX 1650, 8GB RAM, Intel Core i5
- **Recommended**: RTX 3060, 16GB RAM, Intel Core i7
- **Ideal**: RTX 4070, 32GB RAM, Intel Core i9

## AI Optimization Prompts

### System Performance Analysis
```
Analyze and optimize Unity ECS system for RTS performance:

Current System:
```csharp
[PASTE SYSTEM CODE]
```

Performance Requirements:
- Process 500+ entities in <2ms
- Maintain 60 FPS during combat
- Minimize memory allocations
- Use Burst compilation effectively

Current Metrics:
- Frame time: [X]ms
- Entity processing time: [X]ms
- Memory allocations: [X]MB/frame

Provide optimized implementation with:
- Burst-compiled jobs
- Parallel processing strategies
- Memory optimization techniques
- Profiler marker integration
```

### Memory Optimization
```
Optimize memory usage for RTS game systems:

System: [System name and current implementation]
Memory Issues:
- GC allocations: [X]MB/frame
- Peak memory usage: [X]GB
- Allocation hotspots: [Specific areas]

Code to Optimize:
```csharp
[PASTE MEMORY-INTENSIVE CODE]
```

Requirements:
- Eliminate per-frame allocations
- Implement object pooling where appropriate
- Use Unity 6.1 memory management features
- Maintain functionality while reducing footprint

Output: Optimized implementation with memory analysis
```

### Rendering Performance
```
Optimize rendering performance for large-scale RTS battles:

Current Setup:
- Rendering pipeline: HDRP/URP
- Entity count: [X] units
- Draw calls: [X] per frame
- Overdraw issues: [Description]

Performance Issues:
- GPU bottlenecks: [Specific areas]
- CPU-side rendering overhead: [Details]
- LOD system effectiveness: [Current state]

Requirements:
- GPU instancing for unit rendering
- Dynamic LOD based on distance/importance
- Culling optimization
- Batch processing for similar units

Provide optimized rendering strategy
```

### Job System Optimization
```
Optimize Unity Job System usage for RTS performance:

Current Job Implementation:
```csharp
[PASTE JOB CODE]
```

Performance Analysis:
- Job scheduling overhead: [X]ms
- Parallelization efficiency: [X]%
- Dependencies and synchronization points
- Burst compilation status

Target: Maximize CPU core utilization while maintaining determinism for RTS gameplay

Output: Optimized job chain with dependency analysis
```

## ECS/DOTS Performance Patterns

### Efficient Component Design
```csharp
// ✅ GOOD: Value types with minimal data
[System.Serializable]
public struct HealthComponent : IComponentData
{
    public float currentHP;
    public float maxHP;
    public float regenRate;
    // Total: 12 bytes - cache-friendly
}

// ❌ BAD: Reference types with excessive data
public class HealthComponent : IComponentData
{
    public string unitName; // String reference
    public List<DamageModifier> modifiers; // Collection reference
    public Transform healthBarTransform; // GameObject reference
    // Causes cache misses and GC pressure
}

// ✅ GOOD: Separate concerns into different components
public struct HealthComponent : IComponentData
{
    public float currentHP;
    public float maxHP;
}

public struct RegenerationComponent : IComponentData
{
    public float rate;
    public float nextRegenTime;
}

public struct DamageModifiersComponent : IComponentData
{
    public float physicalReduction;
    public float magicalReduction;
    // Fixed-size data instead of collections
}
```

### High-Performance Systems
```csharp
// Optimized combat system for 500+ units
[BurstCompile]
public partial struct CombatSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Use SystemAPI for modern ECS patterns
        var deltaTime = SystemAPI.Time.DeltaTime;
        var combatJob = new CombatJob
        {
            deltaTime = deltaTime,
            damageBufferLookup = SystemAPI.GetBufferLookup<DamageBuffer>()
        };
        
        // Schedule parallel job for all combat entities
        var jobHandle = combatJob.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
    }
}

[BurstCompile]
public partial struct CombatJob : IJobEntity
{
    public float deltaTime;
    public BufferLookup<DamageBuffer> damageBufferLookup;
    
    public void Execute(Entity entity, ref HealthComponent health, 
                       in AttackComponent attack, in LocalTransform transform)
    {
        // Burst-compiled combat logic
        if (damageBufferLookup.TryGetBuffer(entity, out var damageBuffer))
        {
            ProcessDamage(ref health, damageBuffer, deltaTime);
            damageBuffer.Clear(); // Clear damage for next frame
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ProcessDamage(ref HealthComponent health, 
                                    DynamicBuffer<DamageBuffer> damageBuffer, 
                                    float deltaTime)
    {
        for (int i = 0; i < damageBuffer.Length; i++)
        {
            var damage = damageBuffer[i];
            var reduction = CalculateDamageReduction(damage.armorValue);
            var finalDamage = damage.value * (1f - reduction);
            health.currentHP -= finalDamage;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CalculateDamageReduction(int armor)
    {
        // TFT formula optimized for Burst
        const float ARMOR_CONSTANT = 0.06f;
        var armorFloat = (float)armor;
        return (armorFloat * ARMOR_CONSTANT) / (1f + armorFloat * ARMOR_CONSTANT);
    }
}
```

### Memory-Efficient Queries
```csharp
// Optimized queries for large entity counts
public partial struct MovementSystem : ISystem
{
    private EntityQuery movingUnitsQuery;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Create specific queries to avoid unnecessary iteration
        movingUnitsQuery = SystemAPI.QueryBuilder()
            .WithAll<LocalTransform, MoveCommand, Velocity>()
            .WithNone<DeadTag>() // Exclude dead units
            .Build();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Only process entities that actually need movement updates
        if (movingUnitsQuery.IsEmpty) return;
        
        var moveJob = new MoveJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        
        state.Dependency = moveJob.ScheduleParallel(movingUnitsQuery, state.Dependency);
    }
}
```

## Rendering Optimization

### GPU Instancing for Units
```csharp
// Efficient unit rendering with instancing
public class UnitRenderer : MonoBehaviour
{
    [SerializeField] private Mesh unitMesh;
    [SerializeField] private Material unitMaterial;
    [SerializeField] private int maxInstancesPerBatch = 1000;
    
    private Matrix4x4[] instanceMatrices;
    private MaterialPropertyBlock propertyBlock;
    private ComputeBuffer argsBuffer;
    
    void Start()
    {
        instanceMatrices = new Matrix4x4[maxInstancesPerBatch];
        propertyBlock = new MaterialPropertyBlock();
        
        // Set up indirect rendering args
        uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
        args[0] = (uint)unitMesh.GetIndexCount(0);
        args[1] = (uint)maxInstancesPerBatch;
        args[2] = (uint)unitMesh.GetIndexStart(0);
        args[3] = (uint)unitMesh.GetBaseVertex(0);
        
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), 
                                      ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
    }
    
    void Update()
    {
        var visibleUnits = GetVisibleUnits(); // Frustum culled units
        int instanceCount = Mathf.Min(visibleUnits.Count, maxInstancesPerBatch);
        
        // Populate instance matrices
        for (int i = 0; i < instanceCount; i++)
        {
            instanceMatrices[i] = visibleUnits[i].transform.localToWorldMatrix;
        }
        
        // Update GPU buffer and render
        propertyBlock.SetMatrixArray("_InstanceMatrix", instanceMatrices);
        Graphics.DrawMeshInstancedIndirect(unitMesh, 0, unitMaterial, 
                                         GetVisibilityBounds(), argsBuffer, 0, propertyBlock);
    }
}
```

### Dynamic LOD System
```csharp
// Distance-based LOD for RTS performance
[System.Serializable]
public struct LODComponent : IComponentData
{
    public int currentLODLevel;
    public float lastLODUpdate;
    public Entity meshEntity;
}

[BurstCompile]
public partial struct LODSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var cameraPosition = GetMainCameraPosition();
        var currentTime = (float)SystemAPI.Time.ElapsedTime;
        
        // Update LOD every 0.1 seconds to reduce overhead
        var lodJob = new UpdateLODJob
        {
            cameraPosition = cameraPosition,
            currentTime = currentTime,
            lodUpdateInterval = 0.1f
        };
        
        state.Dependency = lodJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct UpdateLODJob : IJobEntity
{
    [ReadOnly] public float3 cameraPosition;
    [ReadOnly] public float currentTime;
    [ReadOnly] public float lodUpdateInterval;
    
    public void Execute(ref LODComponent lod, in LocalTransform transform)
    {
        if (currentTime - lod.lastLODUpdate < lodUpdateInterval) return;
        
        var distance = math.distance(cameraPosition, transform.Position);
        var newLODLevel = CalculateLODLevel(distance);
        
        if (newLODLevel != lod.currentLODLevel)
        {
            lod.currentLODLevel = newLODLevel;
            // LOD change handled by rendering system
        }
        
        lod.lastLODUpdate = currentTime;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CalculateLODLevel(float distance)
    {
        // LOD thresholds optimized for RTS camera distances
        if (distance < 20f) return 0; // High detail
        if (distance < 50f) return 1; // Medium detail
        if (distance < 100f) return 2; // Low detail
        return 3; // Impostor/billboard
    }
}
```

## Memory Management

### Object Pooling Patterns
```csharp
// High-performance object pooling for RTS projectiles
public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int initialPoolSize = 100;
    [SerializeField] private int maxPoolSize = 500;
    
    private Stack<GameObject> availableProjectiles;
    private HashSet<GameObject> activeProjectiles;
    
    void Awake()
    {
        availableProjectiles = new Stack<GameObject>(initialPoolSize);
        activeProjectiles = new HashSet<GameObject>();
        
        // Pre-allocate initial pool
        for (int i = 0; i < initialPoolSize; i++)
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.SetActive(false);
            availableProjectiles.Push(projectile);
        }
    }
    
    public GameObject GetProjectile()
    {
        GameObject projectile;
        
        if (availableProjectiles.Count > 0)
        {
            projectile = availableProjectiles.Pop();
        }
        else if (activeProjectiles.Count < maxPoolSize)
        {
            projectile = Instantiate(projectilePrefab);
        }
        else
        {
            // Pool exhausted, reuse oldest active projectile
            projectile = activeProjectiles.First();
            ReturnProjectile(projectile);
        }
        
        projectile.SetActive(true);
        activeProjectiles.Add(projectile);
        return projectile;
    }
    
    public void ReturnProjectile(GameObject projectile)
    {
        if (!activeProjectiles.Contains(projectile)) return;
        
        projectile.SetActive(false);
        activeProjectiles.Remove(projectile);
        availableProjectiles.Push(projectile);
    }
}

// ECS-based pooling for pure performance
public partial struct ProjectileSpawnSystem : ISystem
{
    private EntityQuery projectileQuery;
    private EntityArchetype projectileArchetype;
    
    public void OnCreate(ref SystemState state)
    {
        projectileArchetype = state.EntityManager.CreateArchetype(
            typeof(LocalTransform),
            typeof(ProjectileComponent),
            typeof(VelocityComponent),
            typeof(LifetimeComponent)
        );
        
        // Pre-allocate entity pool
        var entities = state.EntityManager.CreateEntity(projectileArchetype, 1000);
        
        // Disable all pooled entities initially
        foreach (var entity in entities)
        {
            state.EntityManager.AddComponent<DisabledTag>(entity);
        }
    }
}
```

### Memory-Efficient Data Structures
```csharp
// Cache-friendly spatial partitioning for unit queries
public struct SpatialGrid
{
    private NativeMultiHashMap<int, Entity> grid;
    private int gridSize;
    private float cellSize;
    
    public SpatialGrid(int size, float cellSize, Allocator allocator)
    {
        this.gridSize = size;
        this.cellSize = cellSize;
        this.grid = new NativeMultiHashMap<int, Entity>(size * size, allocator);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddEntity(Entity entity, float3 position)
    {
        var cellIndex = GetCellIndex(position);
        grid.Add(cellIndex, entity);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeArray<Entity> GetNearbyEntities(float3 position, float radius, Allocator allocator)
    {
        var results = new NativeList<Entity>(allocator);
        var cellRadius = (int)math.ceil(radius / cellSize);
        var centerCell = GetCellIndex(position);
        
        // Check surrounding cells
        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int z = -cellRadius; z <= cellRadius; z++)
            {
                var cellIndex = centerCell + x + z * gridSize;
                if (grid.TryGetFirstValue(cellIndex, out var entity, out var iterator))
                {
                    do
                    {
                        results.Add(entity);
                    } while (grid.TryGetNextValue(out entity, ref iterator));
                }
            }
        }
        
        return results.AsArray();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetCellIndex(float3 position)
    {
        var x = (int)(position.x / cellSize);
        var z = (int)(position.z / cellSize);
        return x + z * gridSize;
    }
}
```

## AI Performance Workflows

### 1. Profiler-Driven Optimization
```
Step 1: Baseline Measurement
"Analyze Unity Profiler data to establish performance baseline"

Step 2: Bottleneck Identification
"Identify top 5 performance bottlenecks from profiler data"

Step 3: Optimization Priority
"Rank optimizations by impact vs effort ratio"

Step 4: Implementation
"Generate optimized code for highest priority bottlenecks"

Step 5: Validation
"Measure improvement and validate no regressions"
```

### 2. Memory Optimization Pipeline
```
Memory Analysis Workflow:

Step 1: Memory Profiling
"Analyze memory usage patterns and identify allocations"

Step 2: Allocation Hotspots
"Find systems causing frequent GC collections"

Step 3: Pooling Strategy
"Design object pooling for frequently allocated objects"

Step 4: Data Structure Optimization
"Convert to cache-friendly, value-type data structures"

Step 5: Monitoring
"Add memory tracking to prevent future regressions"
```

### 3. Burst Compilation Optimization
```
Burst Optimization Process:

Step 1: Burst Compatibility Analysis
"Review code for Burst compilation eligibility"

Step 2: Managed Code Elimination
"Remove or isolate managed object dependencies"

Step 3: Math Library Optimization
"Replace Unity Mathf with Unity.Mathematics"

Step 4: Inlining Optimization
"Apply aggressive inlining for hot paths"

Step 5: Compilation Verification
"Validate Burst compilation and measure performance gain"
```

## Performance Monitoring

### Real-Time Performance Tracking
```csharp
public class PerformanceMonitor : MonoBehaviour
{
    [Header("Performance Targets")]
    [SerializeField] private float targetFrameTime = 16.67f; // 60 FPS
    [SerializeField] private int maxEntityCount = 500;
    [SerializeField] private float maxMemoryUsage = 2048f; // MB
    
    [Header("Monitoring")]
    [SerializeField] private bool enableContinuousMonitoring = true;
    [SerializeField] private float monitoringInterval = 1f;
    
    private float lastMonitorTime;
    private MovingAverage frameTimeAverage;
    private MovingAverage memoryAverage;
    
    void Start()
    {
        frameTimeAverage = new MovingAverage(60); // 1 second at 60 FPS
        memoryAverage = new MovingAverage(10);
    }
    
    void Update()
    {
        if (!enableContinuousMonitoring) return;
        
        var frameTime = Time.deltaTime * 1000f;
        frameTimeAverage.AddSample(frameTime);
        
        if (Time.time - lastMonitorTime > monitoringInterval)
        {
            PerformPerformanceCheck();
            lastMonitorTime = Time.time;
        }
    }
    
    void PerformPerformanceCheck()
    {
        var avgFrameTime = frameTimeAverage.Average;
        var currentMemory = System.GC.GetTotalMemory(false) / 1024f / 1024f;
        var entityCount = World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities().Length;
        
        memoryAverage.AddSample(currentMemory);
        
        // Check performance thresholds
        if (avgFrameTime > targetFrameTime)
        {
            Debug.LogWarning($"Performance Warning: Average frame time {avgFrameTime:F2}ms exceeds target {targetFrameTime:F2}ms");
            LogPerformanceDetails();
        }
        
        if (currentMemory > maxMemoryUsage)
        {
            Debug.LogWarning($"Memory Warning: Usage {currentMemory:F1}MB exceeds target {maxMemoryUsage:F1}MB");
        }
        
        if (entityCount > maxEntityCount)
        {
            Debug.LogWarning($"Entity Warning: Count {entityCount} exceeds target {maxEntityCount}");
        }
    }
    
    void LogPerformanceDetails()
    {
        Debug.Log($"Performance Details:");
        Debug.Log($"  Frame Time: {frameTimeAverage.Average:F2}ms (Target: {targetFrameTime:F2}ms)");
        Debug.Log($"  Memory: {memoryAverage.Average:F1}MB (Target: <{maxMemoryUsage:F1}MB)");
        Debug.Log($"  Entities: {World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities().Length}");
        Debug.Log($"  Draw Calls: {UnityEngine.Rendering.FrameDebugger.GetFrameEventCount()}");
    }
}

public struct MovingAverage
{
    private float[] samples;
    private int currentIndex;
    private int sampleCount;
    private bool hasWrapped;
    
    public MovingAverage(int windowSize)
    {
        samples = new float[windowSize];
        currentIndex = 0;
        sampleCount = 0;
        hasWrapped = false;
    }
    
    public void AddSample(float sample)
    {
        samples[currentIndex] = sample;
        currentIndex = (currentIndex + 1) % samples.Length;
        
        if (!hasWrapped)
        {
            sampleCount++;
            if (currentIndex == 0) hasWrapped = true;
        }
    }
    
    public float Average
    {
        get
        {
            if (sampleCount == 0) return 0f;
            
            float sum = 0f;
            int count = hasWrapped ? samples.Length : sampleCount;
            
            for (int i = 0; i < count; i++)
            {
                sum += samples[i];
            }
            
            return sum / count;
        }
    }
}
```

This comprehensive performance guide ensures your RTS game maintains 60 FPS with hundreds of units while leveraging AI assistance for optimization.