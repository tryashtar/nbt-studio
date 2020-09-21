using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2.UI
{
    public partial class NbtTreeModel
    {
        private static INotifyNode NotifyWrap(NbtTreeModel tree, object obj, object parent)
        {
            if (obj is NbtTag tag)
                return NotifyWrapNbt(tree, obj, parent, tag);
            if (obj is NbtFile file)
                return new NotifyNbtFile(file, tree, obj, parent);
            throw new ArgumentException($"Can't notify wrap {obj.GetType()}");
        }

        private static INotifyNbt NotifyWrapNbt(NbtTreeModel tree, object original, object parent, NbtTag tag)
        {
            if (tag == null)
                return null;
            if (tag is NbtByte b)
                return new NotifyNbtByte(b, tree, original, parent);
            if (tag is NbtShort s)
                return new NotifyNbtShort(s, tree, original, parent);
            if (tag is NbtInt i)
                return new NotifyNbtInt(i, tree, original, parent);
            if (tag is NbtLong l)
                return new NotifyNbtLong(l, tree, original, parent);
            if (tag is NbtFloat f)
                return new NotifyNbtFloat(f, tree, original, parent);
            if (tag is NbtDouble d)
                return new NotifyNbtDouble(d, tree, original, parent);
            if (tag is NbtString str)
                return new NotifyNbtString(str, tree, original, parent);
            if (tag is NbtByteArray ba)
                return new NotifyNbtByteArray(ba, tree, original, parent);
            if (tag is NbtIntArray ia)
                return new NotifyNbtIntArray(ia, tree, original, parent);
            if (tag is NbtLongArray la)
                return new NotifyNbtLongArray(la, tree, original, parent);
            if (tag is NbtCompound compound)
                return new NotifyNbtCompound(compound, tree, original, parent);
            if (tag is NbtList list)
                return new NotifyNbtList(list, tree, original, parent);
            throw new ArgumentException($"Can't notify wrap {tag.GetType()}");
        }

        public interface INotifyNode
        { }

        public interface INotifyNbt : INotifyNode, INbtTag
        { }

        public abstract class NotifyNode : INotifyNode
        {
            private readonly NbtTreeModel Tree;
            private readonly object SourceObject;
            private readonly object ParentObject;
            protected NotifyNode(NbtTreeModel tree, object source, object parent)
            {
                Tree = tree;
                SourceObject = source;
                ParentObject = parent;
            }
            protected void Notify()
            {
                Tree.Notify(SourceObject);
            }
            protected void NotifyParent()
            {
                if (ParentObject != null)
                    Tree.Notify(ParentObject);
            }
            protected INotifyNbt Wrap(NbtTag tag, NbtTag parent) => NotifyWrapNbt(Tree, tag, parent, tag);
        }

        public class NotifyNbtFile : NotifyNode
        {
            private readonly NbtFile File;
            public NotifyNbtFile(NbtFile file, NbtTreeModel tree, object original, object parent) : base(tree, original, parent)
            {
                File = file;
            }
        }

        public abstract class NotifyNbtTag : NotifyNode, INotifyNbt
        {
            protected readonly NbtTag Tag;
            public NotifyNbtTag(NbtTag tag, NbtTreeModel tree, object original, object parent) : base(tree, original, parent) { Tag = tag; }
            public string Name { get => Tag.Name; set { Tag.Name = value; Notify(); } }
            public NbtTagType TagType => Tag.TagType;
            public INbtContainer Parent => (INbtContainer)Wrap(Tag.Parent, Tag.Parent?.Parent);
            public int Index
            {
                get
                {
                    if (Tag.Parent is NbtCompound c)
                        return c.IndexOf(Tag);
                    else if (Tag.Parent is NbtList l)
                        return l.IndexOf(Tag);
                    return -1;
                }
            }
            public void Remove()
            {
                if (Tag.Parent is NbtCompound c)
                    c.Remove(Tag);
                else if (Tag.Parent is NbtList l)
                    l.Remove(Tag);
                NotifyParent();
            }
            public void AddTo(INbtContainer container)
            {
                if (Tag.Parent != null)
                    Remove();
                container.Add(Tag);
                NotifyParent();
            }
            public void InsertInto(INbtContainer container, int index)
            {
                if (Tag.Parent != null)
                    Remove();
                container.Insert(index, Tag);
                NotifyParent();
            }
        }

        public class NotifyNbtByte : NotifyNbtTag, INbtByte
        {
            private new NbtByte Tag => (NbtByte)base.Tag;
            public NotifyNbtByte(NbtByte tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public byte Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtShort : NotifyNbtTag, INbtShort
        {
            private new NbtShort Tag => (NbtShort)base.Tag;
            public NotifyNbtShort(NbtShort tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public short Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtInt : NotifyNbtTag, INbtInt
        {
            private new NbtInt Tag => (NbtInt)base.Tag;
            public NotifyNbtInt(NbtInt tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public int Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtLong : NotifyNbtTag, INbtLong
        {
            private new NbtLong Tag => (NbtLong)base.Tag;
            public NotifyNbtLong(NbtLong tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public long Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtFloat : NotifyNbtTag, INbtFloat
        {
            private new NbtFloat Tag => (NbtFloat)base.Tag;
            public NotifyNbtFloat(NbtFloat tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public float Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtDouble : NotifyNbtTag, INbtDouble
        {
            private new NbtDouble Tag => (NbtDouble)base.Tag;
            public NotifyNbtDouble(NbtDouble tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public double Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtString : NotifyNbtTag, INbtString
        {
            private new NbtString Tag => (NbtString)base.Tag;
            public NotifyNbtString(NbtString tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public string Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtByteArray : NotifyNbtTag, INbtByteArray
        {
            private new NbtByteArray Tag => (NbtByteArray)base.Tag;
            public NotifyNbtByteArray(NbtByteArray tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public byte[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtIntArray : NotifyNbtTag, INbtIntArray
        {
            private new NbtIntArray Tag => (NbtIntArray)base.Tag;
            public NotifyNbtIntArray(NbtIntArray tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public int[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtLongArray : NotifyNbtTag, INbtLongArray
        {
            private new NbtLongArray Tag => (NbtLongArray)base.Tag;
            public NotifyNbtLongArray(NbtLongArray tag, NbtTreeModel tree, object original, object parent) : base(tag, tree, original, parent) { }
            public long[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtList : NotifyNbtTag, INbtList
        {
            private NbtList List => (NbtList)base.Tag;
            public NotifyNbtList(NbtList list, NbtTreeModel tree, object original, object parent) : base(list, tree, original, parent) { }
            public INbtTag this[int index] => Wrap(List[index], List);
            public int Count => List.Count;
            public NbtTagType ListType => List.ListType;
            public bool CanAdd(NbtTagType type) => List.Count == 0 || List.ListType == type;
            public void Add(NbtTag tag) { List.Add(tag); Notify(); }
            public void AddRange(IEnumerable<NbtTag> tags) { List.AddRange(tags); Notify(); }
            public void Insert(int index, NbtTag tag) { List.Insert(index, tag); Notify(); }
            public void Clear() { List.Clear(); Notify(); }
            public bool Contains(NbtTag tag) => List.Contains(tag);
            public bool Remove(NbtTag tag) { if (List.Remove(tag)) { Notify(); return true; } return false; }
            public IEnumerator<INbtTag> GetEnumerator() => List.Select(x => Wrap(x, List)).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class NotifyNbtCompound : NotifyNbtTag, INbtCompound
        {
            private NbtCompound Compound => (NbtCompound)base.Tag;
            public NotifyNbtCompound(NbtCompound compound, NbtTreeModel tree, object original, object parent) : base(compound, tree, original, parent) { }
            public int Count => Compound.Count;
            public IEnumerable<INbtTag> Tags => Compound.Tags.Select(x => Wrap(x, Compound));
            public bool CanAdd(NbtTagType type) => true;
            public void Add(NbtTag tag) { Compound.Add(tag); Notify(); }
            public void AddRange(IEnumerable<NbtTag> tags) { Compound.AddRange(tags); Notify(); }
            public void Insert(int index, NbtTag tag) { Compound.Insert(index, tag); Notify(); }
            public void Clear() { Compound.Clear(); Notify(); }
            public bool Contains(NbtTag tag) => Compound.Contains(tag);
            public bool Remove(NbtTag tag) { if (Compound.Remove(tag)) { Notify(); return true; } return false; }
            public IEnumerator<INbtTag> GetEnumerator() => Tags.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public bool Contains(string name) => Compound.Contains(name);
            public bool Remove(string name) { if (Compound.Remove(name)) { Notify(); return true; } return false; }
        }
    }
}
