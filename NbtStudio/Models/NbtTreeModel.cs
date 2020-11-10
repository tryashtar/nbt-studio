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

        public bool HasAnyUnsavedChanges => OpenedFiles.Filter(x => x.GetSaveable()).Any(x => x.HasUnsavedChanges);
        private readonly List<object> Roots;
        private readonly NbtTreeView View;
        private readonly UndoHistory UndoHistory = new UndoHistory();

        public INode SelectedObject
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return Wrap(View.SelectedNode.Tag);
            }
        }
        public IEnumerable<INode> SelectedObjects
        {
            get
            {
                if (View.SelectedNodes == null)
                    return Enumerable.Empty<INode>();
                return View.SelectedNodes.Select(x => Wrap(x.Tag));
            }
        }
        public IEnumerable<INode> OpenedFiles
        {
            get
            {
                foreach (var item in View.BreadthFirstSearch(x => (x.Tag is NbtFolder folder && folder.HasScanned) || x.Tag is ISaveable))
                {
                    if (item.Tag is ISaveable saveable)
                        yield return Wrap(saveable);
                }
            }
        }

        public NbtTreeModel(IEnumerable<object> roots, NbtTreeView view)
        {
            Roots = roots.ToList();
            View = view;
            View.Model = this;
            if (Util.ExactlyOne(Roots)) // if there is one item, expand it
                View.Root.Children.First().Expand();
        }
        public NbtTreeModel(object root, NbtTreeView view) : this(new[] { root }, view) { }

        private INode Wrap(object obj) => NotifyNode.Create(this, obj);

        public IEnumerable<INode> ObjectsFromDrag(DragEventArgs e)
        {
            return View.NodesFromDrag(e).Select(x => Wrap(x.Tag)).Where(x => x != null);
        }
        public INode ObjectFromClick(TreeNodeAdvMouseEventArgs e)
        {
            return Wrap(e.Node.Tag);
        }
        public INode DropObject
        {
            get
            {
                if (View.DropPosition.Node == null)
                    return null;
                return Wrap(View.DropPosition.Node.Tag);
            }
        }
        public NodePosition DropPosition => View.DropPosition.Position;

        // an object changed, refresh the nodes through ITreeModel's API to ensure it matches the true object
        public void Notify(object changed)
        {
#if DEBUG
            if (changed != null)
                Console.WriteLine($"changed: {changed.GetType()}");
#endif
            var node = FindNodeByObject(changed);
            if (node == null)
            {
#if DEBUG
                Console.WriteLine($"could not find node on tree, not updating model");
#endif
                return;
            }
            var path = View.GetPath(node);

            Changed?.Invoke(this, EventArgs.Empty);

            var real_children = GetChildren(path).ToList();
            var current_children = node.Children.Select(x => x.Tag).ToArray();
            var remove = current_children.Except(real_children).ToArray();
            var add = real_children.Except(current_children).ToArray();

            var parent_path = ParentPath(path);
            NodesChanged?.Invoke(this, new TreeModelEventArgs(parent_path, new object[] { changed }));
            if (remove.Any())
                NodesRemoved?.Invoke(this, new TreeModelEventArgs(path, remove));
            if (add.Any())
            {
                if (node.IsExpandedOnce || node.IsLeaf) // avoid duplicating children when this is called at the same time the view loads them
                    NodesInserted?.Invoke(this, new TreeModelEventArgs(path, add.Select(x => real_children.IndexOf(x)).ToArray(), add));
                node.Expand();
            }
        }

        public void Remove(object removed)
        {
#if DEBUG
            if (removed != null)
                Console.WriteLine($"removed: {removed.GetType()}");
#endif
            var node = FindNodeByObject(removed);
            if (node == null)
            {
#if DEBUG
                Console.WriteLine($"could not find node on tree, not updating model");
#endif
                return;
            }
            TreePath path;
            if (Roots.Remove(removed))
                path = TreePath.Empty;
            else
                path = ParentPath(View.GetPath(node));
            NodesRemoved?.Invoke(this, new TreeModelEventArgs(path, new[] { removed }));
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private static TreePath ParentPath(TreePath path)
        {
            return new TreePath(path.FullPath.Take(path.FullPath.Length - 1).ToArray());
        }

        private TreeNodeAdv FindNodeByObject(object obj)
        {
            var quick = View.FindNodeByTag(obj);
            if (quick != null)
                return quick;
            foreach (var item in View.BreadthFirstSearch())
            {
                // notifiers can't tell whether they were added to a file that's being treated as a compound
                // so here we disambiguate them
                if (item.Tag is NbtFile file && file.RootTag == obj)
                    return item;
                if (item.Tag is Chunk chunk && chunk.IsLoaded && chunk.Data == obj)
                    return item;
            }
            return null;
        }

        public (INode destination, int index) GetInsertionLocation(INode target, NodePosition position)
        {
            var obj = target.Object;
            var path = View.GetPath(FindNodeByObject(obj));
            var children = GetChildren(obj);
            if (position == NodePosition.Inside)
                return (target, children.Count());
            else
            {
                var parent = path.FullPath[path.FullPath.Length - 2];
                var parent_children = GetChildren(parent).ToList();
                int index = parent_children.IndexOf(obj);
                if (position == NodePosition.After)
                    index++;
                return (Wrap(parent), index);
            }
        }

        public void SaveAction(UndoableAction action)
        {
            UndoHistory.SaveAction(action);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Undo(int count = 1)
        {
            UndoHistory.Undo(count);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Redo(int count = 1)
        {
            UndoHistory.Redo(count);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool CanUndo => UndoHistory.CanUndo;
        public bool CanRedo => UndoHistory.CanRedo;

        public List<KeyValuePair<int, string>> GetUndoHistory() => UndoHistory.GetUndoHistory();
        public List<KeyValuePair<int, string>> GetRedoHistory() => UndoHistory.GetRedoHistory();

        public void StartBatchOperation()
        {
            UndoHistory.StartBatchOperation();
        }

        public void FinishBatchOperation(string description, bool replace_single)
        {
            UndoHistory.FinishBatchOperation(description, replace_single);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        IEnumerable ITreeModel.GetChildren(TreePath treePath) => GetChildren(treePath);
        public IEnumerable<object> GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Roots;
            else
                return GetChildren(treePath.LastNode);
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !HasChildren(treePath.LastNode);
        }

        private IEnumerable<object> GetChildren(object obj)
        {
            Wrap(obj); // hack fix for folders not notifying tree when contents change
            if (obj is NbtFolder folder)
            {
                if (!folder.HasScanned)
                {
                    folder.Scan();
                    Changed?.Invoke(this, EventArgs.Empty);
                    return null; // hack fix for double folder contents caused by above hack fix
                }
                return folder.Subfolders.Concat<object>(folder.Files);
            }
            if (obj is NbtFile file)
                return file.RootTag.Tags;
            if (obj is RegionFile region)
                return region.AllChunks.Where(x => x != null);
            if (obj is Chunk chunk)
            {
                if (!chunk.IsLoaded)
                {
                    chunk.Load();
                    if (chunk.IsCorrupt)
                        Notify(chunk.Region);
                    Changed?.Invoke(this, EventArgs.Empty);
                }
                if (chunk.IsLoaded)
                    return chunk.Data.Tags;
            }
            if (obj is INbtCompound compound)
                return compound.Tags;
            if (obj is INbtList list)
                return list;
            return Enumerable.Empty<object>();
        }

        private bool HasChildren(object obj)
        {
            if (obj is Chunk chunk && !chunk.IsLoaded)
                return !chunk.IsExternal;
            if (obj is NbtFolder folder && !folder.HasScanned)
                return true;
            var children = GetChildren(obj);
            return children != null && children.Any();
        }
    }
}
