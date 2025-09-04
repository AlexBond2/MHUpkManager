using SharpGL;
using SharpGL.Shaders;
using SharpGLTF.Schema2;
using System.Numerics;
using System.Runtime.InteropServices;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Engine.Material;
using UpkManager.Models.UpkFile.Engine.Mesh;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;
using static MHUpkManager.Model.GLLib;

namespace MHUpkManager.Model
{
    public class ModelMesh
    {
        public UObject Mesh;
        public string ModelName;
        public Vector3 Center;
        public float Radius;

        public int[] Indices;
        public GLVertex[] Vertices;
        public List<Bone> Bones;

        public List<MeshSectionData> Sections;

        public ModelMesh(UObject obj, string name, OpenGL gl)
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

            InitModelBuffers(gl);

        }

        public uint iboId;
        public uint vboId;
        public uint vaoId;

        private void InitModelBuffers(OpenGL gl)
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

            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Position);
            gl.EnableVertexAttribArray(0);

            gl.VertexAttribPointer(1, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Normal);
            gl.EnableVertexAttribArray(1);

            gl.VertexAttribPointer(2, 2, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.TexCoord);
            gl.EnableVertexAttribArray(2);

            gl.VertexAttribPointer(3, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Tangent);
            gl.EnableVertexAttribArray(3);

            gl.VertexAttribPointer(4, 3, OpenGL.GL_FLOAT, false, stride, GLVertexOffsets.Bitangent);
            gl.EnableVertexAttribArray(4);

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

        public void DisposeBuffers(OpenGL gl)
        {
            DisposeModelBuffers(gl);
            DisposeBoneBuffers(gl);
            DisposeLineBuffers(gl);
        }

        public void DisposeModelBuffers(OpenGL gl)
        {
            if (vaoId == 0) return;
            gl.DeleteBuffers(2, [vboId, iboId]);
            gl.DeleteVertexArrays(1, [vaoId]);
        }

        public uint BonePointVAO { get; private set; }
        public uint BonePointVBO { get; private set; }
        public uint BoneLineVAO { get; private set; }
        public uint BoneLineVBO { get; private set; }
        public int BonePointCount { get; private set; }
        public int BoneLineCount { get; private set; }
        public List<string> BoneNames { get; private set; } = [];
        public List<int> BoneNameIndices { get; private set; } = [];

        public bool BonesBuffersInitialized = false;

        public void InitializeBoneBuffers(OpenGL gl)
        {
            if (Bones == null || BonesBuffersInitialized) return;

            BonePointCount = 0;
            BoneLineCount = 0;
            BoneNames.Clear();
            BoneNameIndices.Clear();

            for (int i = 0; i < Bones.Count; i++)
            {
                var bone = Bones[i];
                if (bone.ParentIndex >= 0)
                {
                    BonePointCount++;
                    BoneLineCount += 2;

                    BoneNames.Add(bone.Name);
                    BoneNameIndices.Add(i);
                }
            }

            if (BonePointCount == 0) return;

            uint[] vaos = new uint[2];
            uint[] vbos = new uint[2];
            gl.GenVertexArrays(2, vaos);
            gl.GenBuffers(2, vbos);

            BonePointVAO = vaos[0];
            BonePointVBO = vbos[0];
            BoneLineVAO = vaos[1];
            BoneLineVBO = vbos[1];

            gl.BindVertexArray(BonePointVAO);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, BonePointVBO);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, BonePointCount * 3 * sizeof(float), IntPtr.Zero, OpenGL.GL_DYNAMIC_DRAW);
            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
            gl.EnableVertexAttribArray(0);

            gl.BindVertexArray(BoneLineVAO);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, BoneLineVBO);
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, BoneLineCount * 3 * sizeof(float), IntPtr.Zero, OpenGL.GL_DYNAMIC_DRAW);
            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 0, IntPtr.Zero);
            gl.EnableVertexAttribArray(0);

            gl.BindVertexArray(0);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);

            BonesBuffersInitialized = true;
        }

        public void UpdateBoneBuffers(OpenGL gl)
        {
            if (Bones == null || !BonesBuffersInitialized) return;

            List<Vector3> pointVertices = [];
            List<Vector3> lineVertices = [];

            for (int i = 0; i < Bones.Count; i++)
            {
                var bone = Bones[i];
                if (bone.ParentIndex >= 0)
                {
                    var to = bone.GlobalTransform.Translation;
                    pointVertices.Add(to);

                    var parent = Bones[bone.ParentIndex];
                    var from = parent.GlobalTransform.Translation;
                    lineVertices.Add(from);
                    lineVertices.Add(to);
                }
            }

            if (pointVertices.Count > 0)
                BindVertexBuffer(gl, BonePointVBO, sizeof(float), OpenGL.GL_STATIC_DRAW, Vector3ToFloatArray(pointVertices));

            if (lineVertices.Count > 0)
                BindVertexBuffer(gl, BoneLineVBO, sizeof(float), OpenGL.GL_STATIC_DRAW, Vector3ToFloatArray(lineVertices));

            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);
        }

        public void DisposeBoneBuffers(OpenGL gl)
        {
            if (!BonesBuffersInitialized) return;

            gl.DeleteBuffers(2, [BonePointVBO, BoneLineVBO]);
            gl.DeleteVertexArrays(2, [BonePointVAO, BoneLineVAO]);

            BonesBuffersInitialized = false;
        }

        public uint nlvao;
        private uint nlvbo;
        public int nlCount;
        public uint ntvao;
        private uint ntvbo;
        public int ntCount;

        public void DisposeLineBuffers(OpenGL gl)
        {
            if (nlvao != 0)
            {
                gl.DeleteBuffers(1, [nlvbo]);
                gl.DeleteVertexArrays(1, [nlvao]);
                nlvao = 0;
            }

            if (ntvao != 0)
            {
                gl.DeleteBuffers(1, [ntvbo]);
                gl.DeleteVertexArrays(1, [ntvao]);
                ntvao = 0;
            }
        }

        public void PrepareLines(OpenGL gl, int type)
        {
            uint[] vaos = new uint[1];
            gl.GenVertexArrays(1, vaos);
            uint vaoId = vaos[0];

            uint[] vbos = new uint[1];
            gl.GenBuffers(1, vbos);
            uint vboId = vbos[0];

            List<float> lines = [];

            foreach (var section in Sections)
            {
                uint start = section.BaseIndex;
                uint end = start + section.NumTriangles * 3;
                for (uint i = start; i < end; i++)
                {
                    var vertex = Vertices[Indices[i]];

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
                nlvao = vaoId;
                nlvbo = vboId;
                nlCount = count;
            }
            else
            {
                ntvao = vaoId;
                ntvbo = vboId;
                ntCount = count;
            }
        }

        private static float[] Vector3ToFloatArray(List<Vector3> vectors)
        {
            float[] result = new float[vectors.Count * 3];
            for (int i = 0; i < vectors.Count; i++)
            {
                result[i * 3] = vectors[i].X;
                result[i * 3 + 1] = vectors[i].Y;
                result[i * 3 + 2] = vectors[i].Z;
            }
            return result;
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
            TextureId = BindGLTexture(gl, textureObj, out MipIndex, out Texture2D, out Data);
        }
    }

    public enum TextureType
    {
        uDiffuseMap,
        uNormalMap,
        uSMSPSKMap,
        uSpecColorMap,
        uESPAMap,
        uSMRRMap
    }

    public class MaterialParameters
    {
        // Scalar Parameters
        public float LambertDiffusePower = 1.0f;
        public float PhongDiffusePower = 1.0f;
        public float LightingAmbient = 0.3f;
        public float ShadowAmbientMult = 1.0f;
        public float NormalStrength = 1.0f;
        public float ReflectionMult = 1.0f;
        public float RimColorMult = 0.0f;
        public float RimFalloff = 2.0f;
        public float ScreenLightAmount = 0.0f;
        public float ScreenLightMult = 1.0f;
        public float ScreenLightPower = 1.0f;
        public float SpecMult = 1.0f;
        public float SpecMultLQ = 0.5f;
        public float SpecularPower = 15.0f;
        public float SpecularPowerMask = 1.0f;

        // Vector Parameters
        public Vector3 LambertAmbient = new (0.1f, 0.1f, 0.1f);
        public Vector3 ShadowAmbientColor = new (0.05f, 0.05f, 0.08f);
        public Vector3 FillLightColor = new (0.2f, 0.19f, 0.18f);
        public Vector3 SpecularColor = new (0.502f);

        // Subsurface Scattering
        public Vector3 SubsurfaceInscatteringColor = new (1.0f, 1.0f, 1.0f);
        public Vector3 SubsurfaceAbsorptionColor = new (0.902f, 0.784f, 0.784f);
        public float ImageReflectionNormalDampening = 5.0f;
        public float SkinScatterStrength = 0.5f;
        public float TwoSidedLighting = 0.0f;

        public void LoadFromMaterial(UMaterialInstanceConstant material)
        {
            LoadScalarParameter(material, "lambertdiffusepower", ref LambertDiffusePower);
            LoadScalarParameter(material, "lightingambient", ref LightingAmbient);
            LoadScalarParameter(material, "phongdiffusepower", ref PhongDiffusePower);
            LoadScalarParameter(material, "shadowambientmult", ref ShadowAmbientMult);
            LoadScalarParameter(material, "normalstrength", ref NormalStrength);

            // TODO normalize values

            // LoadScalarParameter(material, "reflectionmult", ref ReflectionMult); // 10
            // LoadScalarParameter(material, "rimcolormult", ref RimColorMult); // 5
            /* 
            LoadScalarParameter(material, "rimfalloff", ref RimFalloff);
            LoadScalarParameter(material, "screenlight_amount", ref ScreenLightAmount);
            LoadScalarParameter(material, "screenlight_mult", ref ScreenLightMult);
            //LoadScalarParameter(material, "screenlight_power", ref ScreenLightPower); // 4
            LoadScalarParameter(material, "specmult", ref SpecMult);
            LoadScalarParameter(material, "specmult_lq", ref SpecMultLQ);
            LoadScalarParameter(material, "specularpower", ref SpecularPower);
            */
            //LoadScalarParameter(material, "specularpowermask", ref SpecularPowerMask); // 255
            
            LoadVectorParameter(material, "lambertambient", ref LambertAmbient);
            LoadVectorParameter(material, "shadowambientcolor", ref ShadowAmbientColor);
            LoadVectorParameter(material, "filllightcolor", ref FillLightColor);
            LoadVectorParameter(material, "specularcolor", ref SpecularColor);            
        }

        private static void LoadScalarParameter(UMaterialInstanceConstant material, string paramName, ref float value)
        {
            var param = material.GetScalarParameterValue(paramName);
            if (param.HasValue) value = param.Value;
        }

        private static void LoadVectorParameter(UMaterialInstanceConstant material, string paramName, ref Vector3 value)
        {
            var param = material.GetVectorParameterValue(paramName);
            if (param.HasValue) value = param.Value;
        }

        public void ApplyToShader(ShaderProgram shader)
        {
            shader.SetFloat("uLambertDiffusePower", LambertDiffusePower);
            shader.SetFloat("uPhongDiffusePower", PhongDiffusePower);
            shader.SetFloat("uLightingAmbient", LightingAmbient);
            shader.SetFloat("uShadowAmbientMult", ShadowAmbientMult);
            shader.SetFloat("uNormalStrength", NormalStrength);
            shader.SetFloat("uReflectionMult", ReflectionMult);
            shader.SetFloat("uRimColorMult", RimColorMult);
            shader.SetFloat("uRimFalloff", RimFalloff);
            shader.SetFloat("uScreenLightAmount", ScreenLightAmount);
            shader.SetFloat("uScreenLightMult", ScreenLightMult);
            shader.SetFloat("uScreenLightPower", ScreenLightPower);
            shader.SetFloat("uSpecMult", SpecMult);
            shader.SetFloat("uSpecMultLQ", SpecMultLQ);
            shader.SetFloat("uSpecularPower", SpecularPower);
            shader.SetFloat("uSpecularPowerMask", SpecularPowerMask);

            shader.SetVector3("uLambertAmbient", LambertAmbient);
            shader.SetVector3("uShadowAmbientColor", ShadowAmbientColor);
            shader.SetVector3("uFillLightColor", FillLightColor);
            shader.SetVector3("uSpecularColor", SpecularColor);

            shader.SetVector3("uSubsurfaceInscatteringColor", SubsurfaceInscatteringColor);
            shader.SetVector3("uSubsurfaceAbsorptionColor", SubsurfaceAbsorptionColor);
            shader.SetFloat("uImageReflectionNormalDampening", ImageReflectionNormalDampening);
            shader.SetFloat("uSkinScatterStrength", SkinScatterStrength);
            shader.SetFloat("uTwoSidedLighting", TwoSidedLighting);
        }
    }

    public struct MeshSectionData
    {
        public uint BaseIndex;
        public uint NumTriangles;

        public UMaterialInstanceConstant Material;
        public int MaterialIndex;
        public List<Texture2DData> Textures;
        public MaterialParameters Parameters;

        public void LoadMaterial(OpenGL gl, FObject material)
        {
            Textures = [];
            Material = material?.LoadObject<UMaterialInstanceConstant>();

            Parameters = new();
            if (Material == null) return;

            Parameters.LoadFromMaterial(Material);

            LoadTexture(gl, "Diffuse", TextureType.uDiffuseMap);
            LoadTexture(gl, "Norm", TextureType.uNormalMap);
            LoadTexture(gl, "specmult_specpow_skinmask", TextureType.uSMSPSKMap);
            LoadTexture(gl, "emissivespecpow", TextureType.uESPAMap);
            LoadTexture(gl, "specmultrimmaskrefl", TextureType.uSMRRMap);
            LoadTexture(gl, "SpecColor", TextureType.uSpecColorMap);
        }

        private readonly void LoadTexture(OpenGL gl, string parameterName, TextureType textureType)
        {
            var textureObj = Material.GetTextureParameterValue(parameterName);
            if (textureObj?.LoadObject<UTexture2D>() != null)
                Textures.Add(new Texture2DData(textureType, gl, textureObj));
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

        public void ApplyToShader(OpenGL gl, ShaderProgram sh, bool showTextures)
        {
            Parameters.ApplyToShader(sh);

            BindTexture(gl, sh, OpenGL.GL_TEXTURE0, TextureType.uDiffuseMap,
                               "uDiffuseMap", "uHasDiffuseMap", showTextures);

            BindTexture(gl, sh, OpenGL.GL_TEXTURE1, TextureType.uNormalMap,
                               "uNormalMap", "uHasNormalMap");

            BindTexture(gl, sh, OpenGL.GL_TEXTURE2, TextureType.uSMSPSKMap,
                               "uSMSPSKMap", "uHasSMSPSK");

            BindTexture(gl, sh, OpenGL.GL_TEXTURE3, TextureType.uESPAMap,
                               "uESPAMap", "uHasESPA");

            BindTexture(gl, sh, OpenGL.GL_TEXTURE4, TextureType.uSMRRMap,
                               "uSMRRMap", "uHasSMRR");

            BindTexture(gl, sh, OpenGL.GL_TEXTURE5, TextureType.uSpecColorMap,
                               "uSpecColorMap", "uHasSpecColorMap");
        }

        private readonly void BindTexture(OpenGL gl, ShaderProgram sh, uint textureUnit,
                                 TextureType textureType, string samplerName,
                                 string flagName, bool enabled = true)
        {
            if (enabled && GetTextureType(textureType, out var texture))
            {
                gl.ActiveTexture(textureUnit);
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.TextureId);
                sh.SetInt(samplerName, (int)(textureUnit - OpenGL.GL_TEXTURE0));
                sh.SetFloat(flagName, 1.0f);
            }
            else
            {
                sh.SetFloat(flagName, 0.0f);
            }
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
        public static readonly IntPtr Bitangent = GetOffset(nameof(GLVertex.Bitangent));

        private static IntPtr GetOffset(string fieldName)
        {
            return new IntPtr(Marshal.OffsetOf(typeof(GLVertex), fieldName).ToInt32());
        }
    }
}
