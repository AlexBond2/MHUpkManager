using System;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Properties
{
    public enum ResultProperty
    {
        None,
        Success,
        Null,
        Size,
        Error
    }

    public sealed class UnrealProperty : UnrealUpkBuilderBase
    {

        #region Constructor

        public UnrealProperty()
        {
            NameIndex = new UnrealNameTableIndex();

            TypeNameIndex = new UnrealNameTableIndex();
        }

        #endregion Constructor

        #region Properties

        public UnrealNameTableIndex NameIndex { get; }

        public UnrealNameTableIndex TypeNameIndex { get; }

        public int Size { get; private set; }

        public int ArrayIndex { get; private set; }

        public UnrealPropertyValueBase Value { get; private set; }

        private VirtualNode propertyNode;
        public VirtualNode VirtualTree { get => GetVirtualTree(); }


        #endregion Properties

        #region Unreal Methods

        public ResultProperty ReadProperty(ByteArrayReader reader, UnrealHeader header)
        {
            try
            {
                NameIndex.ReadNameTableIndex(reader, header);

                if (NameIndex.IsNone()) return ResultProperty.None;
                if (NameIndex.Name == null) return ResultProperty.Null;

                TypeNameIndex.ReadNameTableIndex(reader, header);

                Size = reader.ReadInt32();
                if (Size == 0 && TypeNameIndex.IsNotBool()) return ResultProperty.Size;

                ArrayIndex = reader.ReadInt32();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property '{NameIndex.Name}': {ex.Message}");
                return ResultProperty.Error;
            }
 
            try
            {
                Value = propertyValueFactory();
                Value.ReadPropertyValue(reader, Size, header, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading property value '{NameIndex.Name}': {ex.Message}");
                return ResultProperty.Error;
            }
            return ResultProperty.Success;
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = NameIndex.GetBuilderSize();

            if (NameIndex.Name == ObjectTypes.None.ToString()) return BuilderSize;

            BuilderSize += TypeNameIndex.GetBuilderSize()
                        + sizeof(int) * 2;

            Size = Value.GetBuilderSize();

            return BuilderSize + Size;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await NameIndex.WriteBuffer(Writer, 0);

            if (NameIndex.Name == ObjectTypes.None.ToString()) return;

            await TypeNameIndex.WriteBuffer(Writer, 0);

            Writer.WriteInt32(Size);

            Writer.WriteInt32(ArrayIndex);

            await Value.WriteBuffer(Writer, CurrentOffset);
        }

        #endregion UnrealUpkBuilderBase Implementation

        #region Private Methods

        private VirtualNode GetVirtualTree()
        {
            if (propertyNode == null)
            {
                string name = $"{NameIndex.Name} ::{TypeNameIndex.Name}";
                propertyNode = new VirtualNode(name);
                propertyNode.Children.Add(Value.VirtualTree);
            }

            return propertyNode;
        }

        private UnrealPropertyValueBase propertyValueFactory()
        {
            PropertyTypes type;

            Enum.TryParse(TypeNameIndex?.Name, true, out type);

            return type switch
            {
                PropertyTypes.BoolProperty => new UnrealPropertyBoolValue(),
                PropertyTypes.IntProperty => new UnrealPropertyIntValue(),
                PropertyTypes.FloatProperty => new UnrealPropertyFloatValue(),
                PropertyTypes.ObjectProperty => new UnrealPropertyObjectValue(),
                PropertyTypes.InterfaceProperty => new UnrealPropertyInterfaceValue(),
                PropertyTypes.ComponentProperty => new UnrealPropertyComponentValue(),
                PropertyTypes.ClassProperty => new UnrealPropertyClassValue(),
                PropertyTypes.GuidProperty => new UnrealPropertyGuidValue(),
                PropertyTypes.NameProperty => new UnrealPropertyNameValue(),
                PropertyTypes.ByteProperty => new UnrealPropertyByteValue(),
                PropertyTypes.StrProperty => new UnrealPropertyStringValue(),
                PropertyTypes.StructProperty => new UnrealPropertyStructValue(),
                PropertyTypes.ArrayProperty => new UnrealPropertyArrayValue(),
                _ => new UnrealPropertyValueBase(),
            };
        }

        #endregion Private Methods

    }

}
