using SharpGL;
using System.Drawing.Imaging;
using System.Numerics;
using System.Text;

using UpkManager.Models.UpkFile.Engine;
using UpkManager.Models.UpkFile.Types;

namespace MHUpkManager
{
    public partial class ModelViewForm : Form
    {
        private string title;
        private USkeletalMesh mesh;
        private SkeletalMeshData model;

        private const uint OBJ_AXES = 1;
        private const uint OBJ_TARGET = 2;
        private const uint OBJ_GRID = 3;
        private const uint FONT_GL = 2000;

        private const float MaxDepth = 100000.0f;

        private Point lastMousePos; 
        private bool isPanning = false;
        private bool isRotating = false;
        private TransView transView;

        public ModelViewForm()
        {
            InitializeComponent();
            InitializeScene();
        }

        private void InitializeScene()
        {
            sceneControl.MouseWheel += sceneControl_MouseWheel;

            transView = new TransView
            {
                xpos = 0,
                ypos = 0,
                zpos = 0,
                xrot = 20.0f,
                yrot = 0,
                zrot = 225.0f,
                zoom = 60f,
                Per = 35.0f
            };
        }

        private void sceneControl_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = sceneControl.OpenGL;
            
            InitializeFont(gl);
            GenerateDisplayLists(gl);
            SetupLighting(gl);
        }

        private void SetupLighting(OpenGL gl)
        {
            float[] ambient = { 0.1f, 0.1f, 0.1f, 1.0f };
            float[] lightPos = { 400.0f, 400.0f, 400f, 1.0f }; 
            float[] light1Pos = { -200.0f, -200.0f, 200f, 1.0f };

            float[] matDiffuse1 = { 0.9f, 0.9f, 0.9f, 1.0f };
            float[] matDiffuse2 = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] matSpecular = { 1.0f, 1.0f, 1.0f, 1.0f };
            float shininess = 50.0f;

