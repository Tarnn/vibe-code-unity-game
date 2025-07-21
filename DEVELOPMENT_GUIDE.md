# FrostRealm Chronicles - Development Guide

*For contributors and developers working on FrostRealm Chronicles*

## 🚀 Quick Start

### Prerequisites
- Unity 6000.1.12f1 (6.1 LTS)
- Git with Git LFS
- Visual Studio or VS Code
- Basic C# knowledge

### First Time Setup
```bash
# Clone the repository
git clone <repository-url>
cd video-game

# Setup development environment
dev-setup.bat  # Windows
./dev-setup.sh # Linux/WSL

# Build and run
dev-run.bat    # Windows
./dev-run.sh   # Linux/WSL
```

## 📁 Project Structure

### Core Scripts (`Assets/Scripts/`)
```
Core/                    # Core game systems
├── GameManager.cs      # Central game state management
├── InputManager.cs     # Input handling (keyboard, mouse, gamepad)
├── ResourceManager.cs  # Gold, lumber, food management
├── SelectionManager.cs # Unit selection and control groups
├── RTSCameraController.cs # RTS camera controls
├── AudioManager.cs     # Audio system
└── HeroRegistry.cs     # Hero data management

Data/                   # Data structures
├── HeroData.cs        # Hero ScriptableObject definition
└── [Hero Assets]      # Individual hero configurations

UI/                    # User interface
├── CharacterSelectionController.cs # Hero selection screen
├── HeroSelectionManager.cs       # Hero preview and validation
└── MainMenuController.cs         # Main menu

Editor/                # Development tools
├── BuildScript.cs     # Automated build system
├── ProjectValidator.cs # Project integrity checks
└── SceneValidator.cs  # Scene validation
```

### Assets (`Assets/`)
```
Art/Heroes/           # 3D hero models (8 available)
├── hero_one.fbx      # Hero model files
├── hero_one.png      # Hero textures
└── ...

Data/Heroes/          # Hero ScriptableObjects
├── PaladinHero.asset # Hero configurations
├── ArchmageHero.asset
└── ...

Scenes/               # Game scenes
├── MainMenu/         # Main menu scene
├── CharacterSelection.unity # Hero selection
└── Gameplay/         # Main gameplay scene
```

## 🎮 Current Features

### Working Systems
- **Character Selection**: 8 heroes with 3D preview
- **Input System**: Multi-device support
- **Resource Management**: Gold, lumber, food with upkeep
- **RTS Camera**: Zoom, pan, rotation
- **Build System**: Automated builds for Windows/Linux

### In Development
- **Unit Movement**: Pathfinding and control
- **Combat System**: Attack and damage mechanics
- **Audio Integration**: Music and sound effects

## 🛠 Development Workflow

### Making Changes
1. **Create Feature Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make Changes**
   - Follow Unity coding standards
   - Add comments for complex logic
   - Test your changes

3. **Test Your Changes**
   ```bash
   dev-test.bat    # Windows
   ./dev-test.sh   # Linux/WSL
   ```

4. **Build and Test**
   ```bash
   dev-run.bat     # Windows
   ./dev-run.sh    # Linux/WSL
   ```

5. **Commit and Push**
   ```bash
   git add .
   git commit -m "Add: brief description of changes"
   git push origin feature/your-feature-name
   ```

### Code Standards

#### C# Conventions
- Use PascalCase for public methods and properties
- Use camelCase for private fields and local variables
- Add XML documentation for public methods
- Follow Unity naming conventions

#### Example
```csharp
/// <summary>
/// Manages hero selection and validation.
/// </summary>
public class HeroSelectionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float selectionDuration = 0.3f;
    
    private HeroData currentHero;
    
    /// <summary>
    /// Selects a hero with validation.
    /// </summary>
    /// <param name="hero">The hero to select</param>
    public void SelectHero(HeroData hero)
    {
        if (hero == null) return;
        
        currentHero = hero;
        OnHeroSelected?.Invoke(hero);
    }
}
```

## 🎯 Common Development Tasks

### Adding a New Hero
1. **Create Hero Data**
   ```csharp
   // In Unity Editor: Right-click → Create → FrostRealm → Hero Data
   // Configure stats, abilities, and references
   ```

