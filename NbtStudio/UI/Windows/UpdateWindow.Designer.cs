namespace NbtStudio.UI
{
    partial class UpdateWindow
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
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ChangelogBox = new System.Windows.Forms.TextBox();
            this.CurrentVersionLabel = new System.Windows.Forms.Label();
            this.AvailableVersionLabel = new System.Windows.Forms.Label();
            this.ChangelogLabel = new System.Windows.Forms.Label();
            this.CurrentVersionValue = new System.Windows.Forms.Label();
            this.AvailableVersionValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonCancel.Location = new System.Drawing.Point(410, 304);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 7;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonOk.Location = new System.Drawing.Point(329, 304);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 6;
            this.ButtonOk.Text = "Update";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ChangelogBox
            // 
            this.ChangelogBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChangelogBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ChangelogBox.Location = new System.Drawing.Point(12, 71);
            this.ChangelogBox.Multiline = true;
            this.ChangelogBox.Name = "ChangelogBox";
            this.ChangelogBox.ReadOnly = true;
            this.ChangelogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ChangelogBox.Size = new System.Drawing.Size(473, 227);
            this.ChangelogBox.TabIndex = 5;
            // 
            // CurrentVersionLabel
            // 
            this.CurrentVersionLabel.AutoSize = true;
            this.CurrentVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.CurrentVersionLabel.Location = new System.Drawing.Point(12, 9);
            this.CurrentVersionLabel.Name = "CurrentVersionLabel";
            this.CurrentVersionLabel.Size = new System.Drawing.Size(111, 17);
            this.CurrentVersionLabel.TabIndex = 0;
            this.CurrentVersionLabel.Text = "Current Version:";
            // 
            // AvailableVersionLabel
            // 
            this.AvailableVersionLabel.AutoSize = true;
            this.AvailableVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.AvailableVersionLabel.Location = new System.Drawing.Point(187, 9);
            this.AvailableVersionLabel.Name = "AvailableVersionLabel";
            this.AvailableVersionLabel.Size = new System.Drawing.Size(121, 17);
            this.AvailableVersionLabel.TabIndex = 2;
            this.AvailableVersionLabel.Text = "Available Version:";
            // 
            // ChangelogLabel
            // 
            this.ChangelogLabel.AutoSize = true;
            this.ChangelogLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ChangelogLabel.Location = new System.Drawing.Point(12, 51);
            this.ChangelogLabel.Name = "ChangelogLabel";
            this.ChangelogLabel.Size = new System.Drawing.Size(76, 17);
            this.ChangelogLabel.TabIndex = 4;
            this.ChangelogLabel.Text = "Changelog";
            // 
            // CurrentVersionValue
            // 
            this.CurrentVersionValue.AutoSize = true;
            this.CurrentVersionValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentVersionValue.Location = new System.Drawing.Point(119, 9);
            this.CurrentVersionValue.Name = "CurrentVersionValue";
            this.CurrentVersionValue.Size = new System.Drawing.Size(0, 20);
            this.CurrentVersionValue.TabIndex = 1;
            // 
            // AvailableVersionValue
            // 
            this.AvailableVersionValue.AutoSize = true;
            this.AvailableVersionValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AvailableVersionValue.Location = new System.Drawing.Point(304, 9);
            this.AvailableVersionValue.Name = "AvailableVersionValue";
            this.AvailableVersionValue.Size = new System.Drawing.Size(0, 20);
            this.AvailableVersionValue.TabIndex = 3;
            // 
            // UpdateWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(498, 339);
            this.Controls.Add(this.AvailableVersionValue);
            this.Controls.Add(this.CurrentVersionValue);
            this.Controls.Add(this.ChangelogLabel);
            this.Controls.Add(this.AvailableVersionLabel);
            this.Controls.Add(this.CurrentVersionLabel);
            this.Controls.Add(this.ChangelogBox);
            this.Controls.Add(this.ButtonOk);
            this.Controls.Add(this.ButtonCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update NBT Studio";
            this.Load += new System.EventHandler(this.UpdateWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.TextBox ChangelogBox;
        private System.Windows.Forms.Label CurrentVersionLabel;
        private System.Windows.Forms.Label AvailableVersionLabel;
        private System.Windows.Forms.Label ChangelogLabel;
        private System.Windows.Forms.Label CurrentVersionValue;
        private System.Windows.Forms.Label AvailableVersionValue;
    }
}