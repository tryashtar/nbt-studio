using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            this.Collapsing += NbtTreeView_Collapsing;
            this.LoadOnDemand = true;
        }

        private void NbtTreeView_Collapsing(object sender, TreeViewAdvEventArgs e)
        {
            this.Collapsing -= NbtTreeView_Collapsing;
            e.Node.CollapseAll();
            this.Collapsing += NbtTreeView_Collapsing;
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
            if (e.Clicks == 2 && e.Button == MouseButtons.Left)
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

        public TreeNodeAdv[] NodesFromDrag(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNodeAdv[])))
                return new TreeNodeAdv[0];
            return (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
        }

        public IEnumerable<TreeNodeAdv> BreadthFirstSearch() => BreadthFirstSearch(x => true);
        public IEnumerable<TreeNodeAdv> BreadthFirstSearch(Predicate<TreeNodeAdv> predicate)
        {
            var queue = new Queue<TreeNodeAdv>();
            queue.Enqueue(Root);
            while (queue.Any())
            {
                var item = queue.Dequeue();
                if (item != Root && !predicate(item))
                    continue;
                yield return item;
                foreach (var sub in item.Children)
                {
                    queue.Enqueue(sub);
                }
            }
        }
        public IEnumerable<TreeNodeAdv> DepthFirstSearch() => DepthFirstSearch(x => true);
        public IEnumerable<TreeNodeAdv> DepthFirstSearch(Predicate<TreeNodeAdv> predicate)
        {
            var stack = new Stack<TreeNodeAdv>();
            stack.Push(Root);
            while (stack.Any())
            {
                var item = stack.Pop();
                if (item != Root && !predicate(item))
                    continue;
                yield return item;
                foreach (var sub in item.Children.Reverse())
                {
                    stack.Push(sub);
                }
            }
        }

        private IEnumerable<TreeNodeAdv> AllChildren(TreeNodeAdv start)
        {
            if (!start.IsExpandedOnce)
            {
                start.IsExpanded = true;
                start.IsExpanded = false;
            }
            foreach (var item in start.Children)
            {
                yield return item;
                foreach (var sub in AllChildren(item))
                {
                    yield return sub;
                }
            }
        }

        private IEnumerable<TreeNodeAdv> AllChildrenReversed(TreeNodeAdv start)
        {
            if (!start.IsExpandedOnce)
            {
                start.IsExpanded = true;
                start.IsExpanded = false;
            }
            foreach (var item in start.Children.Reverse())
            {
                foreach (var sub in AllChildrenReversed(item))
                {
                    yield return sub;
                }
                yield return item;
            }
        }

        private IEnumerable<TreeNodeAdv> AllSuccessors(TreeNodeAdv start)
        {
            foreach (var item in SucceedingNodes(start))
            {
                yield return item;
                foreach (var sub in AllChildren(item))
                {
                    yield return sub;
                }
            }
        }

        private IEnumerable<TreeNodeAdv> AllPredecessors(TreeNodeAdv start)
        {
            foreach (var item in PrecedingNodes(start))
            {
                foreach (var sub in AllChildrenReversed(item))
                {
                    yield return sub;
                }
                yield return item;
            }
        }

        private IEnumerable<TreeNodeAdv> SearchForward(TreeNodeAdv start)
        {
            foreach (var item in AllChildren(start))
            {
                yield return item;
            }
            foreach (var item in AllSuccessors(start))
            {
                yield return item;
            }
            foreach (var item in Ancestors(start))
            {
                foreach (var sub in AllSuccessors(item))
                {
                    yield return sub;
                }
            }
        }

        private IEnumerable<TreeNodeAdv> SearchBackward(TreeNodeAdv start)
        {
            foreach (var item in AllPredecessors(start))
            {
                yield return item;
            }
            foreach (var item in Ancestors(start))
            {
                yield return item;
                foreach (var sub in AllPredecessors(item))
                {
                    yield return sub;
                }
            }
        }

        public TreeNodeAdv SearchFrom(TreeNodeAdv start, Predicate<TreeNodeAdv> predicate, bool forward)
        {
            var search = forward ? SearchForward(start) : SearchBackward(start);
            foreach (var item in search)
            {
                if (predicate(item))
                    return item;
            }
            return null;
        }

        public TreeNodeAdv FinalNode
        {
            get
            {
                var current = Root;
                while (true)
                {
                    if (!current.IsExpandedOnce)
                    {
                        current.IsExpanded = true;
                        current.IsExpanded = false;
                    }
                    if (current.Children.Count == 0)
                        return current;
                    current = current.Children.Last();
                }
            }
        }

        private IEnumerable<TreeNodeAdv> SucceedingNodes(TreeNodeAdv start)
        {
            while (start.NextNode != null)
            {
                start = start.NextNode;
                yield return start;
            }
        }

        private IEnumerable<TreeNodeAdv> PrecedingNodes(TreeNodeAdv start)
        {
            while (start.PreviousNode != null)
            {
                start = start.PreviousNode;
                yield return start;
            }
        }

        private IEnumerable<TreeNodeAdv> Ancestors(TreeNodeAdv bottom)
        {
            while (bottom.Parent != null)
            {
                bottom = bottom.Parent;
                yield return bottom;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (SelectedNode != null)
            {
                // space to toggle collapsed/expanded
                if (keyData == Keys.Space)
                {
                    foreach (var item in SelectedNodes)
                    {
                        ToggleExpansion(item);
                    }
                    return true;
                }
                // control-space to expand all
                if (keyData == (Keys.Space | Keys.Control))
                {
                    foreach (var item in SelectedNodes)
                    {
                        ToggleExpansion(item, true);
                    }
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
            return GetImage(node.Tag);
        }

        private static Image GetImage(object obj)
        {
            if (obj is NbtFile)
                return Properties.Resources.file_image;
            if (obj is NbtFolder)
                return Properties.Resources.folder_image;
            if (obj is RegionFile)
                return Properties.Resources.region_image;
            if (obj is Chunk)
                return Properties.Resources.chunk_image;
            if (obj is NbtTag tag)
                return NbtUtil.TagTypeImage(tag.TagType);
            return null;
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
                PointF point = new PointF(context.Bounds.X, context.Bounds.Y + (context.Bounds.Height - size.Height) / 2);
                DrawSelection(node, context);
                var boldfont = new Font(context.Font, FontStyle.Bold);
                if (halves.Item1 != null)
                {
                    context.Graphics.DrawString(halves.Item1, boldfont, new SolidBrush(Parent.ForeColor), point);
                    point.X += context.Graphics.MeasureString(halves.Item1, boldfont).Width;
                }
                context.Graphics.DrawString(halves.Item2, context.Font, new SolidBrush(Parent.ForeColor), point);
            }
        }

        public static void DrawSelection(TreeNodeAdv node, DrawContext context)
        {
            // selected nodes are not "active" while dragging
            // hovered nodes are "active" while dragging
            if (context.DrawSelection == DrawSelectionMode.Active || (node.IsSelected && !node.Tree.Focused))
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
            SizeF s1 = halves.Item1 == null ? SizeF.Empty : context.Graphics.MeasureString(halves.Item1, boldfont);
            SizeF s2 = context.Graphics.MeasureString(halves.Item2, context.Font);
            return new Size((int)Math.Round(s1.Width + s2.Width), (int)Math.Ceiling(Math.Max(s1.Height, s2.Height)));
        }

        public static Tuple<string, string> GetText(TreeNodeAdv node)
        {
            var obj = node.Tag;
            var text = PreviewNameAndValue(obj);
            return Tuple.Create(Flatten(text.Item1), Flatten(text.Item2));
        }

        private static Tuple<string, string> PreviewNameAndValue(object obj)
        {
            string name = PreviewName(obj);
            string value = PreviewValue(obj);
            if (name == null)
                return Tuple.Create((string)null, value);
            return Tuple.Create(name + ": ", value);
        }

        private static string PreviewName(object obj)
        {
            if (obj is NbtFile file)
                return Path.GetFileName(file.Path);
            if (obj is NbtFolder folder)
                return Path.GetFileName(folder.Path);
            if (obj is RegionFile region)
                return Path.GetFileName(region.Path);
            if (obj is Chunk chunk)
                return $"Chunk [{chunk.X}, {chunk.Z}]";
            if (obj is NbtTag tag)
                return tag.Name;
            return null;
        }

        private static string PreviewValue(object obj)
        {
            if (obj is NbtFile file)
                return NbtUtil.PreviewNbtValue(file.RootTag.Adapt());
            if (obj is NbtFolder folder)
            {
                if (folder.HasScanned)
                {
                    if (folder.Subfolders.Any())
                        return $"[{Util.Pluralize(folder.Subfolders.Count, "folder")}, {Util.Pluralize(folder.Files.Count, "file")}]";
                    else
                        return $"[{Util.Pluralize(folder.Files.Count, "file")}]";
                }
                else
                    return "(open to load)";
            }
            if (obj is RegionFile region)
                return $"[{Util.Pluralize(region.ChunkCount, "chunk")}]";
            if (obj is Chunk chunk)
            {
                if (chunk.IsLoaded)
                    return NbtUtil.PreviewNbtValue(chunk.Data.Adapt());
                else
                    return "(open to load)";
            }
            if (obj is NbtTag tag)
                return NbtUtil.PreviewNbtValue(tag.Adapt());
            return null;
        }

        private static string Flatten(string text)
        {
            if (text == null) return null;
            return text.Replace("\n", "⏎").Replace("\r", "⏎");
        }
    }
}
