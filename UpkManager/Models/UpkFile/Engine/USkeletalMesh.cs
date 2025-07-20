using System;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("SkeletalMesh")]
    public class USkeletalMesh: UObject
    {
        [PropertyField]
        public bool bHasVertexColors { get; set; }

        [TreeNodeField]
        public BoxSphereBounds Bounds { get; set; }

        [TreeNodeField("UMaterialInterface")]
        public UArray<FName> Materials { get; set; } // UMaterialInterface

        [TreeNodeField]
        public Vector Origin { get; set; }

        [TreeNodeField]
        public Rotator RotOrigin { get; set; }

        [TreeNodeField("MeshBone")]
        public UArray<MeshBone> RefSkeleton { get; set; }

        [TreeNodeField]
        public int SkeletalDepth { get; set; }

        [TreeNodeField("StaticLODModel")]
        public UArray<StaticLODModel> LODModels { get; set; }

        [TreeNodeField]
        public UMap<FName, int> NameIndexMap { get; set; }

        [TreeNodeField("PerPolyBoneCollisionData")]
        public UArray<PerPolyBoneCollisionData> PerPolyBoneKDOPs { get; set; }

        [TreeNodeField("BoneBreakName")]
        public UArray<string> BoneBreakNames { get; set; }

        [TreeNodeField("Index")]
        public byte[] BoneBreakOptions { get; set; }

        [TreeNodeField("UApexClothingAsset")]
        public UArray<FName> ClothingAssets { get; set; } // UApexClothingAsset

        [TreeNodeField("TexelRatio")]
        public UArray<float> CachedStreamingTextureFactors { get; set; }

        [TreeNodeField]
        public SkeletalMeshSourceData SourceData { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            Bounds = BoxSphereBounds.ReadData(buffer);
            Materials = buffer.ReadArray(UBuffer.ReadObject);
            Origin = Vector.ReadData(buffer);
            RotOrigin = Rotator.ReadData(buffer);
            RefSkeleton = buffer.ReadArray(MeshBone.ReadData);
            SkeletalDepth = buffer.ReadInt32();

            LODModels = ReadLODModels(buffer);

            NameIndexMap = buffer.ReadMap<FName, int>(UName.ReadName, UBuffer.ReadInt32);
            PerPolyBoneKDOPs = buffer.ReadArray(PerPolyBoneCollisionData.ReadData);
            BoneBreakNames = buffer.ReadArray(UBuffer.ReadString);
            BoneBreakOptions = buffer.ReadBytes();

            ClothingAssets = buffer.ReadArray(UBuffer.ReadObject);
            CachedStreamingTextureFactors = buffer.ReadArray(UBuffer.ReadFloat);
            SourceData = SkeletalMeshSourceData.ReadData(buffer, this);
        }

        public UArray<StaticLODModel> ReadLODModels(UBuffer buffer)
        {
            int count = buffer.Reader.ReadInt32();
            var array = new UArray<StaticLODModel>(count);
            for (int i = 0; i < count; i++)
                array.Add(StaticLODModel.ReadData(buffer, this));

            return array;
        }
    }

    public class SkeletalMeshSourceData : IAtomicStruct
    {
        [StructField]
        public bool bHaveSourceData { get; set; }

        [StructField]
        public StaticLODModel LODModel { get; set; }

        public static SkeletalMeshSourceData ReadData(UBuffer buffer, USkeletalMesh mesh)
        {
            var data = new SkeletalMeshSourceData();

            data.bHaveSourceData = buffer.ReadBool();
            if (data.bHaveSourceData)
                data.LODModel = StaticLODModel.ReadData(buffer, mesh);

            return data;
        }

        public string Format => "";
    }

    public class PerPolyBoneCollisionData : IAtomicStruct
    {
        [StructField]
        public SkeletalKDOPTreeLegacy LegacykDOPTree { get; set; }

        [StructField]
        public UArray<Vector> CollisionVerts { get; set; }

        public static PerPolyBoneCollisionData ReadData(UBuffer buffer)
        {
            return new PerPolyBoneCollisionData
            {
                LegacykDOPTree = SkeletalKDOPTreeLegacy.ReadData(buffer),
                CollisionVerts = buffer.ReadArray(Vector.ReadData)
            };
        }

        public string Format => "";
    }

    public class SkeletalKDOPTreeLegacy : IAtomicStruct
    {
        [StructField]
        public UArray<byte[]> Nodes { get; set; } // FkDOPCollisionTriangle

        [StructField]
        public UArray<byte[]> Triangles { get; set; } // FSkelMeshCollisionDataProvider

        public static SkeletalKDOPTreeLegacy ReadData(UBuffer buffer)
        {
            return new SkeletalKDOPTreeLegacy
            {
                Nodes = buffer.ReadArrayUnkElement(),
                Triangles = buffer.ReadArrayUnkElement()
            };
        }

        public string Format => "";
    }

    public class StaticLODModel : IAtomicStruct
    {
        [StructField]
        public UArray<SkelMeshSection> Sections { get; set; }

        [StructField]
        public MultiSizeIndexContainer MultiSizeIndexContainer { get; set; }

        [StructField]
        public UArray<ushort> ActiveBoneIndices { get; set; }

        [StructField]
        public UArray<SkelMeshChunk> Chunks { get; set; }

        [StructField]
        public uint Size { get; set; }

        [StructField]
        public uint NumVertices { get; set; }

        [StructField]
        public byte[] RequiredBones { get; set; }

        [StructField]
        public byte[] RawPointIndices { get; set; } // FIntBulkData

        [StructField]
        public uint NumTexCoords { get; set; }

        [StructField]
        public SkeletalMeshVertexBuffer VertexBufferGPUSkin { get; set; }

        [StructField]
        public SkeletalMeshVertexColorBuffer ColorVertexBuffer { get; set; }

        [StructField]
        public UArray<SkeletalMeshVertexInfluences> VertexInfluences { get; set; }

        [StructField]
        public MultiSizeIndexContainer AdjacencyMultiSizeIndexContainer { get; set; }

        public string Format => "";

        public static StaticLODModel ReadData(UBuffer buffer, USkeletalMesh mesh)
        {
            StaticLODModel lod = new StaticLODModel();
            lod.Sections = buffer.ReadArray(SkelMeshSection.ReadData);
            lod.MultiSizeIndexContainer = MultiSizeIndexContainer.ReadData(buffer);
            lod.ActiveBoneIndices = buffer.ReadArray(UBuffer.ReadUInt16);
            lod.Chunks = buffer.ReadArray(SkelMeshChunk.ReadData);
            lod.Size = buffer.Reader.ReadUInt32();
            lod.NumVertices = buffer.Reader.ReadUInt32();

            lod.RequiredBones = buffer.ReadBytes();
            lod.RawPointIndices = buffer.ReadBulkData();

            lod.NumTexCoords = buffer.Reader.ReadUInt32();
            
            lod.VertexBufferGPUSkin = SkeletalMeshVertexBuffer.ReadData(buffer);

            if (mesh.bHasVertexColors)
                lod.ColorVertexBuffer = SkeletalMeshVertexColorBuffer.ReadData(buffer);

            lod.VertexInfluences = buffer.ReadArray(SkeletalMeshVertexInfluences.ReadData);
            lod.AdjacencyMultiSizeIndexContainer = MultiSizeIndexContainer.ReadData(buffer);
            
            return lod;
        }
    }

    public class VertexBuffer : IAtomicStruct
    {
        public string Format => "";
    }

    public class SkeletalMeshVertexInfluences : VertexBuffer
    {
        [StructField]
        public UArray<VertexInfluence> Influences { get; set; }

        [StructField]
        public UMap<BoneIndexPair, UArray<uint>> VertexInfluenceMapping { get; set; }

        [StructField]
        public UArray<SkelMeshSection> Sections { get; set; }

        [StructField]
        public UArray<SkelMeshChunk> Chunks { get; set; }

        [StructField]
        public byte[] RequiredBones { get; set; }

        [StructField]
        public byte Usage { get; set; }

        public static SkeletalMeshVertexInfluences ReadData(UBuffer buffer)
        {
            return new()
            {
                Influences = buffer.ReadArray(VertexInfluence.ReadData),
                VertexInfluenceMapping = buffer.ReadMap(BoneIndexPair.ReadKeys, UBuffer.ReadArrayUInt32),
                Sections = buffer.ReadArray(SkelMeshSection.ReadData),
                Chunks = buffer.ReadArray(SkelMeshChunk.ReadData),
                RequiredBones = buffer.ReadBytes(),
                Usage = buffer.Reader.ReadByte()
            };
        }
    }

    public struct BoneIndexPair(int index0, int index1)
    {
        public int BoneInd0 = index0;
        public int BoneInd1 = index1;

        public static BoneIndexPair ReadKeys(UBuffer buffer)
        {
            return new(buffer.Reader.ReadInt32(), buffer.Reader.ReadInt32());
        }
    }

    public class VertexInfluence : IAtomicStruct
    {
        [StructField]
        public InfluenceBones Bones { get; set; }

        [StructField]
        public InfluenceWeights Weights { get; set; }

        public string Format => "";

        public static VertexInfluence ReadData(UBuffer buffer)
        {
            return new VertexInfluence
            {
                Bones = InfluenceBones.ReadData(buffer),
                Weights = InfluenceWeights.ReadData(buffer)
            };
        }
    }

    public class InfluenceBones : IAtomicStruct
    {
        [StructField]
        public byte[] Bones { get; set; }
        public static InfluenceBones ReadData(UBuffer buffer)
        {
            return new InfluenceBones
            {
                Bones = buffer.Read4Bytes()
            };
        }
        public string Format => "";
    }

    public class InfluenceWeights : IAtomicStruct
    {
        [StructField]
        public byte[] Weights { get; set; }
        public static InfluenceWeights ReadData(UBuffer buffer)
        {
            return new InfluenceWeights
            {
                Weights = buffer.Read4Bytes()
            };
        }
        public string Format => "";
    }

    public class SkeletalMeshVertexColorBuffer : VertexBuffer
    {
        public UArray<GPUSkinVertexColor> Colors { get; set; }

        public static SkeletalMeshVertexColorBuffer ReadData(UBuffer buffer)
        {
            SkeletalMeshVertexColorBuffer vertexBuffer = new();
            vertexBuffer.Colors = buffer.ReadArrayElement(GPUSkinVertexColor.ReadData, 4);
            return vertexBuffer;
        }
    }

    public class SkeletalMeshVertexBuffer : VertexBuffer
    {
        [StructField]
        public uint NumTexCoords { get; set; }

        [StructField]
        public bool bUseFullPrecisionUVs { get; set; }

        [StructField]
        public bool bUsePackedPosition { get; set; }

        [StructField]
        public Vector MeshExtension { get; set; }

        [StructField]
        public Vector MeshOrigin { get; set; }

        public UArray<GPUSkinVertexFloat16Uvs32Xyz> VertsF16UV32 { get; set; }
        public UArray<GPUSkinVertexFloat16Uvs> VertsF16 { get; set; }
        public UArray<GPUSkinVertexFloat32Uvs32Xyz> VertsF32UV32 { get; set; }
        public UArray<GPUSkinVertexFloat32Uvs> VertsF32 { get; set; }

        public static SkeletalMeshVertexBuffer ReadData(UBuffer buffer)
        {
            SkeletalMeshVertexBuffer vertexBuffer = new()
            {
                NumTexCoords = buffer.Reader.ReadUInt32(),
                bUseFullPrecisionUVs = buffer.Reader.ReadBool(),
                bUsePackedPosition = buffer.Reader.ReadBool(),
                MeshExtension = Vector.ReadData(buffer),
                MeshOrigin = Vector.ReadData(buffer)
            };

            // Forced value for PC
            vertexBuffer.bUsePackedPosition = false;

            if (!vertexBuffer.bUseFullPrecisionUVs)
            {
                if (vertexBuffer.bUsePackedPosition)
                    vertexBuffer.VertsF16UV32 = ReadArrayElement<GPUSkinVertexFloat16Uvs32Xyz>(buffer, vertexBuffer.NumTexCoords);
                else
                    vertexBuffer.VertsF16 = ReadArrayElement<GPUSkinVertexFloat16Uvs>(buffer, vertexBuffer.NumTexCoords);
            }
            else
            {
                if (vertexBuffer.bUsePackedPosition)
                    vertexBuffer.VertsF32UV32 = ReadArrayElement<GPUSkinVertexFloat32Uvs32Xyz>(buffer, vertexBuffer.NumTexCoords);
                else
                    vertexBuffer.VertsF32 = ReadArrayElement<GPUSkinVertexFloat32Uvs>(buffer, vertexBuffer.NumTexCoords);
            }
            return vertexBuffer;
        }

        private static UArray<T> ReadArrayElement<T>(UBuffer buffer, uint numTexCoords) 
            where T : GPUSkinVertexBase, new()
        {
            T readMethod()
            {
                T vertex = new();
                vertex.ReadData(buffer, (int)numTexCoords);
                return vertex;
            }
            int sizeElement = buffer.Reader.ReadInt32();
            int expectedSize = GetExpectedSize<T>(numTexCoords);
            if (sizeElement != expectedSize)
                throw new InvalidOperationException($"Element size mismatch: serialized = {sizeElement}, expected = {expectedSize}, type = {typeof(T).Name}");
            int count = buffer.Reader.ReadInt32();
            var array = new UArray<T>(count);
            for (int i = 0; i < count; i++)
                array.Add(readMethod());

            return array;
        }

        private static int GetExpectedSize<T>(uint numTexCoords)
        {
            if (typeof(T) == typeof(GPUSkinVertexFloat16Uvs))
                return 16 + 12 + 2 * 2 * (int)numTexCoords; // 32
            if (typeof(T) == typeof(GPUSkinVertexFloat16Uvs32Xyz))
                return 16 + 4 + 2 * 2 * (int)numTexCoords; // 24
            if (typeof(T) == typeof(GPUSkinVertexFloat32Uvs))
                return 16 + 12 + 4 * 2 * (int)numTexCoords; // 40
            if (typeof(T) == typeof(GPUSkinVertexFloat32Uvs32Xyz))
                return 16 + 4 + 4 * 2 * (int)numTexCoords; // 28

            return -1;
        }
    }

    public class GPUSkinVertexBase
    {
        [StructField]
        public PackedNormal TangentX { get; set; }

        [StructField]
        public PackedNormal TangentZ { get; set; }

        [StructField]
        public byte[] InfluenceBones { get; set; }

        [StructField]
        public byte[] InfluenceWeights { get; set; }

        public virtual void ReadData(UBuffer buffer, int num)
        {
            TangentX = PackedNormal.ReadData(buffer);
            TangentZ = PackedNormal.ReadData(buffer);
            InfluenceBones = buffer.Read4Bytes();
            InfluenceWeights = buffer.Read4Bytes();
        }

        public static Vector2D[] ReadUVs(UBuffer buffer, int num)
        {
            var verts = new Vector2D[num];
            for (int i = 0; i < num; i++)
                verts[i] = Vector2D.ReadData(buffer);

            return verts;
        }

        public static Vector2DHalf[] ReadHalfUVs(UBuffer buffer, int num)
        {
            var verts = new Vector2DHalf[num];
            for (int i = 0; i < num; i++)
                verts[i] = Vector2DHalf.ReadData(buffer);

            return verts;
        }
    }

    public class GPUSkinVertexColor : IAtomicStruct
    {
        [StructField]
        public Color VertexColor { get; set; }

        public string Format => VertexColor.Format;

        public static GPUSkinVertexColor ReadData(UBuffer buffer)
        {
            return new()
            {
                VertexColor = Color.ReadData(buffer)
            };
        }

        public override string ToString() => Format;
    }

    public class GPUSkinVertexFloat16Uvs32Xyz : GPUSkinVertexBase
    {
        [StructField]
        public PackedPosition Positon { get; set; }

        [StructField]
        public Vector2DHalf[] UVs { get; set; }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = PackedPosition.ReadData(buffer);
            UVs = ReadHalfUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class GPUSkinVertexFloat16Uvs : GPUSkinVertexBase
    {
        [StructField]
        public Vector Positon { get; set; }

        [StructField]
        public Vector2DHalf[] UVs { get; set; }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = Vector.ReadData(buffer);
            UVs = ReadHalfUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class GPUSkinVertexFloat32Uvs32Xyz : GPUSkinVertexBase
    {
        [StructField]
        public PackedPosition Positon { get; set; }

        [StructField]
        public Vector2D[] UVs { get; set; }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = PackedPosition.ReadData(buffer);
            UVs = ReadUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class GPUSkinVertexFloat32Uvs : GPUSkinVertexBase
    {
        [StructField]
        public Vector Positon { get; set; }

        [StructField]
        public Vector2D[] UVs { get; set; }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = Vector.ReadData(buffer);
            UVs = ReadUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class SkelMeshChunk : IAtomicStruct
    {
        [StructField]
        public uint BaseVertexIndex { get; set; }

        [StructField]
        public UArray<RigidSkinVertex> RigidVertices { get; set; }

        [StructField]
        public UArray<SoftSkinVertex> SoftVertices { get; set; }

        [StructField]
        public UArray<ushort> BoneMap { get; set; }

        [StructField]
        public int NumRigidVertices { get; set; }

        [StructField]
        public int NumSoftVertices { get; set; }

        [StructField]
        public int MaxBoneInfluences { get; set; }

        public string Format => "";

        public static SkelMeshChunk ReadData(UBuffer buffer)
        {
            SkelMeshChunk chunk = new()
            {
                BaseVertexIndex = buffer.Reader.ReadUInt32()
            };

            chunk.RigidVertices = buffer.ReadArray(RigidSkinVertex.ReadData);
            chunk.SoftVertices = buffer.ReadArray(SoftSkinVertex.ReadData);
            chunk.BoneMap = buffer.ReadArray(UBuffer.ReadUInt16);
            chunk.NumRigidVertices = buffer.Reader.ReadInt32();
            chunk.NumSoftVertices = buffer.Reader.ReadInt32();
            chunk.MaxBoneInfluences = buffer.Reader.ReadInt32();

            return chunk;
        }
    }

    public class RigidSkinVertex : IAtomicStruct
    {
        [StructField]
        public Vector Position { get; set; }

        [StructField]
        public PackedNormal TangentX { get; set; }

        [StructField]
        public PackedNormal TangentY { get; set; }

        [StructField]
        public PackedNormal TangentZ { get; set; }

        [StructField]
        public Vector2D[] UVs { get; set; }

        [StructField]
        public Color Color { get; set; }

        [StructField]
        public byte Bone { get; set; }

        public string Format => "";

        public static RigidSkinVertex ReadData(UBuffer buffer)
        {
            RigidSkinVertex vertex = new()
            {
                Position = Vector.ReadData(buffer),
                TangentX = PackedNormal.ReadData(buffer),
                TangentY = PackedNormal.ReadData(buffer),
                TangentZ = PackedNormal.ReadData(buffer),
                UVs = SoftSkinVertex.ReadUVs(buffer),
                Color = Color.ReadData(buffer),
                Bone = buffer.Reader.ReadByte()
            };

            return vertex;
        }
    }

    public class SoftSkinVertex : IAtomicStruct
    {
        [StructField]
        public Vector Position { get; set; }

        [StructField]
        public PackedNormal TangentX { get; set; }

        [StructField]
        public PackedNormal TangentY { get; set; }

        [StructField]
        public PackedNormal TangentZ { get; set; }

        [StructField]
        public Vector2D[] UVs { get; set; }

        [StructField]
        public Color Color { get; set; }

        [StructField]
        public byte[] InfluenceBones { get; set; }

        [StructField]
        public byte[] InfluenceWeights { get; set; }

        public string Format => "";

        public static SoftSkinVertex ReadData(UBuffer buffer)
        {
            SoftSkinVertex vertex = new()
            {
                Position = Vector.ReadData(buffer),
                TangentX = PackedNormal.ReadData(buffer),
                TangentY = PackedNormal.ReadData(buffer),
                TangentZ = PackedNormal.ReadData(buffer),
                UVs = ReadUVs(buffer),
                Color = Color.ReadData(buffer),
                InfluenceBones = buffer.Read4Bytes(),
                InfluenceWeights = buffer.Read4Bytes()
            };

            return vertex;
        }

        public static Vector2D[] ReadUVs(UBuffer buffer)
        {
            var verts = new Vector2D[4];
            for (int i = 0; i < 4; i++)
                verts[i] = Vector2D.ReadData(buffer);

            return verts;
        }
    }

    public class SkelMeshSection : IAtomicStruct
    {
        [StructField]
        public ushort MaterialIndex { get; set; }

        [StructField]
        public ushort ChunkIndex { get; set; }

        [StructField]
        public uint BaseIndex { get; set; }

        [StructField]
        public uint NumTriangles { get; set; }

        [StructField]
        public byte TriangleSorting { get; set; }

        public string Format => "";

        public static SkelMeshSection ReadData(UBuffer buffer)
        {
            SkelMeshSection section = new()
            {
                MaterialIndex = buffer.Reader.ReadUInt16(),
                ChunkIndex = buffer.Reader.ReadUInt16(),
                BaseIndex = buffer.Reader.ReadUInt32(),
                NumTriangles = buffer.Reader.ReadUInt32(),
                TriangleSorting = buffer.Reader.ReadByte()
            };

            return section;
        }
    }

    public class MultiSizeIndexContainer : IAtomicStruct
    {
        [StructField]
        public bool NeedsCPUAccess { get; set; }

        [StructField]
        public byte DataTypeSize { get; set; }

        [StructField]
        public UArray<uint> IndexBuffer { get; set; }

        public string Format => "";

        public static MultiSizeIndexContainer ReadData(UBuffer buffer)
        {
            MultiSizeIndexContainer container = new()
            {
                NeedsCPUAccess = buffer.Reader.ReadBool(),
                DataTypeSize = buffer.Reader.ReadByte()
            };

            if (container.DataTypeSize == 2)
            {
                Span<ushort> data = buffer.ReadBulkSpan<ushort>();
                container.IndexBuffer = new UArray<uint>(data.Length);
                for (int i = 0; i < data.Length; i++)
                    container.IndexBuffer.Add(data[i]);
            }
            else
            {
                container.IndexBuffer = [.. buffer.ReadBulkSpan<uint>().ToArray()];
            }

            return container;
        }
    }

    public class MeshBone // : IAtomicStruct
    {
        [StructField]
        public FName Name { get; set; }

        [StructField]
        public uint Flags { get; set; }

        [StructField]
        public VJointPos BonePos { get; set; }

        [StructField]
        public int NumChildren { get; set; }

        [StructField]
        public int ParentIndex { get; set; }

        [StructField]
        public Color BoneColor { get; set; }

        public static MeshBone ReadData(UBuffer buffer)
        {
            MeshBone bone = new()
            {
                Name = buffer.ReadNameIndex(),
                Flags = buffer.Reader.ReadUInt32(),
                BonePos = VJointPos.ReadData(buffer),
                NumChildren = buffer.ReadInt32(),
                ParentIndex = buffer.ReadInt32(),
                BoneColor = Color.ReadData(buffer)
            };
            return bone;
        }
        public string Format => "";

        public override string ToString()
        {
            return $"{Name} [{NumChildren}] [{ParentIndex}]";
        }
    }

    public class VJointPos //: IAtomicStruct
    {
        [StructField]
        public Quat Orientation { get; set; }

        [StructField]
        public Vector Position { get; private set; }

        public static VJointPos ReadData(UBuffer buffer)
        {
            VJointPos joint = new()
            {
                Orientation = Quat.ReadData(buffer),
                Position = Vector.ReadData(buffer)
            };
            return joint;
        }
        public string Format => "";
    }

}
