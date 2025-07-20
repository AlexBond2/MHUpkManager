using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [UnrealClass("Enum")]
    public class UEnum : UField
    {
        [StructField("UName")]
        public UArray<UName> Names { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Names = buffer.ReadArray(UName.ReadName);
        }
    }
}
