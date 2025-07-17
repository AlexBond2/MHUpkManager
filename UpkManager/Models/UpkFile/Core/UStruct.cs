using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [UnrealClass("Struct")]
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
}
