
using DDSLib;
using MHUpkManager.TextureManager;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using UpkManager.Models.UpkFile.Engine.Texture;
using UpkManager.Models.UpkFile.Tables;

namespace MHUpkManager
{
    public partial class TextureViewForm : Form
    {
        private DdsFile ddsFile;

        private TextureEntry textureEntry;
        private UTexture2D textureObject;

        private string title;
        private int minIndex;

        public TextureViewForm()
        {
            ddsFile = new();
            InitializeComponent();
        }

        public class MipMapInfo
        {
            public int Size;
            public int Index;

            public static MipMapInfo AddMipMap(FTexture2DMipMap mipMap, int index)
            {
                var info = new MipMapInfo
                {
                    Size = mipMap.Data.Length,
                    Index = index
                };
                return info;
            }

            public override string ToString()
            {
                return Index.ToString();
            }
        }

        public void SetTitle(string name)
        {
            title = name;
            Text = $"Texture Viewer - [{title}]";
        }

        public void SetTextureObject(FObject textObject, UTexture2D data)
        {
            textureObject = data;

            textureEntry = TextureManifest.Instance.GetTextureEntryFromObject(textObject);
            if (textureEntry != null)
            {
                var textureCache = TextureFileCache.Instance;
                textureCache.SetEntry(textureEntry, textureObject);
                textureCache.LoadTextureCache();
            }

            ReloadTextureView();
        }

        private void mipMapBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mipMapBox.SelectedItem is MipMapInfo mipMap)
            {
                sizeLabel.Text = mipMap.Size.ToString();
                sourceLabel.Text = mipMap.Index < textureObject.FirstResourceMemMip ? "TFC" : "UPK";
                LoadTexture(mipMap.Index);
            }
        }

        public void ReloadTextureView()
        {
            textureNameLabel.Text = title;
            textureGuidLabel.Text = textureObject.TextureFileCacheGuid.ToString();
            mipMapsLabel.Text = textureObject.Mips.Count.ToString();
            textureFileLabel.Text = textureObject.TextureFileCacheName?.Name;

            UpdateMipMapBox();
            LoadTexture(minIndex);
        }

        private void UpdateMipMapBox()
        {
            mipMapBox.Items.Clear();
            int index = 0;
            minIndex = -1;

            if (textureEntry != null)
            {
                index = (int)textureEntry.Data.Maps[0].Index;
                var mips = TextureFileCache.Instance.Texture2D.Mips;
                foreach (var mipMap in mips)
                {
                    if (mipMap.Data != null)
                    {
                        if (minIndex == -1) minIndex = index;
                        mipMapBox.Items.Add(MipMapInfo.AddMipMap(mipMap, index));
                    }
                    index++;
                }
            }

            index = 0;
            foreach (var mipMap in textureObject.Mips) 
            {
                if (mipMap.Data != null)
                {
                    if (minIndex == -1) minIndex = index;
                    mipMapBox.Items.Add(MipMapInfo.AddMipMap(mipMap, index));
                }
                index++;
            }
        }

        private void LoadTexture(int index = 0)
        {
            if (textureObject.Mips.Count > 0)
            {
                UpdateTextureInfo(index);

                Stream stream;
                if (index < textureObject.FirstResourceMemMip)
                    stream = TextureFileCache.Instance.Texture2D.GetObjectStream(index - minIndex);
                else
                    stream = textureObject.GetObjectStream(index);

                ddsFile.Load(stream);
                textureView.Image = BitmapSourceToBitmap(ddsFile.BitmapSource);
                CenterTexture();

                //importDDSToolStripMenuItem.Enabled = true;
                exportDDSToolStripMenuItem.Enabled = true;
            }
        }

        private static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            Bitmap bitmap;

            using (MemoryStream outStream = new())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(outStream);
                bitmap = new Bitmap(outStream);
            }

            return bitmap;
        }

        private void UpdateTextureInfo(int mipmapIndex)
        {
            var mipMap = textureObject.Mips[mipmapIndex];
            formatLabel.Text = mipMap.OverrideFormat.ToString();
            widthLabel.Text = $"{mipMap.SizeX} x {mipMap.SizeY}";
            mipMapBox.SelectedIndex = mipmapIndex - minIndex;
        }

        private void texturePanel_Resize(object sender, EventArgs e)
        {
            CenterTexture();
        }

        private void CenterTexture()
        {
            if (textureView.Image != null)
            {
                int x = (texturePanel.ClientSize.Width - textureView.Width) / 2;
                int y = (texturePanel.ClientSize.Height - textureView.Height) / 2;

                textureView.Location = new Point(Math.Max(x, 0), Math.Max(y, 0));
            }
        }

        private static MemoryStream BitmapSourceToPng(BitmapSource bitmapSource)
        {
            MemoryStream outStream = new();

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(outStream);

            return outStream;
        }

        private void exportDDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textureObject.Mips.Count == 0) return;

            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = textureNameLabel.Text + ".dds";
            saveFileDialog.Filter = "DDS Files (*.dds)|*.dds|PNG Files (*.png)|*.png";
            saveFileDialog.Title = "Save a Texture File";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog.FileName;

                var texture = textureObject;
                var cacheTexture = TextureFileCache.Instance.Texture2D;
                if (textureEntry != null && cacheTexture.Mips.Count > 0)
                    texture = cacheTexture;

                var stream = texture.GetMipMapsStream();
                if (stream == null) return;

                bool isPNG = filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
                if (isPNG)
                {
                    ddsFile.Load(stream);
                    stream = BitmapSourceToPng(ddsFile.BitmapSource);
                }

                using var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }
        }

        private void importDDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Import function not ready", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}
