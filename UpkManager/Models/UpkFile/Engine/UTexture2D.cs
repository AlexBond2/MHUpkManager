using System;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Objects.Textures;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine
{
    public class UTexture2D : UTexture
    {
        [TreeNodeField("Texture2DMipMap")]
        public UArray<Texture2DMipMap> Mips { get; set; }

        [TreeNodeField]
        public Guid TextureFileCacheGuid { get; set; }

        [TreeNodeField("Texture2DMipMap")]
        public UArray<Texture2DMipMap> CachedPVRTCMips { get; set; }

        [TreeNodeField]
        public int CachedFlashMipMaxResolution { get; set; }

        [TreeNodeField("Texture2DMipMap")]
        public UArray<Texture2DMipMap> CachedATITCMips { get; set; }

        [TreeNodeField("UntypedBulkData")]
        public byte[] CachedFlashMipData { get; set; } // UntypedBulkData

        [TreeNodeField("Texture2DMipMap")]
        public UArray<Texture2DMipMap> CachedETCMips { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            Mips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);

            TextureFileCacheGuid = buffer.ReadGuid();
            CachedPVRTCMips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);

            CachedFlashMipMaxResolution = buffer.Reader.ReadInt32();
            CachedATITCMips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);
            CachedFlashMipData = buffer.ReadBulkData();

            CachedETCMips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);

            buffer.SetDataOffset();
        }
    }
}
