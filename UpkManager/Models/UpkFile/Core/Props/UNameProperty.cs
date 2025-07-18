using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("NameProperty")]
    public class UNameProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.NameProperty;
        #region Old
        protected UnrealNameTableIndex NameIndexValue { get; set; }
        public override object PropertyValue => NameIndexValue;
        public override string PropertyString => NameIndexValue.Name;
        #endregion Old

        #region OldMethods

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            NameIndexValue = buffer.ReadNameIndex();
        }

        public override void SetPropertyValue(object value)
        {
            if (value is UnrealNameTableIndex index)
            {
                NameIndexValue = index;
                return;
            }
        }

        #endregion OldMethods
    }
}
