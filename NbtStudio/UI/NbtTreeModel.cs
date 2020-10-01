using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio.UI
{
    public partial class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;
        public event EventHandler Changed;

        public bool HasAnyUnsavedChanges
        {
            get
            {
                return UnsavedChanges.Values.Any(x => x);
            }
        }
        private readonly IEnumerable<object> Roots;
        private readonly NbtTreeView View;
        private readonly Stack<UndoableAction> UndoStack = new Stack<UndoableAction>();
        private readonly Stack<UndoableAction> RedoStack = new Stack<UndoableAction>();

        public INotifyNode SelectedObject
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return NotifyWrap(this, View.SelectedNode.Tag);
            }
        }
        public IEnumerable<INotifyNode> SelectedObjects
        {
            get
            {
                if (View.SelectedNodes == null)
                    return Enumerable.Empty<INotifyNode>();
                return View.SelectedNodes.Select(x => NotifyWrap(this, x.Tag));
            }
        }
        public INotifyNbt SelectedNbt
        {
            get
            {
                if (View.SelectedNode == null)
                    return null;
                return NotifyWrapNbt(this, View.SelectedNode.Tag, GetNbt(View.SelectedNode.Tag));
            }
        }
        public IEnumerable<INotifyNbt> SelectedNbts
        {
            get
            {
                if (View.SelectedNodes == null)
                    Enumerable.Empty<INotifyNbt>();
                return View.SelectedNodes.Select(x => NotifyWrapNbt(this, x.Tag, GetNbt(x.Tag))).Where(x => x != null);
            }
        }
        public IEnumerable<ISaveable> OpenedFiles
        {
            get
            {
                foreach (var item in View.BreadthFirstSearch(x => x.Tag is NbtFile || x.Tag is NbtFolder || x.Tag is RegionFile))
                {
                    if (item.Tag is ISaveable saveable)
                        yield return NotifyWrapSaveable(this, item.Tag, saveable);
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
            foreach (var file in OpenedFiles)
            {
                UnsavedChanges[file] = file.Path == null;
            }
        }
        public NbtTreeModel(object root, NbtTreeView view) : this(new[] { root }, view) { }

        public IEnumerable<INotifyNbt> NbtsFromDrag(DragEventArgs e)
        {
            return View.NodesFromDrag(e).Select(x => NotifyWrapNbt(this, x.Tag, GetNbt(x.Tag))).Where(x => x != null);
        }
        public ISaveable FileFromClick(TreeNodeAdvMouseEventArgs e)
        {
            if (e.Node.Tag is ISaveable saveable)
                return NotifyWrapSaveable(this, e.Node.Tag, saveable);
            return null;
        }
        public INotifyNbt NbtFromClick(TreeNodeAdvMouseEventArgs e)
        {
            var nbt = GetNbt(e.Node.Tag);
            if (nbt != null)
                return NotifyWrapNbt(this, e.Node.Tag, nbt);
            return null;
        }
        public INbtTag DropTag
        {
            get
            {
                if (View.DropPosition.Node == null)
                    return null;
                return NotifyWrapNbt(this, View.DropPosition.Node.Tag, GetNbt(View.DropPosition.Node.Tag));
            }
        }
        public NodePosition DropPosition => View.DropPosition.Position;

        // an object changed, refresh the nodes through ITreeModel's API to ensure it matches the true object
        private void Notify(object changed)
        {
#if DEBUG
            if (changed != null)
                Console.WriteLine($"changed: {changed.GetType()}");
#endif
            var node = FindNodeByObject(changed);
            if (node == null) return;
            var path = View.GetPath(node);

            foreach (var item in path.FullPath)
            {
                if (item is ISaveable saveable)
                    MarkChanged(saveable);
            }
            if (!IsUndoing)
                RedoStack.Clear();
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

        private readonly Dictionary<ISaveable, bool> UnsavedChanges = new Dictionary<ISaveable, bool>();
        private void NotifySaved(ISaveable saveable)
        {
            UnsavedChanges[saveable] = false;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        private void MarkChanged(ISaveable saveable)
        {
            UnsavedChanges[saveable] = true;
        }
        public bool HasUnsavedChanges(ISaveable saveable)
        {
            if (UnsavedChanges.TryGetValue(saveable, out bool result))
                return result;
            return false;
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

        private void PushUndo(UndoableAction action)
        {
            if (BatchNumber == 0)
                UndoStack.Push(action);
            else
                UndoBatch.Add(action);
#if DEBUG
            if (BatchNumber == 0)
                Console.WriteLine($"Added undo to main stack. Undo stack has {UndoStack.Count} items");
            else
                Console.WriteLine($"Added undo to batch. Batch has {UndoBatch.Count} items");
#endif
        }

        private bool IsUndoing = false;
        public void Undo()
        {
            if (UndoStack.Any())
            {
                var action = UndoStack.Pop();
                RedoStack.Push(action);
                IsUndoing = true;
                action.Undo();
                IsUndoing = false;
#if DEBUG
                Console.WriteLine($"Performed undo. Undo stack has {UndoStack.Count} items");
                Console.WriteLine($"Added redo. Redo stack has {RedoStack.Count} items");
#endif
            }
        }

        public void Redo()
        {
            if (RedoStack.Any())
            {
                var action = RedoStack.Pop();
                UndoStack.Push(action);
                IsUndoing = true;
                action.Do();
                IsUndoing = false;
#if DEBUG
                Console.WriteLine($"Performed redo. Redo stack has {RedoStack.Count} items");
                Console.WriteLine($"Added undo. Undo stack has {UndoStack.Count} items");
#endif
            }
        }

        public bool CanUndo => UndoStack.Any();
        public bool CanRedo => RedoStack.Any();

        private int BatchNumber = 0;
        private readonly List<UndoableAction> UndoBatch = new List<UndoableAction>();
        // call this and then do things that signal undos, then call FinishBatchOperation to merge all those undos into one
        public void StartBatchOperation()
        {
            BatchNumber++;
        }

        public void FinishBatchOperation()
        {
            if (BatchNumber == 0)
                return;
            BatchNumber--;
            if (BatchNumber == 0 && UndoBatch.Any())
            {
                var merged_action = UndoableAction.Merge(UndoBatch);
                UndoStack.Push(merged_action);
#if DEBUG
                Console.WriteLine($"Merged {UndoBatch.Count} batch actions onto stack. Stack has {UndoStack.Count} items");
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
            if (obj is NbtCompound compound)
                return compound.Tags;
            if (obj is NbtList list)
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

        private NbtTag GetNbt(object obj)
        {
            if (obj is NbtFile file)
                return file.RootTag;
            if (obj is Chunk chunk)
                return chunk.Data;
            if (obj is NbtTag tag)
                return tag;
            return null;
        }
    }
}
