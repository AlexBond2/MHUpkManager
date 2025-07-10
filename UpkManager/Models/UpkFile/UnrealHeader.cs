using Str.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Compression;
using UpkManager.Models.UpkFile.Objects.Textures;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile
{

    public sealed class UnrealHeader : UnrealHeaderBuilderBase
    {

        #region Private Fields

        private ByteArrayReader reader;

        private ByteArrayWriter writer;

        #endregion Private Fields

        #region Constructor

        public UnrealHeader(ByteArrayReader Reader)
        {
            reader = Reader;

            Group = new UnrealString();

            GenerationTable = [];

            CompressedChunks = [];

            NameTable = [];

            ExportTable = [];
            ImportTable = [];
            DependsTable = [];

            AdditionalPackagesToCook = [];
            TextureAllocations = new();
        }

        #endregion Constructor

        #region Properties

        public uint Signature { get; private set; }

        public ushort Version { get; private set; }

        public ushort Licensee { get; private set; }

        public int Size { get; private set; }

        public UnrealString Group { get; }

        public uint Flags { get; private set; }

        public int NameTableCount { get; private set; }
        public int NameTableOffset { get; private set; }

        public int ExportTableCount { get; private set; }
        public int ExportTableOffset { get; private set; }

        public int ImportTableCount { get; private set; }
        public int ImportTableOffset { get; private set; }

        public int DependsTableOffset { get; private set; }
        public int ImportExportGuidsOffset { get; private set; }

        public int ImportGuidsCount { get; private set; }
        public int ExportGuidsCount { get; private set; }

        public int ThumbnailTableOffset { get; private set; }

        public byte[] Guid { get; private set; }

        public int GenerationTableCount { get; private set; }

        public List<UnrealGenerationTableEntry> GenerationTable { get; private set; }

        public uint EngineVersion { get; private set; }
        public uint CookerVersion { get; private set; }

        public uint CompressionFlags { get; private set; }
        public int CompressionTableCount { get; private set; }
        public List<UnrealCompressedChunk> CompressedChunks { get; private set; }

        public uint PackageSource { get; private set; }

        public List<UnrealString> AdditionalPackagesToCook { get; private set; }
        public UnrealTextureAllocations TextureAllocations { get; private set; }

        public List<UnrealNameTableEntry> NameTable { get; }

        public List<UnrealExportTableEntry> ExportTable { get; }

        public List<UnrealImportTableEntry> ImportTable { get; }

        public List<int> DependsTable { get; private set; } // (Size - DependsOffset) bytes; or ExportTableCount * 4 bytes;

        #endregion Properties

        #region Unreal Properties

        public string FullFilename { get; set; }

        public string Filename => Path.GetFileName(FullFilename);

        public long FileSize { get; set; }

        #endregion Unreal Properties

        #region Unreal Methods

        public async Task ReadHeaderAsync(Action<UnrealLoadProgress> progress)
        {
            var message = new UnrealLoadProgress { Progress = progress };
            message.Update("Parsing Header...");

            await readUpkHeader();

            const CompressionTypes validCompression = CompressionTypes.LZO | CompressionTypes.LZO_ENC;

            if (((CompressionTypes)CompressionFlags & validCompression) > 0)
            {
                message.Update("Decompressing...");
                reader = await decompressChunks();
            }
            else if (CompressionFlags > 0) throw new Exception($"Unsupported compression type 0x{CompressionFlags:X8}.");

            await readNameTable(progress);

            await readImportTable(progress);

            await readExportTable(progress);

            await ExportTable.ForEachAsync(export => Task.Run(() => export.ExpandReferences(this)));

            await ImportTable.ForEachAsync(import => Task.Run(() => import.ExpandReferences(this)));

            message.Update("Slicing and Dicing...");

            await readDependsTable();

            message.Total = ExportTableCount; 
            message.Update("Reading Objects...");
            
            await ExportTable.ForEachAsync(export =>
            {
                return export.ReadUnrealObject(reader).ContinueWith(t =>
                {
                    message.IncrementCurrent();
                });
            });

            message.Complete();
        }

        public UnrealObjectTableEntryBase GetObjectTableEntry(int reference)
        {
            if (reference == 0) return null;

            if (reference < 0 && -reference - 1 < ImportTableCount) return ImportTable[-reference - 1];
            if (reference > 0 && reference - 1 < ExportTableCount) return ExportTable[reference - 1];

            throw new Exception($"Object reference ({reference:X8}) is out of range of both the Import and Export Tables.");
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            if (CompressedChunks.Any()) throw new NotSupportedException("Cannot rebuild compressed files. Yet.");

            BuilderSize = sizeof(uint) * 7 // TODO recalc
                        + sizeof(ushort) * 2
                        + sizeof(int) * 10
                        + Group.GetBuilderSize()
                        + Guid.Length
                        + GenerationTable.Sum(gen => gen.GetBuilderSize());

            BuilderNameTableOffset = BuilderSize;

            BuilderSize += NameTable.Sum(name => name.GetBuilderSize());

            BuilderImportTableOffset = BuilderSize;

            BuilderSize += ImportTable.Sum(import => import.GetBuilderSize());

            BuilderExportTableOffset = BuilderSize;

            BuilderSize += ExportTable.Sum(export => export.GetBuilderSize());

            BuilderDependsTableOffset = BuilderSize;

            BuilderSize += DependsTable.Count * 4;

            ExportTable.Aggregate(BuilderSize, (current, export) => current + export.GetObjectSize(current));

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            writer = Writer;

            await writeUpkHeader();

            await writeNameTable();

            await writeImportTable();

            await writeExportTable();

            await writeDependsTable();
        }

        #endregion UnrealUpkBuilderBase Implementation

        #region Private Methods

        private async Task readUpkHeader()
        {
            reader.Seek(0);

            Signature = reader.ReadUInt32();

            if (Signature != Signatures.Signature) throw new Exception("File is not a properly formatted UPK file.");

            Version = reader.ReadUInt16();
            Licensee = reader.ReadUInt16();

            Size = reader.ReadInt32();

            await Group.ReadString(reader);

            Flags = reader.ReadUInt32();

            NameTableCount = reader.ReadInt32();
            NameTableOffset = reader.ReadInt32();

            ExportTableCount = reader.ReadInt32();
            ExportTableOffset = reader.ReadInt32();

            ImportTableCount = reader.ReadInt32();
            ImportTableOffset = reader.ReadInt32();

            DependsTableOffset = reader.ReadInt32();

            ImportExportGuidsOffset = reader.ReadInt32();
            ImportGuidsCount = reader.ReadInt32();
            ExportGuidsCount = reader.ReadInt32();

            ThumbnailTableOffset = reader.ReadInt32();

            Guid = await reader.ReadBytes(16);

            GenerationTableCount = reader.ReadInt32();

            GenerationTable = await readGenerationTable();

            EngineVersion = reader.ReadUInt32();
            CookerVersion = reader.ReadUInt32();

            CompressionFlags = reader.ReadUInt32();

            CompressionTableCount = reader.ReadInt32();

            CompressedChunks = await readCompressedChunksTable();

            PackageSource = reader.ReadUInt32();

            await readAdditionalPackagesToCook();

            TextureAllocations.ReadTextureAllocations(reader);
        }

        private async Task readAdditionalPackagesToCook()
        {
            AdditionalPackagesToCook.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var pakageToCook = new UnrealString();
                await pakageToCook.ReadString(reader);
                AdditionalPackagesToCook.Add(pakageToCook);
            }
        }

        private async Task writeAdditionalPackagesToCook()
        {
            writer.WriteInt32(AdditionalPackagesToCook.Count);
            foreach (var package in AdditionalPackagesToCook)
            {
                await package.WriteBuffer(writer, 0);
            }
        }

        private async Task writeUpkHeader()
        {
            writer.Seek(0);

            writer.WriteUInt32(Signature);

            writer.WriteUInt16(Version);
            writer.WriteUInt16(Licensee);

            writer.WriteInt32(BuilderSize);

            await Group.WriteBuffer(writer, 0);

            writer.WriteUInt32(Flags);

            writer.WriteInt32(NameTable.Count);
            writer.WriteInt32(BuilderNameTableOffset);

            writer.WriteInt32(ExportTable.Count);
            writer.WriteInt32(BuilderExportTableOffset);

            writer.WriteInt32(ImportTable.Count);
            writer.WriteInt32(BuilderImportTableOffset);

            writer.WriteInt32(BuilderDependsTableOffset);

            writer.WriteInt32(ImportExportGuidsOffset); // TODO BuilderOffset
            writer.WriteInt32(ImportGuidsCount);
            writer.WriteInt32(ExportGuidsCount);

            writer.WriteInt32(ThumbnailTableOffset); // TODO BuilderOffset

            await writer.WriteBytes(Guid);

            writer.WriteInt32(GenerationTable.Count);

            await writeGenerationTable();

            writer.WriteUInt32(EngineVersion);
            writer.WriteUInt32(CookerVersion);

            writer.WriteUInt32(CompressionFlags);

            writer.WriteInt32(CompressedChunks.Count); // ?

            writer.WriteUInt32(PackageSource);

            await writeAdditionalPackagesToCook();

            await TextureAllocations.WriteBuffer(writer, 0);
        }

        private async Task<List<UnrealGenerationTableEntry>> readGenerationTable()
        {
            List<UnrealGenerationTableEntry> generations = [];

            for (int i = 0; i < GenerationTableCount; ++i)
            {
                var info = new UnrealGenerationTableEntry();

                await Task.Run(() => info.ReadGenerationTableEntry(reader));

                generations.Add(info);
            }

            return generations;
        }

        private async Task writeGenerationTable()
        {
            foreach (UnrealGenerationTableEntry entry in GenerationTable)
            {
                await entry.WriteBuffer(writer, 0);
            }
        }

        private async Task<List<UnrealCompressedChunk>> readCompressedChunksTable()
        {
            List<UnrealCompressedChunk> chunks = [];

            for (int i = 0; i < CompressionTableCount; ++i)
            {
                var chunk = new UnrealCompressedChunk();

                await chunk.ReadCompressedChunk(reader);

                chunks.Add(chunk);
            }

            return chunks;
        }

        private async Task<ByteArrayReader> decompressChunks()
        {
            int start = CompressedChunks.Min(ch => ch.UncompressedOffset);

            int totalSize = CompressedChunks
                .SelectMany(ch => ch.Header.Blocks)
                .Sum(block => block.UncompressedSize) + start;

            byte[] data = new byte[totalSize];

            var chunkTasks = CompressedChunks.Select(async chunk =>
            {
                var blocks = chunk.Header.Blocks;

                int[] blockOffsets = new int[blocks.Count];
                int localOffset = 0;
                for (int i = 0; i < blocks.Count; i++)
                {
                    blockOffsets[i] = localOffset;
                    localOffset += blocks[i].UncompressedSize;
                }

                var blockTasks = blocks.Select((block, i) => Task.Run(async () =>
                {
                    if (((CompressionTypes)CompressionFlags & CompressionTypes.LZO_ENC) > 0)
                        await block.CompressedData.Decrypt();

                    byte[] decompressed = block.CompressedData.Decompress(block.UncompressedSize);

                    int writeOffset = chunk.UncompressedOffset + blockOffsets[i];

                    Array.ConstrainedCopy(decompressed, 0, data, writeOffset, block.UncompressedSize);
                })).ToArray();

                await Task.WhenAll(blockTasks);
            });

            await Task.WhenAll(chunkTasks);

            return ByteArrayReader.CreateNew(data, start);
        }

        private async Task readNameTable(Action<UnrealLoadProgress> progress)
        {
            var message = new UnrealLoadProgress { Text = "Reading Name Table...", Current = 0, Total = NameTableCount, Progress = progress };

            reader.Seek(NameTableOffset);

            for (int i = 0; i < NameTableCount; ++i)
            {
                var name = new UnrealNameTableEntry { TableIndex = i };

                await name.ReadNameTableEntry(reader);

                NameTable.Add(name);

                message.IncrementCurrent();
            }
        }

        private async Task writeNameTable()
        {
            foreach (UnrealNameTableEntry entry in NameTable)
            {
                await entry.WriteBuffer(writer, 0);
            }
        }

        private async Task readImportTable(Action<UnrealLoadProgress> progress)
        {
            var message = new UnrealLoadProgress { Text = "Reading Import Table...", Current = 0, Total = ImportTableCount, Progress = progress };

            reader.Seek(ImportTableOffset);

            for (int i = 0; i < ImportTableCount; ++i)
            {
                var import = new UnrealImportTableEntry { TableIndex = -(i + 1) };

                await import.ReadImportTableEntry(reader, this);

                ImportTable.Add(import);

                message.IncrementCurrent();
            }

            message.Current = 0;
            message.Total = 0;
            message.Update("Expanding References...");
        }

        private async Task writeImportTable()
        {
            foreach (UnrealImportTableEntry entry in ImportTable)
            {
                await entry.WriteBuffer(writer, 0);
            }
        }

        private async Task readExportTable(Action<UnrealLoadProgress> progress)
        {
            var message = new UnrealLoadProgress { Text = "Reading Export Table...", Current = 0, Total = ExportTableCount, Progress = progress };

            reader.Seek(ExportTableOffset);

            for (int i = 0; i < ExportTableCount; ++i)
            {
                var export = new UnrealExportTableEntry { TableIndex = i + 1 };

                await export.ReadExportTableEntry(reader, this);

                ExportTable.Add(export);

                message.IncrementCurrent();
            }

            message.Current = 0;
            message.Total = 0;
            message.Update("Expanding References...");
        }

        private async Task writeExportTable()
        {
            foreach (UnrealExportTableEntry entry in ExportTable)
            {
                await entry.WriteBuffer(writer, 0);
            }
        }

        private Task readDependsTable()
        {
            reader.Seek(DependsTableOffset);

            DependsTable.Clear();
            for (int i = 0; i < ExportTableCount; i++)
                DependsTable.Add(reader.ReadInt32());

            return Task.CompletedTask;
        }

        private Task writeDependsTable()
        {
            foreach (var value in DependsTable)
                writer.WriteInt32(value);

            return Task.CompletedTask;
        }

        #endregion Private Methods

    }

}
