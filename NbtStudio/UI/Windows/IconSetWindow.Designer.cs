namespace NbtStudio.UI
{
    partial class IconSetWindow
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
            this.IconTable = new System.Windows.Forms.TableLayoutPanel();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.ImportButton = new System.Windows.Forms.Button();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // IconTable
            // 
            this.IconTable.AutoSize = true;
            this.IconTable.ColumnCount = 2;
            this.IconTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.IconTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.IconTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IconTable.Location = new System.Drawing.Point(0, 37);
            this.IconTable.Name = "IconTable";
            this.IconTable.RowCount = 1;
            this.IconTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.IconTable.Size = new System.Drawing.Size(370, 64);
            this.IconTable.TabIndex = 2;
            this.IconTable.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.IconTable_CellPaint);
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ButtonsPanel.Controls.Add(this.ImportButton);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 0);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(370, 37);
            this.ButtonsPanel.TabIndex = 0;
            // 
            // ImportButton
            // 
            this.ImportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ImportButton.Location = new System.Drawing.Point(6, 5);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(75, 29);
            this.ImportButton.TabIndex = 1;
            this.ImportButton.Text = "Import";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // IconSetWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(370, 101);
            this.Controls.Add(this.IconTable);
            this.Controls.Add(this.ButtonsPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IconSetWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Icon Set";
            this.Load += new System.EventHandler(this.IconSetWindow_Load);
            this.ButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel IconTable;
        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.Button ImportButton;
    }
}