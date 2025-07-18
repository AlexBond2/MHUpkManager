using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("InterfaceProperty")]
    public class UInterfaceProperty : UProperty
    {
        [TreeNodeField("UClass")]
        public UnrealNameTableIndex InterfaceClass { get; private set; } // UClass
        public override PropertyTypes PropertyType => PropertyTypes.InterfaceProperty;
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            InterfaceClass = buffer.ReadObject();
        }
    }
}
