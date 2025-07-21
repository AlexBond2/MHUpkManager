using SharpGL;
using System.Text;
using UpkManager.Models.UpkFile.Engine;

namespace MHUpkManager
{
    public partial class ModelViewForm : Form
    {
        private string title;

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
            ResetTransView();
        }

        private void sceneControl_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = sceneControl.OpenGL;

            GenerateDisplayLists(gl);
            InitializeFont(gl);
        }

        private void InitializeFont(OpenGL gl)
        {
            IntPtr hdc = gl.RenderContextProvider.DeviceContextHandle;

            using var font = new Font("MS Sans Serif", 8, FontStyle.Regular);
            IntPtr hFont = font.ToHfont();
            Win32.SelectObject(hdc, hFont);
            Win32.wglUseFontBitmaps(hdc, 0, 255, FONT_GL);
        }

        public void SetMeshObject(USkeletalMesh mesh)
        {
            // TODO add mesh to scene
        }

        public void SetTitle(string name)
        {
            title = name;
            Text = $"Hex Viewer - [{title}]";
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO save mesh
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
            int dx = lastMousePos.X - cur.X;
            int dy = lastMousePos.Y - cur.Y;

            if (isPanning)
            {
                float sinY = (float)Math.Sin((transView.yrot - 90) * Math.PI / 180.0);
                float cosY = (float)Math.Cos((transView.yrot - 90) * Math.PI / 180.0);
                float sinX = (float)Math.Sin(transView.xrot * Math.PI / 180.0);
                float cosX = (float)Math.Cos(transView.xrot * Math.PI / 180.0);

                float zoom = transView.zoom;
                float perRad = transView.Per * (float)(Math.PI / 180.0);

                float stepX = dx / (float)sceneControl.Width * zoom * perRad;
                float stepY = dy / (float)sceneControl.Height * zoom * perRad;

                transView.xpos -= stepX * sinY;
                transView.zpos += stepX * cosY;

                transView.ypos += stepY * cosX;
                transView.zpos += stepY * sinY * sinX;
                transView.xpos += stepY * cosY * sinX;
            }
            else if (isRotating)
            {
                transView.xrot += dy / 5.0f;
                transView.yrot -= dx / 5.0f;
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

        private void sceneControl_OpenGLDraw(object sender, RenderEventArgs args)
        {
            var gl = sceneControl.OpenGL;

            int width = sceneControl.Width;
            int height = sceneControl.Height;
            float aspect = width / (float)height;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            // --- perspective ---
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, width, height);

            gl.Perspective(transView.Per, aspect, transView.zoom / 50.0f, MaxDepth);

            if (transView.yrot > 360.0f) transView.yrot -= 360.0f;
            if (transView.yrot < -360.0f) transView.yrot += 360.0f;

            // camera
            gl.LookAt(0, 0, -transView.zoom, 0, 0, 0, 0, 1, 0);
            gl.Rotate(transView.xrot, 1, 0, 0);
            gl.Rotate(transView.yrot, 0, 1, 0);
            gl.Translate(transView.xpos, transView.ypos, transView.zpos);

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
            gl.LookAt(0, 0, -10, 0, 0, 0, 0, 1, 0);
            gl.Rotate(transView.xrot, 1, 0, 0);
            gl.Rotate(transView.yrot, 0, 1, 0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            gl.CallList(OBJ_AXES);

            gl.Flush();
        }

        private void DrawModel(OpenGL gl)
        {
            // TODO
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
                gl.Vertex(-gridMax * size, 0, i * size);
                gl.Vertex(gridMax * size, 0, i * size);
                gl.Vertex(i * size, 0, -gridMax * size);
                gl.Vertex(i * size, 0, gridMax * size);
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
                xpos = 0,
                ypos = 0,
                zpos = 0,
                xrot = -20.0f,
                yrot = 136.0f,
                zrot = 0,
                zoom = 50.0f,
                Per = 35.0f
            };
        }

        public struct TransView
        {
            public float xpos, ypos, zpos;
            public float xrot, yrot, zrot;
            public float zoom, Per;
        }
    }
}
