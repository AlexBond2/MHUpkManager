using DDSLib;
using MHUpkManager.TextureManager;
using SharpGL;
using SharpGL.Shaders;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;

namespace MHUpkManager
{
    public class GLLib
    {
        public struct GlyphInfo
        {
            public float U1, V1, U2, V2;
            public int Width, Height;
            public int OffsetX, OffsetY;
            public float XAdvance;
        }

        public class FontRenderer
        {
            const int FontSize = 11;
            const int AtlasWidth = 256;
            const int AtlasHeight = 256;

            private ShaderProgram FontShader;
            private Dictionary<char, GlyphInfo> Glyphs;
            private uint FontTexture;

            private uint vaoId;
            private uint vboVertices;
            private uint vboUVs;
            private int maxVertexCount;

            public void InitializeFont(OpenGL gl, ShaderProgram fontShader, int maxChars = 256)
            {
                FontShader = fontShader;
                Glyphs = [];

                using var bmp = new Bitmap(AtlasWidth, AtlasHeight, PixelFormat.Format32bppArgb);
                using var g = Graphics.FromImage(bmp);

                g.Clear(Color.Transparent);
                using var font = new Font("MS Sans Serif", FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                int x = 0, y = 0, maxHeight = 0;

                for (char c = (char)32; c < 127; c++)
                {
                    var size = g.MeasureString(c.ToString(), font, PointF.Empty, StringFormat.GenericDefault);
                    int w = (int)Math.Ceiling(size.Width);
                    int h = (int)Math.Ceiling(size.Height);

                    if (x + w > AtlasWidth)
                    {
                        x = 0;
                        y += maxHeight;
                        maxHeight = 0;
                    }

                    g.DrawString(c.ToString(), font, Brushes.White, x, y);

                    int offsetX = 0;
                    int offsetY = 0;
                    int xAdvance = w;

                    var format = new StringFormat(StringFormat.GenericDefault);
                    var region = new CharacterRange[] { new(0, 1) };
                    format.SetMeasurableCharacterRanges(region);
                    var regions = g.MeasureCharacterRanges(c.ToString(), font, new RectangleF(x, y, w, h), format);
                    if (regions.Length > 0)
                    {
                        var rect = regions[0].GetBounds(g);
                        offsetX = (int)rect.X - x;
                        offsetY = (int)rect.Y - y;
                        xAdvance = (int)rect.Width;
                    }

                    Glyphs[c] = new GlyphInfo
                    {
                        U1 = (float)x / AtlasWidth,
                        V1 = (float)y / AtlasHeight,
                        U2 = (float)(x + w) / AtlasWidth,
                        V2 = (float)(y + h) / AtlasHeight,
                        Width = w,
                        Height = h,
                        OffsetX = offsetX,
                        OffsetY = offsetY,
                        XAdvance = xAdvance
                    };

                    x += w;
                    if (h > maxHeight) maxHeight = h;
                }

                uint[] textureIds = new uint[1];
                gl.GenTextures(1, textureIds);
                FontTexture = textureIds[0];
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, FontTexture);

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA,
                    bmp.Width, bmp.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);
                
                bmp.UnlockBits(data);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

                InitializeBuffers(gl, maxChars);
            }

