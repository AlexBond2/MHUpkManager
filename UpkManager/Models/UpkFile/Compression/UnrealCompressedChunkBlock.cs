﻿using System.Threading.Tasks;

using UpkManager.Helpers;


namespace UpkManager.Models.UpkFile.Compression
{

    public class UnrealCompressedChunkBlock
    {

        #region Properties

        public int CompressedSize { get; set; }
        public int UncompressedSize { get; set; }
        public ByteArrayReader CompressedData { get; set; }

        #endregion Properties

        #region Unreal Methods

        public void ReadCompressedChunkBlock(ByteArrayReader reader)
        {
            CompressedSize = reader.ReadInt32();
            UncompressedSize = reader.ReadInt32();
        }

        public void ReadCompressedChunkBlockData(ByteArrayReader reader)
        {
            CompressedData = reader.ReadByteArray(CompressedSize);
        }

        public async Task<int> BuildCompressedChunkBlockData(ByteArrayReader reader)
        {
            UncompressedSize = reader.Remaining;

            byte[] compressed = await reader.Compress();

            CompressedData = ByteArrayReader.CreateNew(compressed, 0);

            //await CompressedData.Encrypt(); // TODO: Fix this to use the flag

            CompressedSize = CompressedData.Remaining;

            return CompressedSize + sizeof(int) * 2;
        }

        public int BuildExistingCompressedChunkBlockData()
        {
            //await CompressedData.Encrypt();

            return CompressedSize + sizeof(int) * 2;
        }

        public async Task WriteCompressedChunkBlock(ByteArrayWriter Writer)
        {
            await Task.Run(() =>
            {
                Writer.WriteInt32(CompressedSize);
                Writer.WriteInt32(UncompressedSize);
            });
        }

        public async Task WriteCompressedChunkBlockData(ByteArrayWriter Writer)
        {
            await Writer.WriteBytes(CompressedData.GetBytes());
        }

        #endregion Unreal Methods

    }

}
