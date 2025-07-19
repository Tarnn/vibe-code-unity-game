# Unity Setup Guide for FrostRealm Chronicles

This guide will help you install Unity and set up the development environment for FrostRealm Chronicles.

## Step 1: Install Unity Hub

### Windows
1. Download Unity Hub from: https://unity3d.com/get-unity/download
2. Run the installer and follow the setup wizard
3. Launch Unity Hub after installation

### Linux (Ubuntu/Debian)
```bash
# Download Unity Hub AppImage
wget https://public-cdn.cloud.unity3d.com/hub/prod/UnityHub.AppImage

# Make it executable
chmod +x UnityHub.AppImage

# Run Unity Hub
./UnityHub.AppImage
```

### Linux (Alternative - Snap)
```bash
sudo snap install unity --classic
```

### macOS
1. Download Unity Hub from: https://unity3d.com/get-unity/download
2. Open the downloaded .dmg file
3. Drag Unity Hub to Applications folder
4. Launch Unity Hub from Applications

## Step 2: Install Unity 6000.0.23f1

1. **Open Unity Hub**
2. **Go to "Installs" tab**
3. **Click "Install Editor"**
4. **Select Unity 6000.0.23f1** (Unity 6.1 LTS)
5. **Add these modules:**
   - Linux Build Support (IL2CPP)
   - Windows Build Support (IL2CPP) 
   - WebGL Build Support
   - Documentation

## Step 3: Configure Unity for FrostRealm Chronicles

### Open the Project
1. In Unity Hub, click "Open"
2. Navigate to your `video-game` directory
3. Select the project folder
4. Unity will import the project (this may take several minutes)

### Verify Setup
1. **Check Project Settings:**
   - File → Project Settings
   - Player → Company Name: "FrostRealm Studios"
   - Player → Product Name: "FrostRealm Chronicles"

2. **Check Package Manager:**
   - Window → Package Manager
   - Verify all required packages are installed

3. **Check Build Settings:**
   - File → Build Settings
   - Verify target platform is set correctly

## Step 4: Test the Development Scripts

### Windows
```cmd
# Test the setup
dev-run.bat

# Or just build
dev-build.bat

# Run tests
dev-test.bat

# Clean up
dev-clean.bat
```

### Linux/macOS
```bash
# Test the setup
./dev-run.sh

# Or just build
./dev-build.sh

# Run tests
./dev-test.sh

# Clean up
./dev-clean.sh
```

## Step 5: Configure Script Paths

If the scripts can't find Unity, update the paths:

### Windows (edit dev-run.bat)
```batch
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2023.3.23f1\Editor\Unity.exe"
```

### Linux (edit dev-run.sh)
```bash
UNITY_EDITOR_PATH="/opt/Unity/Editor/Unity"
# or
UNITY_EDITOR_PATH="/snap/unity/current/Editor/Unity"
```

### macOS (edit dev-run.sh)
```bash
UNITY_EDITOR_PATH="/Applications/Unity/Hub/Editor/2023.3.23f1/Unity.app/Contents/MacOS/Unity"
```

## Troubleshooting

### Unity Hub Won't Start
- **Linux**: Try running with `--no-sandbox` flag
- **Windows**: Run as administrator
- **macOS**: Check Security & Privacy settings

### Build Errors
1. **Check Unity Console** for specific error messages
2. **Verify all packages** are installed correctly
3. **Check script execution policy** on Windows:
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

### Performance Issues
- **Increase Unity's memory allocation**
- **Close unnecessary applications**
- **Use SSD storage** for better performance

### Missing Dependencies
```bash
# Linux: Install required libraries
sudo apt-get install build-essential

# Verify Git LFS
git lfs version
```

## Development Workflow

Once Unity is set up:

1. **Open Project** in Unity Hub
2. **Make Changes** in Unity Editor
3. **Test Quickly** with `./dev-run.sh` (or `.bat`)
4. **Run Tests** with `./dev-test.sh` (or `.bat`)
5. **Clean Build** with `./dev-clean.sh` (or `.bat`)

## Character Selection Testing

To test the character selection screen:

1. **Open CharacterSelection scene** in Unity
2. **Click Play** in Unity Editor
3. **Use controls:**
   - WASD/Arrow Keys: Navigate heroes
   - Enter/Space: Select hero
   - Escape: Go back
   - Mouse: Click to select

## Next Steps

After Unity setup is complete:

1. **Explore the codebase** in `Assets/Scripts/`
2. **Review documentation** in `.claude_docs/`
3. **Test character selection** system
4. **Try building** with the automation scripts
5. **Start developing** new features!

## Support

If you encounter issues:

1. **Check Unity Console** for error messages
2. **Review build logs** in `Logs/` directory
3. **Consult Unity documentation**: https://docs.unity3d.com/
4. **Check project documentation** in `.claude_docs/`

---

**Happy developing with FrostRealm Chronicles!** 🎮