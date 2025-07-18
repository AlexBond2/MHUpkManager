using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("ObjectProperty")]
    public class UObjectProperty : UProperty
    {
        [TreeNodeField("UObject")]
        public UnrealNameTableIndex Object { get; private set; } // UObject
        public override string PropertyString => Object != null ? Object.Name : "null";
        public override PropertyTypes PropertyType => PropertyTypes.ObjectProperty;

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Object = buffer.ReadObject();
        }

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Object = buffer.ReadObject();
        }
    }
}
