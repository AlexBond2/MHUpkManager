using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Models.UpkFile.Properties
{
    public enum PropertyArrayTypes
    {
        ReferencedObjects,
        TrackBoneNames,
        UseTranslationBoneNames,
        Sequences,
        BodySetup,
        BoundsBodies,
        Bodies,
        ConstraintSetup,
        Constraints,
        ClothingAssets,
        Sockets,
        Notifies
    }

    public sealed class UnrealPropertyArrayValue : UnrealPropertyValueBase
    {
        private UnrealNameTableIndex nameTableIndex = new();
        private bool showArray;
        private string itemType;

        #region Properties
        public object[] Array { get; private set; }
        public int Size { get; private set; }
        public override PropertyTypes PropertyType => PropertyTypes.ArrayProperty;
        public override string PropertyString => $"{itemType} [{Size}]";

        #endregion Properties

        #region Unreal Methods

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            Size = await Task.Run(() => reader.ReadInt32());
            size -= 4;
            await base.ReadPropertyValue(reader, size, header, property);     
            
            var itemSize = size / Size;

            itemType = $"{itemSize}byte";
            showArray = false;

            Array = new object[Size];
            await BuildArrayFactory(property, DataReader, header, itemSize);
        }

        protected override VirtualNode GetVirtualTree()
        {
            var arrayNone = base.GetVirtualTree();

            if (showArray)
                for (int i = 0; i < Size; i++) 
                {
                    var item = Array[i];
                    var itemNode = new VirtualNode($"[{i}] {item}");

                    if (item is UnrealPropertyStructFields value)
                        value.BuildVirtualTree(itemNode);

                    arrayNone.Children.Add(itemNode);
                }

            return arrayNone;
        }

        private async Task BuildArrayFactory(UnrealProperty property, ByteArrayReader dataReader, UnrealHeader header, int size)
        {
            string name = property.NameIndex.Name;
            Func<Task<object>> readFunc = null;

            if (Enum.TryParse(name, true, out PropertyArrayTypes type))
            {                        
                itemType = $"{type}";

                switch (type) 
                {
                    case PropertyArrayTypes.ReferencedObjects:
                    case PropertyArrayTypes.Sequences:
                    case PropertyArrayTypes.BodySetup:
                    case PropertyArrayTypes.Bodies:
                    case PropertyArrayTypes.ConstraintSetup:
                    case PropertyArrayTypes.Constraints:
                    case PropertyArrayTypes.Sockets:
                        readFunc = () => Task.FromResult<object>(header.GetObjectTableEntry(dataReader.ReadInt32())?.ObjectNameIndex?.Name);
                        break;

                    case PropertyArrayTypes.BoundsBodies:
                    case PropertyArrayTypes.ClothingAssets:
                        readFunc = () => Task.FromResult<object>(dataReader.ReadInt32());
                        break;

                    case PropertyArrayTypes.TrackBoneNames:
                    case PropertyArrayTypes.UseTranslationBoneNames:
                        readFunc = () => { 
                            nameTableIndex.ReadNameTableIndex(dataReader, header); 
                            return Task.FromResult<object>(nameTableIndex?.Name); 
                        };
                        break;
                    case PropertyArrayTypes.Notifies:
                        readFunc = async () => 
                        {
                            var propStruct = new UnrealPropertyStructFields(CustomPropertyStruct.AnimNotifyEvent);
                            await propStruct.ReadPropertyValue(dataReader, size, header);
                            return propStruct;
                        };
                        break;
                }
            }

            if (readFunc == null) return;

            showArray = true;

            for (int i = 0; i < Size; i++)
            {
                try
                {
                    Array[i] = await readFunc();
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
