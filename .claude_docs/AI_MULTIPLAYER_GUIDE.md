# AI Multiplayer/Networking Guide for FrostRealm Chronicles

## Overview
This guide provides AI-assisted development workflows for implementing robust multiplayer functionality in FrostRealm Chronicles. Covers Unity Netcode for GameObjects/Entities, RTS-specific networking challenges, AI bot implementation, matchmaking, and anti-cheat systems for competitive play.

## RTS Networking Fundamentals

### Core Challenges
- **Deterministic Simulation**: Ensure identical game state across all clients
- **Command Synchronization**: Coordinate player actions with minimal latency
- **State Reconciliation**: Handle desync and lag compensation
- **Bandwidth Optimization**: Minimize network traffic for large-scale battles
- **Anti-Cheat**: Prevent cheating in competitive environment

### Unity 6.1 Networking Features
- **Netcode for Entities**: High-performance ECS networking
- **Distributed Authority**: Flexible client-server models
- **Host Migration**: Seamless host transition during disconnects
- **Prediction**: Client-side prediction with server reconciliation
- **Compression**: Built-in data compression for efficiency

## AI-Assisted Architecture Design

### Network Architecture Planning
```
Design multiplayer architecture for FrostRealm Chronicles RTS:

Game Requirements:
- 2-8 players simultaneous
- Large unit counts (500+ entities)
- Real-time strategy gameplay
- Campaign co-op support
- Competitive ranked matches

Technical Constraints:
- Unity Netcode for GameObjects/Entities
- Cross-platform compatibility (PC)
- Bandwidth limit: <100 KB/s per player
- Latency tolerance: <200ms acceptable
- Target regions: Global multiplayer

Architecture Decisions:
- Authority Model: [Client-server vs peer-to-peer analysis]
- Synchronization Method: [Command-based vs state-based]
- Persistence Layer: [Match data and statistics storage]
- Anti-Cheat Strategy: [Server validation approach]
- Scalability Plan: [Dedicated server deployment]

Output: Complete networking architecture specification with implementation roadmap
```

### Netcode Integration Strategy
```
Generate Unity Netcode integration plan for RTS game:

Current Systems to Network:
- Resource Management (authoritative)
- Unit Movement (predicted)
- Combat Resolution (server-side)
- Building Construction (validated)
- Hero Abilities (instant response)

Netcode Components Required:
- NetworkObject identification for entities
- NetworkVariable synchronization for game state
- RPC system for player commands
- Custom serialization for RTS data types
- Lag compensation for real-time actions

Performance Considerations:
- Entity count scaling (500+ networked objects)
- Update frequency optimization
- Bandwidth usage minimization
- Memory allocation patterns
- Frame rate maintenance during network operations

Implementation Priority:
1. Basic connection and lobby system
2. Core gameplay synchronization
3. Advanced features (spectating, replays)
4. Optimization and polish
```

## Core Networking Implementation

