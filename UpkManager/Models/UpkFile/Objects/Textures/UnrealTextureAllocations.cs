using System.Collections.Generic;
using System.Threading.Tasks;
using UpkManager.Helpers;

namespace UpkManager.Models.UpkFile.Objects.Textures
{
    public class UnrealTextureAllocations : UnrealUpkBuilderBase
    {
        public List<UnrealTextureType> TextureTypes { get; set; } = [];
        public int Count { get =>  TextureTypes.Count; }

        public override int GetBuilderSize()
        {
            BuilderSize = 0;

            foreach (var textureType in TextureTypes)
                BuilderSize += textureType.GetBuilderSize();

            return BuilderSize;
        }

        public void ReadTextureAllocations(ByteArrayReader reader)
        {
            TextureTypes.Clear();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var textureType = new UnrealTextureType();
                textureType.ReadTextureType(reader);
                TextureTypes.Add(textureType);
            }
        }

        public override async Task WriteBuffer(ByteArrayWriter writer, int currentOffset)
        {
            writer.WriteInt32(Count);
            foreach (var textureType in TextureTypes) 
                await textureType.WriteBuffer(writer, currentOffset);
        }
    }

    public class UnrealTextureType : UnrealUpkBuilderBase
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int MipMapsCount { get; set; }
        public uint TextureFormat { get; set; }
        public uint TextureCreateFlags { get; set; }
        public List<int> TextureIndices { get; set; } = [];

        public override int GetBuilderSize()
        {
            BuilderSize = sizeof(int) * 3  // Width, Height, MipMapsCount
                         + sizeof(uint) * 2  // TextureFormat, TextureCreateFlags
                         + TextureIndices.Count * sizeof(int);  // TextureIndices

            return BuilderSize;
        }

        public void ReadTextureType(ByteArrayReader reader)
        {
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            MipMapsCount = reader.ReadInt32();

            TextureFormat = reader.ReadUInt32();
            TextureCreateFlags = reader.ReadUInt32();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                TextureIndices.Add(reader.ReadInt32());
        }

        public override Task WriteBuffer(ByteArrayWriter writer, int CurrentOffset)
        {
            writer.WriteInt32(Width);
            writer.WriteInt32(Height);
            writer.WriteInt32(MipMapsCount);

            writer.WriteUInt32(TextureFormat);
            writer.WriteUInt32(TextureCreateFlags);

            writer.WriteInt32(TextureIndices.Count);
            foreach (var index in TextureIndices)
                writer.WriteInt32(index);

            return Task.CompletedTask;
        }
    }
}
