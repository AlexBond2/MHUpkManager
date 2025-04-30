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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
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
            objectsPage = new TabPage();
            panel1 = new Panel();
            objectsTree = new TreeView();
            iconList = new ImageList(components);
            panel4 = new Panel();
            filterClear = new Button();
            label1 = new Label();
            filterBox = new TextBox();
            propertyFilePage = new TabPage();
            propertyGrid = new PropertyGrid();
            tabControl1 = new TabControl();
            propertyPage = new TabPage();
            propertiesView = new TreeView();
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
            importOuter = new DataGridViewTextBoxColumn();
            importPakage = new DataGridViewTextBoxColumn();
            exportPage = new TabPage();
            exportGridView = new DataGridView();
            IndexColumn1 = new DataGridViewTextBoxColumn();
            exportColumn1 = new DataGridViewTextBoxColumn();
            exportColumn2 = new DataGridViewTextBoxColumn();
            exportOuter = new DataGridViewTextBoxColumn();
            exportColumn4 = new DataGridViewTextBoxColumn();
            exportColumn5 = new DataGridViewTextBoxColumn();
            buttonColumn = new DataGridViewButtonColumn();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl2.SuspendLayout();
            objectsPage.SuspendLayout();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
            propertyFilePage.SuspendLayout();
            tabControl1.SuspendLayout();
            propertyPage.SuspendLayout();
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
            menuStrip1.Size = new Size(1218, 24);
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
            statusStrip1.Location = new Point(0, 582);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1218, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(79, 17);
            toolStripStatusLabel1.Text = "Total objects: ";
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
            splitContainer1.Size = new Size(1218, 558);
            splitContainer1.SplitterDistance = 404;
            splitContainer1.TabIndex = 2;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(objectsPage);
            tabControl2.Controls.Add(propertyFilePage);
            tabControl2.Dock = DockStyle.Fill;
            tabControl2.Location = new Point(0, 0);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(404, 558);
            tabControl2.TabIndex = 1;
            // 
            // objectsPage
            // 
            objectsPage.Controls.Add(panel1);
            objectsPage.Location = new Point(4, 24);
            objectsPage.Name = "objectsPage";
            objectsPage.Padding = new Padding(3);
            objectsPage.Size = new Size(396, 530);
            objectsPage.TabIndex = 0;
            objectsPage.Text = "Objects";
            objectsPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(objectsTree);
            panel1.Controls.Add(panel4);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(390, 524);
            panel1.TabIndex = 0;
            // 
            // objectsTree
            // 
            objectsTree.Dock = DockStyle.Fill;
            objectsTree.ImageIndex = 0;
            objectsTree.ImageList = iconList;
            objectsTree.Location = new Point(0, 32);
            objectsTree.Name = "objectsTree";
            objectsTree.SelectedImageIndex = 0;
            objectsTree.Size = new Size(390, 492);
            objectsTree.TabIndex = 0;
            objectsTree.AfterSelect += objectsTree_AfterSelect;
            // 
            // iconList
            // 
            iconList.ColorDepth = ColorDepth.Depth32Bit;
            iconList.ImageStream = (ImageListStreamer)resources.GetObject("iconList.ImageStream");
            iconList.TransparentColor = Color.Transparent;
            iconList.Images.SetKeyName(0, "actor");
            iconList.Images.SetKeyName(1, "import");
            iconList.Images.SetKeyName(2, "export");
            iconList.Images.SetKeyName(3, "package");
            iconList.Images.SetKeyName(4, "MatEd");
            iconList.Images.SetKeyName(5, "Material");
            iconList.Images.SetKeyName(6, "texture2d");
            iconList.Images.SetKeyName(7, "Emitter");
            iconList.Images.SetKeyName(8, "ParticleSystem");
            iconList.Images.SetKeyName(9, "SkeletalMeshes");
            iconList.Images.SetKeyName(10, "StaticMeshes");
            iconList.Images.SetKeyName(11, "SoundActor");
            iconList.Images.SetKeyName(12, "CameraActor");
            iconList.Images.SetKeyName(13, "Lighting");
            iconList.Images.SetKeyName(14, "flash");
            iconList.Images.SetKeyName(15, "anim");
            iconList.Images.SetKeyName(16, "animseq");
            iconList.Images.SetKeyName(17, "Layers");
            iconList.Images.SetKeyName(18, "Media");
            iconList.Images.SetKeyName(19, "partfx");
            iconList.Images.SetKeyName(20, "Refr");
            iconList.Images.SetKeyName(21, "ref");
            iconList.Images.SetKeyName(22, "powerfx");
            iconList.Images.SetKeyName(23, "Collsion");
            iconList.Images.SetKeyName(24, "Sphere");
            iconList.Images.SetKeyName(25, "box");
            iconList.Images.SetKeyName(26, "blueBox");
            iconList.Images.SetKeyName(27, "phys");
            iconList.Images.SetKeyName(28, "Sphyl");
            iconList.Images.SetKeyName(29, "bank");
            iconList.Images.SetKeyName(30, "play");
            iconList.Images.SetKeyName(31, "socket");
            iconList.Images.SetKeyName(32, "rbbody");
            iconList.Images.SetKeyName(33, "setup");
            iconList.Images.SetKeyName(34, "level");
            iconList.Images.SetKeyName(35, "world");
            // 
            // panel4
            // 
            panel4.Controls.Add(filterClear);
            panel4.Controls.Add(label1);
            panel4.Controls.Add(filterBox);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(390, 32);
            panel4.TabIndex = 2;
            // 
            // filterClear
            // 
            filterClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            filterClear.BackColor = SystemColors.GradientActiveCaption;
            filterClear.FlatAppearance.BorderSize = 0;
            filterClear.FlatStyle = FlatStyle.Flat;
            filterClear.Font = new Font("Segoe UI", 9F);
            filterClear.ForeColor = Color.Black;
            filterClear.ImageAlign = ContentAlignment.MiddleLeft;
            filterClear.Location = new Point(360, 4);
            filterClear.Name = "filterClear";
            filterClear.Size = new Size(24, 23);
            filterClear.TabIndex = 2;
            filterClear.Text = "X";
            filterClear.UseVisualStyleBackColor = false;
            filterClear.Click += filterClear_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 7);
            label1.Name = "label1";
            label1.Size = new Size(33, 15);
            label1.TabIndex = 1;
            label1.Text = "Filter";
            // 
            // filterBox
            // 
            filterBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filterBox.Location = new Point(42, 4);
            filterBox.Name = "filterBox";
            filterBox.Size = new Size(312, 23);
            filterBox.TabIndex = 0;
            filterBox.KeyDown += filterBox_KeyDown;
            // 
            // propertyFilePage
            // 
            propertyFilePage.Controls.Add(propertyGrid);
            propertyFilePage.Location = new Point(4, 24);
            propertyFilePage.Name = "propertyFilePage";
            propertyFilePage.Padding = new Padding(3);
            propertyFilePage.Size = new Size(396, 530);
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
            propertyGrid.Size = new Size(390, 524);
            propertyGrid.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(propertyPage);
            tabControl1.Controls.Add(namePage);
            tabControl1.Controls.Add(importPage);
            tabControl1.Controls.Add(exportPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(810, 558);
            tabControl1.TabIndex = 0;
            // 
            // propertyPage
            // 
            propertyPage.Controls.Add(propertiesView);
            propertyPage.Location = new Point(4, 24);
            propertyPage.Name = "propertyPage";
            propertyPage.Padding = new Padding(3);
            propertyPage.Size = new Size(802, 530);
            propertyPage.TabIndex = 0;
            propertyPage.Text = "Object Properties";
            propertyPage.UseVisualStyleBackColor = true;
            // 
            // propertiesView
            // 
            propertiesView.Dock = DockStyle.Fill;
            propertiesView.Location = new Point(3, 3);
            propertiesView.Name = "propertiesView";
            propertiesView.Size = new Size(796, 524);
            propertiesView.TabIndex = 0;
            // 
            // namePage
            // 
            namePage.Controls.Add(nameGridView);
            namePage.Location = new Point(4, 24);
            namePage.Name = "namePage";
            namePage.Size = new Size(802, 530);
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
            nameGridView.Size = new Size(802, 530);
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
            nameTableFlags.Width = 150;
            // 
            // importPage
            // 
            importPage.Controls.Add(importGridView);
            importPage.Location = new Point(4, 24);
            importPage.Name = "importPage";
            importPage.Size = new Size(802, 530);
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
            importGridView.Columns.AddRange(new DataGridViewColumn[] { importIndex, importObject, importClass, importOuter, importPakage });
            importGridView.DefaultCellStyle = dataGridViewCellStyle2;
            importGridView.Dock = DockStyle.Fill;
            importGridView.EnableHeadersVisualStyles = false;
            importGridView.GridColor = SystemColors.GradientActiveCaption;
            importGridView.Location = new Point(0, 0);
            importGridView.Name = "importGridView";
            importGridView.ReadOnly = true;
            importGridView.RowHeadersVisible = false;
            importGridView.Size = new Size(802, 530);
            importGridView.TabIndex = 0;
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
            // importOuter
            // 
            importOuter.DataPropertyName = "Outer";
            importOuter.HeaderText = "Outer";
            importOuter.Name = "importOuter";
            importOuter.ReadOnly = true;
            importOuter.Width = 150;
            // 
            // importPakage
            // 
            importPakage.DataPropertyName = "Package";
            importPakage.HeaderText = "Package";
            importPakage.Name = "importPakage";
            importPakage.ReadOnly = true;
            importPakage.Width = 80;
            // 
            // exportPage
            // 
            exportPage.Controls.Add(exportGridView);
            exportPage.Location = new Point(4, 24);
            exportPage.Name = "exportPage";
            exportPage.Size = new Size(802, 530);
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
            exportGridView.Columns.AddRange(new DataGridViewColumn[] { IndexColumn1, exportColumn1, exportColumn2, exportOuter, exportColumn4, exportColumn5, buttonColumn });
            exportGridView.DefaultCellStyle = dataGridViewCellStyle2;
            exportGridView.Dock = DockStyle.Fill;
            exportGridView.EnableHeadersVisualStyles = false;
            exportGridView.GridColor = SystemColors.GradientActiveCaption;
            exportGridView.Location = new Point(0, 0);
            exportGridView.Name = "exportGridView";
            exportGridView.RowHeadersVisible = false;
            exportGridView.Size = new Size(802, 530);
            exportGridView.TabIndex = 1;
            exportGridView.CellContentClick += exportGridView_CellContentClick;
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
            exportColumn1.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            exportColumn1.DataPropertyName = "Object";
            exportColumn1.HeaderText = "Object";
            exportColumn1.Name = "exportColumn1";
            // 
            // exportColumn2
            // 
            exportColumn2.DataPropertyName = "Class";
            exportColumn2.HeaderText = "Super :: Class";
            exportColumn2.Name = "exportColumn2";
            exportColumn2.Width = 150;
            // 
            // exportOuter
            // 
            exportOuter.DataPropertyName = "Outer";
            exportOuter.HeaderText = "Outer";
            exportOuter.Name = "exportOuter";
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
            exportColumn5.DataPropertyName = "SerialSize";
            exportColumn5.HeaderText = "Size";
            exportColumn5.Name = "exportColumn5";
            exportColumn5.Width = 70;
            // 
            // buttonColumn
            // 
            buttonColumn.DataPropertyName = "Details";
            buttonColumn.HeaderText = "Details";
            buttonColumn.Name = "buttonColumn";
            buttonColumn.Text = "...";
            buttonColumn.UseColumnTextForButtonValue = true;
            buttonColumn.Width = 50;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1218, 604);
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
            objectsPage.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            propertyFilePage.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            propertyPage.ResumeLayout(false);
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
        private TreeView objectsTree;
        private TabPage exportPage;
        private DataGridView importGridView;
        private DataGridView exportGridView;
        private ToolStripMenuItem openFileMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private TabPage namePage;
        private DataGridView nameGridView;
        private PropertyGrid propertyGrid;
        private ToolStripStatusLabel progressStatus;
        private TabControl tabControl2;
        private TabPage objectsPage;
        private TabPage propertyFilePage;
        private DataGridViewTextBoxColumn importIndex;
        private DataGridViewTextBoxColumn importObject;
        private DataGridViewTextBoxColumn importClass;
        private DataGridViewTextBoxColumn importOuter;
        private DataGridViewTextBoxColumn importPakage;
        private DataGridViewTextBoxColumn IndexColumn1;
        private DataGridViewTextBoxColumn exportColumn1;
        private DataGridViewTextBoxColumn exportColumn2;
        private DataGridViewTextBoxColumn exportOuter;
        private DataGridViewTextBoxColumn exportColumn4;
        private DataGridViewTextBoxColumn exportColumn5;
        private DataGridViewButtonColumn buttonColumn;
        private DataGridViewTextBoxColumn nameTableIndex;
        private DataGridViewTextBoxColumn nameTableName;
        private DataGridViewTextBoxColumn nameTableFlags;
        private Panel panel4;
        private Button filterClear;
        private Label label1;
        private TextBox filterBox;
        private ImageList iconList;
        private TreeView propertiesView;
    }
}
