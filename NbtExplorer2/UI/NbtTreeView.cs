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
            NodeControls.Add(new NbtText());
            this.RowHeight = 20;
            this.SelectionMode = TreeSelectionMode.Multi;
        }

        private void ToggleExpansion(TreeNodeAdv node, bool all = false)
        {
            if (node.IsExpanded)
                node.Collapse();
            else
            {
                if (all)
                    node.ExpandAll();
                else
                    node.Expand();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Clicks == 2)
                base.OnMouseDoubleClick(e); // toggle expansion
            base.OnMouseDown(e);
        }

        // this only fires when the mouse is released after two clicks, which feels laggy
        // so just disable the native behavior and reimplement it with OnMouseDown
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        { }


        private TreeNodeAdv LastDragDestination;
        private DateTime LastDragDestinationTime;
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            // expand nodes we hover over while drag and dropping
            if (DropPosition.Node != LastDragDestination)
            {
                LastDragDestination = DropPosition.Node;
                LastDragDestinationTime = DateTime.Now;
            }
            else if (DropPosition.Node != null && DropPosition.Position == NodePosition.Inside)
            {
                TimeSpan hover_time = DateTime.Now.Subtract(LastDragDestinationTime);
                if (hover_time.TotalSeconds > 0.5)
                {
                    // don't expand the node we're dragging itself
                    var nodes = NodesFromDrag(drgevent);
                    if (nodes != null && !nodes.Contains(DropPosition.Node))
                        DropPosition.Node.Expand();
                }
            }
            base.OnDragOver(drgevent);
        }

        private IEnumerable<object> NodesFromDrag(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNodeAdv[])))
                return null;
            return (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (SelectedNode != null)
            {
                // space to toggle collapsed/expanded
                if (keyData == Keys.Space)
                {
                    ToggleExpansion(SelectedNode);
                    return true;
                }
                // control-space to expand all
                if (keyData == (Keys.Space | Keys.Control))
                {
                    ToggleExpansion(SelectedNode, true);
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
            NbtText.DrawSelection(node, context);
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
        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            var halves = GetText(node);
            if (halves != null)
            {
                var size = MeasureSize(node, context);
                Point point = new Point(context.Bounds.X, context.Bounds.Y + (context.Bounds.Height - size.Height) / 2);
                DrawSelection(node, context);
                var boldfont = new Font(context.Font, FontStyle.Bold);
                if (halves.Item1 != null)
                {
                    context.Graphics.DrawString(halves.Item1, boldfont, new SolidBrush(Parent.ForeColor), point);
                    point.X += TextRenderer.MeasureText(halves.Item1, boldfont).Width;
                }
                context.Graphics.DrawString(halves.Item2, context.Font, new SolidBrush(Parent.ForeColor), point);
            }
        }

        public static void DrawSelection(TreeNodeAdv node, DrawContext context)
        {
            // selected nodes are not "active" while dragging
            // hovered nodes are "active" while dragging
            if (context.DrawSelection == DrawSelectionMode.Active)
                context.Graphics.FillRectangle(Brushes.LightBlue, context.Bounds);
            else if (node.IsSelected)
                context.Graphics.FillRectangle(Brushes.LightYellow, context.Bounds);
        }

        public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
        {
            var halves = GetText(node);
            if (halves == null)
                return Size.Empty;
            var boldfont = new Font(context.Font, FontStyle.Bold);
            Size s1 = halves.Item1 == null ? Size.Empty : TextRenderer.MeasureText(halves.Item1, boldfont);
            Size s2 = TextRenderer.MeasureText(halves.Item2, context.Font);
            return new Size(s1.Width + s2.Width, Math.Max(s1.Height, s2.Height));
        }

        private Tuple<string, string> GetText(TreeNodeAdv node)
        {
            var obj = node.Tag;
            return INbt.PreviewNameAndValue(obj);
        }
    }
}
