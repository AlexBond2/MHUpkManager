namespace MHUpkManager
{
    partial class HexViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            hexBox = new Be.Windows.Forms.HexBox();
            menuStrip1 = new MenuStrip();
            editToolStripMenuItem = new ToolStripMenuItem();
            exportBinaryFileToolStripMenuItem = new ToolStripMenuItem();
            dumpHexToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // hexBox
            // 
            hexBox.ColumnInfoVisible = true;
            hexBox.Dock = DockStyle.Fill;
            hexBox.Font = new Font("Courier New", 10F);
            hexBox.LineInfoVisible = true;
            hexBox.Location = new Point(0, 24);
            hexBox.Name = "hexBox";
            hexBox.ReadOnly = true;
            hexBox.SelectionBackColor = SystemColors.Highlight;
            hexBox.ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
            hexBox.Size = new Size(752, 426);
            hexBox.StringViewVisible = true;
            hexBox.TabIndex = 0;
            hexBox.UseFixedBytesPerLine = true;
            hexBox.VScrollBarVisible = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { editToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(752, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportBinaryFileToolStripMenuItem, dumpHexToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // exportBinaryFileToolStripMenuItem
            // 
            exportBinaryFileToolStripMenuItem.Name = "exportBinaryFileToolStripMenuItem";
            exportBinaryFileToolStripMenuItem.Size = new Size(174, 22);
            exportBinaryFileToolStripMenuItem.Text = "Export Binary File...";
            exportBinaryFileToolStripMenuItem.Click += exportBinaryFileToolStripMenuItem_Click;
            // 
            // dumpHexToolStripMenuItem
            // 
            dumpHexToolStripMenuItem.Name = "dumpHexToolStripMenuItem";
            dumpHexToolStripMenuItem.Size = new Size(174, 22);
            dumpHexToolStripMenuItem.Text = "Dump Hex";
            dumpHexToolStripMenuItem.Click += dumpHexToolStripMenuItem_Click;
            // 
            // HexViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(752, 450);
            Controls.Add(hexBox);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "HexViewForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Hex Viewer";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Be.Windows.Forms.HexBox hexBox;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem exportBinaryFileToolStripMenuItem;
        private ToolStripMenuItem dumpHexToolStripMenuItem;
    }
}