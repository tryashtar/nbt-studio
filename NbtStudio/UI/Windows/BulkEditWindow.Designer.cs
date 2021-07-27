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
            this.ReplaceLabel = new System.Windows.Forms.Label();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ActionList = new System.Windows.Forms.ListView();
            this.CurrentColumn = new System.Windows.Forms.ColumnHeader();
            this.NewColumn = new System.Windows.Forms.ColumnHeader();
            this.FindBox = new NbtStudio.UI.RegexTextBox();
            this.ReplaceBox = new System.Windows.Forms.TextBox();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.RegexCheck = new System.Windows.Forms.CheckBox();
            this.TagsChangingLabel = new System.Windows.Forms.Label();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.MainTable.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FindLabel
            // 
            this.FindLabel.AutoSize = true;
            this.FindLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FindLabel.Location = new System.Drawing.Point(12, 12);
            this.FindLabel.Margin = new System.Windows.Forms.Padding(12, 12, 6, 0);
            this.FindLabel.Name = "FindLabel";
            this.FindLabel.Size = new System.Drawing.Size(34, 15);
            this.FindLabel.TabIndex = 1;
            this.FindLabel.Text = "Find:";
            // 
            // ReplaceLabel
            // 
            this.ReplaceLabel.AutoSize = true;
            this.ReplaceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ReplaceLabel.Location = new System.Drawing.Point(12, 45);
            this.ReplaceLabel.Margin = new System.Windows.Forms.Padding(12, 12, 6, 0);
            this.ReplaceLabel.Name = "ReplaceLabel";
            this.ReplaceLabel.Size = new System.Drawing.Size(56, 15);
            this.ReplaceLabel.TabIndex = 3;
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
            this.MainTable.Controls.Add(this.FindLabel, 0, 0);
            this.MainTable.Controls.Add(this.FindBox, 1, 0);
            this.MainTable.Controls.Add(this.ReplaceLabel, 0, 1);
            this.MainTable.Controls.Add(this.ReplaceBox, 1, 1);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 3;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainTable.Size = new System.Drawing.Size(362, 149);
            this.MainTable.TabIndex = 0;
            // 
            // ActionList
            // 
            this.ActionList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ActionList.CheckBoxes = true;
            this.ActionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CurrentColumn,
            this.NewColumn});
            this.ActionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActionList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ActionList.GridLines = true;
            this.ActionList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ActionList.HideSelection = false;
            this.ActionList.Location = new System.Drawing.Point(80, 78);
            this.ActionList.Margin = new System.Windows.Forms.Padding(6, 12, 12, 12);
            this.ActionList.MultiSelect = false;
            this.ActionList.Name = "ActionList";
            this.ActionList.ShowItemToolTips = true;
            this.ActionList.Size = new System.Drawing.Size(270, 59);
            this.ActionList.TabIndex = 5;
            this.ActionList.UseCompatibleStateImageBehavior = false;
            this.ActionList.View = System.Windows.Forms.View.Details;
            this.ActionList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ActionList_ItemCheck);
            this.ActionList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ActionList_ItemChecked);
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
            // FindBox
            // 
            this.FindBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.FindBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.FindBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FindBox.Location = new System.Drawing.Point(80, 12);
            this.FindBox.Margin = new System.Windows.Forms.Padding(6, 12, 12, 0);
            this.FindBox.Name = "FindBox";
            this.FindBox.RegexMode = false;
            this.FindBox.Size = new System.Drawing.Size(243, 21);
            this.FindBox.TabIndex = 2;
            this.FindBox.TextChanged += new System.EventHandler(this.FindBox_TextChanged);
            // 
            // ReplaceBox
            // 
            this.ReplaceBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ReplaceBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ReplaceBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ReplaceBox.Location = new System.Drawing.Point(80, 45);
            this.ReplaceBox.Margin = new System.Windows.Forms.Padding(6, 12, 12, 0);
            this.ReplaceBox.Name = "ReplaceBox";
            this.ReplaceBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ReplaceBox.Size = new System.Drawing.Size(243, 21);
            this.ReplaceBox.TabIndex = 4;
            this.ReplaceBox.TextChanged += new System.EventHandler(this.ReplaceBox_TextChanged);
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Controls.Add(this.RegexCheck);
            this.ButtonsPanel.Controls.Add(this.TagsChangingLabel);
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 149);
            this.ButtonsPanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(362, 70);
            this.ButtonsPanel.TabIndex = 9;
            this.ButtonsPanel.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // RegexCheck
            // 
            this.RegexCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RegexCheck.AutoSize = true;
            this.RegexCheck.Location = new System.Drawing.Point(10, 37);
            this.RegexCheck.Margin = new System.Windows.Forms.Padding(12, 0, 6, 12);
            this.RegexCheck.Name = "RegexCheck";
            this.RegexCheck.Size = new System.Drawing.Size(58, 19);
            this.RegexCheck.TabIndex = 11;
            this.RegexCheck.Text = "Regex";
            this.RegexCheck.UseVisualStyleBackColor = true;
            this.RegexCheck.CheckedChanged += new System.EventHandler(this.RegexCheck_CheckedChanged);
            // 
            // TagsChangingLabel
            // 
            this.TagsChangingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TagsChangingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TagsChangingLabel.ForeColor = System.Drawing.Color.Gray;
            this.TagsChangingLabel.Location = new System.Drawing.Point(168, 10);
            this.TagsChangingLabel.Margin = new System.Windows.Forms.Padding(12, 12, 6, 0);
            this.TagsChangingLabel.Name = "TagsChangingLabel";
            this.TagsChangingLabel.Size = new System.Drawing.Size(182, 18);
            this.TagsChangingLabel.TabIndex = 10;
            this.TagsChangingLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ButtonOk.Location = new System.Drawing.Point(168, 31);
            this.ButtonOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(88, 27);
            this.ButtonOk.TabIndex = 8;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ButtonCancel.Location = new System.Drawing.Point(262, 31);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(88, 27);
            this.ButtonCancel.TabIndex = 9;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // BulkEditWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(362, 219);
            this.Controls.Add(this.MainTable);
            this.Controls.Add(this.ButtonsPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(378, 258);
            this.Name = "BulkEditWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Tags...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BulkEditWindow_FormClosed);
            this.Load += new System.EventHandler(this.BulkEditWindow_Load);
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            this.ButtonsPanel.ResumeLayout(false);
            this.ButtonsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label FindLabel;
        private RegexTextBox FindBox;
        private System.Windows.Forms.Label ReplaceLabel;
        private System.Windows.Forms.TextBox ReplaceBox;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.ListView ActionList;
        private System.Windows.Forms.ColumnHeader CurrentColumn;
        private System.Windows.Forms.ColumnHeader NewColumn;
        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.CheckBox RegexCheck;
        private System.Windows.Forms.Label TagsChangingLabel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Button ButtonCancel;
    }
}