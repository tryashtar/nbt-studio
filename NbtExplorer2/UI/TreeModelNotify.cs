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
