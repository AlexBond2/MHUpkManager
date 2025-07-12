using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Objects;

namespace UpkManager.Models.UpkFile.Tables
{
    [Flags]
    public enum ExportFlags : uint
    {
        ForcedExport = 0x00000001U,
    }

    [Flags]
    public enum EPackageFlags: uint
    {
        None = 0x00000000,
        AllowDownload = 0x00000001,
        ClientOptional = 0x00000002,
        ServerSideOnly = 0x00000004,
        Cooked = 0x00000008,
        Unsecure = 0x00000010,
        Encrypted = 0x00000020, 
        Need = 0x00008000,
        Compiling = 0x00010000,
        ContainsMap = 0x00020000,
        Trashcan = 0x00040000,
        Loading = 0x00080000,
        PlayInEditor = 0x00100000,
        ContainsScript = 0x00200000,
        ContainsDebugData = 0x00400000,
        Imports = 0x00800000,
        Compressed = 0x02000000,
        FullyCompressed = 0x04000000,
        DynamicImports = 0x10000000,
        NoExportsData = 0x20000000,
        Stripped = 0x40000000,
        FilterEditorOnly = 0x80000000,
    }

    public sealed class UnrealExportTableEntry : UnrealExportTableEntryBuilderBase
    {

        #region Constructor

        internal UnrealExportTableEntry()
        {
            ObjectNameIndex = new UnrealNameTableIndex();
            NetObjects = [];
        }

        #endregion Constructor

        #region Properties

        public int ClassReference { get; private set; }
        public int SuperReference { get; private set; }
        //
        // OwnerReference in ObjectTableEntryBase
        // NameTableIndex in ObjectTableEntryBase
        //
        public int ArchetypeReference { get; private set; }
        public ulong ObjectFlags { get; private set; }
        public int SerialDataSize { get; private set; }
        public int SerialDataOffset { get; private set; }
        public uint ExportFlags { get; private set; }
        public int NetObjectCount { get; private set; }
        public List<int> NetObjects { get; private set; }
        public byte[] PackageGuid { get; private set; }
        public uint PackageFlags { get; private set; }

        #endregion Properties

        #region Unreal Properties

        public ByteArrayReader UnrealObjectReader { get; private set; }
        public UnrealObjectBase UnrealObject { get; private set; }
        public UnrealNameTableIndex ClassReferenceNameIndex { get; private set; }
        public UnrealNameTableIndex SuperReferenceNameIndex { get; private set; }
        public UnrealNameTableIndex OuterReferenceNameIndex { get; private set; }
        public UnrealNameTableIndex ArchetypeReferenceNameIndex { get; private set; }

        #endregion Unreal Properties

        #region Unreal Methods

        internal void ReadExportTableEntry(ByteArrayReader reader, UnrealHeader header)
        {
            ClassReference = reader.ReadInt32(); // ClassIndex
            SuperReference = reader.ReadInt32(); // SuperIndex
            OuterReference = reader.ReadInt32(); // OuterIndex

            ObjectNameIndex.ReadNameTableIndex(reader, header); // ObjectName

            ArchetypeReference = reader.ReadInt32(); // ArchetypeIndex

            ObjectFlags = reader.ReadUInt64(); // ObjectFlags

            SerialDataSize = reader.ReadInt32(); // SerialSize
            SerialDataOffset = reader.ReadInt32(); // SerialOffset

            ExportFlags = reader.ReadUInt32();

            NetObjects.Clear();
            int netObjectCount = reader.ReadInt32();
            for (int i = 0; i < netObjectCount; i++)
                NetObjects.Add(reader.ReadInt32());

            PackageGuid = reader.ReadBytes(16); // PackageGuid

            PackageFlags = reader.ReadUInt32(); // PackageFlags
        }

        internal void ExpandReferences(UnrealHeader header)
        {
            ClassReferenceNameIndex = header.GetObjectTableEntry(ClassReference)?.ObjectNameIndex;
            SuperReferenceNameIndex = header.GetObjectTableEntry(SuperReference)?.ObjectNameIndex;
            OuterReferenceNameIndex = header.GetObjectTableEntry(OuterReference)?.ObjectNameIndex;
            ArchetypeReferenceNameIndex = header.GetObjectTableEntry(ArchetypeReference)?.ObjectNameIndex;
        }

        internal void ReadUnrealObject(ByteArrayReader reader)
        {
            UnrealObjectReader = reader.Splice(SerialDataOffset, SerialDataSize);
        }

