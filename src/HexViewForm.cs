using Be.Windows.Forms;
using System.Reflection;

namespace MHUpkManager
{
    public partial class HexViewForm : Form
    {
        private byte[] hexData;
        private string title;

        public HexViewForm()
        {
            InitializeComponent();

            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(hexBox, true);
        }

        public void SetTitle(string name)
        {
            title = name;
            Text = $"Hex Viewer - [{title}]";
        }

        public void SetHexData(byte[] data)
        {
            hexData = data;
            hexBox.ByteProvider = new DynamicByteProvider(data);
        }

        private void exportBinaryFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hexData == null || hexData.Length == 0) return;

            using SaveFileDialog saveFileDialog = new();
            saveFileDialog.FileName = title + ".bin";
            saveFileDialog.Filter = "Binary Files (*.bin)|*.bin|All Files (*.*)|*.*";
            saveFileDialog.Title = "Export Binary File";
            saveFileDialog.DefaultExt = "bin";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                File.WriteAllBytes(saveFileDialog.FileName, hexData);
        }

        private void dumpHexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hexData == null || hexData.Length == 0) return;

            string hexText = BitConverter.ToString(hexData).Replace("-", " ");
            Clipboard.SetText(hexText);
        }
    }
}
