using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public static class NodeRegistry
    {
        private static readonly Dictionary<Type, Func<NbtTreeModel, INode, object, INode>> RegisteredConverters = new Dictionary<Type, Func<NbtTreeModel, INode, object, INode>>();
        public static void Register<T>(Func<NbtTreeModel, INode, T, INode> converter)
        {
            RegisteredConverters[typeof(T)] = (tree, parent, item) => converter(tree, parent, (T)item);
        }

        public static INode CreateNode(NbtTreeModel tree, INode parent, object item)
        {
            foreach (var converter in RegisteredConverters)
            {
                if (converter.Key.IsInstanceOfType(item))
                    return converter.Value(tree, parent, item);
            }
            throw new InvalidOperationException($"No registered converter for {item.GetType()}");
        }

        public static INode CreateRootNode(NbtTreeModel tree, object item) => CreateNode(tree, null, item);
    }

    public interface INode
    {
        INode Parent { get; }
        TreePath Path { get; }
        IEnumerable<INode> Children { get; }
        bool HasChildren { get; }
        string Description { get; }
        bool CanDelete { get; }
        void Delete();
        bool CanSort { get; }
        void Sort();
        bool CanCopy { get; }
        DataObject Copy();
        bool CanCut { get; }
        DataObject Cut();
        bool CanPaste { get; }
        IEnumerable<INode> Paste(IDataObject data);
        bool CanRename { get; }
        bool CanEdit { get; }
        bool CanReceiveDrop(IEnumerable<INode> nodes);
        void ReceiveDrop(IEnumerable<INode> nodes, int index);
    }

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

    public class NbtTagNode : ModelNode
    {
        public readonly NbtTag Tag;
        public NbtTagNode(NbtTreeModel tree, INode parent, NbtTag tag) : base(tree, parent)
        {
            Tag = tag;
            Tag.Changed += Tag_Changed;
        }

        private void Tag_Changed(object sender, NbtTag e)
        {
            RefreshChildren();
        }

        static NbtTagNode()
        {
            NodeRegistry.Register<NbtTag>((tree, parent, tag) => new NbtTagNode(tree, parent, tag));
        }

        protected override IEnumerable<object> GetChildren()
        {
            if (Tag is NbtContainerTag container)
                return container;
            return Enumerable.Empty<object>();
        }

        public override string Description => Tag.TagDescription();

        public override bool CanCopy => true;
        public override DataObject Copy() => NbtNodeOperations.Copy(Tag);
        public override bool CanDelete => true;
        public override void Delete()
        {
            Tag.Remove();
            base.Delete();
        }
        public override bool CanEdit => NbtNodeOperations.CanEdit(Tag);
        public override bool CanPaste => NbtNodeOperations.CanPaste(Tag);
        public override bool CanRename => NbtNodeOperations.CanRename(Tag);
        public override bool CanSort => NbtNodeOperations.CanSort(Tag);
        public override void Sort() => NbtNodeOperations.Sort(Tag);
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(Tag, data);
            return tags.Select(Wrap);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode) && NbtNodeOperations.CanReceiveDrop(Tag, nodes.Filter(x => x.GetNbtTag()));
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(Tag, tags, index);
        }
    }
}
