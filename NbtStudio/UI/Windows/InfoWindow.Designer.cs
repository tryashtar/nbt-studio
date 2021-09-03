namespace NbtStudio.UI
{
    partial class InfoWindow
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
            this.ButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonOk = new System.Windows.Forms.Button();
            this.ButtonCopy = new System.Windows.Forms.Button();
            this.ButtonDetails = new System.Windows.Forms.Button();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.MessagePanel = new System.Windows.Forms.Panel();
            this.ExtraInfoPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ExtraInfoLabel = new System.Windows.Forms.Label();
            this.ButtonsPanel.SuspendLayout();
            this.MessagePanel.SuspendLayout();
            this.ExtraInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonsPanel.Controls.Add(this.ButtonCancel);
            this.ButtonsPanel.Controls.Add(this.ButtonOk);
            this.ButtonsPanel.Controls.Add(this.ButtonCopy);
            this.ButtonsPanel.Controls.Add(this.ButtonDetails);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 59);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Padding = new System.Windows.Forms.Padding(10, 10, 0, 10);
            this.ButtonsPanel.Size = new System.Drawing.Size(385, 58);
            this.ButtonsPanel.TabIndex = 0;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCancel.Location = new System.Drawing.Point(279, 13);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(93, 35);
            this.ButtonCancel.TabIndex = 4;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.Visible = false;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // ButtonOk
            // 
            this.ButtonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOk.Location = new System.Drawing.Point(180, 13);
            this.ButtonOk.Name = "ButtonOk";
            this.ButtonOk.Size = new System.Drawing.Size(93, 35);
            this.ButtonOk.TabIndex = 1;
            this.ButtonOk.Text = "OK";
            this.ButtonOk.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // ButtonCopy
            // 
            this.ButtonCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonCopy.Location = new System.Drawing.Point(81, 13);
            this.ButtonCopy.Name = "ButtonCopy";
            this.ButtonCopy.Size = new System.Drawing.Size(93, 35);
            this.ButtonCopy.TabIndex = 3;
            this.ButtonCopy.Text = "Copy";
            this.ButtonCopy.Visible = false;
            this.ButtonCopy.Click += new System.EventHandler(this.ButtonCopy_Click);
            // 
            // ButtonDetails
            // 
            this.ButtonDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonDetails.Location = new System.Drawing.Point(279, 54);
            this.ButtonDetails.Name = "ButtonDetails";
            this.ButtonDetails.Size = new System.Drawing.Size(93, 35);
            this.ButtonDetails.TabIndex = 2;
            this.ButtonDetails.Text = "More Details";
            this.ButtonDetails.Click += new System.EventHandler(this.ButtonDetails_Click);
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Location = new System.Drawing.Point(0, 0);
            this.MessageLabel.MaximumSize = new System.Drawing.Size(467, 0);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Padding = new System.Windows.Forms.Padding(9, 7, 16, 7);
            this.MessageLabel.Size = new System.Drawing.Size(25, 29);
            this.MessageLabel.TabIndex = 1;
            // 
            // MessagePanel
            // 
            this.MessagePanel.AutoScroll = true;
            this.MessagePanel.AutoSize = true;
            this.MessagePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MessagePanel.Controls.Add(this.MessageLabel);
            this.MessagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MessagePanel.Location = new System.Drawing.Point(0, 0);
            this.MessagePanel.Name = "MessagePanel";
            this.MessagePanel.Size = new System.Drawing.Size(385, 0);
            this.MessagePanel.TabIndex = 2;
            // 
            // ExtraInfoPanel
            // 
            this.ExtraInfoPanel.AutoScroll = true;
            this.ExtraInfoPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ExtraInfoPanel.Controls.Add(this.ExtraInfoLabel);
            this.ExtraInfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ExtraInfoPanel.Location = new System.Drawing.Point(0, -30);
            this.ExtraInfoPanel.Name = "ExtraInfoPanel";
            this.ExtraInfoPanel.Size = new System.Drawing.Size(385, 89);
            this.ExtraInfoPanel.TabIndex = 3;
            this.ExtraInfoPanel.Visible = false;
            // 
            // ExtraInfoLabel
            // 
            this.ExtraInfoLabel.AutoSize = true;
            this.ExtraInfoLabel.Location = new System.Drawing.Point(3, 0);
            this.ExtraInfoLabel.Name = "ExtraInfoLabel";
            this.ExtraInfoLabel.Padding = new System.Windows.Forms.Padding(9, 7, 16, 7);
            this.ExtraInfoLabel.Size = new System.Drawing.Size(25, 29);
            this.ExtraInfoLabel.TabIndex = 2;
            // 
            // InfoWindow
            // 
            this.AcceptButton = this.ButtonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(385, 117);
            this.Controls.Add(this.MessagePanel);
            this.Controls.Add(this.ExtraInfoPanel);
            this.Controls.Add(this.ButtonsPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(581, 802);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(114, 156);
            this.Name = "InfoWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.InfoWindow_Load);
            this.ButtonsPanel.ResumeLayout(false);
            this.MessagePanel.ResumeLayout(false);
            this.MessagePanel.PerformLayout();
            this.ExtraInfoPanel.ResumeLayout(false);
            this.ExtraInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel ButtonsPanel;
        private System.Windows.Forms.Button ButtonOk;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Panel MessagePanel;
        private System.Windows.Forms.Button ButtonDetails;
        private System.Windows.Forms.FlowLayoutPanel ExtraInfoPanel;
        private System.Windows.Forms.Label ExtraInfoLabel;
        private System.Windows.Forms.Button ButtonCopy;
        private System.Windows.Forms.Button ButtonCancel;
    }
}