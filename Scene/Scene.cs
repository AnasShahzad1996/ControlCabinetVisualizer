using CubeViewer.Renderer;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace CubeViewer.Scene
{
    public class Scene
    {
        private List<SceneObject> objects = new List<SceneObject>();

        public void AddObject(SceneObject obj) => objects.Add(obj);

        public void Render(Shader shader, Matrix4 view, Matrix4 projection)
        {
            shader.Use();
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            foreach(var obj in objects)
                if(obj.Visible)
                    obj.Draw(shader);
        }
    }
}