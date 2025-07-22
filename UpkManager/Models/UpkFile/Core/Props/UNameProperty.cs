using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("NameProperty")]
    public class UNameProperty(UObject parent) : UProperty(parent)
    {
        protected FName Value { get; set; }
        public override object PropertyValue => Value;
        public override string PropertyString => Value.Name;

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Value = buffer.ReadNameIndex();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not FName index) return;
            Value = index;
        }
    }
}
