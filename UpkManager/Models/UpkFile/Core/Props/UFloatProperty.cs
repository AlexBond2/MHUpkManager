using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("FloatProperty")]
    public class UFloatProperty : UProperty
    {
        private float Value { get; set; }
        public override object PropertyValue => Value;
        public override string PropertyString => $"{Value}";

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Value = buffer.Reader.ReadSingle();
        }
    }
}
