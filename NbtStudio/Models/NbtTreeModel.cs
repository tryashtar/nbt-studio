using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NbtStudio.UI;
using System.Collections.ObjectModel;

namespace NbtStudio
{
    // the model version of the tree of nodes
    // this is mostly necessary because TreeViewAdv requires it, but it has some extra stuff as well
    public partial class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        private readonly List<Node> Roots;
        public ReadOnlyCollection<Node> RootNodes => Roots.AsReadOnly();
        public readonly UndoHistory UndoHistory;

        public NbtTreeModel(IEnumerable<object> roots)
        {
            UndoHistory = new UndoHistory(GetDescription);
            UndoHistory.Changed += UndoHistory_Changed;
            Roots = roots.Select(x => NodeFactory.Create(null, x)).ToList();
        }
        public NbtTreeModel(object root) : this(new[] { root }) { }
        public NbtTreeModel() : this(Enumerable.Empty<object>()) { }

        public (Node destination, int index) GetInsertionLocation(Node target, NodePosition position)
        {
            if (position == NodePosition.Inside)
                return (target, target.Children.Count);
            else
            {
                var parent = target.Parent;
                var siblings = parent.Children.ToList();
                int index = siblings.IndexOf(target);
                if (position == NodePosition.After)
                    index++;
                return (parent, index);
            }
        }

        public void NotifyNodesAdded(TreePath path, INode[] nodes, int[] indices)
        {
            NodesInserted?.Invoke(this, new TreeModelEventArgs(path, indices, nodes));
        }

        public void NotifyNodesRemoved(TreePath path, INode[] nodes, int[] indices)
        {
            NodesRemoved?.Invoke(this, new TreeModelEventArgs(path, indices, nodes));
        }

        public void NotifyNodeChanged(INode node)
        {
            var path = node.Parent?.Path ?? new TreePath();
            NodesChanged?.Invoke(this, new TreeModelEventArgs(path, new object[] { node }));
        }

        public void NotifyNodesReordered(TreePath path)
        {
            StructureChanged?.Invoke(this, new TreePathEventArgs(path));
        }

        IEnumerable ITreeModel.GetChildren(TreePath treePath) => GetChildren(treePath);
        public IEnumerable<object> GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Roots;
            else
                return ((Node)treePath.LastNode).Children;
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !((Node)treePath.LastNode).HasChildren;
        }
    }
}
