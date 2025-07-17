using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("AnimSequence")]
    public class UAnimSequence : UObject
    {
        [PropertyField]
        public int NumFrames { get; set; }

        [PropertyField]
        public AnimationCompressionFormat RotationCompressionFormat { get; set; } = AnimationCompressionFormat.ACF_Float96NoW;

        [PropertyField]
        public AnimationCompressionFormat TranslationCompressionFormat { get; set; } = AnimationCompressionFormat.ACF_None;

        [PropertyField]
        public AnimationKeyFormat KeyEncodingFormat { get; set; }

        [PropertyField]
        public int[] CompressedTrackOffsets { get; set; }


        [TreeNodeField("RawAnimSequenceTrack")]
        public UArray<RawAnimSequenceTrack> RawAnimationData { get; set; }

        [TreeNodeField("Data")]
        public byte[] CompressedByteStream { get; set; }


        [TreeNodeField("TranslationTrack")]
        public UArray<TranslationTrack> TranslationData { get; set; }

        [TreeNodeField("RotationTrack")]
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
