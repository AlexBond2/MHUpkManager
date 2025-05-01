using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;

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
        MAttachmentClasses,
        PhysicsFloppyBones,
        AnimationSetAliases,
        ThrowPowerWeakComponents,
        ThrowPowerStrongComponents,
        ThrowPutdownPowerWeakComponents,
        ThrowPutdownPowerStrongComponents,
        Components,
        FunctionExpressions,
        LibraryCategories,
        Emitters,
        LODDistances,
        LODSettings,
        LookupTable,
        BurstList,
        LODLevels,
        Modules,
        Expressions,
        MaterialFunctionInfos,
        VectorParameterValues,
    }

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

        public override async Task ReadPropertyValue(ByteArrayReader reader, int size, UnrealHeader header, UnrealProperty property)
        {
            ArraySize = await Task.Run(() => reader.ReadInt32());
            size -= 4;
            await base.ReadPropertyValue(reader, size, header, property);

            int itemSize = 0;
            if (ArraySize != 0) itemSize = size / ArraySize;

            itemType = $"{itemSize}byte";
            showArray = false;

            await BuildArrayFactory(property, DataReader, header, itemSize);
        }

        protected override VirtualNode GetVirtualTree()
        {
            var arrayNone = base.GetVirtualTree();

            if (showArray)
                for (int i = 0; i < ArraySize; i++) 
                {
                    var item = Array[i];
                    var itemNode = new VirtualNode($"[{i}] {item}");

                    if (item is UnrealPropertyCustomStructValue value)
                        value.BuildVirtualTree(itemNode);

                    arrayNone.Children.Add(itemNode);
                }

            return arrayNone;
        }

        private static readonly Dictionary<PropertyArrayTypes, CustomPropertyStruct> CustomPropertyCache = new()
        {
            { PropertyArrayTypes.Notifies, CustomPropertyStruct.FAnimNotifyEvent },
            { PropertyArrayTypes.LODInfo, CustomPropertyStruct.FSkeletalMeshLODInfo },
            { PropertyArrayTypes.TriangleSortSettings, CustomPropertyStruct.FTriangleSortSettings },
            { PropertyArrayTypes.ScalarParameterValues, CustomPropertyStruct.FScalarParameterValue },
            { PropertyArrayTypes.TextureParameterValues, CustomPropertyStruct.FTextureParameterValue },
            { PropertyArrayTypes.AnimationSetAliases, CustomPropertyStruct.FAnimationSetAlias },
            { PropertyArrayTypes.LODSettings, CustomPropertyStruct.FParticleSystemLOD },
            { PropertyArrayTypes.BurstList, CustomPropertyStruct.FParticleBurst },
            { PropertyArrayTypes.MaterialFunctionInfos, CustomPropertyStruct.FMaterialFunctionInfo },
            { PropertyArrayTypes.VectorParameterValues, CustomPropertyStruct.FVectorParameterValue },
        };

        private async Task BuildArrayFactory(UnrealProperty property, ByteArrayReader dataReader, UnrealHeader header, int size)
        {
            string name = property.NameIndex.Name;
            Func<UnrealPropertyValueBase> factory = null;

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
                    case PropertyArrayTypes.ClothingAssets:
                    case PropertyArrayTypes.MAttachmentClasses:
                    case PropertyArrayTypes.ThrowPowerWeakComponents:
                    case PropertyArrayTypes.ThrowPowerStrongComponents:
                    case PropertyArrayTypes.ThrowPutdownPowerWeakComponents:
                    case PropertyArrayTypes.ThrowPutdownPowerStrongComponents:
                    case PropertyArrayTypes.Components:
                    case PropertyArrayTypes.FunctionExpressions:
                    case PropertyArrayTypes.Emitters:
                    case PropertyArrayTypes.LODLevels:
                    case PropertyArrayTypes.Modules:
                    case PropertyArrayTypes.Expressions:
                        factory = () => new UnrealPropertyObjectValue();
                        break;

                    case PropertyArrayTypes.bEnableShadowCasting:
                        factory = () => new UnrealPropertyBoolValue();
                        break;

                    case PropertyArrayTypes.LODDistances:
                    case PropertyArrayTypes.LookupTable:
                        factory = () => new UnrealPropertyFloatValue();
                        break;

                    case PropertyArrayTypes.BoundsBodies:
                    case PropertyArrayTypes.LODMaterialMap:
                    case PropertyArrayTypes.CompressedTrackOffsets:
                        factory = () => new UnrealPropertyIntValue();
                        break;

                    case PropertyArrayTypes.LibraryCategories:
                        factory = () => new UnrealPropertyStringValue();
                        break;

                    case PropertyArrayTypes.TrackBoneNames:
                    case PropertyArrayTypes.UseTranslationBoneNames:
                    case PropertyArrayTypes.AttachmentBones:
                    case PropertyArrayTypes.WeaponSlot:
                    case PropertyArrayTypes.PhysicsFloppyBones:
                        factory = () => new UnrealPropertyNameValue();
                        break;

                    case PropertyArrayTypes.Notifies:
                    case PropertyArrayTypes.LODInfo:
                    case PropertyArrayTypes.TriangleSortSettings:
                    case PropertyArrayTypes.ScalarParameterValues:
                    case PropertyArrayTypes.TextureParameterValues:
                    case PropertyArrayTypes.AnimationSetAliases:
                    case PropertyArrayTypes.LODSettings:
                    case PropertyArrayTypes.BurstList:
                    case PropertyArrayTypes.MaterialFunctionInfos:
                    case PropertyArrayTypes.VectorParameterValues:

                        if (CustomPropertyCache.TryGetValue(type, out var structType))
                            factory = () => new UnrealPropertyCustomStructValue(structType);
                        break;
                }
            }

            Array = new UnrealPropertyValueBase[ArraySize];

            if (factory == null) return;

            showArray = true;

            for (int i = 0; i < ArraySize; i++)
            {
                try
                {
                    var value = factory();
                    await value.ReadPropertyValue(dataReader, size, header, property);
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
