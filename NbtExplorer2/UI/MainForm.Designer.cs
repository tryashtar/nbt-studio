namespace NbtExplorer2.UI
{
    partial class MainForm
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
            this.Tools = new System.Windows.Forms.ToolStrip();
            this.ToolNew = new System.Windows.Forms.ToolStripButton();
            this.ToolOpenFile = new System.Windows.Forms.ToolStripButton();
            this.ToolOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.ToolSave = new System.Windows.Forms.ToolStripButton();
            this.ToolRefresh = new System.Windows.Forms.ToolStripButton();
            this.Seperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolCut = new System.Windows.Forms.ToolStripButton();
            this.ToolCopy = new System.Windows.Forms.ToolStripButton();
            this.ToolPaste = new System.Windows.Forms.ToolStripButton();
            this.Separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolRename = new System.Windows.Forms.ToolStripButton();
            this.ToolEdit = new System.Windows.Forms.ToolStripButton();
            this.ToolEditSnbt = new System.Windows.Forms.ToolStripButton();
            this.ToolDelete = new System.Windows.Forms.ToolStripButton();
            this.Separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Separator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolFind = new System.Windows.Forms.ToolStripButton();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NbtTree = new NbtExplorer2.UI.NbtTreeView();
            this.Tools.SuspendLayout();
            this.MenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NbtTree)).BeginInit();
            this.SuspendLayout();
            // 
            // Tools
            // 
            this.Tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolNew,
            this.ToolOpenFile,
            this.ToolOpenFolder,
            this.ToolSave,
            this.ToolRefresh,
            this.Seperator1,
            this.ToolCut,
            this.ToolCopy,
            this.ToolPaste,
            this.Separator2,
            this.ToolRename,
            this.ToolEdit,
            this.ToolEditSnbt,
            this.ToolDelete,
            this.Separator3,
            this.Separator4,
            this.ToolFind});
            this.Tools.Location = new System.Drawing.Point(0, 24);
            this.Tools.Name = "Tools";
            this.Tools.Size = new System.Drawing.Size(800, 25);
            this.Tools.TabIndex = 1;
            this.Tools.Text = "toolStrip1";
            // 
            // ToolNew
            // 
            this.ToolNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolNew.Image = global::NbtExplorer2.Properties.Resources.action_new_image;
            this.ToolNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolNew.Name = "ToolNew";
            this.ToolNew.Size = new System.Drawing.Size(23, 22);
            this.ToolNew.Text = "New File";
            this.ToolNew.Click += new System.EventHandler(this.ToolNew_Click);
            // 
            // ToolOpenFile
            // 
            this.ToolOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolOpenFile.Image = global::NbtExplorer2.Properties.Resources.action_open_file_image;
            this.ToolOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolOpenFile.Name = "ToolOpenFile";
            this.ToolOpenFile.Size = new System.Drawing.Size(23, 22);
            this.ToolOpenFile.Text = "Open File";
            this.ToolOpenFile.Click += new System.EventHandler(this.ToolOpenFile_Click);
            // 
            // ToolOpenFolder
            // 
            this.ToolOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolOpenFolder.Image = global::NbtExplorer2.Properties.Resources.action_open_folder_image;
            this.ToolOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolOpenFolder.Name = "ToolOpenFolder";
            this.ToolOpenFolder.Size = new System.Drawing.Size(23, 22);
            this.ToolOpenFolder.Text = "Open Folder";
            // 
            // ToolSave
            // 
            this.ToolSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolSave.Image = global::NbtExplorer2.Properties.Resources.action_save_image;
            this.ToolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolSave.Name = "ToolSave";
            this.ToolSave.Size = new System.Drawing.Size(23, 22);
            this.ToolSave.Text = "Save";
            // 
            // ToolRefresh
            // 
            this.ToolRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolRefresh.Image = global::NbtExplorer2.Properties.Resources.action_refresh_image;
            this.ToolRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolRefresh.Name = "ToolRefresh";
            this.ToolRefresh.Size = new System.Drawing.Size(23, 22);
            this.ToolRefresh.Text = "Refresh";
            // 
            // Seperator1
            // 
            this.Seperator1.Name = "Seperator1";
            this.Seperator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolCut
            // 
            this.ToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolCut.Image = global::NbtExplorer2.Properties.Resources.action_cut_image;
            this.ToolCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolCut.Name = "ToolCut";
            this.ToolCut.Size = new System.Drawing.Size(23, 22);
            this.ToolCut.Text = "Cut";
            // 
            // ToolCopy
            // 
            this.ToolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolCopy.Image = global::NbtExplorer2.Properties.Resources.action_copy_image;
            this.ToolCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolCopy.Name = "ToolCopy";
            this.ToolCopy.Size = new System.Drawing.Size(23, 22);
            this.ToolCopy.Text = "Copy";
            // 
            // ToolPaste
            // 
            this.ToolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolPaste.Image = global::NbtExplorer2.Properties.Resources.action_paste_image;
            this.ToolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolPaste.Name = "ToolPaste";
            this.ToolPaste.Size = new System.Drawing.Size(23, 22);
            this.ToolPaste.Text = "Paste";
            // 
            // Separator2
            // 
            this.Separator2.Name = "Separator2";
            this.Separator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolRename
            // 
            this.ToolRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolRename.Image = global::NbtExplorer2.Properties.Resources.action_rename_image;
            this.ToolRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolRename.Name = "ToolRename";
            this.ToolRename.Size = new System.Drawing.Size(23, 22);
            this.ToolRename.Text = "Rename";
            this.ToolRename.Click += new System.EventHandler(this.ToolRename_Click);
            // 
            // ToolEdit
            // 
            this.ToolEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolEdit.Image = global::NbtExplorer2.Properties.Resources.action_edit_image;
            this.ToolEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolEdit.Name = "ToolEdit";
            this.ToolEdit.Size = new System.Drawing.Size(23, 22);
            this.ToolEdit.Text = "Edit";
            this.ToolEdit.Click += new System.EventHandler(this.ToolEdit_Click);
            // 
            // ToolEditSnbt
            // 
            this.ToolEditSnbt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolEditSnbt.Image = global::NbtExplorer2.Properties.Resources.action_edit_snbt_image;
            this.ToolEditSnbt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolEditSnbt.Name = "ToolEditSnbt";
            this.ToolEditSnbt.Size = new System.Drawing.Size(23, 22);
            this.ToolEditSnbt.Text = "Edit as SNBT";
            // 
            // ToolDelete
            // 
            this.ToolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolDelete.Image = global::NbtExplorer2.Properties.Resources.action_delete_image;
            this.ToolDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolDelete.Name = "ToolDelete";
            this.ToolDelete.Size = new System.Drawing.Size(23, 22);
            this.ToolDelete.Text = "Delete";
            this.ToolDelete.Click += new System.EventHandler(this.ToolDelete_Click);
            // 
            // Separator3
            // 
            this.Separator3.Name = "Separator3";
            this.Separator3.Size = new System.Drawing.Size(6, 25);
            // 
            // Separator4
            // 
            this.Separator4.Name = "Separator4";
            this.Separator4.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolFind
            // 
            this.ToolFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolFind.Image = global::NbtExplorer2.Properties.Resources.action_search_image;
            this.ToolFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolFind.Name = "ToolFind";
            this.ToolFind.Size = new System.Drawing.Size(23, 22);
            this.ToolFind.Text = "Search";
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(800, 24);
            this.MenuStrip.TabIndex = 2;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // NbtTree
            // 
            this.NbtTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NbtTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.NbtTree.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.NbtTree.HideSelection = false;
            this.NbtTree.Location = new System.Drawing.Point(0, 49);
            this.NbtTree.Name = "NbtTree";
            this.NbtTree.ShowGroups = false;
            this.NbtTree.Size = new System.Drawing.Size(800, 401);
            this.NbtTree.TabIndex = 0;
            this.NbtTree.UseCompatibleStateImageBehavior = false;
            this.NbtTree.UseExplorerTheme = true;
            this.NbtTree.View = System.Windows.Forms.View.Details;
            this.NbtTree.VirtualMode = true;
            this.NbtTree.SelectedIndexChanged += new System.EventHandler(this.NbtTree_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.NbtTree);
            this.Controls.Add(this.Tools);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.Text = "NbtExplorer 2";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Tools.ResumeLayout(false);
            this.Tools.PerformLayout();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NbtTree)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NbtTreeView NbtTree;
        private System.Windows.Forms.ToolStrip Tools;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripButton ToolOpenFile;
        private System.Windows.Forms.ToolStripButton ToolOpenFolder;
        private System.Windows.Forms.ToolStripButton ToolSave;
        private System.Windows.Forms.ToolStripButton ToolRefresh;
        private System.Windows.Forms.ToolStripSeparator Seperator1;
        private System.Windows.Forms.ToolStripButton ToolCut;
        private System.Windows.Forms.ToolStripButton ToolCopy;
        private System.Windows.Forms.ToolStripButton ToolPaste;
        private System.Windows.Forms.ToolStripSeparator Separator2;
        private System.Windows.Forms.ToolStripButton ToolRename;
        private System.Windows.Forms.ToolStripButton ToolEdit;
        private System.Windows.Forms.ToolStripButton ToolEditSnbt;
        private System.Windows.Forms.ToolStripButton ToolDelete;
        private System.Windows.Forms.ToolStripSeparator Separator3;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator Separator4;
        private System.Windows.Forms.ToolStripButton ToolFind;
        private System.Windows.Forms.ToolStripButton ToolNew;
    }
}

