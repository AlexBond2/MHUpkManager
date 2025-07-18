using System;
using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Engine;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("ArrayProperty")]
    public class UArrayProperty : UProperty
    {
        [TreeNodeField("UProperty")]
        public UnrealNameTableIndex Inner { get; private set; } // UProperty
        public override PropertyTypes PropertyType => PropertyTypes.ArrayProperty;

        #region Old
        private bool showArray;
        private string itemType;
        public UProperty[] Array { get; private set; }
        public int ArraySize { get; private set; }
        public override string PropertyString => $"{itemType} [{ArraySize}]";
        #endregion Old

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Inner = buffer.ReadObject();
        }

        #region OldMethods

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            ArraySize = buffer.ReadInt32();
            size -= 4;
            base.ReadPropertyValue(buffer, size, property);

            int itemSize = 0;
            if (ArraySize != 0) itemSize = size / ArraySize;

            itemType = $"{itemSize}byte";
            showArray = false;
            var arrayBuffer = new UBuffer(DataReader, buffer.Header);
            BuildArrayFactory(property, arrayBuffer, itemSize);
        }

        protected override VirtualNode GetVirtualTree()
        {
            var arrayNone = base.GetVirtualTree();

            if (showArray)
                for (int i = 0; i < ArraySize; i++)
                {
                    var item = Array[i];
                    var itemNode = new VirtualNode($"[{i}] {item}");

                    if (item is EngineProperty value)
                        value.BuildVirtualTree(itemNode);

                    arrayNone.Children.Add(itemNode);
                }

            return arrayNone;
        }

        private void BuildArrayFactory(UnrealProperty property, UBuffer buffer, int size)
        {
            string name = property.NameIndex.Name;
            Func<UProperty> factory = null;

            if (EngineRegistry.TryGetProperty(name, out var def))
            {
                itemType = def.Name;

                factory = def.Type switch
                {
                    PropertyTypes.FloatProperty => () => new UFloatProperty(),
                    PropertyTypes.IntProperty => () => new UIntProperty(),
                    PropertyTypes.BoolProperty => () => new UBoolProperty(),
                    PropertyTypes.StrProperty => () => new UStrProperty(),
                    PropertyTypes.NameProperty => () => new UNameProperty(),
                    PropertyTypes.ObjectProperty => () => new UObjectProperty(),
                    PropertyTypes.StructProperty => () => new EngineProperty(def.Struct!),
                    _ => null
                };
            }

            Array = new UProperty[ArraySize];

            if (factory == null) return;

            showArray = true;

            for (int i = 0; i < ArraySize; i++)
            {
                try
                {
                    var value = factory();
                    value.ReadPropertyValue(buffer, size, property);
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

        #endregion OldMethods
    }
}
