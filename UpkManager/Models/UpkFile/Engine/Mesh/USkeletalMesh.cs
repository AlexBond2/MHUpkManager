using System;
using System.Collections.Generic;
using System.Numerics;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Mesh
{
    [UnrealClass("SkeletalMesh")]
    public class USkeletalMesh: UObject
    {
        [PropertyField]
        public bool bHasVertexColors { get; set; }

        [PropertyField]
        public UArray<FObject> Sockets { get; set; } // SkeletalMeshSocket

        [PropertyField]
        public UArray<FSkeletalMeshLODInfo> LODInfo { get; set; }

        [StructField]
        public FBoxSphereBounds Bounds { get; set; }

        [StructField("UMaterialInterface")]
        public UArray<FObject> Materials { get; set; } // UMaterialInterface

        [StructField]
        public FVector Origin { get; set; }

        [StructField]
        public FRotator RotOrigin { get; set; }

        [StructField("MeshBone")]
        public UArray<FMeshBone> RefSkeleton { get; set; }

        [StructField]
        public int SkeletalDepth { get; set; }

        [StructField("StaticLODModel")]
        public UArray<FStaticLODModel> LODModels { get; set; }

        [StructField]
        public UMap<FName, int> NameIndexMap { get; set; }

        [StructField("PerPolyBoneCollisionData")]
        public UArray<FPerPolyBoneCollisionData> PerPolyBoneKDOPs { get; set; }

        [StructField("BoneBreakName")]
        public UArray<string> BoneBreakNames { get; set; }

        [StructField("Index")]
        public byte[] BoneBreakOptions { get; set; }

        [PropertyField]
        [StructField("UApexClothingAsset")]
        public UArray<FObject> ClothingAssets { get; set; } // UApexClothingAsset

        [StructField("TexelRatio")]
        public UArray<float> CachedStreamingTextureFactors { get; set; }

        [StructField]
        public FSkeletalMeshSourceData SourceData { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            Bounds = FBoxSphereBounds.ReadData(buffer);
            Materials = buffer.ReadArray(UBuffer.ReadObject);
            Origin = FVector.ReadData(buffer);
            RotOrigin = FRotator.ReadData(buffer);
            RefSkeleton = buffer.ReadArray(FMeshBone.ReadData);
            SkeletalDepth = buffer.ReadInt32();

            LODModels = ReadLODModels(buffer);

            NameIndexMap = buffer.ReadMap<FName, int>(UName.ReadName, UBuffer.ReadInt32);
            PerPolyBoneKDOPs = buffer.ReadArray(FPerPolyBoneCollisionData.ReadData);
            BoneBreakNames = buffer.ReadArray(UBuffer.ReadString);
            BoneBreakOptions = buffer.ReadBytes();

            ClothingAssets = buffer.ReadArray(UBuffer.ReadObject);
            CachedStreamingTextureFactors = buffer.ReadArray(UBuffer.ReadFloat);
            SourceData = FSkeletalMeshSourceData.ReadData(buffer, this);
        }

        public UArray<FStaticLODModel> ReadLODModels(UBuffer buffer)
        {
            int count = buffer.Reader.ReadInt32();
            var array = new UArray<FStaticLODModel>(count);
            for (int i = 0; i < count; i++)
                array.Add(FStaticLODModel.ReadData(buffer, this));

            return array;
        }
    }

    [UnrealClass("SkeletalMeshSocket")]
    public class USkeletalMeshSocket : UObject
    {
        [PropertyField]
        public FName SocketName { get; set; }

        [PropertyField]
        public FName BoneName { get; set; }

        [PropertyField]
        public FVector RelativeLocation { get; set; }

        [PropertyField]
        public FRotator RelativeRotation { get; set; }

        [PropertyField]
        public FVector RelativeScale { get; set; }
    }

    [UnrealClass("SkeletalMeshComponent")]
    public class USkeletalMeshComponent : UMeshComponent
    {
        [PropertyField]
        public FObject SkeletalMesh { get; set; } // USkeletalMesh

        [PropertyField]
        public FObject PhysicsAsset { get; set; } // PhysicsAsset

        [PropertyField]
        public UArray<FObject> AnimSets { get; set; } // UAnimSet
    }

    [UnrealClass("MeshComponent")]
    public class UMeshComponent : UPrimitiveComponent
    {

    }

    [UnrealStruct("SkeletalMeshLODInfo")]
    public class FSkeletalMeshLODInfo : IAtomicStruct
    {
        [StructField]
        public float DisplayFactor { get; set; }

        [StructField]
        public float LODHysteresis { get; set; }

        [StructField]
        public UArray<int> LODMaterialMap { get; set; }

        [StructField]
        public UArray<bool> bEnableShadowCasting { get; set; }

        [StructField]
        public UArray<FTriangleSortSettings> TriangleSortSettings { get; set; }

        [StructField]
        public bool bDisableCompression { get; set; }

        [StructField]
        public bool bHasBeenSimplified { get; set; }

        public string Format => "";
    }

    [UnrealStruct("TriangleSortSettings")]
    public class FTriangleSortSettings : IAtomicStruct
    {
        [StructField]
        public TriangleSortOption TriangleSorting { get; set; }

        [StructField]
        public TriangleSortAxis CustomLeftRightAxis { get; set; }

        [StructField]
        public FName CustomLeftRightBoneName { get; set; }
        public string Format => "";
    }

    public enum  TriangleSortOption
    {
        TRISORT_None,                   // 0
        TRISORT_CenterRadialDistance,   // 1
        TRISORT_Random,                 // 2
        TRISORT_MergeContiguous,        // 3
        TRISORT_Custom,                 // 4
        TRISORT_CustomLeftRight,        // 5
        TRISORT_MAX                     // 6
    };

    public enum TriangleSortAxis
    {
        TSA_X_Axis,                     // 0
        TSA_Y_Axis,                     // 1
        TSA_Z_Axis,                     // 2
        TSA_MAX                         // 3
    };

    public class FSkeletalMeshSourceData : IAtomicStruct
    {
        [StructField]
        public bool bHaveSourceData { get; set; }

        [StructField]
        public FStaticLODModel LODModel { get; set; }

        public static FSkeletalMeshSourceData ReadData(UBuffer buffer, USkeletalMesh mesh)
        {
            var data = new FSkeletalMeshSourceData();

            data.bHaveSourceData = buffer.ReadBool();
            if (data.bHaveSourceData)
                data.LODModel = FStaticLODModel.ReadData(buffer, mesh);

            return data;
        }

        public string Format => "";
    }

    public class FPerPolyBoneCollisionData : IAtomicStruct
    {
        [StructField]
        public FSkeletalKDOPTreeLegacy LegacykDOPTree { get; set; }

        [StructField]
        public UArray<FVector> CollisionVerts { get; set; }

        public static FPerPolyBoneCollisionData ReadData(UBuffer buffer)
        {
            return new FPerPolyBoneCollisionData
            {
                LegacykDOPTree = FSkeletalKDOPTreeLegacy.ReadData(buffer),
                CollisionVerts = buffer.ReadArray(FVector.ReadData)
            };
        }

        public string Format => "";
    }

    public class FSkeletalKDOPTreeLegacy : IAtomicStruct
    {
        [StructField]
        public UArray<byte[]> Nodes { get; set; } // FkDOPCollisionTriangle

        [StructField]
        public UArray<byte[]> Triangles { get; set; } // FSkelMeshCollisionDataProvider

        public static FSkeletalKDOPTreeLegacy ReadData(UBuffer buffer)
        {
            return new FSkeletalKDOPTreeLegacy
            {
                Nodes = buffer.ReadArrayUnkElement(),
                Triangles = buffer.ReadArrayUnkElement()
            };
        }

        public string Format => "";
    }

    public class FStaticLODModel : IAtomicStruct
    {
        [StructField("SkelMeshSection")]
        public UArray<FSkelMeshSection> Sections { get; set; }

        [StructField]
        public FMultiSizeIndexContainer MultiSizeIndexContainer { get; set; }

        [StructField("ActiveBoneIndex", true)]
        public UArray<ushort> ActiveBoneIndices { get; set; }

        [StructField("SkelMeshChunk")]
        public UArray<FSkelMeshChunk> Chunks { get; set; }

        [StructField]
        public uint Size { get; set; }

        [StructField]
        public uint NumVertices { get; set; }

        [StructField("BoneIndex")]
        public byte[] RequiredBones { get; set; }

        [StructField("PointIndex")]
        public byte[] RawPointIndices { get; set; } // FIntBulkData

        [StructField]
        public uint NumTexCoords { get; set; }

        [StructField]
        public FSkeletalMeshVertexBuffer VertexBufferGPUSkin { get; set; }

        [StructField]
        public FSkeletalMeshVertexColorBuffer ColorVertexBuffer { get; set; }

        [StructField("SkeletalMeshVertexInfluences")]
        public UArray<FSkeletalMeshVertexInfluences> VertexInfluences { get; set; }

        [StructField]
        public FMultiSizeIndexContainer AdjacencyMultiSizeIndexContainer { get; set; }

        public string Format => "";
        public override string ToString() => "FStaticLODModel";

        public static FStaticLODModel ReadData(UBuffer buffer, USkeletalMesh mesh)
        {
            FStaticLODModel lod = new FStaticLODModel();
            lod.Sections = buffer.ReadArray(FSkelMeshSection.ReadData);
            lod.MultiSizeIndexContainer = FMultiSizeIndexContainer.ReadData(buffer);
            lod.ActiveBoneIndices = buffer.ReadArray(UBuffer.ReadUInt16);
            lod.Chunks = buffer.ReadArray(FSkelMeshChunk.ReadData);
            lod.Size = buffer.Reader.ReadUInt32();
            lod.NumVertices = buffer.Reader.ReadUInt32();

            lod.RequiredBones = buffer.ReadBytes();
            lod.RawPointIndices = buffer.ReadBulkData();

            lod.NumTexCoords = buffer.Reader.ReadUInt32();
            
            lod.VertexBufferGPUSkin = FSkeletalMeshVertexBuffer.ReadData(buffer);

            if (mesh.bHasVertexColors)
                lod.ColorVertexBuffer = FSkeletalMeshVertexColorBuffer.ReadData(buffer);

            lod.VertexInfluences = buffer.ReadArray(FSkeletalMeshVertexInfluences.ReadData);
            lod.AdjacencyMultiSizeIndexContainer = FMultiSizeIndexContainer.ReadData(buffer);
            
            return lod;
        }
    }

    public class FVertexBuffer : IAtomicStruct
    {
        public string Format => "";
    }

    public class FSkeletalMeshVertexInfluences : FVertexBuffer
    {
        [StructField]
        public UArray<FVertexInfluence> Influences { get; set; }

        [StructField]
        public UMap<BoneIndexPair, UArray<uint>> VertexInfluenceMapping { get; set; }

        [StructField]
        public UArray<FSkelMeshSection> Sections { get; set; }

        [StructField]
        public UArray<FSkelMeshChunk> Chunks { get; set; }

        [StructField]
        public byte[] RequiredBones { get; set; }

        [StructField]
        public byte Usage { get; set; }

        public static FSkeletalMeshVertexInfluences ReadData(UBuffer buffer)
        {
            return new()
            {
                Influences = buffer.ReadArray(FVertexInfluence.ReadData),
                VertexInfluenceMapping = buffer.ReadMap(BoneIndexPair.ReadKeys, UBuffer.ReadArrayUInt32),
                Sections = buffer.ReadArray(FSkelMeshSection.ReadData),
                Chunks = buffer.ReadArray(FSkelMeshChunk.ReadData),
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

    public class FVertexInfluence : IAtomicStruct
    {
        [StructField]
        public FInfluenceBones Bones { get; set; }

        [StructField]
        public FInfluenceWeights Weights { get; set; }

        public string Format => "";

        public static FVertexInfluence ReadData(UBuffer buffer)
        {
            return new FVertexInfluence
            {
                Bones = FInfluenceBones.ReadData(buffer),
                Weights = FInfluenceWeights.ReadData(buffer)
            };
        }
    }

    public class FInfluenceBones : IAtomicStruct
    {
        [StructField]
        public byte[] Bones { get; set; }
        public static FInfluenceBones ReadData(UBuffer buffer)
        {
            return new FInfluenceBones
            {
                Bones = buffer.Read4Bytes()
            };
        }
        public string Format => "";
    }

    public class FInfluenceWeights : IAtomicStruct
    {
        [StructField]
        public byte[] Weights { get; set; }
        public static FInfluenceWeights ReadData(UBuffer buffer)
        {
            return new FInfluenceWeights
            {
                Weights = buffer.Read4Bytes()
            };
        }
        public string Format => "";
    }

    public class FSkeletalMeshVertexColorBuffer : FVertexBuffer
    {
        [StructField("GPUSkinVertexColor", true)]
        public UArray<FGPUSkinVertexColor> Colors { get; set; }

        public static FSkeletalMeshVertexColorBuffer ReadData(UBuffer buffer)
        {
            FSkeletalMeshVertexColorBuffer vertexBuffer = new();
            vertexBuffer.Colors = buffer.ReadArrayElement(FGPUSkinVertexColor.ReadData, 4);
            return vertexBuffer;
        }
    }

    public struct GLVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector2 TexCoord;
        public byte[] Bones;
        public byte[] Weights;
    }

    public class FSkeletalMeshVertexBuffer : FVertexBuffer
    {
        [StructField]
        public uint NumTexCoords { get; set; }

        [StructField]
        public bool bUseFullPrecisionUVs { get; set; }

        [StructField]
        public bool bUsePackedPosition { get; set; }

        [StructField]
        public FVector MeshExtension { get; set; }

        [StructField]
        public FVector MeshOrigin { get; set; }

        public UArray<FGPUSkinVertexFloat16Uvs32Xyz> VertsF16UV32 { get; set; }
        public UArray<FGPUSkinVertexFloat16Uvs> VertsF16 { get; set; }
        public UArray<FGPUSkinVertexFloat32Uvs32Xyz> VertsF32UV32 { get; set; }
        public UArray<FGPUSkinVertexFloat32Uvs> VertsF32 { get; set; }

        [StructField("GPUSkinVertexBase", true)]
        public IEnumerable<FGPUSkinVertexBase> VertexData {
            get
            {
                if (VertsF16UV32 != null)  return VertsF16UV32;
                if (VertsF16 != null) return VertsF16;
                if (VertsF32UV32 != null) return VertsF32UV32;
                if (VertsF32 != null) return VertsF32;

                return [];
            }
        }

        public IEnumerable<GLVertex> GetGLVertexData()
        { 
            foreach (var vertex in VertexData)
            {
                GLVertex glVertex = new()
                {
                    Position = vertex.GetVector3(),
                    Normal = vertex.TangentZ.ToVector().ToVector3(),
                    Tangent = vertex.TangentX.ToVector().ToVector3(),
                    TexCoord = vertex.GetVector2(0), // Assuming we only need the first UV set
                    Bones = vertex.InfluenceBones,
                    Weights = vertex.InfluenceWeights
                };
                yield return glVertex;
            }
        }

        public static FSkeletalMeshVertexBuffer ReadData(UBuffer buffer)
        {
            FSkeletalMeshVertexBuffer vertexBuffer = new()
            {
                NumTexCoords = buffer.Reader.ReadUInt32(),
                bUseFullPrecisionUVs = buffer.Reader.ReadBool(),
                bUsePackedPosition = buffer.Reader.ReadBool(),
                MeshExtension = FVector.ReadData(buffer),
                MeshOrigin = FVector.ReadData(buffer)
            };

            // Forced value for PC
            vertexBuffer.bUsePackedPosition = false;

            if (!vertexBuffer.bUseFullPrecisionUVs)
            {
                if (vertexBuffer.bUsePackedPosition)
                    vertexBuffer.VertsF16UV32 = ReadArrayElement<FGPUSkinVertexFloat16Uvs32Xyz>(buffer, vertexBuffer.NumTexCoords);
                else
                    vertexBuffer.VertsF16 = ReadArrayElement<FGPUSkinVertexFloat16Uvs>(buffer, vertexBuffer.NumTexCoords);
            }
            else
            {
                if (vertexBuffer.bUsePackedPosition)
                    vertexBuffer.VertsF32UV32 = ReadArrayElement<FGPUSkinVertexFloat32Uvs32Xyz>(buffer, vertexBuffer.NumTexCoords);
                else
                    vertexBuffer.VertsF32 = ReadArrayElement<FGPUSkinVertexFloat32Uvs>(buffer, vertexBuffer.NumTexCoords);
            }
            return vertexBuffer;
        }

        private static UArray<T> ReadArrayElement<T>(UBuffer buffer, uint numTexCoords) 
            where T : FGPUSkinVertexBase, new()
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
            if (typeof(T) == typeof(FGPUSkinVertexFloat16Uvs))
                return 16 + 12 + 2 * 2 * (int)numTexCoords; // 32
            if (typeof(T) == typeof(FGPUSkinVertexFloat16Uvs32Xyz))
                return 16 + 4 + 2 * 2 * (int)numTexCoords; // 24
            if (typeof(T) == typeof(FGPUSkinVertexFloat32Uvs))
                return 16 + 12 + 4 * 2 * (int)numTexCoords; // 40
            if (typeof(T) == typeof(FGPUSkinVertexFloat32Uvs32Xyz))
                return 16 + 4 + 4 * 2 * (int)numTexCoords; // 28

            return -1;
        }
    }

    public class FGPUSkinVertexBase
    {
        [StructField]
        public FPackedNormal TangentX { get; set; }

        [StructField]
        public FPackedNormal TangentZ { get; set; }

        [StructField]
        public byte[] InfluenceBones { get; set; }

        [StructField]
        public byte[] InfluenceWeights { get; set; }

        public virtual Vector3 GetVector3()
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }

        public virtual Vector2 GetVector2(int index)
        {
            return new Vector2(0.0f, 0.0f);
        }

        public virtual void ReadData(UBuffer buffer, int num)
        {
            TangentX = FPackedNormal.ReadData(buffer);
            TangentZ = FPackedNormal.ReadData(buffer);
            InfluenceBones = buffer.Read4Bytes();
            InfluenceWeights = buffer.Read4Bytes();
        }

        public static FVector2D[] ReadUVs(UBuffer buffer, int num)
        {
            var verts = new FVector2D[num];
            for (int i = 0; i < num; i++)
                verts[i] = FVector2D.ReadData(buffer);

            return verts;
        }

        public static FVector2DHalf[] ReadHalfUVs(UBuffer buffer, int num)
        {
            var verts = new FVector2DHalf[num];
            for (int i = 0; i < num; i++)
                verts[i] = FVector2DHalf.ReadData(buffer);

            return verts;
        }
    }

    public class FGPUSkinVertexColor : IAtomicStruct
    {
        [StructField]
        public FColor VertexColor { get; set; }

        public string Format => VertexColor.Format;

        public static FGPUSkinVertexColor ReadData(UBuffer buffer)
        {
            return new()
            {
                VertexColor = FColor.ReadData(buffer)
            };
        }

        public override string ToString() => Format;
    }

    public class FGPUSkinVertexFloat16Uvs32Xyz : FGPUSkinVertexBase
    {
        [StructField]
        public FPackedPosition Positon { get; set; }

        [StructField]
        public FVector2DHalf[] UVs { get; set; }

        public override Vector3 GetVector3()
        {
            return Positon.ToVector().ToVector3();
        }

        public override Vector2 GetVector2(int index)
        {
            if (index < 0 || index >= UVs.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for UVs array.");
            return new Vector2(UVs[index].X.ToFloat(), UVs[index].Y.ToFloat());
        }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = FPackedPosition.ReadData(buffer);
            UVs = ReadHalfUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class FGPUSkinVertexFloat16Uvs : FGPUSkinVertexBase
    {
        [StructField]
        public FVector Positon { get; set; }

        [StructField]
        public FVector2DHalf[] UVs { get; set; }

        public override Vector3 GetVector3()
        {
            return Positon.ToVector3();
        }

        public override Vector2 GetVector2(int index)
        {
            if (index < 0 || index >= UVs.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for UVs array.");
            return new Vector2(UVs[index].X.ToFloat(), UVs[index].Y.ToFloat());
        }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = FVector.ReadData(buffer);
            UVs = ReadHalfUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class FGPUSkinVertexFloat32Uvs32Xyz : FGPUSkinVertexBase
    {
        [StructField]
        public FPackedPosition Positon { get; set; }

        [StructField]
        public FVector2D[] UVs { get; set; }

        public override Vector3 GetVector3()
        {
            return Positon.ToVector().ToVector3();
        }

        public override Vector2 GetVector2(int index)
        {
            if (index < 0 || index >= UVs.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for UVs array.");
            return UVs[index].ToVector2();
        }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = FPackedPosition.ReadData(buffer);
            UVs = ReadUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class FGPUSkinVertexFloat32Uvs : FGPUSkinVertexBase
    {
        [StructField]
        public FVector Positon { get; set; }

        [StructField]
        public FVector2D[] UVs { get; set; }

        public override Vector3 GetVector3()
        {
            return Positon.ToVector3();
        }

        public override Vector2 GetVector2(int index)
        {
            if (index < 0 || index >= UVs.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for UVs array.");
            return UVs[index].ToVector2();
        }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            Positon = FVector.ReadData(buffer);
            UVs = ReadUVs(buffer, num);
        }

        public override string ToString() => $"Pos: {Positon.Format} UV[0]: {UVs[0].Format}";
    }

    public class FSkelMeshChunk : IAtomicStruct
    {
        [StructField]
        public uint BaseVertexIndex { get; set; }

        [StructField("RigidSkinVertex")]
        public UArray<FRigidSkinVertex> RigidVertices { get; set; }

        [StructField("SoftSkinVertex")]
        public UArray<FSoftSkinVertex> SoftVertices { get; set; }

        [StructField("Bone", true)]
        public UArray<ushort> BoneMap { get; set; }

        [StructField]
        public int NumRigidVertices { get; set; }

        [StructField]
        public int NumSoftVertices { get; set; }

        [StructField]
        public int MaxBoneInfluences { get; set; }

        public string Format => "";
        public override string ToString() => "FSkelMeshChunk";

        public static FSkelMeshChunk ReadData(UBuffer buffer)
        {
            FSkelMeshChunk chunk = new()
            {
                BaseVertexIndex = buffer.Reader.ReadUInt32()
            };

            chunk.RigidVertices = buffer.ReadArray(FRigidSkinVertex.ReadData);
            chunk.SoftVertices = buffer.ReadArray(FSoftSkinVertex.ReadData);
            chunk.BoneMap = buffer.ReadArray(UBuffer.ReadUInt16);
            chunk.NumRigidVertices = buffer.Reader.ReadInt32();
            chunk.NumSoftVertices = buffer.Reader.ReadInt32();
            chunk.MaxBoneInfluences = buffer.Reader.ReadInt32();

            return chunk;
        }
    }

    public class FRigidSkinVertex : IAtomicStruct
    {
        [StructField]
        public FVector Position { get; set; }

        [StructField]
        public FPackedNormal TangentX { get; set; }

        [StructField]
        public FPackedNormal TangentY { get; set; }

        [StructField]
        public FPackedNormal TangentZ { get; set; }

        [StructField]
        public FVector2D[] UVs { get; set; }

        [StructField]
        public FColor Color { get; set; }

        [StructField]
        public byte Bone { get; set; }

        public string Format => "";
        public override string ToString() => "FRigidSkinVertex";

        public static FRigidSkinVertex ReadData(UBuffer buffer)
        {
            FRigidSkinVertex vertex = new()
            {
                Position = FVector.ReadData(buffer),
                TangentX = FPackedNormal.ReadData(buffer),
                TangentY = FPackedNormal.ReadData(buffer),
                TangentZ = FPackedNormal.ReadData(buffer),
                UVs = FSoftSkinVertex.ReadUVs(buffer),
                Color = FColor.ReadData(buffer),
                Bone = buffer.Reader.ReadByte()
            };

            return vertex;
        }
    }

    public class FSoftSkinVertex : IAtomicStruct
    {
        [StructField]
        public FVector Position { get; set; }

        [StructField]
        public FPackedNormal TangentX { get; set; }

        [StructField]
        public FPackedNormal TangentY { get; set; }

        [StructField]
        public FPackedNormal TangentZ { get; set; }

        [StructField]
        public FVector2D[] UVs { get; set; }

        [StructField]
        public FColor Color { get; set; }

        [StructField]
        public byte[] InfluenceBones { get; set; }

        [StructField]
        public byte[] InfluenceWeights { get; set; }

        public string Format => "";
        public override string ToString() => "FSoftSkinVertex";

        public static FSoftSkinVertex ReadData(UBuffer buffer)
        {
            FSoftSkinVertex vertex = new()
            {
                Position = FVector.ReadData(buffer),
                TangentX = FPackedNormal.ReadData(buffer),
                TangentY = FPackedNormal.ReadData(buffer),
                TangentZ = FPackedNormal.ReadData(buffer),
                UVs = ReadUVs(buffer),
                Color = FColor.ReadData(buffer),
                InfluenceBones = buffer.Read4Bytes(),
                InfluenceWeights = buffer.Read4Bytes()
            };

            return vertex;
        }

        public static FVector2D[] ReadUVs(UBuffer buffer)
        {
            var verts = new FVector2D[4];
            for (int i = 0; i < 4; i++)
                verts[i] = FVector2D.ReadData(buffer);

            return verts;
        }
    }

    public class FSkelMeshSection : IAtomicStruct
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
        public override string ToString() => "FSkelMeshSection";

        public static FSkelMeshSection ReadData(UBuffer buffer)
        {
            FSkelMeshSection section = new()
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

    public class FMultiSizeIndexContainer : IAtomicStruct
    {
        [StructField]
        public bool NeedsCPUAccess { get; set; }

        [StructField]
        public byte DataTypeSize { get; set; }

        [StructField("Index", true)]
        public UArray<uint> IndexBuffer { get; set; }

        public string Format => "";

        public static FMultiSizeIndexContainer ReadData(UBuffer buffer)
        {
            FMultiSizeIndexContainer container = new()
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

    public class FMeshBone : IAtomicStruct
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
        public FColor BoneColor { get; set; }

        public static FMeshBone ReadData(UBuffer buffer)
        {
            FMeshBone bone = new()
            {
                Name = buffer.ReadName(),
                Flags = buffer.Reader.ReadUInt32(),
                BonePos = VJointPos.ReadData(buffer),
                NumChildren = buffer.ReadInt32(),
                ParentIndex = buffer.ReadInt32(),
                BoneColor = FColor.ReadData(buffer)
            };
            return bone;
        }
        public string Format => $"BonePos: {BonePos.Format}";

        public override string ToString() => $"{Name} [{NumChildren}] [{ParentIndex}]";
    }

    public class VJointPos : IAtomicStruct
    {
        [StructField]
        public FQuat Orientation { get; set; }

        [StructField]
        public FVector Position { get; private set; }

        public static VJointPos ReadData(UBuffer buffer)
        {
            VJointPos joint = new()
            {
                Orientation = FQuat.ReadData(buffer),
                Position = FVector.ReadData(buffer)
            };
            return joint;
        }
        public string Format => $"{Orientation.Format} {Position.Format}";
    }

}
