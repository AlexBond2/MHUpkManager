using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("MapProperty")]
    public class UMapProperty : UProperty
    {
        [TreeNodeField("UProperty")]
        public UnrealNameTableIndex Key { get; private set; } // UProperty

        [TreeNodeField("UProperty")]
        public UnrealNameTableIndex Value { get; private set; } // UProperty
        public override PropertyTypes PropertyType => PropertyTypes.MapProperty;
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Key = buffer.ReadObject();
            Value = buffer.ReadObject();
        }
    }
}
