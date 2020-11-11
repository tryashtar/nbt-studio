namespace NbtStudio.UI
{
    partial class AboutWindow
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
            this.ImageBox = new System.Windows.Forms.PictureBox();
            this.NameLabel = new System.Windows.Forms.LinkLabel();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.NbtExplorerLabel = new System.Windows.Forms.LinkLabel();
            this.GenericIconLabel = new System.Windows.Forms.LinkLabel();
            this.NbtIconLabel = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageBox
            // 
            this.ImageBox.Image = global::NbtStudio.Properties.Resources.app_image_256;
            this.ImageBox.Location = new System.Drawing.Point(14, 12);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(75, 64);
            this.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageBox.TabIndex = 0;
            this.ImageBox.TabStop = false;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.NameLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.NameLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(102)))), ((int)(((byte)(214)))));
            this.NameLabel.Location = new System.Drawing.Point(95, 12);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(106, 24);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.TabStop = true;
            this.NameLabel.Text = "NBT Studio";
            this.NameLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NameLabel_LinkClicked);
            // 
            // InfoLabel
            // 
            this.InfoLabel.AutoSize = true;
            this.InfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoLabel.Location = new System.Drawing.Point(95, 46);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(161, 96);
            this.InfoLabel.TabIndex = 1;
            this.InfoLabel.Text = "Copyright © 2020 tryashtar\r\n\r\nBased on:\r\n\r\n\r\nIcons created by:";
            // 
            // NbtExplorerLabel
            // 
            this.NbtExplorerLabel.AutoSize = true;
            this.NbtExplorerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.NbtExplorerLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.NbtExplorerLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(102)))), ((int)(((byte)(214)))));
            this.NbtExplorerLabel.Location = new System.Drawing.Point(96, 96);
            this.NbtExplorerLabel.Name = "NbtExplorerLabel";
            this.NbtExplorerLabel.Size = new System.Drawing.Size(167, 17);
            this.NbtExplorerLabel.TabIndex = 2;
            this.NbtExplorerLabel.TabStop = true;
            this.NbtExplorerLabel.Text = "NBTExplorer by jaquadro";
            this.NbtExplorerLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NbtExplorerLabel_LinkClicked);
            // 
            // GenericIconLabel
            // 
            this.GenericIconLabel.AutoSize = true;
            this.GenericIconLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.GenericIconLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.GenericIconLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(102)))), ((int)(((byte)(214)))));
            this.GenericIconLabel.Location = new System.Drawing.Point(96, 142);
            this.GenericIconLabel.Name = "GenericIconLabel";
            this.GenericIconLabel.Size = new System.Drawing.Size(140, 17);
            this.GenericIconLabel.TabIndex = 3;
            this.GenericIconLabel.TabStop = true;
            this.GenericIconLabel.Text = "Yusuke Kamiyamane";
            this.GenericIconLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GenericIconLabel_LinkClicked);
            // 
            // NbtIconLabel
            // 
            this.NbtIconLabel.AutoSize = true;
            this.NbtIconLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.NbtIconLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.NbtIconLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(3)))), ((int)(((byte)(102)))), ((int)(((byte)(214)))));
            this.NbtIconLabel.Location = new System.Drawing.Point(96, 159);
            this.NbtIconLabel.Name = "NbtIconLabel";
            this.NbtIconLabel.Size = new System.Drawing.Size(62, 17);
            this.NbtIconLabel.TabIndex = 4;
            this.NbtIconLabel.TabStop = true;
            this.NbtIconLabel.Text = "AmberW";
            this.NbtIconLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NbtIconLabel_LinkClicked);
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 194);
            this.Controls.Add(this.NbtIconLabel);
            this.Controls.Add(this.GenericIconLabel);
            this.Controls.Add(this.NbtExplorerLabel);
            this.Controls.Add(this.InfoLabel);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.ImageBox);
            this.Font = new System.Drawing.Font("Symbol", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About NBT Studio";
            this.Load += new System.EventHandler(this.AboutWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageBox;
        private System.Windows.Forms.LinkLabel NameLabel;
        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.LinkLabel NbtExplorerLabel;
        private System.Windows.Forms.LinkLabel GenericIconLabel;
        private System.Windows.Forms.LinkLabel NbtIconLabel;
    }
}