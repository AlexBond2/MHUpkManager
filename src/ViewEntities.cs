using System.ComponentModel;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Objects.Textures;
using UpkManager.Constants;
using System.Drawing.Design;

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

        public static void CellValueNeeded(List<UnrealImportTableEntry> importTable, DataGridViewCellValueEventArgs e)
        {
            if (importTable == null || e.RowIndex >= importTable.Count)
                return;

            var entry = importTable[e.RowIndex];

            switch (e.ColumnIndex)
            {
                case 0: e.Value = entry.TableIndex; break;
                case 1: e.Value = entry.NameTableIndex?.Name; break;
                case 2: e.Value = entry.TypeNameIndex?.Name; break;
                case 3: e.Value = entry.OwnerReferenceNameIndex?.Name; break;
                case 4: e.Value = entry.PackageNameIndex?.Name; break;
                case 5: e.Value = entry.OwnerReferenceNameIndex?.Name; break;
            }
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

        public static void CellValueNeeded(List<UnrealExportTableEntry> exportTable, DataGridViewCellValueEventArgs e)
        {
            if (exportTable == null || e.RowIndex >= exportTable.Count)
                return;

            var entry = exportTable[e.RowIndex];

            switch (e.ColumnIndex)
            {
                case 0: e.Value = entry.TableIndex; break;
                case 1: e.Value = entry.NameTableIndex?.Name; break;
                case 2: e.Value = entry.TypeReferenceNameIndex?.Name; break;
                case 3: e.Value = entry.OwnerReferenceNameIndex?.Name; break;
                case 4: e.Value = entry.ParentReferenceNameIndex?.Name; break;
                case 5: e.Value = $"0x{entry.FlagsLow:X8}-0x{entry.FlagsHigh:X8}"; break;
                case 6: e.Value = entry.SerialDataSize; break;
                case 7: e.Value = entry.SerialDataOffset; break;
            }
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
        public GenerationTable[] Generations { get; }

        [Category("Unreal Extra")]
        [DisplayName("Packages to cook")]
        [Description("Additional Packages To Cook Count")]
        public int AdditionalPackagesToCookCount { get; }

        [Category("Unreal Extra")]
        [DisplayName("Texture Allocations")]
        public TextureType[] TextureAllocations { get; }

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
            Generations = header.GenerationTable.Select(e => new GenerationTable(e)).ToArray();
            AdditionalPackagesToCookCount = header.AdditionalPackagesToCook.Count;
            TextureAllocations = header.TextureAllocations.TextureTypes.Select(e => new TextureType(e)).ToArray();

            var guid = new Guid(header.Guid);
            Guid = guid.ToString();
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GenerationTable(UnrealGenerationTableEntry entry)
    {
        private readonly UnrealGenerationTableEntry _entry = entry;

        [DisplayName("Exports")]
        public int ExportCount => _entry.ExportTableCount;
        [DisplayName("Names")]
        public int NameCount => _entry.NameTableCount;
        public int NetObjects => _entry.NetObjectCount;

        public override string ToString() => $"GenerationTable";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TextureType(UnrealTextureType entry)
    {
        private readonly UnrealTextureType _entry = entry;

        public string Size => $"{_entry.Width}x{_entry.Height}";

        public int MipMaps => _entry.MipMapsCount;
        public uint Format => _entry.TextureFormat;
        public uint CreateFlags => _entry.TextureCreateFlags;

        [Editor(typeof(CollectionView), typeof(UITypeEditor))]
        public List<int> Indices => _entry.TextureIndices;


        public override string ToString() => $"TextureType";
    }

    public readonly struct Int32(int value)
    {
        public int Index { get; } = value;
        public override string ToString() => Index.ToString();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Int32Collection(List<int> list)
    {
        private readonly Int32[] _indices = list.Select(i => new Int32(i)).ToArray();
        [Category("Collection")]
        public Int32[] Indicies => _indices;
    }

    public class CollectionView : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var gridControl = new PropertyGrid();

            if (value is List<int> list)
            {
               gridControl.SelectedObject = new Int32Collection(list);
            }

            gridControl.DisabledItemForeColor = SystemColors.ControlText;
            gridControl.ExpandAllGridItems();
            gridControl.HelpVisible = false;

            var parent = (IWin32Window)provider.GetService(typeof(IWin32Window));
            var popupForm = new Form
            {
                Text = "Collection View",
                Size = new Size(300, 450),
                StartPosition = FormStartPosition.CenterParent
            };
            popupForm.Controls.Add(gridControl);
            gridControl.Dock = DockStyle.Fill;
            popupForm.ShowDialog(parent);
            return value; 
        }
    }
}
