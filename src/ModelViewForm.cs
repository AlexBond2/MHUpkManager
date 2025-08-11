using SharpGL;
using SharpGL.Shaders;

using System.Numerics;
using System.Runtime.InteropServices;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Engine.Material;
using UpkManager.Models.UpkFile.Engine.Mesh;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;
using static MHUpkManager.GLLib;

namespace MHUpkManager
{
    public partial class ModelViewForm : Form
    {
        private string title;
        private UObject mesh;
        private ModelMeshData model;
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

            transView = new TransView
            {
                Pos = new(0f, 0f, 0f),
                Rot = new(20.0f, 0f, 45.0f),
                Zoom = 60f,
                Per = 35.0f
            };
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

            modelShaders = new();
            modelShaders.InitShaders(gl);

            fontRenderer = new();
            fontRenderer.InitializeFont(gl, modelShaders.FontShader);

            gridRenderer = new();
            gridRenderer.InitializeBuffers(gl, modelShaders.ColorShader);

            axisRenderer = new();
            axisRenderer.InitializeBuffers(gl, fontRenderer, modelShaders.ColorShader);
            //SetupLighting(gl);
        }

        private void SetupLighting(OpenGL gl)
        {
          /*  float[] ambient = { 0.1f, 0.1f, 0.1f, 1.0f };
            float[] lightPos = { -1.0f, -1.0f, 1.0f, 0.0f };
            float[] light1Pos = { 1.0f, 1.0f, 1.0f, 0.0f };

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

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, 1);*/
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

            BindBlankTexture(gl);

            model = new ModelMeshData(mesh, name, gl);                

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
            if (showGrid) gridRenderer.DrawGrid(gl, zoom, matProjection,  matViewFinal);

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

            axisRenderer.DrawAxes(gl, matProjectionAxis, matViewAxisFinal);

            gl.Flush();
        }

        private void DrawModel(OpenGL gl, Matrix4x4 matProjection, Matrix4x4 matView, Matrix4x4 matModel)
        {
            if (model.Mesh == null || model.Vertices == null) return;            

            var sh = modelShaders.NormalShader;
            sh.Bind(gl);

            sh.SetUniformMatrix4(gl, "uProj", matProjection.ToArray());
            sh.SetUniformMatrix4(gl, "uView", matView.ToArray());
            sh.SetUniformMatrix4(gl, "uModel", matModel.ToArray());

            // Light 0
            Vector3 uLightDir = Vector3.Normalize(new(1.0f, 1.0f, 1.0f));
            sh.SetUniform3(gl, "uLightDir", uLightDir.X, uLightDir.Y, uLightDir.Z);
            sh.SetUniform3(gl, "uLight0Color", 0.9f, 0.9f, 0.9f);

            // Light 1
            Vector3 uLight1Dir = Vector3.Normalize(new(-1.0f, -1.0f, 1.0f));
            sh.SetUniform3(gl, "uLight1Dir", uLight1Dir.X, uLight1Dir.Y, uLight1Dir.Z);
            sh.SetUniform3(gl, "uLight1Color", 0.6f, 0.6f, 0.6f);

            sh.SetUniform1(gl, "uDiffuseMap", 0);
            sh.SetUniform1(gl, "uNormalMap", 2);

            gl.BindVertexArray(model.vaoId);

            foreach (var section in model.Sections)
            {
                gl.ActiveTexture(OpenGL.GL_TEXTURE0);
                if (showTextures && section.GetTextureType(TextureType.uDiffuseMap, out var diffuse))
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, diffuse.TextureId);
                else
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, whiteTexId);

