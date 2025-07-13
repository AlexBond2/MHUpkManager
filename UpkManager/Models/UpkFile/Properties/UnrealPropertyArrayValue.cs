using System;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Engine;

namespace UpkManager.Models.UpkFile.Properties
{
    public sealed class UnrealPropertyArrayValue : UnrealPropertyValueBase
    {
        private bool showArray;
        private string itemType;

        #region Properties
        public UnrealPropertyValueBase[] Array { get; private set; }
        public int ArraySize { get; private set; }
        public override PropertyTypes PropertyType => PropertyTypes.ArrayProperty;
        public override string PropertyString => $"{itemType} [{ArraySize}]";

        #endregion Properties

        #region Unreal Methods

        public override void ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            ArraySize = reader.ReadInt32();
            size -= 4;
            base.ReadPropertyValue(reader, size, header, property);

            int itemSize = 0;
            if (ArraySize != 0) itemSize = size / ArraySize;

            itemType = $"{itemSize}byte";
            showArray = false;

            BuildArrayFactory(property, DataReader, header, itemSize);
        }

        protected override VirtualNode GetVirtualTree()
        {
            var arrayNone = base.GetVirtualTree();

            if (showArray)
                for (int i = 0; i < ArraySize; i++) 
                {
                    var item = Array[i];
                    var itemNode = new VirtualNode($"[{i}] {item}");

                    if (item is UnrealPropertyEngineValue value)
                        value.BuildVirtualTree(itemNode);

                    arrayNone.Children.Add(itemNode);
                }

            return arrayNone;
        }

        private void BuildArrayFactory(UnrealProperty property, ByteArrayReader dataReader, UnrealHeader header, int size)
        {
            string name = property.NameIndex.Name;
            Func<UnrealPropertyValueBase> factory = null;

            if (EngineRegistry.TryGetProperty(name, out var def))
            {
                itemType = def.Name;

                factory = def.Type switch
                {
                    PropertyTypes.FloatProperty => () => new UnrealPropertyFloatValue(),
                    PropertyTypes.IntProperty => () => new UnrealPropertyIntValue(),
                    PropertyTypes.BoolProperty => () => new UnrealPropertyBoolValue(),
                    PropertyTypes.StrProperty => () => new UnrealPropertyStringValue(),
                    PropertyTypes.NameProperty => () => new UnrealPropertyNameValue(),
                    PropertyTypes.ObjectProperty => () => new UnrealPropertyObjectValue(),
                    PropertyTypes.StructProperty => () => new UnrealPropertyEngineValue(def.Struct!),
                    _ => null
                };
            }

            Array = new UnrealPropertyValueBase[ArraySize];

            if (factory == null) return;

            showArray = true;

            for (int i = 0; i < ArraySize; i++)
            {
                try
                {
                    var value = factory();
                    value.ReadPropertyValue(dataReader, size, header, property);
                    Array[i] = value;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BuildArrayFactory] Error at index {i}: {ex.Message}");
                    Console.WriteLine(ex.StackTrace);
                    break;
                }
            }
        }

        #endregion Unreal Methods
    }

}
