namespace MHUpkManager
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openFileMenuItem = new ToolStripMenuItem();
            saveMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            totalStatus = new ToolStripStatusLabel();
            progressStatus = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            panel2 = new Panel();
            panel1 = new Panel();
            treeView1 = new TreeView();
            tabControl1 = new TabControl();
            propertyPage = new TabPage();
            propertyGrid1 = new PropertyGrid();
            importPage = new TabPage();
            importGridView = new DataGridView();
            IndexColumn = new DataGridViewTextBoxColumn();
            importColumn1 = new DataGridViewTextBoxColumn();
            importColumn2 = new DataGridViewTextBoxColumn();
            importColumn3 = new DataGridViewTextBoxColumn();
            importColum4 = new DataGridViewTextBoxColumn();
            exportPage = new TabPage();
            exportGridView = new DataGridView();
            IndexColumn1 = new DataGridViewTextBoxColumn();
            exportColumn1 = new DataGridViewTextBoxColumn();
            exportColumn2 = new DataGridViewTextBoxColumn();
            exportColumn3 = new DataGridViewTextBoxColumn();
            exportColumn4 = new DataGridViewTextBoxColumn();
            exportColumn5 = new DataGridViewTextBoxColumn();
            exportColumn6 = new DataGridViewTextBoxColumn();
            namePage = new TabPage();
            dataGridView1 = new DataGridView();
            nameTableIndex = new DataGridViewTextBoxColumn();
            nameTableName = new DataGridViewTextBoxColumn();
            nameTableFlags = new DataGridViewTextBoxColumn();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            tabControl1.SuspendLayout();
            propertyPage.SuspendLayout();
            importPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)importGridView).BeginInit();
            exportPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)exportGridView).BeginInit();
            namePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(981, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openFileMenuItem, saveMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openFileMenuItem
            // 
            openFileMenuItem.Name = "openFileMenuItem";
            openFileMenuItem.Size = new Size(112, 22);
            openFileMenuItem.Text = "Open...";
            openFileMenuItem.Click += openMenuItem_Click;
            // 
            // saveMenuItem
            // 
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.Size = new Size(112, 22);
            saveMenuItem.Text = "Save...";
            saveMenuItem.Click += saveMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, totalStatus, progressStatus });
            statusStrip1.Location = new Point(0, 465);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(981, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(77, 17);
            toolStripStatusLabel1.Text = "Total classes: ";
            // 
            // totalStatus
            // 
            totalStatus.AutoSize = false;
            totalStatus.Name = "totalStatus";
            totalStatus.Size = new Size(50, 17);
            totalStatus.Text = "0";
            totalStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // progressStatus
            // 
            progressStatus.Name = "progressStatus";
            progressStatus.Size = new Size(0, 17);
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(981, 441);
            splitContainer1.SplitterDistance = 326;
            splitContainer1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(326, 40);
            panel2.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.Controls.Add(treeView1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(326, 441);
            panel1.TabIndex = 0;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(326, 441);
            treeView1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(propertyPage);
            tabControl1.Controls.Add(importPage);
            tabControl1.Controls.Add(exportPage);
            tabControl1.Controls.Add(namePage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(651, 441);
            tabControl1.TabIndex = 0;
            // 
            // propertyPage
            // 
            propertyPage.Controls.Add(propertyGrid1);
            propertyPage.Location = new Point(4, 24);
            propertyPage.Name = "propertyPage";
            propertyPage.Padding = new Padding(3);
            propertyPage.Size = new Size(643, 413);
            propertyPage.TabIndex = 0;
            propertyPage.Text = "File Properties";
            propertyPage.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Fill;
            propertyGrid1.Location = new Point(3, 3);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(637, 407);
            propertyGrid1.TabIndex = 0;
            // 
            // importPage
            // 
            importPage.Controls.Add(importGridView);
            importPage.Location = new Point(4, 24);
            importPage.Name = "importPage";
            importPage.Size = new Size(643, 413);
            importPage.TabIndex = 1;
            importPage.Text = "Import Table";
            importPage.UseVisualStyleBackColor = true;
            // 
            // importGridView
            // 
            importGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            importGridView.Columns.AddRange(new DataGridViewColumn[] { IndexColumn, importColumn1, importColumn2, importColumn3, importColum4 });
            importGridView.Dock = DockStyle.Fill;
            importGridView.Location = new Point(0, 0);
            importGridView.Name = "importGridView";
            importGridView.Size = new Size(643, 413);
            importGridView.TabIndex = 0;
            // 
            // IndexColumn
            // 
            IndexColumn.HeaderText = "Index";
            IndexColumn.Name = "IndexColumn";
            // 
            // importColumn1
            // 
            importColumn1.HeaderText = "Object";
            importColumn1.Name = "importColumn1";
            // 
            // importColumn2
            // 
            importColumn2.HeaderText = "Class";
            importColumn2.Name = "importColumn2";
            // 
            // importColumn3
            // 
            importColumn3.HeaderText = "Package";
            importColumn3.Name = "importColumn3";
            // 
            // importColum4
            // 
            importColum4.HeaderText = "Group";
            importColum4.Name = "importColum4";
            // 
            // exportPage
            // 
            exportPage.Controls.Add(exportGridView);
            exportPage.Location = new Point(4, 24);
            exportPage.Name = "exportPage";
            exportPage.Size = new Size(643, 413);
            exportPage.TabIndex = 2;
            exportPage.Text = "Export Table";
            exportPage.UseVisualStyleBackColor = true;
            // 
            // exportGridView
            // 
            exportGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            exportGridView.Columns.AddRange(new DataGridViewColumn[] { IndexColumn1, exportColumn1, exportColumn2, exportColumn3, exportColumn4, exportColumn5, exportColumn6 });
            exportGridView.Dock = DockStyle.Fill;
            exportGridView.Location = new Point(0, 0);
            exportGridView.Name = "exportGridView";
            exportGridView.Size = new Size(643, 413);
            exportGridView.TabIndex = 1;
            // 
            // IndexColumn1
            // 
            IndexColumn1.HeaderText = "Index";
            IndexColumn1.Name = "IndexColumn1";
            // 
            // exportColumn1
            // 
            exportColumn1.HeaderText = "Class";
            exportColumn1.Name = "exportColumn1";
            // 
            // exportColumn2
            // 
            exportColumn2.HeaderText = "Parent";
            exportColumn2.Name = "exportColumn2";
            // 
            // exportColumn3
            // 
            exportColumn3.HeaderText = "Group";
            exportColumn3.Name = "exportColumn3";
            // 
            // exportColumn4
            // 
            exportColumn4.HeaderText = "Flags";
            exportColumn4.Name = "exportColumn4";
            // 
            // exportColumn5
            // 
            exportColumn5.HeaderText = "Size";
            exportColumn5.Name = "exportColumn5";
            // 
            // exportColumn6
            // 
            exportColumn6.HeaderText = "Offset";
            exportColumn6.Name = "exportColumn6";
            // 
            // namePage
            // 
            namePage.Controls.Add(dataGridView1);
            namePage.Location = new Point(4, 24);
            namePage.Name = "namePage";
            namePage.Size = new Size(643, 413);
            namePage.TabIndex = 3;
            namePage.Text = "Name Table";
            namePage.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { nameTableIndex, nameTableName, nameTableFlags });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(643, 413);
            dataGridView1.TabIndex = 1;
            // 
            // nameTableIndex
            // 
            nameTableIndex.HeaderText = "Index";
            nameTableIndex.Name = "nameTableIndex";
            // 
            // nameTableName
            // 
            nameTableName.HeaderText = "Name";
            nameTableName.Name = "nameTableName";
            // 
            // nameTableFlags
            // 
            nameTableFlags.HeaderText = "Flags";
            nameTableFlags.Name = "nameTableFlags";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(981, 487);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MH UPK Manager v.1.0 by AlexBond";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            propertyPage.ResumeLayout(false);
            importPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)importGridView).EndInit();
            exportPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)exportGridView).EndInit();
            namePage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel totalStatus;
        private SplitContainer splitContainer1;
        private Panel panel1;
        private TabControl tabControl1;
        private TabPage propertyPage;
        private TabPage importPage;
        private Panel panel2;
        private TreeView treeView1;
        private TabPage exportPage;
        private DataGridView importGridView;
        private DataGridView exportGridView;
        private ToolStripMenuItem openFileMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private TabPage namePage;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn nameTableIndex;
        private DataGridViewTextBoxColumn nameTableName;
        private DataGridViewTextBoxColumn nameTableFlags;
        private PropertyGrid propertyGrid1;
        private DataGridViewTextBoxColumn IndexColumn;
        private DataGridViewTextBoxColumn importColumn1;
        private DataGridViewTextBoxColumn importColumn2;
        private DataGridViewTextBoxColumn importColumn3;
        private DataGridViewTextBoxColumn importColum4;
        private DataGridViewTextBoxColumn IndexColumn1;
        private DataGridViewTextBoxColumn exportColumn1;
        private DataGridViewTextBoxColumn exportColumn2;
        private DataGridViewTextBoxColumn exportColumn3;
        private DataGridViewTextBoxColumn exportColumn4;
        private DataGridViewTextBoxColumn exportColumn5;
        private DataGridViewTextBoxColumn exportColumn6;
        private ToolStripStatusLabel progressStatus;
    }
}
