using SharpGL;
using System.Numerics;
using System.Runtime.InteropServices;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Engine.Material;
using UpkManager.Models.UpkFile.Engine.Mesh;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace MHUpkManager
{
    public partial class ModelViewForm : Form
    {
        SceneControl sceneControl;
        private string title;
        private UObject mesh;
        private ModelMeshData model;

        private const float MaxDepth = 100000.0f;

        private Point lastMousePos;
        private bool isPanning = false;
        private bool isRotating = false;
        private TransView transView;

        public double[] matMH =
        [
            -1,  0,  0, 0,
            0,  1,  0, 0,
            0,  0,  1, 0,
            0,  0,  0, 1
        ];

        private bool showNormal = false;
        private bool showBones = false;
        private bool showBoneNames = false;
        private bool showTextures = true;
        private bool showGrid = true;

        public ModelViewForm()
        {
            InitializeComponent();
            InitializeScene();
        }

        private void InitializeScene()
        {
            sceneControl = new SceneControl();

            ((System.ComponentModel.ISupportInitialize)sceneControl).BeginInit();

            sceneControl.Dock = DockStyle.Fill;
            sceneControl.DrawFPS = false;
            sceneControl.Location = new Point(0, 24);
            sceneControl.Margin = new Padding(4, 3, 4, 3);
            sceneControl.Name = "sceneControl";
            sceneControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            sceneControl.RenderContextType = RenderContextType.NativeWindow;
            sceneControl.RenderTrigger = RenderTrigger.Manual;
            sceneControl.Size = new Size(800, 426);
            sceneControl.TabIndex = 0;
            sceneControl.OpenGLInitialized += sceneControl_OpenGLInitialized;
            sceneControl.OpenGLDraw += sceneControl_OpenGLDraw;
            sceneControl.KeyDown += sceneControl_KeyDown;
            sceneControl.MouseDown += sceneControl_MouseDown;
            sceneControl.MouseMove += sceneControl_MouseMove;
            sceneControl.MouseUp += sceneControl_MouseUp;

            sceneControl.MouseWheel += sceneControl_MouseWheel;
            SceneControlShortcut(Keys.T, showTexturesToolStripMenuItem_Click);
            SceneControlShortcut(Keys.G, showGridToolStripMenuItem_Click);
            SceneControlShortcut(Keys.N, showNormalsToolStripMenuItem_Click);
            SceneControlShortcut(Keys.B, showBonesToolStripMenuItem_Click);

            Controls.Add(sceneControl);

            ((System.ComponentModel.ISupportInitialize)sceneControl).EndInit();

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

            GLLib.InitializeFont(gl);
            GenerateDisplayLists(gl);
            SetupLighting(gl);
        }

        private void SetupLighting(OpenGL gl)
        {
            float[] ambient = { 0.1f, 0.1f, 0.1f, 1.0f };
            float[] lightPos = { -400.0f, -400.0f, 400f, 1.0f };
            float[] light1Pos = { 200.0f, 200.0f, 200f, 1.0f };

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

        public void SetMeshObject(string name, UObject obj)
        {
            SetTitle(name);

            mesh = obj;

            if (mesh == null)
            {
                MessageBox.Show("No mesh object set.");
                return;
            }

            model = new ModelMeshData(mesh, name, sceneControl.OpenGL);
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

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            // face culling
            gl.Enable(OpenGL.GL_CULL_FACE);

            // --- perspective ---
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, width, height);

            gl.Perspective(transView.Per, aspect, transView.Zoom / 50.0f, MaxDepth);

            if (transView.Rot.Z > 360.0f) transView.Rot.Z -= 360.0f;
            if (transView.Rot.Z < -360.0f) transView.Rot.Z += 360.0f;

            // camera
            gl.LookAt(0, -transView.Zoom, 0, 0, 0, 0, 0, 0, 1);
            gl.Rotate(transView.Rot.X, 1, 0, 0);
            gl.Rotate(transView.Rot.Z, 0, 0, 1);
            gl.Translate(-transView.Pos.X, -transView.Pos.Y, -transView.Pos.Z);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // grid
            if (showGrid) GLLib.DrawGrid(gl, transView.Zoom);

            gl.PushMatrix();
            gl.MultMatrix(matMH);
            DrawModel(gl);
            gl.PopMatrix();

            float fzoom = transView.Zoom / 15.0f;
            gl.Scale(fzoom, fzoom, fzoom);

            // --- axis ---
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, 0, width / 7, height / 7);

            gl.Perspective(20.0f, aspect, 5.0f, 20.0f);
            gl.LookAt(0, -10, 0, 0, 0, 0, 0, 0, 1);
            gl.Rotate(transView.Rot.X, 1, 0, 0);
            gl.Rotate(transView.Rot.Z, 0, 0, 1);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            gl.CallList(GLLib.OBJ_AXES);

            gl.Flush();
        }

        private void DrawModel(OpenGL gl)
        {
            if (model.Mesh == null || model.Vertices == null) return;

            if (showTextures)
                gl.Enable(OpenGL.GL_TEXTURE_2D);
            else
                gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, model.vboId);
            gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, model.iboId);

            gl.EnableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.EnableClientState(OpenGL.GL_NORMAL_ARRAY);
            if (showTextures)
                gl.EnableClientState(OpenGL.GL_TEXTURE_COORD_ARRAY);

            int stride = Marshal.SizeOf(typeof(GLVertex));

            gl.VertexPointer(3, OpenGL.GL_FLOAT, stride, IntPtr.Zero);

            gl.NormalPointer(OpenGL.GL_FLOAT, stride, new IntPtr(Marshal.OffsetOf(typeof(GLVertex), nameof(GLVertex.Normal)).ToInt32()));

            if (showTextures)
            {
                gl.TexCoordPointer(2, OpenGL.GL_FLOAT, stride, new IntPtr(Marshal.OffsetOf(typeof(GLVertex), nameof(GLVertex.TexCoord)).ToInt32()));
            }

            foreach (var section in model.Sections)
            {
                if (showTextures)
                {
                    if (section.GLTextureId != 0)
                        gl.BindTexture(OpenGL.GL_TEXTURE_2D, section.GLTextureId);
                    else
                        gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
                }

                int indexStart = (int)section.BaseIndex;
                int indexCount = (int)(section.NumTriangles * 3);

                gl.DrawElements(OpenGL.GL_TRIANGLES, indexCount, OpenGL.GL_UNSIGNED_INT, new IntPtr(indexStart * sizeof(uint)));
            }

            gl.DisableClientState(OpenGL.GL_VERTEX_ARRAY);
            gl.DisableClientState(OpenGL.GL_NORMAL_ARRAY);
            if (showTextures)
                gl.DisableClientState(OpenGL.GL_TEXTURE_COORD_ARRAY);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            if (showNormal)
                DrawNormal(gl);

            if (showBones || showBoneNames)
                DrawBones(gl);
        }

        private static Vector3 GetTranslation(Matrix4x4 m)
        {
            return new Vector3(m.M41, m.M42, m.M43);
        }

        private void DrawBones(OpenGL gl)
        {
            if (model.Bones != null)
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
            }
        }

        private void DrawNormal(OpenGL gl)
        {
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(1.0f, 0.0f, 1.0f);

            gl.Begin(OpenGL.GL_LINES);
            foreach (var section in model.Sections)
            {
                uint start = section.BaseIndex;
                uint end = start + section.NumTriangles * 3;
                for (uint i = start; i < end; i++)
                {
                    var vertex = model.Vertices[model.Indices[i]];

                    var pos = vertex.Position;
                    var norm = vertex.Normal;

                    float scale = 1.0f;
                    var endPos = new Vector3(
                        pos.X + norm.X * scale,
                        pos.Y + norm.Y * scale,
                        pos.Z + norm.Z * scale
                    );

                    gl.Vertex(pos.X, pos.Y, pos.Z);
                    gl.Vertex(endPos.X, endPos.Y, endPos.Z);
                }
            }

            gl.End();
            gl.Enable(OpenGL.GL_LIGHTING);
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

        private void GenerateDisplayLists(OpenGL gl)
        {
            gl.NewList(GLLib.OBJ_AXES, OpenGL.GL_COMPILE);
            GLLib.DrawAxes(gl);
            gl.EndList();
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

        public struct MeshSectionData
        {
            public uint BaseIndex;
            public uint NumTriangles;

            public int MaterialIndex;
            public UMaterialInstanceConstant Material;
            public UTexture2D DiffuseTexture;
            public int MipIndex;
            public uint GLTextureId;
            public string TextureName;
            public byte[] TextureData;

            public void LoadMaterial(OpenGL gl, FObject material)
            {
                Material = material?.LoadObject<UMaterialInstanceConstant>();
                var textureObj = Material?.GetTextureParameterValue("Diffuse");
                TextureName = textureObj?.Name;
                GLTextureId = GLLib.BindGLTexture(gl, textureObj, out MipIndex, out DiffuseTexture, out TextureData);
            }
        }

        public struct Bone
        {
            public string Name;
            public int ParentIndex;
            public Matrix4x4 LocalTransform;
            public Matrix4x4 GlobalTransform;
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

                uint[] buffers = new uint[2];
                gl.GenBuffers(2, buffers);
                iboId = buffers[0];
                vboId = buffers[1];

                BindIndexBuffer(gl);
                BindVertexBuffer(gl);
            }

            private readonly void BindVertexBuffer(OpenGL gl)
            {
                var handle = GCHandle.Alloc(Vertices, GCHandleType.Pinned);
                try
                {
                    gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, vboId);
                    gl.BufferData(OpenGL.GL_ARRAY_BUFFER, Vertices.Length * Marshal.SizeOf(typeof(GLVertex)),
                        handle.AddrOfPinnedObject(), OpenGL.GL_STATIC_DRAW);
                }
                finally
                {
                    handle.Free();
                }
            }

            private readonly void BindIndexBuffer(OpenGL gl)
            {
                var handle = GCHandle.Alloc(Indices, GCHandleType.Pinned);
                try
                {
                    gl.BindBuffer(OpenGL.GL_ELEMENT_ARRAY_BUFFER, iboId);
                    gl.BufferData(OpenGL.GL_ELEMENT_ARRAY_BUFFER, Indices.Length * sizeof(int),
                        handle.AddrOfPinnedObject(), OpenGL.GL_STATIC_DRAW);
                }
                finally
                {
                    handle.Free();
                }
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
