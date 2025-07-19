#!/bin/bash

echo "================================"
echo "FrostRealm Chronicles Clean"
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

print_info "Cleaning build artifacts..."

# Clean build directories
if [ -d "Build" ]; then
    print_info "Removing Build directory..."
    rm -rf "Build"
    print_success "Build directory removed"
else
    print_info "Build directory doesn't exist"
fi

# Clean Unity temp files
if [ -d "Temp" ]; then
    print_info "Removing Unity Temp directory..."
    rm -rf "Temp"
    print_success "Temp directory removed"
else
    print_info "Temp directory doesn't exist"
fi

# Clean Library cache (optional)
read -p "Remove Library folder? This will force Unity to reimport all assets (y/N): " choice
case "$choice" in 
    y|Y ) 
        if [ -d "Library" ]; then
            print_info "Removing Library directory..."
            rm -rf "Library"
            print_success "Library directory removed"
        fi
        ;;
    * ) 
        print_info "Library directory preserved"
        ;;
esac

# Clean logs older than 7 days
if [ -d "Logs" ]; then
    print_info "Cleaning old log files..."
    find "Logs" -name "*.log" -type f -mtime +7 -delete 2>/dev/null
    print_info "Old log files cleaned"
fi

# Clean test results older than 7 days
if [ -d "TestResults" ]; then
    print_info "Cleaning old test results..."
    find "TestResults" -name "*.xml" -type f -mtime +7 -delete 2>/dev/null
    print_info "Old test results cleaned"
fi

echo
print_success "Clean complete!"
echo
echo "Files preserved:"
echo "- Assets/"
echo "- ProjectSettings/"
echo "- Packages/"
echo "- UserSettings/"
echo "- Source files"
echo
echo "Files removed:"
echo "- Build/ directory"
echo "- Temp/ directory"
echo "- Old log files (7+ days)"
echo "- Old test results (7+ days)"
echo