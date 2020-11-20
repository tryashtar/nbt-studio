using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace NbtStudio.UI
{
    public partial class IconSourcePreview : UserControl
    {
        private FullIconPreviewWindow FullDisplayForm;
        private readonly IconSource Source;
        public IconSourcePreview(IconSource source, params IconType[] types) : this(source, true, types)
        { }

        public IconSourcePreview(IconSource source, bool show_preview_button, params IconType[] types)
        {
            Source = source;
            InitializeComponent();

            var boxes = types.Select(x => MakePictureBox(source, x)).ToArray();
            IconsPanel.Controls.AddRange(boxes);
            IconsPanel.Controls.Add(PreviewButton);
            if (!show_preview_button)
                PreviewButton.Visible = false;
        }

        private PictureBox MakePictureBox(IconSource source, IconType type)
        {
            var box = new SpecialPictureBox
            {
                Height = 32,
                Width = 32,
                Margin = new Padding(5),
                Image = source.GetImage(type).Image,
                SizeMode = PictureBoxSizeMode.Zoom,
                InterpolationMode = InterpolationMode.NearestNeighbor
            };
            if (source is DeferToDefaultIconSource defer && defer.IsDeferring(type))
                box.Enabled = false;
            return box;
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            if (FullDisplayForm == null || FullDisplayForm.IsDisposed)
                FullDisplayForm = new FullIconPreviewWindow(Source);
            if (!FullDisplayForm.Visible)
            {
                FullDisplayForm.Show(this.ParentForm);
                FullDisplayForm.Location = this.PointToScreen(new Point(-20, 0));
            }
            else
                FullDisplayForm.Close();
            FullDisplayForm.Focus();
        }
    }

    public class SpecialPictureBox : PictureBox
    {
        public InterpolationMode InterpolationMode;
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
            if (!this.Enabled && this.Image != null)
            {
                using (var img = new Bitmap(this.Image, this.ClientSize))
                {
                    ControlPaint.DrawImageDisabled(pe.Graphics, img, 0, 0, this.BackColor);
                }
            }
        }
    }
}
