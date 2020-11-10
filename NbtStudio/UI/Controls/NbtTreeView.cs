using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using fNbt;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ReadOnlyCollection<TreeNodeAdv> ForceChildren(TreeNodeAdv node)
        {
            if (!node.IsExpandedOnce && !node.IsLeaf && node.Children.Count == 0)
            {
                node.ExpandAll();
                node.CollapseAll();
            }
            return node.Children;
        }

        public TreeNodeAdv NextNode(TreeNodeAdv node)
        {
            var children = ForceChildren(node);
            if (children.Count > 0)
                return children.First();
            TreeNodeAdv next = null;
            while (next == null && node != null)
            {
                next = node.NextNode;
                if (next == null)
                    node = node.Parent;
            }
            return next;
        }

        public TreeNodeAdv PreviousNode(TreeNodeAdv node)
        {
            var prev = node.PreviousNode;
            if (prev == null)
                return node.Parent;
            while (prev != null)
            {
                var children = ForceChildren(prev);
                if (children.Count == 0)
                    return prev;
                prev = children[children.Count - 1];
            }
            return null;
        }

        public TreeNodeAdv SearchFrom(TreeNodeAdv start, Predicate<TreeNodeAdv> predicate, SearchDirection direction, IProgress<TreeSearchReport> progress, bool wrap)
        {
            if (direction == SearchDirection.Forward)
            {
                var first = Root.Children.First();
                if (start == null)
                    start = first;
                else
                    start = NextNode(start);
                return SearchFromNext(start, predicate, NextNode, progress, new TreeSearchReport(), wrap ? first : null);
            }
            else
            {
                var last = FinalNode;
                if (start == null)
                    start = last;
                else
                    start = PreviousNode(start);
                return SearchFromNext(start, predicate, PreviousNode, progress, new TreeSearchReport(), wrap ? last : null);
            }
        }

        private TreeNodeAdv SearchFromNext(TreeNodeAdv node, Predicate<TreeNodeAdv> predicate, Func<TreeNodeAdv, TreeNodeAdv> next, IProgress<TreeSearchReport> progress, TreeSearchReport report, TreeNodeAdv wrap_start)
        {
            var start = node;
            report.TotalNodes = this.AllNodes.Count();
            while (node != null && !predicate(node))
            {
                node = next(node);
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = this.AllNodes.Count();
                    progress.Report(report);
                }
            }
            if (node != null && node != Root)
                return node;
            if (wrap_start == null)
                return null;

            // search again from new starting point, until reaching original starting point
            node = SearchFromNext(wrap_start, x => x == start || predicate(x), next, progress, report, null);
            if (node == start)
                return null;
            else
                return node;
        }

        public IEnumerable<TreeNodeAdv> SearchAll(Predicate<TreeNodeAdv> predicate, IProgress<TreeSearchReport> progress)
        {
            var report = new TreeSearchReport();
            report.TotalNodes = this.AllNodes.Count();
            var node = Root.Children.First();
            while (node != null)
            {
                node = NextNode(node);
                if (node != null && predicate(node))
                    yield return node;
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = this.AllNodes.Count();
                    progress.Report(report);
                }
            }
        }

        public TreeNodeAdv FinalNode
        {
            get
            {
                var current = Root;
                while (true)
                {
                    var children = ForceChildren(current);
                    if (children.Count == 0)
                        return current;
                    current = current.Children.Last();
                }
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

    public class TreeSearchReport
    {
        public int NodesSearched;
        public int TotalNodes;
        public decimal Percentage => (decimal)NodesSearched / TotalNodes;
    }
}
