using MHUpkManager.Model;
using SharpGL;
using SharpGL.Shaders;
using System.Numerics;
using UpkManager.Models.UpkFile.Classes;
using static MHUpkManager.Model.GLLib;

namespace MHUpkManager
{
    public partial class ModelViewForm : Form
    {
        private string title;
        private UObject mesh;
        private ModelMesh model;
        private ModelShaders modelShaders;
        private FontRenderer fontRenderer;
        private GridRenderer gridRenderer;
        private AxisRenderer axisRenderer;

        private const float MaxDepth = 100000.0f;

        private Point lastMousePos;
        private bool isPanning = false;
        private bool isRotating = false;
        private TransView transView;

        public Matrix4x4 matMH = new
        (
            -1,  0,  0, 0,
            0,  1,  0, 0,
            0,  0,  1, 0,
            0,  0,  0, 1
        );

        private bool showNormal = false;
        private bool showTangent = false;
        private bool showBones = false;
        private bool showBoneNames = false;
        private bool showTextures = true;
        private bool showGrid = true;
        private uint whiteTexId;

        public ModelViewForm()
        {
            InitializeComponent();
            InitializeScene();
        }

        private void InitializeScene()
        {
            SceneControlShortcut(Keys.T, showTexturesToolStripMenuItem_Click);
            SceneControlShortcut(Keys.G, showGridToolStripMenuItem_Click);
            SceneControlShortcut(Keys.N, showNormalsToolStripMenuItem_Click);
            SceneControlShortcut(Keys.B, showBonesToolStripMenuItem_Click);

            sceneControl.Disposed += (sender, e) => {
                OpenGLFinalized(sceneControl.OpenGL);
            };

            transView = new TransView
            {
                Pos = new(0f, 0f, 0f),
                Rot = new(20.0f, 0f, 45.0f),
                Zoom = 60f,
                Per = 35.0f
            };
        }

        private void OpenGLFinalized(OpenGL gl)
        {
            if (gl == null) return;

            fontRenderer.DeleteBuffers(gl);
            gridRenderer.DeleteBuffers(gl);
            axisRenderer.DeleteBuffers(gl);
            modelShaders.DestroyShaders(gl);
            model?.DisposeBuffers(gl);
        }

        public void SceneControlShortcut(Keys key, EventHandler handler)
        {
            sceneControl.KeyDown += (s, e) =>
            {
                if (e.KeyCode == key && !e.Control && !e.Alt && !e.Shift)
                {
                    handler.Invoke(s, e);
                    e.Handled = true;
                }
            };
        }

        private void sceneControl_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = sceneControl.OpenGL;

            BindBlankTexture(gl);

            modelShaders = new();
            modelShaders.InitShaders(gl);

            fontRenderer = new();
            fontRenderer.InitializeFont(gl, modelShaders.FontShader);

            gridRenderer = new();
            gridRenderer.InitializeBuffers(gl, modelShaders.ColorShader);

            axisRenderer = new();
            axisRenderer.InitializeBuffers(gl, fontRenderer, modelShaders.ColorShader);
        }

        public void SetMeshObject(string name, UObject obj)
        {
            SetTitle(name);

            mesh = obj;

            if (mesh == null)
            {
                MessageBox.Show("No mesh object set.");
                return;
            }

            var gl = sceneControl.OpenGL;

            model = new ModelMesh(mesh, name, gl);                

            ResetTransView();
        }

