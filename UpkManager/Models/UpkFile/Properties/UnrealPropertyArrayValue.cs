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
        Sockets,
        Notifies,
        LODInfo,
        LODMaterialMap,
        bEnableShadowCasting,
        TriangleSortSettings,
        CompressedTrackOffsets,
        ScalarParameterValues,
        TextureParameterValues,
        ModelMesh,
        AttachmentBones,
        WeaponSlot,
        AnimationSet,
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

            int itemSize = 0;
            if (Size != 0) itemSize = size / Size;

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

        private static CustomPropertyStruct GetCustomProperty(PropertyArrayTypes type)
        {
            return type switch { 
                PropertyArrayTypes.Notifies => CustomPropertyStruct.FAnimNotifyEvent,
                PropertyArrayTypes.LODInfo => CustomPropertyStruct.FSkeletalMeshLODInfo,
                PropertyArrayTypes.TriangleSortSettings => CustomPropertyStruct.FTriangleSortSettings,
                PropertyArrayTypes.ScalarParameterValues => CustomPropertyStruct.FScalarParameterValue,
                PropertyArrayTypes.TextureParameterValues => CustomPropertyStruct.FTextureParameterValue,
                _ => 0
            };
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
                    case PropertyArrayTypes.ModelMesh:
                    case PropertyArrayTypes.AnimationSet:
                        readFunc = () =>
                        {
                            int index = dataReader.ReadInt32();
                            var obj = header.GetObjectTableEntry(index);
                            return Task.FromResult<object>(obj?.ObjectNameIndex?.Name);
                        };
                        break;

                    case PropertyArrayTypes.bEnableShadowCasting:
                        readFunc = () =>
                        {
                            byte index = dataReader.ReadByte();
                            return Task.FromResult<object>(index != 0);
                        };
                        break;

                    case PropertyArrayTypes.BoundsBodies:
                    case PropertyArrayTypes.ClothingAssets:
                    case PropertyArrayTypes.LODMaterialMap:
                    case PropertyArrayTypes.CompressedTrackOffsets:
                        readFunc = () =>
                        {
                            int index = dataReader.ReadInt32();
                            return Task.FromResult<object>(index);
                        };
                        break;

                    case PropertyArrayTypes.TrackBoneNames:
                    case PropertyArrayTypes.UseTranslationBoneNames:
                    case PropertyArrayTypes.AttachmentBones:
                    case PropertyArrayTypes.WeaponSlot:
                        readFunc = () => { 
                            nameTableIndex.ReadNameTableIndex(dataReader, header); 
                            return Task.FromResult<object>(nameTableIndex?.Name); 
                        };
                        break;

                    case PropertyArrayTypes.Notifies:
                    case PropertyArrayTypes.LODInfo:
                    case PropertyArrayTypes.TriangleSortSettings:
                    case PropertyArrayTypes.ScalarParameterValues:
                    case PropertyArrayTypes.TextureParameterValues:

                        readFunc = async () => 
                        {
                            var propStruct = new UnrealPropertyStructFields(GetCustomProperty(type));
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
