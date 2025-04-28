using System.Collections.Generic;

namespace UpkManager.Models.UpkFile.Properties
{
    public class VirtualNode
    {
        public string Text { get; set; }
        public List<VirtualNode> Children { get; set; } = [];

        public VirtualNode() { }

        public VirtualNode(string text)
        {
            Text = text;
        }
    }
}
