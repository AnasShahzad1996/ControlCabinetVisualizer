using CubeViewer.Scene;
using CubeViewer.Renderer;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CubeViewer.Scene;

namespace CubeViewer.Utils
{
    public static class ObjImporter
    {
        public static List<SceneObject> Load(string path, Vector3 position, Vector3 scale, Vector3 color, bool normalizeToOrigin)
        {
            List<Vector3> vertices = new();
            List<uint> indices = new();
            List<SceneObject> objects = new();

            bool objectStarted = false;

            foreach (var raw in File.ReadLines(path))
            {
                string line = raw.Trim();

                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                if (line.StartsWith("mtllib") || line.StartsWith("usemtl"))
                    continue;

                // new object
                if (line.StartsWith("o "))
                {
                    if (indices.Count > 0)
                    {
                        objects.Add(CreateObject(vertices, indices, position, scale, color, normalizeToOrigin));
                        indices = new List<uint>();
                    }

                    objectStarted = true;
                    continue;
                }

                // vertex
                if (line.StartsWith("v "))
                {
                    var p = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    float x = float.Parse(p[1], CultureInfo.InvariantCulture);
                    float y = float.Parse(p[2], CultureInfo.InvariantCulture);
                    float z = float.Parse(p[3], CultureInfo.InvariantCulture);

                    vertices.Add(new Vector3(x, y, z));
                    continue;
                }

                // normal (ignored but allowed)
                if (line.StartsWith("vn "))
                    continue;

                // face
                if (line.StartsWith("f "))
                {
                    var p = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    List<uint> face = new();

                    for (int i = 1; i < p.Length; i++)
                    {
                        string token = p[i];

                        // handles
                        // f 1 2 3
                        // f 1/2/3
                        // f 1//3
                        var parts = token.Split('/');

                        uint index = uint.Parse(parts[0]) - 1;
                        face.Add(index);
                    }

                    // triangulate polygon
                    for (int i = 1; i < face.Count - 1; i++)
                    {
                        indices.Add(face[0]);
                        indices.Add(face[i]);
                        indices.Add(face[i + 1]);
                    }
                }
            }

            // finalize last object (important for bunny files)
            if (indices.Count > 0)
            {
                objects.Add(CreateObject(vertices, indices, position, scale, color, normalizeToOrigin));
            }

            return objects;
        }

        private static SceneObject CreateObject(
            List<Vector3> rawVertices,
            List<uint> triangleIndices,
            Vector3 position,
            Vector3 scale,
            Vector3 color,
            bool normalizeToOrigin
        )
        {

            Vector3 min = rawVertices[0];
            Vector3 max = rawVertices[0];

            foreach (var v in rawVertices)
            {
                min = Vector3.ComponentMin(min, v);
                max = Vector3.ComponentMax(max, v);
            }

            Console.WriteLine("OBJ Bounding Box:");
            Console.WriteLine($"X: min={min.X} max={max.X}");
            Console.WriteLine($"Y: min={min.Y} max={max.Y}");
            Console.WriteLine($"Z: min={min.Z} max={max.Z}");
            Console.WriteLine("-------------------------");
            
            Vector3 center = (min + max) * 0.5f;

            List<float> verts = new();

            foreach (var v in rawVertices)
            {
                Vector3 p;

                if (normalizeToOrigin)
                    p = (v - center);   // center object
                else
                    p = v;              // keep original coordinates

                p *= scale;
                p += position;

                verts.Add(p.X);
                verts.Add(p.Y);
                verts.Add(p.Z);

                verts.Add(color.X);
                verts.Add(color.Y);
                verts.Add(color.Z);
            }

            uint[] edges = GenerateEdges(triangleIndices);

            Mesh triMesh = new Mesh(
                verts.ToArray(),
                triangleIndices.ToArray(),
                PrimitiveType.Triangles
            );

            Mesh edgeMesh = new Mesh(
                verts.ToArray(),
                edges,
                PrimitiveType.Lines
            );

            return new SceneObject(triMesh, edgeMesh);
        }
        private static uint[] GenerateEdges(List<uint> triangles)
        {
            HashSet<(uint, uint)> edgeSet = new();

            for (int i = 0; i < triangles.Count; i += 3)
            {
                uint a = triangles[i];
                uint b = triangles[i + 1];
                uint c = triangles[i + 2];

                AddEdge(edgeSet, a, b);
                AddEdge(edgeSet, b, c);
                AddEdge(edgeSet, c, a);
            }

            List<uint> edges = new();

            foreach (var (a, b) in edgeSet)
            {
                edges.Add(a);
                edges.Add(b);
            }

            return edges.ToArray();
        }

        private static void AddEdge(HashSet<(uint, uint)> set, uint a, uint b)
        {
            if (a < b)
                set.Add((a, b));
            else
                set.Add((b, a));
        }
    }
}