# Unity Package Management Guide for FrostRealm Chronicles

## Overview
This guide provides comprehensive package management strategies for Unity 6.1 LTS development of FrostRealm Chronicles. It covers essential packages, version management, dependency resolution, and automated package configuration for efficient RTS development workflow.

## Unity 6.1 LTS Package Ecosystem

### Core Unity Packages (Required)
```json
{
  "dependencies": {
    "com.unity.entities": "1.2.0-pre.12",
    "com.unity.netcode.gameobjects": "1.8.1",
    "com.unity.render-pipelines.high-definition": "16.0.6",
    "com.unity.inputsystem": "1.7.0",
    "com.unity.ui.toolkit": "1.0.0-preview.18",
    "com.unity.visualeffectgraph": "16.0.6",
    "com.unity.shadergraph": "16.0.6",
    "com.unity.timeline": "1.8.6",
    "com.unity.cinemachine": "2.10.0",
    "com.unity.addressables": "1.21.19",
    "com.unity.analytics": "5.0.0",
    "com.unity.cloud.build": "1.0.2",
    "com.unity.test-framework": "1.3.9"
  }
}
```

### Performance and ECS Packages
```json
{
  "dependencies": {
    "com.unity.entities.graphics": "1.2.0-pre.12",
    "com.unity.physics": "1.2.3",
    "com.unity.burst": "1.8.12",
    "com.unity.jobs": "0.70.0-preview.7",
    "com.unity.collections": "2.2.1",
    "com.unity.mathematics": "1.3.1",
    "com.unity.profiling.core": "1.0.2"
  }
}
```

### Audio and Multimedia Packages
```json
{
  "dependencies": {
    "com.unity.audio.mixer": "1.0.0",
    "com.unity.video": "1.0.0",
    "com.unity.recorder": "4.0.3"
  }
}
```

## AI-Assisted Package Selection

### Package Recommendation System
```
Analyze Unity package requirements for FrostRealm Chronicles RTS:

Game Requirements:
- 500+ units with 60 FPS performance target
- Multiplayer networking for 2-8 players
- Campaign system with cutscenes and progression
- High-quality visuals (Warcraft III: Reforged level)
- Cross-platform compatibility (Windows/macOS)
- Modding support for custom maps/units

Technical Constraints:
- Unity 6.1 LTS only
- Free packages preferred (avoid paid assets)
- Stable packages only (no experimental)
- Minimal dependency conflicts
- Easy CI/CD integration

Package Categories Needed:
1. Core ECS/DOTS for performance
2. Networking for multiplayer
3. Rendering for visual quality
4. Audio for immersive experience
5. UI for game interface
6. Testing for quality assurance
7. Analytics for player insights
8. Build automation for deployment

Output: Curated package list with version specifications, rationale for each choice, and integration notes.
```

