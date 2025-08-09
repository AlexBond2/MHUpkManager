using DDSLib;

using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Transforms;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;

using System.Numerics;
using System.Windows.Media.Imaging;

using UpkManager.Models.UpkFile.Engine.Mesh;
using UpkManager.Models.UpkFile.Engine.Texture;
using static MHUpkManager.ModelViewForm;

namespace MHUpkManager
{
    public class ModelFormats
    {
        public enum ExportFormat
        {
            GLTF,
            GLB,
            DAE,
            OBJ
        }

        public static void ExportToDAE(string fileName, ModelMeshData model)
        {
            using var writer = new StreamWriter(fileName);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine("<COLLADA xmlns=\"http://www.collada.org/2005/11/COLLADASchema\" version=\"1.4.1\">");
            writer.WriteLine("<asset><unit name=\"meter\" meter=\"1\"/><up_axis>Y_UP</up_axis></asset>");

            writer.WriteLine($"<library_geometries><geometry id=\"mesh\" name=\"{model.ModelName}\">");
            writer.WriteLine("<mesh>");

            // Positions
            writer.WriteLine("<source id=\"positions\">");
            writer.WriteLine("<float_array id=\"positions-array\" count=\"" + model.Vertices.Length * 3 + "\">");
            foreach (var v in model.Vertices)
                writer.Write($"{v.Position.X} {v.Position.Z} {v.Position.Y} "); // MH invert
            writer.WriteLine("</float_array>");
            writer.WriteLine("<technique_common><accessor source=\"#positions-array\" count=\"" + model.Vertices.Length + "\" stride=\"3\">");
            writer.WriteLine("<param name=\"X\" type=\"float\"/><param name=\"Y\" type=\"float\"/><param name=\"Z\" type=\"float\"/>");
            writer.WriteLine("</accessor></technique_common></source>");

            // Normals
            writer.WriteLine("<source id=\"normals\">");
            writer.WriteLine("<float_array id=\"normals-array\" count=\"" + model.Vertices.Length * 3 + "\">");
            foreach (var v in model.Vertices)
                writer.Write($"{v.Normal.X} {v.Normal.Z} {v.Normal.Y} "); // MH invert
            writer.WriteLine("</float_array>");
            writer.WriteLine("<technique_common><accessor source=\"#normals-array\" count=\"" + model.Vertices.Length + "\" stride=\"3\">");
            writer.WriteLine("<param name=\"X\" type=\"float\"/><param name=\"Y\" type=\"float\"/><param name=\"Z\" type=\"float\"/>");
            writer.WriteLine("</accessor></technique_common></source>");

            // UVs
            writer.WriteLine("<source id=\"uvs\">");
            writer.WriteLine("<float_array id=\"uvs-array\" count=\"" + model.Vertices.Length * 2 + "\">");
            foreach (var v in model.Vertices)
                writer.Write($"{v.TexCoord.X} {1.0f - v.TexCoord.Y} ");
            writer.WriteLine("</float_array>");
            writer.WriteLine("<technique_common><accessor source=\"#uvs-array\" count=\"" + model.Vertices.Length + "\" stride=\"2\">");
            writer.WriteLine("<param name=\"S\" type=\"float\"/><param name=\"T\" type=\"float\"/>");
            writer.WriteLine("</accessor></technique_common></source>");

            // Vertices
            writer.WriteLine("<vertices id=\"mesh-vertices\">");
            writer.WriteLine("<input semantic=\"POSITION\" source=\"#positions\"/>");
            writer.WriteLine("</vertices>");

            // Triangles
            writer.WriteLine($"<triangles count=\"{model.Indices.Length / 3}\">");
            writer.WriteLine("<input semantic=\"VERTEX\" source=\"#mesh-vertices\" offset=\"0\"/>");
            writer.WriteLine("<input semantic=\"NORMAL\" source=\"#normals\" offset=\"1\"/>");
            writer.WriteLine("<input semantic=\"TEXCOORD\" source=\"#uvs\" offset=\"2\" set=\"0\"/>");

            writer.Write("<p>");
            foreach (var i in model.Indices)
                writer.Write($"{i} {i} {i} ");
            writer.WriteLine("</p>");
            writer.WriteLine("</triangles>");

            writer.WriteLine("</mesh></geometry></library_geometries>");
            writer.WriteLine("<library_visual_scenes><visual_scene id=\"Scene\" name=\"Scene\">");
            writer.WriteLine("<node id=\"mesh-node\" name=\"mesh\" type=\"NODE\">");
            writer.WriteLine("<instance_geometry url=\"#mesh\"/>");
            writer.WriteLine("</node></visual_scene></library_visual_scenes>");
            writer.WriteLine("<scene><instance_visual_scene url=\"#Scene\"/></scene>");
            writer.WriteLine("</COLLADA>");
        }

