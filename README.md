# Unity-Screenshots

![](https://img.shields.io/badge/unity-2022.3%2B-000.svg) ![](https://img.shields.io/badge/version-1.0.0-blue.svg)

A premium, zero-friction screenshot tool for Unity Editor. Take high-resolution, perfectly framed screenshots of your game or scene with a single click, right from the editor.

## Requirements

* Unity 2022.3 or newer (Full support for Unity 6 / `UNITY_6000_5_OR_NEWER`).

* Built with UI Toolkit.

## Installation

### Via Unity Asset Store

*(Link coming soon pending approval)*

### Via Unity Package Manager (Git URL)

1. Open Unity and go to `Window > Package Manager`.

2. Click the **\+** button in the top left corner and select **"Add package from git URL..."**.

3. Paste the following URL and click **Add**:

```
https://github.com/MUVITON/Unity-Screenshots.git?path=Package
```

## Quick Start

Open the tool by navigating to the top menu:

**`Tools > Screenshots`**

## Interface & Features

![Main Window Interface](./Images/main_window_full.PNG)

### 1\. Camera Source

![Camera Source Section](./Images/section_camera.PNG)

Drag and drop any camera from your scene here. If left empty, the tool automatically uses your active **Scene View** camera and displays a helpful info box to let you know.

### 2\. Orientation & Resolution

![Orientation and Resolution Section](./Images/section_resolution.PNG)

Choose from a list of predefined popular device resolutions (e.g., iPhone Pro Max, Full HD, 4K). The Orientation dropdown offers three modes: **Landscape**, **Portrait**, and **Custom**.

#### 2.1 Custom Mode

![Custom Mode Controls](./Images/section_custom_mode.PNG)

When **Custom** orientation is selected, additional manual controls will dynamically appear:

1. **Aspect Ratio Lock:** Lock proportions while resizing.  
2. **Flip:** Quickly swap width and height.  
3. **Reset:** Return to default dimensions.

### 3\. Destination Folder

![Destination Folder Section](./Images/section_folder.PNG) 

Configure where your screenshots are saved. Click the foldout arrow next to **Folder** to access the base path settings:

* **Folder (Active Path):** The actual directory where screenshots will be saved. Use **`[...]`** to pick any custom folder manually.
* **`[A]` (Auto-Generate Path):** Automatically builds the target path by taking your **Default Folder** and appending the active Scene or Prefab name as a subfolder (e.g., `Screenshots/SampleScene`).
* **Default Folder:** The root directory used by the **`[A]`** auto-mode. You can change it via **`[...]`** or click **`[R]`** to reset it back to the default `Screenshots` folder.

### 4\. Dynamic Naming

![Dynamic Naming Section](./Images/section_naming.PNG)

Create a naming template using built-in macros. For example, `{P}_{S}_{R}` will generate a name like `MyProject_Level1_1920x1080.png`. A live preview of the final filename is displayed right below the input field so you always know exactly what you'll get.

### 5\. Action Panel

![Action Panel Section](./Images/section_actions.PNG)

* **Capture:** Takes the screenshot with the specified settings.  
* **Delete All / Delete Last:** Quickly wipe recent screenshots if you're not satisfied with the result directly from the editor.  
* **Open folder after capture:** Check this box to automatically reveal and highlight the new screenshot in your OS file browser (Explorer/Finder) immediately after a successful capture.

## Credits & License

Developed by [Muviton](https://github.com/MUVITON).

Released under the MIT License.