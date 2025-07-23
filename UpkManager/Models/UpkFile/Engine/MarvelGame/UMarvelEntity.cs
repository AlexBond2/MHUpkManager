using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.MarvelGame
{
    [UnrealClass("MarvelEntity")]
    public class UMarvelEntity : UActor
    {
        [PropertyField]
        public UArray<FObject> MAttachmentClasses { get; set; } // UMarvelAttachment

        [PropertyField]
        public UArray<FName> PhysicsFloppyBones { get; set; }

        [PropertyField]
        public UArray<AnimationSetAlias> AnimationSetAliases { get; set; }

        [PropertyField]
        public UArray<FObject> ThrowPowerWeakComponents { get; set; } // MarvelFX

        [PropertyField]
        public UArray<FObject> ThrowPowerStrongComponents { get; set; } // MarvelFX

        [PropertyField]
        public UArray<FObject> ThrowPutdownPowerWeakComponents { get; set; } // MarvelFX

        [PropertyField]
        public UArray<FObject> ThrowPutdownPowerStrongComponents { get; set; } // MarvelFX
    }

    [UnrealStruct("AnimationSetAlias")]
    public class AnimationSetAlias
    {
        [StructField]
        public FName alias { get; set; }

        [StructField("UAnimSet")]
        public FObject AnimSet { get; set; } // UAnimSet
    }

    [UnrealClass("MarvelAttachment")]
    public class UMarvelAttachment : UActor
    {
        [PropertyField]
        public EVisibilityPoint VisibilityPoint { get; set; }

        public enum EVisibilityPoint
        {
            VISIBLE_ON_SPAWN,               // 0
            VISIBLE_ON_AGGRO,               // 1
            VISIBLE_ON_ENDURANCE_CHANGE,    // 2
            VISIBLE_ON_THROW,               // 3
            VISIBLE_ON_DEMAND,              // 4
            VISIBLE_ON_MAX                  // 5
        };
    }

    [UnrealClass("MarvelAttachmentMeshBase")]
    public class UMarvelAttachmentMeshBase : UMarvelAttachment
    {
        [PropertyField]
        public UArray<FName> AttachmentBones { get; set; }

        [PropertyField]
        public UArray<FName> WeaponSlot { get; set; }
    }

    [UnrealClass("MarvelAttachment_Models")]
    public class UMarvelAttachment_Models : UMarvelAttachmentMeshBase
    {
        [PropertyField]
        public UArray<FObject> ModelMesh { get; set; } // SkeletalMesh
    }

    [UnrealClass("MarvelAttachmentAnimated")]
    public class UMarvelAttachmentAnimated : UMarvelAttachment_Models
    {
        [PropertyField]
        public UArray<FObject> AnimationSet { get; set; } // AnimSet
    }
}
