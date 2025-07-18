using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("StrProperty")]
    public class UStrProperty : UProperty
    {
        private UnrealString Value { get; set; } // FString
        public override object PropertyValue => Value;
        public override string PropertyString => Value.String;

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Value = new();
            Value.ReadString(buffer.Reader);
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not string str) return;
            Value.SetString(str);
        }
    }
}
