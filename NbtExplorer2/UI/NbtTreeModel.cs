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
        public event EventHandler Changed;

        private bool _HasUnsavedChanges = false;
        public bool HasUnsavedChanges { get => _HasUnsavedChanges; private set { _HasUnsavedChanges = value; Changed?.Invoke(this, EventArgs.Empty); } }
        private readonly IEnumerable<object> Roots;
        private readonly NbtTreeView View;

        public INotifyNode SelectedObject
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return NotifyWrap(this, View.SelectedNode.Tag);
            }
        }
        public IEnumerable<INotifyNode> SelectedObjects
        {
            get
            {
                if (View.SelectedNodes == null)
                    return Enumerable.Empty<INotifyNode>();
                return View.SelectedNodes.Select(x => NotifyWrap(this, x.Tag));
            }
        }
        public INotifyNbt SelectedNbt
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return NotifyWrapNbt(this, View.SelectedNode.Tag, NbtUtil.GetNbt(View.SelectedNode.Tag));
            }
        }
        public IEnumerable<INotifyNbt> SelectedNbts
        {
            get
            {
                if (View.SelectedNodes == null)
                    Enumerable.Empty<INotifyNbt>();
                return View.SelectedNodes.Select(x => NotifyWrapNbt(this, x.Tag, NbtUtil.GetNbt(x.Tag))).Where(x => x != null);
            }
        }
        public IEnumerable<NotifyNbtFile> OpenedFiles
        {
            get
            {
                var queue = new Queue<TreeNodeAdv>();
                foreach (var item in View.Root.Children)
                {
                    queue.Enqueue(item);
                }
                while (queue.Any())
                {
                    var item = queue.Dequeue();
                    if (item.Tag is NbtFile file)
                        yield return NotifyWrapFile(this, file);
                    else if (item.Tag is NbtFolder)
                    {
                        foreach (var sub in item.Children)
                        {
                            queue.Enqueue(sub);
                        }
                    }
                }
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
            return ((TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]))).Select(x => NotifyWrapNbt(this, x.Tag, NbtUtil.GetNbt(x.Tag))).Where(x => x != null);
        }
        public INbtTag DropTag
        {
            get
            {
                if (View.DropPosition.Node == null)
                    return null;
                return NotifyWrapNbt(this, View.DropPosition.Node.Tag, NbtUtil.GetNbt(View.DropPosition.Node.Tag));
            }
        }
        public NodePosition DropPosition => View.DropPosition.Position;

        // an object changed, refresh the nodes through ITreeModel's API to ensure it matches the true object
        private void Notify(object changed)
        {
#if DEBUG
            if (changed != null)
                Console.WriteLine($"changed: {changed.GetType()}");
#endif
            var node = FindNodeByObject(changed);
            if (node == null) return;
            var path = View.GetPath(node);
            HasUnsavedChanges = true;

            var real_children = GetChildren(path).ToList();
            var current_children = node == null ? new TreeNodeAdv[0] : node.Children.Select(x => x.Tag).ToArray();
            var remove = current_children.Except(real_children).ToArray();
            var add = real_children.Except(current_children).ToArray();

            NodesChanged?.Invoke(this, new TreeModelEventArgs(path, real_children.ToArray()));
            if (remove.Any())
                NodesRemoved?.Invoke(this, new TreeModelEventArgs(path, remove));
            if (add.Any())
            {
                node.Expand();
                NodesInserted?.Invoke(this, new TreeModelEventArgs(path, add.Select(x => real_children.IndexOf(x)).ToArray(), add));
            }
        }

        private TreeNodeAdv FindNodeByObject(object obj)
        {
            var quick = View.FindNodeByTag(obj);
            if (quick != null)
                return quick;
            // breadth-first search, scan tree for the object
            var queue = new Queue<TreeNodeAdv>();
            queue.Enqueue(View.Root);
            while (queue.Any())
            {
                var item = queue.Dequeue();
                // notifiers can't tell whether they were added to a file that's being treated as a compound
                // so here we disambiguate them
                if (item.Tag is NbtFile file && file.RootTag == obj)
                    return item;
                foreach (var sub in item.Children)
                {
                    queue.Enqueue(sub);
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
            if (obj is NbtFolder folder)
                return folder.Subfolders.Concat<object>(folder.Files);
            if (obj is NbtFile file)
                return file.RootTag.Tags;
            if (obj is NbtCompound compound)
                return compound.Tags;
            if (obj is NbtList list)
                return list;
            return Enumerable.Empty<object>();
        }

        private bool HasChildren(object obj)
        {
            var children = GetChildren(obj);
            return children != null && children.Any();
        }
    }
}
