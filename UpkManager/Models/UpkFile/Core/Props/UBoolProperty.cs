using System;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("BoolProperty")]
    public class UBoolProperty(UObject parent) : UProperty(parent)
    {
        private byte Value { get; set; }
        public override object PropertyValue => Value;
        public override string PropertyString => $"{Value != 0}";

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Value = buffer.Reader.ReadByte();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not bool) return;
            Value = (byte)Convert.ToUInt32((bool)value);
        }
    }
}
