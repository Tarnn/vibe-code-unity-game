@echo off
echo ================================
echo FrostRealm Chronicles Launcher
echo ================================
echo.

:: Check if game executable exists
if exist "Build\FrostRealmChronicles.exe" (
    echo [INFO] Starting FrostRealm Chronicles...
    echo [INFO] Game Path: Build\FrostRealmChronicles.exe
    echo.
    
    :: Launch the game
    start "" "Build\FrostRealmChronicles.exe"
    
    echo [SUCCESS] Game launched successfully!
    echo.
    echo Controls:
    echo - WASD or Arrow Keys: Navigate heroes
    echo - Enter or Space: Select hero and start game
    echo - Escape: Go back
    echo - Mouse: Click to select
    echo.
    echo Have fun playing!
    
) else (
    echo [ERROR] Game not found at: Build\FrostRealmChronicles.exe
    echo.
    echo To build the game, run one of these commands:
    echo   dev-run.bat      - Build and run automatically
    echo   dev-build.bat    - Build only
    echo.
    echo Or open the project in Unity and build manually.
    pause
)

echo.
echo Launcher closing in 3 seconds...
timeout /t 3 > nul