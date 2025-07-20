#!/bin/bash

echo "================================"
echo "FrostRealm Chronicles Build Only"
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
UNITY_EDITOR_PATH="/opt/Unity/Editor/Unity"

# Detect WSL and attempt to use Windows Unity installation
UNITY_HUB_ROOTS=(
    "/mnt/c/Program Files/Unity/Hub/Editor"
    "/mnt/c/Program Files/Unity Hub/Editor"
)
if grep -qi "microsoft" /proc/sys/kernel/osrelease 2>/dev/null; then
    for root in "${UNITY_HUB_ROOTS[@]}"; do
        if [ -d "$root" ]; then
            CANDIDATE=$(find "$root" -maxdepth 3 -type f -name Unity.exe | sort -V | tail -n 1)
            if [ -n "$CANDIDATE" ]; then
                UNITY_EDITOR_PATH="$CANDIDATE"
                break
            fi
        fi
    done
fi

PROJECT_PATH=$(pwd)

# Convert project path when running Windows Unity.exe from WSL
echo "$UNITY_EDITOR_PATH" | grep -q "\.exe$" && PROJECT_PATH_WIN=$(wslpath -w "$PROJECT_PATH") && PROJECT_PATH="$PROJECT_PATH_WIN"

# Check for Unity installation
if [ ! -f "$UNITY_EDITOR_PATH" ]; then
    print_error "Unity Editor not found at: $UNITY_EDITOR_PATH"
    echo
    echo "Please update UNITY_EDITOR_PATH in this script to match your Unity installation."
    echo
    echo "Common Unity paths:"
    echo "- Linux: /opt/Unity/Editor/Unity"
    echo "- Linux (snap): /snap/unity/current/Editor/Unity"
    echo "- macOS: /Applications/Unity/Hub/Editor/[VERSION]/Unity.app/Contents/MacOS/Unity"
    echo
    exit 1
fi

# Get timestamp for logs
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")

# Create logs directory
mkdir -p "Logs"

print_info "Building FrostRealm Chronicles..."
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
    -executeMethod FrostRealm.Editor.BuildScript.BuildGame \
    -logFile "Logs/build_$TIMESTAMP.log"

BUILD_RESULT=$?

# Check build result
if [ $BUILD_RESULT -eq 0 ]; then
    echo
    print_success "Build completed successfully!"
    
    if [ -f "Build/FrostRealmChronicles" ]; then
        print_info "Executable created: Build/FrostRealmChronicles"
        
        # Get file size
        SIZE=$(du -h "Build/FrostRealmChronicles" | cut -f1)
        print_info "Build size: $SIZE"
        
    elif [ -f "Build/FrostRealmChronicles.x86_64" ]; then
        print_info "Executable created: Build/FrostRealmChronicles.x86_64"
        
        # Get file size
        SIZE=$(du -h "Build/FrostRealmChronicles.x86_64" | cut -f1)
        print_info "Build size: $SIZE"
        
    else
        print_warning "Build succeeded but executable not found at expected location."
        echo "Contents of Build directory:"
        ls -la Build/ 2>/dev/null || echo "Build directory not found"
    fi
else
    echo
    print_error "Build failed with error code: $BUILD_RESULT"
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