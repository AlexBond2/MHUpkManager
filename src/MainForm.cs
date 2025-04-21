using System.Security.Cryptography;
using UpkManager.Models;
using UpkManager.Extensions;
using UpkManager.Contracts;
using UpkManager.Repository;

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
            await Task.Run(() => upkFile.Header.ReadHeaderAsync(OnLoadProgress));
        }

        private void OnLoadProgress(UnrealLoadProgress progress)
        {
            totalStatus.Text = $"{progress.Current:N0}";

            if (progress.IsComplete)
            {
                totalStatus.Text = $"{progress.Total:N0}";
                progressStatus.Text = "";
            }

            if (!string.IsNullOrEmpty(progress.Text))
                progressStatus.Text = progress.Text;
        }

        private async Task<UnrealUpkFile> OpenUpkFile(string filePath)
        {
            var file = new FileInfo(filePath);
            var fileHash = await Task.Run(() => file.OpenRead().GetHash<MD5>((int)file.Length));

            return new UnrealUpkFile { 
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
    }
}
