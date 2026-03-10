// MapView2D.cs
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using CubeViewer.Scene;

public class MapView2D
{
    private int _shader, _vao, _vbo;
    private int _fontTex;
    private int _textShader, _tVao, _tVbo;
    private int _vpX, _vpY, _vpW, _vpH;

    private static readonly Vector4[] Colors = {
        new(0.96f, 0.26f, 0.21f, 0.85f),
        new(0.13f, 0.59f, 0.95f, 0.85f),
        new(0.30f, 0.69f, 0.31f, 0.85f),
        new(1.00f, 0.76f, 0.03f, 0.85f),
        new(0.61f, 0.15f, 0.69f, 0.85f),
        new(1.00f, 0.34f, 0.13f, 0.85f),
        new(0.00f, 0.74f, 0.83f, 0.85f),
        new(0.91f, 0.12f, 0.39f, 0.85f),
    };

    // 8x8 bitmap font — same as SimpleGui
    private static readonly byte[] Font8x8 = {
        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        0x18,0x3C,0x3C,0x18,0x18,0x00,0x18,0x00,
        0x36,0x36,0x00,0x00,0x00,0x00,0x00,0x00,
        0x36,0x36,0x7F,0x36,0x7F,0x36,0x36,0x00,
        0x0C,0x3E,0x03,0x1E,0x30,0x1F,0x0C,0x00,
        0x00,0x63,0x33,0x18,0x0C,0x66,0x63,0x00,
        0x1C,0x36,0x1C,0x6E,0x3B,0x33,0x6E,0x00,
        0x06,0x06,0x03,0x00,0x00,0x00,0x00,0x00,
        0x18,0x0C,0x06,0x06,0x06,0x0C,0x18,0x00,
        0x06,0x0C,0x18,0x18,0x18,0x0C,0x06,0x00,
        0x00,0x66,0x3C,0xFF,0x3C,0x66,0x00,0x00,
        0x00,0x0C,0x0C,0x3F,0x0C,0x0C,0x00,0x00,
        0x00,0x00,0x00,0x00,0x00,0x0C,0x0C,0x06,
        0x00,0x00,0x00,0x3F,0x00,0x00,0x00,0x00,
        0x00,0x00,0x00,0x00,0x00,0x0C,0x0C,0x00,
        0x60,0x30,0x18,0x0C,0x06,0x03,0x01,0x00,
        0x3E,0x63,0x73,0x7B,0x6F,0x67,0x3E,0x00,
        0x0C,0x0E,0x0C,0x0C,0x0C,0x0C,0x3F,0x00,
        0x1E,0x33,0x30,0x1C,0x06,0x33,0x3F,0x00,
        0x1E,0x33,0x30,0x1C,0x30,0x33,0x1E,0x00,
        0x38,0x3C,0x36,0x33,0x7F,0x30,0x78,0x00,
        0x3F,0x03,0x1F,0x30,0x30,0x33,0x1E,0x00,
        0x1C,0x06,0x03,0x1F,0x33,0x33,0x1E,0x00,
        0x3F,0x33,0x30,0x18,0x0C,0x0C,0x0C,0x00,
        0x1E,0x33,0x33,0x1E,0x33,0x33,0x1E,0x00,
        0x1E,0x33,0x33,0x3E,0x30,0x18,0x0E,0x00,
        0x00,0x0C,0x0C,0x00,0x00,0x0C,0x0C,0x00,
        0x00,0x0C,0x0C,0x00,0x00,0x0C,0x0C,0x06,
        0x18,0x0C,0x06,0x03,0x06,0x0C,0x18,0x00,
        0x00,0x00,0x3F,0x00,0x00,0x3F,0x00,0x00,
        0x06,0x0C,0x18,0x30,0x18,0x0C,0x06,0x00,
        0x1E,0x33,0x30,0x18,0x0C,0x00,0x0C,0x00,
        0x3E,0x63,0x7B,0x7B,0x7B,0x03,0x1E,0x00,
        0x0C,0x1E,0x33,0x33,0x3F,0x33,0x33,0x00,
        0x3F,0x66,0x66,0x3E,0x66,0x66,0x3F,0x00,
        0x3C,0x66,0x03,0x03,0x03,0x66,0x3C,0x00,
        0x1F,0x36,0x66,0x66,0x66,0x36,0x1F,0x00,
        0x7F,0x46,0x16,0x1E,0x16,0x46,0x7F,0x00,
        0x7F,0x46,0x16,0x1E,0x16,0x06,0x0F,0x00,
        0x3C,0x66,0x03,0x03,0x73,0x66,0x7C,0x00,
        0x33,0x33,0x33,0x3F,0x33,0x33,0x33,0x00,
        0x1E,0x0C,0x0C,0x0C,0x0C,0x0C,0x1E,0x00,
        0x78,0x30,0x30,0x30,0x33,0x33,0x1E,0x00,
        0x67,0x66,0x36,0x1E,0x36,0x66,0x67,0x00,
        0x0F,0x06,0x06,0x06,0x46,0x66,0x7F,0x00,
        0x63,0x77,0x7F,0x7F,0x6B,0x63,0x63,0x00,
        0x63,0x67,0x6F,0x7B,0x73,0x63,0x63,0x00,
        0x1C,0x36,0x63,0x63,0x63,0x36,0x1C,0x00,
        0x3F,0x66,0x66,0x3E,0x06,0x06,0x0F,0x00,
        0x1E,0x33,0x33,0x33,0x3B,0x1E,0x38,0x00,
        0x3F,0x66,0x66,0x3E,0x36,0x66,0x67,0x00,
        0x1E,0x33,0x07,0x0E,0x38,0x33,0x1E,0x00,
        0x3F,0x2D,0x0C,0x0C,0x0C,0x0C,0x1E,0x00,
        0x33,0x33,0x33,0x33,0x33,0x33,0x3F,0x00,
        0x33,0x33,0x33,0x33,0x33,0x1E,0x0C,0x00,
        0x63,0x63,0x63,0x6B,0x7F,0x77,0x63,0x00,
        0x63,0x63,0x36,0x1C,0x1C,0x36,0x63,0x00,
        0x33,0x33,0x33,0x1E,0x0C,0x0C,0x1E,0x00,
        0x7F,0x63,0x31,0x18,0x4C,0x66,0x7F,0x00,
        0x1E,0x06,0x06,0x06,0x06,0x06,0x1E,0x00,
        0x03,0x06,0x0C,0x18,0x30,0x60,0x40,0x00,
        0x1E,0x18,0x18,0x18,0x18,0x18,0x1E,0x00,
        0x08,0x1C,0x36,0x63,0x00,0x00,0x00,0x00,
        0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,
        0x0C,0x0C,0x18,0x00,0x00,0x00,0x00,0x00,
        0x00,0x00,0x1E,0x30,0x3E,0x33,0x6E,0x00,
        0x07,0x06,0x06,0x3E,0x66,0x66,0x3B,0x00,
        0x00,0x00,0x1E,0x33,0x03,0x33,0x1E,0x00,
        0x38,0x30,0x30,0x3e,0x33,0x33,0x6E,0x00,
        0x00,0x00,0x1E,0x33,0x3f,0x03,0x1E,0x00,
        0x1C,0x36,0x06,0x0f,0x06,0x06,0x0F,0x00,
        0x00,0x00,0x6E,0x33,0x33,0x3E,0x30,0x1F,
        0x07,0x06,0x36,0x6E,0x66,0x66,0x67,0x00,
        0x0C,0x00,0x0E,0x0C,0x0C,0x0C,0x1E,0x00,
        0x30,0x00,0x30,0x30,0x30,0x33,0x33,0x1E,
        0x07,0x06,0x66,0x36,0x1E,0x36,0x67,0x00,
        0x0E,0x0C,0x0C,0x0C,0x0C,0x0C,0x1E,0x00,
        0x00,0x00,0x33,0x7F,0x7F,0x6B,0x63,0x00,
        0x00,0x00,0x1F,0x33,0x33,0x33,0x33,0x00,
        0x00,0x00,0x1E,0x33,0x33,0x33,0x1E,0x00,
        0x00,0x00,0x3B,0x66,0x66,0x3E,0x06,0x0F,
        0x00,0x00,0x6E,0x33,0x33,0x3E,0x30,0x78,
        0x00,0x00,0x3B,0x6E,0x66,0x06,0x0F,0x00,
        0x00,0x00,0x3E,0x03,0x1E,0x30,0x1F,0x00,
        0x08,0x0C,0x3E,0x0C,0x0C,0x2C,0x18,0x00,
        0x00,0x00,0x33,0x33,0x33,0x33,0x6E,0x00,
        0x00,0x00,0x33,0x33,0x33,0x1E,0x0C,0x00,
        0x00,0x00,0x63,0x6B,0x7F,0x7F,0x36,0x00,
        0x00,0x00,0x63,0x36,0x1C,0x36,0x63,0x00,
        0x00,0x00,0x33,0x33,0x33,0x3E,0x30,0x1F,
        0x00,0x00,0x3F,0x19,0x0C,0x26,0x3F,0x00,
        0x38,0x0C,0x0C,0x07,0x0C,0x0C,0x38,0x00,
        0x18,0x18,0x18,0x00,0x18,0x18,0x18,0x00,
        0x07,0x0C,0x0C,0x38,0x0C,0x0C,0x07,0x00,
        0x6E,0x3B,0x00,0x00,0x00,0x00,0x00,0x00,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF
    };