### AI Network Object Management
```csharp
// AI-generated network object management for RTS entities
[System.Serializable]
public class RTSNetworkObject : NetworkBehaviour
{
    [Header("RTS Network Identity")]
    [SerializeField] private NetworkVariable<int> ownerPlayerID = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<UnitType> unitType = new NetworkVariable<UnitType>();
    [SerializeField] private NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    
    [Header("Position Synchronization")]
    [SerializeField] private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    
    private Vector3 lastSentPosition;
    private const float POSITION_THRESHOLD = 0.1f;
    private const float SEND_RATE = 20f; // Updates per second
    private float lastSendTime;
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            InitializeNetworkVariables();
        }
        
        // Subscribe to network variable changes
        networkPosition.OnValueChanged += OnPositionChanged;
        currentHealth.OnValueChanged += OnHealthChanged;
    }
    
    void Update()
    {
        if (IsOwner && ShouldSendPositionUpdate())
        {
            SendPositionUpdate();
        }
        
        if (!IsOwner)
        {
            InterpolatePosition();
        }
    }
    
    bool ShouldSendPositionUpdate()
    {
        var timeSinceLastSend = Time.time - lastSendTime;
        var positionDelta = Vector3.Distance(transform.position, lastSentPosition);
        
        return timeSinceLastSend > (1f / SEND_RATE) && positionDelta > POSITION_THRESHOLD;
    }
    
    void SendPositionUpdate()
    {
        if (IsServer)
        {
            networkPosition.Value = transform.position;
            networkRotation.Value = transform.rotation;
        }
        else
        {
            UpdatePositionServerRpc(transform.position, transform.rotation);
        }
        
        lastSentPosition = transform.position;
        lastSendTime = Time.time;
    }
    
    [ServerRpc]
    void UpdatePositionServerRpc(Vector3 position, Quaternion rotation)
    {
        // Server validation for movement
        if (IsValidMovement(position))
        {
            networkPosition.Value = position;
            networkRotation.Value = rotation;
        }
    }
    
    bool IsValidMovement(Vector3 newPosition)
    {
        // Anti-cheat: Validate movement speed and boundaries
        var maxMovementPerFrame = GetMaxMovementSpeed() * Time.deltaTime * 2f; // Generous buffer
        var distance = Vector3.Distance(networkPosition.Value, newPosition);
        
        return distance <= maxMovementPerFrame && IsWithinMapBounds(newPosition);
    }
}
```

### Command Synchronization System
```csharp
// AI-generated RTS command synchronization
public class RTSCommandSystem : NetworkBehaviour
{
    [Header("Command Queue")]
    [SerializeField] private Queue<RTSCommand> pendingCommands = new Queue<RTSCommand>();
    [SerializeField] private int maxCommandsPerSecond = 10;
    
    private float lastCommandTime;
    private int commandsThisSecond;
    
    public enum CommandType
    {
        Move, Attack, Build, Cast, Stop, Hold
    }
    
    [System.Serializable]
    public struct RTSCommand : INetworkSerializable
    {
        public CommandType type;
        public Vector3 targetPosition;
        public ulong targetEntityId;
        public int castingPlayerId;
        public float timestamp;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref type);
            serializer.SerializeValue(ref targetPosition);
            serializer.SerializeValue(ref targetEntityId);
            serializer.SerializeValue(ref castingPlayerId);
            serializer.SerializeValue(ref timestamp);
        }
    }
    
    public void IssueCommand(CommandType type, Vector3 target, ulong targetEntity = 0)
    {
        if (!CanIssueCommand()) return;
        
        var command = new RTSCommand
        {
            type = type,
            targetPosition = target,
            targetEntityId = targetEntity,
            castingPlayerId = (int)NetworkManager.Singleton.LocalClientId,
            timestamp = (float)NetworkManager.Singleton.ServerTime.Time
        };
        
        if (IsServer)
        {
            ProcessCommand(command);
        }
        else
        {
            IssueCommandServerRpc(command);
        }
        
        TrackCommandRate();
    }
    
    [ServerRpc(RequireOwnership = false)]
    void IssueCommandServerRpc(RTSCommand command, ServerRpcParams serverRpcParams = default)
    {
        // Server-side validation
        if (ValidateCommand(command, serverRpcParams.Receive.SenderClientId))
        {
            ProcessCommand(command);
            // Broadcast to all clients for immediate feedback
            ExecuteCommandClientRpc(command);
        }
    }
    
    [ClientRpc]
    void ExecuteCommandClientRpc(RTSCommand command)
    {
        if (!IsServer) // Clients execute, server already processed
        {
            ProcessCommand(command);
        }
    }
    
    bool ValidateCommand(RTSCommand command, ulong senderId)
    {
        // Anti-cheat validation
        if (command.castingPlayerId != senderId) return false;
        if (!IsPlayerAuthorizedForCommand(senderId, command)) return false;
        if (IsCommandRateLimited(senderId)) return false;
        
        return true;
    }
    
    void ProcessCommand(RTSCommand command)
    {
        // Execute command on game state
        switch (command.type)
        {
            case CommandType.Move:
                ExecuteMoveCommand(command);
                break;
            case CommandType.Attack:
                ExecuteAttackCommand(command);
                break;
            // Additional command types...
        }
    }
    
    bool CanIssueCommand()
    {
        var currentTime = Time.time;
        if (currentTime - lastCommandTime > 1f)
        {
            commandsThisSecond = 0;
            lastCommandTime = currentTime;
        }
        
        return commandsThisSecond < maxCommandsPerSecond;
    }
    
    void TrackCommandRate()
    {
        commandsThisSecond++;
    }
}
```