2. **Add to Registry**
   ```csharp
   // In HeroRegistry asset, add to availableHeroes array
   ```

3. **Add Model/Texture**
   ```
   // Place FBX model in Assets/Art/Heroes/
   // Place texture in Assets/Art/Heroes/
   ```

### Adding New Gameplay Features
1. **Create Script**
   ```csharp
   // Create new script in appropriate folder
   // Follow singleton pattern if needed
   ```

2. **Add to GameManager**
   ```csharp
   // Register in GameManager if it's a core system
   // Add initialization in InitializeGame()
   ```

3. **Test Integration**
   ```csharp
   // Test in CharacterSelection scene first
   // Then integrate into gameplay
   ```

### Debugging Tips
- Use `Debug.Log()` for temporary debugging
- Check Unity Console for errors
- Use Unity Profiler for performance issues
- Test on both Windows and Linux

## 🔧 Build System

### Build Scripts
- **dev-build.bat/sh**: Build only
- **dev-run.bat/sh**: Build and run
- **dev-test.bat/sh**: Run tests
- **dev-clean.bat/sh**: Clean build files

### Build Targets
- **Windows**: StandaloneWindows64
- **Linux**: StandaloneLinux64
- **Development**: With debugging enabled
- **Release**: Optimized builds

### Build Process
1. Unity batch mode execution
2. Scene compilation
3. Asset processing
4. IL2CPP compilation
5. Executable creation

## 📊 Testing

### Unit Tests
- Located in `Assets/Scripts/Editor/Tests/`
- Use Unity Test Framework
- Test core systems and calculations

### Integration Tests
- Test scene loading and transitions
- Test hero selection flow
- Test build process

### Performance Tests
- Target 60 FPS on RTX 3060
- Monitor memory usage
- Profile frame times

## 🎨 Asset Guidelines

### 3D Models
- **Format**: FBX with embedded textures
- **Polygon Count**: 1,000-5,000 triangles for units
- **Textures**: 4K resolution, PNG format
- **Optimization**: Use LOD for distance

### Audio
- **Format**: WAV, 48kHz, 16-bit
- **Music**: 128-320 kbps
- **SFX**: Compressed for size
- **Voice**: Clear, consistent quality

### UI Assets
- **Icons**: 64x64 pixels, PNG
- **Portraits**: 256x256 pixels
- **Backgrounds**: 1920x1080 minimum

## 🚨 Common Issues

### Build Failures
- **Unity Version**: Ensure 6000.1.12f1
- **Missing Packages**: Check Package Manager
- **Path Issues**: Verify Unity installation path
- **Permissions**: Run as administrator if needed

### Runtime Errors
- **Null References**: Check inspector assignments
- **Missing Prefabs**: Verify asset references
- **Scene Issues**: Validate scene setup
- **Script Errors**: Check console for details

### Performance Issues
- **High CPU**: Profile with Unity Profiler
- **Memory Leaks**: Check for unmanaged resources
- **Frame Drops**: Optimize update loops
- **Build Size**: Compress textures and audio

## 📚 Resources

### Documentation
- **README.md**: Project overview
- **UNITY_SETUP_GUIDE.md**: Environment setup
- **PROJECT_STATUS.md**: Current status
- **.claude_docs/**: Technical specifications

### Unity Resources
- [Unity Manual](https://docs.unity3d.com/)
- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/)
- [Unity Learn](https://learn.unity.com/)

### Community
- GitHub Issues for bug reports
- Discord server for discussions
- Wiki for detailed guides

## 🎯 Next Steps

### For New Contributors
1. **Setup Environment**: Follow UNITY_SETUP_GUIDE.md
2. **Explore Codebase**: Read through core scripts
3. **Run Tests**: Ensure everything works
4. **Pick an Issue**: Start with "good first issue" labels
5. **Ask Questions**: Don't hesitate to ask for help

### For Experienced Developers
1. **Review Architecture**: Understand the system design
2. **Identify Gaps**: Look for missing features
3. **Propose Improvements**: Suggest optimizations
4. **Mentor Others**: Help new contributors

---

*This guide is updated as the project evolves. Check for the latest version.* 