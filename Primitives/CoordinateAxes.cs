using CubeViewer.Scene;
using CubeViewer.Renderer;
using OpenTK.Graphics.OpenGL4;

namespace CubeViewer.Primitives
{
    public static class CoordinateAxes
    {
        public static SceneObject Create(float length = 2f)
        {
            float[] vertices = {
                0,0,0,1,0,0,  length,0,0,1,0,0,  // X axis red
                0,0,0,0,1,0,  0,length,0,0,1,0,  // Y axis green
                0,0,0,0,0,1,  0,0,length,0,0,1   // Z axis blue
            };

            uint[] lines = {0,1,2,3,4,5}; // start/end indices for lines

            return new SceneObject(
                new Mesh(vertices, new uint[]{}, PrimitiveType.Triangles), // empty triangles
                new Mesh(vertices, lines, PrimitiveType.Lines)
            );
        }
    }
}