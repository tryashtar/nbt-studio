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
            this.Tools = new System.Windows.Forms.ToolStrip();
            this.ToolNew = new System.Windows.Forms.ToolStripButton();
            this.ToolOpenFile = new System.Windows.Forms.ToolStripButton();
            this.ToolOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.ToolSave = new System.Windows.Forms.ToolStripButton();
            this.ToolRefresh = new System.Windows.Forms.ToolStripButton();
            this.ToolSeperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolCut = new System.Windows.Forms.ToolStripButton();
            this.ToolCopy = new System.Windows.Forms.ToolStripButton();
            this.ToolPaste = new System.Windows.Forms.ToolStripButton();
            this.ToolSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolRename = new System.Windows.Forms.ToolStripButton();
            this.ToolEdit = new System.Windows.Forms.ToolStripButton();
            this.ToolEditSnbt = new System.Windows.Forms.ToolStripButton();
            this.ToolDelete = new System.Windows.Forms.ToolStripButton();
            this.ToolSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolFind = new System.Windows.Forms.ToolStripButton();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEditValue = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEditSnbt = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.NbtTree = new NbtExplorer2.UI.NbtTreeView();
            this.Tools.SuspendLayout();
            this.MenuStrip.SuspendLayout();
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
            this.ToolSeperator1,
            this.ToolCut,
            this.ToolCopy,
            this.ToolPaste,
            this.ToolSeparator2,
            this.ToolRename,
            this.ToolEdit,
            this.ToolEditSnbt,
            this.ToolDelete,
            this.ToolSeparator3,
            this.ToolSeparator4,
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
            this.ToolNew.Name = "ToolNew";
            this.ToolNew.Size = new System.Drawing.Size(23, 22);
            this.ToolNew.Text = "New File";
            this.ToolNew.Click += new System.EventHandler(this.ToolNew_Click);
            // 
            // ToolOpenFile
            // 
            this.ToolOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolOpenFile.Image = global::NbtExplorer2.Properties.Resources.action_open_file_image;
            this.ToolOpenFile.Name = "ToolOpenFile";
            this.ToolOpenFile.Size = new System.Drawing.Size(23, 22);
            this.ToolOpenFile.Text = "Open File";
            this.ToolOpenFile.Click += new System.EventHandler(this.ToolOpenFile_Click);
            // 
            // ToolOpenFolder
            // 
            this.ToolOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolOpenFolder.Image = global::NbtExplorer2.Properties.Resources.action_open_folder_image;
            this.ToolOpenFolder.Name = "ToolOpenFolder";
            this.ToolOpenFolder.Size = new System.Drawing.Size(23, 22);
            this.ToolOpenFolder.Text = "Open Folder";
            this.ToolOpenFolder.Click += new System.EventHandler(this.ToolOpenFolder_Click);
            // 
            // ToolSave
            // 
            this.ToolSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolSave.Image = global::NbtExplorer2.Properties.Resources.action_save_image;
            this.ToolSave.Name = "ToolSave";
            this.ToolSave.Size = new System.Drawing.Size(23, 22);
            this.ToolSave.Text = "Save";
            this.ToolSave.Click += new System.EventHandler(this.ToolSave_Click);
            // 
            // ToolRefresh
            // 
            this.ToolRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolRefresh.Image = global::NbtExplorer2.Properties.Resources.action_refresh_image;
            this.ToolRefresh.Name = "ToolRefresh";
            this.ToolRefresh.Size = new System.Drawing.Size(23, 22);
            this.ToolRefresh.Text = "Refresh";
            this.ToolRefresh.Click += new System.EventHandler(this.ToolRefresh_Click);
            // 
            // ToolSeperator1
            // 
            this.ToolSeperator1.Name = "ToolSeperator1";
            this.ToolSeperator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolCut
            // 
            this.ToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolCut.Image = global::NbtExplorer2.Properties.Resources.action_cut_image;
            this.ToolCut.Name = "ToolCut";
            this.ToolCut.Size = new System.Drawing.Size(23, 22);
            this.ToolCut.Text = "Cut";
            this.ToolCut.Click += new System.EventHandler(this.ToolCut_Click);
            // 
            // ToolCopy
            // 
            this.ToolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolCopy.Image = global::NbtExplorer2.Properties.Resources.action_copy_image;
            this.ToolCopy.Name = "ToolCopy";
            this.ToolCopy.Size = new System.Drawing.Size(23, 22);
            this.ToolCopy.Text = "Copy";
            this.ToolCopy.Click += new System.EventHandler(this.ToolCopy_Click);
            // 
            // ToolPaste
            // 
            this.ToolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolPaste.Image = global::NbtExplorer2.Properties.Resources.action_paste_image;
            this.ToolPaste.Name = "ToolPaste";
            this.ToolPaste.Size = new System.Drawing.Size(23, 22);
            this.ToolPaste.Text = "Paste";
            this.ToolPaste.Click += new System.EventHandler(this.ToolPaste_Click);
            // 
            // ToolSeparator2
            // 
            this.ToolSeparator2.Name = "ToolSeparator2";
            this.ToolSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolRename
            // 
            this.ToolRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolRename.Image = global::NbtExplorer2.Properties.Resources.action_rename_image;
            this.ToolRename.Name = "ToolRename";
            this.ToolRename.Size = new System.Drawing.Size(23, 22);
            this.ToolRename.Text = "Rename";
            this.ToolRename.Click += new System.EventHandler(this.ToolRename_Click);
            // 
            // ToolEdit
            // 
            this.ToolEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolEdit.Image = global::NbtExplorer2.Properties.Resources.action_edit_image;
            this.ToolEdit.Name = "ToolEdit";
            this.ToolEdit.Size = new System.Drawing.Size(23, 22);
            this.ToolEdit.Text = "Edit";
            this.ToolEdit.Click += new System.EventHandler(this.ToolEdit_Click);
            // 
            // ToolEditSnbt
            // 
            this.ToolEditSnbt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolEditSnbt.Image = global::NbtExplorer2.Properties.Resources.action_edit_snbt_image;
            this.ToolEditSnbt.Name = "ToolEditSnbt";
            this.ToolEditSnbt.Size = new System.Drawing.Size(23, 22);
            this.ToolEditSnbt.Text = "Edit as SNBT";
            this.ToolEditSnbt.Click += new System.EventHandler(this.ToolEditSnbt_Click);
            // 
            // ToolDelete
            // 
            this.ToolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolDelete.Image = global::NbtExplorer2.Properties.Resources.action_delete_image;
            this.ToolDelete.Name = "ToolDelete";
            this.ToolDelete.Size = new System.Drawing.Size(23, 22);
            this.ToolDelete.Text = "Delete";
            this.ToolDelete.Click += new System.EventHandler(this.ToolDelete_Click);
            // 
            // ToolSeparator3
            // 
            this.ToolSeparator3.Name = "ToolSeparator3";
            this.ToolSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolSeparator4
            // 
            this.ToolSeparator4.Name = "ToolSeparator4";
            this.ToolSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolFind
            // 
            this.ToolFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolFind.Image = global::NbtExplorer2.Properties.Resources.action_search_image;
            this.ToolFind.Name = "ToolFind";
            this.ToolFind.Size = new System.Drawing.Size(23, 22);
            this.ToolFind.Text = "Search";
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuEdit,
            this.MenuSearch,
            this.MenuHelp});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(800, 24);
            this.MenuStrip.TabIndex = 2;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNew,
            this.MenuOpenFile,
            this.MenuOpenFolder,
            this.MenuFileSeparator1,
            this.MenuSave,
            this.MenuSaveAs,
            this.MenuRefresh,
            this.MenuFileSeparator2,
            this.MenuRecent});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "&File";
            // 
            // MenuNew
            // 
            this.MenuNew.Image = global::NbtExplorer2.Properties.Resources.action_new_image;
            this.MenuNew.Name = "MenuNew";
            this.MenuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.MenuNew.Size = new System.Drawing.Size(214, 22);
            this.MenuNew.Text = "&New";
            this.MenuNew.Click += new System.EventHandler(this.ToolNew_Click);
            // 
            // MenuOpenFile
            // 
            this.MenuOpenFile.Image = global::NbtExplorer2.Properties.Resources.action_open_file_image;
            this.MenuOpenFile.Name = "MenuOpenFile";
            this.MenuOpenFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuOpenFile.Size = new System.Drawing.Size(214, 22);
            this.MenuOpenFile.Text = "&Open File";
            this.MenuOpenFile.Click += new System.EventHandler(this.ToolOpenFile_Click);
            // 
            // MenuOpenFolder
            // 
            this.MenuOpenFolder.Image = global::NbtExplorer2.Properties.Resources.action_open_folder_image;
            this.MenuOpenFolder.Name = "MenuOpenFolder";
            this.MenuOpenFolder.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
            this.MenuOpenFolder.Size = new System.Drawing.Size(214, 22);
            this.MenuOpenFolder.Text = "Open &Folder";
            this.MenuOpenFolder.Click += new System.EventHandler(this.ToolOpenFolder_Click);
            // 
            // MenuFileSeparator1
            // 
            this.MenuFileSeparator1.Name = "MenuFileSeparator1";
            this.MenuFileSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // MenuSave
            // 
            this.MenuSave.Image = global::NbtExplorer2.Properties.Resources.action_save_image;
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MenuSave.Size = new System.Drawing.Size(214, 22);
            this.MenuSave.Text = "&Save";
            this.MenuSave.Click += new System.EventHandler(this.ToolSave_Click);
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.Image = global::NbtExplorer2.Properties.Resources.action_save_image;
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.MenuSaveAs.Size = new System.Drawing.Size(214, 22);
            this.MenuSaveAs.Text = "Save &As";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // MenuRefresh
            // 
            this.MenuRefresh.Image = global::NbtExplorer2.Properties.Resources.action_refresh_image;
            this.MenuRefresh.Name = "MenuRefresh";
            this.MenuRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.MenuRefresh.Size = new System.Drawing.Size(214, 22);
            this.MenuRefresh.Text = "&Refresh";
            this.MenuRefresh.Click += new System.EventHandler(this.ToolRefresh_Click);
            // 
            // MenuFileSeparator2
            // 
            this.MenuFileSeparator2.Name = "MenuFileSeparator2";
            this.MenuFileSeparator2.Size = new System.Drawing.Size(211, 6);
            // 
            // MenuRecent
            // 
            this.MenuRecent.Name = "MenuRecent";
            this.MenuRecent.Size = new System.Drawing.Size(214, 22);
            this.MenuRecent.Text = "&Recent";
            // 
            // MenuEdit
            // 
            this.MenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuCut,
            this.MenuCopy,
            this.MenuPaste,
            this.MenuEditSeparator1,
            this.MenuRename,
            this.MenuEditValue,
            this.MenuEditSnbt,
            this.MenuDelete});
            this.MenuEdit.Name = "MenuEdit";
            this.MenuEdit.Size = new System.Drawing.Size(39, 20);
            this.MenuEdit.Text = "&Edit";
            // 
            // MenuCut
            // 
            this.MenuCut.Image = global::NbtExplorer2.Properties.Resources.action_cut_image;
            this.MenuCut.Name = "MenuCut";
            this.MenuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.MenuCut.Size = new System.Drawing.Size(210, 22);
            this.MenuCut.Text = "Cu&t";
            this.MenuCut.Click += new System.EventHandler(this.ToolCut_Click);
            // 
            // MenuCopy
            // 
            this.MenuCopy.Image = global::NbtExplorer2.Properties.Resources.action_copy_image;
            this.MenuCopy.Name = "MenuCopy";
            this.MenuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.MenuCopy.Size = new System.Drawing.Size(210, 22);
            this.MenuCopy.Text = "&Copy";
            this.MenuCopy.Click += new System.EventHandler(this.ToolCopy_Click);
            // 
            // MenuPaste
            // 
            this.MenuPaste.Image = global::NbtExplorer2.Properties.Resources.action_paste_image;
            this.MenuPaste.Name = "MenuPaste";
            this.MenuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.MenuPaste.Size = new System.Drawing.Size(210, 22);
            this.MenuPaste.Text = "&Paste";
            this.MenuPaste.Click += new System.EventHandler(this.ToolPaste_Click);
            // 
            // MenuEditSeparator1
            // 
            this.MenuEditSeparator1.Name = "MenuEditSeparator1";
            this.MenuEditSeparator1.Size = new System.Drawing.Size(207, 6);
            // 
            // MenuRename
            // 
            this.MenuRename.Image = global::NbtExplorer2.Properties.Resources.action_rename_image;
            this.MenuRename.Name = "MenuRename";
            this.MenuRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.MenuRename.Size = new System.Drawing.Size(210, 22);
            this.MenuRename.Text = "&Rename";
            this.MenuRename.Click += new System.EventHandler(this.ToolRename_Click);
            // 
            // MenuEditValue
            // 
            this.MenuEditValue.Image = global::NbtExplorer2.Properties.Resources.action_edit_image;
            this.MenuEditValue.Name = "MenuEditValue";
            this.MenuEditValue.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.MenuEditValue.Size = new System.Drawing.Size(210, 22);
            this.MenuEditValue.Text = "&Edit Value";
            this.MenuEditValue.Click += new System.EventHandler(this.ToolEdit_Click);
            // 
            // MenuEditSnbt
            // 
            this.MenuEditSnbt.Image = global::NbtExplorer2.Properties.Resources.action_edit_snbt_image;
            this.MenuEditSnbt.Name = "MenuEditSnbt";
            this.MenuEditSnbt.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.MenuEditSnbt.Size = new System.Drawing.Size(210, 22);
            this.MenuEditSnbt.Text = "Edit as &SNBT";
            this.MenuEditSnbt.Click += new System.EventHandler(this.ToolEditSnbt_Click);
            // 
            // MenuDelete
            // 
            this.MenuDelete.Image = global::NbtExplorer2.Properties.Resources.action_delete_image;
            this.MenuDelete.Name = "MenuDelete";
            this.MenuDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.MenuDelete.Size = new System.Drawing.Size(210, 22);
            this.MenuDelete.Text = "&Delete";
            this.MenuDelete.Click += new System.EventHandler(this.ToolDelete_Click);
            // 
            // MenuSearch
            // 
            this.MenuSearch.Name = "MenuSearch";
            this.MenuSearch.Size = new System.Drawing.Size(54, 20);
            this.MenuSearch.Text = "&Search";
            // 
            // MenuHelp
            // 
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Size = new System.Drawing.Size(44, 20);
            this.MenuHelp.Text = "&Help";
            // 
            // NbtTree
            // 
            this.NbtTree.BackColor = System.Drawing.SystemColors.Window;
            this.NbtTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NbtTree.DefaultToolTipProvider = null;
            this.NbtTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NbtTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.NbtTree.Font = new System.Drawing.Font("Arial", 9F);
            this.NbtTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.NbtTree.Location = new System.Drawing.Point(0, 49);
            this.NbtTree.Model = null;
            this.NbtTree.Name = "NbtTree";
            this.NbtTree.RowHeight = 20;
            this.NbtTree.SelectedNode = null;
            this.NbtTree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.NbtTree.Size = new System.Drawing.Size(800, 401);
            this.NbtTree.TabIndex = 3;
            this.NbtTree.Text = "NBT Tree";
            this.NbtTree.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.NbtTree_NodeMouseDoubleClick);
            this.NbtTree.SelectionChanged += new System.EventHandler(this.NbtTree_SelectionChanged);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip Tools;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripButton ToolOpenFile;
        private System.Windows.Forms.ToolStripButton ToolOpenFolder;
        private System.Windows.Forms.ToolStripButton ToolSave;
        private System.Windows.Forms.ToolStripButton ToolRefresh;
        private System.Windows.Forms.ToolStripSeparator ToolSeperator1;
        private System.Windows.Forms.ToolStripButton ToolCut;
        private System.Windows.Forms.ToolStripButton ToolCopy;
        private System.Windows.Forms.ToolStripButton ToolPaste;
        private System.Windows.Forms.ToolStripSeparator ToolSeparator2;
        private System.Windows.Forms.ToolStripButton ToolRename;
        private System.Windows.Forms.ToolStripButton ToolEdit;
        private System.Windows.Forms.ToolStripButton ToolEditSnbt;
        private System.Windows.Forms.ToolStripButton ToolDelete;
        private System.Windows.Forms.ToolStripSeparator ToolSeparator3;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuEdit;
        private System.Windows.Forms.ToolStripMenuItem MenuSearch;
        private System.Windows.Forms.ToolStripMenuItem MenuHelp;
        private System.Windows.Forms.ToolStripSeparator ToolSeparator4;
        private System.Windows.Forms.ToolStripButton ToolFind;
        private System.Windows.Forms.ToolStripButton ToolNew;
        private NbtTreeView NbtTree;
        private System.Windows.Forms.ToolStripMenuItem MenuNew;
        private System.Windows.Forms.ToolStripMenuItem MenuOpenFile;
        private System.Windows.Forms.ToolStripMenuItem MenuOpenFolder;
        private System.Windows.Forms.ToolStripSeparator MenuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuSave;
        private System.Windows.Forms.ToolStripMenuItem MenuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem MenuRefresh;
        private System.Windows.Forms.ToolStripSeparator MenuFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MenuRecent;
        private System.Windows.Forms.ToolStripMenuItem MenuCut;
        private System.Windows.Forms.ToolStripMenuItem MenuCopy;
        private System.Windows.Forms.ToolStripMenuItem MenuPaste;
        private System.Windows.Forms.ToolStripSeparator MenuEditSeparator1;
        private System.Windows.Forms.ToolStripMenuItem MenuRename;
        private System.Windows.Forms.ToolStripMenuItem MenuEditValue;
        private System.Windows.Forms.ToolStripMenuItem MenuEditSnbt;
        private System.Windows.Forms.ToolStripMenuItem MenuDelete;
    }
}

