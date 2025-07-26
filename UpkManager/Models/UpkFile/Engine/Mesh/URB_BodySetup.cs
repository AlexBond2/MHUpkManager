using System;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Mesh
{
    [UnrealClass("KMeshProps")]
    public class UKMeshProps : UObject
    {
        [PropertyField]
        public FVector COMNudge { get; set; }

        [PropertyField]
        public FKAggregateGeom AggGeom { get; set; }
    }


    [UnrealStruct("KAggregateGeom")]
    public class FKAggregateGeom
    {
        [StructField]
        public UArray<FKSphereElem> SphereElems { get; set; }

        [StructField]
        public UArray<FKBoxElem> BoxElems { get; set; }

        [StructField]
        public UArray<FKSphylElem> SphylElems { get; set; }

        [StructField]
        public UArray<FKConvexElem> ConvexElems { get; set; }

        [StructField]
        public IntPtr RenderInfo { get; set; } // Pointer

        [StructField]
        public bool bSkipCloseAndParallelChecks { get; set; }
    }

    [UnrealStruct("KConvexElem")]
    public class FKConvexElem
    {
        [StructField]
        public UArray<FVector> VertexData { get; set; }

        [StructField]
        public UArray<FPlane> PermutedVertexData { get; set; }

        [StructField]
        public UArray<int> FaceTriData { get; set; }

        [StructField]
        public UArray<FVector> EdgeDirections { get; set; }

        [StructField]
        public UArray<FVector> FaceNormalDirections { get; set; }

        [StructField]
        public UArray<FPlane> FacePlaneData { get; set; }

        [StructField]
        public FBox ElemBox { get; set; }
    }

    [UnrealStruct("KSphylElem")]
    public class FKSphylElem
    {
        [StructField]
        public FMatrix TM { get; set; }

        [StructField]
        public float Radius { get; set; }

        [StructField]
        public float Length { get; set; }

        [StructField]
        public bool bNoRBCollision { get; set; }

        [StructField]
        public bool bPerPolyShape { get; set; }
    }

    [UnrealStruct("KBoxElem")]
    public class FKBoxElem
    {
        [StructField]
        public FMatrix TM { get; set; }

        [StructField]
        public float X { get; set; }

        [StructField]
        public float Y { get; set; }

        [StructField]
        public float Z { get; set; }

        [StructField]
        public bool bNoRBCollision { get; set; }

        [StructField]
        public bool bPerPolyShape { get; set; }
    }

    [UnrealStruct("KSphereElem")]
    public class FKSphereElem
    {
        [StructField]
        public FMatrix TM { get; set; }

        [StructField]
        public float Radius { get; set; }

        [StructField]
        public bool bNoRBCollision { get; set; }

        [StructField]
        public bool bPerPolyShape { get; set; }
    }

    [UnrealClass("RB_BodySetup")]
    public class URB_BodySetup : UKMeshProps
    {
        [StructField("KCachedConvexData")]
        public UArray<FKCachedConvexData> PreCachedPhysData { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            PreCachedPhysData = buffer.ReadArray(FKCachedConvexData.ReadData);
        }
    }

    public class FKCachedConvexData
    {
        public UArray<FKCachedConvexDataElement> CachedConvexElements { get; set; }

        public static FKCachedConvexData ReadData(UBuffer buffer)
        {
            var data = new FKCachedConvexData
            {
                CachedConvexElements = buffer.ReadArray(FKCachedConvexDataElement.ReadData)
            };
            return data;
        }
    }

    public class FKCachedConvexDataElement
    {
        public byte[] ConvexElementData { get; set; }

        public static FKCachedConvexDataElement ReadData(UBuffer buffer)
        {
            var data = new FKCachedConvexDataElement
            {
                ConvexElementData = buffer.ReadBytes()
            };
            return data;
        }
    }
}
