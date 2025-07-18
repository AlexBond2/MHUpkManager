using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("StrProperty")]
    public class UStrProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.StrProperty;
        #region Old
        private UnrealString stringValue { get; }
        public override object PropertyValue => stringValue;
        public override string PropertyString => stringValue.String;
        #endregion Old

        #region OldMethods

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            stringValue.ReadString(buffer.Reader);
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not string str) return;

            stringValue.SetString(str);
        }

        #endregion OldMethods
    }
}
