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
    static bool rotating=false, panning=false;

    static void Main()
    {
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings(){ ClientSize=new Vector2i(1024,768), Title="CubeViewer" };
        using var window = new GameWindow(gws,nws);

        // Shader
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

        // Add primitives
        //var cube = new Cube(1.9f);
        //cube.Position = new Vector3(-1,0,0);
        //cube.ColorHex="#FF5555"; cube.EdgeColorHex="#000000";
        //scene.AddObject(cube);

        //var sphere = new Sphere(0.7f);
        //sphere.Position = new Vector3(0,0,0);
        //sphere.ColorHex="#55FF55"; sphere.EdgeColorHex="#000000";
        //scene.AddObject(sphere);

        var axes = CoordinateAxes.Create(3f);
        scene.AddObject(axes);

        //var dog = new MiniDog(Vector3.Zero);
        //foreach(var obj in dog.SceneObjects)
        //    scene.AddObject(obj);

        var objs = ObjImporter.Load(
            "Models/Rittal_AX_1033_300_300_210/new.obj",
            new Vector3(0,0,0),
            new Vector3(-0.01f),
            new Vector3(0.8f),
            false
        );

        // print length of objs
        foreach(var o in objs)
            scene.AddObject(o);

        window.Load += () =>
        {
            GL.ClearColor(0.2f,0.3f,0.3f,1f);
            GL.Enable(EnableCap.DepthTest);
        };

        // Input
        window.MouseDown += e => { if(e.Button==MouseButton.Left) rotating=true; if(e.Button==MouseButton.Middle)panning=true; };
        window.MouseUp += e => { rotating=false; panning=false; lastMouse=Vector2.Zero; };
        window.MouseMove += e =>
        {
            if(!rotating && !panning){ lastMouse=Vector2.Zero; return; }
            if(lastMouse==Vector2.Zero){ lastMouse=e.Position; return; }
            var delta = e.Position - lastMouse;
            if(rotating){ camera.Yaw += delta.X*0.5f; camera.Pitch -= delta.Y*0.5f; camera.Pitch=Math.Clamp(camera.Pitch,-89f,89f);}
            if(panning)
            {
                var right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY,camera.Target));
                camera.Target += -right*delta.X*0.01f + Vector3.UnitY*delta.Y*0.01f;
            }
            lastMouse=e.Position;
        };
        window.MouseWheel += e => { camera.Distance -= e.OffsetY*0.5f; camera.Distance=Math.Clamp(camera.Distance,1f,20f); };

        // Render loop
        window.RenderFrame += args =>
        {
            GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit);
            var view = camera.GetViewMatrix();
            var proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), window.ClientSize.X/(float)window.ClientSize.Y,0.1f,100f);
            scene.Render(shader, view, proj);
            window.SwapBuffers();
        };

        window.Run();
    }
}