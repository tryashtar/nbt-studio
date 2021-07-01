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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public class NbtTreeView : TreeViewAdv
    {
        private readonly NbtIcon IconControl = new NbtIcon();
        private readonly NbtText TextControl = new NbtText();
        public NbtTreeView()
        {
            NodeControls.Add(IconControl);
            NodeControls.Add(TextControl);
            this.RowHeight = 20;
            this.SelectionMode = TreeSelectionMode.Multi;
            this.Collapsing += NbtTreeView_Collapsing;
            this.FontChanged += NbtTreeView_FontChanged;
            this.LoadOnDemand = true;
            this.ShowNodeToolTips = true;
        }

        public void SetIconSource(IconSource source)
        {
            IconControl.IconSource = source;
        }

        public INode SelectedINode => SelectedNode?.Tag as INode;
        public IEnumerable<INode> SelectedINodes => this.SelectedNodes.Select(x => x.Tag).OfType<INode>();

        public IEnumerable<INode> INodesFromDrag(DragEventArgs e)
        {
            return NodesFromDrag(e).Select(x => x.Tag).OfType<INode>();
        }
        public INode INodeFromClick(TreeNodeAdvMouseEventArgs e)
        {
            return e.Node.Tag as INode;
        }
        public INode DropINode => DropPosition.Node?.Tag as INode;

        private void NbtTreeView_FontChanged(object sender, EventArgs e)
        {
            // make sure rows are high enough to see tall/low letters
            this.RowHeight = TextRenderer.MeasureText("fyWM", this.Font).Height + 6;
        }

        protected override void OnNodesInserted(TreeNodeAdv parent, TreeModelEventArgs e)
        {
            base.OnNodesInserted(parent, e);
            parent.Expand();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (SelectedNodes.Any())
            {
                string text = $"{StringUtils.Pluralize(SelectedNodes.Count, "item")} selected";
                var size = e.Graphics.MeasureString(text, this.Font).ToSize();
                var format = new StringFormat { Alignment = StringAlignment.Far };
                int y_buffer = 5;
                int x_buffer = 10;
                var rectangle = new Rectangle(DisplayRectangle.Width - size.Width - x_buffer, DisplayRectangle.Height - size.Height - y_buffer, size.Width + 3, size.Height + 3);
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), rectangle);
                e.Graphics.DrawString(text, this.Font, Brushes.Gray, rectangle, format);
            }
        }

        protected override void OnModelChanged()
        {
            base.OnModelChanged();
            foreach (var item in Root.Children)
            {
                item.Expand();
            }
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
            else if (DropPosition.Node is not null && DropPosition.Position == NodePosition.Inside)
            {
                TimeSpan hover_time = DateTime.Now.Subtract(LastDragDestinationTime);
                if (hover_time.TotalSeconds > 0.5)
                {
                    // don't expand the node we're dragging itself
                    var nodes = NodesFromDrag(drgevent);
                    if (nodes is not null && !nodes.Contains(DropPosition.Node))
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

        public IEnumerable<TreeNodeAdv> AllChildren(TreeNodeAdv node)
        {
            var children = ForceChildren(node);
            foreach (var child in children)
            {
                yield return child;
                foreach (var grandchild in AllChildren(child))
                {
                    yield return grandchild;
                }
            }
        }

        private ReadOnlyCollection<TreeNodeAdv> ForceChildren(TreeNodeAdv node)
        {
            if (!node.IsExpandedOnce && !node.IsLeaf && node.Children.Count == 0)
            {
                node.Expand();
                node.Collapse();
            }
            return node.Children;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (SelectedNode is not null)
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
                    if (SelectedNode.Parent.Parent is not null) // this seems weird but is correct
                    {
                        SelectedNode = SelectedNode.Parent;
                        return true;
                    }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
