#!/bin/bash

echo "================================"
echo "FrostRealm Chronicles Test Runner"
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
    echo "Please update UNITY_EDITOR_PATH in this script."
    exit 1
fi

# Get timestamp for logs
TIMESTAMP=$(date +"%Y-%m-%d_%H-%M-%S")

# Create directories
mkdir -p "Logs"
mkdir -p "TestResults"

print_info "Running Unity tests..."
print_info "Test results will be saved to: TestResults/"
print_info "Log files: Logs/"
echo

# Run Edit Mode tests
print_info "Running Edit Mode tests..."
"$UNITY_EDITOR_PATH" \
    -batchmode \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -runTests \
    -testPlatform EditMode \
    -testResults "TestResults/EditMode_$TIMESTAMP.xml" \
    -logFile "Logs/test_editmode_$TIMESTAMP.log"

EDITMODE_RESULT=$?

# Run Play Mode tests
print_info "Running Play Mode tests..."
"$UNITY_EDITOR_PATH" \
    -batchmode \
    -quit \
    -projectPath "$PROJECT_PATH" \
    -runTests \
    -testPlatform PlayMode \
    -testResults "TestResults/PlayMode_$TIMESTAMP.xml" \
    -logFile "Logs/test_playmode_$TIMESTAMP.log"

PLAYMODE_RESULT=$?

# Display results
echo
echo "================================"
echo "Test Results Summary"
echo "================================"

if [ $EDITMODE_RESULT -eq 0 ]; then
    print_success "Edit Mode tests: PASSED"
else
    print_error "Edit Mode tests: FAILED (Error $EDITMODE_RESULT)"
fi

if [ $PLAYMODE_RESULT -eq 0 ]; then
    print_success "Play Mode tests: PASSED"
else
    print_error "Play Mode tests: FAILED (Error $PLAYMODE_RESULT)"
fi

# Check for test result files
if [ -f "TestResults/EditMode_$TIMESTAMP.xml" ]; then
    print_info "Edit Mode results: TestResults/EditMode_$TIMESTAMP.xml"
fi

if [ -f "TestResults/PlayMode_$TIMESTAMP.xml" ]; then
    print_info "Play Mode results: TestResults/PlayMode_$TIMESTAMP.xml"
fi

# Overall result
TOTAL_RESULT=$((EDITMODE_RESULT + PLAYMODE_RESULT))
if [ $TOTAL_RESULT -eq 0 ]; then
    echo
    print_success "All tests passed!"
    exit 0
else
    echo
    print_error "Some tests failed. Check log files for details."
    exit 1
fi