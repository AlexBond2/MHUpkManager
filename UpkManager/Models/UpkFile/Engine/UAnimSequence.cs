using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("AnimSequence")]
    public class UAnimSequence : UObject
    {
        [PropertyField]
        public FName SequenceName { get; set; }

        [PropertyField]
        public UArray<AnimNotifyEvent> Notifies { get; set; }

        [PropertyField]
        public float SequenceLength { get; set; }

        [PropertyField]
        public int NumFrames { get; set; }

        [PropertyField]
        public float RateScale { get; set; } = 1.0f;

        [PropertyField]
        public AnimationCompressionFormat TranslationCompressionFormat { get; set; } = AnimationCompressionFormat.ACF_None;

        [PropertyField]
        public AnimationCompressionFormat RotationCompressionFormat { get; set; } = AnimationCompressionFormat.ACF_Float96NoW;

        [PropertyField]
        public AnimationKeyFormat KeyEncodingFormat { get; set; }

        [PropertyField]
        public int[] CompressedTrackOffsets { get; set; }


        [StructField("RawAnimSequenceTrack")]
        public UArray<RawAnimSequenceTrack> RawAnimationData { get; set; }

        [StructField("Data")]
        public byte[] CompressedByteStream { get; set; }


        [StructField("TranslationTrack")]
        public UArray<TranslationTrack> TranslationData { get; set; }

        [StructField("RotationTrack")]
        public UArray<RotationTrack> RotationData { get; set; }

        public IAnimationCodec TranslationCodec { get; set; }
        public IAnimationCodec RotationCodec { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            RawAnimationData = buffer.ReadArray(RawAnimSequenceTrack.ReadData);

            CompressedByteStream = buffer.ReadBytes();

            if (AnimationFormat.SetInterfaceLinks(this))
                AnimationEncodingCodec.Decompress(this, CompressedByteStream);            
        }
    }

    public class AnimNotifyEvent : IAtomicStruct
    {
        [StructField]
        public float Time { get; set; }

        [StructField("UAnimNotify")]
        public FName Notify { get; set; } // UAnimNotify

        [StructField]
        public FName Comment { get; set; }

        [StructField]
        public float Duration { get; set; }

        public string Format => "";
    }

    public class RawAnimSequenceTrack
    {
        public UArray<Vector> PosKeys { get; set; }
        public UArray<Quat> RotKeys { get; set; }

        public static RawAnimSequenceTrack ReadData(UBuffer buffer)
        {
            var track = new RawAnimSequenceTrack
            {
                PosKeys = buffer.ReadArray(Vector.ReadData),
                RotKeys = buffer.ReadArray(Quat.ReadData)
            };
            return track;
        }

        public override string ToString()
        {
            return $"PosKeys[{PosKeys.Count}] RotKeys[{RotKeys.Count}]";
        }
    }

    public class TranslationTrack
    {
        public UArray<Vector> PosKeys { get; set; } = [];
        public UArray<float> Times { get; set; } = [];

        public override string ToString()
        {
            int count = PosKeys.Count;
            string data = (count > 0) ? PosKeys[0].Format : "";
            return $"{data} PosKeys[{count}] Times[{Times.Count}]";
        }
    }

    public class RotationTrack
    {
        public UArray<Quat> RotKeys { get; set; } = [];
        public UArray<float> Times { get; set; } = [];

        public override string ToString()
        {
            int count = RotKeys.Count;
            string data = (count > 0) ? RotKeys[0].Format : "";
            return $"{data} RotKeys[{RotKeys.Count}] Times[{Times.Count}]";
        }
    }
}
