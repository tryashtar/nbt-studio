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
            this.Icon = current.Refresh.Icon;
            RefreshIcons();
        }

        public void RefreshIcons()
        {
            Action select = () => { };
            int row = 0;
            IconTable.Controls.Clear();
            IconTable.RowStyles.Clear();
            foreach (var item in IconSourceRegistry.RegisteredSources)
            {
                var source = item.Value;
                var button = new Button()
                {
                    Font = this.Font,
                    Text = source.Name,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5)
                };
                button.Click += (s, e) =>
                {
                    SelectedSource = source;
                    this.Close();
                };
                var preview = new FlowLayoutPanel()
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true
                };
                preview.Controls.AddRange(new[]
                {
                    MakePictureBox(source.OpenFile),
                    MakePictureBox(source.Save),
                    MakePictureBox(source.Edit),
                    MakePictureBox(source.Cut),
                    MakePictureBox(source.Undo),
                    MakePictureBox(source.ByteTag),
                    MakePictureBox(source.StringTag),
                    MakePictureBox(source.IntArrayTag),
                    MakePictureBox(source.ListTag),
                    MakePictureBox(source.Region),
                    MakePictureBox(source.Chunk)
                });
                if (CurrentSource == source)
                {
                    SelectedRow = row;
                    preview.BackColor = Color.FromArgb(201, 255, 221);
                    select = () => button.Select();
                }
                IconTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                IconTable.Controls.Add(button, 0, row);
                IconTable.Controls.Add(preview, 1, row);
                row++;
            }
            select();
        }

        private void IconTable_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == SelectedRow)
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(201, 255, 221)), e.CellBounds);
        }

        private PictureBox MakePictureBox(ImageIcon icon)
        {
            return new InterpPictureBox
            {
                Height = 32,
                Width = 32,
                Margin = new Padding(5),
                Image = icon.Image,
                SizeMode = PictureBoxSizeMode.Zoom,
                InterpolationMode = InterpolationMode.NearestNeighbor
            };
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

    public class InterpPictureBox : PictureBox
    {
        public InterpolationMode InterpolationMode;
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
        }
    }
}