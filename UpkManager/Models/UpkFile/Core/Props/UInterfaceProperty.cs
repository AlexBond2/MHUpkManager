using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("InterfaceProperty")]
    public class UInterfaceProperty(UObject parent) : UProperty(parent)
    {
        [StructField("UClass")]
        public FName InterfaceClass { get; private set; } // UClass
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            InterfaceClass = buffer.ReadObject();
        }
    }
}
