using System.ComponentModel;
using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Objects;
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
                Object = entry.ObjectNameIndex.Name,
                Class = $"::{entry.ClassNameIndex.Name}",
                Package = entry.PackageNameIndex?.Name,
                Outer = entry.OuterReferenceNameIndex?.Name
            }).ToList();

            return data;
        }

        public static string PrintFlags(UnrealExportTableEntry entry)
        {
            string flags = $"{(EObjectFlags)entry.ObjectFlags}";
            if (entry.ExportFlags != 0) flags += $" | {(ExportFlags)entry.ExportFlags}";
            return flags;
        }

        public static object GetDataSource(List<UnrealExportTableEntry> exportTable)
        {
            var data = exportTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Object = entry.ObjectNameIndex.Name,
                Class = $"{entry.SuperReferenceNameIndex?.Name}::{entry.ClassReferenceNameIndex?.Name}",
                Outer = entry.OuterReferenceNameIndex?.Name,
                Flags = PrintFlags(entry),
                SerialSize = entry.SerialDataSize,
                Details = entry
            }).ToList();

            return data;
        }

        public static object GetDataSource(List<UnrealNameTableEntry> nameTable)
        {
            var data = nameTable.Select(entry => new
            {
                Index = entry.TableIndex,
                Name = entry.Name.String,
                Flags = (EObjectFlags)entry.Flags
            }).ToList();

            return data;
        }

        public static void ShowPropertyGrid(object entry, Form parent)
        {
            var gridControl = new PropertyGrid();

            if (entry is UnrealExportTableEntry exportEntry)
            {
                gridControl.SelectedObject = new UnrealExportViewModel(exportEntry);
            }

            gridControl.DisabledItemForeColor = SystemColors.ControlText;
            gridControl.ExpandAllGridItems();
            gridControl.HelpVisible = false;

            var popupForm = new Form
            {
                Text = "Properties View",
                Size = new Size(440, 420),
                StartPosition = FormStartPosition.CenterParent
            };
            popupForm.Controls.Add(gridControl);
            gridControl.Dock = DockStyle.Fill;

            popupForm.ShowDialog(parent);
        }

        public static List<TreeNode> BuildObjectTree(UnrealHeader header)
        {
            Dictionary<int, TreeNode> nodes = [];

            foreach (var entry in header.ImportTable)
            {
                var className = $"::{entry.ClassNameIndex.Name}";
                var name = $"{entry.ObjectNameIndex.Name} [{entry.TableIndex}] {className}";
                var node = new TreeNode(name);
                node.Tag = entry;
                nodes[entry.TableIndex] = node;
            }

            foreach (var entry in header.ExportTable)
            {
                var className = $"{entry.SuperReferenceNameIndex?.Name}::{entry.ClassReferenceNameIndex?.Name}";
                var name = $"{entry.ObjectNameIndex.Name} [{entry.TableIndex}] {className}";
                var node = new TreeNode(name);
                node.Tag = entry;
                nodes[entry.TableIndex] = node;
            }

            List<TreeNode> rootNodes = [];

            var importsRoot = new TreeNode("Imports");
            BuildBranch(header.ImportTable, importsRoot, nodes);

            var exportsRoot = new TreeNode("Exports");
            BuildBranch(header.ExportTable, exportsRoot, nodes);

            rootNodes.Add(exportsRoot);
            rootNodes.Add(importsRoot);
            return rootNodes;
        }

        private static void BuildBranch<T>(IEnumerable<T> table, TreeNode root, Dictionary<int, TreeNode> nodes)
            where T : UnrealObjectTableEntryBase
        {
            foreach (var entry in table)
            {
                var node = nodes[entry.TableIndex];

                if (entry.OuterReference == 0)
                    root.Nodes.Add(node);
                else if (nodes.TryGetValue(entry.OuterReference, out var parent))
                    parent.Nodes.Add(node);
                else
                    root.Nodes.Add(node);
            }
        }
    }

    public class UnrealExportViewModel
    {
        [Category("General")]
        public int Index { get; }

        [Category("General")]
        public string Object { get; }

        [Category("General")]
        public string Class { get; }

        [Category("General")]
        public string Super { get; }

        [Category("General")]
        public string Outer { get; }

        [Category("General")]
        public string Archetype { get; }

        [Category("Flags")]
        public EObjectFlags ObjectFlags { get; }

        [Category("Flags")]
        public ExportFlags ExportFlags { get; }

        [Category("Pakage")]
        public int SerialSize { get; }

        [Category("Pakage")]
        public int SerialOffset { get; }

        [Category("Pakage")]
        public Guid PackageGuid { get; }

        [Category("Pakage")]
        public EPackageFlags PackageFlags { get; }

        [Category("Pakage")]
        public Int32[] NetObjects { get; }

        public UnrealExportViewModel(UnrealExportTableEntry entry)
        {
            Index = entry.TableIndex;
            Object = entry.ObjectNameIndex?.Name;
            Class = entry.ClassReferenceNameIndex?.Name;
            Super = entry.SuperReferenceNameIndex?.Name;
            Outer = entry.OuterReferenceNameIndex?.Name;
            Archetype = entry.ArchetypeReferenceNameIndex?.Name;
            ObjectFlags = (EObjectFlags)entry.ObjectFlags;
            ExportFlags = (ExportFlags)entry.ExportFlags;
            SerialSize = entry.SerialDataSize;
            SerialOffset = entry.SerialDataOffset;
            PackageGuid = new (entry.PackageGuid);
            PackageFlags = (EPackageFlags)entry.PackageFlags;
            NetObjects = entry.NetObjects.Select(i => new Int32(i)).ToArray();
        }
    }

    public class UnrealHeaderViewModel
    {
        [Category("General")]
        [DisplayName("GUID")]
        public Guid Guid { get; }

        [Category("General")]
        [DisplayName("Package Source")]
        [Description("Package CRC-32 checksum")]
        public string PackageSource { get; }

        [Category("General")]
        [DisplayName("Package Flags")]
        public EPackageFlags PackageFlags { get; }

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
        [Description("Depends Table")]
        [Editor(typeof(CollectionView), typeof(UITypeEditor))]
        public List<int> Depends { get; }

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
            PackageFlags = (EPackageFlags)header.Flags;
            PackageSource = $"{header.PackageSource:X8}";
            NameTableCount = header.NameTableCount;
            ExportTableCount = header.ExportTableCount;
            ImportTableCount = header.ImportTableCount;
            Filename = header.Filename;
            CompressionFlags = (CompressionTypes)header.CompressionFlags;
            CookerVersion = header.CookerVersion;
            EngineVersion = header.EngineVersion;
            Depends = header.DependsTable;
            ThumbnailTableOffset = header.ThumbnailTableOffset;
            Generations = header.GenerationTable.Select(e => new GenerationTable(e)).ToArray();
            AdditionalPackagesToCookCount = header.AdditionalPackagesToCook.Count;
            TextureAllocations = header.TextureAllocations.TextureTypes.Select(e => new TextureType(e)).ToArray();
            Guid = new (header.Guid);
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
        public EPixelFormat Format => (EPixelFormat)_entry.TextureFormat;
        public ETextureCreateFlags CreateFlags => (ETextureCreateFlags)_entry.TextureCreateFlags;

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
        public Int32[] Indices => _indices;
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
