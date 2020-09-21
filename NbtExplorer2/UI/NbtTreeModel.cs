using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtExplorer2.UI
{
    public partial class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public bool HasUnsavedChanges { get; private set; } = false;
        private readonly IEnumerable<object> Roots;
        private readonly NbtTreeView View;

        public INotifyNode SelectedObject
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return NotifyWrap(this, View.SelectedNode.Tag, View.SelectedNode.Parent?.Tag);
            }
        }
        public IEnumerable<INotifyNode> SelectedObjects
        {
            get
            {
                if (View.SelectedNodes == null)
                    return Enumerable.Empty<INotifyNode>();
                return View.SelectedNodes.Select(x => NotifyWrap(this, x.Tag, x.Parent?.Tag));
            }
        }
        public INotifyNbt SelectedNbt
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return NotifyWrapNbt(this, View.SelectedNode.Tag, View.SelectedNode.Parent?.Tag, INbt.GetNbt(View.SelectedNode.Tag));
            }
        }
        public IEnumerable<INotifyNbt> SelectedNbts
        {
            get
            {
                if (View.SelectedNodes == null)
                    Enumerable.Empty<INotifyNbt>();
                return View.SelectedNodes.Select(x => NotifyWrapNbt(this, x.Tag, x.Parent?.Tag, INbt.GetNbt(x.Tag))).Where(x => x != null);
            }
        }

        public NbtTreeModel(IEnumerable<object> roots, NbtTreeView view)
        {
            Roots = roots;
            View = view;
            View.Model = this;
            // expand all top-level objects
            foreach (var item in View.Root.Children)
            {
                item.Expand();
            }
        }
        public NbtTreeModel(object root, NbtTreeView view) : this(new[] { root }, view) { }

        public IEnumerable<INotifyNbt> NbtsFromDrag(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(TreeNodeAdv[])))
                return Enumerable.Empty<INotifyNbt>();
            return ((TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]))).Select(x => NotifyWrapNbt(this, x.Tag, x.Parent?.Tag, INbt.GetNbt(x.Tag))).Where(x => x != null);
        }
        public INbtTag DropTag
        {
            get
            {
                if (View.DropPosition.Node == null)
                    return null;
                return NotifyWrapNbt(this, View.DropPosition.Node.Tag, View.DropPosition.Node.Parent?.Tag, INbt.GetNbt(View.DropPosition.Node.Tag));
            }
        }
        public NodePosition DropPosition => View.DropPosition.Position;

        // an object changed, refresh the nodes through ITreeModel's API to ensure it matches the true object
        private void Notify(object changed)
        {
#if DEBUG
            Console.WriteLine($"changed: {changed.GetType()}");
#endif
            var path = GetPath(changed);
            if (path == null) return;
            HasUnsavedChanges = true;
            var node = View.FindNodeByTag(changed);

            var real_children = GetChildren(path).ToList();
            var current_children = node == null ? new TreeNodeAdv[0] : node.Children.Select(x => x.Tag).ToArray();
            var remove = current_children.Except(real_children).ToArray();
            var add = real_children.Except(current_children).ToArray();

            NodesChanged?.Invoke(this, new TreeModelEventArgs(path, real_children.ToArray()));
            if (remove.Any())
                NodesRemoved?.Invoke(this, new TreeModelEventArgs(path, remove));
            if (add.Any())
                NodesInserted?.Invoke(this, new TreeModelEventArgs(path, add.Select(x => real_children.IndexOf(x)).ToArray(), add));
        }

        private TreePath GetPath(object item)
        {
            var node = View.FindNodeByTag(item);
            if (node != null)
                return View.GetPath(node);
            return GetPathSlow(item);
        }

        private TreePath GetPathSlow(object item)
        {
#if DEBUG
            Console.WriteLine($"Slowly looking for {item.GetType()}");
#endif
            var queue = new Queue<TreePath>();
            queue.Enqueue(new TreePath());
            while (queue.Any())
            {
                var current = queue.Dequeue();
                if (current.LastNode == item)
                    return current;
                foreach (var child in GetChildren(current))
                {
                    queue.Enqueue(new TreePath(current, child));
                }
            }
            return null;
        }

        IEnumerable ITreeModel.GetChildren(TreePath treePath) => GetChildren(treePath);
        public IEnumerable<object> GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Roots;
            else
                return GetChildren(treePath.LastNode);
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !HasChildren(treePath.LastNode);
        }

        private IEnumerable<object> GetChildren(object obj)
        {
            var children = INbt.GetChildren(obj);
            return children;
        }

        private bool HasChildren(object obj)
        {
            var children = GetChildren(obj);
            return children != null && children.Any();
        }
    }
}
