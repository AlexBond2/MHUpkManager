using MHUpkManager.Models;
using System.Reflection;
using System.Security.Cryptography;

using UpkManager.Contracts;
using UpkManager.Extensions;
using UpkManager.Models;
using UpkManager.Models.UpkFile.Classes;
using UpkManager.Models.UpkFile.Core;
using UpkManager.Models.UpkFile.Engine;
using UpkManager.Models.UpkFile.Engine.Mesh;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Objects;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Repository;

namespace MHUpkManager
{
    public partial class MainForm : Form
    {
        private readonly IUpkFileRepository repository;
        public const string AppName = "MH UPK Manager v.1.0 by AlexBond";
        public UnrealUpkFile UpkFile { get; set; }

        private HexViewForm hexViewForm;
        private TextureViewForm textureViewForm;
        private ModelViewForm modelViewForm;

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

            RegistryInstances();

            hexViewForm = new HexViewForm();
            textureViewForm = new TextureViewForm();
            modelViewForm = new ModelViewForm();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            hexViewForm?.Dispose();
            textureViewForm?.Dispose();
            modelViewForm?.Dispose();
        }

        private void RegistryInstances()
        {
            _ = CoreRegistry.Instance;
            _ = EngineRegistry.Instance;
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
            viewObjectInHEXMenuItem.Enabled = false;
            viewObjectMenuItem.Enabled = false;
            viewParentMenuItem.Enabled = false;
            viewTextureMenuItem.Enabled = false;
            viewModelMenuItem.Enabled = false;

            if (currentObject is null) return;

            viewObjectMenuItem.Enabled = true;
            viewParentMenuItem.Enabled = true;
            objectNameClassMenuItem.Text = e.Node.Text;

            if (currentObject is UnrealExportTableEntry export)
            {
                try
                {
                    if (export.UnrealObject == null)
                        await export.ParseUnrealObject(false, false);

                    viewObjectInHEXMenuItem.Enabled = true;

                    if (export.UnrealObject is IUnrealObject uObject)
                        BuildPropertyTree(uObject);
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

        private void BuildPropertyTree(IUnrealObject uObject)
        {
            propertiesView.BeginUpdate();
            propertiesView.Nodes.Clear();

            foreach (VirtualNode virtualNode in uObject.FieldNodes)
                propertiesView.Nodes.Add(CreateRealNode(virtualNode));

            var buffer = uObject.Buffer;
            if (!buffer.IsAbstractClass && (buffer.ResultProperty != ResultProperty.None || buffer.DataSize != 0))
            {
                var dataNode = new TreeNode($"Data [{buffer.ResultProperty}][{buffer.DataSize}]");

                var data = uObject.Buffer.Reader.GetBytes();
                if (uObject.Buffer.DataOffset >= 0 && uObject.Buffer.DataOffset < data.Length)
                {
                    int length = data.Length - uObject.Buffer.DataOffset;
                    byte[] offsetData = new byte[length];
                    Array.Copy(data, uObject.Buffer.DataOffset, offsetData, 0, length);
                    data = offsetData;
                }
                dataNode.Tag = data;

                propertiesView.Nodes.Add(dataNode);
            }

            if (uObject.UObject is UTexture2D) viewTextureMenuItem.Enabled = true;
            if (CheckMeshObject(uObject)) viewModelMenuItem.Enabled = true;

            ExpandFiltered(propertiesView.Nodes);
            propertiesView.EndUpdate();
        }

        private static bool CheckMeshObject(IUnrealObject uObject)
        {
            return uObject.UObject is USkeletalMesh || uObject.UObject is UStaticMesh;
        }

        private static void ExpandFiltered(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
                if ((node.Nodes.Count > 0 && node.Nodes.Count < 11) || node.Text == "Properties")
                {
                    node.Expand();
                    ExpandFiltered(node.Nodes);
                }
        }

        private static TreeNode CreateRealNode(VirtualNode virtualNode)
        {
            var node = new TreeNode(virtualNode.Text);
            node.Tag = virtualNode.Tag;
            foreach (var child in virtualNode.Children)
                node.Nodes.Add(CreateRealNode(child));

            return node;
        }

        private void viewObjectInHEXMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export && export.UnrealObject is IUnrealObject uObject)
            {
                var data = uObject.Buffer.Reader.GetBytes();
                openHexView(export.ObjectNameIndex.Name, data);
            }
        }

        private void propertiesMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewDataInHEXMenuItem.Enabled = false;

            if (propertiesView.SelectedNode?.Tag is not byte[] data || data.Length == 0) return;

            viewDataInHEXMenuItem.Enabled = true;
        }

        private void viewDataInHEXMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export)
            {
                if (propertiesView.SelectedNode.Tag is byte[] data)
                    openHexView(export.ObjectNameIndex.Name, data);
            }
        }

        private void objectNameClassMenuItem_Click(object sender, EventArgs e)
        {
            if (objectNameClassMenuItem.Text != null)
                Clipboard.SetText(objectNameClassMenuItem.Text);
        }

        private void openHexView(string name, byte[] data)
        {
            if (data == null || data.Length == 0) return;

            hexViewForm.SetTitle(name);
            hexViewForm.SetHexData(data);
            hexViewForm.ShowDialog();
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

        private void viewTextureMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject == null) return;
            if (currentObject is UnrealExportTableEntry export)
                openTextureView(export.ObjectNameIndex.Name, export.UnrealObject);
        }

        private void openTextureView(string name, UnrealObjectBase unrealObject)
        {
            if (unrealObject is IUnrealObject uObject && uObject.UObject is UTexture2D data)
            {
                textureViewForm.SetTitle(name);
                textureViewForm.SetTextureObject(data);
                textureViewForm.ShowDialog();
            }
        }

        private void viewModelMenuItem_Click(object sender, EventArgs e)
        {
            if (currentObject is UnrealExportTableEntry export)
                openModelView(export.ObjectNameIndex.Name, export.UnrealObject);
        }

        private void openModelView(string name, UnrealObjectBase unrealObject)
        {
            if (unrealObject is IUnrealObject uObject && CheckMeshObject(uObject))
            {
                modelViewForm.SetTitle(name);
                modelViewForm.SetMeshObject(uObject.UObject as UObject);
                modelViewForm.ShowDialog();
            }
        }
    }
}
