using System;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("ByteProperty")]
    public class UByteProperty : UProperty
    {
        [StructField("UEnum")]
        public FName Enum { get; private set; } // UEnum
        public FName EnumValueName { get; set; } // FName
        private byte? Value { get; set; }
        public string EnumValue => EnumValueName?.Name;
        public override object PropertyValue => Value ?? base.PropertyValue;
        public override string PropertyString => Value.HasValue ? $"{Value.Value}" : $"({Enum?.Name}){EnumValueName?.Name}";

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Enum = buffer.ReadNameIndex();
            if (Enum?.Name == "none")
                Value = buffer.Reader.ReadByte();
            else
                EnumValueName = buffer.ReadNameIndex();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is UnrealNameTableEntry entry)
            {
                var index = new FName();
                index.SetNameTableIndex(entry);
            }

            if (value is bool && Value.HasValue) Value = Convert.ToByte(value);
        }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Enum = buffer.ReadObject();
        }
    }
}
