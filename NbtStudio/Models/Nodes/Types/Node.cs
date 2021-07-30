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
    public abstract class Node
    {
        public readonly Node Parent;
        public int DescendantsCount { get; private set; }
        public TreePath Path
        {
            get
            {
                var path = new Stack<Node>();
                Node item = this;
                while (item is not null)
                {
                    path.Push(item);
                    item = item.Parent;
                }
                return new TreePath(path.ToArray());
            }
        }
        public IList<Node> Children
        {
            get
            {
                if (IsDirty)
                    RefreshChildren();
                return ChildNodes.Values;
            }
        }
        public virtual bool HasChildren => GetChildren().Any();

        private readonly OrderedDictionary<object, Node> ChildNodes = new();
        // start off true since child nodes aren't ready yet
        private bool IsDirty = true;

        public Node(Node parent)
        {
            Parent = parent;
        }

        public (string name, string value) Preview()
        {
            return (PreviewName(), PreviewValue());
        }

        protected void MarkDirty()
        {
            IsDirty = true;
        }

        protected abstract IEnumerable<object> GetChildren();
        protected abstract Node MakeChild(object item);
        public virtual string GetTooltip() => null;
        public abstract string PreviewName();
        public abstract string PreviewValue();
        public abstract IconType GetIcon();

        private void RefreshChildren()
        {
            // make sure to reuse existing nodes
            var new_nodes = GetChildren().Select(x => KeyValuePair.Create(x, GetOrCreateChild(x))).ToList();
            ChildNodes.Clear();
            DescendantsCount = 0;
            foreach (var node in new_nodes)
            {
                DescendantsCount += node.Value.DescendantsCount + 1; // +1 for the node itself
                ChildNodes.Add(node);
            }
            IsDirty = false;
        }

        private Node GetOrCreateChild(object obj)
        {
            if (ChildNodes.TryGetValue(obj, out var result))
                return result;
            return MakeChild(obj);
        }

        protected virtual NbtTag GetNbtTag() => null;
        public Lens<NbtTag> GetNbtTagLens()
        {
            var nbt = GetNbtTag();
            if (nbt == null)
                return null;
            return new Lens<NbtTag>(nbt, MarkDirty);
        }
    }
}