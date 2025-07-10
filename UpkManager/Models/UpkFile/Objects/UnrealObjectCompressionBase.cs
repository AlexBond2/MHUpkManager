using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Compression;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Objects
{

    public class UnrealObjectCompressionBase : UnrealObjectBase
    {

        #region Constructor

        public UnrealObjectCompressionBase()
        {
            CompressedChunks = new List<UnrealCompressedChunkBulkData>();
        }

        #endregion Constructor

        #region Properties

        protected byte[] Unknown1 { get; private set; }

        protected int CompressedChunkOffset { get; private set; }

        #endregion Properties

        #region Unreal Properties

        protected List<UnrealCompressedChunkBulkData> CompressedChunks { get; }

        #endregion Unreal Properties

        #region Unreal Methods

        public override async Task ReadUnrealObject(ByteArrayReader reader, UnrealHeader header, UnrealExportTableEntry export, bool skipProperties, bool skipParse)
        {
            await base.ReadUnrealObject(reader, header, export, skipProperties, skipParse);

            if (skipParse) return;

            Unknown1 = reader.ReadBytes(sizeof(uint) * 3);

            CompressedChunkOffset = reader.ReadInt32();
        }

        protected async Task ProcessCompressedBulkData(ByteArrayReader reader, Func<UnrealCompressedChunkBulkData, Task> chunkHandler)
        {
            UnrealCompressedChunkBulkData compressedChunk = new UnrealCompressedChunkBulkData();

            //    CompressedChunks.Add(compressedChunk);

            compressedChunk.ReadCompressedChunk(reader);

            await chunkHandler(compressedChunk);
        }

        protected async Task<int> ProcessUncompressedBulkData(ByteArrayReader reader, BulkDataCompressionTypes compressionFlags)
        {
            UnrealCompressedChunkBulkData compressedChunk = new UnrealCompressedChunkBulkData();

            CompressedChunks.Add(compressedChunk);

            int builderSize = await compressedChunk.BuildCompressedChunk(reader, compressionFlags);

            return builderSize;
        }

        protected int ProcessExistingBulkData(int index, ByteArrayReader reader, BulkDataCompressionTypes compressionFlags)
        {
            int builderSize = CompressedChunks[index].BuildExistingCompressedChunk(reader, compressionFlags);

            return builderSize;
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public override int GetBuilderSize()
        {
            return sizeof(uint) * 3
                 + sizeof(int);
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await Writer.WriteBytes(Unknown1);

            Writer.WriteInt32(CurrentOffset + Writer.Index + sizeof(int));
        }

        #endregion UnrealUpkBuilderBase Implementation

    }

}
