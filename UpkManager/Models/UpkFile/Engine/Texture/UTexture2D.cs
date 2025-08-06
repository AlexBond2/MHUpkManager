using DDSLib;
using DDSLib.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Compression;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Types;

namespace UpkManager.Models.UpkFile.Engine.Texture
{
    [UnrealClass("Texture2D")]
    public class UTexture2D : UTexture
    {
        [PropertyField]
        public int SizeX { get; set; }

        [PropertyField]
        public int SizeY { get; set; }

        [PropertyField]
        public int OriginalSizeX { get; set; }

        [PropertyField]
        public int OriginalSizeY { get; set; }

        [PropertyField]
        public EPixelFormat Format { get; set; }

        [PropertyField]
        public FName TextureFileCacheName { get; set; }

        [PropertyField]
        public int MipTailBaseIdx { get; set; }

        [PropertyField]
        public int FirstResourceMemMip { get; set; }

        [StructField("Texture2DMipMap")]
        public UArray<FTexture2DMipMap> Mips { get; set; }

        [StructField]
        public FGuid TextureFileCacheGuid { get; set; }

        [StructField("Texture2DMipMap")]
        public UArray<FTexture2DMipMap> CachedPVRTCMips { get; set; }

        [StructField]
        public int CachedFlashMipMaxResolution { get; set; }

        [StructField("Texture2DMipMap")]
        public UArray<FTexture2DMipMap> CachedATITCMips { get; set; }

        [StructField("UntypedBulkData")]
        public byte[] CachedFlashMipData { get; set; } // UntypedBulkData

        [StructField("Texture2DMipMap")]
        public UArray<FTexture2DMipMap> CachedETCMips { get; set; }

        public override void ReadBuffer(UBuffer buffer)
        {
            base.ReadBuffer(buffer);

            Mips = buffer.ReadArray(FTexture2DMipMap.ReadMipMap);

            SetMipsFormat();

            TextureFileCacheGuid = buffer.ReadGuid();
            CachedPVRTCMips = buffer.ReadArray(FTexture2DMipMap.ReadMipMap);

            CachedFlashMipMaxResolution = buffer.Reader.ReadInt32();
            CachedATITCMips = buffer.ReadArray(FTexture2DMipMap.ReadMipMap);
            CachedFlashMipData = buffer.ReadBulkData();

            CachedETCMips = buffer.ReadArray(FTexture2DMipMap.ReadMipMap);
        }

        private void SetMipsFormat()
        {
            var imageFormat = ParseFileFormat(Format);
            foreach (var mip in Mips)
                mip.OverrideFormat = imageFormat;
        }

        public static FileFormat ParseFileFormat(EPixelFormat format)
        {
            return format switch
            {
                EPixelFormat.PF_DXT1 => FileFormat.DXT1,
                EPixelFormat.PF_DXT3 => FileFormat.DXT3,
                EPixelFormat.PF_DXT5 => FileFormat.DXT5,
                EPixelFormat.PF_A8R8G8B8 => FileFormat.A8R8G8B8,
                EPixelFormat.PF_G8 => FileFormat.G8,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported DDS format")
            };
        }