### Package Evaluation Criteria
```csharp
// AI-generated package evaluation system
[System.Serializable]
public class PackageEvaluation
{
    [Header("Package Information")]
    public string packageName;
    public string currentVersion;
    public string recommendedVersion;
    public PackageStability stability;
    public PackageSupport supportLevel;
    
    [Header("Evaluation Criteria")]
    [Range(1, 5)] public int performanceImpact = 3;
    [Range(1, 5)] public int developmentVelocity = 3;
    [Range(1, 5)] public int communitySupport = 3;
    [Range(1, 5)] public int documentationQuality = 3;
    [Range(1, 5)] public int updateFrequency = 3;
    
    [Header("Project Compatibility")]
    public bool compatibleWithECS = true;
    public bool compatibleWithHDRP = true;
    public bool compatibleWithNetcode = true;
    public bool hasKnownIssues = false;
    
    [Header("Decision")]
    public RecommendationStatus recommendation;
    public string rationale;
    public List<string> alternatives;
}

public enum PackageStability
{
    Experimental, Preview, PreRelease, Release, LTS
}

public enum PackageSupport
{
    CommunityOnly, UnitySupported, UnityOwned
}

public enum RecommendationStatus
{
    StronglyRecommended, Recommended, Conditional, NotRecommended, Avoid
}

public class PackageManager : MonoBehaviour
{
    [Header("Package Evaluation")]
    [SerializeField] private PackageEvaluation[] availablePackages;
    [SerializeField] private PackageConfiguration targetConfiguration;
    
    [System.Serializable]
    public class PackageConfiguration
    {
        public GameType gameType = GameType.RTS;
        public PerformanceTarget performanceTarget = PerformanceTarget.High;
        public DevelopmentPhase currentPhase = DevelopmentPhase.EarlyDevelopment;
        public bool allowPreviewPackages = false;
        public bool prioritizePerformance = true;
    }
    
    public enum GameType
    {
        RTS, FPS, RPG, Platformer, Puzzle
    }
    
    public enum PerformanceTarget
    {
        Low, Medium, High, Ultra
    }
    
    public enum DevelopmentPhase
    {
        EarlyDevelopment, Production, Polish, Maintenance
    }
    
    [ContextMenu("Evaluate All Packages")]
    public void EvaluateAllPackages()
    {
        Debug.Log("=== PACKAGE EVALUATION REPORT ===");
        
        var recommendedPackages = new List<PackageEvaluation>();
        var warningPackages = new List<PackageEvaluation>();
        var avoidPackages = new List<PackageEvaluation>();
        
        foreach (var package in availablePackages)
        {
            var score = CalculatePackageScore(package);
            var recommendation = DetermineRecommendation(package, score);
            
            package.recommendation = recommendation;
            package.rationale = GenerateRationale(package, score);
            
            switch (recommendation)
            {
                case RecommendationStatus.StronglyRecommended:
                case RecommendationStatus.Recommended:
                    recommendedPackages.Add(package);
                    break;
                case RecommendationStatus.Conditional:
                    warningPackages.Add(package);
                    break;
                case RecommendationStatus.NotRecommended:
                case RecommendationStatus.Avoid:
                    avoidPackages.Add(package);
                    break;
            }
        }
        
        OutputRecommendations(recommendedPackages, warningPackages, avoidPackages);
        GenerateManifestFile(recommendedPackages);
    }
    
    float CalculatePackageScore(PackageEvaluation package)
    {
        var score = 0f;
        
        // Base metrics (weighted)
        score += package.performanceImpact * 0.3f;
        score += package.developmentVelocity * 0.25f;
        score += package.communitySupport * 0.2f;
        score += package.documentationQuality * 0.15f;
        score += package.updateFrequency * 0.1f;
        
        // Stability bonus
        score += GetStabilityBonus(package.stability);
        
        // Support bonus
        score += GetSupportBonus(package.supportLevel);
        
        // Compatibility requirements (critical)
        if (!package.compatibleWithECS && targetConfiguration.gameType == GameType.RTS)
            score *= 0.5f; // Major penalty for RTS without ECS
        
        if (!package.compatibleWithHDRP)
            score *= 0.7f; // Penalty for HDRP incompatibility
        
        if (!package.compatibleWithNetcode)
            score *= 0.8f; // Penalty for networking incompatibility
        
        if (package.hasKnownIssues)
            score *= 0.6f; // Major penalty for known issues
        
        return score;
    }
    
    float GetStabilityBonus(PackageStability stability)
    {
        switch (stability)
        {
            case PackageStability.LTS: return 2f;
            case PackageStability.Release: return 1.5f;
            case PackageStability.PreRelease: return 1f;
            case PackageStability.Preview: return targetConfiguration.allowPreviewPackages ? 0.5f : -1f;
            case PackageStability.Experimental: return -2f;
            default: return 0f;
        }
    }
    
    float GetSupportBonus(PackageSupport support)
    {
        switch (support)
        {
            case PackageSupport.UnityOwned: return 1.5f;
            case PackageSupport.UnitySupported: return 1f;
            case PackageSupport.CommunityOnly: return 0.5f;
            default: return 0f;
        }
    }
    
    RecommendationStatus DetermineRecommendation(PackageEvaluation package, float score)
    {
        if (score >= 7f) return RecommendationStatus.StronglyRecommended;
        if (score >= 5f) return RecommendationStatus.Recommended;
        if (score >= 3f) return RecommendationStatus.Conditional;
        if (score >= 1f) return RecommendationStatus.NotRecommended;
        return RecommendationStatus.Avoid;
    }
    
    string GenerateRationale(PackageEvaluation package, float score)
    {
        var rationale = new System.Text.StringBuilder();
        
        rationale.Append($"Score: {score:F1}/10. ");
        
        if (package.stability == PackageStability.LTS)
            rationale.Append("LTS stability. ");
        else if (package.stability == PackageStability.Experimental)
            rationale.Append("Experimental - high risk. ");
        
        if (package.supportLevel == PackageSupport.UnityOwned)
            rationale.Append("Unity-owned package. ");
        
        if (!package.compatibleWithECS && targetConfiguration.gameType == GameType.RTS)
            rationale.Append("WARNING: Not ECS compatible. ");
        
        if (package.hasKnownIssues)
            rationale.Append("WARNING: Known issues reported. ");
        
        if (package.performanceImpact >= 4)
            rationale.Append("High performance benefit. ");
        else if (package.performanceImpact <= 2)
            rationale.Append("Performance concerns. ");
        
        return rationale.ToString().Trim();
    }
    
    void OutputRecommendations(List<PackageEvaluation> recommended, 
                              List<PackageEvaluation> warnings, 
                              List<PackageEvaluation> avoid)
    {
        if (recommended.Any())
        {
            Debug.Log("RECOMMENDED PACKAGES:");
            foreach (var package in recommended.OrderByDescending(p => CalculatePackageScore(p)))
            {
                Debug.Log($"  ✓ {package.packageName} v{package.recommendedVersion}");
                Debug.Log($"    {package.rationale}");
            }
        }
        
        if (warnings.Any())
        {
            Debug.LogWarning("CONDITIONAL PACKAGES (Use with caution):");
            foreach (var package in warnings)
            {
                Debug.LogWarning($"  ⚠ {package.packageName} v{package.recommendedVersion}");
                Debug.LogWarning($"    {package.rationale}");
            }
        }
        
        if (avoid.Any())
        {
            Debug.LogError("PACKAGES TO AVOID:");
            foreach (var package in avoid)
            {
                Debug.LogError($"  ✗ {package.packageName}");
                Debug.LogError($"    {package.rationale}");
                
                if (package.alternatives.Any())
                {
                    Debug.Log($"    Alternatives: {string.Join(", ", package.alternatives)}");
                }
            }
        }
    }
    
    void GenerateManifestFile(List<PackageEvaluation> recommendedPackages)
    {
        var manifest = new
        {
            dependencies = recommendedPackages.ToDictionary(
                p => p.packageName,
                p => p.recommendedVersion
            )
        };
        
        var json = JsonUtility.ToJson(manifest, true);
        var manifestPath = "Packages/manifest_recommended.json";
        
        System.IO.File.WriteAllText(manifestPath, json);
        Debug.Log($"Generated recommended manifest: {manifestPath}");
    }
}
```

