using System.Security.Cryptography;
using UpkManager.Models;
using UpkManager.Extensions;
using UpkManager.Contracts;
using UpkManager.Repository;
using System.Reflection;
using UpkManager.Models.UpkFile.Tables;
using UpkManager.Models.UpkFile.Properties;
using MHUpkManager.Models;

namespace MHUpkManager
{
    public partial class MainForm : Form
    {
        private readonly IUpkFileRepository repository;
        public const string AppName = "MH UPK Manager v.1.0 by AlexBond";
        public UnrealUpkFile UpkFile { get; set; }
        private List<TreeNode> rootNodes;

        public MainForm()
        {
            InitializeComponent();
            repository = new UpkFileRepository();
            rootNodes = [];

            EnableDoubleBuffering(nameGridView);
            EnableDoubleBuffering(importGridView);
            EnableDoubleBuffering(exportGridView);
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
            if (e.Node?.Tag is null) return;

            if (e.Node.Tag is UnrealExportTableEntry export)
            {
                if (export.UnrealObject == null)
                    await export.ParseUnrealObject(UpkFile.Header, false, false);

                BuildPropertyTree(export.UnrealObject.PropertyHeader);
            }
            else if (e.Node.Tag is UnrealImportTableEntry importEntry)
            {
                propertiesView.Nodes.Clear();
            }
        }

        private void BuildPropertyTree(UnrealPropertyHeader propertyHeader)
        {
            propertiesView.BeginUpdate();
            propertiesView.Nodes.Clear();

            foreach (var property in propertyHeader.Properties)
                propertiesView.Nodes.Add(CreateRealNode(property.VirtualTree));

            propertiesView.ExpandAll();
            propertiesView.EndUpdate();
        }

        private static TreeNode CreateRealNode(VirtualNode virtualNode)
        {
            var node = new TreeNode(virtualNode.Text);
            foreach (var child in virtualNode.Children)
                node.Nodes.Add(CreateRealNode(child));

            return node;
        }
    }
}
