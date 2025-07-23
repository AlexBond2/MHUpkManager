using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;
using static System.Windows.Media.Imaging.WriteableBitmapExtensions;

namespace UpkManager.Models.UpkFile.Engine.Material
{
    [UnrealClass("UMaterialInterface")]
    public class UMaterialInterface : USurface
    {

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

    }

    [UnrealClass("MaterialInstanceConstant")]
    public class MaterialInstanceConstant : UMaterialInstance
    {
        [PropertyField]
        public UArray<ScalarParameterValue> ScalarParameterValues { get; set; }

        [PropertyField]
        public UArray<TextureParameterValue> TextureParameterValues { get; set; }

        [PropertyField]
        public UArray<VectorParameterValue> VectorParameterValues { get; set; }
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