        public static void ExportModel(string filename, ModelMeshData model, ExportFormat format)
        {
            if (model.Vertices == null || model.Indices == null || model.Indices.Length % 3 != 0)
                return;

            if (format == ExportFormat.DAE)
            {
                ExportToDAE(filename, model);
                return;
            }

            var scene = model.Bones?.Count > 0
                ? BuildSceneWithSkeleton(model)
                : BuildSceneRigid(model);

            var modelRoot = scene.ToGltf2();

            switch (format)
            {
                case ExportFormat.OBJ:
                    if (File.Exists(filename)) File.Delete(filename);
                    modelRoot.SaveAsWavefront(filename);
                    return;
                case ExportFormat.GLB:
                    modelRoot.SaveGLB(filename);
                    break;
                case ExportFormat.GLTF:
                    modelRoot.SaveGLTF(filename);
                    break;
                default:
                    break;
            }
        }

        private static SceneBuilder BuildSceneRigid(ModelMeshData model)
        {
            var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(model.ModelName);

            var vertexBuilders = model.Vertices.Select(v =>
                new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                    ToVertexPositionNormal(v),
                    ToVertexTexture1(v),
                    new VertexEmpty())
            ).ToArray();

            var materialCache = new Dictionary<int, MaterialBuilder>();

            foreach (var section in model.Sections)
            {
                var matBuilder = BuildMaterals(section, materialCache);
                var primitive = meshBuilder.UsePrimitive(matBuilder);

                uint start = section.BaseIndex;
                uint end = start + section.NumTriangles * 3;

                for (uint i = start; i < end; i += 3)
                {
                    var i0 = model.Indices[i];
                    var i1 = model.Indices[i + 1];
                    var i2 = model.Indices[i + 2];

                    primitive.AddTriangle(vertexBuilders[i0], vertexBuilders[i1], vertexBuilders[i2]);
                }
            }

            var scene = new SceneBuilder();
            scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
            return scene;
        }

        public static readonly Matrix4x4 MHInvert = new(
            1, 0, 0, 0,
            0, 0, 1, 0,
            0, 1, 0, 0,
            0, 0, 0, 1
        );

        private static SceneBuilder BuildSceneWithSkeleton(ModelMeshData model)
        {
            var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(model.ModelName);

            var vertexBuilders = model.Vertices.Select(v =>
            {
                var pos = ToVertexPositionNormal(v);
                var tex = ToVertexTexture1(v);

                var bindings = new[]
                {
                    ((int)v.Bone0, v.Weight0 / 255.0f),
                    ((int)v.Bone1, v.Weight1 / 255.0f),
                    ((int)v.Bone2, v.Weight2 / 255.0f),
                    ((int)v.Bone3, v.Weight3 / 255.0f)
                };

                var sparse = SparseWeight8.Create(bindings.Where(x => x.Item2 > 0).ToArray());
                var joints = new VertexJoints4(sparse);

                return new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                    pos, tex, joints);
            }).ToArray();

            var materialCache = new Dictionary<int, MaterialBuilder>();

