#!/bin/bash

echo "================================"
echo "FrostRealm Chronicles Dev Setup"
echo "================================"
echo

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
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

# Check if Git is installed
if ! command -v git &> /dev/null; then
    print_error "Git not found! Please install Git first."
    exit 1
fi

print_success "Git found"

# Check if Git LFS is installed
if ! command -v git-lfs &> /dev/null; then
    print_warning "Git LFS not found! Attempting to install..."
    
    # Try to install Git LFS
    if command -v apt-get &> /dev/null; then
        sudo apt-get update && sudo apt-get install git-lfs
    elif command -v yum &> /dev/null; then
        sudo yum install git-lfs
    elif command -v brew &> /dev/null; then
        brew install git-lfs
    else
        print_error "Could not install Git LFS automatically. Please install it manually:"
        echo "  Ubuntu/Debian: sudo apt-get install git-lfs"
        echo "  CentOS/RHEL: sudo yum install git-lfs"
        echo "  macOS: brew install git-lfs"
        echo "  Or download from: https://git-lfs.github.io/"
        exit 1
    fi
fi

print_info "Initializing Git LFS..."
git lfs install

# Pull LFS files
print_info "Pulling Git LFS files..."
git lfs pull

# Create necessary directories
print_info "Creating project directories..."

directories=(
    "Assets/Art/Heroes"
    "Assets/Art/UI/Backgrounds"
    "Assets/Audio/Music"
    "Assets/Audio/SFX"
    "Assets/Audio/Voice"
    "Assets/Prefabs/Heroes"
    "Assets/Prefabs/UI"
    "Assets/Resources"
    "Build"
    "Logs"
    "TestResults"
)

for dir in "${directories[@]}"; do
    if [ ! -d "$dir" ]; then
        mkdir -p "$dir"
        print_info "Created directory: $dir"
    fi
done

# Create build configuration if it doesn't exist
if [ ! -f "BuildSettings.json" ]; then
    print_info "Creating build configuration..."
    cat > BuildSettings.json << EOF
{
  "buildTarget": "StandaloneLinux64",
  "development": true,
  "autoRunPlayer": true,
  "buildPath": "./Build/FrostRealmChronicles"
}
EOF
    print_success "Build configuration created"
fi

# Create Unity project marker if it doesn't exist
if [ ! -f "ProjectSettings/ProjectVersion.txt" ]; then
    print_warning "Unity project files not found. This might not be a Unity project yet."
    print_info "To set up Unity:"
    echo "  1. Install Unity Hub"
    echo "  2. Install Unity 6000.0.23f1"
    echo "  3. Open this project in Unity Hub"
fi

# Make scripts executable
chmod +x *.sh 2>/dev/null

echo
echo "================================"
echo "Setup Complete!"
echo "================================"
echo
echo "Available commands:"
echo "  ./dev-run.sh         - Quick build and run"
echo "  ./dev-build.sh       - Build only" 
echo "  ./dev-clean.sh       - Clean build directory"
echo "  ./dev-test.sh        - Run tests"
echo
echo "To start developing:"
echo "1. Install Unity Hub and Unity 6000.0.23f1"
echo "2. Open this project in Unity"
echo "3. Use the dev scripts for automation"
echo

print_success "FrostRealm Chronicles development environment ready!"