## RTS-Specific Networking Solutions

### Resource Synchronization
```csharp
// AI-generated resource management networking
public class NetworkedResourceManager : NetworkBehaviour
{
    [Header("Resource State")]
    [SerializeField] private NetworkVariable<int> networkGold = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> networkLumber = new NetworkVariable<int>();
    [SerializeField] private NetworkVariable<int> networkFood = new NetworkVariable<int>();
    
    private Dictionary<ulong, PlayerResources> playerResources = new Dictionary<ulong, PlayerResources>();
    
    [System.Serializable]
    public struct PlayerResources : INetworkSerializable
    {
        public int gold;
        public int lumber;
        public int food;
        public int foodUsed;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref gold);
            serializer.SerializeValue(ref lumber);
            serializer.SerializeValue(ref food);
            serializer.SerializeValue(ref foodUsed);
        }
    }
    
    public bool TrySpendResources(ulong playerId, int gold, int lumber)
    {
        if (!IsServer) 
        {
            RequestSpendResourcesServerRpc(playerId, gold, lumber);
            return false; // Client waits for server response
        }
        
        if (!playerResources.ContainsKey(playerId)) return false;
        
        var resources = playerResources[playerId];
        if (resources.gold >= gold && resources.lumber >= lumber)
        {
            resources.gold -= gold;
            resources.lumber -= lumber;
            playerResources[playerId] = resources;
            
            // Notify clients of resource change
            UpdatePlayerResourcesClientRpc(playerId, resources);
            return true;
        }
        
        return false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    void RequestSpendResourcesServerRpc(ulong playerId, int gold, int lumber, ServerRpcParams serverRpcParams = default)
    {
        // Validate request is from correct player
        if (serverRpcParams.Receive.SenderClientId == playerId)
        {
            var success = TrySpendResources(playerId, gold, lumber);
            NotifySpendResultClientRpc(playerId, success);
        }
    }
    
    [ClientRpc]
    void NotifySpendResultClientRpc(ulong playerId, bool success)
    {
        // Client can now proceed with action or show error
        if (playerId == NetworkManager.Singleton.LocalClientId)
        {
            OnResourceSpendResult?.Invoke(success);
        }
    }
    
    [ClientRpc]
    void UpdatePlayerResourcesClientRpc(ulong playerId, PlayerResources resources)
    {
        playerResources[playerId] = resources;
        
        // Update UI if local player
        if (playerId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateResourceUI(resources);
        }
    }
    
    public event System.Action<bool> OnResourceSpendResult;
}
```

