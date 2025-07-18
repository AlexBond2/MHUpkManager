using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("StructProperty")]
    public class UStructProperty : UProperty
    {
        [TreeNodeField("UStruct")]
        public UnrealNameTableIndex Struct { get; private set; } // UStruct
        public override PropertyTypes PropertyType => PropertyTypes.StructProperty;
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Struct = buffer.ReadObject();
        }
    }
}
