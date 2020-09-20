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
        private static INotifyNode NotifyWrap(NbtTreeModel tree, object obj)
        {
            if (obj is NbtTag tag)
                return NotifyWrapNbt(tree, obj, tag);
            if (obj is NbtFile file)
                return new NotifyNbtFile(tree, obj, file);
            throw new ArgumentException($"Can't notify wrap {obj.GetType()}");
        }

        private static INotifyNbt NotifyWrapNbt(NbtTreeModel tree, object original, NbtTag tag)
        {
            if (tag == null)
                return null;
            if (tag is NbtByte b)
                return new NotifyNbtByte(tree, original, b);
            if (tag is NbtShort s)
                return new NotifyNbtShort(tree, original, s);
            if (tag is NbtInt i)
                return new NotifyNbtInt(tree, original, i);
            if (tag is NbtLong l)
                return new NotifyNbtLong(tree, original, l);
            if (tag is NbtFloat f)
                return new NotifyNbtFloat(tree, original, f);
            if (tag is NbtDouble d)
                return new NotifyNbtDouble(tree, original, d);
            if (tag is NbtString str)
                return new NotifyNbtString(tree, original, str);
            if (tag is NbtByteArray ba)
                return new NotifyNbtByteArray(tree, original, ba);
            if (tag is NbtIntArray ia)
                return new NotifyNbtIntArray(tree, original, ia);
            if (tag is NbtLongArray la)
                return new NotifyNbtLongArray(tree, original, la);
            if (tag is NbtCompound compound)
                return new NotifyNbtCompound(tree, original, compound);
            if (tag is NbtList list)
                return new NotifyNbtList(tree, original, list);
            throw new ArgumentException($"Can't notify wrap {tag.GetType()}");
        }

        public interface INotifyNode
        { }

        public interface INotifyNbt : INotifyNode, INbtTag
        {
            void Remove();
        }

        public abstract class NotifyNode : INotifyNode
        {
            private readonly NbtTreeModel Tree;
            private readonly object SourceObject;
            protected NotifyNode(NbtTreeModel tree, object source)
            {
                Tree = tree;
                SourceObject = source;
            }
            protected void Notify()
            {
                Tree.Notify(SourceObject);
            }
            protected INotifyNbt Wrap(NbtTag tag) => NotifyWrapNbt(Tree, tag, tag);
        }

        public class NotifyNbtFile : NotifyNode
        {
            private readonly NbtFile File;
            public NotifyNbtFile(NbtTreeModel tree, object original, NbtFile file) : base(tree, original)
            {
                File = file;
            }
        }

        public abstract class NotifyNbtTag : NotifyNode, INotifyNbt
        {
            protected readonly NbtTag Tag;
            public NotifyNbtTag(NbtTreeModel tree, object original, NbtTag tag) : base(tree, original) { Tag = tag; }
            public string Name { get => Tag.Name; set { Tag.Name = value; Notify(); } }
            public NbtTagType TagType => Tag.TagType;
            public INbtContainer Parent => (INbtContainer)Wrap(Tag.Parent);
            public void Remove()
            {
                if (Tag.Parent is NbtCompound c)
                    c.Remove(Tag);
                else if (Tag.Parent is NbtList l)
                    l.Remove(Tag);
                Notify();
            }
        }

        public class NotifyNbtByte : NotifyNbtTag, INbtByte
        {
            private new NbtByte Tag => (NbtByte)base.Tag;
            public NotifyNbtByte(NbtTreeModel tree, object original, NbtByte tag) : base(tree, original, tag) { }
            public byte Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtShort : NotifyNbtTag, INbtShort
        {
            private new NbtShort Tag => (NbtShort)base.Tag;
            public NotifyNbtShort(NbtTreeModel tree, object original, NbtShort tag) : base(tree, original, tag) { }
            public short Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtInt : NotifyNbtTag, INbtInt
        {
            private new NbtInt Tag => (NbtInt)base.Tag;
            public NotifyNbtInt(NbtTreeModel tree, object original, NbtInt tag) : base(tree, original, tag) { }
            public int Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtLong : NotifyNbtTag, INbtLong
        {
            private new NbtLong Tag => (NbtLong)base.Tag;
            public NotifyNbtLong(NbtTreeModel tree, object original, NbtLong tag) : base(tree, original, tag) { }
            public long Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtFloat : NotifyNbtTag, INbtFloat
        {
            private new NbtFloat Tag => (NbtFloat)base.Tag;
            public NotifyNbtFloat(NbtTreeModel tree, object original, NbtFloat tag) : base(tree, original, tag) { }
            public float Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtDouble : NotifyNbtTag, INbtDouble
        {
            private new NbtDouble Tag => (NbtDouble)base.Tag;
            public NotifyNbtDouble(NbtTreeModel tree, object original, NbtDouble tag) : base(tree, original, tag) { }
            public double Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtString : NotifyNbtTag, INbtString
        {
            private new NbtString Tag => (NbtString)base.Tag;
            public NotifyNbtString(NbtTreeModel tree, object original, NbtString tag) : base(tree, original, tag) { }
            public string Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtByteArray : NotifyNbtTag, INbtByteArray
        {
            private new NbtByteArray Tag => (NbtByteArray)base.Tag;
            public NotifyNbtByteArray(NbtTreeModel tree, object original, NbtByteArray tag) : base(tree, original, tag) { }
            public byte[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtIntArray : NotifyNbtTag, INbtIntArray
        {
            private new NbtIntArray Tag => (NbtIntArray)base.Tag;
            public NotifyNbtIntArray(NbtTreeModel tree, object original, NbtIntArray tag) : base(tree, original, tag) { }
            public int[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtLongArray : NotifyNbtTag, INbtLongArray
        {
            private new NbtLongArray Tag => (NbtLongArray)base.Tag;
            public NotifyNbtLongArray(NbtTreeModel tree, object original, NbtLongArray tag) : base(tree, original, tag) { }
            public long[] Value { get => Tag.Value; set { Tag.Value = value; Notify(); } }
        }

        public class NotifyNbtList : NotifyNbtTag, INbtList
        {
            private NbtList List => (NbtList)base.Tag;
            public NotifyNbtList(NbtTreeModel tree, object original, NbtList list) : base(tree, original, list) { }
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
            public NotifyNbtCompound(NbtTreeModel tree, object original, NbtCompound compound) : base(tree, original, compound) { }
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
