namespace NbtStudio.UI
{
    partial class EditChunkWindow
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
            this.XLabel = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ZLabel = new System.Windows.Forms.Label();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ZBox = new NbtStudio.UI.ChunkCoordEditControl();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.XBox = new NbtStudio.UI.ChunkCoordEditControl();
            this.MainTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZBox)).BeginInit();
            this.ButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.XBox)).BeginInit();
            this.SuspendLayout();
            // 
            // XLabel
            // 
            this.XLabel.AutoSize = true;
            this.XLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.XLabel.Location = new System.Drawing.Point(10, 10);
            this.XLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.XLabel.Name = "XLabel";
            this.XLabel.Size = new System.Drawing.Size(29, 24);
            this.XLabel.TabIndex = 1;
            this.XLabel.Text = "X:";
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonCancel.Location = new System.Drawing.Point(93, 9);
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
            this.ButtonOk.Location = new System.Drawing.Point(12, 9);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 6;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ZLabel
            // 
            this.ZLabel.AutoSize = true;
            this.ZLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.ZLabel.Location = new System.Drawing.Point(10, 49);
            this.ZLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.ZLabel.Name = "ZLabel";
            this.ZLabel.Size = new System.Drawing.Size(27, 24);
            this.ZLabel.TabIndex = 3;
            this.ZLabel.Text = "Z:";
            // 
            // MainTable
            // 
            this.MainTable.AutoSize = true;
            this.MainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainTable.ColumnCount = 2;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.Controls.Add(this.ZBox, 1, 1);
            this.MainTable.Controls.Add(this.XLabel, 0, 0);
            this.MainTable.Controls.Add(this.ZLabel, 0, 1);
            this.MainTable.Controls.Add(this.ButtonsPanel, 1, 2);
            this.MainTable.Controls.Add(this.XBox, 1, 0);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 3;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTable.Size = new System.Drawing.Size(463, 185);
            this.MainTable.TabIndex = 0;
            // 
            // ZBox
            // 
            this.ZBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.ZBox.Location = new System.Drawing.Point(49, 49);
            this.ZBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.ZBox.Name = "ZBox";
            this.ZBox.Size = new System.Drawing.Size(127, 29);
            this.ZBox.TabIndex = 4;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Location = new System.Drawing.Point(283, 141);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(177, 41);
            this.ButtonsPanel.TabIndex = 5;
            // 
            // XBox
            // 
            this.XBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.XBox.Location = new System.Drawing.Point(49, 10);
            this.XBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.XBox.Name = "XBox";
            this.XBox.Size = new System.Drawing.Size(127, 29);
            this.XBox.TabIndex = 2;
            // 
            // EditChunkWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(463, 185);
            this.Controls.Add(this.MainTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditChunkWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Chunk";
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ZBox)).EndInit();
            this.ButtonsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.XBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label XLabel;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label ZLabel;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.Panel ButtonsPanel;
        private ChunkCoordEditControl ZBox;
        private ChunkCoordEditControl XBox;
    }
}