    public MapView2D()
    {
        InitShader();
        InitTextShader();
        _fontTex = CreateFontTexture();
    }

    public void Render(
        List<(string Name, SceneObject Obj)> objects,
        Dictionary<string, bool> visibility,
        int vpX, int vpY, int vpW, int vpH)
    {
        _vpX = vpX; _vpY = vpY; _vpW = vpW; _vpH = vpH;

        // margins for axis labels
        const int marginL = 48, marginB = 24, marginT = 10, marginR = 10;
        int plotX = vpX + marginL, plotY = vpY + marginB;
        int plotW = vpW - marginL - marginR, plotH = vpH - marginB - marginT;

        // Collect bounds
        float worldMinX = float.MaxValue, worldMinY = float.MaxValue;
        float worldMaxX = float.MinValue, worldMaxY = float.MinValue;
        var boundsList = new List<(string name, float x0, float y0, float x1, float y1, int ci)>();

        int ci = 0;
        foreach (var (name, obj) in objects)
        {
            if (!visibility.TryGetValue(name, out bool vis) || !vis) { ci++; continue; }
            var (mnX, mnY, mxX, mxY) = obj.GetBounds2D();
            if (mnX > mxX) { ci++; continue; }
            boundsList.Add((name, mnX, mnY, mxX, mxY, ci % Colors.Length));
            if (mnX < worldMinX) worldMinX = mnX;
            if (mnY < worldMinY) worldMinY = mnY;
            if (mxX > worldMaxX) worldMaxX = mxX;
            if (mxY > worldMaxY) worldMaxY = mxY;
            ci++;
        }

        if (boundsList.Count == 0) { worldMinX = -1; worldMinY = -1; worldMaxX = 1; worldMaxY = 1; }

        float pad = Math.Max(worldMaxX - worldMinX, worldMaxY - worldMinY) * 0.1f + 0.01f;
        worldMinX -= pad; worldMinY -= pad;
        worldMaxX += pad; worldMaxY += pad;

        // Print bounding boxes to console
        Console.WriteLine("=== 2D Bounding Boxes ===");
        foreach (var (name, x0, y0, x1, y1, _) in boundsList)
            Console.WriteLine($"  {name,-20} X:[{x0:F3} → {x1:F3}]  Y:[{y0:F3} → {y1:F3}]  W:{x1-x0:F3}  H:{y1-y0:F3}");
        Console.WriteLine();

        // Full panel background
        GL.Viewport(vpX, vpY, vpW, vpH);
        GL.Scissor(vpX, vpY, vpW, vpH);
        GL.Enable(EnableCap.ScissorTest);
        GL.ClearColor(0.08f, 0.08f, 0.12f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Disable(EnableCap.ScissorTest);

        GL.Disable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // ── Plot area ──
        GL.Viewport(plotX, plotY, plotW, plotH);
        GL.Scissor(plotX, plotY, plotW, plotH);
        GL.Enable(EnableCap.ScissorTest);

        var worldProj = Matrix4.CreateOrthographicOffCenter(
            worldMinX, worldMaxX, worldMinY, worldMaxY, -1, 1);

        GL.UseProgram(_shader);
        GL.UniformMatrix4(GL.GetUniformLocation(_shader, "proj"), false, ref worldProj);

        float step = NiceStep(Math.Max(worldMaxX - worldMinX, worldMaxY - worldMinY) / 6f);
        DrawGrid(worldMinX, worldMinY, worldMaxX, worldMaxY, step);

        foreach (var (_, x0, y0, x1, y1, colorIdx) in boundsList)
        {
            var c = Colors[colorIdx];
            DrawFilledRect(x0, y0, x1, y1, new Vector4(c.X, c.Y, c.Z, 0.25f));
            DrawRectOutline(x0, y0, x1, y1, c);
        }

        GL.Disable(EnableCap.ScissorTest);

        // ── Tick labels — rendered in full panel space ──
        GL.Viewport(vpX, vpY, vpW, vpH);
        var screenProj = Matrix4.CreateOrthographicOffCenter(0, vpW, vpH, 0, -1, 1);

        // Helper: world X → screen pixel X within panel
        float WtoSX(float wx) => marginL + (wx - worldMinX) / (worldMaxX - worldMinX) * plotW;
        float WtoSY(float wy) => vpH - marginB - (wy - worldMinY) / (worldMaxY - worldMinY) * plotH;

        // X axis ticks
        for (float tx = (float)Math.Ceiling(worldMinX / step) * step; tx <= worldMaxX; tx += step)
        {
            float sx = WtoSX(tx);
            // tick line
            GL.UseProgram(_shader);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "proj"), false, ref screenProj);
            GL.Uniform4(GL.GetUniformLocation(_shader, "color"), new Vector4(0.7f, 0.7f, 0.7f, 1f));
            float[] tl = { sx, vpH - marginB, sx, vpH - marginB + 5 };
            UploadAndDraw(tl, PrimitiveType.Lines);
            // label
            string lbl = FormatTick(tx);
            float lx = sx - lbl.Length * 6f * 0.5f;
            float ly = vpH - marginB + 8;
            DrawString(lbl, lx, ly, screenProj, 1f, true);
        }

