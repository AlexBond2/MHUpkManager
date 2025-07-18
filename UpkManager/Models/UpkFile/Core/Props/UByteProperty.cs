using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("ByteProperty")]
    public class UByteProperty : UProperty
    {
        [TreeNodeField("UEnum")]
        public UnrealNameTableIndex Enum { get; private set; } // UEnum
        public override PropertyTypes PropertyType => PropertyTypes.ByteProperty;
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Enum = buffer.ReadObject();
        }
    }
}
