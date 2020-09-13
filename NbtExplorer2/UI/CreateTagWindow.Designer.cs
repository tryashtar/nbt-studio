namespace NbtExplorer2.UI
{
    partial class CreateTagWindow
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
            this.SizeBox = new System.Windows.Forms.TextBox();
            this.SizeLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.CancellingButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.ValueBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SizeBox
            // 
            this.SizeBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.SizeBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.SizeBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.SizeBox.Location = new System.Drawing.Point(70, 33);
            this.SizeBox.Name = "SizeBox";
            this.SizeBox.Size = new System.Drawing.Size(71, 21);
            this.SizeBox.TabIndex = 1;
            // 
            // SizeLabel
            // 
            this.SizeLabel.AutoSize = true;
            this.SizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.SizeLabel.Location = new System.Drawing.Point(12, 36);
            this.SizeLabel.Name = "SizeLabel";
            this.SizeLabel.Size = new System.Drawing.Size(34, 15);
            this.SizeLabel.TabIndex = 6;
            this.SizeLabel.Text = "Size:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NameLabel.Location = new System.Drawing.Point(12, 9);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(44, 15);
            this.NameLabel.TabIndex = 5;
            this.NameLabel.Text = "Name:";
            // 
            // NameBox
            // 
            this.NameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.NameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.NameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NameBox.Location = new System.Drawing.Point(70, 6);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(209, 21);
            this.NameBox.TabIndex = 0;
            // 
            // CancellingButton
            // 
            this.CancellingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancellingButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancellingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.CancellingButton.Location = new System.Drawing.Point(123, 65);
            this.CancellingButton.Name = "CancellingButton";
            this.CancellingButton.Size = new System.Drawing.Size(75, 23);
            this.CancellingButton.TabIndex = 2;
            this.CancellingButton.Text = "Cancel";
            this.CancellingButton.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.OkButton.Location = new System.Drawing.Point(204, 65);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 3;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // ValueLabel
            // 
            this.ValueLabel.AutoSize = true;
            this.ValueLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ValueLabel.Location = new System.Drawing.Point(12, 36);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(41, 15);
            this.ValueLabel.TabIndex = 7;
            this.ValueLabel.Text = "Value:";
            // 
            // ValueBox
            // 
            this.ValueBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ValueBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ValueBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ValueBox.Location = new System.Drawing.Point(70, 33);
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(209, 21);
            this.ValueBox.TabIndex = 1;
            // 
            // CreateTagWindow
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancellingButton;
            this.ClientSize = new System.Drawing.Size(291, 100);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.CancellingButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.SizeBox);
            this.Controls.Add(this.SizeLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.NameBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateTagWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Tag";
            this.Load += new System.EventHandler(this.CreateTagWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SizeBox;
        private System.Windows.Forms.Label SizeLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Button CancellingButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.TextBox ValueBox;
    }
}