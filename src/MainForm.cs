using System.Security.Cryptography;
using System.Reflection;
using MHUpkManager.Models;

using UpkManager.Models;
using UpkManager.Extensions;
using UpkManager.Contracts;
using UpkManager.Repository;
using UpkManager.Models.UpkFile.Objects;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Properties;

namespace MHUpkManager
{
    public partial class MainForm : Form
    {
        private readonly IUpkFileRepository repository;
        public const string AppName = "MH UPK Manager v.1.0 by AlexBond";
        public const string EngineJson = "MHEngineTypes.json";
        public const string CoreJson = "MHCoreTypes.json";
        public const string ComponentsTxt = "MHComponents.txt";
        public UnrealUpkFile UpkFile { get; set; }
        private List<TreeNode> rootNodes;
        private object currentObject;

        public MainForm()
        {
            InitializeComponent();
            repository = new UpkFileRepository();
            rootNodes = [];

            EnableDoubleBuffering(nameGridView);
            EnableDoubleBuffering(importGridView);
            EnableDoubleBuffering(exportGridView);

            LoadDataFiles();
        }

        private void LoadDataFiles()
        {
            string warning;
            string path = Path.Combine("Data", EngineJson);

            if (!File.Exists(path))
            {
                WarningBox($"File with Engine types not found. Path: {path}");
                return;
            }
            else
            {
                warning = EngineRegistry.LoadFromJson(path);
                if (!string.IsNullOrEmpty(warning))
                    WarningBox($"Warning while loading Engine types from {EngineJson}:\n\n{warning}");
            }

            path = Path.Combine("Data", CoreJson);
            if (!File.Exists(path))
            {
                WarningBox($"File with Engine types not found. {path}");
                return;
            }
            else
            {
                warning = CoreRegistry.LoadFromJson(path);

                if (!string.IsNullOrEmpty(warning))
                    WarningBox($"Warning while loading Core types from {CoreJson}:\n\n{warning}");
            }

            path = Path.Combine("Data", ComponentsTxt);
            if (!File.Exists(path))
                WarningBox($"File with component list not found. {path}");
            else
                ComponentRegistry.LoadFromFile(path);
        }

        private void WarningBox(string msg)
        {
            MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void EnableDoubleBuffering(DataGridView dgv)
        {
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        private async void openMenuItem_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Unreal Package Files (*.upk)|*.upk";
            openFileDialog.Title = "Open Upk file";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                UpkFile = await OpenUpkFile(openFileDialog.FileName);
                Text = $"{AppName} - [{UpkFile.GameFilename}]";
                await LoadUpkFile(UpkFile);
            }
        }

        private async Task LoadUpkFile(UnrealUpkFile upkFile)
        {
            upkFile.Header = await repository.LoadUpkFile(Path.Combine(upkFile.ContentsRoot, upkFile.GameFilename));
            var header = upkFile.Header;
            await Task.Run(() => header.ReadHeaderAsync(OnLoadProgress));

            nameGridView.DataSource = ViewEntities.GetDataSource(header.NameTable);
            importGridView.DataSource = ViewEntities.GetDataSource(header.ImportTable);
            exportGridView.DataSource = ViewEntities.GetDataSource(header.ExportTable);
            propertyGrid.SelectedObject = new UnrealHeaderViewModel(header);

            ViewEntities.BuildObjectTree(rootNodes, header);
            UpdateObjectsTree();
        }

        private void UpdateObjectsTree()
        {
            filterBox.Text = "";
            if (rootNodes.Count == 0) return;

            objectsTree.Nodes.Clear();
            objectsTree.BeginUpdate();
            objectsTree.Nodes.AddRange([.. rootNodes]);
            foreach (TreeNode node in objectsTree.Nodes) node.Expand();
            objectsTree.EndUpdate();

            totalStatus.Text = objectsTree.Nodes.Count.ToString();
        }

