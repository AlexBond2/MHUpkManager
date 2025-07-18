using UpkManager.Constants;
using UpkManager.Models.UpkFile.Classes;

namespace UpkManager.Models.UpkFile.Core
{
[UnrealClass("ComponentProperty")]
    public class UComponentProperty : UObjectProperty
    {
        public override PropertyTypes PropertyType => PropertyTypes.ComponentProperty;
    }
}
