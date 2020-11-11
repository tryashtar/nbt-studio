namespace NbtStudio.UI
{
    partial class BulkEditWindow
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
            this.FindLabel = new System.Windows.Forms.Label();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ReplaceLabel = new System.Windows.Forms.Label();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ActionList = new System.Windows.Forms.ListView();
            this.CurrentColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NewColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RegexCheck = new System.Windows.Forms.CheckBox();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.FindBox = new NbtStudio.UI.RegexTextBox();
            this.ReplaceBox = new System.Windows.Forms.TextBox();
            this.MainTable.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FindLabel
            // 
            this.FindLabel.AutoSize = true;
            this.FindLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FindLabel.Location = new System.Drawing.Point(10, 10);
            this.FindLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.FindLabel.Name = "FindLabel";
            this.FindLabel.Size = new System.Drawing.Size(34, 15);
            this.FindLabel.TabIndex = 0;
            this.FindLabel.Text = "Find:";
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
            // ReplaceLabel
            // 
            this.ReplaceLabel.AutoSize = true;
            this.ReplaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ReplaceLabel.Location = new System.Drawing.Point(10, 41);
            this.ReplaceLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.ReplaceLabel.Name = "ReplaceLabel";
            this.ReplaceLabel.Size = new System.Drawing.Size(56, 15);
            this.ReplaceLabel.TabIndex = 4;
            this.ReplaceLabel.Text = "Replace:";
            // 
            // MainTable
            // 
            this.MainTable.AutoSize = true;
            this.MainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainTable.ColumnCount = 2;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.Controls.Add(this.ActionList, 1, 2);
            this.MainTable.Controls.Add(this.RegexCheck, 0, 3);
            this.MainTable.Controls.Add(this.FindLabel, 0, 0);
            this.MainTable.Controls.Add(this.FindBox, 1, 0);
            this.MainTable.Controls.Add(this.ReplaceLabel, 0, 1);
            this.MainTable.Controls.Add(this.ReplaceBox, 1, 1);
            this.MainTable.Controls.Add(this.ButtonsPanel, 1, 3);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 4;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTable.Size = new System.Drawing.Size(463, 341);
            this.MainTable.TabIndex = 8;
            // 
            // ActionList
            // 
            this.ActionList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ActionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CurrentColumn,
            this.NewColumn});
            this.ActionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActionList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ActionList.GridLines = true;
            this.ActionList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ActionList.HideSelection = false;
            this.ActionList.Location = new System.Drawing.Point(77, 72);
            this.ActionList.Margin = new System.Windows.Forms.Padding(5, 10, 10, 10);
            this.ActionList.MultiSelect = false;
            this.ActionList.Name = "ActionList";
            this.ActionList.Size = new System.Drawing.Size(376, 180);
            this.ActionList.TabIndex = 10;
            this.ActionList.UseCompatibleStateImageBehavior = false;
            this.ActionList.View = System.Windows.Forms.View.Details;
            this.ActionList.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.ActionList_ColumnWidthChanging);
            this.ActionList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ActionList_ItemSelectionChanged);
            // 
            // CurrentColumn
            // 
            this.CurrentColumn.Text = "Old Name";
            this.CurrentColumn.Width = 50;
            // 
            // NewColumn
            // 
            this.NewColumn.Text = "New Name";
            this.NewColumn.Width = 50;
            // 
            // RegexCheck
            // 
            this.RegexCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RegexCheck.AutoSize = true;
            this.RegexCheck.Location = new System.Drawing.Point(10, 314);
            this.RegexCheck.Margin = new System.Windows.Forms.Padding(10, 0, 5, 10);
            this.RegexCheck.Name = "RegexCheck";
            this.RegexCheck.Size = new System.Drawing.Size(57, 17);
            this.RegexCheck.TabIndex = 9;
            this.RegexCheck.Text = "Regex";
            this.RegexCheck.UseVisualStyleBackColor = true;
            this.RegexCheck.CheckedChanged += new System.EventHandler(this.RegexCheck_CheckedChanged);
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Location = new System.Drawing.Point(283, 297);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(177, 41);
            this.ButtonsPanel.TabIndex = 8;
            // 
            // FindBox
            // 
            this.FindBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.FindBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.FindBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FindBox.Location = new System.Drawing.Point(77, 10);
            this.FindBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.FindBox.Name = "FindBox";
            this.FindBox.RegexMode = false;
            this.FindBox.Size = new System.Drawing.Size(209, 21);
            this.FindBox.TabIndex = 1;
            this.FindBox.TextChanged += new System.EventHandler(this.FindBox_TextChanged);
            // 
            // ReplaceBox
            // 
            this.ReplaceBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ReplaceBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ReplaceBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ReplaceBox.Location = new System.Drawing.Point(77, 41);
            this.ReplaceBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.ReplaceBox.Name = "ReplaceBox";
            this.ReplaceBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReplaceBox.Size = new System.Drawing.Size(209, 21);
            this.ReplaceBox.TabIndex = 5;
            this.ReplaceBox.TextChanged += new System.EventHandler(this.ReplaceBox_TextChanged);
            // 
            // BulkEditWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(463, 341);
            this.Controls.Add(this.MainTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BulkEditWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Tags";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BulkEditWindow_FormClosed);
            this.Load += new System.EventHandler(this.BulkEditWindow_Load);
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            this.ButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FindLabel;
        private RegexTextBox FindBox;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label ReplaceLabel;
        private System.Windows.Forms.TextBox ReplaceBox;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.CheckBox RegexCheck;
        private System.Windows.Forms.ListView ActionList;
        private System.Windows.Forms.ColumnHeader CurrentColumn;
        private System.Windows.Forms.ColumnHeader NewColumn;
    }
}