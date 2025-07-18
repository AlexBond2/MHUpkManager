using System;
using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;


namespace UpkManager.Models.UpkFile.Core
{
    public enum ResultProperty
    {
        None,
        Success,
        Null,
        Size,
        Error
    }

    public sealed class UnrealProperty
    {

        #region Properties

        public UnrealNameTableIndex NameIndex { get; set; }

        public UnrealNameTableIndex Category { get; set; }

        public int ElementSize { get; private set; }

        public int ArrayEnum { get; private set; }

        public UProperty Value { get; private set; }

        private VirtualNode propertyNode;
        public VirtualNode VirtualTree { get => GetVirtualTree(); }


        #endregion Properties

        #region Unreal Methods

        public ResultProperty ReadProperty(UBuffer buffer)
        {
            try
            {
                NameIndex = buffer.ReadNameIndex(); 

                if (NameIndex.IsNone()) return ResultProperty.None;
                if (NameIndex.Name == null) return ResultProperty.Null;

                Category = buffer.ReadNameIndex();

                ElementSize = buffer.ReadInt32(); 
                if (ElementSize == 0 && Category.IsNotBool()) return ResultProperty.Size;

                ArrayEnum = buffer.ReadInt32();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property '{NameIndex.Name}': {ex.Message}");
                return ResultProperty.Error;
            }
 
            try
            {
                Value = PropertyValueFactory();
                Value.ReadPropertyValue(buffer, ElementSize, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property value '{NameIndex.Name}': {ex.Message}");
                return ResultProperty.Error;
            }
            return ResultProperty.Success;
        }

        #endregion Unreal Methods

        #region Private Methods

        private VirtualNode GetVirtualTree()
        {
            if (propertyNode == null)
            {
                string name = $"{NameIndex.Name} ::{Category.Name}";
                propertyNode = new VirtualNode(name);
                propertyNode.Children.Add(Value.VirtualTree);
            }

            return propertyNode;
        }

        private UProperty PropertyValueFactory()
        {
            PropertyTypes type;

            Enum.TryParse(Category?.Name, true, out type);

            return type switch
            {
                PropertyTypes.BoolProperty => new UBoolProperty(),
                PropertyTypes.IntProperty => new UIntProperty(),
                PropertyTypes.FloatProperty => new UFloatProperty(),
                PropertyTypes.ObjectProperty => new UObjectProperty(),
                PropertyTypes.InterfaceProperty => new UInterfaceProperty(),
                PropertyTypes.ComponentProperty => new UComponentProperty(),
                PropertyTypes.ClassProperty => new UClassProperty(),
                PropertyTypes.NameProperty => new UNameProperty(),
                PropertyTypes.ByteProperty => new UByteProperty(),
                PropertyTypes.StrProperty => new UStrProperty(),
                PropertyTypes.StructProperty => new UStructProperty(),
                PropertyTypes.ArrayProperty => new UArrayProperty(),
                _ => new UProperty(),
            };
        }

        #endregion Private Methods

    }

}
