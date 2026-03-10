using CubeViewer.Scene;
using CubeViewer.Renderer;
using OpenTK.Graphics.OpenGL4;

namespace CubeViewer.Renderer
{
    public class ParametricObject : SceneObject
    {
        public ParametricObject(float[] vertices, uint[] triangles, uint[] edgeLines)
            : base(
                new Mesh(vertices, triangles, PrimitiveType.Triangles),
                new Mesh(vertices, edgeLines, PrimitiveType.Lines)
            )
        {}
    }
}