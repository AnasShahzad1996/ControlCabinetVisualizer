using OpenTK.Graphics.OpenGL4;
public class Mesh
{
    public int Vao { get; private set; }
    public int Vbo { get; private set; }
    public int Ebo { get; private set; }
    public int IndexCount { get; private set; }
    private PrimitiveType drawMode;

    public Mesh(float[] vertices, uint[] indices, PrimitiveType mode = PrimitiveType.Triangles)
    {
        drawMode = mode;
        IndexCount = indices.Length;

        Vao = GL.GenVertexArray();
        Vbo = GL.GenBuffer();
        Ebo = GL.GenBuffer();

        GL.BindVertexArray(Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
    }

    public void Draw()
    {
        GL.BindVertexArray(Vao);
        GL.DrawElements(drawMode, IndexCount, DrawElementsType.UnsignedInt, 0);
    }
}