        public async Task ParseUnrealObject(UnrealHeader header, bool skipProperties, bool skipParse)
        {
            UnrealObject = objectTypeFactory();

            await UnrealObject.ReadUnrealObject(UnrealObjectReader, header, this, skipProperties, skipParse);
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int) * 7
                        + sizeof(uint) * 4
                        + ObjectNameIndex.GetBuilderSize()
                        + PackageGuid.Length
                        + NetObjects.Count * sizeof(int);

            return BuilderSize;
        }

        public override int GetObjectSize(int CurrentOffset)
        {
            if (UnrealObject == null) throw new Exception("All objects in file must be fully parsed before writing back to disk.");

            SerialDataOffset = BuilderSerialDataOffset = CurrentOffset;

            SerialDataSize = BuilderSerialDataSize = UnrealObject.GetBuilderSize();

            return BuilderSerialDataSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter writer, int currentOffset)
        {
            writer.WriteInt32(ClassReference);
            writer.WriteInt32(SuperReference);
            writer.WriteInt32(OuterReference);

            await ObjectNameIndex.WriteBuffer(writer, 0);

            writer.WriteInt32(ArchetypeReference);

            writer.WriteUInt64(ObjectFlags);

            writer.WriteInt32(BuilderSerialDataSize);
            writer.WriteInt32(BuilderSerialDataOffset);

            writer.WriteUInt32(ExportFlags);

            writer.WriteInt32(NetObjects.Count);
            foreach (var index in NetObjects)
                writer.WriteInt32(index);

            await writer.WriteBytes(PackageGuid);

            writer.WriteUInt32(PackageFlags);

        }

        public override async Task<ByteArrayWriter> WriteObjectBuffer()
        {
            ByteArrayWriter writer = ByteArrayWriter.CreateNew(SerialDataSize);

            await UnrealObject.WriteBuffer(writer, SerialDataOffset);

            return writer;
        }

        #endregion UnrealUpkBuilderBase Implementation

        #region Private Methods

        private UnrealObjectBase objectTypeFactory()
        {
            if (ClassReferenceNameIndex == null) return new UnrealObject<UClass>();

            string className = ClassReferenceNameIndex?.Name;

            if (Enum.TryParse(className, true, out PropertyTypes property))
            {
                return property switch
                {
                    PropertyTypes.ByteProperty => new UnrealObject<UByteProperty>(),
                    PropertyTypes.BoolProperty => new UnrealObject<UBoolProperty>(),
                    PropertyTypes.IntProperty => new UnrealObject<UIntProperty>(),
                    PropertyTypes.FloatProperty => new UnrealObject<UFloatProperty>(),
                    PropertyTypes.ObjectProperty => new UnrealObject<UObjectProperty>(),
                    PropertyTypes.ComponentProperty => new UnrealObject<UComponentProperty>(),
                    PropertyTypes.InterfaceProperty => new UnrealObject<UInterfaceProperty>(),
                    PropertyTypes.ClassProperty => new UnrealObject<UClassProperty>(),
                    PropertyTypes.NameProperty => new UnrealObject<UNameProperty>(),
                    PropertyTypes.StructProperty => new UnrealObject<UStructProperty>(),
                    PropertyTypes.StrProperty => new UnrealObject<UStrProperty>(),
                    PropertyTypes.ArrayProperty => new UnrealObject<UArrayProperty>(),
                    _ => new UnrealObject<UObject>(),
                };
            }

            Enum.TryParse(className, true, out ObjectTypes type);
            
            if (type == ObjectTypes.Unknown && ComponentRegistry.HasComponent(className))
                type = ObjectTypes.Component;

            return type switch
            {
 /*               ObjectTypes.ArchetypeObjectReference => new UnrealObjectArchetypeBase(),
                ObjectTypes.ObjectRedirector => new UnrealObjectObjectRedirector(),
                ObjectTypes.ShadowMapTexture2D => new UnrealObjectShadowMapTexture2D(),
                ObjectTypes.SoundNodeWave => new UnrealObjectSoundNodeWave(),
                ObjectTypes.Texture2D => new UnrealObjectTexture2D(),
                ObjectTypes.TextureMovie => new UnrealObjectTextureMovie(),*/
                ObjectTypes.Component => new UnrealObject<UComponent>(),
                ObjectTypes.Const => new UnrealObject<UConst>(),
                ObjectTypes.Enum => new UnrealObject<UEnum>(),
                ObjectTypes.ScriptStruct => new UnrealObject<UScriptStruct>(),
                _ => new UnrealObject<UObject>(),
            };
        }

        #endregion Private Methods

    }

}
