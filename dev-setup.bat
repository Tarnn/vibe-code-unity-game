@echo off
echo ================================
echo FrostRealm Chronicles Dev Setup
echo ================================
echo.

:: Check if Unity Hub is installed
where unity-hub >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] Unity Hub not found in PATH!
    echo Please install Unity Hub and add it to your system PATH.
    echo Download from: https://unity3d.com/get-unity/download
    pause
    exit /b 1
)

:: Check if Unity 6000.0.23f1 is installed
echo [INFO] Checking Unity installation...
unity-hub --version >nul 2>nul
if %errorlevel% neq 0 (
    echo [ERROR] Unity Hub CLI not working properly!
    pause
    exit /b 1
)

:: Check if Git LFS is installed
where git-lfs >nul 2>nul
if %errorlevel% neq 0 (
    echo [WARNING] Git LFS not found! Installing...
    git lfs install
    if %errorlevel% neq 0 (
        echo [ERROR] Failed to install Git LFS!
        echo Please install Git LFS manually: https://git-lfs.github.io/
        pause
        exit /b 1
    )
) else (
    echo [INFO] Git LFS found, ensuring it's initialized...
    git lfs install
)

:: Pull LFS files
echo [INFO] Pulling Git LFS files...
git lfs pull
if %errorlevel% neq 0 (
    echo [WARNING] Git LFS pull failed. This might be normal for a new repository.
)

:: Check if project is in Unity Hub
echo [INFO] Adding project to Unity Hub...
unity-hub --projectPath "%cd%" >nul 2>nul

:: Install Unity Editor if not present
echo [INFO] Checking Unity 6000.0.23f1 installation...
unity-hub --version >nul 2>nul

:: Create necessary directories if they don't exist
echo [INFO] Creating project directories...
if not exist "Assets\Art\Heroes" mkdir "Assets\Art\Heroes"
if not exist "Assets\Art\UI\Backgrounds" mkdir "Assets\Art\UI\Backgrounds"
if not exist "Assets\Audio\Music" mkdir "Assets\Audio\Music"
if not exist "Assets\Audio\SFX" mkdir "Assets\Audio\SFX"
if not exist "Assets\Audio\Voice" mkdir "Assets\Audio\Voice"
if not exist "Assets\Prefabs\Heroes" mkdir "Assets\Prefabs\Heroes"
if not exist "Assets\Prefabs\UI" mkdir "Assets\Prefabs\UI"
if not exist "Assets\Resources" mkdir "Assets\Resources"
if not exist "Build" mkdir "Build"
if not exist "Logs" mkdir "Logs"

:: Create build configuration
echo [INFO] Setting up build configuration...
if not exist "BuildSettings.json" (
    echo {
    echo   "buildTarget": "StandaloneWindows64",
    echo   "development": true,
    echo   "autoRunPlayer": true,
    echo   "buildPath": "./Build/FrostRealmChronicles.exe"
    echo } > BuildSettings.json
)

echo.
echo ================================
echo Setup Complete!
echo ================================
echo.
echo Available commands:
echo   dev-run.bat         - Quick build and run
echo   dev-build.bat       - Build only
echo   dev-clean.bat       - Clean build directory
echo   dev-test.bat        - Run tests
echo.
echo To start developing:
echo 1. Open Unity Hub
echo 2. Open this project
echo 3. Or run 'dev-run.bat' for quick testing
echo.
pause