## Package Configuration for RTS Development

### Essential RTS Package Stack
```
Core ECS/DOTS Stack:
- com.unity.entities: Entity Component System architecture
- com.unity.entities.graphics: ECS rendering integration
- com.unity.physics: High-performance physics simulation
- com.unity.burst: Native code compilation
- com.unity.jobs: Parallel job system
- com.unity.collections: High-performance collections
- com.unity.mathematics: SIMD math library

Rationale: RTS games require handling hundreds of units simultaneously. ECS/DOTS provides the performance architecture needed for 500+ unit battles at 60 FPS.

Networking Stack:
- com.unity.netcode.gameobjects: Multiplayer framework
- com.unity.transport: Low-level networking
- com.unity.relay: Connection facilitation
- com.unity.lobby: Matchmaking services

Rationale: Multiplayer RTS requires authoritative server, lag compensation, and smooth multiplayer experience for 2-8 players.

Rendering Stack:
- com.unity.render-pipelines.high-definition: Advanced rendering
- com.unity.visualeffectgraph: Particle systems
- com.unity.shadergraph: Custom shader creation
- com.unity.postprocessing: Visual effects

Rationale: Warcraft III: Reforged quality visuals require modern rendering pipeline with advanced lighting and effects.

Audio Stack:
- com.unity.audio.mixer: Advanced audio mixing
- com.unity.audio.dsp: Audio processing
- FMOD Studio (External): Professional audio middleware

Rationale: RTS games need spatial audio, dynamic music, and complex audio mixing for immersive experience.
```

