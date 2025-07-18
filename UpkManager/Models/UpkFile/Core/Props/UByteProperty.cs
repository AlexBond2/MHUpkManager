using System;

using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("ByteProperty")]
    public class UByteProperty : UProperty
    {
        [TreeNodeField("UEnum")]
        public UnrealNameTableIndex Enum { get; private set; } // UEnum

        private byte? Value { get; set; }
        public UnrealNameTableIndex EnumNameIndex { get; set; }
        public UnrealNameTableIndex EnumValueIndex { get; set; }
        public string EnumValue => EnumValueIndex?.Name;
        public override object PropertyValue => Value ?? base.PropertyValue;
        public override string PropertyString => Value.HasValue ? $"{Value.Value}" : $"({EnumNameIndex?.Name}){EnumValueIndex?.Name}";

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            EnumNameIndex = buffer.ReadNameIndex();
            if (EnumNameIndex?.Name == "none")
                Value = buffer.Reader.ReadByte();
            else
                EnumValueIndex = buffer.ReadNameIndex();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is UnrealNameTableEntry entry)
            {
                var index = new UnrealNameTableIndex();
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
