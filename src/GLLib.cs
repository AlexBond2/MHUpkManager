using DDSLib;
using SharpGL;

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using UpkManager.Models.UpkFile.Engine.Texture;

namespace MHUpkManager
{
    public class GLLib
    {
        public const uint OBJ_AXES = 1;
        public const uint OBJ_TARGET = 2;
        public const uint OBJ_GRID = 3;
        public const uint FONT_GL = 2000;

        public static void InitializeFont(OpenGL gl)
        {
            IntPtr hdc = gl.RenderContextProvider.DeviceContextHandle;

            using var font = new Font("MS Sans Serif", 8, FontStyle.Regular);
            IntPtr hFont = font.ToHfont();
            Win32.SelectObject(hdc, hFont);
            Win32.wglUseFontBitmaps(hdc, 0, 255, FONT_GL);
        }

        public static void DrawGrid(OpenGL gl, float zoomLevel, int gridMax = 100)
        {
            gl.Disable(OpenGL.GL_LIGHTING);

            float visibleGridSize = gridMax * 5.0f;

            float step;
            if (zoomLevel <= 10) step = 1.0f;
            else if (zoomLevel <= 400) step = 10.0f;
            else step = 100.0f;

            int lineCount = (int)MathF.Ceiling(visibleGridSize / step);

            gl.Begin(OpenGL.GL_LINES);

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

                gl.Color(alpha, alpha, alpha);

                float zOffset = alpha / 10;

                gl.Vertex(-visibleGridSize, pos, zOffset);
                gl.Vertex(visibleGridSize, pos, zOffset);

                gl.Vertex(pos, -visibleGridSize, zOffset);
                gl.Vertex(pos, visibleGridSize, zOffset);
            }

            gl.End();

            gl.Enable(OpenGL.GL_LIGHTING);
        }

        public static void DrawText(OpenGL gl, string text)
        {
            gl.ListBase(FONT_GL);
            byte[] array = Encoding.ASCII.GetBytes(text);
            gl.CallLists(array.Length, array);
        }

        public static void DrawAxes(OpenGL gl)
        {
            gl.Disable(OpenGL.GL_LIGHTING);

            gl.Color(1f, 0f, 0f); // Red X
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(0, 0, 0);
            gl.Vertex(1, 0, 0);
            gl.End();
            gl.RasterPos(1.2f, 0, 0);
            DrawText(gl, "x");

            gl.Color(0f, 1f, 0f); // Green Y
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, 1, 0);
            gl.End();
            gl.RasterPos(0, 1.2f, 0);
            DrawText(gl, "y");

            gl.Color(0f, 0f, 1f); // Blue Z
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(0, 0, 0);
            gl.Vertex(0, 0, 1);
            gl.End();
            gl.RasterPos(0, 0, 1.2f);
            DrawText(gl, "z");

            gl.Enable(OpenGL.GL_LIGHTING);
        }

        public static void DrawTarget(OpenGL gl, float size)
        {
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(-size, 0, 0);
            gl.Vertex(size, 0, 0);
            gl.Vertex(0, -size, 0);
            gl.Vertex(0, size, 0);
            gl.Vertex(0, 0, -size);
            gl.Vertex(0, 0, size);
            gl.End();
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

        public static uint BindGLTexture(OpenGL gl, UTexture2D texture, out byte[] outData)
        {
            outData = [];
            if (texture == null)
                return 0;

            uint[] textureIds = new uint[1];
            gl.GenTextures(1, textureIds);
            uint textureId = textureIds[0];

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);
            SetDefaultTextureParameters(gl);

            var mip = texture.Mips[texture.FirstResourceMemMip];
            var data = mip.Data;
            int width = mip.SizeX;
            int height = mip.SizeY;

            if (texture.Format != EPixelFormat.PF_A8R8G8B8)
            {
                DdsFile ddsFile = new();
                var stream = texture.GetObjectStream(texture.FirstResourceMemMip);
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
}
