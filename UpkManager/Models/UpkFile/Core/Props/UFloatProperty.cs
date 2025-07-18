using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("FloatProperty")]
    public class UFloatProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.FloatProperty;

        #region Old
        private float floatValue { get; set; }
        public override object PropertyValue => floatValue;
        public override string PropertyString => $"{floatValue}";
        #endregion Old

        #region OldMethods

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            floatValue = buffer.Reader.ReadSingle();
        }

        #endregion OldMethods
    }
}
