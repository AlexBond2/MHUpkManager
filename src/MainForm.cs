using System.Security.Cryptography;
using UpkManager.Models;
using UpkManager.Extensions;
using UpkManager.Contracts;
using UpkManager.Repository;
using System.Reflection;

namespace MHUpkManager
{
    public partial class MainForm : Form
    {
        private readonly IUpkFileRepository repository;
        public UnrealUpkFile UpkFile { get; set; }

        public MainForm()
        {
            InitializeComponent();
            repository = new UpkFileRepository();

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
                await LoadUpkFile(UpkFile);
            }
        }

        private async Task LoadUpkFile(UnrealUpkFile upkFile)
        {
            upkFile.Header = await repository.LoadUpkFile(Path.Combine(upkFile.ContentsRoot, upkFile.GameFilename));
            var header = upkFile.Header;
            await Task.Run(() => header.ReadHeaderAsync(OnLoadProgress));
            nameGridView.DataSource = ViewEntities.GetDataSource(header.NameTable);

            importGridView.VirtualMode = false;
            importGridView.DataSource = ViewEntities.GetDataSource(header.ImportTable);
            //importGridView.RowCount = header.ImportTableCount;

            exportGridView.VirtualMode = false;
            exportGridView.DataSource = ViewEntities.GetDataSource(header.ExportTable);
            //exportGridView.RowCount = header.ExportTableCount;
            propertyGrid.SelectedObject = new UnrealHeaderViewModel(header);
        }

        private void importGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            ViewEntities.CellValueNeeded(UpkFile.Header.ImportTable, e);
        }

        private void exportGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            ViewEntities.CellValueNeeded(UpkFile.Header.ExportTable, e);
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
    }
}
