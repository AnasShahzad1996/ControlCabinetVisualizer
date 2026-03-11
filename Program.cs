using CubeViewer.Camera;
using CubeViewer.Scene;
using CubeViewer.Renderer;
using CubeViewer.Primitives;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using CubeViewer.Utils;

class Program
{
    static OrbitCamera camera = new();
    static Vector2 lastMouse = Vector2.Zero;
    static bool rotating = false, panning = false;

    static void Main()
    {
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings() { ClientSize = new Vector2i(1400, 768), Title = "ControlCabinetViewer" };
        using var window = new GameWindow(gws, nws);

        var shader = new Shader(
            @"#version 330 core
              layout(location=0) in vec3 aPos;
              layout(location=1) in vec3 aColor;
              uniform mat4 model;
              uniform mat4 view;
              uniform mat4 projection;
              out vec3 ourColor;
              void main(){ gl_Position=projection*view*model*vec4(aPos,1.0); ourColor=aColor; }",
            @"#version 330 core
              in vec3 ourColor;
              out vec4 FragColor;
              uniform vec3 objectColor;
              void main(){ FragColor=vec4(objectColor,1.0); }"
        );

        var scene = new Scene();
        SimpleGui? gui = null;
        MapView2D? mapView = null;
        var namedObjects = new List<(string Name, SceneObject Obj)>();
        var visibility = new Dictionary<string, bool>();

        // Axes
        foreach (var a in CoordinateAxes.CreateAll(3f))
            scene.AddObject(a);

        // ── Parse electrical_diagram.txt ──────────────────────────────────
        const string diagramPath = "Electrical_diagrams/diagram1.txt";
        if (!File.Exists(diagramPath))
        {
            Console.WriteLine($"ERROR: '{diagramPath}' not found.");
            return;
        }

        int cabinetCount = 0, railCount = 0, mcbCount = 0, rcdCount = 0;

        foreach (var rawLine in File.ReadLines(diagramPath))
        {
            var line = rawLine.Contains('#') ? rawLine[..rawLine.IndexOf('#')] : rawLine;
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 2) continue;

            string type = parts[0].ToUpperInvariant();
            string path = parts[1];

            try
            {
                switch (type)
                {
                    case "CABINET":
                    {
                        // CABINET, path, pos_x, pos_y, pos_z, scale, r, g, b
                        var pos   = new Vector3(F(parts,2), F(parts,3), F(parts,4));
                        var scale = new Vector3(F(parts,5));
                        var color = new Vector3(F(parts,6), F(parts,7), F(parts,8));

                        int partIdx = 0;
                        foreach (var o in ObjImporter.Load(path, pos, scale, color, false))
                        {
                            scene.AddObject(o);
                            namedObjects.Add((partIdx == 0 ? "Cabinet Enclosure" : "Mounting Plate", o));
                            partIdx++;
                        }
                        cabinetCount++;
                        break;
                    }

                    case "RAIL":
                    {
                        // RAIL, path, pos_x, pos_y, pos_z, scale, r, g, b
                        var pos   = new Vector3(F(parts,2), F(parts,3), F(parts,4));
                        var scale = new Vector3(F(parts,5));
                        var color = new Vector3(F(parts,6), F(parts,7), F(parts,8));

                        railCount++;
                        foreach (var o in ObjImporter.Load(path, pos, scale, color, false))
                        {
                            scene.AddObject(o);
                            namedObjects.Add(($"Rail {railCount}", o));
                        }
                        break;
                    }

                    case "RAIL_STACK":
                    {
                        // RAIL_STACK, path, count, start_x, start_y, z, spacing, scale, r, g, b
                        int   total   = (int)F(parts,2);
                        float startX  = F(parts,3);
                        float startY  = F(parts,4);
                        float z       = F(parts,5);
                        float spacing = F(parts,6);  // vertical distance between rail centers
                        var   scale   = new Vector3(F(parts,7));
                        var   color   = new Vector3(F(parts,8), F(parts,9), F(parts,10));

                        float curX = startX;
                        for (int i = 0; i < total; i++)
                        {
                            railCount++;
                            foreach (var o in ObjImporter.Load(path, new Vector3(curX, startY, z), scale, color, false))
                            {
                                scene.AddObject(o);
                                namedObjects.Add(($"Rail {railCount}", o));
                            }
                            curX += (2*spacing); // stack rightward
                        }
                        break;
                    }

                    case "MCB":
                    {
                        // MCB, path, count, start_x, y, z, width, offset, scale, r, g, b
                        int   total  = (int)F(parts,2);
                        float startX = F(parts,3);
                        float y      = F(parts,4);
                        float z      = F(parts,5);
                        float width  = F(parts,6);
                        float offset = F(parts,7);
                        var   scale  = new Vector3(F(parts,8));
                        var   color  = new Vector3(F(parts,9), F(parts,10), F(parts,11));

                        float curX = startX;
                        for (int i = 0; i < total; i++)
                        {
                            mcbCount++;
                            int partIdx = 1;
                            foreach (var o in ObjImporter.Load(path, new Vector3(curX, y, z), scale, color, false))
                            {
                                scene.AddObject(o);
                                namedObjects.Add(($"MCB {mcbCount}.{partIdx}", o));
                                partIdx++;
                            }
                            curX += width + offset;
                        }
                        break;
                    }

                    case "RCD":
                    {
                        // RCD, path, count, start_x, y, z, width, offset, scale, r, g, b
                        int   total  = (int)F(parts,2);
                        float startX = F(parts,3);
                        float y      = F(parts,4);
                        float z      = F(parts,5);
                        float width  = F(parts,6);
                        float offset = F(parts,7);
                        var   scale  = new Vector3(F(parts,8));
                        var   color  = new Vector3(F(parts,9), F(parts,10), F(parts,11));

                        float curX = startX;
                        for (int i = 0; i < total; i++)
                        {
                            rcdCount++;
                            int partIdx = 1;
                            foreach (var o in ObjImporter.Load(path, new Vector3(curX, y, z), scale, color, false))
                            {
                                scene.AddObject(o);
                                namedObjects.Add(($"RCD {rcdCount}.{partIdx}", o));
                                partIdx++;
                            }
                            curX += width + offset;
                        }
                        break;
                    }

                    default:
                        Console.WriteLine($"WARNING: Unknown type '{type}' — skipping.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR parsing line: '{rawLine}'\n  {ex.Message}");
            }
        }

        Console.WriteLine($"Loaded: {cabinetCount} cabinet(s), {railCount} rail(s), {mcbCount} MCB(s), {rcdCount} RCD(s)");

        // ── Window setup ─────────────────────────────────────────────────────
        window.Load += () =>
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
            GL.Enable(EnableCap.DepthTest);

            gui = new SimpleGui(window.ClientSize.X, window.ClientSize.Y);
            mapView = new MapView2D();

            float y = 10;
            foreach (var (name, obj) in namedObjects)
            {
                visibility[name] = true;
                string n = name; SceneObject captured = obj;
                gui.AddToggleButton(name, 10, y, 180, 28,
                    () => visibility[n],
                    () => { visibility[n] = !visibility[n]; captured.Visible = visibility[n]; });
                y += 34;
            }
        };

