using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class FullIconPreviewWindow : Form
    {
        public FullIconPreviewWindow(IconSource source)
        {
            InitializeComponent();
            var preview = new IconSourcePreview(source, false, (IconType[])Enum.GetValues(typeof(IconType)));
            preview.MaximumSize = new Size(470, 0);
            preview.Dock = DockStyle.Fill;
            Controls.Add(preview);
            this.Text = source.Name + " Icon Set";
            this.Icon = source.GetImage(IconType.Search).Icon;
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
}