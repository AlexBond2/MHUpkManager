using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UField : UObject
    {
        [StructField("UStruct")]
        public FObject SuperIndex { get; protected set; }

        [StructField("UField")]
        public FObject NextFieldIndex { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            NextFieldIndex = buffer.ReadObject();
        }
    }
}
