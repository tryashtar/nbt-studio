namespace NbtStudio.UI
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
            this.PreviewButton = new System.Windows.Forms.Button();
            this.IconsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // IconsPanel
            // 
            this.IconsPanel.AutoSize = true;
            this.IconsPanel.Controls.Add(this.PreviewButton);
            this.IconsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IconsPanel.Location = new System.Drawing.Point(0, 0);
            this.IconsPanel.Name = "IconsPanel";
            this.IconsPanel.Size = new System.Drawing.Size(299, 46);
            this.IconsPanel.TabIndex = 0;
            // 
            // PreviewButton
            // 
            this.PreviewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.PreviewButton.Location = new System.Drawing.Point(3, 3);
            this.PreviewButton.Name = "PreviewButton";
            this.PreviewButton.Size = new System.Drawing.Size(40, 40);
            this.PreviewButton.TabIndex = 1;
            this.PreviewButton.Text = "👁️";
            this.PreviewButton.UseVisualStyleBackColor = true;
            this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
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
            this.IconsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel IconsPanel;
        private System.Windows.Forms.Button PreviewButton;
    }
}
