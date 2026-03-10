using CubeViewer.Scene;
using CubeViewer.Renderer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace CubeViewer.Renderer
{
    public class Sphere : SceneObject
    {
        public Sphere(float radius = 1f, int sectors = 24, int stacks = 16)
            : base(CreateMesh(radius, sectors, stacks), CreateEdgeMesh(radius, sectors, stacks))
        { }

        private static Mesh CreateMesh(float radius, int sectors, int stacks)
        {
            var vertices = new List<float>();
            var indices = new List<uint>();

            for(int i=0;i<=stacks;i++)
            {
                float phi = MathHelper.PiOver2 - i*MathHelper.Pi/stacks;
                float y = radius * MathF.Sin(phi);
                float r = radius * MathF.Cos(phi);

                for(int j=0;j<=sectors;j++)
                {
                    float theta = j*2*MathF.PI/sectors;
                    float x = r * MathF.Cos(theta);
                    float z = r * MathF.Sin(theta);
                    vertices.Add(x); vertices.Add(y); vertices.Add(z);
                    vertices.Add((x/radius+1f)/2f); vertices.Add((y/radius+1f)/2f); vertices.Add((z/radius+1f)/2f);
                }
            }

            for(int i=0;i<stacks;i++)
            {
                int k1 = i*(sectors+1);
                int k2 = k1 + sectors+1;

                for(int j=0;j<sectors;j++,k1++,k2++)
                {
                    if(i!=0) { indices.Add((uint)k1); indices.Add((uint)k2); indices.Add((uint)(k1+1)); }
                    if(i!=(stacks-1)){indices.Add((uint)(k1+1)); indices.Add((uint)k2); indices.Add((uint)(k2+1)); }
                }
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), PrimitiveType.Triangles);
        }

        private static Mesh CreateEdgeMesh(float radius,int sectors,int stacks)
        {
            var vertices = new List<float>();
            var indices = new List<uint>();

            for(int i=0;i<=stacks;i++)
            {
                float phi = MathHelper.PiOver2 - i*MathHelper.Pi/stacks;
                float y = radius * MathF.Sin(phi);
                float r = radius * MathF.Cos(phi);

                for(int j=0;j<=sectors;j++)
                {
                    float theta = j*2*MathF.PI/sectors;
                    float x = r * MathF.Cos(theta);
                    float z = r * MathF.Sin(theta);
                    vertices.Add(x); vertices.Add(y); vertices.Add(z);
                    vertices.Add(0); vertices.Add(0); vertices.Add(0);
                }
            }

            for(int i=0;i<stacks;i++)
            {
                int k1 = i*(sectors+1);
                int k2 = k1+sectors+1;

                for(int j=0;j<sectors;j++,k1++,k2++)
                {
                    indices.Add((uint)k1); indices.Add((uint)(k1+1));
                    indices.Add((uint)k1); indices.Add((uint)k2);
                }
            }

            return new Mesh(vertices.ToArray(),indices.ToArray(),PrimitiveType.Lines);
        }
    }
}