        window.Resize += e =>
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            gui?.Resize(e.Width, e.Height);
        };

        window.MouseDown += e =>
        {
            if (e.Button == MouseButton.Left)
            {
                float split = window.ClientSize.X * 0.7f;
                if (window.MouseState.X < split)
                {
                    gui?.HandleClick(window.MouseState.X, window.MouseState.Y);
                    rotating = true;
                }
            }
            if (e.Button == MouseButton.Middle) panning = true;
        };
        window.MouseUp += e => { rotating = false; panning = false; lastMouse = Vector2.Zero; };
        window.MouseMove += e =>
        {
            if (!rotating && !panning) { lastMouse = Vector2.Zero; return; }
            if (lastMouse == Vector2.Zero) { lastMouse = e.Position; return; }
            var delta = e.Position - lastMouse;
            if (rotating) { camera.Yaw += delta.X * 0.5f; camera.Pitch -= delta.Y * 0.5f; camera.Pitch = Math.Clamp(camera.Pitch, -89f, 89f); }
            if (panning)
            {
                var right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, camera.Target));
                camera.Target += -right * delta.X * 0.01f + Vector3.UnitY * delta.Y * 0.01f;
            }
            lastMouse = e.Position;
        };
        window.MouseWheel += e =>
        {
            if (window.MouseState.X < window.ClientSize.X * 0.7f)
            {
                camera.Distance -= e.OffsetY * 0.5f;
                camera.Distance = Math.Clamp(camera.Distance, 1f, 20f);
            }
        };

        window.RenderFrame += args =>
        {
            int W = window.ClientSize.X, H = window.ClientSize.Y;
            int split = (int)(W * 0.7f);
            int mapW  = W - split;

            GL.Viewport(0, 0, split, H);
            GL.Scissor(0, 0, split, H);
            GL.Enable(EnableCap.ScissorTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.ScissorTest);

            var view = camera.GetViewMatrix();
            var proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f), split / (float)H, 0.1f, 100f);
            scene.Render(shader, view, proj);

            GL.Viewport(0, 0, split, H);
            gui?.Render(label => visibility[label]);

            mapView?.Render(namedObjects, visibility, split, 0, mapW, H);

            GL.Viewport(0, 0, W, H);
            window.SwapBuffers();
        };

        window.Run();
    }

    static float F(string[] parts, int idx) =>
        float.Parse(parts[idx], System.Globalization.CultureInfo.InvariantCulture);
}