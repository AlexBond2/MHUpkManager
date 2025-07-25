using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [UnrealClass("Component")]
    public class UComponent : UObject
    {
        [StructField("UClass")]
        public FObject TemplateOwnerClass { get; set; } // UClass

        [StructField]
        public UName TemplateName { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            TemplateOwnerClass = buffer.ReadObject();
            TemplateName = UName.ReadName(buffer);
            base.ReadBuffer(buffer);
        }
    }

    [UnrealClass("DistributionFloat")]
    public class UDistributionFloat : UComponent
    {
    }

    [UnrealClass("DistributionVector")]
    public class UDistributionVector : UComponent
    {
    }
}
