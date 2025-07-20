using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Compression;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Models.UpkFile.Types
{
    public class UBuffer(ByteArrayReader reader, UnrealHeader header)
    {
        public ByteArrayReader Reader = reader;
        public UnrealHeader Header = header;
        public bool IsAbstractClass = false;

        public ResultProperty ResultProperty { get; set; }
        public int DataOffset { get; private set; }
        public int DataSize { get; private set; }

        public List<T> ReadList<T>(Func<UBuffer, T> readMethod)
        {
            int count = Reader.ReadInt32();
            var list = new List<T>(count);
            for (int i = 0; i < count; i++)
                list.Add(readMethod(this));

            return list;
        }

        public UArray<T> ReadArray<T>(Func<UBuffer, T> readMethod)
        {
            int count = Reader.ReadInt32();
            var array = new UArray<T>(count);
            for (int i = 0; i < count; i++)
                array.Add(readMethod(this));

            return array;
        }

        public bool ReadBool()
        {
            int count = Reader.ReadInt32();
            return count == 1;
        }

        public UMap<UName, FName> ReadUMap()
        {
            int size = Reader.ReadInt32();
            UMap<UName, FName> map = new(size);
            for (var i = 0; i < size; i++)
            {
                var key = UName.ReadName(this);
                var value = ReadObject();
                map.Add(key, value);
            }
            return map;
        }

        public UMap<I, T> ReadMap<I, T>(Func<UBuffer, I> readKeys, Func<UBuffer, T> readValue)
        {
            int size = Reader.ReadInt32();
            UMap<I, T> map = new(size);
            for (var i = 0; i < size; i++)
            {
                I key = readKeys(this);
                T value = readValue(this);
                map.Add(key, value);
            }
            return map;
        }

        public FName ReadObject()
        {
            return Header.GetObjectTableEntry(Reader.ReadInt32())?.ObjectNameIndex;
        }

        public static FName ReadObject(UBuffer buffer)
        {
            return buffer.Header.GetObjectTableEntry(buffer.Reader.ReadInt32())?.ObjectNameIndex;
        }

        public string ReadString()
        {
            var ustring = new UnrealString();
            ustring.ReadString(Reader);
            return ustring.String;
        }

        public static string ReadString(UBuffer buffer)
        {
            var ustring = new UnrealString();
            ustring.ReadString(buffer.Reader);
            return ustring.String;
        }

        public ResultProperty ReadProperty(UnrealProperty property)
        {
            return property.ReadProperty(this);
        }

        public void SetDataOffset()
        {
            DataOffset = Reader.CurrentOffset;
            DataSize = Reader.Remaining;
        }

        public byte[] ReadBulkData()
        {
            var bulkData = new UnrealCompressedChunkBulkData();
            bulkData.ReadCompressedChunk(Reader);
            var reader = Task.Run(() => bulkData.DecompressChunk(0)).Result;
            return reader?.GetBytes();
        }

        public UArray<T> ReadBulkArray<T>() where T : unmanaged
        {
            var spanBytes = ReadBulkSpan<T>();
            return [.. spanBytes.ToArray()];
        }

        public UArray<byte[]> ReadArrayUnkElement()
        {
            int size = Reader.ReadInt32();

            int count = Reader.ReadInt32();
            var array = new UArray<byte[]>(count);
            for (int i = 0; i < count; i++)
                array.Add(Reader.ReadBytes(size));

            return array;
        }

        public UArray<T> ReadArrayElement<T>(Func<UBuffer, T> readMethod, int size)
        {
            int serializedElementSize = Reader.ReadInt32();
            int expectedSize = size;
            if (serializedElementSize != expectedSize)
                throw new InvalidOperationException($"Element size mismatch: serialized = {serializedElementSize}, expected = {expectedSize}");

            int count = Reader.ReadInt32();
            var array = new UArray<T>(count);
            for (int i = 0; i < count; i++)
                array.Add(readMethod(this));

            return array;
        }

        public Span<T> ReadBulkSpan<T>() where T : unmanaged
        {
            int serializedElementSize = Reader.ReadInt32();
            int expectedSize = Marshal.SizeOf<T>();
            if (serializedElementSize != expectedSize)
                throw new InvalidOperationException($"Element size mismatch: serialized = {serializedElementSize}, expected = {expectedSize}");

            int count = Reader.ReadInt32();
            int byteCount = count * expectedSize;

            byte[] bytes = Reader.ReadBytes(byteCount);
            return MemoryMarshal.Cast<byte, T>(bytes.AsSpan());
        }

        public System.Guid ReadGuid()
        {
            byte[] bytes = Reader.ReadBytes(16);
            return new System.Guid(bytes);
        }

        public byte[] ReadBytes()
        {
            int size = Reader.ReadInt32();
            return Reader.ReadBytes(size);
        }

        public FName ReadNameIndex()
        {
            var nameIndex = new FName();
            nameIndex.ReadNameTableIndex(Reader, Header);
            return nameIndex;
        }

        public int ReadInt32()
        {
            return Reader.ReadInt32();
        }

        public static int ReadInt32(UBuffer buffer)
        {
            return buffer.Reader.ReadInt32();
        }

        public static ushort ReadUInt16(UBuffer buffer)
        {
            return buffer.Reader.ReadUInt16();
        }

        public static uint ReadUInt32(UBuffer buffer)
        {
            return buffer.Reader.ReadUInt32();
        }

        public byte[] Read4Bytes()
        {
            var data = new byte[4];
            for (int i = 0; i < 4; i++)
                data[i] = Reader.ReadByte();

            return data;
        }

        public static float ReadFloat(UBuffer buffer)
        {
            return buffer.Reader.ReadSingle();
        }


        public static UArray<uint> ReadArrayUInt32(UBuffer buffer)
        {
            return buffer.ReadArray(ReadUInt32);
        }
    }
}
