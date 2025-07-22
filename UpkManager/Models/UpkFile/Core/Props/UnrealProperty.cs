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
        public FName NameIndex { get; set; }
        public FName Category { get; set; }
        public int ElementSize { get; private set; }
        public int ArrayEnum { get; private set; }
        public UProperty Value { get; private set; }

        private VirtualNode propertyNode;
        public VirtualNode VirtualTree { get => GetVirtualTree(); }

        public ResultProperty ReadProperty(UBuffer buffer, UObject parent)
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
                Console.WriteLine($"Error reading property '{NameIndex?.Name}': {ex.Message}");
                return ResultProperty.Error;
            }
 
            try
            {
                Value = PropertyValueFactory(parent);
                Value.ReadPropertyValue(buffer, ElementSize, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property value '{NameIndex.Name}': {ex.Message}");
                return ResultProperty.Error;
            }
            return ResultProperty.Success;
        }

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

        private UProperty PropertyValueFactory(UObject parent)
        {
            Enum.TryParse(Category?.Name, true, out PropertyTypes type);

            return type switch
            {
                PropertyTypes.BoolProperty => new UBoolProperty(parent),
                PropertyTypes.IntProperty => new UIntProperty(parent),
                PropertyTypes.FloatProperty => new UFloatProperty(parent),
                PropertyTypes.ObjectProperty => new UObjectProperty(parent),
                PropertyTypes.InterfaceProperty => new UInterfaceProperty(parent),
                PropertyTypes.ComponentProperty => new UComponentProperty(parent),
                PropertyTypes.ClassProperty => new UClassProperty(parent),
                PropertyTypes.NameProperty => new UNameProperty(parent),
                PropertyTypes.ByteProperty => new UByteProperty(parent),
                PropertyTypes.StrProperty => new UStrProperty(parent),
                PropertyTypes.StructProperty => new UStructProperty(parent),
                PropertyTypes.ArrayProperty => new UArrayProperty(parent),
                _ => new UProperty(parent),
            };
        }
    }

}
