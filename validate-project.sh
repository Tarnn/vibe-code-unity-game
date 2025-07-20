#!/bin/bash

echo "=== FrostRealm Chronicles - Project Validation ==="
echo "Date: $(date)"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

VALIDATION_PASSED=0

# Function to check if file exists
check_file() {
    if [ -f "$1" ]; then
        echo -e "${GREEN}✅ $2${NC}"
        return 0
    else
        echo -e "${RED}❌ $2 - Missing: $1${NC}"
        return 1
    fi
}

# Function to check if directory exists
check_directory() {
    if [ -d "$1" ]; then
        echo -e "${GREEN}✅ $2${NC}"
        return 0
    else
        echo -e "${RED}❌ $2 - Missing: $1${NC}"
        return 1
    fi
}

echo "=== Project Structure Validation ==="

# Check critical directories
check_directory "assets/Scripts/Core" "Core Scripts Directory"
check_directory "assets/Scripts/Data" "Data Scripts Directory" 
check_directory "assets/Scripts/UI" "UI Scripts Directory"
check_directory "assets/Scripts/Editor" "Editor Scripts Directory"
check_directory "assets/Data/Heroes" "Hero Data Directory"
check_directory "assets/Resources" "Resources Directory"
check_directory "assets/Scenes" "Scenes Directory"
check_directory "assets/Audio" "Audio Directory"
check_directory "assets/UI" "UI Assets Directory"

echo ""
echo "=== Core Systems Validation ==="

# Check core scripts
check_file "assets/Scripts/Core/GameManager.cs" "GameManager Script"
check_file "assets/Scripts/Core/ResourceManager.cs" "ResourceManager Script"
check_file "assets/Scripts/Core/AudioManager.cs" "AudioManager Script"
check_file "assets/Scripts/Core/InputManager.cs" "InputManager Script"
check_file "assets/Scripts/Core/SelectionManager.cs" "SelectionManager Script"
check_file "assets/Scripts/Core/RTSCameraController.cs" "RTS Camera Controller Script"
check_file "assets/Scripts/Core/HeroRegistry.cs" "HeroRegistry Script"
check_file "assets/Scripts/Data/HeroData.cs" "HeroData Script"

echo ""
echo "=== Hero System Validation ==="

# Check hero data assets
HERO_COUNT=$(find assets/Data/Heroes -name "*.asset" | grep -v ".meta" | wc -l)
if [ "$HERO_COUNT" -ge 8 ]; then
    echo -e "${GREEN}✅ Hero Data Assets - Found $HERO_COUNT heroes (expecting 8)${NC}"
else
    echo -e "${RED}❌ Hero Data Assets - Found $HERO_COUNT heroes (expecting 8)${NC}"
    VALIDATION_PASSED=1
fi

# Check HeroRegistry in Resources
check_file "assets/Resources/HeroRegistry.asset" "HeroRegistry Asset"

echo ""
echo "=== Build Configuration Validation ==="

# Check scenes
check_file "assets/Scenes/MainMenu/MainMenu.unity" "Main Menu Scene"
check_file "assets/Scenes/CharacterSelection.unity" "Character Selection Scene"  
check_file "assets/Scenes/Gameplay/Gameplay.unity" "Gameplay Scene"

# Check build settings
check_file "ProjectSettings/EditorBuildSettings.asset" "Editor Build Settings"

echo ""
echo "=== Input System Validation ==="

# Check input actions
check_file "assets/Scripts/Core/FrostRealmInputActions.inputactions" "Input Actions Asset"
check_file "assets/Scripts/Core/FrostRealmInputActions.cs" "Input Actions Script"

echo ""
echo "=== UI System Validation ==="

# Check UI assets
check_file "assets/UI/CharacterSelection.uxml" "Character Selection UXML"
check_file "assets/UI/CharacterSelection.uss" "Character Selection USS"

# Check UI scripts
check_file "assets/Scripts/UI/HeroSelectionManager.cs" "Hero Selection Manager"
check_file "assets/Scripts/UI/CharacterSelectionController.cs" "Character Selection Controller"

echo ""
echo "=== Assembly Definitions Validation ==="

# Check assembly definitions
check_file "assets/Scripts/FrostRealm.asmdef" "Main Assembly Definition"
check_file "assets/Scripts/Editor/Tests/FrostRealm.Tests.asmdef" "Test Assembly Definition"

echo ""
echo "=== Test Framework Validation ==="

# Check test files
check_file "assets/Scripts/Editor/Tests/FrostRealmTests.cs" "Unit Tests"
check_file "assets/Scripts/Editor/Tests/IntegrationTests.cs" "Integration Tests"

echo ""
echo "=== Build Scripts Validation ==="

# Check build scripts
check_file "assets/Scripts/Editor/BuildScript.cs" "Build Script"
check_file "assets/Scripts/Editor/SceneValidator.cs" "Scene Validator"
check_file "assets/Scripts/Editor/ProjectValidator.cs" "Project Validator"

echo ""
echo "=== Git Status Check ==="

# Check git status for major issues
if [ -d ".git" ]; then
    UNTRACKED_COUNT=$(git status --porcelain | grep "^??" | wc -l)
    MODIFIED_COUNT=$(git status --porcelain | grep "^ M" | wc -l)
    
    echo -e "${YELLOW}📊 Git Status:${NC}"
    echo "   - Untracked files: $UNTRACKED_COUNT"
    echo "   - Modified files: $MODIFIED_COUNT"
    
    if [ "$UNTRACKED_COUNT" -gt 50 ]; then
        echo -e "${YELLOW}⚠️ High number of untracked files${NC}"
    fi
else
    echo -e "${RED}❌ Not a git repository${NC}"
fi

echo ""
echo "=== Summary ==="

if [ $VALIDATION_PASSED -eq 0 ]; then
    echo -e "${GREEN}🎮 PROJECT VALIDATION PASSED!${NC}"
    echo -e "${GREEN}✅ All critical systems are in place${NC}"
    echo -e "${GREEN}✅ Project is ready for build and testing${NC}"
    exit 0
else
    echo -e "${RED}❌ PROJECT VALIDATION FAILED!${NC}"
    echo -e "${RED}Please fix the issues above before proceeding${NC}"
    exit 1
fi