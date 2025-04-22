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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
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
            tabControl2 = new TabControl();
            propertyFilePage = new TabPage();
            propertyGrid = new PropertyGrid();
            objectsPage = new TabPage();
            panel1 = new Panel();
            panel3 = new Panel();
            treeView1 = new TreeView();
            panel2 = new Panel();
            tabControl1 = new TabControl();
            namePage = new TabPage();
            nameGridView = new DataGridView();
            nameTableIndex = new DataGridViewTextBoxColumn();
            nameTableName = new DataGridViewTextBoxColumn();
            nameTableFlags = new DataGridViewTextBoxColumn();
            importPage = new TabPage();
            importGridView = new DataGridView();
            importIndex = new DataGridViewTextBoxColumn();
            importObject = new DataGridViewTextBoxColumn();
            importClass = new DataGridViewTextBoxColumn();
            importPakage = new DataGridViewTextBoxColumn();
            importGroup = new DataGridViewTextBoxColumn();
            exportPage = new TabPage();
            exportGridView = new DataGridView();
            IndexColumn1 = new DataGridViewTextBoxColumn();
            exportColumn1 = new DataGridViewTextBoxColumn();
            exportColumn2 = new DataGridViewTextBoxColumn();
            exportPakage = new DataGridViewTextBoxColumn();
            exportColumn3 = new DataGridViewTextBoxColumn();
            exportColumn4 = new DataGridViewTextBoxColumn();
            exportColumn5 = new DataGridViewTextBoxColumn();
            exportColumn6 = new DataGridViewTextBoxColumn();
            propertyPage = new TabPage();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl2.SuspendLayout();
            propertyFilePage.SuspendLayout();
            objectsPage.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            tabControl1.SuspendLayout();
            namePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nameGridView).BeginInit();
            importPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)importGridView).BeginInit();
            exportPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)exportGridView).BeginInit();
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
            splitContainer1.Panel1.Controls.Add(tabControl2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(981, 441);
            splitContainer1.SplitterDistance = 326;
            splitContainer1.TabIndex = 2;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(propertyFilePage);
            tabControl2.Controls.Add(objectsPage);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new Point(0, 0);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(326, 441);
            tabControl2.TabIndex = 1;
            // 
            // propertyFilePage
            // 
            propertyFilePage.Controls.Add(propertyGrid);
            propertyFilePage.Location = new Point(4, 24);
            propertyFilePage.Name = "propertyFilePage";
            propertyFilePage.Padding = new Padding(3);
            propertyFilePage.Size = new Size(318, 413);
            propertyFilePage.TabIndex = 1;
            propertyFilePage.Text = "File Properties";
            propertyFilePage.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            propertyGrid.DisabledItemForeColor = SystemColors.ControlText;
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.HelpBackColor = SystemColors.GradientInactiveCaption;
            propertyGrid.HelpBorderColor = SystemColors.GradientActiveCaption;
            propertyGrid.Location = new Point(3, 3);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new Size(312, 407);
            propertyGrid.TabIndex = 0;
            // 
            // objectsPage
            // 
            objectsPage.Controls.Add(panel1);
            objectsPage.Location = new Point(4, 24);
            objectsPage.Name = "objectsPage";
            objectsPage.Padding = new Padding(3);
            objectsPage.Size = new Size(318, 413);
            objectsPage.TabIndex = 0;
            objectsPage.Text = "Objects";
            objectsPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(312, 407);
            panel1.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.Controls.Add(treeView1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 40);
            panel3.Name = "panel3";
            panel3.Size = new Size(312, 367);
            panel3.TabIndex = 2;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(312, 367);
            treeView1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Transparent;
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(312, 40);
            panel2.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(namePage);
            tabControl1.Controls.Add(importPage);
            tabControl1.Controls.Add(exportPage);
            tabControl1.Controls.Add(propertyPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(651, 441);
            tabControl1.TabIndex = 0;
            // 
            // namePage
            // 
            namePage.Controls.Add(nameGridView);
            namePage.Location = new Point(4, 24);
            namePage.Name = "namePage";
            namePage.Size = new Size(643, 413);
            namePage.TabIndex = 3;
            namePage.Text = "Name Table";
            namePage.UseVisualStyleBackColor = true;
            // 
            // nameGridView
            // 
            nameGridView.AllowDrop = true;
            nameGridView.AllowUserToAddRows = false;
            nameGridView.AllowUserToDeleteRows = false;
            nameGridView.AllowUserToResizeRows = false;
            nameGridView.BackgroundColor = SystemColors.Window;
            nameGridView.BorderStyle = BorderStyle.None;
            nameGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            nameGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            nameGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            nameGridView.Columns.AddRange(new DataGridViewColumn[] { nameTableIndex, nameTableName, nameTableFlags });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            nameGridView.DefaultCellStyle = dataGridViewCellStyle2;
            nameGridView.Dock = DockStyle.Fill;
            nameGridView.EnableHeadersVisualStyles = false;
            nameGridView.GridColor = SystemColors.GradientActiveCaption;
            nameGridView.Location = new Point(0, 0);
            nameGridView.Name = "nameGridView";
            nameGridView.RowHeadersVisible = false;
            nameGridView.Size = new Size(643, 413);
            nameGridView.TabIndex = 1;
            // 
            // nameTableIndex
            // 
            nameTableIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            nameTableIndex.DataPropertyName = "Index";
            nameTableIndex.HeaderText = "Index";
            nameTableIndex.Name = "nameTableIndex";
            nameTableIndex.Width = 50;
            // 
            // nameTableName
            // 
            nameTableName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            nameTableName.DataPropertyName = "Name";
            nameTableName.HeaderText = "Name";
            nameTableName.Name = "nameTableName";
            // 
            // nameTableFlags
            // 
            nameTableFlags.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            nameTableFlags.DataPropertyName = "Flags";
            nameTableFlags.HeaderText = "Flags";
            nameTableFlags.Name = "nameTableFlags";
            nameTableFlags.Width = 120;
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
            importGridView.AllowUserToAddRows = false;
            importGridView.AllowUserToDeleteRows = false;
            importGridView.AllowUserToResizeRows = false;
            importGridView.BackgroundColor = SystemColors.Window;
            importGridView.BorderStyle = BorderStyle.None;
            importGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            importGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            importGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            importGridView.Columns.AddRange(new DataGridViewColumn[] { importIndex, importObject, importClass, importPakage, importGroup });
            importGridView.DefaultCellStyle = dataGridViewCellStyle2;
            importGridView.Dock = DockStyle.Fill;
            importGridView.EnableHeadersVisualStyles = false;
            importGridView.GridColor = SystemColors.GradientActiveCaption;
            importGridView.Location = new Point(0, 0);
            importGridView.Name = "importGridView";
            importGridView.ReadOnly = true;
            importGridView.RowHeadersVisible = false;
            importGridView.Size = new Size(643, 413);
            importGridView.TabIndex = 0;
            importGridView.CellValueNeeded += importGridView_CellValueNeeded;
            // 
            // importIndex
            // 
            importIndex.DataPropertyName = "Index";
            importIndex.FillWeight = 50F;
            importIndex.HeaderText = "Index";
            importIndex.Name = "importIndex";
            importIndex.ReadOnly = true;
            importIndex.Width = 50;
            // 
            // importObject
            // 
            importObject.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            importObject.DataPropertyName = "Object";
            importObject.HeaderText = "Object";
            importObject.Name = "importObject";
            importObject.ReadOnly = true;
            // 
            // importClass
            // 
            importClass.DataPropertyName = "Class";
            importClass.HeaderText = "Class";
            importClass.Name = "importClass";
            importClass.ReadOnly = true;
            // 
            // importPakage
            // 
            importPakage.DataPropertyName = "Package";
            importPakage.HeaderText = "Package";
            importPakage.Name = "importPakage";
            importPakage.ReadOnly = true;
            importPakage.Width = 80;
            // 
            // importGroup
            // 
            importGroup.DataPropertyName = "Group";
            importGroup.HeaderText = "Group";
            importGroup.Name = "importGroup";
            importGroup.ReadOnly = true;
            importGroup.Width = 70;
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
            exportGridView.AllowUserToAddRows = false;
            exportGridView.AllowUserToDeleteRows = false;
            exportGridView.AllowUserToResizeRows = false;
            exportGridView.BackgroundColor = SystemColors.Window;
            exportGridView.BorderStyle = BorderStyle.None;
            exportGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            exportGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            exportGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            exportGridView.Columns.AddRange(new DataGridViewColumn[] { IndexColumn1, exportColumn1, exportColumn2, exportPakage, exportColumn3, exportColumn4, exportColumn5, exportColumn6 });
            exportGridView.DefaultCellStyle = dataGridViewCellStyle2;
            exportGridView.Dock = DockStyle.Fill;
            exportGridView.EnableHeadersVisualStyles = false;
            exportGridView.GridColor = SystemColors.GradientActiveCaption;
            exportGridView.Location = new Point(0, 0);
            exportGridView.Name = "exportGridView";
            exportGridView.RowHeadersVisible = false;
            exportGridView.Size = new Size(643, 413);
            exportGridView.TabIndex = 1;
            exportGridView.CellValueNeeded += exportGridView_CellValueNeeded;
            // 
            // IndexColumn1
            // 
            IndexColumn1.DataPropertyName = "Index";
            IndexColumn1.HeaderText = "Index";
            IndexColumn1.Name = "IndexColumn1";
            IndexColumn1.Width = 50;
            // 
            // exportColumn1
            // 
            exportColumn1.DataPropertyName = "Object";
            exportColumn1.HeaderText = "Object";
            exportColumn1.Name = "exportColumn1";
            exportColumn1.Width = 180;
            // 
            // exportColumn2
            // 
            exportColumn2.DataPropertyName = "Class";
            exportColumn2.HeaderText = "Class";
            exportColumn2.Name = "exportColumn2";
            // 
            // exportPakage
            // 
            exportPakage.DataPropertyName = "Pakage";
            exportPakage.HeaderText = "Pakage";
            exportPakage.Name = "exportPakage";
            // 
            // exportColumn3
            // 
            exportColumn3.DataPropertyName = "Group";
            exportColumn3.HeaderText = "Group";
            exportColumn3.Name = "exportColumn3";
            exportColumn3.Width = 80;
            // 
            // exportColumn4
            // 
            exportColumn4.DataPropertyName = "Flags";
            exportColumn4.HeaderText = "Flags";
            exportColumn4.Name = "exportColumn4";
            exportColumn4.Width = 120;
            // 
            // exportColumn5
            // 
            exportColumn5.DataPropertyName = "Size";
            exportColumn5.HeaderText = "Size";
            exportColumn5.Name = "exportColumn5";
            exportColumn5.Width = 50;
            // 
            // exportColumn6
            // 
            exportColumn6.DataPropertyName = "Offset";
            exportColumn6.HeaderText = "Offset";
            exportColumn6.Name = "exportColumn6";
            exportColumn6.Width = 50;
            // 
            // propertyPage
            // 
            propertyPage.Location = new Point(4, 24);
            propertyPage.Name = "propertyPage";
            propertyPage.Padding = new Padding(3);
            propertyPage.Size = new Size(643, 413);
            propertyPage.TabIndex = 0;
            propertyPage.Text = "Object Properties";
            propertyPage.UseVisualStyleBackColor = true;
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
            tabControl2.ResumeLayout(false);
            propertyFilePage.ResumeLayout(false);
            objectsPage.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            namePage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nameGridView).EndInit();
            importPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)importGridView).EndInit();
            exportPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)exportGridView).EndInit();
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
        private DataGridView nameGridView;
        private PropertyGrid propertyGrid;
        private ToolStripStatusLabel progressStatus;
        private DataGridViewTextBoxColumn IndexColumn1;
        private DataGridViewTextBoxColumn exportColumn1;
        private DataGridViewTextBoxColumn exportColumn2;
        private DataGridViewTextBoxColumn exportPakage;
        private DataGridViewTextBoxColumn exportColumn3;
        private DataGridViewTextBoxColumn exportColumn4;
        private DataGridViewTextBoxColumn exportColumn5;
        private DataGridViewTextBoxColumn exportColumn6;
        private DataGridViewTextBoxColumn importIndex;
        private DataGridViewTextBoxColumn importObject;
        private DataGridViewTextBoxColumn importClass;
        private DataGridViewTextBoxColumn importPakage;
        private DataGridViewTextBoxColumn importGroup;
        private TabControl tabControl2;
        private TabPage objectsPage;
        private TabPage propertyFilePage;
        private Panel panel3;
        private DataGridViewTextBoxColumn nameTableIndex;
        private DataGridViewTextBoxColumn nameTableName;
        private DataGridViewTextBoxColumn nameTableFlags;
    }
}
