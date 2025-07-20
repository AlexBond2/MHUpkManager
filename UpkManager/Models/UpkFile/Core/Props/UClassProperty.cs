using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("ClassProperty")]
    public class UClassProperty : UObjectProperty
    {
        [TreeNodeField("UClass")]
        public FName MetaClass { get; private set; } // UClass
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            MetaClass = buffer.ReadObject();
        }
    }
}
