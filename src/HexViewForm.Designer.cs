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
            SuspendLayout();
            // 
            // hexBox
            // 
            hexBox.Dock = DockStyle.Fill;
            hexBox.Font = new Font("Segoe UI", 9F);
            hexBox.Location = new Point(0, 0);
            hexBox.Name = "hexBox";
            hexBox.ShadowSelectionColor = Color.FromArgb(100, 60, 188, 255);
            hexBox.Size = new Size(800, 450);
            hexBox.TabIndex = 0;
            // 
            // HexViewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(hexBox);
            Name = "HexViewForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Hex Viewer";
            ResumeLayout(false);
        }

        #endregion

        private Be.Windows.Forms.HexBox hexBox;
    }
}