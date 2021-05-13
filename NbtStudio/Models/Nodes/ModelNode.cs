using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    // the main implementation of INode
    // this node does not wrap an object, but allows derived classes to do so easily
    // type T is the type of the children, e.g. RegionNode is ModelNode<Chunk>
    public abstract class ModelNode<T> : IModelNode, IDisposable
    {
        private readonly NbtTreeModel Tree;
        public INode Parent { get; private set; }
        private bool ChildrenReady = false;
        private OrderedDictionary<T, INode> ChildNodes;
        public IReadOnlyList<INode> Children
        {
            get
            {
                // some nodes call RefreshChildren when calling GetChildren (especially for the first time)
                // so use this flag to make sure we don't enter this section twice
                if (!ChildrenReady)
                {
                    ChildNodes = new OrderedDictionary<T, INode>();
                    var children = GetChildren().Where(x => x is not null);
                    foreach (var item in children)
                    {
                        var node = NodeRegistry.CreateNode(Tree, this, item);
                        ChildNodes.Add(item, node);
                        int plus = node.DescendantsCount + 1;
                        UpdateDescendantsCount(x => x + plus);
                    }
                    ChildrenReady = true;
                }
                return ChildNodes.Values.ToList();
            }
        }

        // default implementation fetches children, this can be overridden to defer it to later
        public virtual bool HasChildren => Children.Any();

        public int DescendantsCount { get; private set; } = 0;
        public void SetDescendantsCount(int value)
        {
            DescendantsCount = value;
        }
        private void UpdateDescendantsCount(Func<int, int> apply)
        {
            IModelNode item = this;
            while (item is not null)
            {
                item.SetDescendantsCount(apply(item.DescendantsCount));
                item = item.Parent as IModelNode;
            }
        }

        public TreePath Path
        {
            get
            {
                var path = new List<INode>();
                INode item = this;
                while (item is not null && item is not ModelRootNode)
                {
                    path.Add(item);
                    item = item.Parent;
                }
                path.Reverse();
                return new TreePath(path.ToArray());
            }
        }

        public ModelNode(NbtTreeModel tree, INode parent)
        {
            Tree = tree;
            Parent = parent;
        }

        // allow derived nodes to simply fetch children from their wrapped object
        // they don't have to worry about converting them into INodes
        protected abstract IEnumerable<T> GetChildren();

        // derived class should call this when its children change
        // this notifies the model of any added/removed children, which in turn notifies the view
        protected void RefreshChildren()
        {
            if (!ChildrenReady)
                return;
            var path = Path;
            var new_children = GetChildren().Where(x => x is not null).ToList();
            var remove = ChildNodes.Keys.Except(new_children).ToArray();
            var add = new_children.Except(ChildNodes.Keys).ToArray();
            if (remove.Any())
            {
                int[] indices = new int[remove.Length];
                INode[] nodes = new INode[remove.Length];
                for (int i = 0; i < remove.Length; i++)
                {
                    var item = remove[i];
                    int index = ChildNodes.IndexOf(item);
                    var node = ChildNodes[index];
                    indices[i] = index;
                    nodes[i] = node;
                }
                // remove afterwards to ensure indices don't shift as we go
                foreach (var item in remove)
                {
                    int minus = ChildNodes[item].DescendantsCount + 1;
                    UpdateDescendantsCount(x => x - minus);
                    var child = ChildNodes[item];
                    if (child is IDisposable d)
                        d.Dispose();
                    ChildNodes.Remove(item);
                }
                Tree.NotifyNodesRemoved(path, nodes, indices);
            }
            if (add.Any())
            {
                int[] indices = new int[add.Length];
                INode[] nodes = new INode[add.Length];
                for (int i = 0; i < add.Length; i++)
                {
                    var item = add[i];
                    int index = new_children.IndexOf(item);
                    var node = NodeRegistry.CreateNode(Tree, this, item);
                    int plus = node.DescendantsCount + 1;
                    UpdateDescendantsCount(x => x + plus);
                    indices[i] = index;
                    nodes[i] = node;
                    ChildNodes.Insert(index, item, node);
                }
                Tree.NotifyNodesAdded(path, nodes, indices);
            }
            if (!new_children.SequenceEqual(ChildNodes.Keys))
            {
                ChildNodes.SortKeys(new OrderLikeList(new_children));
                Tree.NotifyNodesReordered(path);
            }
            Tree.NotifyNodeChanged(this);
        }

        public void Dispose()
        {
            SelfDispose();
            foreach (var item in Children.OfType<IDisposable>())
            {
                item.Dispose();
            }
        }

        protected abstract void SelfDispose();

        private class OrderLikeList : IComparer<T>
        {
            private readonly List<T> List;
            public OrderLikeList(List<T> list)
            {
                List = list;
            }

            public int Compare(T x, T y)
            {
                return List.IndexOf(x).CompareTo(List.IndexOf(y));
            }
        }

        protected void NotifyChanged()
        {
            Tree.NotifyNodeChanged(this);
        }

        // save an undoable action to the model
        protected void NoticeAction(UndoableAction action)
        {
            Tree.UndoHistory.SaveAction(action);
        }

        // help derived nodes find their children in INode form, since they usually just fetch the raw objects
        // mostly used for implementing Paste which returns the nodes it created
        protected IEnumerable<INode> NodeChildren(IEnumerable<T> objects)
        {
            foreach (var item in objects)
            {
                if (ChildNodes.TryGetValue(item, out var result))
                    yield return result;
            }
        }

        protected Dictionary<T, INode> NodeChildrenMap(IEnumerable<T> objects)
        {
            var dictionary = new Dictionary<T, INode>();
            foreach (var item in objects)
            {
                if (ChildNodes.TryGetValue(item, out var result))
                    dictionary[item] = result;
            }
            return dictionary;
        }

        // make all action implementations virtual, disallowing everything
        // derived nodes can allow and implement actions if they wish
        public virtual string Description => "unknown node";
        public virtual bool CanDelete => false;
        public virtual void Delete()
        {
            if (Parent is ModelRootNode root)
                root.Remove(this);
        }
        public virtual bool CanSort => false;
        public virtual void Sort() { }
        public virtual bool CanCopy => false;
        public virtual DataObject Copy() => null;
        public virtual bool CanCut => CanDelete && CanCopy; // sensible defaults for cut
        public virtual DataObject Cut() { var copy = Copy(); Delete(); return copy; }
        public virtual bool CanPaste => false;
        public virtual IEnumerable<INode> Paste(IDataObject data) => Enumerable.Empty<INode>();
        public virtual bool CanRename => false;
        public virtual bool CanEdit => false;
        public virtual bool CanReceiveDrop(IEnumerable<INode> nodes) => false;
        public virtual void ReceiveDrop(IEnumerable<INode> nodes, int index) { }
    }

    // hidden down here because it's shameful!
    internal interface IModelNode : INode
    {
        void SetDescendantsCount(int value);
    }
}
