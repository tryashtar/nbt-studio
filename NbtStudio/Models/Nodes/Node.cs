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
        public Node Parent { get; private set; }
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

        public delegate void NodeStructureChangedEvent(ChildrenChangedReport report);
        public event NodeStructureChangedEvent SomethingChanged;
        protected void CascadeChanges(ChildrenChangedReport report)
        {
            Node node = this;
            while (node != null)
            {
                node.SomethingChanged?.Invoke(report);
                node = node.Parent;
            }
        }

        // to do: ehh...
        public static readonly HashSet<Node> DirtyNodes = new();
        protected void MarkDirty()
        {
            IsDirty = true;
            DirtyNodes.Add(this);
        }

        protected abstract IEnumerable<object> GetChildren();
        protected abstract Node MakeChild(object item);
        public virtual string GetTooltip() => null;
        public abstract string PreviewName();
        public abstract string PreviewValue();
        public abstract IconType GetIcon();

        public ChildrenChangedReport RefreshChildren()
        {
            // make sure to reuse existing nodes
            var new_nodes = GetChildren().Select(x => KeyValuePair.Create(x, GetOrCreateChild(x))).ToList();
            var report = new ChildrenChangedReport(this, ChildNodes, new_nodes);
            // clear the parent, might not be necessary but could help the GC
            foreach (var item in report.RemovedChildren)
            {
                item.Value.Parent = null;
                DirtyNodes.Remove(item.Value);
            }
            ChildNodes.Clear();
            DescendantsCount = 0;
            foreach (var node in new_nodes)
            {
                DescendantsCount += node.Value.DescendantsCount + 1; // +1 for the node itself
                ChildNodes.Add(node);
            }
            IsDirty = false;
            DirtyNodes.Remove(this);
            CascadeChanges(report);
            return report;
        }

        private Node GetOrCreateChild(object obj)
        {
            if (ChildNodes.TryGetValue(obj, out var result))
                return result;
            var child = MakeChild(obj);
            child.Parent = this;
            return child;
        }

        public virtual NbtTag GetNbtTag() => null;
    }
}