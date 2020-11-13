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

namespace NbtStudio
{
    public static class NodeRegistry
    {
        private static readonly Dictionary<Type, Func<NbtTreeModel, ModelNode, object, ModelNode>> RegisteredConverters = new Dictionary<Type, Func<NbtTreeModel, ModelNode, object, ModelNode>>();
        public static void Register<T>(Func<NbtTreeModel, ModelNode, T, ModelNode> converter)
        {
            RegisteredConverters[typeof(T)] = (tree, parent, item) => converter(tree, parent, (T)item);
        }

        public static ModelNode CreateNode(NbtTreeModel tree, ModelNode parent, object item)
        {
            foreach (var converter in RegisteredConverters)
            {
                if (converter.Key.IsInstanceOfType(item))
                    return converter.Value(tree, parent, item);
            }
            throw new InvalidOperationException($"No registered converter for {item.GetType()}");
        }
    }

    public abstract class ModelNode
    {
        protected readonly NbtTreeModel Tree;
        public ModelNode Parent { get; private set; }
        private OrderedDictionary<object, ModelNode> ChildNodes;
        public IEnumerable<ModelNode> Children
        {
            get
            {
                if (ChildNodes == null)
                {
                    ChildNodes = new OrderedDictionary<object, ModelNode>();
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
                var item = this;
                while (item != null)
                {
                    path.Add(item);
                    item = item.Parent;
                }
                path.Reverse();
                return new TreePath(path.ToArray());
            }
        }

        public ModelNode(NbtTreeModel tree, ModelNode parent)
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
                ModelNode[] nodes = new ModelNode[remove.Length];
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
                ModelNode[] nodes = new ModelNode[add.Length];
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
    }

    public class NbtTagNode2 : ModelNode
    {
        private readonly NbtTag Tag;
        public NbtTagNode2(NbtTreeModel tree, ModelNode parent, NbtTag tag) : base(tree, parent)
        {
            Tag = tag;
            Tag.Changed += Tag_Changed;
        }

        private void Tag_Changed(object sender, NbtTag e)
        {
            RefreshChildren();
        }

        static NbtTagNode2()
        {
            NodeRegistry.Register<NbtTag>((tree, parent, tag) => new NbtTagNode2(tree, parent, tag));
        }

        protected override IEnumerable<object> GetChildren()
        {
            if (Tag is NbtContainerTag container)
                return container;
            return Enumerable.Empty<object>();
        }
    }
}
