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
        private readonly IconSource CurrentSource;
        public IconSource SelectedSource { get; private set; }
        public IconSetWindow(IconSource current)
        {
            InitializeComponent();
            CurrentSource = current;
            this.Icon = current.Refresh.Icon;
            var table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            Action select = () => { };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            table.CellPaint += Table_CellPaint;
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
                    MakePictureBox(source.ListTag)
                });
                if (current == source)
                {
                    preview.BackColor = Color.FromArgb(201, 255, 221);
                    select = () => button.Select();
                }
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
                table.Controls.Add(button, 0, item.Key);
                table.Controls.Add(preview, 1, item.Key);
            }
            Controls.Add(table);
            select();
        }

        private void Table_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (IconSourceRegistry.GetID(CurrentSource) == e.Row)
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