﻿using System;
using System.Threading.Tasks;

using UpkManager.Compression;


namespace UpkManager.Helpers
{
    public sealed class ByteArrayReader
    {

        #region Private Fields

        private byte[] data;

        private int index;

        private ILzoCompression compression;

        #endregion Private Fields

        #region Constructor

        private ByteArrayReader()
        {
            compression = new LzoCompression();
        }

        #endregion Constructor

        #region Public Methods

        public byte[] GetBytes()
        {
            return data;
        }

        public static ByteArrayReader CreateNew(byte[] Data, int Index)
        {
            ByteArrayReader reader = new ByteArrayReader();

            if (Data == null) Data = new byte[0];

            if (Index < 0 || Index > Data.Length) throw new ArgumentOutOfRangeException(nameof(Index), "Index value is outside the bounds of the byte array.");

            reader.Initialize(Data, Index);

            return reader;
        }

        public void Initialize(byte[] Data, int Index)
        {
            data = Data;

            if (Index < 0 || Index > data.Length) throw new ArgumentOutOfRangeException(nameof(Index), "Index value is outside the bounds of the byte array.");

            index = Index;
        }

        public void Seek(int Offset)
        {
            if (Offset < 0 || Offset > data.Length) throw new ArgumentOutOfRangeException(nameof(Offset), "Index value is outside the bounds of the byte array.");

            index = Offset;
        }

        public void Skip(int Count)
        {
            if (index + Count < 0 || index + Count > data.Length) throw new ArgumentOutOfRangeException(nameof(Count), "Index + Count is out of the bounds of the byte array.");

            index += Count;
        }

        public ByteArrayReader Branch(int Offset)
        {
            ByteArrayReader reader = new ByteArrayReader();

            if (Offset < 0 || Offset > data.Length) throw new ArgumentOutOfRangeException(nameof(Offset), "Index value is outside the bounds of the byte array.");

            reader.Initialize(data, Offset);

            return reader;
        }

        public async Task<ByteArrayReader> ReadByteArray(int Length)
        {
            if (index + Length < 0 || index + Length > data.Length) throw new ArgumentOutOfRangeException(nameof(Length), "Index + Length is out of the bounds of the byte array.");

            ByteArrayReader reader = new ByteArrayReader();

            reader.Initialize(await ReadBytes(Length), 0);

            return reader;
        }

        public async Task<ByteArrayReader> Splice()
        {
            return await Splice(index, data.Length - index);
        }

        public async Task<ByteArrayReader> Splice(int Offset, int Length)
        {
            if (Offset + Length < 0 || Offset + Length > data.Length) throw new ArgumentOutOfRangeException(nameof(Offset), "Offset + Length is out of the bounds of the byte array.");

            ByteArrayReader reader = new ByteArrayReader();

            reader.Initialize(await ReadBytes(Offset, Length), 0);

            return reader;
        }

        public async Task Encrypt()
        {
            await Decrypt();
        }

        public async Task Decrypt()
        {
            if (data.Length < 32) return;
            await Task.Run(() => Console.WriteLine("Put here your decrypt algorythm "));
        }

        public async Task<byte[]> Compress()
        {
            byte[] compressed = await compression.Compress(data);

            return compressed;
        }

        public async Task<byte[]> Decompress(int UncompressedSize)
        {
            byte[] decompressed = new byte[UncompressedSize];

            await compression.Decompress(data, decompressed);

            return decompressed;
        }

        public byte ReadByte()
        {
            byte value = data[index]; index += sizeof(byte);

            return value;
        }

        public short ReadInt16()
        {
            short value = BitConverter.ToInt16(data, index); index += sizeof(short);

            return value;
        }

        public ushort ReadUInt16()
        {
            ushort value = BitConverter.ToUInt16(data, index); index += sizeof(ushort);

            return value;
        }

        public int ReadInt32()
        {
            int value = BitConverter.ToInt32(data, index); index += sizeof(int);

            return value;
        }

        public uint ReadUInt32()
        {
            uint value = BitConverter.ToUInt32(data, index); index += sizeof(uint);

            return value;
        }

        public long ReadInt64()
        {
            long value = BitConverter.ToInt64(data, index); index += sizeof(long);

            return value;
        }

        public ulong ReadUInt64()
        {
            ulong value = BitConverter.ToUInt64(data, index); index += sizeof(ulong);

            return value;
        }

        public float ReadSingle()
        {
            float value = BitConverter.ToSingle(data, index); index += sizeof(float);

            return value;
        }

        public async Task<byte[]> ReadBytes(int Length)
        {
            if (Length == 0) return new byte[0];

            if (index + Length < 0 || index + Length > data.Length) throw new ArgumentOutOfRangeException(nameof(Length), "Index + Length is out of the bounds of the byte array.");

            byte[] value = new byte[Length];

            await Task.Run(() => { Array.ConstrainedCopy(data, index, value, 0, Length); index += Length; });

            return value;
        }

        public async Task<byte[]> ReadBytes(int Offset, int Length)
        {
            if (Offset + Length < 0 || Offset + Length > data.Length) throw new ArgumentOutOfRangeException(nameof(Offset), "Offset + Length is out of the bounds of the byte array.");

            byte[] value = new byte[Length];

            await Task.Run(() => Array.ConstrainedCopy(data, Offset, value, 0, Length));

            return value;
        }

        public int CurrentOffset => index;

        public int Remaining => data.Length - index;

        #endregion Public Methods

    }

}
