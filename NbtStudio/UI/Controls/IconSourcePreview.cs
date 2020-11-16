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

namespace NbtStudio
{
    public partial class IconSourcePreview : UserControl
    {
        public IconSourcePreview(IconSource source)
        {
            InitializeComponent();

            IconsPanel.Controls.AddRange(new[]
               {
                    MakePictureBox(source, IconType.OpenFile),
                    MakePictureBox(source, IconType.Save),
                    MakePictureBox(source, IconType.Edit),
                    MakePictureBox(source, IconType.Cut),
                    MakePictureBox(source, IconType.Undo),
                    MakePictureBox(source, IconType.ByteTag),
                    MakePictureBox(source, IconType.StringTag),
                    MakePictureBox(source, IconType.IntArrayTag),
                    MakePictureBox(source, IconType.ListTag),
                    MakePictureBox(source, IconType.Region),
                    MakePictureBox(source, IconType.Chunk)
                });
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
    }

    public class SpecialPictureBox : PictureBox
    {
        public InterpolationMode InterpolationMode;
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode;
            base.OnPaint(pe);
            if (!this.Enabled)
            {
                using (var img = new Bitmap(this.Image, this.ClientSize))
                {
                    ControlPaint.DrawImageDisabled(pe.Graphics, img, 0, 0, this.BackColor);
                }
            }
        }
    }
}
