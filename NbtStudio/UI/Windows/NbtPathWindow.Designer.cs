namespace NbtStudio.UI
{
    partial class NbtPathWindow
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
            this.PathBox = new NbtStudio.UI.NbtPathTextBox();
            this.ButtonSelect = new System.Windows.Forms.Button();
            this.FoundResultsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PathBox
            // 
            this.PathBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.PathBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.PathBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PathBox.Location = new System.Drawing.Point(16, 22);
            this.PathBox.Margin = new System.Windows.Forms.Padding(6, 12, 12, 0);
            this.PathBox.Name = "PathBox";
            this.PathBox.Size = new System.Drawing.Size(303, 21);
            this.PathBox.TabIndex = 0;
            // 
            // ButtonSelect
            // 
            this.ButtonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ButtonSelect.Location = new System.Drawing.Point(231, 60);
            this.ButtonSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonSelect.Name = "ButtonSelect";
            this.ButtonSelect.Size = new System.Drawing.Size(88, 27);
            this.ButtonSelect.TabIndex = 1;
            this.ButtonSelect.Text = "Select";
            this.ButtonSelect.UseVisualStyleBackColor = true;
            this.ButtonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // FoundResultsLabel
            // 
            this.FoundResultsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FoundResultsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FoundResultsLabel.ForeColor = System.Drawing.Color.Gray;
            this.FoundResultsLabel.Location = new System.Drawing.Point(39, 67);
            this.FoundResultsLabel.Margin = new System.Windows.Forms.Padding(12, 12, 6, 0);
            this.FoundResultsLabel.Name = "FoundResultsLabel";
            this.FoundResultsLabel.Size = new System.Drawing.Size(182, 20);
            this.FoundResultsLabel.TabIndex = 6;
            this.FoundResultsLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.FoundResultsLabel.Visible = false;
            // 
            // NbtPathWindow
            // 
            this.AcceptButton = this.ButtonSelect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(335, 99);
            this.Controls.Add(this.FoundResultsLabel);
            this.Controls.Add(this.ButtonSelect);
            this.Controls.Add(this.PathBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NbtPathWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select by NBT Path";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NbtPathWindow_FormClosed);
            this.Load += new System.EventHandler(this.NbtPathWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NbtPathTextBox PathBox;
        private System.Windows.Forms.Button ButtonSelect;
        private System.Windows.Forms.Label FoundResultsLabel;
    }
}