### Package Version Management Strategy
```csharp
// AI-generated version management system
public class PackageVersionManager : MonoBehaviour
{
    [Header("Version Management")]
    [SerializeField] private VersionPolicy versionPolicy;
    [SerializeField] private bool autoUpdatePatch = true;
    [SerializeField] private bool autoUpdateMinor = false;
    [SerializeField] private bool autoUpdateMajor = false;
    
    [System.Serializable]
    public class VersionPolicy
    {
        public UpdateFrequency checkFrequency = UpdateFrequency.Weekly;
        public RiskTolerance riskTolerance = RiskTolerance.Conservative;
        public bool allowPreviewInDevelopment = false;
        public bool allowExperimentalFeatures = false;
        public List<string> pinnedPackages = new List<string>();
    }
    
    public enum UpdateFrequency
    {
        Daily, Weekly, Monthly, Manual
    }
    
    public enum RiskTolerance
    {
        Conservative, Balanced, Aggressive
    }
    
    [System.Serializable]
    public class PackageUpdate
    {
        public string packageName;
        public string currentVersion;
        public string availableVersion;
        public UpdateType updateType;
        public RiskLevel riskLevel;
        public string changelog;
        public bool hasBreakingChanges;
        public List<string> affectedSystems;
    }
    
    public enum UpdateType
    {
        Patch, Minor, Major, Preview, Experimental
    }
    
    public enum RiskLevel
    {
        Low, Medium, High, Critical
    }
    
    void Start()
    {
        if (versionPolicy.checkFrequency != UpdateFrequency.Manual)
        {
            var intervalHours = GetCheckIntervalHours(versionPolicy.checkFrequency);
            InvokeRepeating(nameof(CheckForUpdates), 0f, intervalHours * 3600f);
        }
    }
    
    float GetCheckIntervalHours(UpdateFrequency frequency)
    {
        switch (frequency)
        {
            case UpdateFrequency.Daily: return 24f;
            case UpdateFrequency.Weekly: return 168f; // 7 days
            case UpdateFrequency.Monthly: return 720f; // 30 days
            default: return float.MaxValue;
        }
    }
    
    [ContextMenu("Check for Package Updates")]
    public void CheckForUpdates()
    {
        var availableUpdates = ScanForAvailableUpdates();
        var filteredUpdates = FilterUpdatesByPolicy(availableUpdates);
        var categorizedUpdates = CategorizeUpdatesByRisk(filteredUpdates);
        
        ReportAvailableUpdates(categorizedUpdates);
        
        if (ShouldAutoUpdate())
        {
            ApplyAutomaticUpdates(categorizedUpdates);
        }
    }
    
    List<PackageUpdate> ScanForAvailableUpdates()
    {
        // In real implementation, this would query Unity Package Manager API
        var updates = new List<PackageUpdate>();
        
        // Simulate package update detection
        var currentManifest = LoadCurrentManifest();
        var latestVersions = QueryLatestVersions();
        
        foreach (var package in currentManifest)
        {
            if (latestVersions.ContainsKey(package.Key))
            {
                var current = package.Value;
                var latest = latestVersions[package.Key];
                
                if (CompareVersions(current, latest) < 0)
                {
                    updates.Add(new PackageUpdate
                    {
                        packageName = package.Key,
                        currentVersion = current,
                        availableVersion = latest,
                        updateType = DetermineUpdateType(current, latest),
                        riskLevel = AssessUpdateRisk(package.Key, current, latest),
                        hasBreakingChanges = CheckForBreakingChanges(package.Key, current, latest)
                    });
                }
            }
        }
        
        return updates;
    }
    
    List<PackageUpdate> FilterUpdatesByPolicy(List<PackageUpdate> updates)
    {
        return updates.Where(update => IsUpdateAllowedByPolicy(update)).ToList();
    }
    
    bool IsUpdateAllowedByPolicy(PackageUpdate update)
    {
        // Check if package is pinned
        if (versionPolicy.pinnedPackages.Contains(update.packageName))
            return false;
        
        // Check risk tolerance
        switch (versionPolicy.riskTolerance)
        {
            case RiskTolerance.Conservative:
                return update.riskLevel <= RiskLevel.Low && 
                       update.updateType <= UpdateType.Minor &&
                       !update.hasBreakingChanges;
                
            case RiskTolerance.Balanced:
                return update.riskLevel <= RiskLevel.Medium &&
                       update.updateType <= UpdateType.Major;
                
            case RiskTolerance.Aggressive:
                return update.riskLevel <= RiskLevel.High;
                
            default:
                return false;
        }
    }
    
    Dictionary<RiskLevel, List<PackageUpdate>> CategorizeUpdatesByRisk(List<PackageUpdate> updates)
    {
        return updates.GroupBy(u => u.riskLevel)
                     .ToDictionary(g => g.Key, g => g.ToList());
    }
    
    void ReportAvailableUpdates(Dictionary<RiskLevel, List<PackageUpdate>> categorizedUpdates)
    {
        if (!categorizedUpdates.Any())
        {
            Debug.Log("No package updates available matching current policy");
            return;
        }
        
        Debug.Log("=== PACKAGE UPDATES AVAILABLE ===");
        
        foreach (var category in categorizedUpdates.OrderBy(kvp => kvp.Key))
        {
            var riskLevel = category.Key;
            var updates = category.Value;
            
            Debug.Log($"\n{riskLevel} Risk Updates ({updates.Count}):");
            
            foreach (var update in updates)
            {
                var symbol = GetUpdateSymbol(update.updateType, update.riskLevel);
                Debug.Log($"  {symbol} {update.packageName}: {update.currentVersion} → {update.availableVersion}");
                
                if (update.hasBreakingChanges)
                {
                    Debug.LogWarning($"    ⚠ Breaking changes detected");
                }
                
                if (update.affectedSystems.Any())
                {
                    Debug.Log($"    Affects: {string.Join(", ", update.affectedSystems)}");
                }
            }
        }
    }
    
    string GetUpdateSymbol(UpdateType updateType, RiskLevel riskLevel)
    {
        if (riskLevel >= RiskLevel.High) return "🔥";
        if (updateType >= UpdateType.Major) return "⬆️";
        if (updateType >= UpdateType.Minor) return "📈";
        return "🔧";
    }
    
    bool ShouldAutoUpdate()
    {
        return autoUpdatePatch || autoUpdateMinor || autoUpdateMajor;
    }
    
    void ApplyAutomaticUpdates(Dictionary<RiskLevel, List<PackageUpdate>> categorizedUpdates)
    {
        var autoUpdates = new List<PackageUpdate>();
        
        foreach (var category in categorizedUpdates)
        {
            foreach (var update in category.Value)
            {
                if (IsAutoUpdateAllowed(update))
                {
                    autoUpdates.Add(update);
                }
            }
        }
        
        if (autoUpdates.Any())
        {
            Debug.Log($"Auto-updating {autoUpdates.Count} packages...");
            
            foreach (var update in autoUpdates)
            {
                ApplyPackageUpdate(update);
            }
            
            Debug.Log("Automatic updates complete. Consider restarting Unity Editor.");
        }
    }
    
    bool IsAutoUpdateAllowed(PackageUpdate update)
    {
        switch (update.updateType)
        {
            case UpdateType.Patch:
                return autoUpdatePatch && update.riskLevel <= RiskLevel.Low;
            case UpdateType.Minor:
                return autoUpdateMinor && update.riskLevel <= RiskLevel.Medium && !update.hasBreakingChanges;
            case UpdateType.Major:
                return autoUpdateMajor && update.riskLevel <= RiskLevel.Medium && !update.hasBreakingChanges;
            default:
                return false;
        }
    }
    
    void ApplyPackageUpdate(PackageUpdate update)
    {
        Debug.Log($"Updating {update.packageName} from {update.currentVersion} to {update.availableVersion}");
        
        // In real implementation, this would use Unity Package Manager API
        // UnityEditor.PackageManager.Client.Add($"{update.packageName}@{update.availableVersion}");
        
        // Log the update for tracking
        LogPackageUpdate(update);
    }
    
    void LogPackageUpdate(PackageUpdate update)
    {
        var logEntry = $"{System.DateTime.Now:yyyy-MM-dd HH:mm:ss} - Updated {update.packageName} from {update.currentVersion} to {update.availableVersion}";
        var logPath = "Logs/package_updates.log";
        
        System.IO.File.AppendAllText(logPath, logEntry + System.Environment.NewLine);
    }
}
```

