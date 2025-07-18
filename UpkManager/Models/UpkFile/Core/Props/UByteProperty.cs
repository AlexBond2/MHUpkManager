using System;
using UpkManager.Constants;
using UpkManager.Helpers;
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
        public override PropertyTypes PropertyType => PropertyTypes.ByteProperty;

        #region Old
        private byte? byteValue { get; set; }
        public UnrealNameTableIndex EnumNameIndex { get; set; }
        public UnrealNameTableIndex EnumValueIndex { get; set; }
        public string EnumValue => EnumValueIndex?.Name;

        public override object PropertyValue => byteValue ?? base.PropertyValue;

        public override string PropertyString => byteValue.HasValue ? $"{byteValue.Value}" : $"({EnumNameIndex?.Name}){EnumValueIndex?.Name}";
        #endregion Old

        #region OldMethods
        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            EnumNameIndex = buffer.ReadNameIndex();
            if (EnumNameIndex?.Name == "none")
                byteValue = buffer.Reader.ReadByte();
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

            if (value is bool && byteValue.HasValue) byteValue = Convert.ToByte(value);
        }

        #endregion OldMethods

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Enum = buffer.ReadObject();
        }
    }
}
