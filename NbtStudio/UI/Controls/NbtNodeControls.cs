using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public class NbtIcon : NodeControl
    {
        public IconSource IconSource;
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            NbtText.DrawSelection(node, context);
            var image = GetIcon(node);
            if (image is not null)
            {
                float ratio = Math.Min((float)context.Bounds.Width / (float)image.Width, (float)context.Bounds.Height / (float)image.Height);
                var rectangle = new Rectangle();
                rectangle.Width = (int)(image.Width * ratio);
                rectangle.Height = (int)(image.Height * ratio);
                rectangle.X = context.Bounds.X + (context.Bounds.Width - rectangle.Width) / 2;
                rectangle.Y = context.Bounds.Y + (context.Bounds.Height - rectangle.Height) / 2;
                if (context.Bounds.Width < image.Width || context.Bounds.Height < image.Height)
                    context.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                else
                    context.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                context.Graphics.DrawImage(image, rectangle);
            }
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            int height = node.Tree.RowHeight - 4;
            return image is null ? Size.Empty : new Size((int)(((float)height / image.Height) * image.Width), height);
        }

        private Image GetIcon(TreeNodeAdv node)
        {
            if (IconSource == null)
                return null;
            return IconSource.GetImage(((Node)node.Tag).GetIcon()).Image;
        }
    }

    public class NbtText : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            DrawSelection(node, context);
            DrawOrMeasure(node, context, draw: true);
        }

        private SizeF DrawOrMeasure(TreeNodeAdv node, DrawContext context, bool draw)
        {
            var (name, value) = ((Node)node.Tag).Preview();
            var boldfont = new Font(context.Font, FontStyle.Bold);
            context.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            context.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            SizeF size = SizeF.Empty;
            var rectangle = context.Bounds;
            var format = TextFormatFlags.PreserveGraphicsClipping |
                TextFormatFlags.PreserveGraphicsTranslateTransform |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPrefix;

            if (name is not null)
            {
                if (draw)
                    TextRenderer.DrawText(context.Graphics, name, boldfont, rectangle, Parent.ForeColor, format);
                var name_size = TextRenderer.MeasureText(context.Graphics, name, boldfont, rectangle.Size, format);
                size = AppendSizes(size, name_size);
                rectangle.X += (int)name_size.Width;
            }
            if (value is not null)
            {
                if (draw)
                    TextRenderer.DrawText(context.Graphics, value, context.Font, rectangle, Parent.ForeColor, format);
                var value_size = TextRenderer.MeasureText(context.Graphics, value, context.Font, rectangle.Size, format);
                size = AppendSizes(size, value_size);
            }
            return size;
        }

        private static SizeF AppendSizes(SizeF size1, SizeF size2)
        {
            return new SizeF(size1.Width + size2.Width, Math.Max(size1.Height, size2.Height));
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var size = DrawOrMeasure(node, context, draw: false);
            return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
        }

        public static void DrawSelection(TreeNodeAdv node, DrawContext context)
        {
            // selected nodes are not "active" while dragging
            // hovered nodes are "active" while dragging
            if (context.DrawSelection == DrawSelectionMode.Active || (node.IsSelected && !node.Tree.Focused))
                context.Graphics.FillRectangle(new SolidBrush(Constants.SelectionColor), context.Bounds);
            else if (node.IsSelected)
                context.Graphics.FillRectangle(Brushes.LightYellow, context.Bounds);
        }

        public override string GetToolTip(TreeNodeAdv node)
        {
            return ((Node)node.Tag).GetTooltip();
        }
    }
}
