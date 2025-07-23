using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Classes
{
    [UnrealClass("Struct")]
    public class UStruct : UField
    {
        [StructField("UTextBuffer")]
        public FObject ScriptText { get; private set; }

        [StructField("UField")]
        public FObject Children { get; private set; }

        [StructField("UTextBuffer")]
        public FObject CppText { get; private set; }

        [StructField]
        public int Line { get; private set; }

        [StructField]
        public int TextPos { get; private set; }

        [StructField]
        public int ByteScriptSize { get; private set; }

        [StructField]
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
