using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Objects
{

    public sealed class UnrealObjectObjectRedirector : UnrealObjectBase
    {

        #region Properties

        public int ObjectTableReference { get; private set; }

        #endregion Properties

        #region Unreal Properties

        public override ObjectTypes ObjectType => ObjectTypes.ObjectRedirector;

        public UnrealNameTableIndex ObjectReferenceNameIndex { get; set; }

        #endregion Unreal Properties

        #region Unreal Methods

        public override async Task ReadUnrealObject(ByteArrayReader reader, UnrealHeader header, UnrealExportTableEntry export, bool skipProperties, bool skipParse)
        {
            await base.ReadUnrealObject(reader, header, export, skipProperties, skipParse);

            if (skipParse) return;

            ObjectTableReference = reader.ReadInt32();

            ObjectReferenceNameIndex = header.GetObjectTableEntry(ObjectTableReference)?.ObjectNameIndex;
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int);

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            Writer.WriteInt32(ObjectTableReference);
            await Task.CompletedTask;
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
