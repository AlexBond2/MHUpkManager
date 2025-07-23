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
        public Vector COMNudge { get; set; }

        [PropertyField]
        public KAggregateGeom AggGeom { get; set; }
    }


    [UnrealStruct("KAggregateGeom")]
    public class KAggregateGeom
    {
        [StructField]
        public UArray<KSphereElem> SphereElems { get; set; }

        [StructField]
        public UArray<KBoxElem> BoxElems { get; set; }

        [StructField]
        public UArray<KSphylElem> SphylElems { get; set; }

        [StructField]
        public UArray<KConvexElem> ConvexElems { get; set; }

        [StructField]
        public IntPtr RenderInfo { get; set; } // Pointer

        [StructField]
        public bool bSkipCloseAndParallelChecks { get; set; }
    }

    [UnrealStruct("KConvexElem")]
    public class KConvexElem
    {
        [StructField]
        public UArray<Vector> VertexData { get; set; }

        [StructField]
        public UArray<Plane> PermutedVertexData { get; set; }

        [StructField]
        public UArray<int> FaceTriData { get; set; }

        [StructField]
        public UArray<Vector> EdgeDirections { get; set; }

        [StructField]
        public UArray<Vector> FaceNormalDirections { get; set; }

        [StructField]
        public UArray<Plane> FacePlaneData { get; set; }

        [StructField]
        public Box ElemBox { get; set; }
    }

    [UnrealStruct("KSphylElem")]
    public class KSphylElem
    {
        [StructField]
        public Matrix TM { get; set; }

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
    public class KBoxElem
    {
        [StructField]
        public Matrix TM { get; set; }

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
    public class KSphereElem
    {
        [StructField]
        public Matrix TM { get; set; }

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
        public UArray<KCachedConvexData> PreCachedPhysData { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            PreCachedPhysData = buffer.ReadArray(KCachedConvexData.ReadData);
        }
    }

    public class KCachedConvexData
    {
        public UArray<KCachedConvexDataElement> CachedConvexElements { get; set; }

        public static KCachedConvexData ReadData(UBuffer buffer)
        {
            var data = new KCachedConvexData
            {
                CachedConvexElements = buffer.ReadArray(KCachedConvexDataElement.ReadData)
            };
            return data;
        }
    }

    public class KCachedConvexDataElement
    {
        public byte[] ConvexElementData { get; set; }

        public static KCachedConvexDataElement ReadData(UBuffer buffer)
        {
            var data = new KCachedConvexDataElement
            {
                ConvexElementData = buffer.ReadBytes()
            };
            return data;
        }
    }
}
