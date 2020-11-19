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
    public abstract class ModelNode<T> : INode
    {
        private readonly NbtTreeModel Tree;
        public INode Parent { get; private set; }
        private bool ChildrenReady = false;
        private OrderedDictionary<T, INode> ChildNodes;
        public IEnumerable<INode> Children
        {
            get
            {
                // some nodes call RefreshChildren when calling GetChildren (especially for the first time)
                // so use this flag to make sure we don't enter this section twice
                if (!ChildrenReady)
                {
                    ChildNodes = new OrderedDictionary<T, INode>();
                    var children = GetChildren().Where(x => x != null);
                    foreach (var item in children)
                    {
                        ChildNodes.Add(item, NodeRegistry.CreateNode(Tree, this, item));
                    }
                    ChildrenReady = true;
                }
                return ChildNodes.Values;
            }
        }

        // default implementation fetches children, this can be overridden to defer it to later
        public virtual bool HasChildren => Children.Any();

        public TreePath Path
        {
            get
            {
                var path = new List<INode>();
                INode item = this;
                while (item != null && !(item is ModelRootNode))
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
            var new_children = GetChildren().Where(x => x != null).ToList();
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
                    indices[i] = index;
                    nodes[i] = node;
                    ChildNodes.Insert(index, item, node);
                }
                Tree.NotifyNodesAdded(path, nodes, indices);
            }
            Tree.NotifyNodeChanged(this);
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

        protected Dictionary<T,INode> NodeChildrenMap(IEnumerable<T> objects)
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
}