        private void OnLoadProgress(UnrealLoadProgress progress)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => OnLoadProgress(progress)));
                return;
            }

            totalStatus.Text = $"{progress.Current:N0}";

            if (progress.IsComplete)
            {
                totalStatus.Text = $"{progress.Total:N0}";
                progressStatus.Text = "";
                return;
            }

            if (!string.IsNullOrEmpty(progress.Text))
                progressStatus.Text = progress.Text;
        }

        private async Task<UnrealUpkFile> OpenUpkFile(string filePath)
        {
            var file = new FileInfo(filePath);
            var fileHash = await Task.Run(() => file.OpenRead().GetHash<MD5>((int)file.Length));

            return new UnrealUpkFile
            {
                GameFilename = Path.GetFileName(file.FullName),
                Package = Path.GetFileNameWithoutExtension(file.Name).ToLowerInvariant(),
                ContentsRoot = Path.GetDirectoryName(file.FullName),
                Filesize = file.Length,
                Filehash = fileHash
            };
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Unreal Package Files (*.upk)|*.upk";
            saveFileDialog.Title = "Save UPK file";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;
                SaveUpkFile(filename);
            }
        }

        private void SaveUpkFile(string filename)
        {
            MessageBox.Show("Save function not ready", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void exportGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && exportGridView.Columns[e.ColumnIndex].Name == "buttonColumn")
            {
                var boundItem = exportGridView.Rows[e.RowIndex].DataBoundItem;
                var detailsProp = boundItem.GetType().GetProperty("Details");
                var entry = detailsProp?.GetValue(boundItem);

                if (entry != null)
                {
                    var grid = sender as DataGridView;
                    var parentForm = grid?.FindForm();
                    ViewEntities.ShowPropertyGrid(entry, parentForm);
                }
            }
        }

        private void filterClear_Click(object sender, EventArgs e)
        {
            UpdateObjectsTree();
        }

        private void filterBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (FilterTree(filterBox.Text))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                totalStatus.Text = objectsTree.Nodes.Count.ToString();
            }
        }

        private bool FilterTree(string filterText)
        {
            filterText = filterText.Trim().ToLower();
            if (filterText.Length < 3 || rootNodes.Count == 0) return false;

            objectsTree.BeginUpdate();
            objectsTree.Nodes.Clear();

            foreach (TreeNode rootNode in rootNodes)
            {
                var filteredNode = FilterNode(rootNode, filterText);
                if (filteredNode != null)
                    objectsTree.Nodes.Add(filteredNode);
            }

            objectsTree.ExpandAll();
            objectsTree.EndUpdate();

            return objectsTree.Nodes.Count > 0;
        }

        private static TreeNode FilterNode(TreeNode node, string filterText)
        {
            List<TreeNode> matchingChildren = [];

            foreach (TreeNode child in node.Nodes)
            {
                var filteredChild = FilterNode(child, filterText);
                if (filteredChild != null)
                    matchingChildren.Add(filteredChild);
            }

            if (node.Text.ToLower().Contains(filterText) || matchingChildren.Count > 0)
            {
                var newNode = new TreeNode(node.Text)
                {
                    Tag = node.Tag,
                    ImageIndex = node.ImageIndex,
                    SelectedImageIndex = node.SelectedImageIndex
                };

                newNode.Nodes.AddRange(matchingChildren.ToArray());
                return newNode;
            }

            return null;
        }

        private async void objectsTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentObject = e.Node?.Tag;
            viewDataInHEXMenuItem.Enabled = false;
            viewObjectInHEXMenuItem.Enabled = false;
            viewObjectMenuItem.Enabled = false;
            viewParentMenuItem.Enabled = false;

            if (currentObject is null) return;

            viewObjectMenuItem.Enabled = true;
            viewParentMenuItem.Enabled = true;
            objectNameClassMenuItem.Text = e.Node.Text;

            if (currentObject is UnrealExportTableEntry export)
            {
                try
                {
                    if (export.UnrealObject == null)
                        await export.ParseUnrealObject(UpkFile.Header, false, false);
                    viewObjectInHEXMenuItem.Enabled = true;
                    BuildPropertyTree(export.UnrealObject);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error parse object:\n{export.ObjectNameIndex} :: {export.ClassReferenceNameIndex}\n\n{ex.Message}",
                        "Parse error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else if (currentObject is UnrealImportTableEntry importEntry)
            {
                propertiesView.Nodes.Clear();
            }
        }

        private void BuildPropertyTree(UnrealObjectBase unrealObject)
        {
            propertiesView.BeginUpdate();
            propertiesView.Nodes.Clear();

            if (unrealObject is IUnrealObject uObject)
            {
                foreach (VirtualNode virtualNode in uObject.FieldNodes)
                    propertiesView.Nodes.Add(CreateRealNode(virtualNode));
            }
            else
            {
                var propertyHeader = unrealObject.PropertyHeader;

                foreach (var property in propertyHeader.Properties)
                    propertiesView.Nodes.Add(CreateRealNode(property.VirtualTree));

                if (propertyHeader.Properties.Count == 0 && propertyHeader.Result == ResultProperty.None)
                    propertiesView.Nodes.Add(new TreeNode($"none"));

                if (propertyHeader.Result != ResultProperty.None || propertyHeader.RemainingData != 0)
                {
                    propertiesView.Nodes.Add(new TreeNode($"Data [{propertyHeader.Result}][{propertyHeader.RemainingData}]"));
                    viewDataInHEXMenuItem.Enabled = true;
                }
            }

            ExpandFiltered(propertiesView.Nodes);
            propertiesView.EndUpdate();
        }

        private static void ExpandFiltered(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
                if (node.Nodes.Count > 0 && node.Nodes.Count < 11)
                {
                    node.Expand();
                    ExpandFiltered(node.Nodes);
                }
        }

        private static TreeNode CreateRealNode(VirtualNode virtualNode)
        {
            var node = new TreeNode(virtualNode.Text);
            foreach (var child in virtualNode.Children)
                node.Nodes.Add(CreateRealNode(child));

            return node;
        }

        private void viewObjectInHEXMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export)
            {
                var obj = export.UnrealObject;
            }
        }

        private void viewDataInHEXMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export)
            {
                var obj = export.UnrealObject;
            }
        }

        private void objectNameClassMenuItem_Click(object sender, EventArgs e)
        {
            if (objectNameClassMenuItem.Text != null)
                Clipboard.SetText(objectNameClassMenuItem.Text);
        }

        private void viewParentMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export)
            {
                int index = export.SuperReference;
                if (index == 0)
                    index = export.ClassReference;

                if (index > 0)
                    selectExportIndex(index);
                else
                    selectImportIndex(index);
            }
            else if (currentObject is UnrealImportTableEntry import)
            {
                int nameIndex = import.ClassNameIndex.Index;
                int index = UpkFile.Header.GetClassNameTableIndex(nameIndex);
                if (index > 0)
                    selectExportIndex(index);
                else
                    selectImportIndex(index);
            }
        }

        private void viewObjectMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export) 
                selectExportIndex(export.TableIndex);
            else if (currentObject is UnrealImportTableEntry import)
                selectImportIndex(import.TableIndex);
        }

        private void selectExportIndex(int tableIndex)
        {
            tabControl1.SelectTab(3);
            int index = tableIndex - 1;
            if (index >= 0 && index < exportGridView.Rows.Count)
            {
                exportGridView.ClearSelection();
                exportGridView.Rows[index].Selected = true;
                exportGridView.CurrentCell = exportGridView.Rows[index].Cells[0];
                exportGridView.FirstDisplayedScrollingRowIndex = index;
            }
        }

        private void selectImportIndex(int tableIndex)
        {
            tabControl1.SelectTab(2);
            int index = -tableIndex - 1;
            if (index >= 0 && index < importGridView.Rows.Count)
            {
                importGridView.ClearSelection();
                importGridView.Rows[index].Selected = true;
                importGridView.CurrentCell = importGridView.Rows[index].Cells[0];
                importGridView.FirstDisplayedScrollingRowIndex = index;
            }
        }
    }
}
