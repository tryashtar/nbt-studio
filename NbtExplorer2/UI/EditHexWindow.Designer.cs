namespace NbtExplorer2.UI
{
    partial class EditHexWindow
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
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.HexBox = new Be.Windows.Forms.HexBox();
            this.MainTable = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.TabView = new System.Windows.Forms.TabControl();
            this.HexPage = new System.Windows.Forms.TabPage();
            this.TextPage = new System.Windows.Forms.TabPage();
            this.HexInfoPanel = new System.Windows.Forms.Panel();
            this.CursorLabel = new System.Windows.Forms.Label();
            this.TextBox = new System.Windows.Forms.TextBox();
            this.MainTable.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.TabView.SuspendLayout();
            this.HexPage.SuspendLayout();
            this.TextPage.SuspendLayout();
            this.HexInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NameLabel.Location = new System.Drawing.Point(10, 10);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(44, 15);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Name:";
            // 
            // NameBox
            // 
            this.NameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.NameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.NameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NameBox.Location = new System.Drawing.Point(64, 10);
            this.NameBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(209, 21);
            this.NameBox.TabIndex = 1;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonCancel.Location = new System.Drawing.Point(549, 10);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 4;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonOk.Location = new System.Drawing.Point(628, 10);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(75, 23);
            this.ButtonOk.TabIndex = 3;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.UseVisualStyleBackColor = true;
            this.ButtonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // HexBox
            // 
            this.HexBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HexBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HexBox.Location = new System.Drawing.Point(3, 3);
            this.HexBox.Margin = new System.Windows.Forms.Padding(0);
            this.HexBox.Name = "HexBox";
            this.HexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.HexBox.Size = new System.Drawing.Size(678, 210);
            this.HexBox.TabIndex = 2;
            this.HexBox.VScrollBarVisible = true;
            this.HexBox.CurrentLineChanged += new System.EventHandler(this.HexBox_CurrentLineChanged);
            this.HexBox.CurrentPositionInLineChanged += new System.EventHandler(this.HexBox_CurrentPositionInLineChanged);
            this.HexBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HexBox_KeyDown);
            // 
            // MainTable
            // 
            this.MainTable.AutoSize = true;
            this.MainTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainTable.ColumnCount = 2;
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainTable.Controls.Add(this.NameLabel, 0, 0);
            this.MainTable.Controls.Add(this.NameBox, 1, 0);
            this.MainTable.Controls.Add(this.TabView, 0, 1);
            this.MainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTable.Location = new System.Drawing.Point(0, 0);
            this.MainTable.Name = "MainTable";
            this.MainTable.RowCount = 3;
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainTable.Size = new System.Drawing.Size(712, 311);
            this.MainTable.TabIndex = 14;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 311);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(712, 42);
            this.ButtonsPanel.TabIndex = 9;
            // 
            // TabView
            // 
            this.MainTable.SetColumnSpan(this.TabView, 2);
            this.TabView.Controls.Add(this.TextPage);
            this.TabView.Controls.Add(this.HexPage);
            this.TabView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabView.Location = new System.Drawing.Point(10, 41);
            this.TabView.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.TabView.Name = "TabView";
            this.TabView.SelectedIndex = 0;
            this.TabView.Size = new System.Drawing.Size(692, 270);
            this.TabView.TabIndex = 15;
            this.TabView.SelectedIndexChanged += new System.EventHandler(this.TabView_SelectedIndexChanged);
            // 
            // HexPage
            // 
            this.HexPage.Controls.Add(this.HexBox);
            this.HexPage.Controls.Add(this.HexInfoPanel);
            this.HexPage.Location = new System.Drawing.Point(4, 22);
            this.HexPage.Name = "HexPage";
            this.HexPage.Padding = new System.Windows.Forms.Padding(3);
            this.HexPage.Size = new System.Drawing.Size(684, 244);
            this.HexPage.TabIndex = 0;
            this.HexPage.Text = "Hex View";
            this.HexPage.UseVisualStyleBackColor = true;
            // 
            // TextPage
            // 
            this.TextPage.Controls.Add(this.TextBox);
            this.TextPage.Location = new System.Drawing.Point(4, 22);
            this.TextPage.Name = "TextPage";
            this.TextPage.Padding = new System.Windows.Forms.Padding(3);
            this.TextPage.Size = new System.Drawing.Size(684, 244);
            this.TextPage.TabIndex = 1;
            this.TextPage.Text = "Text View";
            this.TextPage.UseVisualStyleBackColor = true;
            // 
            // HexInfoPanel
            // 
            this.HexInfoPanel.Controls.Add(this.CursorLabel);
            this.HexInfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.HexInfoPanel.Location = new System.Drawing.Point(3, 213);
            this.HexInfoPanel.Name = "HexInfoPanel";
            this.HexInfoPanel.Size = new System.Drawing.Size(678, 28);
            this.HexInfoPanel.TabIndex = 10;
            // 
            // CursorLabel
            // 
            this.CursorLabel.AutoSize = true;
            this.CursorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.CursorLabel.Location = new System.Drawing.Point(7, 8);
            this.CursorLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.CursorLabel.Name = "CursorLabel";
            this.CursorLabel.Size = new System.Drawing.Size(0, 15);
            this.CursorLabel.TabIndex = 6;
            // 
            // TextBox
            // 
            this.TextBox.AcceptsReturn = true;
            this.TextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.TextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox.Location = new System.Drawing.Point(3, 3);
            this.TextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TextBox.MaxLength = 2147483647;
            this.TextBox.Multiline = true;
            this.TextBox.Name = "TextBox";
            this.TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox.Size = new System.Drawing.Size(678, 238);
            this.TextBox.TabIndex = 3;
            // 
            // EditHexWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(712, 353);
            this.Controls.Add(this.MainTable);
            this.Controls.Add(this.ButtonsPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(309, 235);
            this.Name = "EditHexWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Hex";
            this.Load += new System.EventHandler(this.EditHexWindow_Load);
            this.MainTable.ResumeLayout(false);
            this.MainTable.PerformLayout();
            this.ButtonsPanel.ResumeLayout(false);
            this.TabView.ResumeLayout(false);
            this.HexPage.ResumeLayout(false);
            this.TextPage.ResumeLayout(false);
            this.TextPage.PerformLayout();
            this.HexInfoPanel.ResumeLayout(false);
            this.HexInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Button ButtonOk;
        private Be.Windows.Forms.HexBox HexBox;
        private System.Windows.Forms.TableLayoutPanel MainTable;
        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TabControl TabView;
        private System.Windows.Forms.TabPage HexPage;
        private System.Windows.Forms.TabPage TextPage;
        private System.Windows.Forms.Panel HexInfoPanel;
        private System.Windows.Forms.Label CursorLabel;
        private System.Windows.Forms.TextBox TextBox;
    }
}