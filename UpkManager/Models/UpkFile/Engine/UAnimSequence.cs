using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;
using UpkManager.Models.UpkFile.Core;

namespace UpkManager.Models.UpkFile.Engine
{
    public class UAnimSequence : UObject
    {
        [TreeNodeField("RawAnimSequenceTrack")]
        public UArray<RawAnimSequenceTrack> RawAnimationData { get; set; }

        [TreeNodeField("Data")]
        public byte[] CompressedByteStream { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            RawAnimationData = buffer.ReadArray(RawAnimSequenceTrack.ReadData);

            CompressedByteStream = buffer.ReadBytes();
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
    }

    public enum AnimationCompressionFormat
    {
        ACF_None,                       // 0
        ACF_Float96NoW,                 // 1
        ACF_Fixed48NoW,                 // 2
        ACF_IntervalFixed32NoW,         // 3
        ACF_Fixed32NoW,                 // 4
        ACF_Float32NoW,                 // 5
        ACF_Identity,                   // 6
        ACF_MAX                         // 7
    };

    public enum AnimationKeyFormat
    {
        AKF_ConstantKeyLerp,            // 0
        AKF_VariableKeyLerp,            // 1
        AKF_PerTrackCompression,        // 2
        AKF_MAX                         // 3
    };

}
