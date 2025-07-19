@echo off
setlocal enabledelayedexpansion

echo ================================
echo FrostRealm Chronicles Test Runner
echo ================================
echo.

:: Set Unity paths
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2023.3.23f1\Editor\Unity.exe"
set PROJECT_PATH=%cd%

:: Check if Unity executable exists
if not exist %UNITY_PATH% (
    echo [ERROR] Unity not found at: %UNITY_PATH%
    echo Please update the UNITY_PATH in this script.
    pause
    exit /b 1
)

:: Get timestamp for logs
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "timestamp=%dt:~0,4%-%dt:~4,2%-%dt:~6,2%_%dt:~8,2%-%dt:~10,2%-%dt:~12,2%"

:: Create logs directory
if not exist "Logs" mkdir "Logs"
if not exist "TestResults" mkdir "TestResults"

echo [INFO] Running Unity tests...
echo [INFO] Test results will be saved to: TestResults\
echo [INFO] Log file: Logs\test_%timestamp%.log
echo.

:: Run Edit Mode tests
echo [INFO] Running Edit Mode tests...
%UNITY_PATH% ^
    -batchmode ^
    -quit ^
    -projectPath "%PROJECT_PATH%" ^
    -runTests ^
    -testPlatform EditMode ^
    -testResults "TestResults\EditMode_%timestamp%.xml" ^
    -logFile "Logs\test_editmode_%timestamp%.log"

set EDITMODE_RESULT=%errorlevel%

:: Run Play Mode tests
echo [INFO] Running Play Mode tests...
%UNITY_PATH% ^
    -batchmode ^
    -quit ^
    -projectPath "%PROJECT_PATH%" ^
    -runTests ^
    -testPlatform PlayMode ^
    -testResults "TestResults\PlayMode_%timestamp%.xml" ^
    -logFile "Logs\test_playmode_%timestamp%.log"

set PLAYMODE_RESULT=%errorlevel%

:: Display results
echo.
echo ================================
echo Test Results Summary
echo ================================

if %EDITMODE_RESULT% equ 0 (
    echo [SUCCESS] Edit Mode tests: PASSED
) else (
    echo [FAILED] Edit Mode tests: FAILED ^(Error %EDITMODE_RESULT%^)
)

if %PLAYMODE_RESULT% equ 0 (
    echo [SUCCESS] Play Mode tests: PASSED
) else (
    echo [FAILED] Play Mode tests: FAILED ^(Error %PLAYMODE_RESULT%^)
)

:: Check for test result files
if exist "TestResults\EditMode_%timestamp%.xml" (
    echo [INFO] Edit Mode results: TestResults\EditMode_%timestamp%.xml
)

if exist "TestResults\PlayMode_%timestamp%.xml" (
    echo [INFO] Play Mode results: TestResults\PlayMode_%timestamp%.xml
)

:: Overall result
set /a TOTAL_RESULT=%EDITMODE_RESULT%+%PLAYMODE_RESULT%
if %TOTAL_RESULT% equ 0 (
    echo.
    echo [SUCCESS] All tests passed!
    exit /b 0
) else (
    echo.
    echo [FAILED] Some tests failed. Check log files for details.
    exit /b 1
)

pause