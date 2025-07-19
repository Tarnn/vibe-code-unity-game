#!/bin/bash

echo "================================"
echo "FrostRealm Chronicles Quick Run"
echo "================================"
echo

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Unity paths (adjust these based on your installation)
UNITY_HUB_PATH="/opt/Unity/Unity Hub/UnityHub.AppImage"
UNITY_EDITOR_PATH="/opt/Unity/Editor/Unity"

# Detect WSL and attempt to use Windows Unity installation
if grep -qi "microsoft" /proc/sys/kernel/osrelease 2>/dev/null; then
    WINDOWS_UNITY_PATH="/mnt/c/Program Files/Unity/Hub/Editor/6000.0.23f1/Editor/Unity.exe"
    if [ -f "$WINDOWS_UNITY_PATH" ]; then
        UNITY_EDITOR_PATH="$WINDOWS_UNITY_PATH"
    fi
fi

PROJECT_PATH=$(pwd)

# Check for Unity installation
if [ ! -f "$UNITY_EDITOR_PATH" ]; then
    print_warning "Unity Editor not found at: $UNITY_EDITOR_PATH"
    
    # Try common Unity installation paths
    COMMON_PATHS=(
        "/opt/unity/Editor/Unity"
        "/usr/bin/unity-editor"
        "$HOME/Unity/Hub/Editor/2023.3.23f1/Editor/Unity"
        "/Applications/Unity/Hub/Editor/2023.3.23f1/Unity.app/Contents/MacOS/Unity"
    )
    
    for path in "${COMMON_PATHS[@]}"; do
        if [ -f "$path" ]; then
            UNITY_EDITOR_PATH="$path"
            print_success "Found Unity at: $path"
            break
        fi
    done
    
    if [ ! -f "$UNITY_EDITOR_PATH" ]; then
        print_error "Unity Editor not found!"
        echo
        echo "Please install Unity 6000.0.23f1 and update UNITY_EDITOR_PATH in this script."
        echo
        echo "Installation options:"
        echo "1. Download Unity Hub from: https://unity3d.com/get-unity/download"
        echo "2. Install Unity 6000.0.23f1 through Unity Hub"
        echo "3. Or install via package manager if available"
        echo
        echo "Common Linux installation paths:"
        echo "  - /opt/Unity/Editor/Unity"
        echo "  - /usr/bin/unity-editor"
        echo "  - ~/Unity/Hub/Editor/[VERSION]/Editor/Unity"
        exit 1
    fi
fi

# Get timestamp for logs
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")

# Create logs directory
mkdir -p "Logs"

print_info "Starting Unity build process..."
print_info "Unity Path: $UNITY_EDITOR_PATH"
print_info "Project Path: $PROJECT_PATH"
print_info "Log File: Logs/build_$TIMESTAMP.log"
echo

# Build the project
"$UNITY_EDITOR_PATH" \
    -batchmode \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -buildTarget StandaloneLinux64 \
    -executeMethod BuildScript.BuildGame \
    -logFile "Logs/build_$TIMESTAMP.log" \
    -development

BUILD_RESULT=$?

# Check if build was successful
if [ $BUILD_RESULT -eq 0 ]; then
    echo
    print_success "Build completed successfully!"
    
    # Check if executable exists
    if [ -f "Build/FrostRealmChronicles" ]; then
        print_info "Starting game..."
        chmod +x "Build/FrostRealmChronicles"
        
        # Launch the game
        "./Build/FrostRealmChronicles" &
        
        print_success "Game launched!"
        echo
        echo "Controls:"
        echo "- WASD or Arrow Keys: Navigate heroes"
        echo "- Enter or Space: Select hero and start game"
        echo "- Escape: Go back"
        echo "- Mouse: Click to select"
        
    elif [ -f "Build/FrostRealmChronicles.x86_64" ]; then
        print_info "Starting game..."
        chmod +x "Build/FrostRealmChronicles.x86_64"
        
        # Launch the game
        "./Build/FrostRealmChronicles.x86_64" &
        
        print_success "Game launched!"
        
    else
        print_warning "Build succeeded but executable not found."
        echo "Check Build directory for output files:"
        ls -la Build/ 2>/dev/null || echo "Build directory not found"
    fi
else
    echo
    print_error "Build failed! Error code: $BUILD_RESULT"
    print_info "Check the log file for details: Logs/build_$TIMESTAMP.log"
    
    if [ -f "Logs/build_$TIMESTAMP.log" ]; then
        echo
        print_info "Last few lines of build log:"
        echo "----------------------------------------"
        tail -10 "Logs/build_$TIMESTAMP.log"
        echo "----------------------------------------"
    fi
fi

echo
echo "================================"
echo "Build process complete"
echo "================================"