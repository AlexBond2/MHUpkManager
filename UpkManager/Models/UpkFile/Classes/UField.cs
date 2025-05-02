
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UField : UObject
    {
        public UnrealNameTableIndex SuperIndex { get; protected set; } // UStruct
        public UnrealNameTableIndex NextFieldIndex { get; private set; } // UField

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            NextFieldIndex = buffer.ReadObject();
        }
    }
}
