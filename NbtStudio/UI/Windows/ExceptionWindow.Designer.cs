namespace NbtStudio.UI
{
    partial class ExceptionWindow
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
            this.OKPanel = new System.Windows.Forms.Panel();
            this.ButtonDetails = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.MessagePanel = new System.Windows.Forms.Panel();
            this.ExtraInfoPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ExtraInfoLabel = new System.Windows.Forms.Label();
            this.ButtonCopy = new System.Windows.Forms.Button();
            this.OKPanel.SuspendLayout();
            this.MessagePanel.SuspendLayout();
            this.ExtraInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKPanel
            // 
            this.OKPanel.BackColor = System.Drawing.SystemColors.Control;
            this.OKPanel.Controls.Add(this.ButtonCopy);
            this.OKPanel.Controls.Add(this.ButtonDetails);
            this.OKPanel.Controls.Add(this.ButtonOk);
            this.OKPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OKPanel.Location = new System.Drawing.Point(0, 74);
            this.OKPanel.Name = "OKPanel";
            this.OKPanel.Size = new System.Drawing.Size(289, 50);
            this.OKPanel.TabIndex = 0;
            // 
            // ButtonDetails
            // 
            this.ButtonDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonDetails.Location = new System.Drawing.Point(114, 11);
            this.ButtonDetails.Name = "ButtonDetails";
            this.ButtonDetails.Size = new System.Drawing.Size(80, 30);
            this.ButtonDetails.TabIndex = 2;
            this.ButtonDetails.Text = "More Details";
            this.ButtonDetails.Click += new System.EventHandler(this.ButtonDetails_Click);
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Location = new System.Drawing.Point(200, 11);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(80, 30);
            this.ButtonOk.TabIndex = 1;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Location = new System.Drawing.Point(0, 0);
            this.MessageLabel.MaximumSize = new System.Drawing.Size(400, 0);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Padding = new System.Windows.Forms.Padding(8, 6, 14, 6);
            this.MessageLabel.Size = new System.Drawing.Size(22, 25);
            this.MessageLabel.TabIndex = 1;
            // 
            // MessagePanel
            // 
            this.MessagePanel.AutoSize = true;
            this.MessagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MessagePanel.Controls.Add(this.ExtraInfoPanel);
            this.MessagePanel.Controls.Add(this.MessageLabel);
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagePanel.Location = new System.Drawing.Point(0, 0);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Size = new System.Drawing.Size(289, 74);
            this.MessagePanel.TabIndex = 2;
            // 
            // ExtraInfoPanel
            // 
            this.ExtraInfoPanel.AutoScroll = true;
            this.ExtraInfoPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ExtraInfoPanel.Controls.Add(this.ExtraInfoLabel);
            this.ExtraInfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExtraInfoPanel.Location = new System.Drawing.Point(0, -96);
            this.ExtraInfoPanel.Name = "ExtraInfoPanel";
            this.ExtraInfoPanel.Size = new System.Drawing.Size(289, 170);
            this.ExtraInfoPanel.TabIndex = 3;
            this.ExtraInfoPanel.Visible = false;
            // 
            // ExtraInfoLabel
            // 
            this.ExtraInfoLabel.AutoSize = true;
            this.ExtraInfoLabel.Location = new System.Drawing.Point(3, 0);
            this.ExtraInfoLabel.MaximumSize = new System.Drawing.Size(400, 0);
            this.ExtraInfoLabel.Name = "ExtraInfoLabel";
            this.ExtraInfoLabel.Padding = new System.Windows.Forms.Padding(8, 6, 14, 6);
            this.ExtraInfoLabel.Size = new System.Drawing.Size(22, 25);
            this.ExtraInfoLabel.TabIndex = 2;
            // 
            // ButtonCopy
            // 
            this.ButtonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCopy.Location = new System.Drawing.Point(28, 11);
            this.ButtonCopy.Name = "ButtonCopy";
            this.ButtonCopy.Size = new System.Drawing.Size(80, 30);
            this.ButtonCopy.TabIndex = 3;
            this.ButtonCopy.Text = "Copy";
            this.ButtonCopy.Click += new System.EventHandler(this.ButtonCopy_Click);
            // 
            // ExceptionWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(289, 124);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.OKPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 140);
            this.Name = "ExceptionWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.ExceptionWindow_Load);
            this.OKPanel.ResumeLayout(false);
            this.MessagePanel.ResumeLayout(false);
            this.MessagePanel.PerformLayout();
            this.ExtraInfoPanel.ResumeLayout(false);
            this.ExtraInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel OKPanel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Panel MessagePanel;
        private System.Windows.Forms.Button ButtonDetails;
        private System.Windows.Forms.FlowLayoutPanel ExtraInfoPanel;
        private System.Windows.Forms.Label ExtraInfoLabel;
        private System.Windows.Forms.Button ButtonCopy;
    }
}