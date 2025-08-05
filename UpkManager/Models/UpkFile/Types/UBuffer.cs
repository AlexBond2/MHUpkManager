using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Classes;
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

        public static bool ReadBool(UBuffer buffer) => buffer.ReadBool();

        public bool ReadAtomicBool()
        {
            byte count = Reader.ReadByte();
            return count == 1;
        }

        public byte ReadByte() => Reader.ReadByte();

        public UMap<UName, FObject> ReadUMap()
        {
            int size = Reader.ReadInt32();
            UMap<UName, FObject> map = new(size);
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

        public FObject ReadObject()
        {
            return Header.GetObjectTableEntry(Reader.ReadInt32())?.ObjectNameIndex;
        }

        public static FObject ReadObject(UBuffer buffer) => buffer.ReadObject();

        public string ReadString()
        {
            var ustring = new UnrealString();
            ustring.ReadString(Reader);
            return ustring.String;
        }

        public static string ReadString(UBuffer buffer) => buffer.ReadString();

        public ResultProperty ReadProperty(UnrealProperty property, UObject uObject)
        {
            return property.ReadProperty(this, uObject);
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

        public FGuid ReadGuid()
        {            
            return FGuid.ReadData(this);
        }

        public byte[] ReadBytes()
        {
            int size = Reader.ReadInt32();
            return Reader.ReadBytes(size);
        }

        public FName ReadName()
        {
            var nameIndex = new FName();
            nameIndex.ReadNameTableIndex(Reader, Header);
            return nameIndex;
        }

        public static FName ReadName(UBuffer buffer) => buffer.ReadName();

        public int ReadInt32() => Reader.ReadInt32();
        public static int ReadInt32(UBuffer buffer) => buffer.ReadInt32();

        public static ushort ReadUInt16(UBuffer buffer)
        {
            return buffer.Reader.ReadUInt16();
        }

        public static short ReadInt16(UBuffer buffer)
        {
            return buffer.Reader.ReadInt16();
        }

        public uint ReadUInt32() => Reader.ReadUInt32();
        public static uint ReadUInt32(UBuffer buffer) => buffer.ReadUInt32();

        public byte[] Read4Bytes()
        {
            var data = new byte[4];
            for (int i = 0; i < 4; i++)
                data[i] = Reader.ReadByte();

            return data;
        }

        public float ReadFloat() => Reader.ReadSingle();
        public static float ReadFloat(UBuffer buffer) => buffer.ReadFloat();

        public static UArray<uint> ReadArrayUInt32(UBuffer buffer)
        {
            return buffer.ReadArray(ReadUInt32);
        }

        public void SkipOffset(int skipOffset)
        {
            Reader.Seek(skipOffset - Reader.SpliceOffset);
        }
    }
}
