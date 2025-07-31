using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Physics
{
    [UnrealClass("PhysicsAsset")]
    public class UPhysicsAsset : UObject
    {
        [PropertyField]
        public UArray<FObject> BodySetup { get; set; } // RB_BodySetup

        [PropertyField]
        public UArray<int> BoundsBodies { get; set; }

        [PropertyField]
        public UArray<FObject> ConstraintSetup { get; set; } // RB_ConstraintSetup
        
        [PropertyField]
        public UArray<FObject> DefaultInstance { get; set; } // PhysicsAssetInstance
    }

    [UnrealClass("PhysicsAssetInstance")]
    public class UPhysicsAssetInstance : UObject
    {
        [PropertyField]
        public UArray<FObject> Bodies { get; set; } // RB_BodyInstance

        [PropertyField]
        public UArray<FObject> Constraints { get; set; } // RB_ConstraintInstance

        [StructField("UMap<RigidBodyIndexPair, Bool>")]
        public UMap<RigidBodyIndexPair, bool> CollisionDisableTable { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            CollisionDisableTable = buffer.ReadMap(RigidBodyIndexPair.ReadKeys, UBuffer.ReadBool);            
        }
    }


    public struct RigidBodyIndexPair(int index0, int index1)
    {
        public int Index1 = index0;
        public int Index2 = index1;

        public static RigidBodyIndexPair ReadKeys(UBuffer buffer)
        {
            return new(buffer.Reader.ReadInt32(), buffer.Reader.ReadInt32());
        }

        public override string ToString() => $"[{Index1}; {Index2}]";
    }
}
