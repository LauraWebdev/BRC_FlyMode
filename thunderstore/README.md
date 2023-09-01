# BRC_FlyMode
A Bomb Rush Cyberfunk mod that adds a simple fly mode.

## Installation

### Automatically (via ThunderStore/R2ModMan)
- Download the mod through [Thunderstore](https://thunderstore.io/c/bomb-rush-cyberfunk/p/LauraSofia/BRC_FlyMode/) or R2ModMan

### Manually
- You must have BepInEx 5.4 already installed.
- Go to the [latest release](https://github.com/LauraWebdev/BRC_FlyMode/releases/latest), and download the FlyMode.dll
- Put the .dll in your BepInEx/Plugins/ folder

## How to use
Once you are ingame, you can press "Enter" on your keypad to enable flymode.

### Controls
- **Enter (Keypad) / P** - Activate/Deactivate FlyMode
- **Plus (Keypad) / O** - Enable/Disable PlayerModel
- **WASD** - Forward/Backward/Left/Right
- **Space** - Up
- **Shift** - Down
- **ScrollWheel** - Increase/Decrease fly speed

## Building from source
To build this project from source, you need to place the following dll files from the game in a folder named "libs":
- Assembly-CSharp.dll
- Unity.TextMeshPro.dll
- UnityEngine.IMGUIModule.dll
- UnityEngine.UI.dll