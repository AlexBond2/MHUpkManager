using SharpGL;
using System.Numerics;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Engine.Material;
using UpkManager.Models.UpkFile.Engine.Mesh;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;

namespace MHUpkManager
{
    public partial class ModelViewForm : Form
    {
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
                transView.zpos -= stepY * cosX;
            }
            else if (isRotating)
            {
                transView.xrot -= dy / 5.0f;
                transView.zrot -= dx / 5.0f;
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

            gl.Perspective(transView.Per, aspect, transView.zoom / 50.0f, MaxDepth);

            if (transView.zrot > 360.0f) transView.zrot -= 360.0f;
            if (transView.zrot < -360.0f) transView.zrot += 360.0f;

            // camera
            gl.LookAt(0, -transView.zoom, 0, 0, 0, 0, 0, 0, 1);
            gl.Rotate(transView.xrot, 1, 0, 0);
            gl.Rotate(transView.zrot, 0, 0, 1);
            gl.Translate(-transView.xpos, -transView.ypos, -transView.zpos);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // grid
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.CallList(GLLib.OBJ_GRID);
            gl.Enable(OpenGL.GL_LIGHTING);

            gl.PushMatrix();
            gl.MultMatrix(matMH);
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

            gl.CallList(GLLib.OBJ_AXES);

            gl.Flush();
        }

        private void DrawModel(OpenGL gl)
        {
            if (model.Mesh == null || model.Vertices == null) return;

            gl.Enable(OpenGL.GL_TEXTURE_2D);

            foreach (var section in model.Sections)
            {
                if (section.GLTextureId != 0)
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, section.GLTextureId);
                else
                    gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0); // или текстура по умолчанию

                gl.Begin(OpenGL.GL_TRIANGLES);
                uint start = section.BaseIndex;
                uint end = start + section.NumTriangles * 3;

                for (uint i = start; i < end; i++)
                {
                    var vertex = model.Vertices[model.Indices[i]];
                    gl.Normal(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z);
                    gl.TexCoord(vertex.TexCoord.X, vertex.TexCoord.Y);
                    gl.Vertex(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
                }

                gl.End();
            }

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            if (showNormal)
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

                        float scale = 3.0f;
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
            gl.NewList(GLLib.OBJ_AXES, OpenGL.GL_COMPILE);
            GLLib.DrawAxes(gl);
            gl.EndList();

            gl.NewList(GLLib.OBJ_GRID, OpenGL.GL_COMPILE);
            GLLib.DrawGrid(gl, 16);
            gl.EndList();
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
                zrot = 45.0f,
                zoom = model.Radius * 3f,
                Per = 35.0f
            };
        }

        public struct MeshSectionData
        {
            public uint BaseIndex;
            public uint NumTriangles;

            public int MaterialIndex;
            public UMaterialInstanceConstant Material;
            public UTexture2D DiffuseTexture;
            public uint GLTextureId;
            public string TextureName;
            public byte[] TextureData;

            public void LoadMaterial(OpenGL gl, FObject material)
            {
                Material = material?.LoadObject<UMaterialInstanceConstant>();
                var textureObj = Material?.GetTextureParameterValue("DiffuseTexture");
                TextureName = textureObj?.Name;
                DiffuseTexture = textureObj?.LoadObject<UTexture2D>();
                GLTextureId = GLLib.BindGLTexture(gl, DiffuseTexture, out TextureData);
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

            public List<MeshSectionData> Sections;

            public ModelMeshData(UObject obj, string name, OpenGL gl)
            {
                Mesh = obj;
                ModelName = name;
                Sections = [];

                if (obj is USkeletalMesh mesh)
                {
                    var lod = mesh.LODModels[0];

                    Vertices = [.. lod.VertexBufferGPUSkin.GetGLVertexData()];
                    Indices = ConvertIndices(lod.MultiSizeIndexContainer.IndexBuffer);

                    CalculateCenterAndRadius(Vertices);

                    foreach (var section in lod.Sections)
                    {
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
            public float xpos, ypos, zpos;
            public float xrot, yrot, zrot;
            public float zoom, Per;
        }
    }
}