        private void BindBlankTexture(OpenGL gl)
        {
            uint[] tmp = new uint[1];
            gl.GenTextures(1, tmp);
            whiteTexId = tmp[0];
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, whiteTexId);
            byte[] white = [255, 255, 255, 255];
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, 1, 1, 0,
                          OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, white);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
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
                    ModelFormats.ExportModel(sfd.FileName, model, ModelFormats.GetExportFormat(ext));
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
            int dx = lastMousePos.X - cur.X;
            int dy = lastMousePos.Y - cur.Y;

            if (isPanning)
            {
                float sinY = (float)Math.Sin((transView.Rot.Z - 90) * Math.PI / 180.0);
                float cosY = (float)Math.Cos((transView.Rot.Z - 90) * Math.PI / 180.0);
                float sinX = (float)Math.Sin(transView.Rot.X * Math.PI / 180.0);
                float cosX = (float)Math.Cos(transView.Rot.X * Math.PI / 180.0);

                float zoom = transView.Zoom;
                float perRad = transView.Per * (float)(Math.PI / 180.0);

                float stepX = dx / (float)sceneControl.Width * zoom * perRad;
                float stepY = dy / (float)sceneControl.Height * zoom * perRad;

                transView.Pos.X -= stepX * sinY;
                transView.Pos.Y -= stepX * cosY;

                transView.Pos.X -= stepY * cosY * sinX;
                transView.Pos.Y += stepY * sinY * sinX;
                transView.Pos.Z -= stepY * cosX;
            }
            else if (isRotating)
            {
                transView.Rot.X -= dy / 5.0f;
                transView.Rot.Z -= dx / 5.0f;
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

            gl.Viewport(0, 0, width, height);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            // face culling
            gl.Enable(OpenGL.GL_CULL_FACE);

            // perspective
            float zoom = transView.Zoom;

            var matProjection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI * transView.Per / 180.0f,
                aspect,
                zoom / 50.0f,
                MaxDepth);

            if (transView.Rot.Z > 360.0f) transView.Rot.Z -= 360.0f;
            if (transView.Rot.Z < -360.0f) transView.Rot.Z += 360.0f;

            // camera
            var matView = Matrix4x4.CreateLookAt(
                new Vector3(0, -zoom, 0),
                Vector3.Zero,
                new Vector3(0, 0, 1));

            var matTranslate = Matrix4x4.CreateTranslation(-transView.Pos.X, -transView.Pos.Y, -transView.Pos.Z);
            var matRotZ = Matrix4x4.CreateRotationZ(MathF.PI * transView.Rot.Z / 180.0f);
            var matRotX = Matrix4x4.CreateRotationX(MathF.PI * transView.Rot.X / 180.0f);

            float fzoom = zoom / 15.0f;
            var matScale = Matrix4x4.CreateScale(fzoom);

            var matViewFinal =  matTranslate * matRotZ * matRotX * matView * matScale;

            // grid
            if (showGrid) gridRenderer.DrawGrid(gl, zoom, matProjection,  matViewFinal, Matrix4x4.Identity);

            // model
            DrawModel(gl, matProjection, matViewFinal, matMH);

            // axis
            int axisWidth = width / 7;
            int axisHeight = axisWidth;
            gl.Viewport(0, 0, axisWidth, axisHeight);

            var matProjectionAxis = Matrix4x4.CreatePerspectiveFieldOfView(
                    MathF.PI * 20.0f / 180.0f, 1.0f, 5.0f, 20.0f);

            var matViewAxis = Matrix4x4.CreateLookAt(
                new Vector3(0, -10, 0),
                Vector3.Zero,
                new Vector3(0, 0, 1));

            var matViewAxisFinal = Matrix4x4.CreateRotationZ(MathF.PI * transView.Rot.Z / 180.0f) *
                                   Matrix4x4.CreateRotationX(MathF.PI * transView.Rot.X / 180.0f) *
                                   matViewAxis;

            axisRenderer.DrawAxes(gl, matProjectionAxis, matViewAxisFinal, Matrix4x4.Identity);

            gl.Flush();
        }

        private void DrawModel(OpenGL gl, in Matrix4x4 matProjection, in Matrix4x4 matView, in Matrix4x4 matModel)
        {
            if (model.Mesh == null || model.Vertices == null) return;            

            var sh = modelShaders.NormalShader;
            sh.Bind(gl);

            sh.SetUniformMatrix4(gl, "uProj", matProjection.ToArray());
            sh.SetUniformMatrix4(gl, "uView", matView.ToArray());
            sh.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

            Matrix4x4.Invert(matView, out Matrix4x4 invView);
            Vector3 camPos = new (invView.M41, invView.M42, invView.M43);

            sh.SetVector3("uViewPos", camPos);

            // Light 0
            Vector3 uLightDir = Vector3.Normalize(new(1.0f));
            sh.SetVector3("uLightDir", uLightDir);
            sh.SetVector3("uLight0Color", new(0.9f));

            // Light 1
            Vector3 uLight1Dir = Vector3.Normalize(new(-1.0f, -1.0f, 1.0f));
            sh.SetVector3("uLight1Dir", uLight1Dir);
            sh.SetVector3("uLight1Color", new(0.6f));

            gl.BindVertexArray(model.vaoId);

            foreach (var section in model.Sections)
            {
                section.ApplyToShader(gl, sh, showTextures);

                int indexStart = (int)section.BaseIndex;
                int indexCount = (int)(section.NumTriangles * 3);

                gl.DrawElements(OpenGL.GL_TRIANGLES, indexCount, OpenGL.GL_UNSIGNED_INT, new IntPtr(indexStart * sizeof(uint)));
            }

            gl.BindVertexArray(0);

            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE1);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE2);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE3);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE4);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE5);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            sh.Unbind(gl);

            if (showNormal)
                DrawLines(gl, 0, modelShaders.ColorShader1, matProjection, matView, matModel);

            if (showTangent)
                DrawLines(gl, 1, modelShaders.ColorShader1, matProjection, matView, matModel);

            if (showBones || showBoneNames)
                DrawBones(gl, modelShaders.ColorShader1, matProjection, matView, matModel);            
        }

        private void DrawBones(OpenGL gl, ShaderProgram shader, in Matrix4x4 matProjection, in Matrix4x4 matView, in Matrix4x4 matModel)
        {
            if (model.Bones == null) return;

            if (!model.BonesBuffersInitialized)
                model.InitializeBoneBuffers(gl);

            model.UpdateBoneBuffers(gl);

            if (model.BonePointCount == 0) return;

            gl.Disable(OpenGL.GL_DEPTH_TEST);

            List<Vector3> bonePositions = [];
            if (showBoneNames)
                for (int i = 0; i < model.BoneNameIndices.Count; i++)
                {
                    int boneIndex = model.BoneNameIndices[i];
                    var to = model.Bones[boneIndex].GlobalTransform.Translation;
                    bonePositions.Add(to);
                }

            shader.Bind(gl);
            shader.SetUniformMatrix4(gl, "uProjection", matProjection.ToArray());
            shader.SetUniformMatrix4(gl, "uView", matView.ToArray());
            shader.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

            if (showBones)
            {
                shader.SetUniform4(gl, "uColor", 0.8f, 0.8f, 0.8f, 1.0f);
                gl.BindVertexArray(model.BonePointVAO);
                gl.PointSize(4.0f);
                gl.DrawArrays(OpenGL.GL_POINTS, 0, model.BonePointCount);

                shader.SetUniform4(gl, "uColor", 0.5f, 0.5f, 0.5f, 1.0f);
                gl.BindVertexArray(model.BoneLineVAO);
                gl.LineWidth(1f);
                gl.DrawArrays(OpenGL.GL_LINES, 0, model.BoneLineCount);
            }

            gl.BindVertexArray(0);
            shader.Unbind(gl);

            if (showBoneNames)
            {
                Vector4 color = new(1.0f, 1.0f, 0.0f, 1.0f);
                for (int i = 0; i < model.BoneNames.Count; i++)
                    fontRenderer.DrawText(gl, " " + model.BoneNames[i], bonePositions[i],
                                        matProjection, matView, matModel, color);
            }

            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void DrawLines(OpenGL gl, int type, ShaderProgram shader, in Matrix4x4 matProjection, in Matrix4x4 matView, in Matrix4x4 matModel)
        {
            uint vaoId;
            int count;
            Vector4 color;
            if (type == 0) 
            {
                vaoId = model.nlvao;
                count = model.nlCount;
                color = new Vector4(1f, 0f, 1f, 1f);
            }
            else
            {
                vaoId = model.ntvao;
                count = model.ntCount;
                color = new Vector4(0f, 1f, 1f, 1f);
            }              

            if (vaoId == 0) model.PrepareLines(gl, type);

            shader.Bind(gl);

            shader.SetUniformMatrix4(gl, "uProjection", matProjection.ToArray());
            shader.SetUniformMatrix4(gl, "uView", matView.ToArray());
            shader.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

            shader.SetUniform4(gl, "uColor", color.X, color.Y, color.Z, color.W);

            gl.BindVertexArray(vaoId);
            gl.DrawArrays(OpenGL.GL_LINES, 0, count);
            gl.BindVertexArray(0);

            shader.Unbind(gl);
        }

        private void sceneControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                transView.Zoom -= transView.Zoom * 0.1f;
            else
                transView.Zoom += transView.Zoom * 0.1f;

            if (transView.Zoom < 1.0f)
                transView.Zoom = 1.0f;
            if (transView.Zoom > 1000.0f)
                transView.Zoom = 1000.0f;

            sceneControl.DoRender();
        }

        private void sceneControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad7:
                    transView.Rot.X += 10.0f;
                    break;
                case Keys.NumPad1:
                    transView.Rot.X -= 10.0f;
                    break;
                case Keys.NumPad8:
                    transView.Pos.Z += 10.0f;
                    break;
                case Keys.NumPad2:
                    transView.Pos.Z -= 10.0f;
                    break;
                case Keys.NumPad4:
                    transView.Pos.X += 10.0f;
                    break;
                case Keys.NumPad6:
                    transView.Pos.X -= 10.0f;
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
                Pos = model.Center,
                Rot = new(20.0f, 0f, 45.0f),
                Zoom = model.Radius * 3.5f,
                Per = 35.0f
            };
        }

        private void showNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showNormalsToolStripMenuItem.Checked = !showNormalsToolStripMenuItem.Checked;
            showNormal = showNormalsToolStripMenuItem.Checked;
            sceneControl.Invalidate();
        }

        private void showBonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBonesToolStripMenuItem.Checked = !showBonesToolStripMenuItem.Checked;
            showBones = showBonesToolStripMenuItem.Checked;
            sceneControl.Invalidate();
        }

        private void showBoneNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showBoneNameToolStripMenuItem.Checked = !showBoneNameToolStripMenuItem.Checked;
            showBoneNames = showBoneNameToolStripMenuItem.Checked;
            sceneControl.Invalidate();
        }

        private void showTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTexturesToolStripMenuItem.Checked = !showTexturesToolStripMenuItem.Checked;
            showTextures = showTexturesToolStripMenuItem.Checked;
            sceneControl.Invalidate();
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showGridToolStripMenuItem.Checked = !showGridToolStripMenuItem.Checked;
            showGrid = showGridToolStripMenuItem.Checked;
            sceneControl.Invalidate();
        }

        private void showTangentsMenuItem_Click(object sender, EventArgs e)
        {
            showTangentMenuItem.Checked = !showTangentMenuItem.Checked;
            showTangent = showTangentMenuItem.Checked;
            sceneControl.Invalidate();
        }

        public struct TransView
        {
            public Vector3 Pos;
            public Vector3 Rot;
            public float Zoom, Per;
        }
    }
}
