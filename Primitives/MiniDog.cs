using CubeViewer.Scene;
using CubeViewer.Renderer;
using OpenTK.Mathematics;

namespace CubeViewer.Primitives
{
    public class MiniDog
    {
        // Store the dog parts in a list
        private List<SceneObject> objects = new List<SceneObject>();

        public IEnumerable<SceneObject> SceneObjects => objects; // getter for Program.cs

        public MiniDog(Vector3 position)
        {
            // Body
            var body = new Cube(1.0f){ Position = position + new Vector3(0,0.5f,0), ColorHex = "#AAAAAA", EdgeColorHex = "#000000" };
            objects.Add(body);

            // Head
            var head = new Sphere(0.4f){ ColorHex="#AAAAAA", EdgeColorHex="#000000" };
            head.Position = position + new Vector3(0,1.2f,0);
            objects.Add(head);

            // Tail
            var tail = new Cube(0.2f){ Position = position + new Vector3(0,-0.1f,0.6f), ColorHex = "#AAAAAA", EdgeColorHex = "#000000" };
            objects.Add(tail);
            objects.Add(tail);

            // Legs
            for(int x=-1;x<=1;x+=2)
            for(int z=-1;z<=1;z+=2)
            {
                var leg = new Cube(0.2f){ Position = position + new Vector3(0.3f*x,0,0.3f*z), ColorHex = "#AAAAAA", EdgeColorHex = "#000000" };
                objects.Add(leg);
            }

            // Ears
            for(int x=-1;x<=1;x+=2)
            {
                var ear = new Cube(0.15f){ Position = position + new Vector3(0.2f*x,1.5f,0.15f), ColorHex = "#AAAAAA", EdgeColorHex = "#000000" };
                objects.Add(ear);
            }
        }
    }
}