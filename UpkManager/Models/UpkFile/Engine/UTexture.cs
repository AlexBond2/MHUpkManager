using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    public class UTexture: USurface
    {
        [TreeNodeField("UntypedBulkData")]
        public byte[] SourceArt { get; set; } // UntypedBulkData

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            SourceArt = buffer.ReadBulkData();
        }
    }
}
