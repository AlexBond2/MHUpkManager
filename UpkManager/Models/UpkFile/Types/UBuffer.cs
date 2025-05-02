using System.Collections.Generic;
using System;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;

namespace UpkManager.Models.UpkFile.Types
{
    public class UBuffer(ByteArrayReader reader, UnrealHeader header)
    {
        public ByteArrayReader Reader = reader;
        public UnrealHeader Header = header;

        public List<T> ReadList<T>(Func<UBuffer, T> readMethod)
        {
            int count = Reader.ReadInt32();
            var list = new List<T>(count);
            for (int i = 0; i < count; i++)
                list.Add(readMethod(this));

            return list;
        }

        public bool ReadBool()
        {
            int count = Reader.ReadInt32();
            return count == 1;
        }

        public UMap<UName, UnrealNameTableIndex> ReadMap()
        {
            int size = Reader.ReadInt32();
            UMap<UName, UnrealNameTableIndex> map = new(size);
            for (var i = 0; i < size; ++i)
            {
                var key = UName.ReadName(this);
                var value = ReadObject();
                map.Add(key, value);
            }
            return map;
        }

        public UnrealNameTableIndex ReadObject()
        {
            return Header.GetObjectTableEntry(Reader.ReadInt32())?.ObjectNameIndex;
        }

        public string ReadString()
        {
            UnrealString ustring = new();
            ustring.ReadString(Reader).GetAwaiter().GetResult();
            return ustring.String;
        }
    }
}
