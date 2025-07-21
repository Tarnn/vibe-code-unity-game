#!/bin/bash

# FrostRealm Chronicles - One-Click Build and Play
# This script builds the game and runs it automatically
# No Unity UI configuration required!

echo "🎮 FrostRealm Chronicles - One-Click Build & Play"
echo "=================================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

print_step() {
    echo -e "${CYAN}[STEP]${NC} $1"
}

print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Auto-detect Unity installation
print_step "Detecting Unity installation..."

UNITY_PATH=""
PROJECT_PATH=$(pwd)

# Try different Unity installation paths
UNITY_SEARCH_PATHS=(
    "/mnt/c/Program Files/Unity/Hub/Editor"
    "/mnt/c/Program Files/Unity Hub/Editor"
    "/Applications/Unity/Hub/Editor"
    "/opt/Unity/Editor"
    "/snap/unity/current/Editor"
)

# First try to find Unity Hub installations
for base_path in "${UNITY_SEARCH_PATHS[@]}"; do
    if [ -d "$base_path" ]; then
        # Find the most recent Unity version
        unity_version_dir=$(find "$base_path" -maxdepth 1 -type d -name "*.*.*f*" | sort -V | tail -1)
        if [ -n "$unity_version_dir" ]; then
            if [ -f "$unity_version_dir/Editor/Unity.exe" ]; then
                UNITY_PATH="$unity_version_dir/Editor/Unity.exe"
                break
            elif [ -f "$unity_version_dir/Unity.app/Contents/MacOS/Unity" ]; then
                UNITY_PATH="$unity_version_dir/Unity.app/Contents/MacOS/Unity"
                break
            elif [ -f "$unity_version_dir/Editor/Unity" ]; then
                UNITY_PATH="$unity_version_dir/Editor/Unity"
                break
            fi
        fi
    fi
done

# Fallback: try direct paths
if [ -z "$UNITY_PATH" ]; then
    FALLBACK_PATHS=(
        "/usr/bin/unity-editor"
        "/opt/Unity/Editor/Unity"
    )
    
    for path in "${FALLBACK_PATHS[@]}"; do
        if [ -f "$path" ]; then
            UNITY_PATH="$path"
            break
        fi
    done
fi

if [ -z "$UNITY_PATH" ]; then
    print_error "Unity Editor not found!"
    echo ""
    echo "Please install Unity 2022.3 LTS or later from:"
    echo "https://unity.com/download"
    echo ""
    echo "Or manually set UNITY_PATH in this script"
    exit 1
fi

print_success "Found Unity: $UNITY_PATH"
echo ""

# Handle Windows paths in WSL
if [[ "$UNITY_PATH" == *".exe" ]]; then
    PROJECT_PATH_WIN=$(wslpath -w "$PROJECT_PATH" 2>/dev/null || echo "$PROJECT_PATH")
    BUILD_TARGET="StandaloneWindows64"
    EXECUTABLE_NAME="FrostRealmChronicles.exe"
else
    PROJECT_PATH_WIN="$PROJECT_PATH"
    BUILD_TARGET="StandaloneLinux64"
    EXECUTABLE_NAME="FrostRealmChronicles.x86_64"
fi

print_info "Project path: $PROJECT_PATH"
print_info "Build target: $BUILD_TARGET"
echo ""

# Clean previous builds
print_step "Cleaning previous builds..."
rm -rf Build/
rm -f build*.log
mkdir -p Build

# Get timestamp for logs
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
LOG_FILE="build_$TIMESTAMP.log"

print_step "Building FrostRealm Chronicles..."
print_info "This may take a few minutes..."
echo ""

# Build the game
"$UNITY_PATH" \
    -batchmode \
    -quit \
    -projectPath "$PROJECT_PATH_WIN" \
    -buildTarget "$BUILD_TARGET" \
    -executeMethod FrostRealm.Editor.BuildScript.BuildGame \
    -logFile "$LOG_FILE" \
    -nographics

BUILD_RESULT=$?
echo ""

if [ $BUILD_RESULT -eq 0 ]; then
    print_success "Build completed successfully!"
    
    # Find the executable
    EXECUTABLE_PATH=""
    if [ -f "Build/$EXECUTABLE_NAME" ]; then
        EXECUTABLE_PATH="Build/$EXECUTABLE_NAME"
    elif [ -f "Build/FrostRealmChronicles.exe" ]; then
        EXECUTABLE_PATH="Build/FrostRealmChronicles.exe"
    elif [ -f "Build/FrostRealmChronicles" ]; then
        EXECUTABLE_PATH="Build/FrostRealmChronicles"
    else
        # Search for any executable in Build directory
        EXECUTABLE_PATH=$(find Build/ -type f -executable 2>/dev/null | head -1)
    fi
    
    if [ -n "$EXECUTABLE_PATH" ] && [ -f "$EXECUTABLE_PATH" ]; then
        # Get file size
        BUILD_SIZE=$(du -h "$EXECUTABLE_PATH" | cut -f1)
        print_info "Executable: $EXECUTABLE_PATH"
        print_info "Build size: $BUILD_SIZE"
        echo ""
        
        # Make executable if on Linux/Mac
        if [[ "$EXECUTABLE_PATH" != *".exe" ]]; then
            chmod +x "$EXECUTABLE_PATH"
        fi
        
        print_step "Starting FrostRealm Chronicles..."
        echo ""
        echo "🎮 Launching game..."
        echo "   Press CTRL+C to return to terminal"
        echo ""
        
        # Run the game
        if [[ "$EXECUTABLE_PATH" == *".exe" ]]; then
            # Windows executable in WSL
            cmd.exe /c "$(wslpath -w "$PWD/$EXECUTABLE_PATH")" 2>/dev/null &
        else
            # Linux/Mac executable
            "./$EXECUTABLE_PATH" &
        fi
        
        GAME_PID=$!
        print_success "Game launched successfully! (PID: $GAME_PID)"
        echo ""
        echo "🎮 Game Controls:"
        echo "   - WASD: Move camera"
        echo "   - Mouse: Look around"
        echo "   - ESC: Pause/Menu"
        echo "   - Alt+F4 or Cmd+Q: Quit"
        echo ""
        echo "📁 Game files are in: $(pwd)/Build/"
        echo "📄 Build log: $LOG_FILE"
        
    else
        print_warning "Build succeeded but executable not found!"
        echo ""
        echo "Contents of Build directory:"
        ls -la Build/ 2>/dev/null || echo "Build directory is empty"
    fi
    
else
    print_error "Build failed! (Exit code: $BUILD_RESULT)"
    echo ""
    print_info "Build log: $LOG_FILE"
    
    if [ -f "$LOG_FILE" ]; then
        echo ""
        echo "Last 20 lines of build log:"
        echo "=================================================="
        tail -20 "$LOG_FILE"
        echo "=================================================="
    fi
    
    echo ""
    echo "Common fixes:"
    echo "1. Make sure Unity 2022.3 LTS or later is installed"
    echo "2. Check that all required packages are installed"
    echo "3. Verify project settings are correct"
    echo ""
    exit 1
fi

echo ""
echo "=================================================="
echo "🎮 FrostRealm Chronicles - Ready to Play!"
echo "=================================================="