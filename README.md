# Project ID: 30

# class: SE G1 Gen10

# Game Name: Whispering Wood Cafe

# Project Member: Prom Sereyreaksa, Kao Sodavann, Chea Ilong, Kosal Sophanith, Chheng Bunheang

## Project Overview

This is a comprehensive Unity-based 3D adventure game featuring a cafe-themed world with NPCs, quests, inventory systems, and interactive dialogue game set in a charming low-poly cafe environment.

## Features

### Core Gameplay Systems

- **Third-Person Character Controller** - Smooth player movement and camera controls
- **Dialogue System** - Interactive conversations with NPCs
- **Quest System** - Track and complete quests with requirements and rewards
- **Inventory Management** - Collect and manage items (berries, water, etc.)
- **Scene Transitions** - Seamless movement between different game areas

### Game Scenes

- **GUI** - Main menu and UI interfaces
- **Inside** - Interior cafe environment
- **OutsideMap** - Outdoor exploration area

### Interactive Elements

- NPC dialogue and interactions
- Quest tracking and completion
- Item collection and crafting
- Door triggers for scene transitions
- Background animations, music and visual effects

## Technical Details

### Unity Version

- Built with Unity (Universal Render Pipeline)

### Key Scripts & Systems

#### Player Systems

- `ThirdPersonController.cs` - Advanced character movement
- `PlayerController.cs` - Player input handling
- `PlayerMovement.cs` - Movement mechanics
- `PersistentPlayer.cs` - Player persistence across scenes

#### Dialogue & NPCs

- `DialogueManager.cs` - Manages dialogue flow
- `NPCDialogue.cs` - NPC conversation handlers
- `InteractionUI.cs` - Player-NPC interaction interface

#### Quest System

- `QuestManager.cs` - Quest tracking and management
- `Quest.cs` - Quest data structures
- `QuestUI.cs` - Quest display interface
- `QuestDebugHelper.cs` - Development debugging tools

#### Inventory

- `StartingItems.cs` - Initial player inventory setup
- Various inventory management scripts

#### Utilities

- `SceneTransition.cs` - Scene loading management
- `EscapeMenu.cs` - Pause menu functionality

### Editor Tools

Located in `Assets/Scripts/Editor/`:

- **FindMissingScripts.cs** - Scan scenes and prefabs for missing script references
- **UnityProjectDiagnostics.cs** - Comprehensive project health checker
- **FixMissingScripts.cs** - Automatically remove missing script components

Access these tools in Unity via: `Tools > Find Missing Scripts`, `Tools > Project Diagnostics`, `Tools > Fix Missing Scripts`

## Assets & Resources

### 3D Models & Environments

- Coffee Shop Starter Pack
- Dining Set
- Low Poly Nature Pack Lite
- Free Low Poly Modular Character Pack (Fantasy Dream)
- Little Ghost LP (FREE)
- Country House & 17th Century Building models

### Visual Effects

- Free HDR Skyboxes Pack
- Custom skybox materials
- Post-processing effects

### Audio

- Background music (Soothing Tunic Music)
- Sound effects (Bubble Pop)

### UI/Graphics

- Custom buttons and backgrounds
- Logo assets
- TextMesh Pro integration

## Getting Started

### Prerequisites

- Unity 2021.3 or later (LTS recommended)
- Universal Render Pipeline package
- TextMesh Pro (included)

### Setup Instructions

#### Option 1: Play the Pre-Built Game

1. **Navigate to the `Build Game` folder** - Contains the pre-built executable and video demo
2. **Run the game executable** - Double-click to play immediately
3. **Watch the video demo** - See gameplay footage

#### Option 2: Open in Unity Editor

1. **Clone or download** this project
2. **Open in Unity Hub** - Add the project folder
3. **Let Unity import** all assets (may take a few minutes on first load)
4. **Open a scene** - Start with `GUI.unity` or `OutsideMap.unity`
5. **Press Play** to test the game


## Building the Game

### Important: Editor Scripts Location

All Unity Editor scripts MUST be in a folder named `Editor` to compile correctly. The following scripts are in `Assets/Scripts/Editor/`:

- FindMissingScripts.cs
- UnityProjectDiagnostics.cs
- FixMissingScripts.cs

**Do not move these scripts** out of the Editor folder or the project will fail to build.

### Build Steps

1. Go to `File > Build Settings`
2. Add all required scenes (GUI, Inside, OutsideMap)
3. Select target platform (Windows, Mac, Linux)
4. Click `Build` or `Build and Run`

## Troubleshooting

### Compilation Errors

If you see errors about `EditorWindow` or `MenuItem` not being found:

- Ensure all Editor scripts are in a folder named `Editor`
- Unity automatically compiles Editor scripts into a separate assembly
- Never use Editor scripts in runtime code

### Missing Script References

Use the built-in diagnostic tools:

1. Open `Tools > Find Missing Scripts`
2. Click "Search Everything"
3. Use `Tools > Fix Missing Scripts` to clean up

### Performance Issues

- Check `Tools > Project Diagnostics` for missing references
- Reduce post-processing effects for lower-end hardware
- Optimize scene lighting and shadows

## Project Structure

```
Assets/
├── Scripts/
│   ├── Audio/          - Sound management
│   ├── Cafe/           - Cafe-specific logic
│   ├── Camera/         - Camera controllers
│   ├── Data/           - Game data structures
│   ├── Dialogue/       - NPC dialogue system
│   ├── Editor/         - Unity Editor tools 
│   ├── Inventory/      - Item management
│   ├── Managers/       - Game managers
│   ├── Player/         - Player controllers
│   ├── Quest/          - Quest system
│   ├── Test/           - Testing scripts
│   └── Utility/        - Helper utilities
├── Scenes/             - Game scenes
├── Prefabs/            - Reusable game objects
├── ScriptableObject/   - Data assets
├── Materials/          - Visual materials
└── [Asset Packs]/      - Third-party assets
```

## Credits

### Third-Party Assets

- BOXOPHOBIC - Skybox Cubemap Extended
- Coffee Shop Starter Pack
- Low Poly Nature Pack Lite
- Fantasy Dream Character Pack
- Little Ghost LP (FREE)
- HDR Skyboxes Pack
- Maxime Brunoni - Buildings
- Neko Legends - Music
- Scanta - Assets
- Skyden Games - Assets
- Supercyan - Character Pack Animal People

## License

This is a student project for educational purposes.

## Contributing

This is a final project, but suggestions and feedback are welcome!

---

**Last Updated:** December 28, 2025  
**Unity Version:** 2021.3+ LTS  
**Status:** In Development
