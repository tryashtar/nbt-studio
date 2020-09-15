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
    public class NbtTreeView : TreeViewAdv
    {
        public NbtTreeView()
        {
            NodeControls.Add(new NbtIcon());
            NodeControls.Add(new NbtText(':'));
            this.RowHeight = 20;
            this.SelectionMode = TreeSelectionMode.Multi;
        }

        public object SelectedObject => SelectedNode?.Tag;
        public IEnumerable<object> SelectedObjects => SelectedNodes?.Select(x => x.Tag);

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (SelectedNode != null)
            {
                // space to toggle collapsed/expanded
                if (keyData == Keys.Space)
                {
                    if (SelectedNode.IsExpanded)
                        SelectedNode.Collapse();
                    else
                        SelectedNode.Expand();
                    return true;
                }
                // control-space to expand all
                if (keyData == (Keys.Space | Keys.Control))
                {
                    if (SelectedNode.IsExpanded)
                        SelectedNode.Collapse();
                    else
                        SelectedNode.ExpandAll();
                    return true;
                }
                // control-up to select parent
                if (keyData == (Keys.Up | Keys.Control))
                {
                    if (SelectedNode.Parent.Parent != null) // this seems weird but is correct
                    {
                        SelectedNode = SelectedNode.Parent;
                        return true;
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public class NbtIcon : NodeControl
    {
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            NbtText.DrawSelection(context);
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
            return INbt.Image(obj);
        }
    }

    public class NbtText : NodeControl
    {
        char? BoldBefore;
        public NbtText(char? bold_before)
        {
            BoldBefore = bold_before;
        }

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var text = GetText(node);
            if (text != null)
            {
                var size = MeasureSize(node, context);
                Point point = new Point(context.Bounds.X, context.Bounds.Y + (context.Bounds.Height - size.Height) / 2);
                DrawSelection(context);
                var halves = GetTextHalves(text);
                var boldfont = new Font(context.Font, FontStyle.Bold);
                context.Graphics.DrawString(halves[0], boldfont, new SolidBrush(Parent.ForeColor), point);
                point.X += TextRenderer.MeasureText(halves[0], boldfont).Width;
                context.Graphics.DrawString(halves[1], context.Font, new SolidBrush(Parent.ForeColor), point);
            }
        }

        public static void DrawSelection(DrawContext context)
        {
            if (context.DrawSelection == DrawSelectionMode.Active)
                context.Graphics.FillRectangle(Brushes.LightBlue, context.Bounds);
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var text = GetText(node);
            if (text == null)
                return Size.Empty;
            var halves = GetTextHalves(text);
            var boldfont = new Font(context.Font, FontStyle.Bold);
            Size s1 = TextRenderer.MeasureText(halves[0], boldfont);
            Size s2 = TextRenderer.MeasureText(halves[1], context.Font);
            return new Size(s1.Width + s2.Width, Math.Max(s1.Height, s2.Height));
        }

        private string GetText(TreeNodeAdv node)
        {
            var obj = node.Tag;
            return INbt.PreviewNameAndValue(obj);
        }

        private string[] GetTextHalves(string text)
        {
            if (BoldBefore.HasValue && text.Contains(BoldBefore.Value))
            {
                var split = text.Split(BoldBefore.Value);
                split[0] += BoldBefore.Value;
                return split;
            }
            return new[] { "", text };
        }
    }
}