### Combat Resolution Networking
```csharp
// AI-generated networked combat system
public class NetworkedCombatSystem : NetworkBehaviour
{
    [Header("Combat Configuration")]
    [SerializeField] private float combatTickRate = 10f; // Combat updates per second
    [SerializeField] private NetworkVariable<uint> combatFrameCounter = new NetworkVariable<uint>();
    
    private Dictionary<ulong, CombatState> activeCombats = new Dictionary<ulong, CombatState>();
    
    [System.Serializable]
    public struct CombatState : INetworkSerializable
    {
        public ulong attackerId;
        public ulong defenderId;
        public float damage;
        public float timestamp;
        public bool resolved;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref attackerId);
            serializer.SerializeValue(ref defenderId);
            serializer.SerializeValue(ref damage);
            serializer.SerializeValue(ref timestamp);
            serializer.SerializeValue(ref resolved);
        }
    }
    
    void FixedUpdate()
    {
        if (IsServer)
        {
            ProcessCombatTick();
        }
    }
    
    void ProcessCombatTick()
    {
        var deltaTime = 1f / combatTickRate;
        var currentFrame = combatFrameCounter.Value + 1;
        combatFrameCounter.Value = currentFrame;
        
        // Process all active combats deterministically
        var combatsToRemove = new List<ulong>();
        
        foreach (var kvp in activeCombats)
        {
            var combatId = kvp.Key;
            var combat = kvp.Value;
            
            if (ProcessCombatResolution(ref combat))
            {
                // Combat resolved, broadcast result
                BroadcastCombatResultClientRpc(combatId, combat);
                combatsToRemove.Add(combatId);
            }
            else
            {
                activeCombats[combatId] = combat;
            }
        }
        
        // Clean up resolved combats
        foreach (var combatId in combatsToRemove)
        {
            activeCombats.Remove(combatId);
        }
    }
    
    public void InitiateCombat(ulong attackerId, ulong defenderId)
    {
        if (!IsServer)
        {
            InitiateCombatServerRpc(attackerId, defenderId);
            return;
        }
        
        var combatId = GenerateCombatId(attackerId, defenderId);
        var combat = new CombatState
        {
            attackerId = attackerId,
            defenderId = defenderId,
            timestamp = (float)NetworkManager.Singleton.ServerTime.Time,
            resolved = false
        };
        
        // Calculate damage using TFT formulas
        combat.damage = CalculateDamage(attackerId, defenderId);
        
        activeCombats[combatId] = combat;
    }
    
    [ServerRpc(RequireOwnership = false)]
    void InitiateCombatServerRpc(ulong attackerId, ulong defenderId)
    {
        // Validate combat initiation
        if (CanInitiateCombat(attackerId, defenderId))
        {
            InitiateCombat(attackerId, defenderId);
        }
    }
    
    [ClientRpc]
    void BroadcastCombatResultClientRpc(ulong combatId, CombatState result)
    {
        // Apply combat result on all clients
        ApplyCombatResult(result);
        
        // Play visual/audio effects
        PlayCombatEffects(result);
    }
    
    float CalculateDamage(ulong attackerId, ulong defenderId)
    {
        // Server-authoritative damage calculation using exact TFT formulas
        var attacker = GetEntityStats(attackerId);
        var defender = GetEntityStats(defenderId);
        
        // Apply TFT damage formula
        var baseDamage = UnityEngine.Random.Range(attacker.minDamage, attacker.maxDamage);
        var typeMultiplier = GetTypeEffectiveness(attacker.damageType, defender.armorType);
        var reduction = CalculateArmorReduction(defender.armor);
        
        return baseDamage * typeMultiplier * (1f - reduction);
    }
    
    bool CanInitiateCombat(ulong attackerId, ulong defenderId)
    {
        // Validate combat rules (range, line of sight, etc.)
        return IsValidTarget(attackerId, defenderId) && 
               IsInRange(attackerId, defenderId) && 
               HasLineOfSight(attackerId, defenderId);
    }
}
```

## AI Bot Implementation

### Multiplayer AI Design
```
Design AI bot system for multiplayer RTS matches:

AI Difficulty Levels:
- Easy: Basic build orders, passive strategy
- Normal: Competent economy, simple tactics
- Hard: Advanced strategies, micro management
- Expert: Near-optimal play, adaptive tactics

AI Capabilities Required:
- Economic management (resource optimization)
- Military tactics (unit composition, positioning)
- Strategic decision making (expansion, tech choices)
- Micro management (individual unit control)
- Adaptation (respond to player strategies)

Network Integration:
- AI runs on server for authority
- Commands issued through same system as players
- Deterministic decision making for consistency
- Bandwidth optimization for AI actions
- Spectator-friendly AI behavior

Performance Constraints:
- AI thinking time: <100ms per decision
- Memory usage: <50MB per AI player
- CPU usage: <10% per AI player
- Network traffic: Similar to human player

Personality Traits:
- Aggressive: Early military pressure
- Economic: Focus on expansion and tech
- Turtle: Defensive, late-game focused
- Adaptive: Changes strategy based on opponents
```

