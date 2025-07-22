using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("IntProperty")]
    public class UIntProperty(UObject parent) : UProperty(parent)
    {
        protected int Value { get; set; }
        public override object PropertyValue => Value;
        public override string PropertyString => $"{Value:N0}";

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Value = buffer.ReadInt32();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not int) return;
            Value = (int)value;
        }
    }
}
