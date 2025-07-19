@echo off
setlocal enabledelayedexpansion

echo ================================
echo FrostRealm Chronicles Quick Run
echo ================================
echo.

:: Set Unity paths - adjust these based on your installation
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2023.3.23f1\Editor\Unity.exe"
set PROJECT_PATH=%cd%

:: Check if Unity executable exists
if not exist %UNITY_PATH% (
    echo [INFO] Unity not found at default path, searching...
    
    :: Try to find Unity through Unity Hub
    for /f "delims=" %%i in ('where unity-hub 2^>nul') do set UNITY_HUB=%%i
    
    if defined UNITY_HUB (
        echo [INFO] Found Unity Hub, attempting to launch project...
        unity-hub -- --projectPath "%PROJECT_PATH%"
        echo [INFO] Project should open in Unity Hub
        goto :end
    ) else (
        echo [ERROR] Unity not found! Please:
        echo 1. Install Unity 6000.0.23f1 through Unity Hub
        echo 2. Update UNITY_PATH in this script
        echo 3. Or use Unity Hub to open the project manually
        pause
        exit /b 1
    )
)

:: Get timestamp for logs
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "timestamp=%dt:~0,4%-%dt:~4,2%-%dt:~6,2%_%dt:~8,2%-%dt:~10,2%-%dt:~12,2%"

:: Create logs directory
if not exist "Logs" mkdir "Logs"

echo [INFO] Starting Unity build process...
echo [INFO] Logs will be saved to: Logs\build_%timestamp%.log
echo.

:: Build the project
%UNITY_PATH% ^
    -batchmode ^
    -quit ^
    -projectPath "%PROJECT_PATH%" ^
    -buildTarget Win64 ^
    -executeMethod BuildScript.BuildGame ^
    -logFile "Logs\build_%timestamp%.log" ^
    -development

:: Check if build was successful
if %errorlevel% equ 0 (
    echo [SUCCESS] Build completed successfully!
    echo.
    
    :: Check if executable exists
    if exist "Build\FrostRealmChronicles.exe" (
        echo [INFO] Starting game...
        start "" "Build\FrostRealmChronicles.exe"
        echo [INFO] Game launched!
    ) else (
        echo [WARNING] Build succeeded but executable not found.
        echo Check Build directory for output files.
    )
) else (
    echo [ERROR] Build failed! Error code: %errorlevel%
    echo [INFO] Check the log file for details: Logs\build_%timestamp%.log
    echo.
    echo Opening log file...
    if exist "Logs\build_%timestamp%.log" (
        start "" "Logs\build_%timestamp%.log"
    )
)

:end
echo.
echo ================================
echo Build process complete
echo ================================
echo.
pause