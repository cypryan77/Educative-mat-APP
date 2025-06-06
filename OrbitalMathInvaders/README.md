# Orbital Math Invaders

A 2D Unity game where players defend against falling meteors by solving math problems.

## Current Status

This project is currently under development. The core C# scripts and basic structure are being established.
**Note:** Due to the development environment, Unity Editor specific configurations (Prefabs, Scene setup, UI linking, Project Settings) are described in instructions rather than being pre-configured in the project files. Manual setup in the Unity Editor is required using the provided scripts and guidelines.

## Build Targets

The intended build targets for this project are:
- WebGL
- Windows (Standalone)
- Android

## How to Use (Manual Unity Setup)

1.  Create a new Unity 2D project (Unity 2022 LTS).
2.  Copy the `Assets` folder from this repository into your Unity project, replacing the existing one.
3.  Open the Unity project.
4.  **Scenes:** The `Assets/Scenes/MainMenu.unity` and `Assets/Scenes/GamePlay.unity` files are currently placeholders. You will need to:
    *   Recreate these scenes or open them and save them properly to make them valid Unity scenes.
    *   Follow the UI setup instructions (provided separately by the AI assistant) to populate the scenes with Canvases, TextMeshPro elements, buttons, etc.
5.  **Prefabs:** Follow the prefab creation instructions (provided separately by the AI assistant) to create:
    *   `MeteorPrefab`
    *   `CometPrefab`
    *   `ProjectilePrefab`
    *   `PlayerCannonPrefab`
    *   Assign the corresponding scripts from `Assets/Scripts` to these prefabs and configure their components in the Inspector.
6.  **TextMeshPro:** Ensure TextMeshPro is imported (Window -> TextMeshPro -> Import TMP Essentials).
7.  **ScriptableObjects:** Create `DifficultyLevelData` ScriptableObject instances as per later instructions.
8.  **Linkages:** Connect UI elements' events (e.g., button clicks, dropdown changes) to the appropriate public methods in the `GameManager.cs`, `InputManager.cs`, `EquationGenerator.cs` scripts. Assign prefabs to spawner fields, etc.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