                gl.ActiveTexture(OpenGL.GL_TEXTURE2);
                if (section.GetTextureType(TextureType.uNormalMap, out var normal))
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, normal.TextureId);
                else
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, whiteTexId);

                int indexStart = (int)section.BaseIndex;
                int indexCount = (int)(section.NumTriangles * 3);

                gl.DrawElements(OpenGL.GL_TRIANGLES, indexCount, OpenGL.GL_UNSIGNED_INT, new IntPtr(indexStart * sizeof(uint)));
            }

            gl.BindVertexArray(0);

            gl.ActiveTexture(OpenGL.GL_TEXTURE0);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.ActiveTexture(OpenGL.GL_TEXTURE2);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

            sh.Unbind(gl);

            if (showNormal)
                DrawLines(gl, 0, modelShaders.ColorShader1, matProjection, matView, matModel);

            if (showTangent)
                DrawLines(gl, 1, modelShaders.ColorShader1, matProjection, matView, matModel);

            if (showBones || showBoneNames)
                DrawBones(gl);            
        }

        private static Vector3 GetTranslation(Matrix4x4 m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }

        private void DrawBones(OpenGL gl)
        {
   /*         if (model.Bones != null)
            {
                gl.Disable(OpenGL.GL_LIGHTING);
                gl.Disable(OpenGL.GL_DEPTH_TEST);

                for (int i = 0; i < model.Bones.Count; i++)
                {
                    var bone = model.Bones[i];
                    if (bone.ParentIndex >= 0)
                    {
                        var to = GetTranslation(bone.GlobalTransform);

                        if (showBones)
                        {
                            gl.PointSize(4.0f);
                            gl.Color(0.8f, 0.8f, 0.8f);
                            gl.Begin(OpenGL.GL_POINTS);
                            gl.Vertex(to.X, to.Y, to.Z);
                            gl.End();
                            gl.PointSize(1.0f);

                            gl.LineWidth(1f);
                            gl.Color(0.5f, 0.5f, 0.5f);
                            var parent = model.Bones[bone.ParentIndex];
                            var from = GetTranslation(parent.GlobalTransform);
                            gl.Begin(OpenGL.GL_LINES);
                            gl.Vertex(from.X, from.Y, from.Z);
                            gl.Vertex(to.X, to.Y, to.Z);
                            gl.End();
                        }

                        if (showBoneNames)
                        {
                            gl.Color(1.0f, 1.0f, 0.0f);
                            gl.RasterPos(to.X, to.Y, to.Z);
                            GLLib.DrawText(gl, " " + bone.Name);
                        }
                    }
                }
                gl.Enable(OpenGL.GL_LIGHTING);
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }*/
        }

        private void PrepareLines(OpenGL gl, int type)
        {
            uint[] vaos = new uint[1];
            gl.GenVertexArrays(1, vaos);
            uint vaoId = vaos[0];

            uint[] vbos = new uint[1];
            gl.GenBuffers(1, vbos);
            uint vboId = vbos[0];

            List<float> lines = []; 

            foreach (var section in model.Sections)
            {
                uint start = section.BaseIndex;
                uint end = start + section.NumTriangles * 3;
                for (uint i = start; i < end; i++)
                {
                    var vertex = model.Vertices[model.Indices[i]];

                    var pos = vertex.Position;
                    var line = type == 0 ? vertex.Normal : vertex.Tangent;

                    float scale = 1.0f;
                    var endPos = new Vector3(
                        pos.X + line.X * scale,
                        pos.Y + line.Y * scale,
                        pos.Z + line.Z * scale
                    );

                    lines.Add(pos.X);
                    lines.Add(pos.Y);
                    lines.Add(pos.Z);

                    lines.Add(endPos.X);
                    lines.Add(endPos.Y);
                    lines.Add(endPos.Z);
                }
            }

            int count = lines.Count / 3;
            gl.BindVertexArray(vaoId);
            BindVertexBuffer(gl, vboId, sizeof(float), OpenGL.GL_STATIC_DRAW, [.. lines]);
            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0, 0);
            gl.EnableVertexAttribArray(0);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
            gl.BindVertexArray(0);

            if (type == 0)
            {
                model.nlvao = vaoId;
                model.nlCount = count;
            }
            else
            {
                model.ntvao = vaoId;
                model.ntCount = count;
            }
        }

        private void DrawLines(OpenGL gl, int type, ShaderProgram shader, Matrix4x4 matProjection, Matrix4x4 matView, Matrix4x4 matModel)
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

            if (vaoId == 0) PrepareLines(gl, type);

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

        public struct Texture2DData
        {
            public TextureType Type;
            public UTexture2D Texture2D;
            public int MipIndex;
            public uint TextureId;
            public string Name;
            public byte[] Data;

            public Texture2DData(TextureType type, OpenGL gl, FObject textureObj) : this()
            {
                Type = type;
                Name = textureObj?.Name;
                TextureId = GLLib.BindGLTexture(gl, textureObj, out MipIndex, out Texture2D, out Data);
            }
        }

        public enum TextureType
        {
            uDiffuseMap,
            uNormalMap
        }

        public struct MeshSectionData
        {
            public uint BaseIndex;
            public uint NumTriangles;

            public UMaterialInstanceConstant Material;
            public int MaterialIndex;
            public List<Texture2DData> Textures;

            public void LoadMaterial(OpenGL gl, FObject material)
            {
                Textures = [];
                Material = material?.LoadObject<UMaterialInstanceConstant>();

                var textureObj = Material?.GetTextureParameterValue("Diffuse");
                if (textureObj != null) Textures.Add(new(TextureType.uDiffuseMap, gl, textureObj));

                textureObj = Material?.GetTextureParameterValue("Normal");
                if (textureObj != null) Textures.Add(new(TextureType.uNormalMap, gl, textureObj));
            }

            public bool IsDiffuse()
            {
                if (GetTextureType(TextureType.uDiffuseMap, out var texture))
                    return texture.TextureId != 0;
                return false;
            }

            public bool IsNormal()
            {
                if (GetTextureType(TextureType.uNormalMap, out var texture))
                    return texture.TextureId != 0;
                return false;
            }

            public readonly bool GetTextureType(TextureType type, out Texture2DData texture)
            {
                if (Textures != null)
                    foreach (var tex in Textures)
                        if (tex.Type == type)
                        {
                            texture = tex;
                            return true;
                        }

                texture = default;
                return false;
            }
        }

        public struct Bone
        {
            public string Name;
            public int ParentIndex;
            public Matrix4x4 LocalTransform;
            public Matrix4x4 GlobalTransform;
        }

        public static class GLVertexOffsets
        {
            public static readonly IntPtr Position = new(0);
            public static readonly IntPtr Normal = GetOffset(nameof(GLVertex.Normal));
            public static readonly IntPtr TexCoord = GetOffset(nameof(GLVertex.TexCoord));
            public static readonly IntPtr Tangent = GetOffset(nameof(GLVertex.Tangent));

            private static IntPtr GetOffset(string fieldName)
            {
                return new IntPtr(Marshal.OffsetOf(typeof(GLVertex), fieldName).ToInt32());
            }
        }

        public struct ModelMeshData
        {
            public UObject Mesh;
            public string ModelName;
            public Vector3 Center;
            public float Radius;

            public int[] Indices;
            public GLVertex[] Vertices;
            public List<Bone> Bones;

            public List<MeshSectionData> Sections;
            public uint iboId;
            public uint vboId;
            public uint vaoId;
            public uint nlvao;
            public int nlCount;
            public uint ntvao;
            public int ntCount;

            public ModelMeshData(UObject obj, string name, OpenGL gl)
            {
                Mesh = obj;
                ModelName = name;
                Sections = [];
                Bones = [];

                if (obj is USkeletalMesh mesh)
                {
                    var lod = mesh.LODModels[0];

                    Vertices = [.. lod.VertexBufferGPUSkin.GetGLVertexData()];
                    Indices = ConvertIndices(lod.MultiSizeIndexContainer.IndexBuffer);

                    CalculateCenterAndRadius(Vertices);

                    foreach (var section in lod.Sections)
                    {
                        var processed = new HashSet<int>();
                        var chunk = lod.Chunks[section.ChunkIndex];
                        var boneMap = chunk.BoneMap;

                        uint start = section.BaseIndex;
                        uint end = start + section.NumTriangles * 3;

                        for (uint i = start; i < end; i++)
                        {
                            var vertexIndex = Indices[i];
                            if (processed.Contains(vertexIndex)) continue;

                            RemapBoneIndices(vertexIndex, boneMap);
                            processed.Add(vertexIndex);
                        }

                        var sectionData = new MeshSectionData
                        {
                            BaseIndex = section.BaseIndex,
                            NumTriangles = section.NumTriangles,
                            MaterialIndex = section.MaterialIndex
                        };

                        if (section.MaterialIndex < mesh.Materials.Count)
                        {
                            sectionData.LoadMaterial(gl, mesh.Materials[section.MaterialIndex]);
                        }

                        Sections.Add(sectionData);
                    }

                    if (mesh.RefSkeleton != null)
                    {
                        for (int i = 0; i < mesh.RefSkeleton.Count; i++)
                        {
                            var bone = mesh.RefSkeleton[i];

                            Bones.Add(new Bone
                            {
                                Name = bone.Name.ToString(),
                                ParentIndex = bone.ParentIndex,
                                LocalTransform = bone.BonePos.ToMatrix(),
                                GlobalTransform = Matrix4x4.Identity
                            });
                        }

                        for (int i = 0; i < Bones.Count; i++)
                        {
                            var bone = Bones[i];
                            if (bone.ParentIndex >= 0)
                                bone.GlobalTransform = bone.LocalTransform * Bones[bone.ParentIndex].GlobalTransform;
                            else
                                bone.GlobalTransform = bone.LocalTransform;
                            Bones[i] = bone;
                        }
                    }
                }
                else if (obj is UStaticMesh staticMesh)
                {
                    var lod = staticMesh.LODModels[0];

                    Vertices = [.. lod.GetGLVertexData()];
                    Indices = ConvertIndices(lod.IndexBuffer.Indices);

                    CalculateCenterAndRadius(Vertices);

                    foreach (var element in lod.Elements)
                    {
                        var sectionData = new MeshSectionData
                        {
                            BaseIndex = element.FirstIndex,
                            NumTriangles = element.NumTriangles,
                            MaterialIndex = element.MaterialIndex
                        };

                        sectionData.LoadMaterial(gl, element.Material);

                        Sections.Add(sectionData);
                    }
                }

                InitBuffers(gl);

            }

            private void InitBuffers(OpenGL gl)
            {
                // Gen VAO, VBO, IBO
                uint[] buffers = new uint[2];
                gl.GenVertexArrays(1, buffers);
                vaoId = buffers[0];

                gl.GenBuffers(2, buffers);
                vboId = buffers[0];
                iboId = buffers[1];

                gl.BindVertexArray(vaoId);

                // VBO
                gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vboId);
                var handleVertices = GCHandle.Alloc(Vertices, GCHandleType.Pinned);
                try
                {
                    gl.BufferData(OpenGL.GL_ARRAY_BUFFER, Vertices.Length * Marshal.SizeOf(typeof(GLVertex)),
                        handleVertices.AddrOfPinnedObject(), OpenGL.GL_STATIC_DRAW);
                }
                finally
                {
                    handleVertices.Free();
                }

                int stride = Marshal.SizeOf(typeof(GLVertex));

                gl.EnableVertexAttribArray(0);
                gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Position);

                gl.EnableVertexAttribArray(1);
                gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Normal);

                gl.EnableVertexAttribArray(2);
                gl.VertexAttribPointer(2, 2, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.TexCoord);

                gl.EnableVertexAttribArray(3);
                gl.VertexAttribPointer(3, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Tangent);

                // IBO
                gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, iboId);
                var handleIndices = GCHandle.Alloc(Indices, GCHandleType.Pinned);
                try
                {
                    gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, Indices.Length * sizeof(uint),
                        handleIndices.AddrOfPinnedObject(), OpenGL.GL_STATIC_DRAW);
                }
                finally
                {
                    handleIndices.Free();
                }

                gl.BindVertexArray(0);
            }

            public void RemapBoneIndices(int vertexIndex, UArray<ushort> boneMap)
            {
                var vertex = Vertices[vertexIndex];

                vertex.Bone0 = RemapBone(vertex.Bone0, boneMap);
                vertex.Bone1 = RemapBone(vertex.Bone1, boneMap);
                vertex.Bone2 = RemapBone(vertex.Bone2, boneMap);
                vertex.Bone3 = RemapBone(vertex.Bone3, boneMap);

                Vertices[vertexIndex] = vertex;
            }

            private static byte RemapBone(byte boneIndex, UArray<ushort> boneMap)
            {
                if (boneIndex < boneMap.Count)
                    return (byte)boneMap[boneIndex];
                else
                    return 0;
            }

            public static int[] ConvertIndices<T>(IEnumerable<T> indices) where T : struct, IConvertible
            {
                var indicesArray = indices.ToArray();
                if (indicesArray.Length % 3 != 0) return [];

                int[] converted = new int[indicesArray.Length];

                for (int i = 0; i < indicesArray.Length; i += 3)
                {
                    converted[i] = Convert.ToInt32(indicesArray[i]);
                    converted[i + 1] = Convert.ToInt32(indicesArray[i + 1]);
                    converted[i + 2] = Convert.ToInt32(indicesArray[i + 2]);
                }

                return converted;
            }

            private void CalculateCenterAndRadius(GLVertex[] vertices)
            {
                if (vertices == null || vertices.Length == 0)
                {
                    Center = new Vector3(0f, 0f, 0f);
                    Radius = 0.0f;
                }

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

                Center = new Vector3(
                    (minX + maxX) * 0.5f,
                    (minY + maxY) * 0.5f,
                    (minZ + maxZ) * 0.5f
                );

                var corner = new Vector3(maxX, maxY, maxZ);
                Radius = Vector3.Distance(Center, corner);
            }
        }

        public struct TransView
        {
            public Vector3 Pos;
            public Vector3 Rot;
            public float Zoom, Per;
        }
    }
}
