@echo off
setlocal enabledelayedexpansion

echo ================================
echo FrostRealm Chronicles Build Only
echo ================================
echo.

:: Set Unity paths
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2023.3.23f1\Editor\Unity.exe"
set PROJECT_PATH=%cd%

:: Check if Unity executable exists
if not exist %UNITY_PATH% (
    echo [ERROR] Unity not found at: %UNITY_PATH%
    echo Please update the UNITY_PATH in this script to match your Unity installation.
    echo.
    echo Common Unity paths:
    echo - C:\Program Files\Unity\Hub\Editor\[VERSION]\Editor\Unity.exe
    echo - C:\Program Files\Unity\Editor\Unity.exe
    echo.
    pause
    exit /b 1
)

:: Get timestamp for logs
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "timestamp=%dt:~0,4%-%dt:~4,2%-%dt:~6,2%_%dt:~8,2%-%dt:~10,2%-%dt:~12,2%"

:: Create logs directory
if not exist "Logs" mkdir "Logs"

echo [INFO] Building FrostRealm Chronicles...
echo [INFO] Unity Path: %UNITY_PATH%
echo [INFO] Project Path: %PROJECT_PATH%
echo [INFO] Log File: Logs\build_%timestamp%.log
echo.

:: Build the project
%UNITY_PATH% ^
    -batchmode ^
    -quit ^
    -projectPath "%PROJECT_PATH%" ^
    -buildTarget Win64 ^
    -executeMethod BuildScript.BuildGame ^
    -logFile "Logs\build_%timestamp%.log"

:: Check build result
if %errorlevel% equ 0 (
    echo.
    echo [SUCCESS] Build completed successfully!
    
    if exist "Build\FrostRealmChronicles.exe" (
        echo [INFO] Executable created: Build\FrostRealmChronicles.exe
        
        :: Get file size
        for %%A in ("Build\FrostRealmChronicles.exe") do (
            set size=%%~zA
            set /a sizeKB=!size!/1024
            set /a sizeMB=!sizeKB!/1024
            echo [INFO] Build size: !sizeMB! MB
        )
    ) else (
        echo [WARNING] Build succeeded but executable not found at expected location.
    )
) else (
    echo.
    echo [ERROR] Build failed with error code: %errorlevel%
    echo [INFO] Check the log file for details: Logs\build_%timestamp%.log
    
    if exist "Logs\build_%timestamp%.log" (
        echo.
        echo [INFO] Last few lines of build log:
        echo ----------------------------------------
        powershell "Get-Content 'Logs\build_%timestamp%.log' | Select-Object -Last 10"
        echo ----------------------------------------
    )
)

echo.
echo ================================
echo Build process complete
echo ================================
pause