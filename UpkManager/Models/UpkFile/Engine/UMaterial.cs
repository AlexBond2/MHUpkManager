using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("UMaterialInterface")]
    public class UMaterialInterface : USurface
    {

    }

    [UnrealClass("Material")]
    public class UMaterial : UMaterialInterface
    {

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
}