            gl.Enable(OpenGL.GL_LIGHTING);

            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightPos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, matDiffuse1);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, matSpecular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SHININESS, shininess);

            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1Pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, matDiffuse2);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, matSpecular);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SHININESS, shininess);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, 1);
        }

        private void InitializeFont(OpenGL gl)
        {
            IntPtr hdc = gl.RenderContextProvider.DeviceContextHandle;

            using var font = new Font("MS Sans Serif", 8, FontStyle.Regular);
            IntPtr hFont = font.ToHfont();
            Win32.SelectObject(hdc, hFont);
            Win32.wglUseFontBitmaps(hdc, 0, 255, FONT_GL);
        }

        public void SetMeshObject(USkeletalMesh obj)
        {
            mesh = obj;

            if (mesh == null)
            {
                MessageBox.Show("No mesh object set.");
                return;
            }

            model = new SkeletalMeshData(mesh);
            ResetTransView();
        }

        public void SetTitle(string name)
        {
            title = name;
            Text = $"Model Viewer - [{title}]";
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Wavefront OBJ (*.obj)|*.obj|glTF 2.0 Binary (*.glb)|*.glb|glTF 2.0 (*.gltf)|*.gltf|Collada DAE (*.dae)|*.dae",
                Title = "Save Model As",
                FileName = title
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(sfd.FileName).ToLower();
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    if (ext == ".gltf")
                        ModelFormats.ExportToGLTF(sfd.FileName, model, true);
                    else if (ext == ".glb")
                        ModelFormats.ExportToGLTF(sfd.FileName, model, false);
                    else if (ext == ".obj")
                        ModelFormats.ExportToOBJ(sfd.FileName, model);
                    else if (ext == ".dae")
                        ModelFormats.ExportToDAE(sfd.FileName, model);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void sceneControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                isPanning = true;
            else if (e.Button == MouseButtons.Right)
                isRotating = true;

            lastMousePos = e.Location;
        }

        private void sceneControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point cur = e.Location;
            int dx = cur.X - lastMousePos.X;
            int dy = cur.Y - lastMousePos.Y;

            if (isPanning)
            {
                float sinY = (float)Math.Sin((transView.zrot - 90) * Math.PI / 180.0);
                float cosY = (float)Math.Cos((transView.zrot - 90) * Math.PI / 180.0);
                float sinX = (float)Math.Sin(transView.xrot * Math.PI / 180.0);
                float cosX = (float)Math.Cos(transView.xrot * Math.PI / 180.0);

                float zoom = transView.zoom;
                float perRad = transView.Per * (float)(Math.PI / 180.0);

                float stepX = dx / (float)sceneControl.Width * zoom * perRad;
                float stepY = dy / (float)sceneControl.Height * zoom * perRad;

                transView.xpos -= stepX * sinY;
                transView.ypos -= stepX * cosY;

                transView.xpos -= stepY * cosY * sinX;
                transView.ypos += stepY * sinY * sinX;
                transView.zpos += stepY * cosX;
            }
            else if (isRotating)
            {
                transView.xrot += dy / 5.0f;
                transView.zrot += dx / 5.0f;
            }

            lastMousePos = cur;

            sceneControl.DoRender();
        }

        private void sceneControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
                isPanning = false;
            else if (e.Button == MouseButtons.Right)
                isRotating = false;
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

        private void sceneControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            var gl = sceneControl.OpenGL;

            int width = sceneControl.Width;
            int height = sceneControl.Height;
            float aspect = width / (float)height;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            // face culling
            gl.Enable(OpenGL.GL_CULL_FACE);

            // --- perspective ---
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, width, height);

            gl.Perspective(transView.Per, aspect, transView.zoom / 50.0f, MaxDepth);

            if (transView.zrot > 360.0f) transView.zrot -= 360.0f;
            if (transView.zrot < -360.0f) transView.zrot += 360.0f;

            // camera
            gl.LookAt(0, -transView.zoom, 0, 0, 0, 0, 0, 0, 1);
            gl.Rotate(transView.xrot, 1, 0, 0);
            gl.Rotate(transView.zrot, 0, 0, 1);
            gl.Translate(transView.xpos, transView.ypos, -transView.zpos);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // grid
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.CallList(OBJ_GRID);
            gl.Enable(OpenGL.GL_LIGHTING);

            gl.PushMatrix();
            DrawModel(gl);
            gl.PopMatrix();

            float fzoom = transView.zoom / 15.0f;
            gl.Scale(fzoom, fzoom, fzoom);

            // --- axis ---
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, width / 7, height / 7);

            gl.Perspective(20.0f, aspect, 5.0f, 20.0f);
            gl.LookAt(0, -10, 0, 0, 0, 0, 0, 0, 1);
            gl.Rotate(transView.xrot, 1, 0, 0);
            gl.Rotate(transView.zrot, 0, 0, 1);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            gl.CallList(OBJ_AXES);

            gl.Flush();
        }

        private void DrawModel(OpenGL gl)
        {
            if (mesh == null) return;
            
            // gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId);  
            gl.Begin(OpenGL.GL_TRIANGLES);

            foreach (var index in model.Indices)
            {
                var vertex = model.Vertices[index];

                gl.Normal(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                gl.TexCoord(vertex.TexCoord.X, vertex.TexCoord.Y);
                gl.Vertex(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
            }

            gl.End();
        }

        private void sceneControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                transView.zoom -= transView.zoom * 0.1f;
            else
                transView.zoom += transView.zoom * 0.1f;

            if (transView.zoom < 1.0f)
                transView.zoom = 1.0f;
            if (transView.zoom > 1000.0f)
                transView.zoom = 1000.0f;

            sceneControl.DoRender();
        }

        private void GenerateDisplayLists(OpenGL gl)
        {
            gl.NewList(OBJ_AXES, OpenGL.GL_COMPILE);
            DrawAxes(gl);
            gl.EndList();

            gl.NewList(OBJ_GRID, OpenGL.GL_COMPILE);
            DrawGrid(gl, 16);
            gl.EndList();
        }

        private void DrawGrid(OpenGL gl, int gridMax)
        {
            int size = 5;
            gl.Begin(OpenGL.GL_LINES);
            for (int i = -gridMax; i <= gridMax; i++)
            {
                gl.Vertex(-gridMax * size, i * size, 0);
                gl.Vertex(gridMax * size, i * size, 0);
                gl.Vertex(i * size, -gridMax * size, 0);
                gl.Vertex(i * size, gridMax * size, 0);
            }
            gl.End();
        }

        private void DrawText(OpenGL gl, string text)
        {
            gl.ListBase(FONT_GL);
            byte[] array = Encoding.ASCII.GetBytes(text);
            gl.CallLists(array.Length, array);
        }

        private void DrawAxes(OpenGL gl)
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

        private void DrawTarget(OpenGL gl, float size)
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

        private void sceneControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad7:
                    transView.xrot += 10.0f;
                    break;
                case Keys.NumPad1:
                    transView.xrot -= 10.0f;
                    break;
                case Keys.NumPad8:
                    transView.zpos += 10.0f;
                    break;
                case Keys.NumPad2:
                    transView.zpos -= 10.0f;
                    break;
                case Keys.NumPad4:
                    transView.xpos += 10.0f;
                    break;
                case Keys.NumPad6:
                    transView.xpos -= 10.0f;
                    break;
                case Keys.NumPad9:
                    transView.Per += 10.0f;
                    break;
                case Keys.NumPad3:
                    transView.Per -= 10.0f;
                    break;
                case Keys.NumPad5:
                    ResetTransView();
                    break;
            }

            sceneControl.DoRender();
        }

        private void ResetTransView()
        {
            transView = new TransView
            {
                xpos = model.Center.X,
                ypos = model.Center.Y,
                zpos = model.Center.Z,
                xrot = 20.0f,
                yrot = 0,
                zrot = 225.0f,
                zoom = model.Radius * 2f,
                Per = 35.0f
            };
        }

        public struct SkeletalMeshData
        {
            public USkeletalMesh Mesh;
            public int[] Indices;
            public Vector3 Center;
            public float Radius;
            public GLVertex[] Vertices;

            public SkeletalMeshData(USkeletalMesh mesh)
            {
                Mesh = mesh;

                var lod = mesh.LODModels[0];

                Vertices = [.. lod.VertexBufferGPUSkin.GetGLVertexData()];
                Indices = ConvertIndices(lod.MultiSizeIndexContainer.IndexBuffer);

                Center = CalculateCenter(Vertices);
                Radius = mesh.Bounds.SphereRadius;
            }

            public int[] ConvertIndices(UArray<uint> indices)
            {
                if (indices.Count % 3 != 0) return [];

                int[] flipped = new int[indices.Count];

                for (int i = 0; i < indices.Count; i += 3)
                {
                    flipped[i] = (int)indices[i + 2];
                    flipped[i + 1] = (int)indices[i + 1];
                    flipped[i + 2] = (int)indices[i];
                }

                return flipped;

            }

            private static Vector3 CalculateCenter(GLVertex[] vertices)
            {
                if (vertices == null || vertices.Length == 0)
                    return new Vector3(0, 0, 0);

                float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
                float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

                foreach (var v in vertices)
                {
                    var p = v.Position;
                    if (p.X < minX) minX = p.X;
                    if (p.Y < minY) minY = p.Y;
                    if (p.Z < minZ) minZ = p.Z;

                    if (p.X > maxX) maxX = p.X;
                    if (p.Y > maxY) maxY = p.Y;
                    if (p.Z > maxZ) maxZ = p.Z;
                }

                return new Vector3(
                    (minX + maxX) * 0.5f,
                    (minY + maxY) * 0.5f,
                    (minZ + maxZ) * 0.5f
                );
            }
        }

        public struct TransView
        {
            public float xpos, ypos, zpos;
            public float xrot, yrot, zrot;
            public float zoom, Per;
        }
    }
}
