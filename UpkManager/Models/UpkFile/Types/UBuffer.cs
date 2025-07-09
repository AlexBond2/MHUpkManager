using System.Collections.Generic;
using System.Threading.Tasks;
using System;

using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Properties;

namespace UpkManager.Models.UpkFile.Types
{
    public class UBuffer(ByteArrayReader reader, UnrealHeader header)
    {
        public ByteArrayReader Reader = reader;
        public UnrealHeader Header = header;
        public bool IsAbstractClass = false;

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
            return Task.Run(async () =>
            {
                var ustring = new UnrealString();
                await ustring.ReadString(Reader);
                return ustring.String;
            }).Result;
        }

        public bool ReadProperty(UnrealProperty property)
        {
            return Task.Run(async () =>
            {
                var result = await property.ReadProperty(Reader, Header);
                return result == ResultProperty.Success;
            }).Result;
        }
    }
}
