using System.Text;
using System.Text.Json.Serialization;
using UpkManager.Models.UpkFile.Engine.Texture;

namespace MHUpkManager.TextureManager
{
    public enum ModResult
    {
        Success,
        NotMatch,
        TexutureNotFound,
        Reset
    }

    public class TextureManifest
    {
        public SortedDictionary<TextureHead, TextureEntry> Entries { get; private set; } = [];

        public int LoadManifest(string filePath)
        {
            Entries.Clear();

            using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                uint size = reader.ReadUInt32();

                for (uint i = 0; i < size; i++)
                {
                    var entry = new TextureEntry(reader, i);
                    Entries.Add(entry.Head, entry);
                }
            }

            return Entries.Count;
        }

        public TextureEntry GetTextureEntry(TextureHead head)
        {
            var matchedByName = Entries
                .Where(pair => string.Equals(pair.Key.TextureName, head.TextureName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var pair in matchedByName)
                if (pair.Key.TextureGuid == head.TextureGuid)
                    return pair.Value;

            foreach (var pair in Entries)
                if (pair.Key.TextureGuid == head.TextureGuid)
                    return pair.Value;

            return null;
        }
    }

    public class TextureEntry
    {
        public TextureHead Head;
        public TextureMipMaps Data;

        public TextureEntry(BinaryReader reader, uint index)
        {
            Head = new(reader, index);
            Data = new(this, reader);
        }

        public void Write(BinaryWriter writer)
        {
            Head.Write(writer);
            Data.Write(writer);
        }
    }

    public struct TextureHead : IComparable<TextureHead> 
    {
        public string TextureName { get; set; }
        public Guid TextureGuid { get; set; }

        [JsonIgnore]
        public uint HashIndex; // use HashIndex without the hash function

        public TextureHead(BinaryReader reader, uint index)
        {
            HashIndex = index;
            TextureName = ReadString(reader);
            TextureGuid = new Guid(reader.ReadBytes(16));
        }

        public TextureHead(string textureName, Guid guid) : this()
        {
            TextureName = textureName;
            TextureGuid = guid;
        }

        public override int GetHashCode()
        {
            // TODO Hash function https://github.com/stephank/surreal/blob/master/Core/Inc/UnFile.h#L331C14
            return TextureName?.GetHashCode() ?? 0;
        }

        public int CompareTo(TextureHead other)
        {
            return HashIndex.CompareTo(other.HashIndex);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            TextureHead other = (TextureHead)obj;
            return HashIndex == other.HashIndex;
        }

        public static string ReadString(BinaryReader reader)
        {
            uint length = reader.ReadUInt32();
            byte[] stringBytes = reader.ReadBytes((int)length);
            int nullIndex = Array.IndexOf(stringBytes, (byte)0);
            if (nullIndex >= 0)
                stringBytes = stringBytes[..nullIndex];
            return Encoding.UTF8.GetString(stringBytes);
        }

        public void Write(BinaryWriter writer)
        {
            WriteString(writer, TextureName);
            writer.Write(TextureGuid.ToByteArray());
        }

        public static void WriteString(BinaryWriter writer, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value + '\0');
            writer.Write((uint)stringBytes.Length);
            writer.Write(stringBytes);
        }
    }

    public class TextureMipMap
    {
        [JsonIgnore]
        public uint Index;
        public uint Offset { get; set; }
        public uint Size { get; set; }        
        
        [JsonIgnore]
        public TextureEntry Entry;

        public TextureMipMap() { }

        public TextureMipMap(TextureMipMap map)
        {
            Index = map.Index;
            Offset = map.Offset;
            Size = map.Size;
        }

        public TextureMipMap(TextureEntry entry, BinaryReader reader)
        {
            Entry = entry;
            Index = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
            Size = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return Index.ToString();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Offset);
            writer.Write(Size);
        }
    }

    public class TextureMipMaps
    {
        public string TextureFileName { get; set; }
        public List<TextureMipMap> Maps { get; set; }

        [JsonIgnore]
        public FTexture2DMipMap OverrideMipMap;

        public TextureMipMaps() { }

        public TextureMipMaps(TextureMipMaps data)
        {
            TextureFileName = data.TextureFileName;

            Maps = [];
            foreach (var map in data.Maps)
                Maps.Add(new(map));
        }

        public TextureMipMaps(TextureEntry entry, BinaryReader reader)
        {
            OverrideMipMap = new();
            TextureFileName = TextureHead.ReadString(reader);

            uint num = reader.ReadUInt32();

            Maps = [];
            for (uint i = 0; i < num; i++)
                Maps.Add(new(entry, reader));
        }

        public void Write(BinaryWriter writer)
        {
            TextureHead.WriteString(writer, TextureFileName);

            uint num = (uint)Maps.Count;
            writer.Write(num);

            foreach (var map in Maps)
                map.Write(writer);
        }

        public void SetData(TextureMipMaps updated)
        {
            int num = Maps.Count;
            for (int i = 0; i < num; i++)
            {
                Maps[i].Offset = updated.Maps[i].Offset;
                Maps[i].Size = updated.Maps[i].Size;
            }
        }
    }
}
