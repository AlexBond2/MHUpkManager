using Be.Windows.Forms;

namespace MHUpkManager
{
    public partial class HexViewForm : Form
    {
        public HexViewForm()
        {
            InitializeComponent();
        }

        public void SetHexData(byte[] data)
        {
            hexBox.ByteProvider = new DynamicByteProvider(data);
        }

        public void SetTitle(string name)
        {
            Text = "Hex Viewer - " + name;
        }
    }
}
