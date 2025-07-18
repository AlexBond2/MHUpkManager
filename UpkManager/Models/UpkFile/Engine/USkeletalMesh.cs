using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("SkeletalMesh")]
    public class USkeletalMesh: UObject
    {
        [TreeNodeField]
        public BoxSphereBounds Bounds { get; set; }

        [TreeNodeField("UMaterialInterface")]
        public UArray<UnrealNameTableIndex> Materials { get; set; } // UMaterialInterface

        [TreeNodeField]
        public Vector Origin { get; set; }

        [TreeNodeField]
        public Rotator RotOrigin { get; set; }

        [TreeNodeField("MeshBone")]
        public UArray<MeshBone> RefSkeleton { get; set; }

        [TreeNodeField]
        public int SkeletalDepth { get; set; }

        [TreeNodeField("UApexClothingAsset")]
        public UArray<UnrealNameTableIndex> ClothingAssets { get; set; } // UApexClothingAsset

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            Bounds = BoxSphereBounds.ReadData(buffer);
            Materials = buffer.ReadArray(UBuffer.ReadObject);
            Origin = Vector.ReadData(buffer);
            RotOrigin = Rotator.ReadData(buffer);
            RefSkeleton = buffer.ReadArray(MeshBone.ReadData);
            SkeletalDepth = buffer.ReadInt32();

            //ClothingAssets = buffer.ReadArray(UBuffer.ReadObject);
        }
    }

    public class MeshBone // : IAtomicStruct
    {
        [StructField]
        public UnrealNameTableIndex Name { get; set; }
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
