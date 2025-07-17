using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("ObjectRedirector")]
    public class UObjectRedirector : UObject
    {
        [TreeNodeField("UObject")]
        public UnrealNameTableIndex DestinationObject { get; private set; } // UObject

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            DestinationObject = buffer.ReadObject();
        }
    }
}
