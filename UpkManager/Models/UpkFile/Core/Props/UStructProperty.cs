using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Engine;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Core
{
    [UnrealClass("StructProperty")]
    public class UStructProperty : UProperty
    {
        [StructField("UStruct")]
        public FName Struct { get; private set; } // UStruct

        public override string PropertyString => Struct.Name;
        public UProperty StructValue { get; private set; }

        protected override VirtualNode GetVirtualTree()
        {
            var valueTree = base.GetVirtualTree();

            StructValue?.BuildVirtualTree(valueTree);

            return valueTree;
        }

        public override void ReadPropertyValue(UBuffer buffer, int size, UnrealProperty property)
        {
            Struct = buffer.ReadNameIndex();

            var structType = Struct.Name;
            if (EngineRegistry.Instance.TryGetStruct(structType, Parent, out var type))
            {
                StructValue = new EngineProperty(type);
                StructValue.ReadPropertyValue(buffer, size, property);
            }
            else if (CoreRegistry.Instance.TryGetProperty(structType, out var prop))
            {
                StructValue = new CoreProperty(prop, Parent);
                StructValue.ReadPropertyValue(buffer, size, property);
            }
            else
            {
                base.ReadPropertyValue(buffer, size, property);
            }
        }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            Struct = buffer.ReadObject();
        }
    }
}
