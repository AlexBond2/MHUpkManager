using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UEnum : UField
    {
        [TreeNodeField("UName")]
        public UArray<UName> Names { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            buffer.IsType = true;
            base.ReadBuffer(buffer);
            Names = buffer.ReadArray(UName.ReadName);
        }
    }
}
