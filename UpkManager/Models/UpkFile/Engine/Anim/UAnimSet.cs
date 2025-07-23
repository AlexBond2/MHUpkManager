using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Anim
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

    [UnrealClass("AnimNotify")]
    public class UAnimNotify : UObject
    {
    }

    [UnrealClass("AnimNotify_PlayParticleEffect")]
    public class UAnimNotify_PlayParticleEffect : UAnimNotify
    {
        [PropertyField]
        public FObject PSTemplate { get; set; } // ParticleSystem

        [PropertyField]
        public bool bAttach { get; set; }

        [PropertyField]
        public FName SocketName { get; set; }
    }

    [UnrealClass("MarvelAnimNotify_Footstep")]
    public class UMarvelAnimNotify_Footstep : UAnimNotify
    {
        [PropertyField]
        public int FootDown { get; set; }
    }

    [UnrealClass("AnimNotify_Trails")]
    public class UAnimNotify_Trails : UAnimNotify
    {
        [PropertyField]
        public FObject PSTemplate { get; set; } // ParticleSystem

        [PropertyField]
        public bool bSkipIfOwnerIsHidden { get; set; }

        [PropertyField]
        public FName FirstEdgeSocketName { get; set; }

        [PropertyField]
        public FName ControlPointSocketName { get; set; }

        [PropertyField]
        public FName SecondEdgeSocketName { get; set; }

        [PropertyField]
        public float LastStartTime { get; set; }

        [PropertyField]
        public float EndTime { get; set; }

        [PropertyField]
        public UArray<TrailSample> TrailSampledData { get; set; }
    }

    [UnrealStruct("TrailSample")]
    public class TrailSample : IAtomicStruct
    {
        [StructField]
        public float RelativeTime { get; set; }

        [StructField]
        public Vector FirstEdgeSample { get; set; }

        [StructField]
        public Vector ControlPointSample { get; set; }

        [StructField]
        public Vector SecondEdgeSample { get; set; }

        public string Format => "";
    }

    [UnrealStruct("AnimNotifyEvent")]
    public class AnimNotifyEvent : IAtomicStruct
    {
        [StructField]
        public float Time { get; set; }

        [StructField("UAnimNotify")]
        public FObject Notify { get; set; } // UAnimNotify

        [StructField]
        public FName Comment { get; set; }

        [StructField]
        public float Duration { get; set; }

        public string Format => "";
    }


    [UnrealClass("AnimMetaData")]
    public class UAnimMetaData : UObject
    {
    }

    [UnrealClass("AnimMetaData_SkelControl")]
    public class UAnimMetaData_SkelControl : UAnimMetaData
    {
        [PropertyField]
        public UArray<FName> SkelControlNameList { get; set; }
    }
}
