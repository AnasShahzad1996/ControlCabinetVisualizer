using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CubeViewer.Renderer
{
    public class Shader
    {
        public int Handle { get; private set; }

        public Shader(string vertexSource, string fragmentSource)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSource);
            GL.CompileShader(vertex);
            CheckCompile(vertex);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSource);
            GL.CompileShader(fragment);
            CheckCompile(fragment);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertex);
            GL.AttachShader(Handle, fragment);
            GL.LinkProgram(Handle);

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        private void CheckCompile(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
                throw new Exception(GL.GetShaderInfoLog(shader));
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetMatrix4(string name, Matrix4 mat)
        {
            int loc = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(loc, false, ref mat);
        }

        public void SetVector3(string name, Vector3 vec)
        {
            int loc = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(loc, vec);
        }
    }
}