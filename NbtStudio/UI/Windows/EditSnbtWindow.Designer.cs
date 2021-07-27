namespace NbtStudio.UI
{
    partial class EditSnbtWindow
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
            this.MinifyCheck = new System.Windows.Forms.CheckBox();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.WordWrapCheck = new System.Windows.Forms.CheckBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.NameBox = new NbtStudio.UI.TagNameTextBox();
            this.InputBox = new NbtStudio.UI.TagSnbtTextBox();
            this.ButtonsPanel.SuspendLayout();
            this.MainTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(231, 10);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(88, 27);
            this.ButtonCancel.TabIndex = 7;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Location = new System.Drawing.Point(136, 10);
            this.ButtonOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(88, 27);
            this.ButtonOk.TabIndex = 6;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // MinifyCheck
            // 
            this.MinifyCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MinifyCheck.AutoSize = true;
            this.MinifyCheck.Location = new System.Drawing.Point(12, 4);
            this.MinifyCheck.Margin = new System.Windows.Forms.Padding(12);
            this.MinifyCheck.Name = "MinifyCheck";
            this.MinifyCheck.Size = new System.Drawing.Size(60, 19);
            this.MinifyCheck.TabIndex = 5;
            this.MinifyCheck.Text = "Minify";
            this.MinifyCheck.UseVisualStyleBackColor = true;
            this.MinifyCheck.Visible = false;
            this.MinifyCheck.CheckedChanged += new System.EventHandler(this.MinifyCheck_CheckedChanged);
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonsPanel.Controls.Add(this.WordWrapCheck);
            this.ButtonsPanel.Controls.Add(this.MinifyCheck);
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 91);
            this.ButtonsPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(331, 47);
            this.ButtonsPanel.TabIndex = 4;
            // 
            // WordWrapCheck
            // 
            this.WordWrapCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WordWrapCheck.AutoSize = true;
            this.WordWrapCheck.Location = new System.Drawing.Point(12, 25);
            this.WordWrapCheck.Margin = new System.Windows.Forms.Padding(12);
            this.WordWrapCheck.Name = "WordWrapCheck";
            this.WordWrapCheck.Size = new System.Drawing.Size(86, 19);
            this.WordWrapCheck.TabIndex = 8;
            this.WordWrapCheck.Text = "Word Wrap";
            this.WordWrapCheck.UseVisualStyleBackColor = true;
            this.WordWrapCheck.Visible = false;
            this.WordWrapCheck.CheckedChanged += new System.EventHandler(this.WordWrapCheck_CheckedChanged);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NameLabel.Location = new System.Drawing.Point(12, 12);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(12, 12, 6, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(44, 15);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Name:";
            // 
            // MainTable
            // 
            this.MainTable.ColumnCount = 2;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.Controls.Add(this.NameBox, 1, 0);
            this.MainTable.Controls.Add(this.InputBox, 0, 1);
            this.MainTable.Controls.Add(this.NameLabel, 0, 0);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 2;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.MainTable.Size = new System.Drawing.Size(331, 91);
            this.MainTable.TabIndex = 0;
            // 
            // NameBox
            // 
            this.NameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.NameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.NameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NameBox.Location = new System.Drawing.Point(68, 12);
            this.NameBox.Margin = new System.Windows.Forms.Padding(6, 12, 12, 0);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(243, 21);
            this.NameBox.TabIndex = 2;
            // 
            // InputBox
            // 
            this.InputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.InputBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.MainTable.SetColumnSpan(this.InputBox, 2);
            this.InputBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.InputBox.Location = new System.Drawing.Point(12, 45);
            this.InputBox.Margin = new System.Windows.Forms.Padding(12);
            this.InputBox.MaxLength = 2147483647;
            this.InputBox.Name = "InputBox";
            this.InputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InputBox.Size = new System.Drawing.Size(307, 26);
            this.InputBox.TabIndex = 3;
            this.InputBox.WordWrap = false;
            // 
            // EditSnbtWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(331, 138);
            this.Controls.Add(this.MainTable);
            this.Controls.Add(this.ButtonsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(347, 109);
            this.Name = "EditSnbtWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit SNBT...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditSnbtWindow_FormClosed);
            this.Load += new System.EventHandler(this.EditSnbtWindow_Load);
            this.ButtonsPanel.ResumeLayout(false);
            this.ButtonsPanel.PerformLayout();
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.CheckBox MinifyCheck;
        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.Label NameLabel;
        private TagNameTextBox NameBox;
        private TagSnbtTextBox InputBox;
        private System.Windows.Forms.CheckBox WordWrapCheck;
    }
}