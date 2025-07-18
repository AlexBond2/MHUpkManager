using System;
using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("BoolProperty")]
    public class UBoolProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.BoolProperty;

        #region Old
        private uint boolValue { get; set; }
        public override object PropertyValue => boolValue;
        public override string PropertyString => $"{boolValue != 0}";
        #endregion Old

        #region OldMethods

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            boolValue = buffer.Reader.ReadByte();
        }

        public override void SetPropertyValue(object value)
        {
            if (!(value is bool)) return;

            boolValue = Convert.ToUInt32((bool)value);
        }

        #endregion OldMethods
    }
}