        // Y axis ticks
        for (float ty = (float)Math.Ceiling(worldMinY / step) * step; ty <= worldMaxY; ty += step)
        {
            float sy = WtoSY(ty);
            GL.UseProgram(_shader);
            GL.UniformMatrix4(GL.GetUniformLocation(_shader, "proj"), false, ref screenProj);
            GL.Uniform4(GL.GetUniformLocation(_shader, "color"), new Vector4(0.7f, 0.7f, 0.7f, 1f));
            float[] tl = { (float)marginL - 5, sy, (float)marginL, sy };
            UploadAndDraw(tl, PrimitiveType.Lines);
            string lbl = FormatTick(ty);
            float lx = marginL - lbl.Length * 6f - 7;
            float ly = sy - 4f;
            DrawString(lbl, lx, ly, screenProj, 1f, true);
        }

        // Border around plot
        GL.UseProgram(_shader);
        GL.UniformMatrix4(GL.GetUniformLocation(_shader, "proj"), false, ref screenProj);
        GL.Uniform4(GL.GetUniformLocation(_shader, "color"), new Vector4(0.4f, 0.4f, 0.5f, 1f));
        float[] border = {
            marginL,       marginT,
            marginL+plotW, marginT,
            marginL+plotW, marginT+plotH,
            marginL,       marginT+plotH,
            marginL,       marginT
        };
        UploadAndDraw(border, PrimitiveType.LineStrip);

