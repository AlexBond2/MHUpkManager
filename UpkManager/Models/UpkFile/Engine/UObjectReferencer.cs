using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    [UnrealClass("ObjectReferencer")]
    public class UObjectReferencer : UObject
    {
        [PropertyField]
        public UArray<FObject> ReferencedObjects { get; set; }
    }
}
