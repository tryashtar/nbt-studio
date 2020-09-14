using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public class NbtTreeView2 : TreeViewAdv
    {
        public NbtTreeView2()
        {
            NodeControls.Add(new NbtIcon());
            NodeControls.Add(new NbtText());
            this.RowHeight = 20;
        }
    }

    public class NbtIcon : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            if (image != null)
            {
                float ratio = Math.Min((float)context.Bounds.Width / (float)image.Width, (float)context.Bounds.Height / (float)image.Height);
                var rectangle = new Rectangle();
                rectangle.Width = (int)(image.Width * ratio);
                rectangle.Height = (int)(image.Height * ratio);
                rectangle.X = context.Bounds.X + (context.Bounds.Width - rectangle.Width) / 2;
                rectangle.Y = context.Bounds.Y + (context.Bounds.Height - rectangle.Height) / 2;
                context.Graphics.DrawImage(image, rectangle);
            }
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var image = GetIcon(node);
            return image == null ? Size.Empty : image.Size;
        }

        private Image GetIcon(TreeNodeAdv node)
        {
            var obj = node.Tag;
            if (obj is NbtFile)
                return Properties.Resources.file_image;
            if (obj is NbtFolder)
                return Properties.Resources.folder_image;
            if (obj is NbtTag tag)
                return Util.TagTypeImage(tag.TagType);
            return null;
        }
    }

    public class NbtText : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var text = GetText(node);
            if (text != null)
            {
                var size = MeasureSize(node, context);
                Point point = new Point(context.Bounds.X, context.Bounds.Y + (context.Bounds.Height - size.Height) / 2);
                context.Graphics.DrawString(text, context.Font, new SolidBrush(Parent.ForeColor), point);
            }
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var text = GetText(node);
            return text == null ? Size.Empty : TextRenderer.MeasureText(text, context.Font);
        }

        private string GetText(TreeNodeAdv node)
        {
            var obj = node.Tag;
            if (obj is NbtFile file)
                return Util.PreviewNbtValue(file);
            if (obj is NbtFolder folder)
                return Util.PreviewNbtValue(folder);
            if (obj is NbtTag tag)
                return Util.PreviewNbtValue(tag);
            return null;
        }
    }
}
