using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("IntProperty")]
    public class UIntProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.IntProperty;
    }
}
