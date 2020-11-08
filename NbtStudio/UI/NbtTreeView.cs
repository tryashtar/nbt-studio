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

namespace NbtStudio.UI
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
            this.FontChanged += NbtTreeView_FontChanged;
            this.LoadOnDemand = true;
        }

        private void NbtTreeView_FontChanged(object sender, EventArgs e)
        {
            this.RowHeight = TextRenderer.MeasureText("fyWM", this.Font).Height + 6;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                float delta = (e.Delta > 0 ? 2f : -2f);
                this.Font = new Font(this.Font.FontFamily, Math.Max(3, Math.Min(99, this.Font.Size + delta)));
            }
            else
                base.OnMouseWheel(e);
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

        public TreeNodeAdv SearchFrom(TreeNodeAdv start, Predicate<TreeNodeAdv> predicate, SearchDirection direction)
        {
            var search = direction == SearchDirection.Forward ? SearchForward(start) : SearchBackward(start);
            foreach (var item in search)
            {
                if (predicate(item))
                    return item;
            }
            return null;
        }

        public IEnumerable<TreeNodeAdv> Search(Predicate<TreeNodeAdv> predicate)
        {
            foreach (var item in SearchForward(Root))
            {
                if (predicate(item))
                    yield return item;
            }
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

    public enum SearchDirection
    {
        Forward,
        Backward
    }
}
