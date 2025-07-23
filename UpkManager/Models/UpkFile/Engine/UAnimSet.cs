using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("AnimSet")]
    public class UAnimSet : UObject
    {
        [PropertyField]
        public UArray<FName> TrackBoneNames { get; set; }

        [PropertyField]
        public UArray<FObject> Sequences { get; set; } // UAnimSequence

        [PropertyField]
        public UArray<FName> UseTranslationBoneNames { get; set; }

        [PropertyField]
        public FName PreviewSkelMeshName { get; set; }
    }
}
