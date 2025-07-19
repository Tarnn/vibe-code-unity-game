# FrostRealm Chronicles ― Open-Source RTS in Unity 6.1 LTS

FrostRealm Chronicles is a fully open-source real-time-strategy game inspired by *Warcraft III: The Frozen Throne* and modernized with Unity 6.1 LTS, HDRP ray-traced visuals, and ECS/DOTS performance.  
All development, art, audio, balance, multiplayer, and testing workflows are driven by AI and documented in this repository.

## Quick Start for Developers

> **Note for Windows + WSL users** – The *.sh scripts are fully supported under WSL.  The helper scripts now auto-detect the WSL environment and will invoke the Windows-installed Unity Editor located in `C:\Program Files\Unity\Hub\Editor\6000.0.23f1\Editor\Unity.exe`.  Simply run the same commands shown below from your Ubuntu prompt.

### Setup & Build
```bash
# Initial setup (installs dependencies, configures Git LFS)
dev-setup.bat

# Quick build and run
dev-run.bat

# Build only
dev-build.bat

# Run tests
dev-test.bat

# Clean build files
dev-clean.bat
```

### Requirements
- **Unity 6000.0.23f1** (Unity 6.1 LTS)
- **Unity Hub** (latest version)
- **Git with Git LFS** (for asset management)
- **Windows 10/11** (primary development platform)

---

## Implementation Status

### ✅ Completed
- Unity 6.1 project setup with HDRP
- Hero data architecture (ScriptableObjects)
- Character selection screen with UI Toolkit
- Input system (keyboard, mouse, gamepad)
- Hero selection manager with animations
- Build automation scripts

### 🔄 In Progress
- Scene management framework
- Hero asset integration
- Audio system integration

### ⏳ Planned
- Main gameplay scene
- Unit control system
- Resource management
- Multiplayer networking

---

## Table of Contents
1. Vision & Scope
2. Repository Layout
3. Development Guides
4. Asset & Audio Pipelines
5. Multiplayer & Networking
6. Balance, Campaign, & Testing
7. Project Management & Contribution
8. Getting Started (Build / Play)
9. License

---

## 1. Vision & Scope
* **Gameplay** – 4 asymmetrical factions, hero-centric combat, TFT-accurate formulas (+1 damage per primary stat, armor = `(armor*0.06)/(1+armor*0.06)`, etc.).
* **Visual Target** – Warcraft III Reforged-quality models (5–10 k tris heroes, 4 K textures), HDRP with real-time ray tracing.
* **Platforms** – PC (Windows/Linux), scalable to Steam Deck.
* **Open & Free** – Unity free tier, AI-generated assets using only free tools; everything in this repo is MIT-licensed.

Full design details: `.claude_docs/GAME_PRD.md`, `.claude_docs/GDD.md`, `.claude_docs/TECH_SPECS.md`.

---

## 2. Repository Layout
