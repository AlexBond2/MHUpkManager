using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("StrProperty")]
    public class UStrProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.StrProperty;
    }
}
