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
    public abstract class ModelNode : INode
    {
        protected readonly NbtTreeModel Tree;
        public INode Parent { get; private set; }
        private OrderedDictionary<object, INode> ChildNodes;
        public IEnumerable<INode> Children
        {
            get
            {
                if (ChildNodes == null)
                {
                    ChildNodes = new OrderedDictionary<object, INode>();
                    var children = GetChildren();
                    foreach (var item in children)
                    {
                        ChildNodes.Add(item, NodeRegistry.CreateNode(Tree, this, item));
                    }
                }
                return ChildNodes.Values;
            }
        }
        public virtual bool HasChildren => Children.Any();
        public TreePath Path
        {
            get
            {
                var path = new List<object>();
                INode item = this;
                while (item != null)
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

        protected abstract IEnumerable<object> GetChildren();
        protected void RefreshChildren()
        {
            if (ChildNodes == null)
                return;
            var path = Path;
            var new_children = GetChildren().ToList();
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

        protected void NoticeAction(UndoableAction action)
        {
            Tree.UndoHistory.SaveAction(action);
        }

        protected IEnumerable<T> FindChildren<T>(IEnumerable<object> objects, Func<T, object> getter) where T : INode
        {
            return Children.OfType<T>().Where(x => objects.Contains(getter(x)));
        }

        public virtual string Description => "unknown node";
        public virtual bool CanDelete => false;
        public virtual void Delete() { }
        public virtual bool CanSort => false;
        public virtual void Sort() { }
        public virtual bool CanCopy => false;
        public virtual DataObject Copy() => null;
        public virtual bool CanCut => CanDelete && CanCopy;
        public virtual DataObject Cut() { var copy = Copy(); Delete(); return copy; }
        public virtual bool CanPaste => false;
        public virtual IEnumerable<INode> Paste(IDataObject data) => Enumerable.Empty<INode>();
        public virtual bool CanRename => false;
        public virtual bool CanEdit => false;
        public virtual bool CanReceiveDrop(IEnumerable<INode> nodes) => false;
        public virtual void ReceiveDrop(IEnumerable<INode> nodes, int index) { }
    }
}