        public Stream GetObjectStream()
        {
            if (Mips == null || Mips.Count == 0) return null;

            FileFormat format;

            FTexture2DMipMap mipMap = Mips
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

            FTexture2DMipMap mipMap = orderedMipMaps.FirstOrDefault();

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

        public int MipMapsCount { get; private set; }
        protected List<UnrealCompressedChunkBulkData> CompressedChunks { get; } = [];

        public void ResetMipMaps(int count)
        {
            Mips ??= [];
            Mips.Clear();
            MipMapsCount = count;
        }

        public async Task ReadMipMapCache(ByteArrayReader upkReader, uint index, FTexture2DMipMap overrideMipMap)
        {
            var header = new UnrealCompressedChunkHeader();

            header.ReadCompressedChunkHeader(upkReader, 1, 0, 0);

            if (TryGetImageProperties(header, (int)index, overrideMipMap, out int width, out int height, out FileFormat format))
            {
                FTexture2DMipMap mip = new()
                {
                    SizeX = width,
                    SizeY = height,
                    OverrideFormat = format
                };

                if (mip.SizeX >= 4 || mip.SizeY >= 4)
                {
                    var decompressedData = await header.DecompressChunk().ConfigureAwait(false);
                    mip.Data = decompressedData?.GetBytes();
                }

                Mips.Add(mip);
            }
        }

        public bool TryGetImageProperties(UnrealCompressedChunkHeader header, int index, FTexture2DMipMap overrideMipMap, out int width, out int height, out FileFormat ddsFormat)
        {
            if (overrideMipMap.SizeX > 0)
            {
                int shift = index;
                if (shift < 0)
                {
                    width = overrideMipMap.SizeX << -shift;
                    height = overrideMipMap.SizeY << -shift;
                }
                else
                {
                    width = overrideMipMap.SizeX >> shift;
                    height = overrideMipMap.SizeY >> shift;
                }
                ddsFormat = overrideMipMap.OverrideFormat;
                return true;
            }

            width = 0;
            height = 0;
            ddsFormat = 0;

            return false;
        }

        public byte[] WriteMipMapChache(int index)
        {
            if (index >= Mips.Count) return [];

            // build compressed Chunks
            int dataSize = GetCompressedMipMapSize(index);
            if (dataSize == 0) return [];

            // write header and chunks
            var writer = ByteArrayWriter.CreateNew(dataSize);
            if (CompressedChunks.Count <= index) return [];
            CompressedChunks[index].Header.WriteCompressedChunkHeader(writer, 0).Wait();

            // write compressed data in stream
            return writer.GetBytes();
        }
        protected int BuilderSize { get; set; }

        public int GetCompressedMipMapSize(int index)
        {
            if (index >= Mips.Count) return 0;

            BuilderSize = GetBuilderSize() + sizeof(int); // need sizeof(int)?
            var mipMap = Mips[index];

            BulkDataCompressionTypes flags = mipMap.Data == null ||
                mipMap.Data.Length == 0
                ? BulkDataCompressionTypes.Unused | BulkDataCompressionTypes.StoreInSeparatefile
                : BulkDataCompressionTypes.LZO_ENC;

            BuilderSize += Task.Run(() => ProcessUncompressedBulkData(ByteArrayReader.CreateNew(mipMap.Data, 0), flags)).Result
                        + sizeof(int) * 2;

            return BuilderSize;
        }

        protected async Task<int> ProcessUncompressedBulkData(ByteArrayReader reader, BulkDataCompressionTypes compressionFlags)
        {
            var compressedChunk = new UnrealCompressedChunkBulkData();
            CompressedChunks.Add(compressedChunk);
            int builderSize = await compressedChunk.BuildCompressedChunk(reader, compressionFlags);
            return builderSize;
        }

        public int GetBuilderSize()
        {
            return sizeof(uint) * 3
                 + sizeof(int);
        }

        public void ResetCompressedChunks()
        {
            CompressedChunks.Clear();
        }

        public void ExpandMipMaps(int count, List<DdsMipMap> mipMaps)
        {
            MipMapsCount = count;
            int maxIndex = Mips.Count;
            var format = Mips[0].OverrideFormat;
            for (int index = maxIndex; index < MipMapsCount; index++)
            {
                FTexture2DMipMap mip = new FTexture2DMipMap
                {
                    SizeX = mipMaps[index].Width,
                    SizeY = mipMaps[index].Height,
                    OverrideFormat = format
                };
                Mips.Add(mip);
            }
        }
    }

    public enum EPixelFormat
    {
        PF_Unknown,                     // 0
        PF_A32B32G32R32F,               // 1
        PF_A8R8G8B8,                    // 2
        PF_G8,                          // 3
        PF_G16,                         // 4
        PF_DXT1,                        // 5
        PF_DXT3,                        // 6
        PF_DXT5,                        // 7
        PF_UYVY,                        // 8
        PF_FloatRGB,                    // 9
        PF_FloatRGBA,                   // 10
        PF_DepthStencil,                // 11
        PF_ShadowDepth,                 // 12
        PF_FilteredShadowDepth,         // 13
        PF_R32F,                        // 14
        PF_G16R16,                      // 15
        PF_G16R16F,                     // 16
        PF_G16R16F_FILTER,              // 17
        PF_G32R32F,                     // 18
        PF_A2B10G10R10,                 // 19
        PF_A16B16G16R16,                // 20
        PF_D24,                         // 21
        PF_R16F,                        // 22
        PF_R16F_FILTER,                 // 23
        PF_BC5,                         // 24
        PF_V8U8,                        // 25
        PF_A1,                          // 26
        PF_FloatR11G11B10,              // 27
        PF_A4R4G4B4,                    // 28
        PF_R5G6B5,                      // 29
        PF_MAX                          // 30
    };
}