### AI Networking Implementation
```csharp
// AI-generated multiplayer AI bot system
public class MultiplayerAIBot : NetworkBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] private AIPersonality personality;
    [SerializeField] private AIDifficulty difficulty;
    [SerializeField] private float decisionInterval = 1f;
    
    private RTSCommandSystem commandSystem;
    private AIBehaviorTree behaviorTree;
    private float lastDecisionTime;
    
    public enum AIPersonality
    {
        Aggressive, Economic, Defensive, Adaptive
    }
    
    public enum AIDifficulty
    {
        Easy, Normal, Hard, Expert
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InitializeAI();
            InvokeRepeating(nameof(AIThinkingTick), 1f, decisionInterval);
        }
    }
    
    void InitializeAI()
    {
        commandSystem = FindObjectOfType<RTSCommandSystem>();
        behaviorTree = CreateBehaviorTree();
        
        // Configure AI based on difficulty and personality
        ConfigureAIParameters();
    }
    
    void AIThinkingTick()
    {
        if (!IsServer) return;
        
        var startTime = Time.realtimeSinceStartup;
        
        // AI decision making
        var gameState = AnalyzeGameState();
        var decisions = behaviorTree.Evaluate(gameState);
        
        // Execute decisions through command system
        foreach (var decision in decisions)
        {
            ExecuteAIDecision(decision);
        }
        
        var thinkingTime = Time.realtimeSinceStartup - startTime;
        
        // Performance monitoring
        if (thinkingTime > 0.1f) // 100ms threshold
        {
            Debug.LogWarning($"AI thinking time exceeded: {thinkingTime:F3}s");
        }
    }
    
    AIGameState AnalyzeGameState()
    {
        return new AIGameState
        {
            myResources = GetMyResources(),
            myUnits = GetMyUnits(),
            enemyUnits = GetVisibleEnemyUnits(),
            mapControl = AnalyzeMapControl(),
            gamePhase = DetermineGamePhase()
        };
    }
    
    void ExecuteAIDecision(AIDecision decision)
    {
        switch (decision.type)
        {
            case AIDecisionType.BuildUnit:
                commandSystem.IssueCommand(RTSCommandSystem.CommandType.Build, 
                                         decision.targetPosition, decision.targetEntity);
                break;
                
            case AIDecisionType.MoveUnits:
                foreach (var unitId in decision.affectedUnits)
                {
                    commandSystem.IssueCommand(RTSCommandSystem.CommandType.Move,
                                             decision.targetPosition, unitId);
                }
                break;
                
            case AIDecisionType.AttackTarget:
                commandSystem.IssueCommand(RTSCommandSystem.CommandType.Attack,
                                         decision.targetPosition, decision.targetEntity);
                break;
        }
    }
    
    void ConfigureAIParameters()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                decisionInterval = 2f;
                behaviorTree.SetParameter("aggressiveness", 0.3f);
                behaviorTree.SetParameter("economyFocus", 0.5f);
                break;
                
            case AIDifficulty.Expert:
                decisionInterval = 0.5f;
                behaviorTree.SetParameter("aggressiveness", 0.8f);
                behaviorTree.SetParameter("economyFocus", 0.9f);
                break;
        }
        
        switch (personality)
        {
            case AIPersonality.Aggressive:
                behaviorTree.SetParameter("militaryPriority", 0.9f);
                behaviorTree.SetParameter("expansionPriority", 0.3f);
                break;
                
            case AIPersonality.Economic:
                behaviorTree.SetParameter("militaryPriority", 0.4f);
                behaviorTree.SetParameter("expansionPriority", 0.9f);
                break;
        }
    }
}
```

