using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("MapProperty")]
    public class UMapProperty : UProperty
    {
        [StructField("UProperty")]
        public FName Key { get; private set; } // UProperty

        [StructField("UProperty")]
        public FName Value { get; private set; } // UProperty

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Key = buffer.ReadObject();
            Value = buffer.ReadObject();
        }
    }
}
