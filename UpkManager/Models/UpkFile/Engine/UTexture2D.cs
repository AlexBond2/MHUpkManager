using DDSLib;
using DDSLib.Constants;
using System;
using System.IO;
using System.Linq;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Objects.Textures;
using UpkManager.Models.UpkFile.Properties;
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

            SetMipsFormat();

            TextureFileCacheGuid = buffer.ReadGuid();
            CachedPVRTCMips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);

            CachedFlashMipMaxResolution = buffer.Reader.ReadInt32();
            CachedATITCMips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);
            CachedFlashMipData = buffer.ReadBulkData();

            CachedETCMips = buffer.ReadArray(Texture2DMipMap.ReadMipMap);

            buffer.SetDataOffset();
        }

        private void SetMipsFormat()
        {
            if (GetProperty("Format").FirstOrDefault()?.Value is not UnrealPropertyByteValue formatProp) return;
            string format = formatProp.PropertyString;
            var imageFormat = DdsPixelFormat.ParseFileFormat(format);
            foreach (var mip in Mips)
                mip.OverrideFormat = imageFormat;
        }

        public Stream GetObjectStream()
        {
            if (Mips == null || Mips.Count == 0) return null;

            FileFormat format;

            Texture2DMipMap mipMap = Mips
                .Where(mm => mm.Data != null && mm.Data.Length > 0)
                .OrderByDescending(mm => mm.SizeX > mm.SizeY ? mm.SizeX : mm.SizeY)
                .FirstOrDefault();

            return mipMap == null ? null : buildDdsImage(Mips.IndexOf(mipMap), out format);
        }

        private Stream buildDdsImage(int mipMapIndex, out FileFormat imageFormat)
        {
            var mipMap = Mips[mipMapIndex];

            imageFormat = mipMap.OverrideFormat;  

            var ddsHeader = new DdsHeader(new DdsSaveConfig(imageFormat, 0, 0, false, false), mipMap.SizeX, mipMap.SizeY);
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            ddsHeader.Write(writer);
            stream.Write(mipMap.Data, 0, mipMap.Data.Length);

            stream.Flush();
            stream.Position = 0;

            return stream;
        }

        public Stream GetMipMapsStream()
        {
            if (Mips == null || Mips.Count == 0) return null;

            var orderedMipMaps = Mips.Where(mm => mm.Data != null && mm.Data.Length > 0).OrderByDescending(mip => mip.SizeX);

            Texture2DMipMap mipMap = orderedMipMaps.FirstOrDefault();

            var ddsHeader = new DdsHeader(new DdsSaveConfig(mipMap.OverrideFormat, 0, 0, false, false), mipMap.SizeX, mipMap.SizeY, orderedMipMaps.Count());
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);

            ddsHeader.Write(writer);
            foreach (var map in orderedMipMaps)
                stream.Write(map.Data, 0, map.Data.Length);

            stream.Flush();
            stream.Position = 0;

            return stream;
        }

        public Stream GetObjectStream(int mipMapIndex)
        {
            FileFormat format;
            return buildDdsImage(mipMapIndex, out format);
        }

        public string GetTextureFileCacheName()
        {
            if (GetProperty("TextureFileCacheName").FirstOrDefault()?.Value is not UnrealPropertyByteValue formatProp) return "None";
            return formatProp.PropertyString;
        }
    }
}
