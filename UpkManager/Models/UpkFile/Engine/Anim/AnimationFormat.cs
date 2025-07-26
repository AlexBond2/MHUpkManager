using System.Collections.Generic;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Core;

namespace UpkManager.Models.UpkFile.Engine.Anim
{
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

    public interface IAnimationCodec
    {
        void TranslationDecode(UAnimSequence sequence, ByteArrayReader reader, int numKeys);
        void RotationDecode(UAnimSequence sequence, ByteArrayReader reader, int numKeys);
    }

    public class AnimationFormat
    {
        public static bool SetInterfaceLinks(UAnimSequence sequence)
        {
            sequence.TranslationCodec = null;
            sequence.RotationCodec = null;

            if (sequence.KeyEncodingFormat == AnimationKeyFormat.AKF_VariableKeyLerp)
            {
                if (sequence.TranslationCompressionFormat == AnimationCompressionFormat.ACF_None)
                    sequence.TranslationCodec = new VarKeyLerpCodec();
                if (sequence.RotationCompressionFormat == AnimationCompressionFormat.ACF_Float96NoW)
                    sequence.RotationCodec = new VarKeyLerpCodec();
            }

            if (sequence.TranslationCodec != null)
                sequence.TranslationData = [];

            if (sequence.RotationCodec != null)
            {
                sequence.RotationData = [];
                return true;
            }

            return false;
        }
    }

    public class AnimationEncodingCodec : IAnimationCodec
    {
        public static void Decompress(UAnimSequence sequence, byte[] compressedBytes)
        {
            var reader = ByteArrayReader.CreateNew(compressedBytes, 0);
            int numTracks = sequence.CompressedTrackOffsets.Length / 4;

            for (int trackIndex = 0; trackIndex < numTracks; trackIndex++)
            {
                int offsetTrans = sequence.CompressedTrackOffsets[trackIndex * 4 + 0];
                int numKeysTrans = sequence.CompressedTrackOffsets[trackIndex * 4 + 1];
                int offsetRot = sequence.CompressedTrackOffsets[trackIndex * 4 + 2];
                int numKeysRot = sequence.CompressedTrackOffsets[trackIndex * 4 + 3];

                reader.Seek(offsetTrans);
                sequence.TranslationCodec?.TranslationDecode(sequence, reader, numKeysTrans);

                reader.Seek(offsetRot);
                sequence.RotationCodec?.RotationDecode(sequence, reader, numKeysRot);
            }
        }

        public virtual void RotationDecode(UAnimSequence sequence, ByteArrayReader reader, int numKeys) { }
        public virtual void TranslationDecode(UAnimSequence sequence, ByteArrayReader reader, int numKeys) { }
    }

    public class VarKeyLerpCodec : AnimationEncodingCodec
    {
        public override void RotationDecode(UAnimSequence sequence, ByteArrayReader reader, int numKeys)
        {
            var format = numKeys == 1
                ? AnimationCompressionFormat.ACF_Float96NoW
                : sequence.RotationCompressionFormat;

            int numComponents = 3;

            if (format == AnimationCompressionFormat.ACF_IntervalFixed32NoW)
            {
                numComponents = 1;
                for (int i = 0; i < 6; i++)
                    reader.Skip(sizeof(float)); 
            }

            var track = new RotationTrack();

            for (int k = 0; k < numKeys; k++)
            {
                float x = 0, y = 0, z = 0;

                if (numComponents > 0) x = reader.ReadSingle();
                if (numComponents > 1) y = reader.ReadSingle();
                if (numComponents > 2) z = reader.ReadSingle();

                track.RotKeys.Add(new FQuat(x, y, z, 0));
            }

            TimeDecode(track.Times, sequence, reader, numKeys);

            sequence.RotationData.Add(track);
        }

        private void TimeDecode(List<float> times, UAnimSequence sequence, ByteArrayReader reader, int numKeys)
        {
            if (numKeys <= 1) return;

            reader.Align(4);

            bool useWord = sequence.NumFrames > 0xFF;

            for (int i = 0; i < numKeys; i++)
            {
                float time;
                if (useWord)
                {
                    ushort value = reader.ReadUInt16();
                    time = value;
                }
                else
                {
                    byte value = reader.ReadByte();
                    time = value;
                }
                times.Add(time);
            }

            reader.Align(4);
        }

        public override void TranslationDecode(UAnimSequence sequence, ByteArrayReader reader, int numKeys)
        {
            var format = numKeys == 1
                ? AnimationCompressionFormat.ACF_None
                : sequence.TranslationCompressionFormat;

            int numComponents = 3;

            if (format == AnimationCompressionFormat.ACF_IntervalFixed32NoW)
            {
                numComponents = 1;
                for (int i = 0; i < 6; i++)
                    reader.Skip(sizeof(float));
            }

            var track = new TranslationTrack();

            for (int k = 0; k < numKeys; k++)
            {
                float x = 0, y = 0, z = 0;

                if (numComponents > 0) x = reader.ReadSingle();
                if (numComponents > 1) y = reader.ReadSingle();
                if (numComponents > 2) z = reader.ReadSingle();

                track.PosKeys.Add(new FVector(x, y, z));
            }

            TimeDecode(track.Times, sequence, reader, numKeys);

            sequence.TranslationData.Add(track);
        }

    }
}
