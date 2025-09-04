
using System;
using System.Numerics;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Material
{
    [UnrealClass("MaterialInterface")]
    public class UMaterialInterface : USurface
    {
        [PropertyField]
        public bool bHasQualitySwitch { get; set; }
    }

    [UnrealClass("Material")]
    public class UMaterial : UMaterialInterface
    {
        [PropertyField]
        public FColorMaterialInput DiffuseColor { get; set; }

        [PropertyField]
        public FVectorMaterialInput Normal { get; set; }

        [PropertyField]
        public FColorMaterialInput EmissiveColor { get; set; }

        [PropertyField]
        public FScalarMaterialInput Opacity { get; set; }

        [PropertyField]
        public FScalarMaterialInput OpacityMask { get; set; }

        [PropertyField]
        public FScalarMaterialInput OpacityShadow { get; set; }

        [PropertyField]
        public bool TwoSided { get; set; }

        [PropertyField]
        public bool bUsedWithParticleSprites { get; set; }

        [PropertyField]
        public bool bUsedWithBeamTrails { get; set; }

        [PropertyField]
        public float OpacityMaskClipValue { get; set; }

        [PropertyField]
        public FVector2MaterialInput Distortion { get; set; }

        [PropertyField]
        public EBlendMode BlendMode { get; set; }

        [PropertyField]
        public EMaterialLightingModel LightingModel { get; set; }

        [PropertyField]
        public int EditorX { get; set; }

        [PropertyField]
        public int EditorY { get; set; }

        [PropertyField]
        public int EditorPitch { get; set; }

        [PropertyField]
        public int EditorYaw { get; set; }

        [PropertyField]
        public UArray<FObject> Expressions { get; set; } // MaterialExpression

        [PropertyField]
        public UArray<FMaterialFunctionInfo> MaterialFunctionInfos { get; set; }

        [StructField("MaterialResource")]
        public FMaterialResource[] MaterialResource { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            MaterialResource = new FMaterialResource[2];

            uint qualityMask = buffer.Reader.ReadUInt32();

            for (int qIndex = 0; qIndex < 2; qIndex++)
            {
                if ((qualityMask & (1 << qIndex)) == 0) continue;

                MaterialResource[qIndex] = FMaterialResource.ReadData(buffer);
            }
        }
    }

    public enum EBlendMode
    {
        BLEND_Opaque,                   // 0
        BLEND_Masked,                   // 1
        BLEND_Translucent,              // 2
        BLEND_Additive,                 // 3
        BLEND_Modulate,                 // 4
        BLEND_ModulateAndAdd,           // 5
        BLEND_SoftMasked,               // 6
        BLEND_AlphaComposite,           // 7
        BLEND_DitheredTranslucent,      // 8
        BLEND_MAX                       // 9
    };

    public enum EMaterialLightingModel
    {
        MLM_Phong,                      // 0
        MLM_NonDirectional,             // 1
        MLM_Unlit,                      // 2
        MLM_SHPRT,                      // 3
        MLM_Custom,                     // 4
        MLM_Anisotropic,                // 5
        MLM_MAX                         // 6
    };

    [UnrealStruct("MaterialFunctionInfo")]
    public class FMaterialFunctionInfo
    {
        [StructField]
        public FGuid StateId { get; set; }

        [StructField]
        public FObject Function { get; set; } // MaterialFunction
    }


    [UnrealClass("MaterialFunction")]
    public class UMaterialFunction : UObject
    {
        [PropertyField]
        public FGuid StateId { get; set; }

        [PropertyField]
        public string Description { get; set; }

        [PropertyField]
        public UArray<string> LibraryCategories { get; set; }

        [PropertyField]
        public UArray<FObject> FunctionExpressions { get; set; } // MaterialExpression
    }

    [UnrealClass("MaterialInstance")]
    public class UMaterialInstance : UMaterialInterface
    {
        [PropertyField]
        public FObject Parent { get; set; } // MaterialInterface

        [PropertyField]
        public bool bHasStaticPermutationResource { get; set; }

        [StructField("MaterialResource")]
        public FMaterialResource[] StaticPermutationResources { get; set; }

        [StructField("StaticParameterSet")]
        public FStaticParameterSet[] StaticParameters { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            StaticPermutationResources = new FMaterialResource[2];
            StaticParameters = new FStaticParameterSet[2];

            uint qualityMask = 0x1;
            if (bHasStaticPermutationResource)
            {
                qualityMask = buffer.Reader.ReadUInt32();
            }
            
            for (int qIndex = 0; qIndex < 2; qIndex++)
            {
                if ((qualityMask & (1 << qIndex)) == 0) continue;

                StaticPermutationResources[qIndex] = FMaterialResource.ReadData(buffer);
                StaticParameters[qIndex] = FStaticParameterSet.ReadData(buffer);
            }
        }
    }

    public class FMaterial : IAtomicStruct
    {
        [StructField("String")]
        public UArray<string> CompileErrors { get; set; }

        [StructField("UMap<UMaterialExpression, Int32>")]
        public UMap<FObject, int> TextureDependencyLengthMap { get; set; } // UMaterialExpression

        [StructField]
        public int MaxTextureDependencyLength { get; set; }

        [StructField]
        public FGuid Id { get; set; }

        [StructField]
        public int NumUserTexCoords { get; set; }

        [StructField("UTexture")]
        public UArray<FObject> UniformExpressionTextures { get; set; } // UTexture

        [StructField]
        public bool bUsesSceneColor { get; set; }

        [StructField]
        public bool bUsesSceneDepth { get; set; }

        [StructField]
        public bool bUsesDynamicParameter { get; set; }

        [StructField]
        public bool bUsesLightmapUVs { get; set; }

        [StructField]
        public bool bUsesMaterialVertexPositionOffset { get; set; }

        [StructField]
        public uint UsingTransforms { get; set; }

        [StructField("TextureLookup")]
        public UArray<FTextureLookup> TextureLookups { get; set; }

        public string Format => "";
        public override string ToString() => "FMaterial";

        public virtual void ReadFields(UBuffer buffer)
        {
            CompileErrors = buffer.ReadArray(UBuffer.ReadString);

            TextureDependencyLengthMap = buffer.ReadMap(UBuffer.ReadObject, UBuffer.ReadInt32);
            MaxTextureDependencyLength = buffer.ReadInt32();

            Id = buffer.ReadGuid();
            NumUserTexCoords = buffer.ReadInt32();

            UniformExpressionTextures = buffer.ReadArray(UBuffer.ReadObject);

            bUsesSceneColor = buffer.ReadBool();
            bUsesSceneDepth = buffer.ReadBool();
            bUsesDynamicParameter = buffer.ReadBool();
            bUsesLightmapUVs = buffer.ReadBool();
            bUsesMaterialVertexPositionOffset = buffer.ReadBool();

            UsingTransforms = buffer.Reader.ReadUInt32();

            TextureLookups = buffer.ReadArray(FTextureLookup.ReadData);

            _ = buffer.Reader.ReadUInt32(); // DummyDroppedFallbackComponents
        }
    }

    public class FTextureLookup
    {
        [StructField] public int TexCoordIndex { get; set; }
        [StructField] public int TextureIndex { get; set; }
        [StructField] public float UScale { get; set; }
        [StructField] public float VScale { get; set; }
        
        public override string ToString() => $"[{TexCoordIndex}] [{TextureIndex}] [{UScale:F4}; {VScale:F4}]";
        public static FTextureLookup ReadData(UBuffer buffer)
        {
            return new FTextureLookup
            {
                TexCoordIndex = buffer.ReadInt32(),
                TextureIndex = buffer.ReadInt32(),
                UScale = buffer.Reader.ReadSingle(),
                VScale = buffer.Reader.ReadSingle()
            };
        }
    }

    public class FMaterialResource : FMaterial
    {
        [StructField]
        public EBlendMode BlendModeOverrideValue { get; set; }

        [StructField]
        public bool bIsBlendModeOverrided { get; set; }

        [StructField]
        public bool bIsMaskedOverrideValue { get; set; }

        public override string ToString() => $"FMaterialResource";

        public override void ReadFields(UBuffer buffer)
        {
            base.ReadFields(buffer);
            BlendModeOverrideValue = (EBlendMode)buffer.ReadInt32();
            bIsBlendModeOverrided = buffer.ReadBool();
            bIsMaskedOverrideValue = buffer.ReadBool();
        }

        public static FMaterialResource ReadData(UBuffer buffer)
        {
            var mat = new FMaterialResource();
            mat.ReadFields(buffer);
            return mat;
        }
    }

    public class FStaticParameterSet : IAtomicStruct
    {
        [StructField]
        public FGuid BaseMaterialId { get; set; }

        [StructField("StaticSwitchParameter")]
        public UArray<FStaticSwitchParameter> StaticSwitchParameters { get; set; }

        [StructField("StaticComponentMaskParameter")]
        public UArray<FStaticComponentMaskParameter> StaticComponentMaskParameters { get; set; }

        [StructField("NormalParameter")]
        public UArray<FNormalParameter> NormalParameters { get; set; }

        [StructField("StaticTerrainLayerWeightParameter")]
        public UArray<FStaticTerrainLayerWeightParameter> TerrainLayerWeightParameters { get; set; }
        public string Format => "";
        public override string ToString() => $"FStaticParameterSet";

        public static FStaticParameterSet ReadData(UBuffer buffer)
        {
            var staticset = new FStaticParameterSet
            {
                BaseMaterialId = buffer.ReadGuid(),
                StaticSwitchParameters = buffer.ReadArray(FStaticSwitchParameter.ReadData),
                StaticComponentMaskParameters = buffer.ReadArray(FStaticComponentMaskParameter.ReadData),
                NormalParameters = buffer.ReadArray(FNormalParameter.ReadData),
                TerrainLayerWeightParameters = buffer.ReadArray(FStaticTerrainLayerWeightParameter.ReadData)
            };

            return staticset;
        }
    }

    public class FStaticSwitchParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public bool Value { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public FGuid ExpressionGUID { get; set; }

        public string Format => "";
        public override string ToString() => $"FStaticSwitchParameter ({ParameterName}: {Value})";
        public static FStaticSwitchParameter ReadData(UBuffer buffer)
        {
            var param = new FStaticSwitchParameter
            {
                ParameterName = buffer.ReadName(),
                Value = buffer.ReadBool(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };

            return param;
        }
    }

    public class FStaticComponentMaskParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public bool R { get; set; }
        [StructField] public bool G { get; set; }
        [StructField] public bool B { get; set; }
        [StructField] public bool A { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public FGuid ExpressionGUID { get; set; }
        public string Format => "";
        public override string ToString() => $"FStaticComponentMaskParameter ({ParameterName})";

        public static FStaticComponentMaskParameter ReadData(UBuffer buffer)
        {
            var param = new FStaticComponentMaskParameter
            {
                ParameterName = buffer.ReadName(),
                R = buffer.ReadBool(),
                G = buffer.ReadBool(),
                B = buffer.ReadBool(),
                A = buffer.ReadBool(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };

            return param;
        }
    }

    public class FNormalParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public byte CompressionSettings { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public FGuid ExpressionGUID { get; set; }
        public string Format => "";
        public override string ToString() => $"FNormalParameter ({ParameterName})";

        public static FNormalParameter ReadData(UBuffer buffer)
        {
            return new FNormalParameter
            {
                ParameterName = buffer.ReadName(),
                CompressionSettings = buffer.Reader.ReadByte(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };
        }
    }

    public class FStaticTerrainLayerWeightParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public int WeightmapIndex { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public FGuid ExpressionGUID { get; set; }
        public string Format => "";
        public override string ToString() => $"FStaticTerrainLayerWeightParameter ({ParameterName})";

        public static FStaticTerrainLayerWeightParameter ReadData(UBuffer buffer)
        {
            return new FStaticTerrainLayerWeightParameter
            {
                ParameterName = buffer.ReadName(),
                WeightmapIndex = buffer.ReadInt32(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };
        }
    }

    [UnrealClass("MaterialInstanceConstant")]
    public class UMaterialInstanceConstant : UMaterialInstance
    {
        [PropertyField]
        public UArray<FFontParameterValue> FontParameterValues { get; set; }

        [PropertyField]
        public UArray<FScalarParameterValue> ScalarParameterValues { get; set; }

        [PropertyField]
        public UArray<FTextureParameterValue> TextureParameterValues { get; set; }

        [PropertyField]
        public UArray<FVectorParameterValue> VectorParameterValues { get; set; }

        public FObject GetTextureParameterValue(string parameterName)
        {
            if (TextureParameterValues == null) return null;

            foreach (var parameter in TextureParameterValues)
                if (parameter.ParameterName.Name.Contains(parameterName, StringComparison.OrdinalIgnoreCase))
                    return parameter.ParameterValue;

            return null;
        }

        public float? GetScalarParameterValue(string parameterName)
        {
            if (ScalarParameterValues == null) return null;

            foreach (var parameter in ScalarParameterValues)
                if (parameter.ParameterName.Name.StartsWith(parameterName, StringComparison.OrdinalIgnoreCase))
                    return parameter.ParameterValue;

            return null;
        }

        public Vector3? GetVectorParameterValue(string parameterName)
        {
            if (VectorParameterValues == null) return null;

            foreach (var parameter in VectorParameterValues)
                if (parameter.ParameterName.Name.StartsWith(parameterName, StringComparison.OrdinalIgnoreCase))
                    return parameter.ParameterValue.ToVector3();

            return null;
        }
    }

    [UnrealStruct("FontParameterValue")]
    public class FFontParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField]
        public FObject FontValue { get; set; } // UFont

        [StructField]
        public int FontPage { get; set; }

        [StructField]
        public FGuid ExpressionGUID { get; set; }
    }

    [UnrealStruct("ScalarParameterValue")]
    public class FScalarParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField]
        public float ParameterValue { get; set; }

        [StructField]
        public FGuid ExpressionGUID { get; set; }
    }

    [UnrealStruct("TextureParameterValue")]
    public class FTextureParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField("UTexture")]
        public FObject ParameterValue { get; set; } // UTexture

        [StructField]
        public FGuid ExpressionGUID { get; set; }
    }

    [UnrealStruct("VectorParameterValue")]
    public class FVectorParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField]
        public FLinearColor ParameterValue { get; set; }

        [StructField]
        public FGuid ExpressionGUID { get; set; }
    }

    public class FMaterialInput// : IAtomicStruct
    {
        [StructField] public FObject Expression { get; set; } // MaterialExpression
        [StructField] public int OutputIndex { get; set; }
        [StructField] public string InputName { get; set; }
        [StructField] public int Mask { get; set; }
        [StructField] public int MaskR { get; set; }
        [StructField] public int MaskG { get; set; }
        [StructField] public int MaskB { get; set; }
        [StructField] public int MaskA { get; set; }
        [StructField] public int GCC64_Padding { get; set; }

        public string Format => "";
    }

    [UnrealStruct("ColorMaterialInput")]
    public class FColorMaterialInput : FMaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public FColor Constant { get; set; }
    }

    [UnrealStruct("ScalarMaterialInput")]
    public class FScalarMaterialInput : FMaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public float Constant { get; set; }
    }

    [UnrealStruct("VectorMaterialInput")]
    public class FVectorMaterialInput : FMaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public FVector Constant { get; set; }
    }

    [UnrealStruct("Vector2MaterialInput")]
    public class FVector2MaterialInput : FMaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public float ConstantX { get; set; }
        [StructField] public float ConstantY { get; set; }
    }
}
