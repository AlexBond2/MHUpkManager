using System.Threading.Tasks;

using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Tables
{

    public sealed class UnrealImportTableEntry : UnrealObjectTableEntryBase
    {

        #region Constructor

        public UnrealImportTableEntry()
        {
            PackageNameIndex = new UnrealNameTableIndex();
            ClassNameIndex = new UnrealNameTableIndex();
            ObjectNameIndex = new UnrealNameTableIndex();
        }

        #endregion Constructor

        #region Properties

        public UnrealNameTableIndex PackageNameIndex { get; }

        public UnrealNameTableIndex ClassNameIndex { get; }
        //
        // OwnerReference in ObjectTableEntryBase
        //
        // NameTableIndex in ObjectTableEntryBase
        //
        #endregion Properties

        #region Unreal Properties

        public UnrealNameTableIndex OuterReferenceNameIndex { get; set; }

        #endregion Unreal Properties

        #region Unreal Methods

        public Task ReadImportTableEntry(ByteArrayReader reader, UnrealHeader header)
        {
            PackageNameIndex.ReadNameTableIndex(reader, header); // PackageName
            ClassNameIndex.ReadNameTableIndex(reader, header);   // ClassName
            OuterReference = reader.ReadInt32();                 // OuterIndex
            ObjectNameIndex.ReadNameTableIndex(reader, header);  // ObjectName

            return Task.CompletedTask;
        }

        public void ExpandReferences(UnrealHeader header)
        {
            OuterReferenceNameIndex = header.GetObjectTableEntry(OuterReference)?.ObjectNameIndex;
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = PackageNameIndex.GetBuilderSize()
                        + ClassNameIndex.GetBuilderSize()
                        + sizeof(int)
                        + ObjectNameIndex.GetBuilderSize();

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await PackageNameIndex.WriteBuffer(Writer, 0);

            await ClassNameIndex.WriteBuffer(Writer, 0);

            Writer.WriteInt32(OuterReference);

            await ObjectNameIndex.WriteBuffer(Writer, 0);
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
