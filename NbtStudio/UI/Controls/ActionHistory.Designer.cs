namespace NbtStudio.UI
{
    partial class ActionHistory
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ActionList = new System.Windows.Forms.ListBox();
            this.InfoPanel = new System.Windows.Forms.Panel();
            this.SelectedLabel = new System.Windows.Forms.Label();
            this.InfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ActionList
            // 
            this.ActionList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ActionList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActionList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ActionList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ActionList.ItemHeight = 15;
            this.ActionList.Location = new System.Drawing.Point(0, 0);
            this.ActionList.Name = "ActionList";
            this.ActionList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.ActionList.Size = new System.Drawing.Size(200, 180);
            this.ActionList.TabIndex = 0;
            this.ActionList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ActionList_DrawItem);
            this.ActionList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ActionList_MouseDown);
            this.ActionList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ActionList_MouseMove);
            // 
            // InfoPanel
            // 
            this.InfoPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.InfoPanel.Controls.Add(this.SelectedLabel);
            this.InfoPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InfoPanel.Location = new System.Drawing.Point(0, 180);
            this.InfoPanel.Name = "InfoPanel";
            this.InfoPanel.Size = new System.Drawing.Size(200, 20);
            this.InfoPanel.TabIndex = 1;
            // 
            // SelectedLabel
            // 
            this.SelectedLabel.AutoSize = true;
            this.SelectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.SelectedLabel.Location = new System.Drawing.Point(3, 4);
            this.SelectedLabel.Name = "SelectedLabel";
            this.SelectedLabel.Size = new System.Drawing.Size(0, 15);
            this.SelectedLabel.TabIndex = 0;
            // 
            // ActionHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ActionList);
            this.Controls.Add(this.InfoPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ActionHistory";
            this.Size = new System.Drawing.Size(200, 200);
            this.InfoPanel.ResumeLayout(false);
            this.InfoPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ActionList;
        private System.Windows.Forms.Panel InfoPanel;
        private System.Windows.Forms.Label SelectedLabel;
    }
}