## Dependency Resolution and Conflicts

### AI Conflict Detection
```
Design dependency conflict resolution system for Unity packages:

Common Conflict Scenarios:
- Version incompatibilities between packages
- API breaking changes in dependencies
- Circular dependency loops
- Platform-specific package conflicts
- Performance conflicts (multiple packages doing similar things)

Conflict Detection Methods:
- Static analysis of package manifests
- Runtime compatibility testing
- Performance impact analysis
- API surface comparison
- Community feedback integration

Resolution Strategies:
- Automatic version pinning for stable combinations
- Alternative package suggestions
- Dependency tree optimization
- Gradual migration paths for breaking changes
- Rollback procedures for problematic updates

Risk Mitigation:
- Backup manifest files before changes
- Staged deployment in development/staging/production
- Automated testing after package updates
- Performance regression detection
- Community notification of issues

Generate automated conflict detection system with resolution recommendations.
```

### Conflict Resolution System
```csharp
// AI-generated dependency conflict resolver
public class DependencyConflictResolver : MonoBehaviour
{
    [Header("Conflict Detection")]
    [SerializeField] private ConflictSeverity maxAllowedSeverity = ConflictSeverity.Medium;
    [SerializeField] private bool autoResolveMinorConflicts = true;
    [SerializeField] private ConflictResolutionStrategy defaultStrategy = ConflictResolutionStrategy.Conservative;
    
    [System.Serializable]
    public class DependencyConflict
    {
        public string conflictId;
        public ConflictType type;
        public ConflictSeverity severity;
        public List<string> affectedPackages;
        public string description;
        public List<ResolutionOption> resolutionOptions;
        public string recommendedAction;
    }
    
    public enum ConflictType
    {
        VersionIncompatibility, APIBreaking, CircularDependency, 
        PerformanceConflict, PlatformIncompatibility
    }
    
    public enum ConflictSeverity
    {
        Low, Medium, High, Critical
    }
    
    public enum ConflictResolutionStrategy
    {
        Conservative, Balanced, Aggressive
    }
    
    [System.Serializable]
    public class ResolutionOption
    {
        public string description;
        public List<PackageAction> actions;
        public ConflictSeverity riskLevel;
        public float successProbability;
        public string sideEffects;
    }
    
    [System.Serializable]
    public class PackageAction
    {
        public ActionType type;
        public string packageName;
        public string targetVersion;
        public string rationale;
    }
    
    public enum ActionType
    {
        Update, Downgrade, Remove, Add, Pin
    }
    
    [ContextMenu("Analyze Dependency Conflicts")]
    public void AnalyzeDependencyConflicts()
    {
        var conflicts = DetectConflicts();
        var resolvedConflicts = new List<DependencyConflict>();
        var unresolvedConflicts = new List<DependencyConflict>();
        
        foreach (var conflict in conflicts)
        {
            if (conflict.severity <= maxAllowedSeverity)
            {
                var resolution = GenerateResolution(conflict);
                if (resolution != null)
                {
                    conflict.resolutionOptions.Add(resolution);
                    resolvedConflicts.Add(conflict);
                    
                    if (autoResolveMinorConflicts && conflict.severity <= ConflictSeverity.Low)
                    {
                        ApplyResolution(conflict, resolution);
                    }
                }
                else
                {
                    unresolvedConflicts.Add(conflict);
                }
            }
            else
            {
                unresolvedConflicts.Add(conflict);
            }
        }
        
        ReportConflictAnalysis(resolvedConflicts, unresolvedConflicts);
    }
    
    List<DependencyConflict> DetectConflicts()
    {
        var conflicts = new List<DependencyConflict>();
        
        // Version incompatibility detection
        conflicts.AddRange(DetectVersionIncompatibilities());
        
        // API breaking change detection
        conflicts.AddRange(DetectAPIBreakingChanges());
        
        // Circular dependency detection
        conflicts.AddRange(DetectCircularDependencies());
        
        // Performance conflict detection
        conflicts.AddRange(DetectPerformanceConflicts());
        
        return conflicts;
    }
    
    List<DependencyConflict> DetectVersionIncompatibilities()
    {
        var conflicts = new List<DependencyConflict>();
        var manifest = LoadCurrentManifest();
        
        // Check for packages with conflicting version requirements
        var versionRequirements = AnalyzeVersionRequirements(manifest);
        
        foreach (var package in versionRequirements)
        {
            var incompatibleVersions = FindIncompatibleVersions(package.Key, package.Value);
            
            if (incompatibleVersions.Any())
            {
                conflicts.Add(new DependencyConflict
                {
                    conflictId = $"version_conflict_{package.Key}",
                    type = ConflictType.VersionIncompatibility,
                    severity = ConflictSeverity.Medium,
                    affectedPackages = incompatibleVersions,
                    description = $"Version incompatibility detected for {package.Key}",
                    resolutionOptions = new List<ResolutionOption>()
                });
            }
        }
        
        return conflicts;
    }
    
    List<DependencyConflict> DetectAPIBreakingChanges()
    {
        var conflicts = new List<DependencyConflict>();
        
        // Analyze API surface changes between versions
        var apiChanges = AnalyzeAPIChanges();
        
        foreach (var change in apiChanges)
        {
            if (change.IsBreaking)
            {
                conflicts.Add(new DependencyConflict
                {
                    conflictId = $"api_breaking_{change.PackageName}",
                    type = ConflictType.APIBreaking,
                    severity = DetermineSeverity(change),
                    affectedPackages = new List<string> { change.PackageName },
                    description = $"Breaking API changes in {change.PackageName}: {change.Description}",
                    resolutionOptions = new List<ResolutionOption>()
                });
            }
        }
        
        return conflicts;
    }
    
    List<DependencyConflict> DetectPerformanceConflicts()
    {
        var conflicts = new List<DependencyConflict>();
        
        // Detect packages that provide overlapping functionality
        var functionalityMap = AnalyzePackageFunctionality();
        var overlaps = FindFunctionalityOverlaps(functionalityMap);
        
        foreach (var overlap in overlaps)
        {
            if (overlap.PerformanceImpact > 0.1f) // 10% performance impact
            {
                conflicts.Add(new DependencyConflict
                {
                    conflictId = $"performance_overlap_{string.Join("_", overlap.Packages)}",
                    type = ConflictType.PerformanceConflict,
                    severity = ConflictSeverity.Low,
                    affectedPackages = overlap.Packages,
                    description = $"Performance overlap detected: {overlap.Description}",
                    resolutionOptions = new List<ResolutionOption>()
                });
            }
        }
        
        return conflicts;
    }
    
    ResolutionOption GenerateResolution(DependencyConflict conflict)
    {
        switch (conflict.type)
        {
            case ConflictType.VersionIncompatibility:
                return GenerateVersionResolution(conflict);
            
            case ConflictType.APIBreaking:
                return GenerateAPIResolution(conflict);
            
            case ConflictType.PerformanceConflict:
                return GeneratePerformanceResolution(conflict);
            
            default:
                return GenerateGenericResolution(conflict);
        }
    }
    
    ResolutionOption GenerateVersionResolution(DependencyConflict conflict)
    {
        var actions = new List<PackageAction>();
        
        // Find common compatible version
        var compatibleVersion = FindCommonCompatibleVersion(conflict.affectedPackages);
        
        if (compatibleVersion != null)
        {
            foreach (var package in conflict.affectedPackages)
            {
                actions.Add(new PackageAction
                {
                    type = ActionType.Update,
                    packageName = package,
                    targetVersion = compatibleVersion,
                    rationale = "Update to common compatible version"
                });
            }
            
            return new ResolutionOption
            {
                description = $"Update all affected packages to version {compatibleVersion}",
                actions = actions,
                riskLevel = ConflictSeverity.Low,
                successProbability = 0.9f,
                sideEffects = "May require code changes for new API"
            };
        }
        
        return null;
    }
    
    ResolutionOption GeneratePerformanceResolution(DependencyConflict conflict)
    {
        var actions = new List<PackageAction>();
        
        // Recommend removing redundant packages
        var packagesToRemove = IdentifyRedundantPackages(conflict.affectedPackages);
        
        foreach (var package in packagesToRemove)
        {
            actions.Add(new PackageAction
            {
                type = ActionType.Remove,
                packageName = package,
                rationale = "Remove redundant functionality to improve performance"
            });
        }
        
        return new ResolutionOption
        {
            description = "Remove redundant packages to eliminate performance overlap",
            actions = actions,
            riskLevel = ConflictSeverity.Medium,
            successProbability = 0.8f,
            sideEffects = "May need to refactor code using removed packages"
        };
    }
    
    void ApplyResolution(DependencyConflict conflict, ResolutionOption resolution)
    {
        Debug.Log($"Auto-resolving conflict: {conflict.description}");
        Debug.Log($"Resolution: {resolution.description}");
        
        foreach (var action in resolution.actions)
        {
            ApplyPackageAction(action);
        }
        
        LogConflictResolution(conflict, resolution);
    }
    
    void ApplyPackageAction(PackageAction action)
    {
        switch (action.type)
        {
            case ActionType.Update:
                Debug.Log($"Updating {action.packageName} to {action.targetVersion}");
                // Apply update via Package Manager API
                break;
                
            case ActionType.Remove:
                Debug.Log($"Removing {action.packageName}");
                // Remove package via Package Manager API
                break;
                
            case ActionType.Pin:
                Debug.Log($"Pinning {action.packageName} at current version");
                // Add to pinned packages list
                break;
        }
    }
    
    void ReportConflictAnalysis(List<DependencyConflict> resolved, List<DependencyConflict> unresolved)
    {
        Debug.Log("=== DEPENDENCY CONFLICT ANALYSIS ===");
        
        if (resolved.Any())
        {
            Debug.Log($"\nRESOLVED CONFLICTS ({resolved.Count}):");
            foreach (var conflict in resolved)
            {
                Debug.Log($"  ✓ {conflict.description}");
                if (conflict.resolutionOptions.Any())
                {
                    var option = conflict.resolutionOptions.First();
                    Debug.Log($"    Resolution: {option.description}");
                }
            }
        }
        
        if (unresolved.Any())
        {
            Debug.LogWarning($"\nUNRESOLVED CONFLICTS ({unresolved.Count}):");
            foreach (var conflict in unresolved)
            {
                var severitySymbol = GetSeveritySymbol(conflict.severity);
                Debug.LogWarning($"  {severitySymbol} {conflict.description}");
                Debug.LogWarning($"    Affected packages: {string.Join(", ", conflict.affectedPackages)}");
                Debug.LogWarning($"    Manual resolution required");
            }
        }
        
        if (!resolved.Any() && !unresolved.Any())
        {
            Debug.Log("No dependency conflicts detected. ✓");
        }
    }
    
    string GetSeveritySymbol(ConflictSeverity severity)
    {
        switch (severity)
        {
            case ConflictSeverity.Critical: return "🔥";
            case ConflictSeverity.High: return "⚠️";
            case ConflictSeverity.Medium: return "⚡";
            case ConflictSeverity.Low: return "ℹ️";
            default: return "?";
        }
    }
}
```

