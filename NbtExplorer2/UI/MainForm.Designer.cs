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
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.NbtTree = new NbtExplorer2.UI.NbtTreeView();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tools
            // 
            this.Tools.Location = new System.Drawing.Point(0, 24);
            this.Tools.Name = "Tools";
            this.Tools.Size = new System.Drawing.Size(800, 25);
            this.Tools.TabIndex = 1;
            this.Tools.Text = "toolStrip1";
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
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "&File";
            // 
            // MenuEdit
            // 
            this.MenuEdit.Name = "MenuEdit";
            this.MenuEdit.Size = new System.Drawing.Size(39, 20);
            this.MenuEdit.Text = "&Edit";
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
            this.NbtTree.AllowDrop = true;
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
            this.NbtTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.NbtTree_ItemDrag);
            this.NbtTree.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.NbtTree_NodeMouseDoubleClick);
            this.NbtTree.SelectionChanged += new System.EventHandler(this.NbtTree_SelectionChanged);
            this.NbtTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.NbtTree_DragDrop);
            this.NbtTree.DragOver += new System.Windows.Forms.DragEventHandler(this.NbtTree_DragOver);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip Tools;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuEdit;
        private System.Windows.Forms.ToolStripMenuItem MenuSearch;
        private System.Windows.Forms.ToolStripMenuItem MenuHelp;
        private NbtTreeView NbtTree;
    }
}

