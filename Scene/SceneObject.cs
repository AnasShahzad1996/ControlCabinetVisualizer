using CubeViewer.Renderer;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace CubeViewer.Scene
{
    public class SceneObject
    {
        protected Mesh mesh;
        protected Mesh edgeMesh;
        protected Vector3 position = Vector3.Zero;
        protected Vector3 color = new Vector3(1f,1f,1f);
        protected Vector3 edgeColor = new Vector3(0f,0f,0f);
        public bool Visible { get; set; } = true;
        
        public SceneObject(Mesh mesh, Mesh edgeMesh = null)
        {
            this.mesh = mesh;
            this.edgeMesh = edgeMesh;
        }

        public Vector3 Position { get => position; set => position = value; }
        public Vector3 ColorRGB { get => color; set => color = value; }
        public Vector3 EdgeColor { get => edgeColor; set => edgeColor = value; }

        public string ColorHex
        {
            get => $"#{(int)(color.X*255):X2}{(int)(color.Y*255):X2}{(int)(color.Z*255):X2}";
            set
            {
                if(uint.TryParse(value.Replace("#",""), System.Globalization.NumberStyles.HexNumber, null, out uint hex))
                {
                    color.X = ((hex>>16)&0xFF)/255f;
                    color.Y = ((hex>>8)&0xFF)/255f;
                    color.Z = (hex&0xFF)/255f;
                }
            }
        }

        public string EdgeColorHex
        {
            get => $"#{(int)(edgeColor.X*255):X2}{(int)(edgeColor.Y*255):X2}{(int)(edgeColor.Z*255):X2}";
            set
            {
                if(uint.TryParse(value.Replace("#",""), System.Globalization.NumberStyles.HexNumber, null, out uint hex))
                {
                    edgeColor.X = ((hex>>16)&0xFF)/255f;
                    edgeColor.Y = ((hex>>8)&0xFF)/255f;
                    edgeColor.Z = (hex&0xFF)/255f;
                }
            }
        }

        public void Draw(Shader shader)
        {
            shader.Use();
            Matrix4 model = Matrix4.CreateTranslation(position);

            shader.SetMatrix4("model", model);

            // solid mesh
            shader.SetVector3("objectColor", color);
            mesh.Draw();

            // edges
            if(edgeMesh != null)
            {
                shader.SetVector3("objectColor", edgeColor);
                GL.LineWidth(2f);
                edgeMesh.Draw();
            }
        }

        public void Move(Vector3 delta) => position += delta;
    }
}