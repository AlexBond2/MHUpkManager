using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    public class UStruct : UField
    {
        public UnrealNameTableIndex ScriptText { get; private set; } // UTextBuffer
        public UnrealNameTableIndex CppText { get; private set; } // UTextBuffer
        public int Line { get; private set; }
        public int TextPos { get; private set; }
        public int ByteScriptSize { get; private set; }
        public int DataScriptSize { get; private set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            SuperIndex = buffer.ReadObject();
            ScriptText = buffer.ReadObject();
            CppText = buffer.ReadObject();
            Line = buffer.Reader.ReadInt32();
            TextPos = buffer.Reader.ReadInt32();
            ByteScriptSize = buffer.Reader.ReadInt32();
            DataScriptSize = buffer.Reader.ReadInt32();
        }
    }
}
