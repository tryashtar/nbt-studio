namespace NbtExplorer2.UI
{
    partial class FindWindow
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
            this.SearchLabel = new System.Windows.Forms.Label();
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.ButtonFindNext = new System.Windows.Forms.Button();
            this.ButtonFindPrev = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.SearchLabel.Location = new System.Drawing.Point(14, 19);
            this.SearchLabel.Margin = new System.Windows.Forms.Padding(10, 10, 5, 0);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(49, 15);
            this.SearchLabel.TabIndex = 0;
            this.SearchLabel.Text = "Search:";
            // 
            // SearchBox
            // 
            this.SearchBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.SearchBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.SearchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.SearchBox.Location = new System.Drawing.Point(73, 19);
            this.SearchBox.Margin = new System.Windows.Forms.Padding(5, 10, 10, 0);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(209, 21);
            this.SearchBox.TabIndex = 1;
            // 
            // ButtonFindNext
            // 
            this.ButtonFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonFindNext.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonFindNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonFindNext.Location = new System.Drawing.Point(208, 54);
            this.ButtonFindNext.Name = "ButtonFindNext";
            this.ButtonFindNext.Size = new System.Drawing.Size(75, 23);
            this.ButtonFindNext.TabIndex = 7;
            this.ButtonFindNext.Text = "Find Next";
            this.ButtonFindNext.UseVisualStyleBackColor = true;
            this.ButtonFindNext.Click += new System.EventHandler(this.ButtonFindNext_Click);
            // 
            // ButtonFindPrev
            // 
            this.ButtonFindPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonFindPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ButtonFindPrev.Location = new System.Drawing.Point(104, 54);
            this.ButtonFindPrev.Name = "ButtonFindPrev";
            this.ButtonFindPrev.Size = new System.Drawing.Size(98, 23);
            this.ButtonFindPrev.TabIndex = 6;
            this.ButtonFindPrev.Text = "Find Previous";
            this.ButtonFindPrev.UseVisualStyleBackColor = true;
            this.ButtonFindPrev.Click += new System.EventHandler(this.ButtonFindPrev_Click);
            // 
            // FindWindow
            // 
            this.AcceptButton = this.ButtonFindNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(295, 89);
            this.Controls.Add(this.SearchBox);
            this.Controls.Add(this.ButtonFindPrev);
            this.Controls.Add(this.ButtonFindNext);
            this.Controls.Add(this.SearchLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find...";
            this.Load += new System.EventHandler(this.FindWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.TextBox SearchBox;
        private System.Windows.Forms.Button ButtonFindNext;
        private System.Windows.Forms.Button ButtonFindPrev;
    }
}