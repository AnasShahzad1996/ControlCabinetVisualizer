# CubeViewer — Electrical Cabinet Visualizer

A real-time 3D viewer for electrical cabinet models (e.g. Rittal AX series), built with OpenTK and OpenGL. Includes a 2D top-down map view with scaled axes, per-object visibility toggles, and orbit camera controls.

---

## Screenshot

![CubeViewer Screenshot](screenshot.png)

---

## What It Does

- **3D Viewport** — loads `.obj` cabinet models and renders them with colored faces and edge outlines
- **Orbit Camera** — left-click drag to rotate, middle-click drag to pan, scroll to zoom
- **Visibility Panel** — toggle individual model parts on/off with labeled buttons (green = visible, red = hidden)
- **2D Map View** — side panel showing a top-down X/Y projection of all visible objects as bounding boxes, with a grid and labeled scale axes
- **Coordinate Axes** — X (red), Y (green), Z (blue) with colored cube arrowheads

---

## Requirements

### System

| Requirement | Version |
|-------------|---------|
| .NET SDK | 8.0 or later |
| OpenGL | 3.3 core or later |
| OS | Linux, Windows, or macOS |

### On Linux, install these system libraries:

```bash
sudo apt update
sudo apt install -y \
    libgl1 \
    libglu1-mesa \
    libx11-dev \
    libxrandr-dev \
    libxi-dev \
    libxinerama-dev \
    libxcursor-dev
```

### .NET Packages (auto-installed via NuGet)

| Package | Version |
|---------|---------|
| OpenTK | 4.9.4 |
| ImGui.NET | 1.91.6.1 |
| FontStashSharp | 1.3.10 |
| System.Drawing.Common | 10.0.3 |

---

## Installation

### 1. Install .NET 8 SDK

**Linux (Ubuntu/Debian):**
```bash
sudo apt install -y dotnet-sdk-8.0
```

**Or download from:** https://dotnet.microsoft.com/download/dotnet/8.0

**Verify installation:**
```bash
dotnet --version
# Expected: 8.0.x
```

### 2. Clone the repository

```bash
git clone https://github.com/your-username/ControlCabinetVisualizer.git
cd ControlCabinetVisualizer
```

### 3. Restore NuGet packages

```bash
dotnet restore
```

---

## Running

```bash
dotnet run
```

Or build first then run the binary:

```bash
dotnet build -c Release
./bin/Release/net8.0/CubeViewer
```

---

## Controls

| Input | Action |
|-------|--------|
| Left-click + drag | Rotate camera |
| Middle-click + drag | Pan camera |
| Scroll wheel | Zoom in/out |
| GUI buttons (left panel) | Toggle object visibility |

---

## Electrical Diagram File (`electrical_diagram.txt`)

Place this file in the same directory as the executable. It defines all objects in the scene — no recompilation needed to change the layout.

### Supported Types

| Type | Description |
|------|-------------|
| `CABINET` | The main enclosure (first part = enclosure, second = mounting plate) |
| `RAIL` | A single DIN rail at a fixed position |
| `RAIL_STACK` | Multiple DIN rails stacked vertically with a fixed spacing |
| `MCB` | A row of MCBs placed side by side along the X axis |
| `RCD` | A row of RCDs placed side by side along the X axis |

### Format
```
# Lines starting with # are comments and are ignored

# Single objects
CABINET,    path, pos_x, pos_y, pos_z, scale, r, g, b
RAIL,       path, pos_x, pos_y, pos_z, scale, r, g, b

# Stacked rails (count placed downward from start_y, separated by spacing)
RAIL_STACK, path, count, start_x, start_y, z, spacing, scale, r, g, b

# Repeated components placed side by side along X
MCB,        path, count, start_x, y, z, width, offset, scale, r, g, b
RCD,        path, count, start_x, y, z, width, offset, scale, r, g, b
```

### Example
```
CABINET,    Models/Electrical_cabinet.obj, 0, 0, 0, 0.01, 0.8, 0.8, 0.8
RAIL_STACK, Models/din_rails.obj, 3, -0.675, 0.625, 0.1, 0.625, 0.01, 0.8, 0.2, 0.2
MCB,        Models/MCB.obj, 6, -0.880, 0.625, 0.075, 0.180, 0.01, 0.01, 0.8, 0.2, 0.2
RCD,        Models/RCD.obj, 3, -0.880, -0.625, 0.075, 0.720, 0.01, 0.01, 0.2, 0.8, 0.2
```

> **Note:** Colors are RGB floats in the range `0.0 – 1.0`. Unknown type names print a warning to the console and are skipped without crashing.

---

## Project Structure

```
CubeViewer/
├── Camera/
│   └── OrbitCamera.cs        # Orbit camera with yaw/pitch/distance
├── Primitives/
│   └── CoordinateAxes.cs     # X/Y/Z axis lines and tip cubes
├── Renderer/
│   ├── Shader.cs             # GLSL shader wrapper
│   └── Mesh.cs               # VAO/VBO/EBO mesh
├── Scene/
│   ├── Scene.cs              # Scene graph and render loop
│   └── SceneObject.cs        # Base renderable object with bounds
├── Utils/
│   └── ObjImporter.cs        # .obj file loader
├── SimpleGui.cs              # Immediate-mode button overlay (no ImGui)
├── MapView2D.cs              # 2D top-down map with grid and scale
├── Program.cs                # Entry point
└── CubeViewer.csproj
```

---

## Troubleshooting

**`libgdiplus` error on Linux:**
```bash
sudo apt install libgdiplus
```

**Black screen / OpenGL error:**
Make sure your GPU supports OpenGL 3.3. Check with:
```bash
glxinfo | grep "OpenGL version"
```

**Model not loading:**
Verify the `.obj` path in `Program.cs` matches your file location.