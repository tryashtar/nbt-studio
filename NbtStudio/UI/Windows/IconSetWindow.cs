using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class IconSetWindow : Form
    {
        private int SelectedRow = 0;
        private readonly IconSource CurrentSource;
        public IconSource SelectedSource { get; private set; }
        public IconSetWindow(IconSource current)
        {
            InitializeComponent();
            CurrentSource = current;
            this.Icon = current.GetImage(IconType.Refresh).Icon;
            RefreshIcons();
        }

        public void RefreshIcons()
        {
            SuspendLayout();
            Action select = () => { };
            int row = 0;
            IconTable.Controls.Clear();
            IconTable.RowStyles.Clear();
            foreach (var item in IconSourceRegistry.RegisteredSources)
            {
                var source = item.Value;
                var buttons = new IconSourceButtons(source);
                buttons.Dock = DockStyle.Fill;
                IconTable.RowStyles.Add(new RowStyle(SizeType.Absolute, buttons.Height));
                IconTable.Controls.Add(buttons, 0, row);
                if (buttons.PreferredSize.Width > IconTable.ColumnStyles[0].Width)
                    IconTable.ColumnStyles[0].Width = buttons.PreferredSize.Width;
                buttons.ConfirmClicked += (s, e) =>
                {
                    SelectedSource = source;
                    this.Close();
                };
                buttons.DeleteClicked += (s, e) =>
                {
                    IconSourceRegistry.Unregister(item.Key);
                    Properties.Settings.Default.CustomIconSets.Remove(item.Key);
                    RefreshIcons();
                };
                var preview = new IconSourcePreview(source);
                preview.Dock = DockStyle.Fill;
                IconTable.Controls.Add(preview, 1, row);
                if (CurrentSource == source)
                {
                    SelectedRow = row;
                    buttons.BackColor = Color.FromArgb(201, 255, 221);
                    preview.BackColor = Color.FromArgb(201, 255, 221);
                    select = () => buttons.Select();
                }
                row++;
            }
            select();
            ResumeLayout();
        }

        private void IconTable_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == SelectedRow)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(201, 255, 221)), e.CellBounds);
        }

        private void IconSetWindow_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Title = "Select a custom icon ZIP file",
                RestoreDirectory = false,
                Multiselect = true,
                Filter = "ZIP Files|*.zip"
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in dialog.FileNames)
                    {
                        TryImportSource(file);
                    }
                    RefreshIcons();
                }
            }
        }

        public static bool TryImportSource(string path)
        {
            try
            {
                IconSourceRegistry.RegisterCustomSource(path);
                Properties.Settings.Default.CustomIconSets.Add(path);
                return true;
            }
            catch (Exception ex)
            {
                Properties.Settings.Default.CustomIconSets.Remove(path);
                MessageBox.Show($"The custom icon source at '{path}' failed to load.\n\n{Util.ExceptionMessage(ex)}",
                    "Failed to load custom icon source");
                return false;
            }
        }
    }
}