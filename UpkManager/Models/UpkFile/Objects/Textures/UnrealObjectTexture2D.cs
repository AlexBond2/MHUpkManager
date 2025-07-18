using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DDSLib;
using DDSLib.Constants;

using UpkManager.Constants;
using UpkManager.Helpers;
using UpkManager.Models.UpkFile.Compression;
using UpkManager.Models.UpkFile.Tables;


namespace UpkManager.Models.UpkFile.Objects.Textures
{

    public class UnrealObjectTexture2D : UnrealObjectCompressionBase
    {

        #region Constructor

        public UnrealObjectTexture2D()
        {
            MipMaps = new();
        }

        #endregion Constructor

        #region Properties

        public int MipMapsCount { get; private set; }

        public List<Texture2DMipMap> MipMaps { get; }

        public byte[] Guid { get; private set; }

        #endregion Properties

        #region Unreal Properties

        public override bool IsExportable => true;

        public override ViewableTypes Viewable => ViewableTypes.Image;

        public override ObjectTypes ObjectType => ObjectTypes.Texture2D;

        public override string FileExtension => ".dds";

        public override string FileTypeDesc => "Direct Draw Surface";

        #endregion Unreal Properties

        #region Unreal Methods

        public override async Task ReadUnrealObject(ByteArrayReader reader, UnrealHeader header, UnrealExportTableEntry export, bool skipProperties, bool skipParse)
        {
            await base.ReadUnrealObject(reader, header, export, skipProperties, skipParse);

            if (skipParse) return;

            MipMapsCount = reader.ReadInt32();

            for (int i = 0; i < MipMapsCount; ++i)
            {
                await ProcessCompressedBulkData(reader, async bulkChunk =>
                {
                    Texture2DMipMap mip = new Texture2DMipMap
                    {
                        SizeX = reader.ReadInt32(),
                        SizeY = reader.ReadInt32()
                    };

                    if (mip.SizeX >= 4 || mip.SizeY >= 4) 
                        mip.Data = (await bulkChunk.DecompressChunk(0))?.GetBytes();

                    MipMaps.Add(mip);
                });
            }

            Guid = reader.ReadBytes(16);
        }

        public void ResetMipMaps(int count)
        {
            MipMaps.Clear();
            MipMapsCount = count;
        }

        public async Task ReadMipMapCache(ByteArrayReader upkReader, uint index, Texture2DMipMap overrideMipMap)
        {
            var header = new UnrealCompressedChunkHeader();

            header.ReadCompressedChunkHeader(upkReader, 1, 0, 0);

            if (TryGetImageProperties(header, (int)index, overrideMipMap, out int width, out int height, out FileFormat format))
            {
                Texture2DMipMap mip = new()
                {
                    SizeX = width,
                    SizeY = height,
                    OverrideFormat = format
                };

                if (mip.SizeX >= 4 || mip.SizeY >= 4) 
                    mip.Data = (await header.DecompressChunk())?.GetBytes();

                MipMaps.Add(mip);
            }
        }

