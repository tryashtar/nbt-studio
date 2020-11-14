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
        public ReadOnlyCollection<INode> RootNodes => Roots.AsReadOnly();
        public readonly UndoHistory UndoHistory;

        public NbtTreeModel(IEnumerable<object> roots)
        {
            UndoHistory = new UndoHistory(GetDescription);
            Roots = roots.Select(x => NodeRegistry.CreateRootNode(this, x)).ToList();
        }
        public NbtTreeModel(object root) : this(new[] { root }) { }
        public NbtTreeModel() : this(Enumerable.Empty<object>()) { }

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

        public bool HasAnyUnsavedChanges => OpenedFiles.Any(x => x.HasUnsavedChanges);
        public IEnumerable<ISaveable> OpenedFiles
        {
            get
            {
                foreach (var item in BreadthFirstSearch(x => (x is FolderNode folder && folder.Folder.HasScanned) || x.GetSaveable() != null))
                {
                    var saveable = item.GetSaveable();
                    if (saveable != null)
                        yield return saveable;
                }
            }
        }

        public (INode destination, int index) GetInsertionLocation(INode target, NodePosition position)
        {
            if (position == NodePosition.Inside)
                return (target, target.Children.Count());
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

        private IEnumerable<INode> BreadthFirstSearch(Predicate<INode> predicate)
        {
            var queue = new Queue<INode>();
            foreach (var item in Roots)
            {
                queue.Enqueue(item);
            }
            while (queue.Any())
            {
                var item = queue.Dequeue();
                if (!predicate(item))
                    continue;
                yield return item;
                foreach (var sub in item.Children)
                {
                    queue.Enqueue(sub);
                }
            }
        }

        public void NotifyNodesAdded(TreePath path, INode[] nodes, int[] indices)
        {
            NodesInserted?.Invoke(this, new TreeModelEventArgs(path, indices, nodes));
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyNodesRemoved(TreePath path, INode[] nodes, int[] indices)
        {
            NodesRemoved?.Invoke(this, new TreeModelEventArgs(path, indices, nodes));
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyNodeChanged(INode node)
        {
            var path = node.Parent?.Path ?? new TreePath();
            NodesChanged?.Invoke(this, new TreeModelEventArgs(path, new object[] { node }));
            Changed?.Invoke(this, EventArgs.Empty);
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
