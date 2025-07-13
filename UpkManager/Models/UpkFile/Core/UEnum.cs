using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UEnum : UField
    {
        [TreeNodeField("UName")]
        public UArray<UName> Names { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Names = buffer.ReadArray(UName.ReadName);
        }
    }
}
