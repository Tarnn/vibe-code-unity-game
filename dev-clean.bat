@echo off
echo ================================
echo FrostRealm Chronicles Clean
echo ================================
echo.

echo [INFO] Cleaning build artifacts...

:: Clean build directories
if exist "Build" (
    echo [INFO] Removing Build directory...
    rmdir /s /q "Build"
    echo [SUCCESS] Build directory removed
) else (
    echo [INFO] Build directory doesn't exist
)

:: Clean Unity temp files
if exist "Temp" (
    echo [INFO] Removing Unity Temp directory...
    rmdir /s /q "Temp"
    echo [SUCCESS] Temp directory removed
) else (
    echo [INFO] Temp directory doesn't exist
)

:: Clean Library cache (optional - uncomment if needed)
:: echo [WARNING] This will remove the Library folder and force Unity to reimport all assets!
:: set /p choice="Remove Library folder? (y/N): "
:: if /i "%choice%"=="y" (
::     if exist "Library" (
::         echo [INFO] Removing Library directory...
::         rmdir /s /q "Library"
::         echo [SUCCESS] Library directory removed
::     )
:: )

:: Clean logs older than 7 days
if exist "Logs" (
    echo [INFO] Cleaning old log files...
    forfiles /p "Logs" /m *.log /d -7 /c "cmd /c del @path" 2>nul
    echo [INFO] Old log files cleaned
)

:: Clean user-specific files
if exist "UserSettings\EditorUserSettings.asset" (
    echo [INFO] Preserving editor user settings...
)

echo.
echo [SUCCESS] Clean complete!
echo.
echo Files preserved:
echo - Assets/
echo - ProjectSettings/
echo - Packages/
echo - UserSettings/
echo - Source files
echo.
echo Files removed:
echo - Build/ directory
echo - Temp/ directory  
echo - Old log files (7+ days)
echo.
pause