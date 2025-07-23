using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Mesh
{
    [UnrealClass("KMeshProps")]
    public class UKMeshProps : UObject
    {

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
