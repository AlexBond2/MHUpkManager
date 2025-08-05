namespace MHUpkManager
{
    partial class ModelViewForm
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
            sceneControl = new SharpGL.SceneControl();
            menuStrip1 = new MenuStrip();
            modelToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            showNormalsToolStripMenuItem = new ToolStripMenuItem();
            showBonesToolStripMenuItem = new ToolStripMenuItem();
            showBoneNameToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            showTexturesToolStripMenuItem = new ToolStripMenuItem();
            showGridToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)sceneControl).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // sceneControl
            // 
            sceneControl.Dock = DockStyle.Fill;
            sceneControl.DrawFPS = false;
            sceneControl.Location = new Point(0, 24);
            sceneControl.Margin = new Padding(4, 3, 4, 3);
            sceneControl.Name = "sceneControl";
            sceneControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            sceneControl.RenderContextType = SharpGL.RenderContextType.NativeWindow;
            sceneControl.RenderTrigger = SharpGL.RenderTrigger.Manual;
            sceneControl.Size = new Size(800, 426);
            sceneControl.TabIndex = 0;
            sceneControl.OpenGLInitialized += sceneControl_OpenGLInitialized;
            sceneControl.OpenGLDraw += sceneControl_OpenGLDraw;
            sceneControl.KeyDown += sceneControl_KeyDown;
            sceneControl.MouseDown += sceneControl_MouseDown;
            sceneControl.MouseMove += sceneControl_MouseMove;
            sceneControl.MouseUp += sceneControl_MouseUp;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { modelToolStripMenuItem, viewToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // modelToolStripMenuItem
            // 
            modelToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveAsToolStripMenuItem });
            modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            modelToolStripMenuItem.Size = new Size(53, 20);
            modelToolStripMenuItem.Text = "Model";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(180, 22);
            saveAsToolStripMenuItem.Text = "Save as...";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showNormalsToolStripMenuItem, showBonesToolStripMenuItem, showBoneNameToolStripMenuItem, toolStripMenuItem1, showTexturesToolStripMenuItem, showGridToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // showNormalsToolStripMenuItem
            // 
            showNormalsToolStripMenuItem.Name = "showNormalsToolStripMenuItem";
            showNormalsToolStripMenuItem.ShortcutKeyDisplayString = "N";
            showNormalsToolStripMenuItem.Size = new Size(180, 22);
            showNormalsToolStripMenuItem.Text = "Show Normals";
            showNormalsToolStripMenuItem.Click += showNormalsToolStripMenuItem_Click;
            // 
            // showBonesToolStripMenuItem
            // 
            showBonesToolStripMenuItem.Name = "showBonesToolStripMenuItem";
            showBonesToolStripMenuItem.ShortcutKeyDisplayString = "B";
            showBonesToolStripMenuItem.Size = new Size(180, 22);
            showBonesToolStripMenuItem.Text = "Show Bones";
            showBonesToolStripMenuItem.Click += showBonesToolStripMenuItem_Click;
            // 
            // showBoneNameToolStripMenuItem
            // 
            showBoneNameToolStripMenuItem.Name = "showBoneNameToolStripMenuItem";
            showBoneNameToolStripMenuItem.Size = new Size(180, 22);
            showBoneNameToolStripMenuItem.Text = "Show Bone Names";
            showBoneNameToolStripMenuItem.Click += showBoneNameToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(177, 6);
            // 
            // showTexturesToolStripMenuItem
            // 
            showTexturesToolStripMenuItem.Checked = true;
            showTexturesToolStripMenuItem.CheckState = CheckState.Checked;
            showTexturesToolStripMenuItem.Name = "showTexturesToolStripMenuItem";
            showTexturesToolStripMenuItem.ShortcutKeyDisplayString = "T";
            showTexturesToolStripMenuItem.Size = new Size(180, 22);
            showTexturesToolStripMenuItem.Text = "Show Textures";
            showTexturesToolStripMenuItem.Click += showTexturesToolStripMenuItem_Click;
            // 
            // showGridToolStripMenuItem
            // 
            showGridToolStripMenuItem.Checked = true;
            showGridToolStripMenuItem.CheckState = CheckState.Checked;
            showGridToolStripMenuItem.Name = "showGridToolStripMenuItem";
            showGridToolStripMenuItem.ShortcutKeyDisplayString = "G";
            showGridToolStripMenuItem.Size = new Size(180, 22);
            showGridToolStripMenuItem.Text = "Show Grid";
            showGridToolStripMenuItem.Click += showGridToolStripMenuItem_Click;
            // 
            // ModelViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(sceneControl);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ModelViewForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Model Viewer";
            ((System.ComponentModel.ISupportInitialize)sceneControl).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SharpGL.SceneControl sceneControl;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem modelToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem showNormalsToolStripMenuItem;
        private ToolStripMenuItem showBonesToolStripMenuItem;
        private ToolStripMenuItem showBoneNameToolStripMenuItem;
        private ToolStripMenuItem showTexturesToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem showGridToolStripMenuItem;
    }
}