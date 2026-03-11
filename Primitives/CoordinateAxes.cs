using CubeViewer.Scene;
using CubeViewer.Renderer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CubeViewer.Primitives
{
    public static class CoordinateAxes
    {
        public static List<SceneObject> CreateAll(float length = 2f)
        {
            var result = new List<SceneObject>();
            result.Add(CreateAxesLines(length));
            result.Add(CreateTipCube(new Vector3(length, 0, 0), new Vector3(1, 0, 0))); // X red
            result.Add(CreateTipCube(new Vector3(0, length, 0), new Vector3(0, 1, 0))); // Y green
            result.Add(CreateTipCube(new Vector3(0, 0, length), new Vector3(0, 0, 1))); // Z blue
            return result;
        }

        // Keep old Create() so nothing else breaks
        public static SceneObject Create(float length = 2f) => CreateAxesLines(length);

        static SceneObject CreateAxesLines(float length)
        {
            float[] vertices = {
                0,0,0, 1,0,0,  length,0,0, 1,0,0,  // X red
                0,0,0, 0,1,0,  0,length,0, 0,1,0,  // Y green
                0,0,0, 0,0,1,  0,0,length, 0,0,1   // Z blue
            };
            uint[] lines = { 0,1, 2,3, 4,5 };
            return new SceneObject(
                new Mesh(vertices, new uint[]{}, PrimitiveType.Triangles),
                new Mesh(vertices, lines, PrimitiveType.Lines)
            );
        }

        static SceneObject CreateTipCube(Vector3 pos, Vector3 color)
        {
            float s = 0.08f; // cube half-size
            float r = color.X, g = color.Y, b = color.Z;
            float[] v = {
                // each vertex: x,y,z,r,g,b
                -s,-s,-s,r,g,b,  s,-s,-s,r,g,b,  s, s,-s,r,g,b, -s, s,-s,r,g,b, // back
                -s,-s, s,r,g,b,  s,-s, s,r,g,b,  s, s, s,r,g,b, -s, s, s,r,g,b, // front
            };
            uint[] idx = {
                0,1,2, 0,2,3, // back
                4,5,6, 4,6,7, // front
                0,4,7, 0,7,3, // left
                1,5,6, 1,6,2, // right
                3,2,6, 3,6,7, // top
                0,1,5, 0,5,4  // bottom
            };
            var obj = new SceneObject(new Mesh(v, idx, PrimitiveType.Triangles));
            obj.Position = pos;
            obj.ColorRGB = color;
            return obj;
        }
    }
}