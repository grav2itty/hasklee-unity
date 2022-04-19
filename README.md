# HaskleeUnity

Unity package for loading [Hasklee](https://github.com/grav2itty/hasklee) scenes.

Please note that this is not an editor asset importer. Scenes can only be loaded in play mode and there is no built-in save feature.

This is a **Windows only** release. Due to Unity's lack of proper support for native plugins on Linux (last time I checked) Csound would not work. A No-sound version, however, can be easily made.

## Installation

1. Create a new Unity 3D built-in renderer project
2. Install packages from Unity Store

    - [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)

        - During setup click **Create ASMDEF...**

    - [MoonSharp](https://assetstore.unity.com/packages/tools/moonsharp-33776)

        - After importing navigate to **Plugins/MoonSharp**
        - Create new **Assembly Definition** named **MoonSharpAsm**

3. [Install](https://docs.unity3d.com/Manual/upm-ui-giturl.html) packages from GitHub

    - [https://github.com/grav2ity/CsoundUnity.git](https://github.com/grav2ity/CsoundUnity.git)
    - [https://github.com/grav2ity/HaskleeUnity.git](https://github.com/grav2itty/HaskleeUnity.git)



## Usage

1. Setup Scene

    - Create Empty GameObject
    - Add Component **Hasklee** and choose Default Material
    - Add Component **Lua**
    - (optional) Some built-in widgets may not work correctly without this step. This will override default cursor.
        - Add Component **Curson N**
        - Add **HASKLEE_CURSOR** under **Project Settings -> Player -> Scripting Define Symbols**

2. Use **Hasklee.Hasklee.Instance** interface

        LoadFile
        Destroy
        RegisterComponent

3. Add some player / camera to get around
