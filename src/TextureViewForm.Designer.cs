
namespace MHUpkManager
{
    partial class TextureViewForm
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            textureToolStripMenuItem = new ToolStripMenuItem();
            importDDSToolStripMenuItem = new ToolStripMenuItem();
            exportDDSToolStripMenuItem = new ToolStripMenuItem();
            panel4 = new Panel();
            texturePanel = new Panel();
            textureView = new PictureBox();
            panel5 = new Panel();
            textureFileLabel = new Label();
            label5 = new Label();
            mipMapsLabel = new Label();
            label4 = new Label();
            textureNameLabel = new Label();
            label3 = new Label();
            textureGuidLabel = new Label();
            label2 = new Label();
            panel3 = new Panel();
            sizeLabel = new Label();
            label13 = new Label();
            mipMapBox = new ComboBox();
            label9 = new Label();
            widthLabel = new Label();
            label8 = new Label();
            formatLabel = new Label();
            mipMapLabel = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            modInfoToolStripMenuItem = new ToolStripMenuItem();
            textureToolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripSeparator();
            reloadModsToolStripMenuItem = new ToolStripMenuItem();
            openModsFolderToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            applyModToolStripMenuItem = new ToolStripMenuItem();
            resetModToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            saveModsAsToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            panel4.SuspendLayout();
            texturePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)textureView).BeginInit();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { textureToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(464, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // textureToolStripMenuItem
            // 
            textureToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importDDSToolStripMenuItem, exportDDSToolStripMenuItem });
            textureToolStripMenuItem.Name = "textureToolStripMenuItem";
            textureToolStripMenuItem.Size = new Size(57, 20);
            textureToolStripMenuItem.Text = "Texture";
            // 
            // importDDSToolStripMenuItem
            // 
            importDDSToolStripMenuItem.Enabled = false;
            importDDSToolStripMenuItem.Name = "importDDSToolStripMenuItem";
            importDDSToolStripMenuItem.Size = new Size(160, 22);
            importDDSToolStripMenuItem.Text = "Import Texture...";
            importDDSToolStripMenuItem.Click += importDDSToolStripMenuItem_Click;
            // 
            // exportDDSToolStripMenuItem
            // 
            exportDDSToolStripMenuItem.Enabled = false;
            exportDDSToolStripMenuItem.Name = "exportDDSToolStripMenuItem";
            exportDDSToolStripMenuItem.Size = new Size(160, 22);
            exportDDSToolStripMenuItem.Text = "Export Texture...";
            exportDDSToolStripMenuItem.Click += exportDDSToolStripMenuItem_Click;
            // 
            // panel4
            // 
            panel4.Controls.Add(texturePanel);
            panel4.Controls.Add(panel5);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(0, 24);
            panel4.Name = "panel4";
            panel4.Size = new Size(464, 374);
            panel4.TabIndex = 5;
            // 
            // texturePanel
            // 
            texturePanel.AutoScroll = true;
            texturePanel.BackColor = Color.Silver;
            texturePanel.BorderStyle = BorderStyle.FixedSingle;
            texturePanel.Controls.Add(textureView);
            texturePanel.Dock = DockStyle.Fill;
            texturePanel.Location = new Point(0, 72);
            texturePanel.Name = "texturePanel";
            texturePanel.Size = new Size(464, 302);
            texturePanel.TabIndex = 2;
            texturePanel.Resize += texturePanel_Resize;
            // 
            // textureView
            // 
            textureView.BackColor = Color.LightGray;
            textureView.BackgroundImage = Properties.Resources.bk;
            textureView.Location = new Point(0, 0);
            textureView.Name = "textureView";
            textureView.Size = new Size(256, 256);
            textureView.SizeMode = PictureBoxSizeMode.AutoSize;
            textureView.TabIndex = 1;
            textureView.TabStop = false;
            // 
            // panel5
            // 
            panel5.Controls.Add(textureFileLabel);
            panel5.Controls.Add(label5);
            panel5.Controls.Add(mipMapsLabel);
            panel5.Controls.Add(label4);
            panel5.Controls.Add(textureNameLabel);
            panel5.Controls.Add(label3);
            panel5.Controls.Add(textureGuidLabel);
            panel5.Controls.Add(label2);
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(0, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(464, 72);
            panel5.TabIndex = 0;
            // 
            // textureFileLabel
            // 
            textureFileLabel.AutoSize = true;
            textureFileLabel.Location = new Point(105, 37);
            textureFileLabel.Name = "textureFileLabel";
            textureFileLabel.Size = new Size(36, 15);
            textureFileLabel.TabIndex = 7;
            textureFileLabel.Text = "None";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 37);
            label5.Name = "label5";
            label5.Size = new Size(105, 15);
            label5.TabIndex = 6;
            label5.Text = "Texture File Cache:";
            // 
            // mipMapsLabel
            // 
            mipMapsLabel.AutoSize = true;
            mipMapsLabel.Location = new Point(105, 52);
            mipMapsLabel.Name = "mipMapsLabel";
            mipMapsLabel.Size = new Size(36, 15);
            mipMapsLabel.TabIndex = 5;
            mipMapsLabel.Text = "None";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(48, 52);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 4;
            label4.Text = "MipMaps:";
            // 
            // textureNameLabel
            // 
            textureNameLabel.AutoSize = true;
            textureNameLabel.Location = new Point(105, 22);
            textureNameLabel.Name = "textureNameLabel";
            textureNameLabel.Size = new Size(36, 15);
            textureNameLabel.TabIndex = 3;
            textureNameLabel.Text = "None";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 22);
            label3.Name = "label3";
            label3.Size = new Size(83, 15);
            label3.TabIndex = 2;
            label3.Text = "Texture Name:";
            // 
            // textureGuidLabel
            // 
            textureGuidLabel.AutoSize = true;
            textureGuidLabel.Location = new Point(105, 7);
            textureGuidLabel.Name = "textureGuidLabel";
            textureGuidLabel.Size = new Size(36, 15);
            textureGuidLabel.TabIndex = 1;
            textureGuidLabel.Text = "None";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(73, 7);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 0;
            label2.Text = "Guid:";
            // 
            // panel3
            // 
            panel3.Controls.Add(sizeLabel);
            panel3.Controls.Add(label13);
            panel3.Controls.Add(mipMapBox);
            panel3.Controls.Add(label9);
            panel3.Controls.Add(widthLabel);
            panel3.Controls.Add(label8);
            panel3.Controls.Add(formatLabel);
            panel3.Controls.Add(mipMapLabel);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(0, 353);
            panel3.Name = "panel3";
            panel3.Size = new Size(464, 45);
            panel3.TabIndex = 4;
            // 
            // sizeLabel
            // 
            sizeLabel.AutoSize = true;
            sizeLabel.Location = new Point(426, 22);
            sizeLabel.Name = "sizeLabel";
            sizeLabel.Size = new Size(36, 15);
            sizeLabel.TabIndex = 21;
            sizeLabel.Text = "None";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(373, 22);
            label13.Name = "label13";
            label13.Size = new Size(56, 15);
            label13.TabIndex = 18;
            label13.Text = "Data size:";
            // 
            // mipMapBox
            // 
            mipMapBox.DropDownStyle = ComboBoxStyle.DropDownList;
            mipMapBox.FlatStyle = FlatStyle.Flat;
            mipMapBox.FormattingEnabled = true;
            mipMapBox.Location = new Point(73, 11);
            mipMapBox.Name = "mipMapBox";
            mipMapBox.Size = new Size(46, 23);
            mipMapBox.TabIndex = 16;
            mipMapBox.SelectedIndexChanged += mipMapBox_SelectedIndexChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(175, 22);
            label9.Name = "label9";
            label9.Size = new Size(48, 15);
            label9.TabIndex = 8;
            label9.Text = "Format:";
            // 
            // widthLabel
            // 
            widthLabel.AutoSize = true;
            widthLabel.Location = new Point(220, 7);
            widthLabel.Name = "widthLabel";
            widthLabel.Size = new Size(36, 15);
            widthLabel.TabIndex = 13;
            widthLabel.Text = "None";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(193, 7);
            label8.Name = "label8";
            label8.Size = new Size(30, 15);
            label8.TabIndex = 9;
            label8.Text = "Size:";
            // 
            // formatLabel
            // 
            formatLabel.AutoSize = true;
            formatLabel.Location = new Point(220, 22);
            formatLabel.Name = "formatLabel";
            formatLabel.Size = new Size(36, 15);
            formatLabel.TabIndex = 12;
            formatLabel.Text = "None";
            // 
            // mipMapLabel
            // 
            mipMapLabel.AutoSize = true;
            mipMapLabel.Location = new Point(12, 14);
            mipMapLabel.Name = "mipMapLabel";
            mipMapLabel.Size = new Size(55, 15);
            mipMapLabel.TabIndex = 10;
            mipMapLabel.Text = "MipMap:";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // modInfoToolStripMenuItem
            // 
            modInfoToolStripMenuItem.Name = "modInfoToolStripMenuItem";
            modInfoToolStripMenuItem.Size = new Size(32, 19);
            // 
            // textureToolStripMenuItem1
            // 
            textureToolStripMenuItem1.Name = "textureToolStripMenuItem1";
            textureToolStripMenuItem1.Size = new Size(32, 19);
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(6, 6);
            // 
            // reloadModsToolStripMenuItem
            // 
            reloadModsToolStripMenuItem.Name = "reloadModsToolStripMenuItem";
            reloadModsToolStripMenuItem.Size = new Size(32, 19);
            // 
            // openModsFolderToolStripMenuItem
            // 
            openModsFolderToolStripMenuItem.Name = "openModsFolderToolStripMenuItem";
            openModsFolderToolStripMenuItem.Size = new Size(32, 19);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(6, 6);
            // 
            // applyModToolStripMenuItem
            // 
            applyModToolStripMenuItem.Name = "applyModToolStripMenuItem";
            applyModToolStripMenuItem.Size = new Size(32, 19);
            // 
            // resetModToolStripMenuItem
            // 
            resetModToolStripMenuItem.Name = "resetModToolStripMenuItem";
            resetModToolStripMenuItem.Size = new Size(32, 19);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(6, 6);
            // 
            // saveModsAsToolStripMenuItem
            // 
            saveModsAsToolStripMenuItem.Name = "saveModsAsToolStripMenuItem";
            saveModsAsToolStripMenuItem.Size = new Size(32, 19);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(32, 19);
            // 
            // TextureViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 398);
            Controls.Add(panel3);
            Controls.Add(panel4);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(480, 410);
            Name = "TextureViewForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TextureView";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel4.ResumeLayout(false);
            texturePanel.ResumeLayout(false);
            texturePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)textureView).EndInit();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private Panel panel4;
        private Panel panel3;
        private PictureBox textureView;
        private Panel panel5;
        private Label textureGuidLabel;
        private Label label2;
        private Label textureNameLabel;
        private Label label3;
        private Label textureFileLabel;
        private Label label5;
        private Label mipMapsLabel;
        private Label label4;
        private ToolStripMenuItem textureToolStripMenuItem;
        private ToolStripMenuItem importDDSToolStripMenuItem;
        private ToolStripMenuItem exportDDSToolStripMenuItem;
        private Panel texturePanel;
        private Label mipMapLabel;
        private Label label8;
        private Label label9;
        private Label widthLabel;
        private Label formatLabel;
        private ComboBox mipMapBox;
        private Label sizeLabel;
        private Label label13;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem saveModsAsToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem applyModToolStripMenuItem;
        private ToolStripMenuItem resetModToolStripMenuItem;
        private ToolStripMenuItem reloadModsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem openModsFolderToolStripMenuItem;
        private ToolStripMenuItem modInfoToolStripMenuItem;
        private ToolStripMenuItem textureToolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem3;
    }
}