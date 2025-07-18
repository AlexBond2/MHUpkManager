using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("NameProperty")]
    public class UNameProperty : UProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.NameProperty;
    }
}
