using System;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    public class UAnimSequence : UObject
    {
      /*  [TreeNodeField("RawAnimSequenceTrack")]
        public UArray<RawAnimSequenceTrack> RawAnimationData; */
        
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

           // RawAnimationData = buffer.ReadArray(RawAnimSequenceTrack.ReadTrack);
        }
    }
    /*
    public class RawAnimSequenceTrack
    {
        public array<Vector> PosKeys;
        public array<Quat> RotKeys;

        public static RawAnimSequenceTrack ReadTrack(UBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }*/
}
