using UpkManager.Models.UpkFile;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Objects;
using MHUpkManager.Models;

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

        public enum UClassIndex
        {
            package = 3,
            material = 4,
            materialinstanceconstant = 5,
            texture2d = 6,
            particlespriteemitter = 7,
            particlesystem = 8,
            skeletalmesh = 9,
            staticmesh = 10,
            entityfxsound = 11,
            animset = 15,
            animsequence = 16,
            entityfxparticle = 19,
            materialfunction = 20,
            objectreferencer = 21,
            powerfxanimation = 22,
            physicsasset = 27,
            physicalmaterial = 28,
            akbank = 29,
            akevent = 30,
        }

        public static void BuildObjectTree(List<TreeNode> rootNodes, UnrealHeader header)
        {
            Dictionary<int, TreeNode> nodes = [];

            foreach (var entry in header.ImportTable)
            {
                var className = entry.ClassNameIndex.Name;
                int imageIndex = GetImageIndex(className);
                className = $"::{className}";
                var name = $"{entry.ObjectNameIndex.Name} [{entry.TableIndex}] {className}";
                var node = new TreeNode(name);
                node.Tag = entry;
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
                nodes[entry.TableIndex] = node;
            }

            foreach (var entry in header.ExportTable)
            {
                var className = entry.ClassReferenceNameIndex?.Name;
                int imageIndex = GetImageIndex(className);
                className = $"{entry.SuperReferenceNameIndex?.Name}::{className}";
                var name = $"{entry.ObjectNameIndex.Name} [{entry.TableIndex}] {className}";
                var node = new TreeNode(name);
                node.Tag = entry;
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
                nodes[entry.TableIndex] = node;
            }

            rootNodes.Clear();

            var importsRoot = new TreeNode("Imports");
            importsRoot.ImageIndex = 1;
            importsRoot.SelectedImageIndex = 1;
            BuildBranch(header.ImportTable, importsRoot, nodes);

            var exportsRoot = new TreeNode("Exports");
            exportsRoot.ImageIndex = 2;
            exportsRoot.SelectedImageIndex = 2;
            BuildBranch(header.ExportTable, exportsRoot, nodes);

            rootNodes.Add(exportsRoot);
            rootNodes.Add(importsRoot);
        }

        private static int GetImageIndex(string className)
        {
            if (string.IsNullOrEmpty(className)) return 0;
            if (Enum.TryParse(typeof(UClassIndex), className, out var index))
                return (int)index;
            return 0;
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
}
