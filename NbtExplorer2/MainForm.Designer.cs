namespace NbtExplorer2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.nbtTreeView1 = new NbtExplorer2.NbtTreeView();
            this.Tools = new System.Windows.Forms.ToolStrip();
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolOpenFile = new System.Windows.Forms.ToolStripButton();
            this.ToolOpenFolder = new System.Windows.Forms.ToolStripButton();
            this.ToolSave = new System.Windows.Forms.ToolStripButton();
            this.ToolRefresh = new System.Windows.Forms.ToolStripButton();
            this.ToolCut = new System.Windows.Forms.ToolStripButton();
            this.ToolCopy = new System.Windows.Forms.ToolStripButton();
            this.ToolPaste = new System.Windows.Forms.ToolStripButton();
            this.Seperator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolRename = new System.Windows.Forms.ToolStripButton();
            this.ToolEdit = new System.Windows.Forms.ToolStripButton();
            this.ToolEditSnbt = new System.Windows.Forms.ToolStripButton();
            this.ToolDelete = new System.Windows.Forms.ToolStripButton();
            this.Separator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolAddByte = new System.Windows.Forms.ToolStripButton();
            this.ToolAddShort = new System.Windows.Forms.ToolStripButton();
            this.ToolAddInt = new System.Windows.Forms.ToolStripButton();
            this.ToolAddLong = new System.Windows.Forms.ToolStripButton();
            this.ToolAddFloat = new System.Windows.Forms.ToolStripButton();
            this.ToolAddDouble = new System.Windows.Forms.ToolStripButton();
            this.ToolAddByteArray = new System.Windows.Forms.ToolStripButton();
            this.ToolAddIntArray = new System.Windows.Forms.ToolStripButton();
            this.ToolAddLongArray = new System.Windows.Forms.ToolStripButton();
            this.ToolAddString = new System.Windows.Forms.ToolStripButton();
            this.T = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.nbtTreeView1)).BeginInit();
            this.Tools.SuspendLayout();
            this.Menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // nbtTreeView1
            // 
            this.nbtTreeView1.HideSelection = false;
            this.nbtTreeView1.Location = new System.Drawing.Point(223, 249);
            this.nbtTreeView1.Name = "nbtTreeView1";
            this.nbtTreeView1.OwnerDraw = true;
            this.nbtTreeView1.ShowGroups = false;
            this.nbtTreeView1.Size = new System.Drawing.Size(121, 97);
            this.nbtTreeView1.TabIndex = 0;
            this.nbtTreeView1.UseCompatibleStateImageBehavior = false;
            this.nbtTreeView1.View = System.Windows.Forms.View.Details;
            this.nbtTreeView1.VirtualMode = true;
            // 
            // Tools
            // 
            this.Tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.ToolAddByte,
            this.ToolAddShort,
            this.ToolAddInt,
            this.ToolAddLong,
            this.ToolAddFloat,
            this.ToolAddDouble,
            this.ToolAddByteArray,
            this.ToolAddIntArray,
            this.ToolAddLongArray,
            this.ToolAddString,
            this.T});
            this.Tools.Location = new System.Drawing.Point(0, 24);
            this.Tools.Name = "Tools";
            this.Tools.Size = new System.Drawing.Size(800, 25);
            this.Tools.TabIndex = 1;
            this.Tools.Text = "toolStrip1";
            // 
            // Menu
            // 
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(800, 24);
            this.Menu.TabIndex = 2;
            this.Menu.Text = "menuStrip1";
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
            // ToolOpenFile
            // 
            this.ToolOpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("ToolOpenFile.Image")));
            this.ToolOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolOpenFile.Name = "ToolOpenFile";
            this.ToolOpenFile.Size = new System.Drawing.Size(23, 22);
            this.ToolOpenFile.Text = "toolStripButton1";
            // 
            // ToolOpenFolder
            // 
            this.ToolOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("ToolOpenFolder.Image")));
            this.ToolOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolOpenFolder.Name = "ToolOpenFolder";
            this.ToolOpenFolder.Size = new System.Drawing.Size(23, 22);
            this.ToolOpenFolder.Text = "toolStripButton1";
            // 
            // ToolSave
            // 
            this.ToolSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolSave.Image = ((System.Drawing.Image)(resources.GetObject("ToolSave.Image")));
            this.ToolSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolSave.Name = "ToolSave";
            this.ToolSave.Size = new System.Drawing.Size(23, 22);
            this.ToolSave.Text = "toolStripButton1";
            // 
            // ToolRefresh
            // 
            this.ToolRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolRefresh.Image = ((System.Drawing.Image)(resources.GetObject("ToolRefresh.Image")));
            this.ToolRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolRefresh.Name = "ToolRefresh";
            this.ToolRefresh.Size = new System.Drawing.Size(23, 22);
            this.ToolRefresh.Text = "toolStripButton1";
            // 
            // ToolCut
            // 
            this.ToolCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolCut.Image = ((System.Drawing.Image)(resources.GetObject("ToolCut.Image")));
            this.ToolCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolCut.Name = "ToolCut";
            this.ToolCut.Size = new System.Drawing.Size(23, 22);
            this.ToolCut.Text = "toolStripButton1";
            // 
            // ToolCopy
            // 
            this.ToolCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolCopy.Image = ((System.Drawing.Image)(resources.GetObject("ToolCopy.Image")));
            this.ToolCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolCopy.Name = "ToolCopy";
            this.ToolCopy.Size = new System.Drawing.Size(23, 22);
            this.ToolCopy.Text = "toolStripButton1";
            // 
            // ToolPaste
            // 
            this.ToolPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolPaste.Image = ((System.Drawing.Image)(resources.GetObject("ToolPaste.Image")));
            this.ToolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolPaste.Name = "ToolPaste";
            this.ToolPaste.Size = new System.Drawing.Size(23, 22);
            this.ToolPaste.Text = "toolStripButton1";
            // 
            // Seperator1
            // 
            this.Seperator1.Name = "Seperator1";
            this.Seperator1.Size = new System.Drawing.Size(6, 25);
            // 
            // Separator2
            // 
            this.Separator2.Name = "Separator2";
            this.Separator2.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolRename
            // 
            this.ToolRename.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolRename.Image = ((System.Drawing.Image)(resources.GetObject("ToolRename.Image")));
            this.ToolRename.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolRename.Name = "ToolRename";
            this.ToolRename.Size = new System.Drawing.Size(23, 22);
            this.ToolRename.Text = "toolStripButton1";
            // 
            // ToolEdit
            // 
            this.ToolEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolEdit.Image = ((System.Drawing.Image)(resources.GetObject("ToolEdit.Image")));
            this.ToolEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolEdit.Name = "ToolEdit";
            this.ToolEdit.Size = new System.Drawing.Size(23, 22);
            this.ToolEdit.Text = "toolStripButton1";
            // 
            // ToolEditSnbt
            // 
            this.ToolEditSnbt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolEditSnbt.Image = ((System.Drawing.Image)(resources.GetObject("ToolEditSnbt.Image")));
            this.ToolEditSnbt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolEditSnbt.Name = "ToolEditSnbt";
            this.ToolEditSnbt.Size = new System.Drawing.Size(23, 22);
            this.ToolEditSnbt.Text = "toolStripButton1";
            // 
            // ToolDelete
            // 
            this.ToolDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolDelete.Image = ((System.Drawing.Image)(resources.GetObject("ToolDelete.Image")));
            this.ToolDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolDelete.Name = "ToolDelete";
            this.ToolDelete.Size = new System.Drawing.Size(23, 22);
            this.ToolDelete.Text = "toolStripButton1";
            // 
            // Separator3
            // 
            this.Separator3.Name = "Separator3";
            this.Separator3.Size = new System.Drawing.Size(6, 25);
            // 
            // ToolAddByte
            // 
            this.ToolAddByte.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddByte.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddByte.Image")));
            this.ToolAddByte.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddByte.Name = "ToolAddByte";
            this.ToolAddByte.Size = new System.Drawing.Size(23, 22);
            this.ToolAddByte.Text = "toolStripButton1";
            // 
            // ToolAddShort
            // 
            this.ToolAddShort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddShort.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddShort.Image")));
            this.ToolAddShort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddShort.Name = "ToolAddShort";
            this.ToolAddShort.Size = new System.Drawing.Size(23, 22);
            this.ToolAddShort.Text = "toolStripButton1";
            // 
            // ToolAddInt
            // 
            this.ToolAddInt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddInt.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddInt.Image")));
            this.ToolAddInt.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddInt.Name = "ToolAddInt";
            this.ToolAddInt.Size = new System.Drawing.Size(23, 22);
            this.ToolAddInt.Text = "toolStripButton2";
            // 
            // ToolAddLong
            // 
            this.ToolAddLong.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddLong.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddLong.Image")));
            this.ToolAddLong.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddLong.Name = "ToolAddLong";
            this.ToolAddLong.Size = new System.Drawing.Size(23, 22);
            this.ToolAddLong.Text = "toolStripButton3";
            // 
            // ToolAddFloat
            // 
            this.ToolAddFloat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddFloat.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddFloat.Image")));
            this.ToolAddFloat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddFloat.Name = "ToolAddFloat";
            this.ToolAddFloat.Size = new System.Drawing.Size(23, 22);
            this.ToolAddFloat.Text = "toolStripButton4";
            // 
            // ToolAddDouble
            // 
            this.ToolAddDouble.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddDouble.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddDouble.Image")));
            this.ToolAddDouble.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddDouble.Name = "ToolAddDouble";
            this.ToolAddDouble.Size = new System.Drawing.Size(23, 22);
            this.ToolAddDouble.Text = "toolStripButton5";
            // 
            // ToolAddByteArray
            // 
            this.ToolAddByteArray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddByteArray.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddByteArray.Image")));
            this.ToolAddByteArray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddByteArray.Name = "ToolAddByteArray";
            this.ToolAddByteArray.Size = new System.Drawing.Size(23, 22);
            this.ToolAddByteArray.Text = "toolStripButton6";
            // 
            // ToolAddIntArray
            // 
            this.ToolAddIntArray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddIntArray.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddIntArray.Image")));
            this.ToolAddIntArray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddIntArray.Name = "ToolAddIntArray";
            this.ToolAddIntArray.Size = new System.Drawing.Size(23, 22);
            this.ToolAddIntArray.Text = "toolStripButton7";
            // 
            // ToolAddLongArray
            // 
            this.ToolAddLongArray.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddLongArray.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddLongArray.Image")));
            this.ToolAddLongArray.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddLongArray.Name = "ToolAddLongArray";
            this.ToolAddLongArray.Size = new System.Drawing.Size(23, 22);
            this.ToolAddLongArray.Text = "toolStripButton8";
            // 
            // ToolAddString
            // 
            this.ToolAddString.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolAddString.Image = ((System.Drawing.Image)(resources.GetObject("ToolAddString.Image")));
            this.ToolAddString.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolAddString.Name = "ToolAddString";
            this.ToolAddString.Size = new System.Drawing.Size(23, 22);
            this.ToolAddString.Text = "toolStripButton1";
            // 
            // T
            // 
            this.T.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.T.Image = ((System.Drawing.Image)(resources.GetObject("T.Image")));
            this.T.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.T.Name = "T";
            this.T.Size = new System.Drawing.Size(23, 22);
            this.T.Text = "toolStripButton1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Tools);
            this.Controls.Add(this.Menu);
            this.Controls.Add(this.nbtTreeView1);
            this.MainMenuStrip = this.Menu;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nbtTreeView1)).EndInit();
            this.Tools.ResumeLayout(false);
            this.Tools.PerformLayout();
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NbtTreeView nbtTreeView1;
        private System.Windows.Forms.ToolStrip Tools;
        private System.Windows.Forms.MenuStrip Menu;
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
        private System.Windows.Forms.ToolStripButton ToolAddByte;
        private System.Windows.Forms.ToolStripButton ToolAddShort;
        private System.Windows.Forms.ToolStripButton ToolAddInt;
        private System.Windows.Forms.ToolStripButton ToolAddLong;
        private System.Windows.Forms.ToolStripButton ToolAddFloat;
        private System.Windows.Forms.ToolStripButton ToolAddDouble;
        private System.Windows.Forms.ToolStripButton ToolAddByteArray;
        private System.Windows.Forms.ToolStripButton ToolAddIntArray;
        private System.Windows.Forms.ToolStripButton ToolAddLongArray;
        private System.Windows.Forms.ToolStripButton ToolAddString;
        private System.Windows.Forms.ToolStripButton T;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    }
}

