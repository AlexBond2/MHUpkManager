using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UpkManager.Constants;
using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Compression
{

    public class UnrealCompressedChunkHeader
    {

        #region Constructor

        public UnrealCompressedChunkHeader()
        {
            Blocks = new List<UnrealCompressedChunkBlock>();
        }

        #endregion Constructor

        #region Properties

        public uint Signature { get; set; }
        public int BlockSize { get; set; }
        public int CompressedSize { get; set; }
        public int UncompressedSize { get; set; }

        public List<UnrealCompressedChunkBlock> Blocks { get; set; }

        #endregion Properties

        #region Unreal Methods

        public async Task ReadCompressedChunkHeader(ByteArrayReader reader, uint flags, int uncompressedSize, int compressedSize)
        {
            if (flags > 0)
            {
                Signature = reader.ReadUInt32();

                if (Signature != Signatures.Signature) throw new Exception("Compressed Header Signature not found.");

                BlockSize = reader.ReadInt32();

                CompressedSize = reader.ReadInt32();
                UncompressedSize = reader.ReadInt32();

                Blocks.Clear();

                int blockCount = (UncompressedSize + BlockSize - 1) / BlockSize;

                for (int i = 0; i < blockCount; ++i)
                {
                    UnrealCompressedChunkBlock block = new();

                    block.ReadCompressedChunkBlock(reader);

                    Blocks.Add(block);
                }
            }
            else
            {
                Blocks = [
                  new UnrealCompressedChunkBlock {
                    UncompressedSize = uncompressedSize,
                      CompressedSize =   compressedSize
                  }
                ];
            }

            foreach (UnrealCompressedChunkBlock block in Blocks) await block.ReadCompressedChunkBlockData(reader);
        }

        public async Task<int> BuildCompressedChunkHeader(ByteArrayReader reader, uint flags)
        {
            Signature = Signatures.Signature;
            BlockSize = 0x00020000;

            CompressedSize = 0;
            UncompressedSize = reader.Remaining;

            int blockCount = (reader.Remaining + BlockSize - 1) / BlockSize;

            int builderSize = 0;

            Blocks.Clear();

            for (int i = 0; i < blockCount; ++i)
            {
                UnrealCompressedChunkBlock block = new UnrealCompressedChunkBlock();

                ByteArrayReader uncompressed = await reader.ReadByteArray(Math.Min(BlockSize, reader.Remaining));

                builderSize += await block.BuildCompressedChunkBlockData(uncompressed);

                CompressedSize += block.CompressedSize;

                Blocks.Add(block);
            }

            builderSize += sizeof(uint)
                        + sizeof(int) * 3;

            return builderSize;
        }

        public int BuildExistingCompressedChunkHeader(int uncompressedSize)
        {
            Signature = Signatures.Signature;
            BlockSize = 0x00020000;

            CompressedSize = 0;
            UncompressedSize = uncompressedSize;

            int blockCount = (uncompressedSize + BlockSize - 1) / BlockSize;

            int builderSize = 0;

            for (int i = 0; i < blockCount; ++i)
            {
                builderSize += Blocks[i].BuildExistingCompressedChunkBlockData();

                CompressedSize += Blocks[i].CompressedSize;
            }

            builderSize += sizeof(uint)
                        + sizeof(int) * 3;

            return builderSize;
        }

        public async Task WriteCompressedChunkHeader(ByteArrayWriter Writer, int CurrentOffset)
        {
            Writer.WriteUInt32(Signature);

            Writer.WriteInt32(BlockSize);

            Writer.WriteInt32(CompressedSize);
            Writer.WriteInt32(UncompressedSize);

            foreach (UnrealCompressedChunkBlock block in Blocks) 
                await block.WriteCompressedChunkBlock(Writer);

            foreach (UnrealCompressedChunkBlock block in Blocks) 
                await block.WriteCompressedChunkBlockData(Writer);
        }

        public Task<ByteArrayReader> DecompressChunk()
        {
            var blockOffsets = new int[Blocks.Count];
            int totalSize = 0;

            for (int i = 0; i < Blocks.Count; i++)
            {
                blockOffsets[i] = totalSize;
                totalSize += Blocks[i].UncompressedSize;
            }

            byte[] chunkData = new byte[totalSize];

            var tasks = Blocks.Select((block, index) => Task.Run(() =>
            {
                byte[] decompressed = block.CompressedData.Decompress(block.UncompressedSize);
                int offset = blockOffsets[index];
                Array.ConstrainedCopy(decompressed, 0, chunkData, offset, block.UncompressedSize);
            }));

            return Task.WhenAll(tasks).ContinueWith(_ => ByteArrayReader.CreateNew(chunkData, 0));
        }

        #endregion Unreal Methods

    }

}
