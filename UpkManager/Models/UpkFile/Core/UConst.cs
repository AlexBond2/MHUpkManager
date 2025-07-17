using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [UnrealClass("Const")]
    public class UConst : UField
    {
        [TreeNodeField]
        public string Value { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Value = buffer.ReadString();
        }
    }
}