        GL.Disable(EnableCap.Blend);
        GL.Enable(EnableCap.DepthTest);
    }

    // ── Drawing helpers ──────────────────────────────────────────────────────

    void DrawGrid(float x0, float y0, float x1, float y1, float step)
    {
        var lines = new List<float>();
        for (float x = (float)Math.Floor(x0 / step) * step; x <= x1; x += step)
        { lines.Add(x); lines.Add(y0); lines.Add(x); lines.Add(y1); }
        for (float y = (float)Math.Floor(y0 / step) * step; y <= y1; y += step)
        { lines.Add(x0); lines.Add(y); lines.Add(x1); lines.Add(y); }

        GL.Uniform4(GL.GetUniformLocation(_shader, "color"), new Vector4(0.20f, 0.20f, 0.26f, 1f));
        UploadAndDraw(lines.ToArray(), PrimitiveType.Lines);

        GL.Uniform4(GL.GetUniformLocation(_shader, "color"), new Vector4(0.45f, 0.45f, 0.55f, 1f));
        float[] axes = { x0, 0, x1, 0, 0, y0, 0, y1 };
        UploadAndDraw(axes, PrimitiveType.Lines);
    }

    void DrawFilledRect(float x0, float y0, float x1, float y1, Vector4 color)
    {
        float[] v = { x0, y0, x1, y0, x1, y1, x0, y0, x1, y1, x0, y1 };
        GL.Uniform4(GL.GetUniformLocation(_shader, "color"), color);
        UploadAndDraw(v, PrimitiveType.Triangles);
    }

    void DrawRectOutline(float x0, float y0, float x1, float y1, Vector4 color)
    {
        float[] v = { x0, y0, x1, y0, x1, y1, x0, y1, x0, y0 };
        GL.Uniform4(GL.GetUniformLocation(_shader, "color"), color);
        GL.LineWidth(2f);
        UploadAndDraw(v, PrimitiveType.LineStrip);
    }

