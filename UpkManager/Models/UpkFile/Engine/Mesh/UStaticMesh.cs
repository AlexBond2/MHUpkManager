using System;
using System.Collections.Generic;
using System.Numerics;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Engine.MarvelGame;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Mesh
{
    [UnrealClass("StaticMesh")]
    public class UStaticMesh : UObject
    {
        [PropertyField]
        public int LightMapCoordinateIndex { get; set; }

        [PropertyField]
        public int LightMapResolution { get; set; }

        [StructField]
        public FBoxSphereBounds Bounds { get; set; }

        [StructField("RB_BodySetup")]
        public FObject BodySetup { get; set; } // URB_BodySetup

        [StructField("kDOPTree")]
        public FkDOPTree kDOPTree { get; set; }

        [StructField]
        public int InternalVersion { get; set; }

        [StructField]
        public FStaticMeshSourceData SourceData { get; set; }

        [StructField("StaticMeshOptimizationSettings")]
        public UArray<FStaticMeshOptimizationSettings> OptimizationSettings { get; set; }

        [StructField]
        public bool bHasBeenSimplified { get; set; }

        [StructField]
        public bool bIsMeshProxy { get; set; }

        [StructField("StaticMeshRenderData")]
        public UArray<FStaticMeshRenderData> LODModels { get; set; }

        [StructField("StaticMeshLODInfo")]
        public UArray<FStaticMeshLODInfo> LODInfo { get; set; }

        [StructField]
        public FRotator ThumbnailAngle { get; set; }

        [StructField]
        public float ThumbnailDistance { get; set; }

        [StructField]
        public string HighResSourceMeshName { get; set; }

        [StructField]
        public uint HighResSourceMeshCRC { get; set; }

        [StructField]
        public FGuid LightingGuid { get; set; }

        [StructField]
        public int VertexPositionVersionNumber { get; set; }

        [StructField("TexelRatio")]
        public UArray<float> CachedStreamingTextureFactors { get; set; }

        [StructField]
        public bool bRemoveDegenerates { get; set; }

        [StructField]
        public bool bUseCollisionForNavi { get; set; }

        [StructField]
        public NaviContentTags NaviContentTags { get; set; }

        [StructField]
        public bool bUseCollisionForGroundCheck { get; set; }

        [StructField]
        public bool bPerLODStaticLightingForInstancing { get; private set; }

        [StructField]
        public int ConsolePreallocateInstanceCount { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            Bounds = FBoxSphereBounds.ReadData(buffer);
            BodySetup = buffer.ReadObject();
            kDOPTree = FkDOPTree.ReadData(buffer);

            InternalVersion = buffer.ReadInt32();

            SourceData = FStaticMeshSourceData.ReadData(buffer, this);
            OptimizationSettings = buffer.ReadArray(FStaticMeshOptimizationSettings.ReadData);
            bHasBeenSimplified = buffer.ReadBool();
            
            bIsMeshProxy = buffer.ReadBool();

            LODModels = ReadLODModels(buffer);

            LODInfo = buffer.ReadArray(FStaticMeshLODInfo.ReadData);

            ThumbnailAngle = FRotator.ReadData(buffer);
            ThumbnailDistance = buffer.ReadFloat();

            HighResSourceMeshName = buffer.ReadString();
            HighResSourceMeshCRC = buffer.ReadUInt32();

            LightingGuid = buffer.ReadGuid();
            VertexPositionVersionNumber = buffer.ReadInt32();

            CachedStreamingTextureFactors = buffer.ReadArray(UBuffer.ReadFloat);
            bRemoveDegenerates = buffer.ReadBool();

            // GazillionStaticMeshSerialize
            bUseCollisionForNavi = buffer.ReadBool();
            var naviTagString = buffer.ReadName();
            if (Enum.TryParse(typeof(NaviContentTags), naviTagString.Name, ignoreCase: true, out var enumValue))
                NaviContentTags = (NaviContentTags)enumValue;
            bUseCollisionForGroundCheck = buffer.ReadBool();

            bPerLODStaticLightingForInstancing = buffer.ReadBool();
            ConsolePreallocateInstanceCount = buffer.ReadInt32();
        }       

        public UArray<FStaticMeshRenderData> ReadLODModels(UBuffer buffer)
        {
            int count = buffer.Reader.ReadInt32();
            var array = new UArray<FStaticMeshRenderData>(count);
            for (int i = 0; i < count; i++)
                array.Add(FStaticMeshRenderData.ReadData(buffer, this));

            return array;
        }
    }

    public class FStaticMeshLODInfo
    {
        public override string ToString() => "FStaticMeshLODInfo";
        public static FStaticMeshLODInfo ReadData(UBuffer buffer)
        {
            return new();
        }
    }

    public class FStaticMeshSourceData : IAtomicStruct
    {
        [StructField]
        public FStaticMeshRenderData RenderData { get; set; }

        public string Format => "";

        public static FStaticMeshSourceData ReadData(UBuffer buffer, UStaticMesh staticMesh)
        {
            var data = new FStaticMeshSourceData();
            bool haveData = buffer.ReadBool();
            if (haveData) 
                data.RenderData = FStaticMeshRenderData.ReadData(buffer, staticMesh);

            return data;
        }
    }

    public class FStaticMeshRenderData : IAtomicStruct
    {
        [StructField("StaticMeshTriangleBulkData")]
        public FStaticMeshTriangleBulkData RawTriangles { get; set; }

        [StructField("StaticMeshElement")] 
        public UArray<FStaticMeshElement> Elements { get; set; }

        [StructField]
        public FPositionVertexBuffer PositionVertexBuffer { get; set; }

        [StructField] 
        public FStaticMeshVertexBuffer VertexBuffer { get; set; }

        [StructField] 
        public FColorVertexBuffer ColorVertexBuffer { get; set; }

        [StructField] 
        public uint NumVertices { get; set; }
 
        [StructField] 
        public FRawStaticIndexBuffer IndexBuffer { get; set; }

        [StructField] 
        public FRawIndexBuffer WireframeIndexBuffer { get; set; }

        [StructField] 
        public FRawStaticIndexBuffer AdjacencyIndexBuffer { get; set; }

        public string Format => "";

        public override string ToString() => "FStaticMeshRenderData";

        public IEnumerable<GLVertex> GetGLVertexData()
        {
            int index = 0;
            foreach (var vertex in VertexBuffer.VertexData)
            {
                GLVertex glVertex = new()
                {
                    Position = PositionVertexBuffer.VertexData[index].Position.ToVector3(),
                    Normal = vertex.TangentZ.ToVector().ToVector3(),
                    Tangent = vertex.TangentX.ToVector().ToVector3(),
                    TexCoord = vertex.GetVector2(0) // Assuming we only need the first UV set
                };
                yield return glVertex;
                index++;
            }
        }

        public static FStaticMeshRenderData ReadData(UBuffer buffer, UStaticMesh staticMesh)
        {

            var data = new FStaticMeshRenderData();
            data.RawTriangles = FStaticMeshTriangleBulkData.ReadData(buffer, staticMesh);
            data.Elements = buffer.ReadArray(FStaticMeshElement.ReadData);
            data.PositionVertexBuffer = FPositionVertexBuffer.ReadData(buffer);
            data.VertexBuffer = FStaticMeshVertexBuffer.ReadData(buffer);
            data.ColorVertexBuffer = FColorVertexBuffer.ReadData(buffer);
            data.NumVertices = buffer.Reader.ReadUInt32();
            data.IndexBuffer = FRawStaticIndexBuffer.ReadData(buffer);
            data.WireframeIndexBuffer = FRawIndexBuffer.ReadData(buffer);
            data.AdjacencyIndexBuffer = FRawStaticIndexBuffer.ReadData(buffer);

            return data;
        }
    }
    
    public class FRawStaticIndexBuffer : IAtomicStruct
    {
        [StructField("Index", true)]
        public UArray<ushort> Indices { get; set; }
        public string Format => "";
        public static FRawStaticIndexBuffer ReadData(UBuffer buffer)
        {
            var data = new FRawStaticIndexBuffer();
            data.Indices = buffer.ReadArrayElement(UBuffer.ReadUInt16, 2);
            return data;
        }
    }

    public class FRawIndexBuffer : IAtomicStruct
    {
        [StructField("Index", true)]
        public UArray<ushort> Indices { get; set; }
        public string Format => "";
        public static FRawIndexBuffer ReadData(UBuffer buffer)
        {
            var data = new FRawIndexBuffer();
            data.Indices = buffer.ReadArrayElement(UBuffer.ReadUInt16, 2);
            return data;
        }
    }

    public class FPositionVertexBuffer : FVertexBuffer
    {
        [StructField]
        public uint Stride { get; set; }

        [StructField]
        public uint NumVertices { get; set; }

        [StructField("PositionVertex", true)]
        public UArray<FPositionVertex> VertexData { get; set; }

        public static FPositionVertexBuffer ReadData(UBuffer buffer)
        {
            var posBuffer = new FPositionVertexBuffer();
            posBuffer.Stride = buffer.Reader.ReadUInt32();
            posBuffer.NumVertices = buffer.Reader.ReadUInt32();
            posBuffer.VertexData = buffer.ReadArrayElement(FPositionVertex.ReadData, (int)posBuffer.Stride);
            return posBuffer;
        }
    }

    public class FColorVertexBuffer : FVertexBuffer
    {
        [StructField]
        public uint Stride { get; set; }

        [StructField]
        public uint NumVertices { get; set; }

        [StructField("Color", true)]
        public UArray<FColor> Colors { get; set; }

        public static FColorVertexBuffer ReadData(UBuffer buffer)
        {
            FColorVertexBuffer vertexBuffer = new();

            vertexBuffer.Stride = buffer.Reader.ReadUInt32();
            vertexBuffer.NumVertices = buffer.Reader.ReadUInt32();
            if (vertexBuffer.NumVertices > 0)
                vertexBuffer.Colors = buffer.ReadArrayElement(FColor.ReadData, (int)vertexBuffer.Stride);
            return vertexBuffer;
        }
    }

    public class FStaticMeshVertexBuffer : FVertexBuffer
    {
        [StructField]
        public uint NumTexCoords { get; set; }

        [StructField]
        public uint Stride { get; set; }

        [StructField]
        public uint NumVertices { get; set; }

        [StructField]
        public bool bUseFullPrecisionUVs { get; set; }

        public UArray<FStaticMeshFullVertexFloat16UVs> VertsF16 { get; set; }
        public UArray<FStaticMeshFullVertexFloat32UVs> VertsF32 { get; set; }

        [StructField("FStaticMeshFullVertex", true)]
        public IEnumerable<FStaticMeshFullVertex> VertexData
        {
            get
            {
                if (VertsF16 != null) return VertsF16;
                if (VertsF32 != null) return VertsF32;

                return [];
            }
        }

        public static FStaticMeshVertexBuffer ReadData(UBuffer buffer)
        {
            FStaticMeshVertexBuffer vertexBuffer = new()
            {
                NumTexCoords = buffer.Reader.ReadUInt32(),

                Stride = buffer.Reader.ReadUInt32(),
                NumVertices = buffer.Reader.ReadUInt32(),
                bUseFullPrecisionUVs = buffer.Reader.ReadBool()
            };

            if (!vertexBuffer.bUseFullPrecisionUVs)
                vertexBuffer.VertsF16 = ReadArrayElement<FStaticMeshFullVertexFloat16UVs>(buffer, vertexBuffer.NumTexCoords);
            else
                vertexBuffer.VertsF32 = ReadArrayElement<FStaticMeshFullVertexFloat32UVs>(buffer, vertexBuffer.NumTexCoords);
            return vertexBuffer;
        }

        private static UArray<T> ReadArrayElement<T>(UBuffer buffer, uint numTexCoords)
            where T : FStaticMeshFullVertex, new()
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
            if (typeof(T) == typeof(FStaticMeshFullVertexFloat16UVs))
                return 8 + 2 * 2 * (int)numTexCoords; // 12
            if (typeof(T) == typeof(FStaticMeshFullVertexFloat32UVs))
                return 8 + 4 * 2 * (int)numTexCoords; // 16

            return -1;
        }
    }

    public class FStaticMeshFullVertex
    {
        [StructField]
        public FPackedNormal TangentX { get; set; }

        [StructField]
        public FPackedNormal TangentZ { get; set; }

        public virtual Vector2 GetVector2(int index)
        {
            return new Vector2(0.0f, 0.0f);
        }

        public virtual void ReadData(UBuffer buffer, int num)
        {
            TangentX = FPackedNormal.ReadData(buffer);
            TangentZ = FPackedNormal.ReadData(buffer);
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

    public class FStaticMeshFullVertexFloat16UVs : FStaticMeshFullVertex
    {
        [StructField]
        public FVector2DHalf[] UVs { get; set; }

        public override Vector2 GetVector2(int index)
        {
            if (index < 0 || index >= UVs.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for UVs array.");
            return new Vector2(UVs[index].X.ToFloat(), UVs[index].Y.ToFloat());
        }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            UVs = ReadHalfUVs(buffer, num);
        }

        public override string ToString() => $"UV[0]: {UVs[0].Format}";
    }

    public class FStaticMeshFullVertexFloat32UVs : FStaticMeshFullVertex
    {
        [StructField]
        public FVector2D[] UVs { get; set; }

        public override Vector2 GetVector2(int index)
        {
            if (index < 0 || index >= UVs.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range for UVs array.");
            return UVs[index].ToVector2();
        }

        public override void ReadData(UBuffer buffer, int num)
        {
            base.ReadData(buffer, num);
            UVs = ReadUVs(buffer, num);
        }

        public override string ToString() => $"UV[0]: {UVs[0].Format}";
    }

    public class FStaticMeshTriangleBulkData : IAtomicStruct
    {
        [StructField("BulkData", true)]
        public byte[] BulkData { get; set; }
        public string Format => "";
        public static FStaticMeshTriangleBulkData ReadData(UBuffer buffer, UStaticMesh staticMesh)
        {
            var data = new FStaticMeshTriangleBulkData();
            data.BulkData = buffer.ReadBulkData();
            return data;
        }
    }

    public class FStaticMeshElement : IAtomicStruct
    {
        [StructField("UMaterialInterface")] 
        public FObject Material { get; set; } // UMaterialInterface

        [StructField] 
        public bool EnableCollision { get; set; }

        [StructField] 
        public bool OldEnableCollision { get; set; }

        [StructField] 
        public bool bEnableShadowCasting { get; set; }

        [StructField] 
        public uint FirstIndex { get; set; }

        [StructField] 
        public uint NumTriangles { get; set; }

        [StructField] 
        public uint MinVertexIndex { get; set; }

        [StructField] 
        public uint MaxVertexIndex { get; set; }

        [StructField] 
        public int MaterialIndex { get; set; }

        [StructField("FFragmentRange")] 
        public UArray<FFragmentRange> Fragments { get; set; }

        [StructField] public FPS3StaticMeshData PlatformData { get; set; }
        public string Format => "";
        public override string ToString() => "FStaticMeshElement";

        public static FStaticMeshElement ReadData(UBuffer buffer)
        {
            var data = new FStaticMeshElement();
            data.Material = buffer.ReadObject();
            data.EnableCollision = buffer.ReadBool();
            data.OldEnableCollision = buffer.ReadBool();
            data.bEnableShadowCasting = buffer.ReadBool();

            data.FirstIndex = buffer.ReadUInt32();
            data.NumTriangles = buffer.ReadUInt32();
            data.MinVertexIndex = buffer.ReadUInt32();
            data.MaxVertexIndex = buffer.ReadUInt32();

            data.MaterialIndex = buffer.ReadInt32();

            data.Fragments = buffer.ReadArray(FFragmentRange.ReadData);

            bool loadPlatformData = buffer.ReadAtomicBool();
            if (loadPlatformData)
                data.PlatformData = FPS3StaticMeshData.ReadData(buffer);

            return data;
        }
    }

    public class FPS3StaticMeshData // : FPlatformStaticMeshData
    {
        [StructField("UINT", true)] public UArray<uint> IoBufferSize { get; set; }
        [StructField("UINT", true)] public UArray<uint> ScratchBufferSize { get; set; }
        [StructField("WORD", true)] public UArray<ushort> CommandBufferHoleSize { get; set; }
        [StructField("SWORD", true)] public UArray<short> IndexBias { get; set; } 
        [StructField("WORD", true)] public UArray<ushort> VertexCount { get; set; }
        [StructField("WORD", true)] public UArray<ushort> TriangleCount { get; set; }
        [StructField("WORD", true)] public UArray<ushort> FirstVertex { get; set; }
        [StructField("WORD", true)] public UArray<ushort> FirstTriangle { get; set; }

        public static FPS3StaticMeshData ReadData(UBuffer buffer)
        {
            return new FPS3StaticMeshData
            {
                IoBufferSize = buffer.ReadArray(UBuffer.ReadUInt32),
                ScratchBufferSize = buffer.ReadArray(UBuffer.ReadUInt32),
                CommandBufferHoleSize = buffer.ReadArray(UBuffer.ReadUInt16),
                IndexBias = buffer.ReadArray(UBuffer.ReadInt16),
                VertexCount = buffer.ReadArray(UBuffer.ReadUInt16),
                TriangleCount = buffer.ReadArray(UBuffer.ReadUInt16),
                FirstVertex = buffer.ReadArray(UBuffer.ReadUInt16),
                FirstTriangle = buffer.ReadArray(UBuffer.ReadUInt16),
            };
        }
    }

    public class FFragmentRange : IAtomicStruct
    {
        [StructField] public int BaseIndex { get; set; }
        [StructField] public int NumPrimitives { get; set; }
        public string Format => "";
        public override string ToString() => $"FFragmentRange";
        public static FFragmentRange ReadData(UBuffer buffer)
        {
            return new FFragmentRange
            {
                BaseIndex = buffer.ReadInt32(),
                NumPrimitives = buffer.ReadInt32()
            };
        }
    }

    public class FPositionVertex
    {
        [StructField]
        public FVector Position { get; set; }

        public override string ToString() => Position.Format;

        public static FPositionVertex ReadData(UBuffer buffer)
        {
            return new FPositionVertex
            {
                Position = FVector.ReadData(buffer),
            };
        }
    }

    public class FStaticMeshOptimizationSettings : IAtomicStruct
    {
        [StructField]
        public byte ReductionMethod { get; set; }

        [StructField]
        public float NumOfTrianglesPercentage { get; set; }

        [StructField]
        public float MaxDeviationPercentage { get; set; }

        [StructField]
        public float WeldingThreshold { get; set; }

        [StructField]
        public bool bRecalcNormals { get; set; }

        [StructField]
        public float NormalsThreshold { get; set; }

        [StructField]
        public byte SilhouetteImportance { get; set; }

        [StructField]
        public byte TextureImportance { get; set; }

        [StructField]
        public byte ShadingImportance { get; set; }

        public string Format => "";

        public static FStaticMeshOptimizationSettings ReadData(UBuffer buffer)
        {
            return new FStaticMeshOptimizationSettings
            {
                ReductionMethod = buffer.ReadByte(),
                NumOfTrianglesPercentage = buffer.ReadFloat(),
                MaxDeviationPercentage = buffer.ReadFloat(),
                WeldingThreshold = buffer.ReadFloat(),
                bRecalcNormals = buffer.ReadBool(),
                NormalsThreshold = buffer.ReadFloat(),
                SilhouetteImportance = buffer.ReadByte(),
                TextureImportance = buffer.ReadByte(),
                ShadingImportance = buffer.ReadByte(),
            };
        }
    }

    public class FkDOPBound : IAtomicStruct
    {
        [StructField]
        public FVector Min { get; set; }

        [StructField]
        public FVector Max { get; set; }

        public string Format => "";

        public static FkDOPBound ReadData(UBuffer buffer)
        {
            return new FkDOPBound
            {
                Min = FVector.ReadData(buffer),
                Max = FVector.ReadData(buffer),
            };
        }
    }

    public class FkDOPTree : IAtomicStruct
    {
        [StructField]
        public FkDOPBound RootBound { get; set; }

        [StructField("FkDOPNodeCompact", true)]
        public UArray<byte[]> Nodes { get; set; } // FkDOPNodeCompact

        [StructField("FkDOPCollisionTriangle", true)]
        public UArray<byte[]> Triangles { get; set; } // FkDOPCollisionTriangle

        public static FkDOPTree ReadData(UBuffer buffer)
        {
            return new FkDOPTree
            {
                RootBound = FkDOPBound.ReadData(buffer),
                Nodes = buffer.ReadArrayUnkElement(),
                Triangles = buffer.ReadArrayUnkElement()
            };
        }

        public string Format => "";
    }
}