## Matchmaking and Lobby System

### AI Lobby Management
```
Design matchmaking and lobby system for RTS multiplayer:

Lobby Features:
- Player slots (2-8 players)
- AI bot configuration per slot
- Map selection with preview
- Game settings (difficulty, speed, victory conditions)
- Chat system for coordination
- Ready state management

Matchmaking Categories:
- Quick Match: Automatic skill-based matching
- Custom Games: Player-created lobbies
- Ranked Matches: Competitive rating system
- Co-op Campaign: Story mode with friends

Technical Requirements:
- Unity Relay for connection facilitation
- Unity Lobby service for matchmaking
- Player progression and statistics
- Cross-platform compatibility
- Regional server selection

AI Integration:
- Fill empty slots with configurable AI
- AI difficulty based on player skill levels
- Balanced team composition suggestions
- Practice mode against AI opponents
```

### Lobby System Implementation
```csharp
// AI-generated lobby management system
public class MultiplayerLobbyManager : NetworkBehaviour
{
    [Header("Lobby Configuration")]
    [SerializeField] private int maxPlayers = 8;
    [SerializeField] private string lobbyName = "FrostRealm Match";
    
    private Dictionary<ulong, LobbyPlayerData> lobbyPlayers = new Dictionary<ulong, LobbyPlayerData>();
    private NetworkVariable<LobbyState> currentLobbyState = new NetworkVariable<LobbyState>();
    
    [System.Serializable]
    public struct LobbyPlayerData : INetworkSerializable
    {
        public string playerName;
        public Faction selectedFaction;
        public bool isReady;
        public bool isAI;
        public AIDifficulty aiDifficulty;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref selectedFaction);
            serializer.SerializeValue(ref isReady);
            serializer.SerializeValue(ref isAI);
            serializer.SerializeValue(ref aiDifficulty);
        }
    }
    
    public enum LobbyState
    {
        WaitingForPlayers, AllReady, Starting, InGame
    }
    
    public void JoinLobby(string playerName, Faction faction)
    {
        if (IsServer)
        {
            AddPlayerToLobby(NetworkManager.Singleton.LocalClientId, playerName, faction);
        }
        else
        {
            RequestJoinLobbyServerRpc(playerName, faction);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void RequestJoinLobbyServerRpc(string playerName, Faction faction, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        AddPlayerToLobby(clientId, playerName, faction);
    }
    
    void AddPlayerToLobby(ulong clientId, string playerName, Faction faction)
    {
        if (lobbyPlayers.Count >= maxPlayers) return;
        
        var playerData = new LobbyPlayerData
        {
            playerName = playerName,
            selectedFaction = faction,
            isReady = false,
            isAI = false
        };
        
        lobbyPlayers[clientId] = playerData;
        UpdateLobbyStateClientRpc(lobbyPlayers.ToArray());
        
        CheckLobbyReadyState();
    }
    
    public void AddAIPlayer(AIDifficulty difficulty, Faction faction)
    {
        if (!IsServer) return;
        
        var aiId = GenerateAIClientId();
        var aiData = new LobbyPlayerData
        {
            playerName = $"AI ({difficulty})",
            selectedFaction = faction,
            isReady = true,
            isAI = true,
            aiDifficulty = difficulty
        };
        
        lobbyPlayers[aiId] = aiData;
        UpdateLobbyStateClientRpc(lobbyPlayers.ToArray());
    }
    
    public void SetPlayerReady(bool ready)
    {
        if (IsServer)
        {
            UpdatePlayerReadyState(NetworkManager.Singleton.LocalClientId, ready);
        }
        else
        {
            SetPlayerReadyServerRpc(ready);
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void SetPlayerReadyServerRpc(bool ready, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        UpdatePlayerReadyState(clientId, ready);
    }
    
    void UpdatePlayerReadyState(ulong clientId, bool ready)
    {
        if (!lobbyPlayers.ContainsKey(clientId)) return;
        
        var playerData = lobbyPlayers[clientId];
        playerData.isReady = ready;
        lobbyPlayers[clientId] = playerData;
        
        UpdateLobbyStateClientRpc(lobbyPlayers.ToArray());
        CheckLobbyReadyState();
    }
    
    void CheckLobbyReadyState()
    {
        var allReady = lobbyPlayers.Count >= 2 && lobbyPlayers.Values.All(p => p.isReady);
        
        if (allReady && currentLobbyState.Value == LobbyState.WaitingForPlayers)
        {
            currentLobbyState.Value = LobbyState.AllReady;
            StartCoroutine(StartGameCountdown());
        }
    }
    
    IEnumerator StartGameCountdown()
    {
        currentLobbyState.Value = LobbyState.Starting;
        ShowCountdownClientRpc(5f);
        
        yield return new WaitForSeconds(5f);
        
        currentLobbyState.Value = LobbyState.InGame;
        StartGame();
    }
    
    [ClientRpc]
    void ShowCountdownClientRpc(float duration)
    {
        // Show countdown UI to all players
        StartCoroutine(CountdownUI(duration));
    }
    
    [ClientRpc]
    void UpdateLobbyStateClientRpc(LobbyPlayerData[] players)
    {
        // Update lobby UI with current player list
        RefreshLobbyUI(players);
    }
    
    void StartGame()
    {
        // Initialize game with lobby configuration
        var gameSetup = new GameSetupData
        {
            players = lobbyPlayers,
            mapName = GetSelectedMap(),
            gameSettings = GetGameSettings()
        };
        
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
```