void DrawString(string text, float x, float y, Matrix4 proj, float scale, bool flipY = false)
{
    GL.UseProgram(_textShader);
    GL.UniformMatrix4(GL.GetUniformLocation(_textShader, "proj"), false, ref proj);
    GL.Uniform1(GL.GetUniformLocation(_textShader, "fontTex"), 0);
    GL.ActiveTexture(TextureUnit.Texture0);
    GL.BindTexture(TextureTarget.Texture2D, _fontTex);

    float cw = 8f * scale, ch = 8f * scale;
    for (int i = 0; i < text.Length; i++)
    {
        int c = Math.Clamp(text[i] - 0x20, 0, 95);
        float u0 = (c + 1) / 96f, u1 = c / 96f;
        float v0 = flipY ? 0f : 1f;
        float v1 = flipY ? 1f : 0f;
        float gx = x + i * cw;
        float[] v = {
            gx,    y,    u0, v0,
            gx+cw, y,    u1, v0,
            gx+cw, y+ch, u1, v1,
            gx,    y,    u0, v0,
            gx+cw, y+ch, u1, v1,
            gx,    y+ch, u0, v1
        };
        GL.BindVertexArray(_tVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _tVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, v.Length * 4, v, BufferUsageHint.DynamicDraw);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
}
    void UploadAndDraw(float[] verts, PrimitiveType mode)
    {
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, verts.Length * 4, verts, BufferUsageHint.DynamicDraw);
        GL.DrawArrays(mode, 0, verts.Length / 2);
    }

    // ── Scale helpers ────────────────────────────────────────────────────────

    static float NiceStep(float raw)
    {
        float exp = (float)Math.Pow(10, Math.Floor(Math.Log10(raw)));
        float frac = raw / exp;
        float nice = frac < 1.5f ? 1f : frac < 3.5f ? 2f : frac < 7.5f ? 5f : 10f;
        return nice * exp;
    }

    static string FormatTick(float v)
    {
        if (Math.Abs(v) >= 1000 || (Math.Abs(v) < 0.01f && v != 0)) return $"{v:E1}";
        if (Math.Abs(v) < 1f) return $"{v:F2}";
        if (Math.Abs(v) < 10f) return $"{v:F1}";
        return $"{(int)Math.Round(v)}";
    }

    // ── GL init ──────────────────────────────────────────────────────────────

    int CreateFontTexture()
    {
        int numChars = 96;
        var pixels = new byte[numChars * 8 * 8 * 4];
        for (int ch = 0; ch < numChars; ch++)
            for (int row = 0; row < 8; row++)
            {
                byte bits = Font8x8[ch * 8 + row];
                for (int col = 0; col < 8; col++)
                {
                    bool set = (bits & (1 << (7 - col))) != 0;
                    int px = (ch * 8 + col) + row * (numChars * 8);
                    pixels[px * 4 + 0] = 255; pixels[px * 4 + 1] = 255;
                    pixels[px * 4 + 2] = 255; pixels[px * 4 + 3] = set ? (byte)255 : (byte)0;
                }
            }
        int id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, id);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                      numChars * 8, 8, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        return id;
    }

    void InitShader()
    {
        _vao = GL.GenVertexArray(); GL.BindVertexArray(_vao);
        _vbo = GL.GenBuffer();      GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8, 0);
        _shader = Compile(
            @"#version 330 core
              layout(location=0) in vec2 pos;
              uniform mat4 proj; 
              void main(){ gl_Position=proj*vec4(pos,0,1); }",
            @"#version 330 core
              uniform vec4 color; out vec4 FragColor;
              void main(){ FragColor=color; }");
    }

    void InitTextShader()
    {
        _tVao = GL.GenVertexArray(); GL.BindVertexArray(_tVao);
        _tVbo = GL.GenBuffer();      GL.BindBuffer(BufferTarget.ArrayBuffer, _tVbo);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 16, 0);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 16, 8);
        _textShader = Compile(
            @"#version 330 core
              layout(location=0) in vec2 pos;
              layout(location=1) in vec2 uv;
              uniform mat4 proj; out vec2 vUV;
              void main(){ vUV=uv; gl_Position=proj*vec4(pos,0,1); }",
            @"#version 330 core
              in vec2 vUV; uniform sampler2D fontTex; out vec4 FragColor;
              void main(){ FragColor=texture(fontTex,vUV); }");
    }

    static int Compile(string vert, string frag)
    {
        int v = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(v, vert); GL.CompileShader(v);
        int f = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(f, frag); GL.CompileShader(f);
        int p = GL.CreateProgram();
        GL.AttachShader(p, v); GL.AttachShader(p, f);
        GL.LinkProgram(p);
        GL.DeleteShader(v); GL.DeleteShader(f);
        return p;
    }
}