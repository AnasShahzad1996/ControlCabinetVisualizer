using CubeViewer.Scene;
using OpenTK.Graphics.OpenGL4;

namespace CubeViewer.Renderer
{
    public class Cube : SceneObject
    {
        public Cube(float size = 1f) 
            : base(
                // Solid mesh
                new Mesh(
                    new float[]
                    {
                        -size/2,-size/2,-size/2,1,0,0,
                         size/2,-size/2,-size/2,0,1,0,
                         size/2, size/2,-size/2,0,0,1,
                        -size/2, size/2,-size/2,1,1,0,
                        -size/2,-size/2, size/2,1,0,1,
                         size/2,-size/2, size/2,0,1,1,
                         size/2, size/2, size/2,1,1,1,
                        -size/2, size/2, size/2,0,0,0
                    },
                    new uint[]
                    {
                        0,1,2,2,3,0,
                        4,5,6,6,7,4,
                        0,4,7,7,3,0,
                        1,5,6,6,2,1,
                        3,2,6,6,7,3,
                        0,1,5,5,4,0
                    },
                    PrimitiveType.Triangles
                ),
                // Edge mesh
                new Mesh(
                    new float[]
                    {
                        -size/2,-size/2,-size/2,0,0,0,
                         size/2,-size/2,-size/2,0,0,0,
                         size/2, size/2,-size/2,0,0,0,
                        -size/2, size/2,-size/2,0,0,0,
                        -size/2,-size/2, size/2,0,0,0,
                         size/2,-size/2, size/2,0,0,0,
                         size/2, size/2, size/2,0,0,0,
                        -size/2, size/2, size/2,0,0,0
                    },
                    new uint[]
                    {
                        0,1,1,2,2,3,3,0,
                        4,5,5,6,6,7,7,4,
                        0,4,1,5,2,6,3,7
                    },
                    PrimitiveType.Lines
                )
            )
        {}
    }
}