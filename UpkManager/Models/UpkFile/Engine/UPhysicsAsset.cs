using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
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
    }
}
