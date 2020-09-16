using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2.UI
{
    public class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public bool HasUnsavedChanges { get; private set; } = false;
        public readonly IEnumerable<object> Roots;
        private readonly NbtTreeView View;
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

        public void Remove(object obj)
        {
            INbt.Delete(obj);
            NodesRemoved?.Invoke(this, new TreeModelEventArgs(GetParentPath(obj), new[] { obj }));
            HasUnsavedChanges = true;
        }

        public void RemoveAll(IEnumerable<object> objects)
        {
            foreach (var item in objects.ToList())
            {
                Remove(item);
            }
        }

        public void Add(object parent, NbtTag tag)
        {
            INbt.Add(parent, tag);
            NodesInserted?.Invoke(this, new TreeModelEventArgs(GetPath(parent), new[] { INbt.IndexOf(parent, tag) }, new[] { tag }));
            View.FindNodeByTag(parent).Expand();
            //View.EnsureVisible(View.FindNodeByTag(tag));
            HasUnsavedChanges = true;
        }

        private Tuple<object, int> GetInsertionLocation(object target, NodePosition position)
        {
            var node = View.FindNodeByTag(target);
            if (node == null)
                throw new ArgumentException("Couldn't find target object");
            if (position == NodePosition.Inside)
                return Tuple.Create(target, node.Children.Count);
            else
            {
                int index = node.Parent.Children.IndexOf(node);
                if (position == NodePosition.After)
                    index++;
                return Tuple.Create(node.Parent.Tag, index);
            }
        }

        public bool CanMove(IEnumerable<object> items, object target, NodePosition placement)
        {
            var insertion = GetInsertionLocation(target, placement);
            var path = GetPath(insertion.Item1).FullPath;
            foreach (var item in items)
            {
                // can't become the child of your own descendent
                if (path.Contains(item))
                    return false;
            }
            return INbt.CanDropAll(items, insertion.Item1, insertion.Item2);
        }

        public void Move(IEnumerable<object> items, object target, NodePosition placement)
        {
            var insertion = GetInsertionLocation(target, placement);
            var path = GetPath(insertion.Item1);
            INbt.DropAll(items, insertion.Item1, insertion.Item2);
            foreach (var item in items)
            {
                NodesRemoved?.Invoke(this, new TreeModelEventArgs(GetParentPath(item), new[] { item }));
                // only works for tags right now
                NodesInserted?.Invoke(this, new TreeModelEventArgs(path, new[] { INbt.IndexOf(insertion.Item1, (NbtTag)item) }, new[] { item }));
            }
            HasUnsavedChanges = true;
        }

        public void NoticeChanges(object obj)
        {
            // currently, changes seem to be reflected without needing to raise NodesChanged
            HasUnsavedChanges = true;
        }

        private TreePath GetPath(object item)
        {
            return View.GetPath(View.FindNodeByTag(item));
        }

        private TreePath GetParentPath(object item)
        {
            return View.GetPath(View.FindNodeByTag(item).Parent);
        }

        public IEnumerable GetChildren(TreePath treePath)
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
