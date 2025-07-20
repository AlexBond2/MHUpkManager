using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UField : UObject
    {
        [TreeNodeField("UStruct")]
        public FName SuperIndex { get; protected set; }

        [TreeNodeField("UField")]
        public FName NextFieldIndex { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            NextFieldIndex = buffer.ReadObject();
        }
    }
}
