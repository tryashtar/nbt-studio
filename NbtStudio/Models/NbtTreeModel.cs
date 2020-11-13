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

namespace NbtStudio
{
    public partial class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
#pragma warning disable 67
        public event EventHandler<TreePathEventArgs> StructureChanged;
#pragma warning restore 67
        public event EventHandler Changed;

        private readonly List<INode> Roots;
        public readonly UndoHistory UndoHistory;

        public NbtTreeModel(IEnumerable<object> roots)
        {
            UndoHistory = new UndoHistory(GetDescription);
            Roots = roots.Select(x => NodeRegistry.CreateRootNode(this, x)).ToList();
        }
        public NbtTreeModel(object root) : this(new[] { root }) { }

        public static string GetDescription(object obj)
        {
            if (obj is INode node)
                return node.Description;
            if (obj is IEnumerable<INode> nodes)
                return ExtractNodeOperations.Description(nodes);
            if (obj is NbtTag tag)
                return NbtUtil.TagDescription(tag);
            if (obj is IEnumerable<NbtTag> tags)
                return NbtUtil.TagDescription(tags);
            return obj.ToString();
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
            NodesChanged?.Invoke(this, new TreeModelEventArgs(node.Parent.Path, new object[] { node }));
        }

        IEnumerable ITreeModel.GetChildren(TreePath treePath) => GetChildren(treePath);
        public IEnumerable<object> GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Roots;
            else
                return ((INode)treePath.LastNode).Children;
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !((INode)treePath.LastNode).HasChildren;
        }
    }
}
