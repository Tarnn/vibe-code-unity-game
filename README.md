# FrostRealm Chronicles ― Open-Source RTS in Unity 6.1 LTS

FrostRealm Chronicles is a fully open-source real-time-strategy game inspired by *Warcraft III: The Frozen Throne* and modernized with Unity 6.1 LTS, HDRP ray-traced visuals, and ECS/DOTS performance.  
All development, art, audio, balance, multiplayer, and testing workflows are driven by AI and documented in this repository.

## Quick Start for Developers

> **Note for Windows + WSL users** – The *.sh scripts are fully supported under WSL.  The helper scripts now auto-detect the WSL environment and will invoke the Windows-installed Unity Editor located in `C:\Program Files\Unity\Hub\Editor\6000.1.12f1\Editor\Unity.exe`.  Simply run the same commands shown below from your Ubuntu prompt.

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
- **Unity 6000.1.12f1** (Unity 6.1 LTS or newer 6.x build)
  - *Make sure to add the "Linux Build Support (IL2CPP)" module in Unity Hub so the WSL build scripts work.*
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
- Core game management systems
- Resource management framework
- RTS camera controller
- Unit selection system
- Audio management system
- Scene management framework

### 🔄 In Progress
- Hero asset integration (8 hero models available)
- Audio system integration
- Main gameplay scene development
- Unit control system implementation

### ⏳ Planned
- Main gameplay scene completion
- Unit control system refinement
- Multiplayer networking
- Campaign system
- Advanced AI systems

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

```text
Assets/                – Unity assets (art, audio, prefabs, scenes, scripts)
  Art/                 – Concept art, textures, models (8 hero models available)
  Data/                – ScriptableObjects, hero data (8 heroes configured)
  Prefabs/             – Prefab variants for heroes, UI, etc.
  Scripts/             – Runtime, Editor, and test scripts (C#)
      Core/            – Core engine-like utilities (GameManager, InputManager, etc.)
      Data/            – Data‐oriented ECS code
      Heroes/          – Hero abilities and logic
      UI/              – UI Toolkit components
  Scenes/              – MainMenu, CharacterSelection, Gameplay scenes
Build/                 – Generated builds (ignored by Git)
Logs/                  – Editor & build logs (gitignored)
Packages/              – Unity package manifest cache
ProjectSettings/       – Unity project settings (versioned)
TestResults/           – XML result files from Unity Test Runner
Tools/                 – Helper scripts and CI configs (if any)
```

---

## 3. Development Guides
1. **Build automation** – See `dev-*.sh` (Linux/WSL) and `dev-*.bat` (Windows) scripts for one-click build, run, test, and clean.
2. **Continuous Integration** – Sample GitHub Actions workflow coming soon to automate builds and tests on every pull request.
3. **Coding standards** – C# code follows Microsoft + Unity conventions; run `dotnet format` before committing.

## 4. Asset & Audio Pipelines
* Art uses Blender & Substance Painter; exports land in `Assets/Art/`.
* Audio is generated or recorded at 48 kHz WAV in `Assets/Audio/`.
* Addressables handle asset streaming; see `AddressableAssetsData/`.

## 5. Multiplayer & Networking
Planned Netcode for Entities (Unity's DOTS networking) with Steam Relay fallback.

## 6. Balance, Campaign, & Testing
Balance spreadsheets and campaign scripts live in `.claude_docs/` and are executed via in-Editor tools.

## 7. Project Management & Contribution
* Issues → GitHub Issues, labelled `bug`, `feature`, `tech-debt`.
* Branching model → **main** (stable) / **dev** (integration) / feature branches.
* Pull requests require passing CI and at least one code-review approval.

## 8. Getting Started (Build / Play)
```bash
# Windows + WSL (Ubuntu)
./dev-setup.sh   # one-time setup, pulls LFS assets
./dev-run.sh     # build & run Linux player via WSL-g or X-server

# pure Windows
dev-setup.bat
dev-run.bat
```

## 9. License
MIT — see `LICENSE` for details. Commercial use permitted; attribution appreciated.
