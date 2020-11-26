namespace NbtStudio.UI
{
    partial class ExportWindow
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
            this.components = new System.ComponentModel.Container();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.RadioSnbt = new System.Windows.Forms.RadioButton();
            this.RadioNbt = new System.Windows.Forms.RadioButton();
            this.CheckMinify = new System.Windows.Forms.CheckBox();
            this.CheckLittleEndian = new System.Windows.Forms.CheckBox();
            this.CompressionBox = new System.Windows.Forms.ComboBox();
            this.CheckJson = new System.Windows.Forms.CheckBox();
            this.CheckBedrockHeader = new System.Windows.Forms.CheckBox();
            this.Tooltips = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(103, 147);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 8;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Location = new System.Drawing.Point(22, 147);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 7;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // RadioSnbt
            // 
            this.RadioSnbt.AutoSize = true;
            this.RadioSnbt.Location = new System.Drawing.Point(12, 89);
            this.RadioSnbt.Name = "RadioSnbt";
            this.RadioSnbt.Size = new System.Drawing.Size(54, 17);
            this.RadioSnbt.TabIndex = 4;
            this.RadioSnbt.Text = "SNBT";
            this.RadioSnbt.UseVisualStyleBackColor = true;
            // 
            // RadioNbt
            // 
            this.RadioNbt.AutoSize = true;
            this.RadioNbt.Location = new System.Drawing.Point(12, 12);
            this.RadioNbt.Name = "RadioNbt";
            this.RadioNbt.Size = new System.Drawing.Size(47, 17);
            this.RadioNbt.TabIndex = 0;
            this.RadioNbt.Text = "NBT";
            this.RadioNbt.UseVisualStyleBackColor = true;
            this.RadioNbt.CheckedChanged += new System.EventHandler(this.RadioNbt_CheckedChanged);
            // 
            // CheckMinify
            // 
            this.CheckMinify.AutoSize = true;
            this.CheckMinify.Location = new System.Drawing.Point(75, 90);
            this.CheckMinify.Name = "CheckMinify";
            this.CheckMinify.Size = new System.Drawing.Size(53, 17);
            this.CheckMinify.TabIndex = 5;
            this.CheckMinify.Text = "Minify";
            this.CheckMinify.UseVisualStyleBackColor = true;
            // 
            // CheckLittleEndian
            // 
            this.CheckLittleEndian.AutoSize = true;
            this.CheckLittleEndian.Location = new System.Drawing.Point(75, 38);
            this.CheckLittleEndian.Name = "CheckLittleEndian";
            this.CheckLittleEndian.Size = new System.Drawing.Size(84, 17);
            this.CheckLittleEndian.TabIndex = 2;
            this.CheckLittleEndian.Text = "Little-Endian";
            this.CheckLittleEndian.UseVisualStyleBackColor = true;
            // 
            // CompressionBox
            // 
            this.CompressionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionBox.FormattingEnabled = true;
            this.CompressionBox.Location = new System.Drawing.Point(75, 11);
            this.CompressionBox.Name = "CompressionBox";
            this.CompressionBox.Size = new System.Drawing.Size(103, 21);
            this.CompressionBox.TabIndex = 1;
            // 
            // CheckJson
            // 
            this.CheckJson.AutoSize = true;
            this.CheckJson.Location = new System.Drawing.Point(75, 113);
            this.CheckJson.Name = "CheckJson";
            this.CheckJson.Size = new System.Drawing.Size(54, 17);
            this.CheckJson.TabIndex = 6;
            this.CheckJson.Text = "JSON";
            this.CheckJson.UseVisualStyleBackColor = true;
            // 
            // CheckBedrockHeader
            // 
            this.CheckBedrockHeader.AutoSize = true;
            this.CheckBedrockHeader.Location = new System.Drawing.Point(75, 61);
            this.CheckBedrockHeader.Name = "CheckBedrockHeader";
            this.CheckBedrockHeader.Size = new System.Drawing.Size(104, 17);
            this.CheckBedrockHeader.TabIndex = 3;
            this.CheckBedrockHeader.Text = "Bedrock Header";
            this.CheckBedrockHeader.UseVisualStyleBackColor = true;
            // 
            // ExportWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(190, 182);
            this.Controls.Add(this.CheckBedrockHeader);
            this.Controls.Add(this.CheckJson);
            this.Controls.Add(this.CompressionBox);
            this.Controls.Add(this.CheckLittleEndian);
            this.Controls.Add(this.CheckMinify);
            this.Controls.Add(this.RadioNbt);
            this.Controls.Add(this.RadioSnbt);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.RadioButton RadioSnbt;
        private System.Windows.Forms.RadioButton RadioNbt;
        private System.Windows.Forms.CheckBox CheckMinify;
        private System.Windows.Forms.CheckBox CheckLittleEndian;
        private System.Windows.Forms.ComboBox CompressionBox;
        private System.Windows.Forms.CheckBox CheckJson;
        private System.Windows.Forms.CheckBox CheckBedrockHeader;
        private System.Windows.Forms.ToolTip Tooltips;
    }
}