## Anti-Cheat and Security

### Server Validation System
```csharp
// AI-generated anti-cheat validation
public class AntiCheatValidator : NetworkBehaviour
{
    [Header("Validation Thresholds")]
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private int maxCommandsPerSecond = 15;
    [SerializeField] private float maxResourceIncreaseRate = 100f; // Per minute
    
    private Dictionary<ulong, PlayerValidationData> playerData = new Dictionary<ulong, PlayerValidationData>();
    
    [System.Serializable]
    public class PlayerValidationData
    {
        public Vector3 lastPosition;
        public float lastPositionTime;
        public Queue<float> recentCommands = new Queue<float>();
        public PlayerResources lastResources;
        public float lastResourceCheck;
        public int suspiciousActivityCount;
    }
    
    public bool ValidateMovement(ulong playerId, Vector3 oldPos, Vector3 newPos, float deltaTime)
    {
        if (!IsServer) return true;
        
        var distance = Vector3.Distance(oldPos, newPos);
        var speed = distance / deltaTime;
        
        if (speed > maxMovementSpeed * 1.5f) // Allow some buffer for network lag
        {
            ReportSuspiciousActivity(playerId, $"Excessive movement speed: {speed}");
            return false;
        }
        
        return true;
    }
    
    public bool ValidateResourceChange(ulong playerId, PlayerResources oldResources, PlayerResources newResources)
    {
        if (!IsServer) return true;
        
        var timeDiff = Time.time - GetPlayerData(playerId).lastResourceCheck;
        var goldIncrease = newResources.gold - oldResources.gold;
        
        if (goldIncrease > 0)
        {
            var goldRate = goldIncrease / (timeDiff / 60f); // Per minute
            
            if (goldRate > maxResourceIncreaseRate)
            {
                ReportSuspiciousActivity(playerId, $"Excessive resource generation: {goldRate}/min");
                return false;
            }
        }
        
        GetPlayerData(playerId).lastResources = newResources;
        GetPlayerData(playerId).lastResourceCheck = Time.time;
        
        return true;
    }
    
    public bool ValidateCommandRate(ulong playerId)
    {
        if (!IsServer) return true;
        
        var playerData = GetPlayerData(playerId);
        var currentTime = Time.time;
        
        // Clean old commands (older than 1 second)
        while (playerData.recentCommands.Count > 0 && 
               currentTime - playerData.recentCommands.Peek() > 1f)
        {
            playerData.recentCommands.Dequeue();
        }
        
        // Add current command
        playerData.recentCommands.Enqueue(currentTime);
        
        if (playerData.recentCommands.Count > maxCommandsPerSecond)
        {
            ReportSuspiciousActivity(playerId, $"Command rate exceeded: {playerData.recentCommands.Count}/s");
            return false;
        }
        
        return true;
    }
    
    void ReportSuspiciousActivity(ulong playerId, string reason)
    {
        var playerData = GetPlayerData(playerId);
        playerData.suspiciousActivityCount++;
        
        Debug.LogWarning($"Suspicious activity from player {playerId}: {reason}");
        
        if (playerData.suspiciousActivityCount >= 3)
        {
            // Escalate to kick/ban
            KickPlayerForCheating(playerId);
        }
    }
    
    void KickPlayerForCheating(ulong playerId)
    {
        Debug.LogError($"Kicking player {playerId} for suspected cheating");
        NetworkManager.Singleton.DisconnectClient(playerId);
    }
    
    PlayerValidationData GetPlayerData(ulong playerId)
    {
        if (!playerData.ContainsKey(playerId))
        {
            playerData[playerId] = new PlayerValidationData();
        }
        return playerData[playerId];
    }
}
```

