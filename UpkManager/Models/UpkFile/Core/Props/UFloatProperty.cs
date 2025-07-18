using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("FloatProperty")]
    public class UFloatProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.FloatProperty;
    }
}
