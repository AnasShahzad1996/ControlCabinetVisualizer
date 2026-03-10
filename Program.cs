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
        var nws = new NativeWindowSettings() { ClientSize = new Vector2i(1400, 768), Title = "CubeViewer" };
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

        // Axes (excluded from map)
        var axes = CoordinateAxes.Create(3f);
        scene.AddObject(axes);

        // Load model
        var objs = ObjImporter.Load(
            "Models/Rittal_AX_1033_300_300_210/new.obj",
            new Vector3(0, 0, 0),
            new Vector3(-0.01f),
            new Vector3(0.8f),
            false
        );
        foreach (var o in objs)
        {
            scene.AddObject(o);
            namedObjects.Add(($"Part {namedObjects.Count + 1}", o));
        }

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
                // Only rotate if clicking in the 3D viewport (left 70%)
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
            int split = (int)(W * 0.7f);   // 3D view: left 70%
            int mapW  = W - split;          // 2D map:  right 30%

            // ── 3D viewport ──
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

            // GUI overlaid on 3D view
            GL.Viewport(0, 0, split, H);
            gui?.Render(label => visibility[label]);

            // ── 2D map viewport ──
            mapView?.Render(namedObjects, visibility, split, 0, mapW, H);

            // Restore full viewport for swap
            GL.Viewport(0, 0, W, H);
            window.SwapBuffers();
        };

        window.Run();
    }
}