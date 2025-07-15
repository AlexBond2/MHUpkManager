using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Properties;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Objects
{
    public interface IUnrealObject
    {
        List<VirtualNode> FieldNodes { get; }
        public UBuffer Buffer { get; }
    }

    public class UnrealObject<T> : UnrealObjectBase, IUnrealObject where T : UObject, new()
    {
        protected List<VirtualNode> classNodes;
        public T UnrealType { get; set; }
        public UBuffer Buffer { get; set; }

        public List<VirtualNode> FieldNodes => GetFieldNodes();

        public UnrealObject()
        {
            UnrealType = new T();
        }

        private List<VirtualNode> GetFieldNodes()
        {
            classNodes ??= UnrealType.GetVirtualNode().Children;
            return classNodes;
        }

        public override Task ReadUnrealObject(ByteArrayReader reader, UnrealHeader header, UnrealExportTableEntry export, bool skipProperties, bool skipParse)
        {
            Buffer = new UBuffer(reader, header);
            UnrealType.ReadBuffer(Buffer);
            if (UnrealType.GetType().IsSubclassOf(typeof(UObject)))
                Buffer.SetDataOffset();
            return Task.CompletedTask;
        }

        public override Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            // TODO
            return Task.CompletedTask;
        }
    }
}
