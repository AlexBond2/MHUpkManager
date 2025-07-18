using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("ArrayProperty")]
    public class UArrayProperty : UProperty
    {
        [TreeNodeField("UProperty")]
        public UnrealNameTableIndex Inner { get; private set; } // UProperty
        public override PropertyTypes PropertyType => PropertyTypes.ArrayProperty;

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Inner = buffer.ReadObject();
        }
    }
}
