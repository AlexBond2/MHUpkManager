using System.ComponentModel;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Constants;

namespace MHUpkManager
{
    public abstract class ViewEntities
    {
        public static object GetDataSource(List<UnrealImportTableEntry> importTable)
        {
            var data = importTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Object = entry.NameTableIndex?.Name,
                Class = entry.TypeNameIndex?.Name,
                Package = entry.PackageNameIndex?.Name,
                Group = entry.OwnerReferenceNameIndex?.Name
            }).ToList();

            return data;
        }

        public static object GetDataSource(List<UnrealExportTableEntry> exportTable)
        {
            var data = exportTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Object = entry.NameTableIndex?.Name,
                Class = entry.TypeReferenceNameIndex?.Name,
                Pakage = entry.OwnerReferenceNameIndex?.Name,
                Group = entry.ParentReferenceNameIndex?.Name,
                Flags = $"0x{entry.FlagsLow:X8}-0x{entry.FlagsHigh:X8}",
                Size = entry.SerialDataSize,
                Offset = entry.SerialDataOffset
            }).ToList();

            return data;
        }

        public static object GetDataSource(List<UnrealNameTableEntry> nameTable)
        {
            var data = nameTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Name = entry.Name.String,
                Flags = $"0x{entry.Flags:X16}"
            }).ToList();

            return data;
        }
    }

    public class UnrealHeaderViewModel
    {
        [Category("General")]
        [DisplayName("GUID")]
        public string Guid { get; }

        [Category("General")]
        [DisplayName("Package Source")]
        [Description("Package CRC-32 checksum")]
        public string PackageSource { get; }

        [Category("General")]
        [DisplayName("Game Version")]
        public ushort Version { get; }

        [Category("General")]
        [DisplayName("Licensee Version")]
        public ushort Licensee { get; }

        [Category("Tables")]
        [DisplayName("Names")]
        public int NameTableCount { get; }

        [Category("Tables")]
        [DisplayName("Exports")]
        public int ExportTableCount { get; }

        [Category("Tables")]
        [DisplayName("Imports")]
        public int ImportTableCount { get; }

        [Category("General")]
        [DisplayName("Filename")]
        public string Filename { get; }

        [Category("General")]
        [DisplayName("Compression")]
        public CompressionTypes CompressionFlags { get; }

        [Category("General")]
        [DisplayName("Engine Version")]
        public uint EngineVersion { get; }

        [Category("General")]
        [DisplayName("Cooker Version")]
        public uint CookerVersion { get; }

        [Category("Unreal Extra")]
        [DisplayName("Thumbnails")]
        [Description("Thumbnail Table Offset")]
        public int ThumbnailTableOffset { get; }

        [Category("Unreal Extra")]
        [DisplayName("Depends")]
        [Description("Depends Table Offset")]
        public int DependsTableOffset { get; }

        [Category("Unreal Extra")]
        [DisplayName("Generations")]
        public int GenerationTableCount { get; }

        [Category("Unreal Extra")]
        [DisplayName("Packages to cook")]
        [Description("Additional Packages To Cook Count")]
        public int AdditionalPackagesToCookCount { get; }

        [Category("Unreal Extra")]
        [DisplayName("Texture Allocations")]
        [Description("Texture Allocations Count")]
        public int TextureAllocationsCount { get; }

        public UnrealHeaderViewModel(UnrealHeader header)
        {
            Version = header.Version;
            Licensee = header.Licensee;
            PackageSource = $"{header.PackageSource:X8}";
            NameTableCount = header.NameTableCount;
            ExportTableCount = header.ExportTableCount;
            ImportTableCount = header.ImportTableCount;
            Filename = header.Filename;
            CompressionFlags = (CompressionTypes)header.CompressionFlags;
            CookerVersion = header.CookerVersion;
            EngineVersion = header.EngineVersion;
            DependsTableOffset = header.DependsTableOffset;
            ThumbnailTableOffset = header.ThumbnailTableOffset;
            GenerationTableCount = header.GenerationTableCount;
            AdditionalPackagesToCookCount = header.AdditionalPackagesToCook.Count;
            TextureAllocationsCount = header.TextureAllocations.Count;

            var guid = new Guid(header.Guid);
            Guid = guid.ToString();
        }
    }

}
