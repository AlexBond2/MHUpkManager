using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UComponent : UObject
    {
        [TreeNodeField("UClass")]
        public UnrealNameTableIndex TemplateOwnerClass { get; set; } // UClass

        [TreeNodeField]
        public UName TemplateName { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            TemplateOwnerClass = buffer.ReadObject();
            TemplateName = UName.ReadName(buffer);
            base.ReadBuffer(buffer);
        }
    }
}
