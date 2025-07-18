using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("BoolProperty")]
    public class UBoolProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.BoolProperty;
    }
}
