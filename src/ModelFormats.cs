using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using System.Numerics;
using UpkManager.Models.UpkFile.Engine;
using static MHUpkManager.ModelViewForm;

namespace MHUpkManager
{
    public class ModelFormats
    {
        public static void ExportToDAE(string fileName, SkeletalMeshData model)
        {
            using var writer = new StreamWriter(fileName);
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            writer.WriteLine("<COLLADA xmlns=\"http://www.collada.org/2005/11/COLLADASchema\" version=\"1.4.1\">");
            writer.WriteLine("<asset><unit name=\"meter\" meter=\"1\"/><up_axis>Z_UP</up_axis></asset>");

            writer.WriteLine("<library_geometries><geometry id=\"mesh\" name=\"mesh\">");
            writer.WriteLine("<mesh>");

            // Positions
            writer.WriteLine("<source id=\"positions\">");
            writer.WriteLine("<float_array id=\"positions-array\" count=\"" + model.Vertices.Length * 3 + "\">");
            foreach (var v in model.Vertices)
                writer.Write($"{v.Position.X} {v.Position.Y} {v.Position.Z} ");
            writer.WriteLine("</float_array>");
            writer.WriteLine("<technique_common><accessor source=\"#positions-array\" count=\"" + model.Vertices.Length + "\" stride=\"3\">");
            writer.WriteLine("<param name=\"X\" type=\"float\"/><param name=\"Y\" type=\"float\"/><param name=\"Z\" type=\"float\"/>");
            writer.WriteLine("</accessor></technique_common></source>");

            // Normals
            writer.WriteLine("<source id=\"normals\">");
            writer.WriteLine("<float_array id=\"normals-array\" count=\"" + model.Vertices.Length * 3 + "\">");
            foreach (var v in model.Vertices)
                writer.Write($"{v.Normal.X} {v.Normal.Y} {v.Normal.Z} ");
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

        public static void ExportToOBJ(string fileName, SkeletalMeshData model)
        {
            using var writer = new StreamWriter(fileName);

            writer.WriteLine("# Exported OBJ");

            foreach (var v in model.Vertices)
            {
                writer.WriteLine($"v {v.Position.X} {v.Position.Y} {v.Position.Z}");
                writer.WriteLine($"vt {v.TexCoord.X} {1.0f - v.TexCoord.Y}"); // flip Y?
                writer.WriteLine($"vn {v.Normal.X} {v.Normal.Y} {v.Normal.Z}");
            }

            for (int i = 0; i < model.Indices.Length; i += 3)
            {
                int i1 = model.Indices[i] + 1;
                int i2 = model.Indices[i + 1] + 1;
                int i3 = model.Indices[i + 2] + 1;
                writer.WriteLine($"f {i1}/{i1}/{i1} {i2}/{i2}/{i2} {i3}/{i3}/{i3}");
            }
        }

        public static void ExportToGLTF(string filename, SkeletalMeshData model, bool gltf)
        {
            if (model.Vertices == null || model.Indices == null || model.Indices.Length % 3 != 0)
                return;

            var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>("Mesh");

            var primitive = meshBuilder.UsePrimitive(new MaterialBuilder("Default"));

            var vertexBuilders = model.Vertices.Select(v =>
                new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                    ToVertexPositionNormal(v),
                    ToVertexTexture1(v),
                    new VertexEmpty())
            ).ToArray();

            for (int i = 0; i < model.Indices.Length; i += 3)
            {
                var idx0 = model.Indices[i];
                var idx1 = model.Indices[i + 1];
                var idx2 = model.Indices[i + 2];

                primitive.AddTriangle(vertexBuilders[idx0], vertexBuilders[idx1], vertexBuilders[idx2]);
            }

            var scene = new SharpGLTF.Scenes.SceneBuilder();
            scene.AddRigidMesh(meshBuilder, Matrix4x4.CreateRotationX(-MathF.PI / 2));
            var modelRoot = scene.ToGltf2();

            if (gltf)
                modelRoot.SaveGLTF(filename);
            else
                modelRoot.SaveGLB(filename);
        }

        private static VertexTexture1 ToVertexTexture1(GLVertex v)
        {
            return new VertexTexture1(new Vector2(v.TexCoord.X, 1.0f - v.TexCoord.Y));
        }

        private static VertexPositionNormal ToVertexPositionNormal(GLVertex v)
        {
            var pos = v.Position;
            var norm = SafeNormal(v.Normal);

            return new VertexPositionNormal(pos, norm);
        }

        private static Vector3 SafeNormal(Vector3 n)
        {
            if (float.IsNaN(n.X) || float.IsNaN(n.Y) || float.IsNaN(n.Z) ||
                float.IsInfinity(n.X) || float.IsInfinity(n.Y) || float.IsInfinity(n.Z))
                return Vector3.UnitY;

            if (n.LengthSquared() < 1e-5f) 
                return Vector3.UnitY;

            return Vector3.Normalize(n);
        }
    }
}
