using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("IntProperty")]
    public class UIntProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.IntProperty;

        #region Old
        protected int IntValue { get; set; }
        public override object PropertyValue => IntValue;
        public override string PropertyString => $"{IntValue:N0}";
        #endregion Old

        #region OldMethods

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            IntValue = buffer.ReadInt32();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is not int) return;

            IntValue = (int)value;
        }

        #endregion OldMethods
    }
}
