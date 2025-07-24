
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Material
{
    [UnrealClass("UMaterialInterface")]
    public class UMaterialInterface : USurface
    {
        [PropertyField]
        public bool bHasQualitySwitch { get; set; }
    }

    [UnrealClass("Material")]
    public class UMaterial : UMaterialInterface
    {
        [PropertyField]
        public ColorMaterialInput EmissiveColor { get; set; }

        [PropertyField]
        public ScalarMaterialInput Opacity { get; set; }

        [PropertyField]
        public ScalarMaterialInput OpacityMask { get; set; }

        [PropertyField]
        public ScalarMaterialInput OpacityShadow { get; set; }

        [PropertyField]
        public bool TwoSided { get; set; }

        [PropertyField]
        public bool bUsedWithParticleSprites { get; set; }

        [PropertyField]
        public bool bUsedWithBeamTrails { get; set; }

        [PropertyField]
        public float OpacityMaskClipValue { get; set; }

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
        public UArray<MaterialFunctionInfo> MaterialFunctionInfos { get; set; }
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
    public class MaterialFunctionInfo
    {
        [StructField]
        public Guid StateId { get; set; }

        [StructField]
        public FObject Function { get; set; } // MaterialFunction
    }


    [UnrealClass("MaterialFunction")]
    public class UMaterialFunction : UObject
    {
        [PropertyField]
        public Guid StateId { get; set; }

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
        public MaterialResource[] StaticPermutationResources { get; set; }

        [StructField("StaticParameterSet")]
        public StaticParameterSet[] StaticParameters { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            StaticPermutationResources = new MaterialResource[2];
            StaticParameters = new StaticParameterSet[2];

            uint qualityMask = 0x1;
            if (bHasStaticPermutationResource)
            {
                qualityMask = buffer.Reader.ReadUInt32();
            }
            
            for (int qIndex = 0; qIndex < 2; qIndex++)
            {
                if ((qualityMask & (1 << qIndex)) == 0) continue;

                StaticPermutationResources[qIndex] = MaterialResource.ReadData(buffer);
                StaticParameters[qIndex] = StaticParameterSet.ReadData(buffer);
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
        public Guid Id { get; set; }

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
        public UArray<TextureLookup> TextureLookups { get; set; }

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

            TextureLookups = buffer.ReadArray(TextureLookup.ReadData);

            _ = buffer.Reader.ReadUInt32(); // DummyDroppedFallbackComponents
        }
    }

    public class TextureLookup
    {
        [StructField] public int TexCoordIndex { get; set; }
        [StructField] public int TextureIndex { get; set; }
        [StructField] public float UScale { get; set; }
        [StructField] public float VScale { get; set; }
        
        public override string ToString() => $"[{TexCoordIndex}] [{TextureIndex}] [{UScale:F4}; {VScale:F4}]";
        public static TextureLookup ReadData(UBuffer buffer)
        {
            return new TextureLookup
            {
                TexCoordIndex = buffer.ReadInt32(),
                TextureIndex = buffer.ReadInt32(),
                UScale = buffer.Reader.ReadSingle(),
                VScale = buffer.Reader.ReadSingle()
            };
        }
    }

    public class MaterialResource : FMaterial
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

        public static MaterialResource ReadData(UBuffer buffer)
        {
            var mat = new MaterialResource();
            mat.ReadFields(buffer);
            return mat;
        }
    }

    public class StaticParameterSet : IAtomicStruct
    {
        [StructField]
        public Guid BaseMaterialId { get; set; }

        [StructField("StaticSwitchParameter")]
        public UArray<StaticSwitchParameter> StaticSwitchParameters { get; set; }

        [StructField("StaticComponentMaskParameter")]
        public UArray<StaticComponentMaskParameter> StaticComponentMaskParameters { get; set; }

        [StructField("NormalParameter")]
        public UArray<NormalParameter> NormalParameters { get; set; }

        [StructField("StaticTerrainLayerWeightParameter")]
        public UArray<StaticTerrainLayerWeightParameter> TerrainLayerWeightParameters { get; set; }
        public string Format => "";
        public override string ToString() => $"FStaticParameterSet";

        public static StaticParameterSet ReadData(UBuffer buffer)
        {
            var staticset = new StaticParameterSet
            {
                BaseMaterialId = buffer.ReadGuid(),
                StaticSwitchParameters = buffer.ReadArray(StaticSwitchParameter.ReadData),
                StaticComponentMaskParameters = buffer.ReadArray(StaticComponentMaskParameter.ReadData),
                NormalParameters = buffer.ReadArray(NormalParameter.ReadData),
                TerrainLayerWeightParameters = buffer.ReadArray(StaticTerrainLayerWeightParameter.ReadData)
            };

            return staticset;
        }
    }

    public class StaticSwitchParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public bool Value { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public Guid ExpressionGUID { get; set; }

        public string Format => "";
        public override string ToString() => $"FStaticSwitchParameter ({ParameterName}: {Value})";
        public static StaticSwitchParameter ReadData(UBuffer buffer)
        {
            var param = new StaticSwitchParameter
            {
                ParameterName = buffer.ReadName(),
                Value = buffer.ReadBool(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };

            return param;
        }
    }

    public class StaticComponentMaskParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public bool R { get; set; }
        [StructField] public bool G { get; set; }
        [StructField] public bool B { get; set; }
        [StructField] public bool A { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public Guid ExpressionGUID { get; set; }
        public string Format => "";
        public override string ToString() => $"FStaticComponentMaskParameter ({ParameterName})";

        public static StaticComponentMaskParameter ReadData(UBuffer buffer)
        {
            var param = new StaticComponentMaskParameter
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

    public class NormalParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public byte CompressionSettings { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public Guid ExpressionGUID { get; set; }
        public string Format => "";
        public override string ToString() => $"FNormalParameter ({ParameterName})";

        public static NormalParameter ReadData(UBuffer buffer)
        {
            return new NormalParameter
            {
                ParameterName = buffer.ReadName(),
                CompressionSettings = buffer.Reader.ReadByte(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };
        }
    }

    public class StaticTerrainLayerWeightParameter : IAtomicStruct
    {
        [StructField] public FName ParameterName { get; set; }
        [StructField] public int WeightmapIndex { get; set; }
        [StructField] public bool bOverride { get; set; }
        [StructField] public Guid ExpressionGUID { get; set; }
        public string Format => "";
        public override string ToString() => $"FStaticTerrainLayerWeightParameter ({ParameterName})";

        public static StaticTerrainLayerWeightParameter ReadData(UBuffer buffer)
        {
            return new StaticTerrainLayerWeightParameter
            {
                ParameterName = buffer.ReadName(),
                WeightmapIndex = buffer.ReadInt32(),
                bOverride = buffer.ReadBool(),
                ExpressionGUID = buffer.ReadGuid()
            };
        }
    }

    [UnrealClass("MaterialInstanceConstant")]
    public class MaterialInstanceConstant : UMaterialInstance
    {
        [PropertyField]
        public UArray<FontParameterValue> FontParameterValues { get; set; }

        [PropertyField]
        public UArray<ScalarParameterValue> ScalarParameterValues { get; set; }

        [PropertyField]
        public UArray<TextureParameterValue> TextureParameterValues { get; set; }

        [PropertyField]
        public UArray<VectorParameterValue> VectorParameterValues { get; set; }
    }

    [UnrealStruct("FontParameterValue")]
    public class FontParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField]
        public FObject FontValue { get; set; } // UFont

        [StructField]
        public int FontPage { get; set; }

        [StructField]
        public Guid ExpressionGUID { get; set; }
    }

    [UnrealStruct("ScalarParameterValue")]
    public class ScalarParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField]
        public float ParameterValue { get; set; }

        [StructField]
        public Guid ExpressionGUID { get; set; }
    }

    [UnrealStruct("TextureParameterValue")]
    public class TextureParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField("UTexture")]
        public FObject ParameterValue { get; set; } // UTexture

        [StructField]
        public Guid ExpressionGUID { get; set; }
    }

    [UnrealStruct("VectorParameterValue")]
    public class VectorParameterValue
    {
        [StructField]
        public FName ParameterName { get; set; }

        [StructField]
        public LinearColor ParameterValue { get; set; }

        [StructField]
        public Guid ExpressionGUID { get; set; }
    }

    public class MaterialInput// : IAtomicStruct
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

    public class ColorMaterialInput : MaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public Color Constant { get; set; }
    }

    public class ScalarMaterialInput : MaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public float Constant { get; set; }
    }

    public class VectorMaterialInput : MaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public Vector Constant { get; set; }
    }

    public class Vector2MaterialInput : MaterialInput
    {
        [StructField] public bool UseConstant { get; set; }
        [StructField] public float ConstantX { get; set; }
        [StructField] public float ConstantY { get; set; }
    }
}
