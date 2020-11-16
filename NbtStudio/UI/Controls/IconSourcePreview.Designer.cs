namespace NbtStudio
{
    partial class IconSourcePreview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IconsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // IconsPanel
            // 
            this.IconsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IconsPanel.Location = new System.Drawing.Point(0, 0);
            this.IconsPanel.Name = "IconsPanel";
            this.IconsPanel.Size = new System.Drawing.Size(299, 46);
            this.IconsPanel.TabIndex = 0;
            // 
            // IconSourcePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.IconsPanel);
            this.Name = "IconSourcePreview";
            this.Size = new System.Drawing.Size(299, 46);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel IconsPanel;
    }
}
