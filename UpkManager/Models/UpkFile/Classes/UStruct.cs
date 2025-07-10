using System;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UStruct : UField
    {
        [TreeNodeField("UTextBuffer")]
        public UnrealNameTableIndex ScriptText { get; private set; }

        [TreeNodeField("UField")]
        public UnrealNameTableIndex Children { get; private set; }

        [TreeNodeField("UTextBuffer")]
        public UnrealNameTableIndex CppText { get; private set; }

        [TreeNodeField]
        public int Line { get; private set; }

        [TreeNodeField]
        public int TextPos { get; private set; }

        [TreeNodeField]
        public int ByteScriptSize { get; private set; }

        [TreeNodeField]
        public int DataScriptSize { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            SuperIndex = buffer.ReadObject();
            ScriptText = buffer.ReadObject();
            Children = buffer.ReadObject();
            CppText = buffer.ReadObject();
            Line = buffer.Reader.ReadInt32();
            TextPos = buffer.Reader.ReadInt32();
            ByteScriptSize = buffer.Reader.ReadInt32();
            DataScriptSize = buffer.Reader.ReadInt32();
        }
    }

    public class UScriptStruct : UStruct
    {
        [TreeNodeField]
        public StructFlags StructFlags { get; private set; }
        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);
            StructFlags = (StructFlags)buffer.Reader.ReadUInt32();
        }
    }

    [Flags]
    public enum StructFlags : uint
    {
        Native = 0x00000001U,
        Export = 0x00000002U,

        Long = 0x00000004U,      // @Redefined(UE3, HasComponents)
        Init = 0x00000008U,      // @Redefined(UE3, Transient)

        // UE3

        HasComponents = 0x00000004U,      // @Redefined
        Transient = 0x00000008U,      // @Redefined
        Atomic = 0x00000010U,
        Immutable = 0x00000020U,
        StrictConfig = 0x00000040U,
        ImmutableWhenCooked = 0x00000080U,
        AtomicWhenCooked = 0x00000100U,
    }
}
