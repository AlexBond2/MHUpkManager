using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Properties;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Objects
{
    public class UnrealClassObject : UnrealObjectBase
    {
        public UnrealClassObject() 
        {
            Class = new UClass();
        }

        private List<VirtualNode> classNodes;
        public UClass Class { get; set; }
        public List<VirtualNode> FieldNodes { get => GetFieldNodes(); }
        public UBuffer Buffer { get; set; }

        private List<VirtualNode> GetFieldNodes()
        {
            classNodes ??= Class.GetVirtualNode().Children;
            return classNodes;
        }

        public override Task ReadUnrealObject(ByteArrayReader reader, UnrealHeader header, UnrealExportTableEntry export, bool skipProperties, bool skipParse)
        {
            Buffer = new (reader, header);
            Class.ReadBuffer(Buffer);

            return Task.CompletedTask;
        }

        public override Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            // TODO

            return Task.CompletedTask;
        }
    }
}
