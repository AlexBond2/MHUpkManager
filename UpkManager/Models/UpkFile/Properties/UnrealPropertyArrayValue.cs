using System;
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
        Sockets
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
            BuildArrayFactory(property.NameIndex.Name, DataReader, header, Size);
        }

        protected override VirtualNode GetVirtualTree()
        {
            var arrayNone = base.GetVirtualTree();

            if (showArray)
                for (int i = 0; i < Size; i++)
                    arrayNone.Children.Add(new($"[{i}] {Array[i]}"));

            return arrayNone;
        }

        private void BuildArrayFactory(string name, ByteArrayReader dataReader, UnrealHeader header, int size)
        {
            Func<object> readFunc = null;

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
                        readFunc = () => header.GetObjectTableEntry(dataReader.ReadInt32())?.ObjectNameIndex?.Name;
                        break;

                    case PropertyArrayTypes.BoundsBodies:
                    case PropertyArrayTypes.ClothingAssets:
                        readFunc = () => dataReader.ReadInt32();
                        break;

                    case PropertyArrayTypes.TrackBoneNames:
                    case PropertyArrayTypes.UseTranslationBoneNames:
                        readFunc = () => { 
                            nameTableIndex.ReadNameTableIndex(dataReader, header); 
                            return nameTableIndex?.Name; 
                        };
                        break;
                }
            }

            if (readFunc == null) return;

            for (int i = 0; i < size; i++)
                Array[i] = readFunc();

            showArray = true;
        }

        #endregion Unreal Methods
    }

}
