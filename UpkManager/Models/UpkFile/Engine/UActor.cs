using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("Actor")]
    public class UActor : UObject
    {
        [PropertyField]
        public UArray<FObject> Components { get; set; } // ActorComponent

        [PropertyField]
        public FObject CollisionComponent { get; set; } // PrimitiveComponent
    }

    [UnrealClass("ActorComponent")]
    public class UActorComponent : UComponent
    {

    }

    [UnrealClass("PrimitiveComponent")]
    public class UPrimitiveComponent : UActorComponent
    {

    }

    [UnrealClass("CylinderComponent")]
    public class UCylinderComponent : UPrimitiveComponent
    {

    }
}
