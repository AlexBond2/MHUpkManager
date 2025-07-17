using UpkManager.Models.UpkFile.Classes;

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
}