## Automated Package Setup

### Project Template System
```csharp
// AI-generated project template for RTS games
[CreateAssetMenu(fileName = "RTS Project Template", menuName = "Package Management/Project Template")]
public class RTSProjectTemplate : ScriptableObject
{
    [Header("Template Configuration")]
    [SerializeField] private string templateName = "FrostRealm Chronicles";
    [SerializeField] private string templateVersion = "1.0";
    [SerializeField] private ProjectType projectType = ProjectType.RTS;
    
    [Header("Required Packages")]
    [SerializeField] private PackageDefinition[] requiredPackages;
    [SerializeField] private PackageDefinition[] optionalPackages;
    [SerializeField] private PackageDefinition[] developmentPackages;
    
    [Header("Project Settings")]
    [SerializeField] private ProjectSettings projectSettings;
    [SerializeField] private BuildSettings buildSettings;
    [SerializeField] private QualitySettings qualitySettings;
    
    [System.Serializable]
    public class PackageDefinition
    {
        public string packageName;
        public string version;
        public string description;
        public PackageCategory category;
        public bool autoInstall = true;
        public List<string> dependencies;
    }
    
    public enum PackageCategory
    {
        Core, Rendering, Networking, Audio, UI, Testing, Analytics
    }
    
    public enum ProjectType
    {
        RTS, FPS, RPG, Platformer, Mobile
    }
    
    [ContextMenu("Apply Template")]
    public void ApplyTemplate()
    {
        Debug.Log($"Applying {templateName} template...");
        
        InstallRequiredPackages();
        ConfigureProjectSettings();
        SetupDirectoryStructure();
        GenerateInitialScripts();
        
        Debug.Log("Template application complete!");
    }
    
    void InstallRequiredPackages()
    {
        Debug.Log("Installing required packages...");
        
        var packagesToInstall = requiredPackages.Where(p => p.autoInstall).ToList();
        
        foreach (var package in packagesToInstall)
        {
            Debug.Log($"  Installing {package.packageName} v{package.version}");
            // Install via Package Manager API
            InstallPackage(package);
        }
        
        // Generate manifest file
        GenerateManifestFile(packagesToInstall);
    }
    
    void InstallPackage(PackageDefinition package)
    {
        // In real implementation, use Unity Package Manager API
        // UnityEditor.PackageManager.Client.Add($"{package.packageName}@{package.version}");
        
        Debug.Log($"Package {package.packageName} installed successfully");
    }
    
    void ConfigureProjectSettings()
    {
        Debug.Log("Configuring project settings...");
        
        // Configure rendering pipeline
        ConfigureRenderPipeline();
        
        // Configure input system
        ConfigureInputSystem();
        
        // Configure physics settings
        ConfigurePhysicsSettings();
        
        // Configure quality settings
        ConfigureQualitySettings();
    }
    
    void ConfigureRenderPipeline()
    {
        Debug.Log("  Configuring HDRP...");
        
        // Set HDRP as active render pipeline
        // Configure HDRP settings for RTS games
        // Setup lighting and post-processing
    }
    
    void ConfigureInputSystem()
    {
        Debug.Log("  Configuring Input System...");
        
        // Enable new Input System
        // Configure RTS-specific input mappings
        // Setup camera controls and unit selection
    }
    
    void SetupDirectoryStructure()
    {
        Debug.Log("Setting up directory structure...");
        
        var directories = new[]
        {
            "Assets/Scripts/Core",
            "Assets/Scripts/UI",
            "Assets/Scripts/Gameplay",
            "Assets/Scripts/Networking",
            "Assets/Scripts/Audio",
            "Assets/Scripts/Tests",
            "Assets/Data/Units",
            "Assets/Data/Buildings", 
            "Assets/Data/Abilities",
            "Assets/Art/Models",
            "Assets/Art/Textures",
            "Assets/Art/Materials",
            "Assets/Audio/Music",
            "Assets/Audio/SFX",
            "Assets/Audio/Voice",
            "Assets/Scenes/Gameplay",
            "Assets/Scenes/UI",
            "Assets/Maps"
        };
        
        foreach (var directory in directories)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                Debug.Log($"  Created directory: {directory}");
            }
        }
    }
    
    void GenerateInitialScripts()
    {
        Debug.Log("Generating initial scripts...");
        
        // Generate basic script templates
        GenerateGameManagerScript();
        GenerateResourceManagerScript();
        GenerateUnitControllerScript();
        GenerateUIManagerScript();
    }
    
    void GenerateGameManagerScript()
    {
        var scriptContent = @"using UnityEngine;
using Unity.Entities;

namespace FrostRealm.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header(""Game State"")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        
        public enum GameState
        {
            MainMenu, Loading, InGame, Paused, GameOver
        }
        
        void Start()
        {
            InitializeGame();
        }
        
        void InitializeGame()
        {
            // Initialize core systems
            InitializeResourceSystem();
            InitializeUnitSystem();
            InitializeUISystem();
            
            Debug.Log(""FrostRealm Chronicles initialized successfully!"");
        }
        
        void InitializeResourceSystem()
        {
            // TODO: Initialize resource management
        }
        
        void InitializeUnitSystem()
        {
            // TODO: Initialize unit management
        }
        
        void InitializeUISystem()
        {
            // TODO: Initialize UI system
        }
    }
}";
        
        var scriptPath = "Assets/Scripts/Core/GameManager.cs";
        System.IO.File.WriteAllText(scriptPath, scriptContent);
        Debug.Log($"  Generated: {scriptPath}");
    }
    
    void GenerateResourceManagerScript()
    {
        var scriptContent = @"using UnityEngine;
using Unity.Entities;

namespace FrostRealm.Core
{
    public class ResourceManager : MonoBehaviour
    {
        [Header(""Starting Resources"")]
        [SerializeField] private int startingGold = 500;
        [SerializeField] private int startingLumber = 150;
        [SerializeField] private int startingFood = 5;
        
        [Header(""Current Resources"")]
        [SerializeField] private int currentGold;
        [SerializeField] private int currentLumber;
        [SerializeField] private int currentFood;
        [SerializeField] private int usedFood;
        
        public static ResourceManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeResources();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void InitializeResources()
        {
            currentGold = startingGold;
            currentLumber = startingLumber;
            currentFood = startingFood;
            usedFood = 0;
        }
        
        public bool CanAfford(int gold, int lumber)
        {
            return currentGold >= gold && currentLumber >= lumber;
        }
        
        public bool TrySpendResources(int gold, int lumber)
        {
            if (CanAfford(gold, lumber))
            {
                currentGold -= gold;
                currentLumber -= lumber;
                OnResourcesChanged?.Invoke();
                return true;
            }
            return false;
        }
        
        public void AddResources(int gold, int lumber)
        {
            currentGold += gold;
            currentLumber += lumber;
            OnResourcesChanged?.Invoke();
        }
        
        public System.Action OnResourcesChanged;
        
        // Properties for UI binding
        public int Gold => currentGold;
        public int Lumber => currentLumber;
        public int Food => currentFood;
        public int UsedFood => usedFood;
        public float UpkeepMultiplier => CalculateUpkeepMultiplier();
        
        float CalculateUpkeepMultiplier()
        {
            // TFT upkeep formula
            if (usedFood <= 50) return 1.0f;
            if (usedFood <= 80) return 0.7f;
            return 0.4f;
        }
    }
}";
        
        var scriptPath = "Assets/Scripts/Core/ResourceManager.cs";
        System.IO.File.WriteAllText(scriptPath, scriptContent);
        Debug.Log($"  Generated: {scriptPath}");
    }
    
    void GenerateManifestFile(List<PackageDefinition> packages)
    {
        var manifest = new
        {
            dependencies = packages.ToDictionary(
                p => p.packageName,
                p => p.version
            )
        };
        
        var json = JsonUtility.ToJson(manifest, true);
        var manifestPath = "Packages/manifest.json";
        
        // Backup existing manifest
        if (System.IO.File.Exists(manifestPath))
        {
            var backupPath = $"Packages/manifest_backup_{System.DateTime.Now:yyyyMMdd_HHmmss}.json";
            System.IO.File.Copy(manifestPath, backupPath);
            Debug.Log($"Backed up existing manifest to: {backupPath}");
        }
        
        System.IO.File.WriteAllText(manifestPath, json);
        Debug.Log($"Generated new manifest: {manifestPath}");
    }
}
```

This comprehensive package management guide ensures your Unity project is properly configured with the right packages for RTS development while providing tools for ongoing maintenance and conflict resolution.