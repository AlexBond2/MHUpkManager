using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("NameProperty")]
    public class UNameProperty : UProperty
    {
        protected UnrealNameTableIndex NameIndexValue { get; set; }
        public override object PropertyValue => NameIndexValue;
        public override string PropertyString => NameIndexValue.Name;

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            NameIndexValue = buffer.ReadNameIndex();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not UnrealNameTableIndex index) return;
            NameIndexValue = index;
        }
    }
}
