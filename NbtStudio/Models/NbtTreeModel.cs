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
        public event EventHandler<TreePathEventArgs> StructureChanged;
        public event EventHandler Changed;

        public bool HasAnyUnsavedChanges => OpenedFiles.Any(x => x.GetSaveable().HasUnsavedChanges);
        private readonly IEnumerable<object> Roots;
        private readonly NbtTreeView View;
        private readonly Stack<UndoableAction> UndoStack = new Stack<UndoableAction>();
        private readonly Stack<UndoableAction> RedoStack = new Stack<UndoableAction>();

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
                foreach (var item in View.BreadthFirstSearch(x => x.Tag is NbtFolder || x.Tag is ISaveable))
                {
                    if (item.Tag is ISaveable saveable)
                        yield return Wrap(saveable);
                }
            }
        }

        public NbtTreeModel(IEnumerable<object> roots, NbtTreeView view)
        {
            Roots = roots;
            View = view;
            View.Model = this;
            if (Roots.Take(2).Count() == 1) // if there is one item, expand it
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
            if (node == null) return;
            var path = View.GetPath(node);

            Changed?.Invoke(this, EventArgs.Empty);

            var real_children = GetChildren(path).ToList();
            var current_children = node.Children.Select(x => x.Tag).ToArray();
            var remove = current_children.Except(real_children).ToArray();
            var add = real_children.Except(current_children).ToArray();

            var parent_path = new TreePath(path.FullPath.Take(path.FullPath.Length - 1).ToArray());
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

        public void PerformAction(UndoableAction action)
        {
            RedoStack.Clear();
            if (BatchNumber == 0)
                UndoStack.Push(action);
            else
                UndoBatch.Add(action);
#if DEBUG
            if (BatchNumber == 0)
                Console.WriteLine($"Added action to main stack: \"{action.Description}\". Undo stack has {UndoStack.Count} items");
            else
                Console.WriteLine($"Added action to batch: \"{action.Description}\". Batch has {UndoBatch.Count} items");
#endif
            action.Do();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Undo(int count = 1)
        {
            for (int i = 0; i < count && UndoStack.Any(); i++)
            {
                var action = UndoStack.Pop();
                RedoStack.Push(action);
                action.Undo();
#if DEBUG
                Console.WriteLine($"Performed undo of action \"{action.Description}\". Undo stack has {UndoStack.Count} items. Redo stack has {RedoStack.Count} items");
#endif
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Redo(int count = 1)
        {
            for (int i = 0; i < count && RedoStack.Any(); i++)
            {
                var action = RedoStack.Pop();
                UndoStack.Push(action);
                action.Do();
#if DEBUG
                Console.WriteLine($"Performed redo of action \"{action.Description}\". Redo stack has {RedoStack.Count} items. Undo stack has {UndoStack.Count} items");
#endif
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool CanUndo => UndoStack.Any();
        public bool CanRedo => RedoStack.Any();

        public List<KeyValuePair<int, string>> GetUndoHistory()
        {
            return UndoStack.Select((v, i) => new KeyValuePair<int, string>(i + 1, v.Description)).ToList();
        }
        public List<KeyValuePair<int, string>> GetRedoHistory()
        {
            return RedoStack.Select((v, i) => new KeyValuePair<int, string>(i + 1, v.Description)).ToList();
        }

        private int BatchNumber = 0;
        private readonly List<UndoableAction> UndoBatch = new List<UndoableAction>();
        // call this and then do things that signal undos, then call FinishBatchOperation to merge all those undos into one
        public void StartBatchOperation()
        {
            BatchNumber++;
        }

        public void FinishBatchOperation(string description, bool replace_single)
        {
            if (BatchNumber == 0)
                return;
            BatchNumber--;
            if (BatchNumber == 0 && UndoBatch.Any())
            {
                UndoableAction merged_action;
                if (replace_single || UndoBatch.Count > 1)
                    merged_action = UndoableAction.Merge(description, UndoBatch);
                else
                    merged_action = UndoBatch.Single();
                UndoStack.Push(merged_action);
#if DEBUG
                Console.WriteLine($"Merged {UndoBatch.Count} batch actions onto stack as action: \"{description}\". Stack has {UndoStack.Count} items");
#endif
                UndoBatch.Clear();
                Changed?.Invoke(this, EventArgs.Empty);
            }
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
            if (obj is NbtFolder folder)
            {
                if (!folder.HasScanned)
                {
                    folder.Scan();
                    Changed?.Invoke(this, EventArgs.Empty);
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
                return true;
            if (obj is NbtFolder folder && !folder.HasScanned)
                return true;
            var children = GetChildren(obj);
            return children != null && children.Any();
        }
    }
}
