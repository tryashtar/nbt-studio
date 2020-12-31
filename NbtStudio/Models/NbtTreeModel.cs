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
#pragma warning disable 67
        public event EventHandler<TreePathEventArgs> StructureChanged;
#pragma warning restore 67
        public event EventHandler Changed;

        public readonly ModelRootNode Root;
        public readonly UndoHistory UndoHistory;

        public NbtTreeModel(IEnumerable<object> roots)
        {
            UndoHistory = new UndoHistory(GetDescription);
            UndoHistory.Changed += UndoHistory_Changed;
            Root = new ModelRootNode(this, roots);
        }
        public NbtTreeModel(object root) : this(new[] { root }) { }
        public NbtTreeModel() : this(Enumerable.Empty<object>()) { }

        public void Import(object obj)
        {
            Root.Add(obj);
        }

        public void ImportMany(IEnumerable<object> objects)
        {
            Root.AddRange(objects);
        }

        public void RemoveMany(IEnumerable<INode> nodes)
        {
            Root.RemoveNodes(nodes);
        }

        private void UndoHistory_Changed(object sender, EventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

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
        public IEnumerable<IFile> OpenedFiles
        {
            get
            {
                foreach (var item in BreadthFirstSearch(x => (x is FolderNode folder && folder.Folder.HasScanned) || x.Get<IFile>() != null))
                {
                    var file = item.Get<IFile>();
                    if (file != null)
                        yield return file;
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
            foreach (var item in Root.Children)
            {
                // don't just enqueue Root directly because the predicate might not match it
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

        public void NotifyNodesReordered(TreePath path)
        {
            StructureChanged?.Invoke(this, new TreePathEventArgs(path));
        }

        IEnumerable ITreeModel.GetChildren(TreePath treePath) => GetChildren(treePath);
        public IEnumerable<object> GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Root.Children;
            else
                return ((INode)treePath.LastNode).Children;
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !((INode)treePath.LastNode).HasChildren;
        }
    }
}