## Performance Optimization

### Bandwidth Optimization
```csharp
// AI-generated network optimization for RTS
public class NetworkOptimizer : NetworkBehaviour
{
    [Header("Optimization Settings")]
    [SerializeField] private float positionUpdateRate = 20f;
    [SerializeField] private float unimportantUpdateRate = 5f;
    [SerializeField] private float maxUpdateDistance = 50f;
    
    private Dictionary<ulong, float> lastUpdateTimes = new Dictionary<ulong, float>();
    
    public bool ShouldSendUpdate(ulong entityId, Vector3 position, Vector3 lastSentPosition)
    {
        // Distance-based updates
        var distanceToCamera = Vector3.Distance(position, Camera.main.transform.position);
        var positionDelta = Vector3.Distance(position, lastSentPosition);
        
        // Reduce update rate for distant objects
        var updateRate = distanceToCamera > maxUpdateDistance ? unimportantUpdateRate : positionUpdateRate;
        
        var timeSinceLastUpdate = Time.time - GetLastUpdateTime(entityId);
        var minTimeBetweenUpdates = 1f / updateRate;
        
        // Send update if enough time passed AND position changed significantly
        return timeSinceLastUpdate >= minTimeBetweenUpdates && positionDelta > 0.1f;
    }
    
    public void CompressUnitData(ref UnitNetworkData data)
    {
        // Quantize position to reduce precision
        data.position = QuantizePosition(data.position, 0.1f);
        
        // Compress health to percentage (0-100)
        data.healthPercentage = (byte)Mathf.Clamp(data.health / data.maxHealth * 100f, 0, 100);
        
        // Use bit flags for boolean states
        data.stateFlags = 0;
        if (data.isMoving) data.stateFlags |= 1;
        if (data.isAttacking) data.stateFlags |= 2;
        if (data.isSelected) data.stateFlags |= 4;
    }
    
    Vector3 QuantizePosition(Vector3 position, float precision)
    {
        return new Vector3(
            Mathf.Round(position.x / precision) * precision,
            Mathf.Round(position.y / precision) * precision,
            Mathf.Round(position.z / precision) * precision
        );
    }
    
    float GetLastUpdateTime(ulong entityId)
    {
        return lastUpdateTimes.ContainsKey(entityId) ? lastUpdateTimes[entityId] : 0f;
    }
    
    public void RecordUpdateTime(ulong entityId)
    {
        lastUpdateTimes[entityId] = Time.time;
    }
}
```

This comprehensive multiplayer guide ensures robust, scalable, and secure networking for your RTS game while leveraging AI assistance for rapid development and optimization.