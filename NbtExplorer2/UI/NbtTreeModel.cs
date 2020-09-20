using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2.UI
{
    public class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public bool HasUnsavedChanges { get; private set; } = false;
        private readonly IEnumerable<object> Roots;
        private readonly NbtTreeView View;

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
                return NotifyWrapNbt(this, INbt.GetNbt(View.SelectedNode.Tag));
            }
        }
        public IEnumerable<INotifyNbt> SelectedNbts
        {
            get
            {
                if (View.SelectedNodes == null)
                    Enumerable.Empty<INotifyNbt>();
                return View.SelectedNodes.Select(x => NotifyWrapNbt(this, INbt.GetNbt(x.Tag))).Where(x => x != null);
            }
        }

        public NbtTreeModel(IEnumerable<object> roots, NbtTreeView view)
        {
            Roots = roots;
            View = view;
            View.Model = this;
            // expand all top-level objects
            foreach (var item in View.Root.Children)
            {
                item.Expand();
            }
        }
        public NbtTreeModel(object root, NbtTreeView view) : this(new[] { root }, view) { }

        public void Remove(object obj)
        {
            var node = View.FindNodeByTag(obj);
            if (node != null && node.IsSelected)
            {
                node.IsSelected = false;
                if (node.NextNode != null)
                    node.NextNode.IsSelected = true;
                else if (node.PreviousNode != null)
                    node.PreviousNode.IsSelected = true;
                else if (node.Parent != null)
                    node.Parent.IsSelected = true;
            }
            INbt.Delete(obj);
            NodesRemoved?.Invoke(this, new TreeModelEventArgs(GetParentPath(obj), new[] { obj }));
            HasUnsavedChanges = true;
        }

        public void RemoveAll(IEnumerable<object> objects)
        {
            foreach (var item in objects.ToList())
            {
                Remove(item);
            }
        }

        public void Add(object parent, NbtTag tag)
        {
            INbt.Add(parent, tag);
            NodesInserted?.Invoke(this, new TreeModelEventArgs(GetPath(parent), new[] { INbt.IndexOf(parent, tag) }, new[] { tag }));
            View.FindNodeByTag(parent).Expand();
            //View.EnsureVisible(View.FindNodeByTag(tag));
            HasUnsavedChanges = true;
        }

        private Tuple<object, int> GetInsertionLocation(object target, NodePosition position)
        {
            var node = View.FindNodeByTag(target);
            if (node == null)
                throw new ArgumentException("Couldn't find target object");
            if (position == NodePosition.Inside)
                return Tuple.Create(target, node.Children.Count);
            else
            {
                int index = node.Parent.Children.IndexOf(node);
                if (position == NodePosition.After)
                    index++;
                return Tuple.Create(node.Parent.Tag, index);
            }
        }

        public bool CanMove(IEnumerable<object> items, object target, NodePosition placement)
        {
            var insertion = GetInsertionLocation(target, placement);
            var path = GetPath(insertion.Item1).FullPath;
            foreach (var item in items)
            {
                // can't become the child of your own descendent
                if (path.Contains(item))
                    return false;
            }
            return INbt.CanDropAll(items, insertion.Item1, insertion.Item2);
        }

        public void Move(IEnumerable<object> items, object target, NodePosition placement)
        {
            var insertion = GetInsertionLocation(target, placement);
            var path = GetPath(insertion.Item1);
            INbt.DropAll(items, insertion.Item1, insertion.Item2);
            foreach (var item in items)
            {
                NodesRemoved?.Invoke(this, new TreeModelEventArgs(GetParentPath(item), new[] { item }));
                // only works for tags right now
                NodesInserted?.Invoke(this, new TreeModelEventArgs(path, new[] { INbt.IndexOf(insertion.Item1, (NbtTag)item) }, new[] { item }));
            }
            HasUnsavedChanges = true;
        }

        public void NoticeChanges(object obj)
        {
            // currently, changes seem to be reflected without needing to raise NodesChanged
            HasUnsavedChanges = true;
        }

        private TreePath GetPath(object item)
        {
            return View.GetPath(View.FindNodeByTag(item));
        }

        private TreePath GetParentPath(object item)
        {
            return View.GetPath(View.FindNodeByTag(item).Parent);
        }

        public IEnumerable GetChildren(TreePath treePath)
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
            var children = INbt.GetChildren(obj);
            return children;
        }

        private bool HasChildren(object obj)
        {
            var children = GetChildren(obj);
            return children != null && children.Any();
        }

        private static INotifyNode NotifyWrap(NbtTreeModel tree, object obj)
        {
            if (obj is NbtTag tag)
                return NotifyWrapNbt(tree, tag);
            if (obj is NbtFile file)
                return new NotifyNbtFile(tree, file);
            throw new ArgumentException($"Can't create a model node from {obj.GetType()}");
        }

        private static INotifyNbt NotifyWrapNbt(NbtTreeModel tree, NbtTag tag)
        {
            if (tag == null)
                return null;
            if (tag is NbtByte b)
                return new NotifyNbtByte(tree, b);
            if (tag is NbtShort s)
                return new NotifyNbtShort(tree, s);
            if (tag is NbtInt i)
                return new NotifyNbtInt(tree, i);
            if (tag is NbtLong l)
                return new NotifyNbtLong(tree, l);
            if (tag is NbtFloat f)
                return new NotifyNbtFloat(tree, f);
            if (tag is NbtDouble d)
                return new NotifyNbtDouble(tree, d);
            if (tag is NbtString str)
                return new NotifyNbtString(tree, str);
            if (tag is NbtByteArray ba)
                return new NotifyNbtByteArray(tree, ba);
            if (tag is NbtIntArray ia)
                return new NotifyNbtIntArray(tree, ia);
            if (tag is NbtLongArray la)
                return new NotifyNbtLongArray(tree, la);
            if (tag is NbtCompound compound)
                return new NotifyNbtCompound(tree, compound);
            if (tag is NbtList list)
                return new NotifyNbtList(tree, list);
            throw new ArgumentException($"Can't notify wrap {tag.GetType()}");
        }

        public interface INotifyNode
        { }

        public interface INotifyNbt : INotifyNode, INbtTag
        { }

        public abstract class NotifyNode : INotifyNode
        {
            private readonly NbtTreeModel Tree;
            protected NotifyNode(NbtTreeModel tree)
            {
                Tree = tree;
            }
            protected void Notify()
            {
                //Tree.Notify();
            }
            protected INotifyNbt Wrap(NbtTag tag) => NotifyWrapNbt(Tree, tag);
        }

        public class NotifyNbtFile : NotifyNode
        {
            private readonly NbtFile File;
            public NotifyNbtFile(NbtTreeModel tree, NbtFile file) : base(tree)
            {
                File = file;
            }
        }

        public abstract class NotifyNbtTag : NotifyNode, INotifyNbt
        {
            protected readonly NbtTag Tag;
            public NotifyNbtTag(NbtTreeModel tree, NbtTag tag) : base(tree) { Tag = tag; }
            public string Name { get => Tag.Name; set { Tag.Name = value; Notify(); } }
            public NbtTagType TagType => Tag.TagType;
            public INbtContainer Parent => (INbtContainer)Wrap(Tag.Parent);
        }

        public class NotifyNbtByte : NotifyNbtTag, INbtByte
        {
            private new NbtByte Tag => (NbtByte)base.Tag;
            public NotifyNbtByte(NbtTreeModel tree, NbtByte tag) : base(tree, tag) { }
            public byte Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtShort : NotifyNbtTag, INbtShort
        {
            private new NbtShort Tag => (NbtShort)base.Tag;
            public NotifyNbtShort(NbtTreeModel tree, NbtShort tag) : base(tree, tag) { }
            public short Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtInt : NotifyNbtTag, INbtInt
        {
            private new NbtInt Tag => (NbtInt)base.Tag;
            public NotifyNbtInt(NbtTreeModel tree, NbtInt tag) : base(tree, tag) { }
            public int Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtLong : NotifyNbtTag, INbtLong
        {
            private new NbtLong Tag => (NbtLong)base.Tag;
            public NotifyNbtLong(NbtTreeModel tree, NbtLong tag) : base(tree, tag) { }
            public long Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtFloat : NotifyNbtTag, INbtFloat
        {
            private new NbtFloat Tag => (NbtFloat)base.Tag;
            public NotifyNbtFloat(NbtTreeModel tree, NbtFloat tag) : base(tree, tag) { }
            public float Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtDouble : NotifyNbtTag, INbtDouble
        {
            private new NbtDouble Tag => (NbtDouble)base.Tag;
            public NotifyNbtDouble(NbtTreeModel tree, NbtDouble tag) : base(tree, tag) { }
            public double Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtString : NotifyNbtTag, INbtString
        {
            private new NbtString Tag => (NbtString)base.Tag;
            public NotifyNbtString(NbtTreeModel tree, NbtString tag) : base(tree, tag) { }
            public string Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtByteArray : NotifyNbtTag, INbtByteArray
        {
            private new NbtByteArray Tag => (NbtByteArray)base.Tag;
            public NotifyNbtByteArray(NbtTreeModel tree, NbtByteArray tag) : base(tree, tag) { }
            public byte[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtIntArray : NotifyNbtTag, INbtIntArray
        {
            private new NbtIntArray Tag => (NbtIntArray)base.Tag;
            public NotifyNbtIntArray(NbtTreeModel tree, NbtIntArray tag) : base(tree, tag) { }
            public int[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtLongArray : NotifyNbtTag, INbtLongArray
        {
            private new NbtLongArray Tag => (NbtLongArray)base.Tag;
            public NotifyNbtLongArray(NbtTreeModel tree, NbtLongArray tag) : base(tree, tag) { }
            public long[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtList : NotifyNbtTag, INbtList
        {
            private NbtList List => (NbtList)base.Tag;
            public NotifyNbtList(NbtTreeModel tree, NbtList list) : base(tree, list) { }
            public INbtTag this[int index] => Wrap(List[index]);
            public int Count => List.Count;
            public NbtTagType ListType => List.ListType;
            public void Add(NbtTag tag) { List.Add(tag); Notify(); }
            public void AddRange(IEnumerable<NbtTag> tags) { List.AddRange(tags); Notify(); }
            public void Clear() { List.Clear(); Notify(); }
            public bool Contains(NbtTag tag) => List.Contains(tag);
            public bool Remove(NbtTag tag) { if (List.Remove(tag)) { Notify(); return true; } return false; }
            public IEnumerator<INbtTag> GetEnumerator() => List.Select(Wrap).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class NotifyNbtCompound : NotifyNbtTag, INbtCompound
        {
            private NbtCompound Compound => (NbtCompound)base.Tag;
            public NotifyNbtCompound(NbtTreeModel tree, NbtCompound compound) : base(tree, compound) { }
            public int Count => Compound.Count;
            public IEnumerable<INbtTag> Tags => Compound.Tags.Select(Wrap);
            public void Add(NbtTag tag) { Compound.Add(tag); Notify(); }
            public void AddRange(IEnumerable<NbtTag> tags) { Compound.AddRange(tags); Notify(); }
            public void Clear() { Compound.Clear(); Notify(); }
            public bool Contains(NbtTag tag) => Compound.Contains(tag);
            public bool Remove(NbtTag tag) { if (Compound.Remove(tag)) { Notify(); return true; } return false; }
            public IEnumerator<INbtTag> GetEnumerator() => Compound.Select(Wrap).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public bool Contains(string name) => Compound.Contains(name);
            public bool Remove(string name) { if (Compound.Remove(name)) { Notify(); return true; } return false; }
        }
    }
}
