using System;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Properties
{

    public sealed class UnrealPropertyByteValue : UnrealPropertyNameValue
    {

        #region Properties

        private byte? byteValue { get; set; }

        #endregion Properties

        #region Unreal Properties

        public UnrealNameTableIndex EnumNameIndex { get; set; } = new();
        public UnrealNameTableIndex EnumValueIndex { get; set; } = new();
        public override PropertyTypes PropertyType => PropertyTypes.ByteProperty;

        public override object PropertyValue => byteValue ?? base.PropertyValue;

        public override string PropertyString => byteValue.HasValue ? $"{byteValue.Value}" : $"({EnumNameIndex?.Name}){EnumValueIndex?.Name}";

        #endregion Unreal Properties

        #region Unreal Methods

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            EnumNameIndex.ReadNameTableIndex(reader, header);

            if (EnumNameIndex?.Name == "none")
                byteValue = reader.ReadByte();
            else
                EnumValueIndex.ReadNameTableIndex(reader, header);
        }

        public override void SetPropertyValue(object value)
        {
            UnrealNameTableIndex index = value as UnrealNameTableIndex;

            if (index == null)
            {
                UnrealNameTableEntry entry = value as UnrealNameTableEntry;

                if (entry != null)
                {
                    index = new UnrealNameTableIndex();

                    index.SetNameTableIndex(entry);
                }
            }

            if (index != null)
            {
                NameIndexValue = index;

                return;
            }

            if (value is bool && byteValue.HasValue) byteValue = Convert.ToByte(value);
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = byteValue.HasValue ? sizeof(byte) : NameIndexValue.GetBuilderSize();

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            if (byteValue.HasValue) Writer.WriteByte(byteValue.Value);
            else await NameIndexValue.WriteBuffer(Writer, CurrentOffset);
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