            foreach (var section in model.Sections)
            {
                var matBuilder = BuildMaterals(section, materialCache);
                var primitive = meshBuilder.UsePrimitive(matBuilder);

                uint start = section.BaseIndex;
                uint end = start + section.NumTriangles * 3;

                for (uint i = start; i < end; i += 3)
                {
                    var i0 = model.Indices[i];
                    var i1 = model.Indices[i + 1];
                    var i2 = model.Indices[i + 2];

                    primitive.AddTriangle(vertexBuilders[i0], vertexBuilders[i1], vertexBuilders[i2]);
                }
            }

            var boneNodes = new List<NodeBuilder>(model.Bones.Count);

            for (int i = 0; i < model.Bones.Count; i++)
            {
                var bone = model.Bones[i];
                var localMat = MHInvert * bone.LocalTransform * MHInvert;
                var node = new NodeBuilder(bone.Name);

                var transform = new AffineTransform(localMat);
                var decomposed = transform.GetDecomposed();
                node.SetLocalTransform(decomposed, true);

                boneNodes.Add(node);
            }

            for (int i = 0; i < model.Bones.Count; i++)
            {
                int parent = model.Bones[i].ParentIndex;
                if (parent >= 0 && parent < boneNodes.Count && parent != i)
                    boneNodes[parent].AddNode(boneNodes[i]);
            }

            var inverseBindMatrices = new (NodeBuilder Joint, Matrix4x4 InverseBindMatrix)[model.Bones.Count];
            for (int i = 0; i < model.Bones.Count; i++)
            {
                var boneNode = boneNodes[i];

                Matrix4x4 gltfGlobalTransform = boneNode.WorldMatrix;
                Matrix4x4.Invert(gltfGlobalTransform, out var inverseBindMatrix);
                inverseBindMatrices[i] = (boneNodes[i], inverseBindMatrix);
            }

            var scene = new SceneBuilder();
            scene.AddSkinnedMesh(meshBuilder, inverseBindMatrices);

            return scene;
        }

        private static MaterialBuilder BuildMaterals(MeshSectionData section, Dictionary<int, MaterialBuilder> materialCache)
        {
            if (!materialCache.TryGetValue(section.MaterialIndex, out var matBuilder))
            {
                matBuilder = new MaterialBuilder($"Material_{section.MaterialIndex}");                

                if (section.GetTextureType(TextureType.uDiffuseMap, out var texture))
                {
                    var imageBuilder = CreateImageFromRgba(texture.Texture2D, texture.MipIndex, texture.Data);
                    matBuilder.WithChannelImage(KnownChannel.BaseColor, imageBuilder);
                }

                if (section.GetTextureType(TextureType.uNormalMap, out texture))
                {
                    var imageBuilder = CreateImageFromRgba(texture.Texture2D, texture.MipIndex, texture.Data);
                    matBuilder.WithChannelImage(KnownChannel.Normal, imageBuilder);
                }

                materialCache[section.MaterialIndex] = matBuilder;
            }

            return matBuilder;
        }

        private static ImageBuilder CreateImageFromRgba(UTexture2D texture, int mipIndex, byte[] textureData)
        {
            int width = texture.Mips[mipIndex].SizeX;

            var bitmapSource = new RgbaBitmapSource(textureData, width);
            MemoryStream outStream = new();

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(outStream);

            return ImageBuilder.From(new(outStream.ToArray()));
        }

        private static VertexTexture1 ToVertexTexture1(GLVertex v)
        {
            return new VertexTexture1(new Vector2(v.TexCoord.X, v.TexCoord.Y)); // No flip Y !!!
        }

        private static VertexPositionNormal ToVertexPositionNormal(GLVertex v)
        {
            var p = v.Position;
            var n = v.Normal;

            var pos = new Vector3(p.X, p.Z, p.Y); // MH invert
            var norm = new Vector3(n.X, n.Z, n.Y); // MH invert

            return new VertexPositionNormal(pos, norm);
        }

        public static ExportFormat GetExportFormat(string extension)
        {
            return extension switch
            {
                ".gltf" => ExportFormat.GLTF,
                ".glb" => ExportFormat.GLB,
                ".obj" => ExportFormat.OBJ,
                ".dae" => ExportFormat.DAE,
                _ => throw new NotSupportedException($"Unsupported file extension: {extension}")
            };
        }
    }
}