            public void InitializeBuffers(OpenGL gl, int maxChars)
            {
                uint[] vaos = new uint[1];
                gl.GenVertexArrays(1, vaos);
                vaoId = vaos[0];

                uint[] buffers = new uint[2];
                gl.GenBuffers(2, buffers);
                vboVertices = buffers[0];
                vboUVs = buffers[1];
                maxVertexCount = maxChars * 6;

                gl.BindVertexArray(vaoId);

                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vboVertices);
                gl.BufferData(OpenGL.GL_ARRAY_BUFFER, maxVertexCount * 3 * sizeof(float), IntPtr.Zero, OpenGL.GL_DYNAMIC_DRAW);                 
                gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
                gl.EnableVertexAttribArray(0);


                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vboUVs);
                gl.BufferData(OpenGL.GL_ARRAY_BUFFER, maxVertexCount * 2 * sizeof(float), IntPtr.Zero, OpenGL.GL_DYNAMIC_DRAW);                
                gl.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
                gl.EnableVertexAttribArray(1);

                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
                gl.BindVertexArray(0);
            }

            public void DeleteBuffers(OpenGL gl)
            {
                if (vboVertices != 0)
                {
                    gl.DeleteBuffers(1, [vboVertices]);
                    vboVertices = 0;
                }
                if (vboUVs != 0)
                {
                    gl.DeleteBuffers(1, [vboUVs]);
                    vboUVs = 0;
                }
            }

            public void DrawText(OpenGL gl, string text, Vector3 startPos, 
                Matrix4x4 matProjection, Matrix4x4 matView, Matrix4x4 matModel, Vector4 color, float scale = 1.0f)
            {
                if (string.IsNullOrEmpty(text))
                    return;

                gl.Disable(OpenGL.GL_DEPTH_TEST);
                gl.Enable(OpenGL.GL_BLEND);
                gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

                FontShader.Bind(gl);

                int[] data = new int[4];
                gl.GetInteger(OpenGL.GL_VIEWPORT, data);
                float viewportWidth = data[2];
                float viewportHeight = data[3];
                FontShader.SetUniform2(gl, "uViewportSize", viewportWidth, viewportHeight);

                var matOrtho = Matrix4x4.CreateOrthographicOffCenter(
                    0.0f, viewportWidth,
                    viewportHeight, 0.0f,
                    -1.0f, 1.0f
                );
                FontShader.SetUniformMatrix4(gl, "uOrtho", matOrtho.ToArray());

                FontShader.SetUniformMatrix4(gl, "uProjection", matProjection.ToArray());
                FontShader.SetUniformMatrix4(gl, "uView", matView.ToArray());
                FontShader.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

                FontShader.SetUniform3(gl, "uStartPos", startPos.X, startPos.Y, startPos.Z);
                FontShader.SetUniform1(gl, "uScale", scale);
                FontShader.SetUniform4(gl, "uTextColor", color.X, color.Y, color.Z, color.W);

                FontShader.SetUniform1(gl, "uFontTexture", 0);

                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, FontTexture);

                float cursorX = -FontSize * 0.4f;
                float cursorY = -FontSize * 0.8f;
                float z = 0f;

                List<float> vertices = [];
                List<float> uvs = [];

                foreach (char c in text)
                {
                    if (!Glyphs.TryGetValue(c, out var g))
                        continue;

                    float x0 = cursorX + g.OffsetX;
                    float y0 = cursorY + g.OffsetY;
                    float x1 = x0 + g.Width;
                    float y1 = y0 + g.Height;

                    vertices.AddRange(
                    [
                        x0, y0, z,
                        x0, y1, z,
                        x1, y1, z,

                        x0, y0, z,
                        x1, y1, z,
                        x1, y0, z,
                    ]);

                    uvs.AddRange(
                    [
                        g.U1, g.V1,
                        g.U1, g.V2,
                        g.U2, g.V2,

                        g.U1, g.V1,
                        g.U2, g.V2,
                        g.U2, g.V1,
                    ]);

                    cursorX += g.XAdvance;
                }

                int vertexCount = vertices.Count / 3;

                BindVertexSubBuffer(gl, vboVertices, sizeof(float), [.. vertices]);
                BindVertexSubBuffer(gl, vboUVs, sizeof(float), [.. uvs]);
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);

                gl.BindVertexArray(vaoId);
                gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, vertexCount);
                gl.BindVertexArray(0);

                FontShader.Unbind(gl);

                gl.Disable(OpenGL.GL_BLEND);
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }
        }

        public class GridRenderer
        {
            public int GridMax = 100;

            private uint gridVAO = 0;
            private uint gridVBOVertices = 0;
            private uint gridVBOColors = 0;
            private int gridVertexCount = 0;
            private float lastStep = -1f;
            private ShaderProgram shader;
            private float visibleGridSize;

            public void InitializeBuffers(OpenGL gl, ShaderProgram sh)
            {
                shader = sh;
                visibleGridSize = GridMax * 5.0f;

                uint[] vaos = new uint[1];
                gl.GenVertexArrays(1, vaos);
                gridVAO = vaos[0];

                uint[] buffers = new uint[2];
                gl.GenBuffers(2, buffers);
                gridVBOVertices = buffers[0];
                gridVBOColors = buffers[1];

                float minStep = 1.0f;
                int maxLineCount = (int)MathF.Ceiling(visibleGridSize / minStep);
                int maxTotalLines = (maxLineCount * 2 + 1) * 2;
                int maxVertexCount = maxTotalLines * 2;

                int maxVerticesArraySize = maxVertexCount * 3; // x, y, z
                int maxColorsArraySize = maxVertexCount * 4;   // r, g, b, a

                gl.BindVertexArray(gridVAO);

                BindVertexBuffer(gl, gridVBOVertices, sizeof(float), OpenGL.GL_DYNAMIC_DRAW,
                                new float[maxVerticesArraySize]);
                gl.EnableVertexAttribArray(0);
                gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);

                BindVertexBuffer(gl, gridVBOColors, sizeof(float), OpenGL.GL_DYNAMIC_DRAW,
                                new float[maxColorsArraySize]);
                gl.EnableVertexAttribArray(1);
                gl.VertexAttribPointer(1, 4, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);

                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
                gl.BindVertexArray(0);               
            }

            public void DrawGrid(OpenGL gl, float zoomLevel, Matrix4x4 matProjection, Matrix4x4 matView, Matrix4x4 matModel)
            {
                float step;
                if (zoomLevel <= 10) step = 1.0f;
                else if (zoomLevel <= 400) step = 10.0f;
                else step = 100.0f;

                if (step != lastStep)
                {
                    UpdateGridBuffers(gl, step);
                    lastStep = step;
                }

                shader.Bind(gl);

                shader.SetUniformMatrix4(gl, "uProjection", matProjection.ToArray());
                shader.SetUniformMatrix4(gl, "uView", matView.ToArray());
                shader.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

                gl.BindVertexArray(gridVAO);
                gl.DrawArrays(OpenGL.GL_LINES, 0, gridVertexCount);
                gl.BindVertexArray(0);

                shader.Unbind(gl);
            }

            private void UpdateGridBuffers(OpenGL gl, float step)
            {
                int lineCount = (int)MathF.Ceiling(visibleGridSize / step);
                int totalLines = (lineCount * 2 + 1) * 2;

                float[] vertices = new float[totalLines * 3 * 2];
                float[] colors = new float[totalLines * 4 * 2];

                int vIndex = 0;
                int cIndex = 0;

                for (int i = -lineCount; i <= lineCount; i++)
                {
                    float pos = i * step;

                    float alpha;
                    if (MathF.Abs(pos % 1000f) < 0.01f)
                        alpha = 0.5f;
                    else if (MathF.Abs(pos % 100f) < 0.01f)
                        alpha = 0.35f;
                    else if (MathF.Abs(pos % 10f) < 0.01f)
                        alpha = 0.2f;
                    else
                        alpha = 0.1f;

                    float r = alpha, g = alpha, b = alpha;
                    float zOffset = alpha / 10f;

                    vertices[vIndex++] = -visibleGridSize;
                    vertices[vIndex++] = pos;
                    vertices[vIndex++] = zOffset;

                    vertices[vIndex++] = visibleGridSize;
                    vertices[vIndex++] = pos;
                    vertices[vIndex++] = zOffset;

                    for (int j = 0; j < 2; j++)
                    {
                        colors[cIndex++] = r;
                        colors[cIndex++] = g;
                        colors[cIndex++] = b;
                        colors[cIndex++] = 1f;
                    }

                    vertices[vIndex++] = pos;
                    vertices[vIndex++] = -visibleGridSize;
                    vertices[vIndex++] = zOffset;

                    vertices[vIndex++] = pos;
                    vertices[vIndex++] = visibleGridSize;
                    vertices[vIndex++] = zOffset;

                    for (int j = 0; j < 2; j++)
                    {
                        colors[cIndex++] = r;
                        colors[cIndex++] = g;
                        colors[cIndex++] = b;
                        colors[cIndex++] = 1f;
                    }
                }

                gridVertexCount = totalLines * 2;

                BindVertexSubBuffer(gl, gridVBOVertices, sizeof(float), vertices);
                BindVertexSubBuffer(gl, gridVBOColors, sizeof(float), colors);
            }
        }

        public static void BindVertexBuffer(OpenGL gl, uint vbo, int elementSize, uint draw, float[] vertices)
        {
            var handle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            try
            {
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo);
                gl.BufferData(OpenGL.GL_ARRAY_BUFFER, vertices.Length * elementSize,
                    handle.AddrOfPinnedObject(), draw);
            }
            finally
            {
                handle.Free();
            }
        }

        public static void BindVertexSubBuffer(OpenGL gl, uint vbo, int elementSize, float[] vertices)
        {
            var handle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            try
            {
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vbo);
                gl.BufferSubData(OpenGL.GL_ARRAY_BUFFER, 0, vertices.Length * elementSize,
                    handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        public class AxisRenderer
        {
            private ShaderProgram shader;
            private FontRenderer font;

            private uint axesVao = 0;
            private uint axesVboVertices;
            private uint axesVboColors;

            public void InitializeBuffers(OpenGL gl, FontRenderer fr, ShaderProgram sh)
            {
                shader = sh;
                font = fr;

                float[] vertices =
                [
                    0, 0, 0,   1, 0, 0,  // X axis
                    0, 0, 0,   0, 1, 0,  // Y axis
                    0, 0, 0,   0, 0, 1,  // Z axis
                ];

                float[] colors =
                [
                    1, 0, 0, 1, 1, 0, 0, 1, // Red X start/end
                    0, 1, 0, 1, 0, 1, 0, 1, // Green Y start/end
                    0, 0, 1, 1, 0, 0, 1, 1, // Blue Z start/end
                ];

                uint[] vaos = new uint[1];
                uint[] vbos = new uint[2];
                gl.GenVertexArrays(1, vaos);
                gl.GenBuffers(2, vbos);

                axesVao = vaos[0];
                axesVboVertices = vbos[0];
                axesVboColors = vbos[1];

                gl.BindVertexArray(axesVao);

                BindVertexBuffer(gl, axesVboVertices, sizeof(float), OpenGL.GL_STATIC_DRAW, vertices);
                gl.EnableVertexAttribArray(0);
                gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);

                BindVertexBuffer(gl, axesVboColors, sizeof(float), OpenGL.GL_STATIC_DRAW, colors);
                gl.EnableVertexAttribArray(1);
                gl.VertexAttribPointer(1, 4, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);

                gl.BindVertexArray(0);
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
            }

            public void DrawAxes(OpenGL gl, Matrix4x4 matProjection, Matrix4x4 matView, Matrix4x4 matModel)
            {
                shader.Bind(gl);
                shader.SetUniformMatrix4(gl, "uProjection", matProjection.ToArray());
                shader.SetUniformMatrix4(gl, "uView", matView.ToArray());
                shader.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

                gl.BindVertexArray(axesVao);
                gl.DrawArrays(OpenGL.GL_LINES, 0, 6);
                gl.BindVertexArray(0);

                shader.Unbind(gl);

                // Draw labels
                font.DrawText(gl, "x", new Vector3(1.2f, 0f, 0f), matProjection, matView, matModel, new(1f, 0f, 0f, 1f));
                font.DrawText(gl, "y", new Vector3(0f, 1.2f, 0f), matProjection, matView, matModel, new(0f, 1f, 0f, 1f));
                font.DrawText(gl, "z", new Vector3(0f, 0f, 1.2f), matProjection, matView, matModel, new(0f, 0f, 1f, 1f));
            }
        }

        public uint LoadTextureFromPng(string filePath, OpenGL gl)
        {
            Bitmap bitmap = new Bitmap(filePath);

            uint[] textureIds = new uint[1];
            gl.GenTextures(1, textureIds);
            uint textureId = textureIds[0];

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0,
                OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, data.Scan0);

            bitmap.UnlockBits(data);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            return textureId;
        }

        public static uint BindGLTexture(OpenGL gl, FObject textureObject, out int mipIndex, out UTexture2D texture, out byte[] outData)
        {
            texture = textureObject?.LoadObject<UTexture2D>();
            outData = [];
            mipIndex = -1;
            if (texture == null)
                return 0;

            uint[] textureIds = new uint[1];
            gl.GenTextures(1, textureIds);
            uint textureId = textureIds[0];

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
            SetDefaultTextureParameters(gl);

            mipIndex = texture.FirstResourceMemMip;
            FTexture2DMipMap mip;
            var textureCache = TextureFileCache.Instance;

            var textureEntry = TextureManifest.Instance.GetTextureEntryFromObject(textureObject);
            if (textureEntry != null)
            {
                textureCache.SetEntry(textureEntry, texture);
                textureCache.LoadTextureCache();
                mipIndex = (int)textureEntry.Data.Maps[0].Index;
                mip = textureCache.Texture2D.Mips[0];
            }
            else 
            {
                mip = texture.Mips[mipIndex];
            }

            var data = mip.Data;
            int width = mip.SizeX;
            int height = mip.SizeY;

            if (texture.Format != EPixelFormat.PF_A8R8G8B8)
            {
                DdsFile ddsFile = new();
                Stream stream;
                if (textureEntry != null)
                    stream = textureCache.Texture2D.GetObjectStream(0);
                else
                    stream = texture.GetObjectStream(mipIndex);

                ddsFile.Load(stream);
                data = ddsFile.BitmapData;
                UploadUncompressedTexture(gl, OpenGL.GL_RGBA, width, height, data);                
            }
            else if (texture.Format == EPixelFormat.PF_A8R8G8B8)
            {
                UploadUncompressedTexture(gl, OpenGL.GL_BGRA, width, height, data);
            }

            outData = data;
            return textureId;
        }

        public static void SetDefaultTextureParameters(OpenGL gl)
        {
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
        }

        public static void UploadUncompressedTexture(OpenGL gl, uint format, int width, int height, byte[] data)
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, width, height, 0, format, OpenGL.GL_UNSIGNED_BYTE, ptr);
            }
            finally
            {
                handle.Free();
            }
        }

        public static void UploadFallbackTexture(OpenGL gl)
        {
            byte[] whitePixel = { 255, 255, 255, 255 };
            GCHandle handle = GCHandle.Alloc(whitePixel, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, 1, 1, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, ptr);
            }
            finally
            {
                handle.Free();
            }
        }
    }

    public static class MatrixExtensions
    {
        public static float[] ToArray(this Matrix4x4 matrix)
        {
            return
            [
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            ];
        }
    }
}