        public bool TryGetImageProperties(UnrealCompressedChunkHeader header, int index, Texture2DMipMap overrideMipMap, out int width, out int height, out FileFormat ddsFormat)
        {
            if (overrideMipMap.SizeX > 0)
            {
                int shift = index - overrideMipMap.Data[0];
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

            switch (header.Blocks.Count)
            {
                case 2:
                    width = 512;
                    height = 512;
                    ddsFormat = FileFormat.DXT5;
                    break;
                case 4:
                    width = 1024;
                    height = 1024;
                    ddsFormat = FileFormat.DXT1;
                    break;
                case 8:
                    width = 1024;
                    height = 1024;
                    ddsFormat = FileFormat.DXT5;
                    break;
                case 16:
                    width = 2048;
                    height = 2048;
                    ddsFormat = FileFormat.DXT1;
                    break;
                case 32:
                    width = 2048;
                    height = 2048;
                    ddsFormat = FileFormat.DXT5;
                    break;
                case 64:
                    width = 4096;
                    height = 4096;
                    ddsFormat = FileFormat.DXT1;
                    break;
                case 128:
                    width = 4096;
                    height = 4096;
                    ddsFormat = FileFormat.DXT5;
                    break;
                default:
                    switch (header.Blocks[0].UncompressedSize)
                    {
                        case 0x0200:
                            width = 32;
                            height = 32;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x0800:
                            width = 64;
                            height = 64;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x0FF0:
                            width = 68;
                            height = 60;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x1000:
                            width = 64;
                            height = 64;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x2000:
                            width = 128;
                            height = 128;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x2420:
                            width = 136;
                            height = 136;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x2FD0:
                            width = 180;
                            height = 136;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x3200:
                            width = 158;
                            height = 158;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x4000:
                            width = 128;
                            height = 128;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x5F00:
                            width = 152;
                            height = 160;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x5FA0:
                            width = 180;
                            height = 136;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x8000:
                            width = 256;
                            height = 256;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0xBF40:
                            width = 360;
                            height = 272;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0xDAC0:
                            width = 280;
                            height = 400;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x10000:
                            width = 256;
                            height = 256;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x17C00:
                            width = 152;
                            height = 160;
                            ddsFormat = FileFormat.A8R8G8B8;
                            break;
                        case 0x1EC30:
                            width = 300;
                            height = 420;
                            ddsFormat = FileFormat.DXT5;
                            break;
                        case 0x1F5B8:
                            width = 676;
                            height = 380;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        case 0x20000:
                            width = 512;
                            height = 512;
                            ddsFormat = FileFormat.DXT1;
                            break;
                        default:
                            width = 0;
                            height = 0;
                            ddsFormat = FileFormat.Unknown;
                            return false;
                    }
                    break;
            }
            return true;
        }

        public override async Task SaveObject(string filename, object configuration)
        {
            if (MipMaps == null || !MipMaps.Any()) return;

         /*   DdsSaveConfig config = configuration as DdsSaveConfig ?? new DdsSaveConfig(FileFormat.Unknown, 0, 0, false, false);

            FileFormat format;

            Texture2DMipMap mipMap = MipMaps
                .Where(mm => mm.Data != null && mm.Data.Length > 0)
                .OrderByDescending(mm => mm.SizeX > mm.SizeY ? mm.SizeX : mm.SizeY).FirstOrDefault();

            if (mipMap == null) return;

            Stream memory = buildDdsImage(MipMaps.IndexOf(mipMap), out format);

            if (memory == null) return;

            DdsFile ddsImage = new DdsFile(memory);

            FileStream ddsStream = new FileStream(filename, FileMode.Create);

            config.FileFormat = format;

            await Task.Run(() => ddsImage.Save(ddsStream, config));

            ddsStream.Close();

            memory.Close();*/
            await Task.CompletedTask;
        }

        public override async Task SetObject(string filename, List<UnrealNameTableEntry> nameTable, object configuration)
        {
            DdsSaveConfig config = configuration as DdsSaveConfig ?? new DdsSaveConfig(FileFormat.Unknown, 0, 0, false, false);

            DdsFile image = await Task.Run(() => new DdsFile(filename));
            /*
            bool skipFirstMip = false;

            int width = image.Width;
            int height = image.Height;

            if (MipMaps[0].ImageData == null || MipMaps[0].ImageData.Length == 0)
            {
                width *= 2;
                height *= 2;

                skipFirstMip = true;
            }
            
            UnrealPropertyIntValue sizeX = PropertyHeader.GetProperty("SizeX").FirstOrDefault()?.Value as UnrealPropertyIntValue;
            UnrealPropertyIntValue sizeY = PropertyHeader.GetProperty("SizeY").FirstOrDefault()?.Value as UnrealPropertyIntValue;

            sizeX?.SetPropertyValue(skipFirstMip ? width * 2 : width);
            sizeY?.SetPropertyValue(skipFirstMip ? height * 2 : height);

            UnrealPropertyIntValue mipTailBaseIdx = PropertyHeader.GetProperty("MipTailBaseIdx").FirstOrDefault()?.Value as UnrealPropertyIntValue;

            int indexSize = width > height ? width : height;

            mipTailBaseIdx?.SetPropertyValue((int)Math.Log(skipFirstMip ? indexSize * 2 : indexSize, 2));

            UnrealPropertyStringValue filePath = PropertyHeader.GetProperty("SourceFilePath").FirstOrDefault()?.Value as UnrealPropertyStringValue;
            UnrealPropertyStringValue fileTime = PropertyHeader.GetProperty("SourceFileTimestamp").FirstOrDefault()?.Value as UnrealPropertyStringValue;

            filePath?.SetPropertyValue(filename);
            fileTime?.SetPropertyValue(File.GetLastWriteTime(filename).ToString("yyyy-MM-dd hh:mm:ss"));

            UnrealPropertyByteValue pfFormat = PropertyHeader.GetProperty("Format").FirstOrDefault()?.Value as UnrealPropertyByteValue;

            FileFormat imageFormat = FileFormat.Unknown;

            if (pfFormat != null) imageFormat = DdsPixelFormat.ParseFileFormat(pfFormat.PropertyString);

            if (imageFormat == FileFormat.Unknown) throw new Exception($"Unknown DDS File Format ({pfFormat?.PropertyString ?? "Unknown"}).");

            if (config.FileFormat == FileFormat.Unknown) config.FileFormat = imageFormat;
            else
            {
                string formatStr = DdsPixelFormat.BuildFileFormat(config.FileFormat);

                UnrealNameTableEntry formatTableEntry = nameTable.SingleOrDefault(nt => nt.Name.String == formatStr) ?? nameTable.AddUnrealNameTableEntry(formatStr);

                pfFormat?.SetPropertyValue(formatTableEntry);
            }

            MipMaps.Clear();

            if (skipFirstMip)
            {
                MipMaps.Add(new UnrealMipMap
                {
                    ImageData = null,
                    Width = width,
                    Height = height
                });
            }

            image.GenerateMipMaps(4, 4);

            foreach (DdsMipMap mipMap in image.MipMaps.OrderByDescending(mip => mip.Width))
            {
                MipMaps.Add(new UnrealMipMap
                {
                    ImageData = image.WriteMipMap(mipMap, config),
                    Width = mipMap.Width,
                    Height = mipMap.Height
                });
            }*/
        }

        #endregion Unreal Methods

        #region UnrealUpkBuilderBase Implementation

        public byte[] WriteMipMapChache(int index)
        {
            if (index >= MipMaps.Count) return [];

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

        public override int GetBuilderSize()
        {
            BuilderSize = base.GetBuilderSize()
                        + sizeof(int);

            foreach (Texture2DMipMap mipMap in MipMaps)
            {
                BulkDataCompressionTypes flags = mipMap.Data == null || 
                    mipMap.Data.Length == 0 
                    ? BulkDataCompressionTypes.Unused | BulkDataCompressionTypes.StoreInSeparatefile 
                    : BulkDataCompressionTypes.LZO_ENC;

                BuilderSize += Task.Run(() => ProcessUncompressedBulkData(ByteArrayReader.CreateNew(mipMap.Data, 0), flags)).Result
                            + sizeof(int) * 2;
            }

            BuilderSize += Guid.Length;

            return BuilderSize;
        }

        public int GetCompressedMipMapSize(int index)
        {
            if (index >= MipMaps.Count) return 0;

            BuilderSize = base.GetBuilderSize() + sizeof(int); // need sizeof(int)?
            var mipMap = MipMaps[index];

            BulkDataCompressionTypes flags = mipMap.Data == null ||
                mipMap.Data.Length == 0
                ? BulkDataCompressionTypes.Unused | BulkDataCompressionTypes.StoreInSeparatefile
                : BulkDataCompressionTypes.LZO_ENC;

            BuilderSize += Task.Run(() => ProcessUncompressedBulkData(ByteArrayReader.CreateNew(mipMap.Data, 0), flags)).Result
                        + sizeof(int) * 2;

            return BuilderSize;
        }

        public override async Task WriteBuffer(ByteArrayWriter Writer, int CurrentOffset)
        {
            await base.WriteBuffer(Writer, CurrentOffset);

            Writer.WriteInt32(MipMaps.Count);

            for (int i = 0; i < MipMaps.Count; ++i)
            {
                await CompressedChunks[i].WriteCompressedChunk(Writer, CurrentOffset);

                Writer.WriteInt32(MipMaps[i].SizeX);
                Writer.WriteInt32(MipMaps[i].SizeY);
            }

            await Writer.WriteBytes(Guid);
        }

        #endregion UnrealUpkBuilderBase Implementation

        #region Private Methods

        public void ResetCompressedChunks()
        {
            CompressedChunks.Clear();
        }

        public void ExpandMipMaps(int count, List<DdsMipMap> mipMaps)
        {
            MipMapsCount = count;
            int maxIndex = MipMaps.Count;
            var format = MipMaps[0].OverrideFormat;
            for (int index = maxIndex; index < MipMapsCount; index++)
            {
                Texture2DMipMap mip = new Texture2DMipMap
                {
                    SizeX = mipMaps[index].Width,
                    SizeY = mipMaps[index].Height,
                    OverrideFormat = format
                };
                MipMaps.Add(mip);
            }
        }

        #endregion Private Methods

    }

}
