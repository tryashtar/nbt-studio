using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface INode
    {
        string Description { get; }
        bool CanDelete { get; }
        void Delete();
        bool CanSort { get; }
        void Sort();
        bool CanCopy { get; }
        string Copy();
        bool CanPaste { get; }
        IEnumerable<INode> Paste(string data);
        bool CanRename { get; }
        bool CanEdit { get; }
    }

    public abstract class NotifyNode : INode
    {
        protected readonly NbtTreeModel Tree;
        protected readonly object SourceObject;
        protected NotifyNode(NbtTreeModel tree, object source)
        {
            Tree = tree;
            SourceObject = source;
        }

        protected virtual void Notify()
        {
            Tree.Notify(SourceObject);
        }
        protected virtual void Notify(object obj)
        {
            Tree.Notify(obj);
        }
        protected void PerformAction(string description, Action action, Action undo)
        {
            Tree.PerformAction(new UndoableAction(description, action, undo));
        }

        public virtual string Description => "unknown node";
        public virtual bool CanDelete => false;
        public virtual void Delete() { }
        public virtual bool CanSort => false;
        public virtual void Sort() { }
        public virtual bool CanCopy => false;
        public virtual string Copy() => null;
        public virtual bool CanPaste => false;
        public virtual IEnumerable<INode> Paste(string data) => Enumerable.Empty<INode>();
        public virtual bool CanRename => false;
        public virtual bool CanEdit => false;
    }

    public class NbtTagNode : NotifyNode
    {
        public readonly NbtTagNodeData Tag;
        public NbtTagNode(NbtTreeModel tree, NbtTag source) : base(tree, source)
        {
            Tag = Wrap(source);
        }

        public static NbtTagNodeData Wrap(NbtTag tag)
        {
            if (tag == null)
                return null;
            if (tag is NbtByte b)
                return new NbtByteNodeData(b);
            if (tag is NbtShort s)
                return new NbtShortNodeData(s);
            if (tag is NbtInt i)
                return new NbtIntNodeData(i);
            if (tag is NbtLong l)
                return new NbtLongNodeData(l);
            if (tag is NbtFloat f)
                return new NbtFloatNodeData(f);
            if (tag is NbtDouble d)
                return new NbtDoubleNodeData(d);
            if (tag is NbtString str)
                return new NbtStringNodeData(str);
            if (tag is NbtByteArray ba)
                return new NbtByteArrayNodeData(ba);
            if (tag is NbtIntArray ia)
                return new NbtIntArrayNodeData(ia);
            if (tag is NbtLongArray la)
                return new NbtLongArrayNodeData(la);
            if (tag is NbtCompound compound)
                return new NbtCompoundNodeData(compound);
            if (tag is NbtList list)
                return new NbtListNodeData(list);
            throw new ArgumentException($"Can't wrap {tag.GetType()}");
        }
    }

    public abstract class NbtTagNodeData : INbtTag
    {
        protected readonly NbtTag Tag;
        public NbtTagNodeData(NbtTag tag) { Tag = tag; }
        public virtual string Name { get => Tag.Name; set => Tag.Name = value; }
        public virtual NbtTagType TagType => Tag.TagType;
        public virtual INbtContainer Parent => (INbtContainer)Tag.Parent.Adapt();
        public virtual int Index
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
        public virtual void Remove()
        {
            if (Tag.Parent is NbtCompound c)
                c.Remove(Tag);
            else if (Tag.Parent is NbtList l)
                l.Remove(Tag);
        }
        public virtual void AddTo(INbtContainer container)
        {
            if (Tag.Parent != null)
                Remove();
            container.Add(Tag);
        }
        public virtual void InsertInto(INbtContainer container, int index)
        {
            if (Tag.Parent != null)
                Remove();
            container.Insert(index, Tag);
        }
        public virtual bool IsInside(INbtContainer container) => container.Contains(Tag);
        public override bool Equals(object obj) => obj.Equals(Tag);
        public override int GetHashCode() => Tag.GetHashCode();
        public override string ToString() => Tag.ToString();
    }

    public class NbtByteNodeData : NbtTagNodeData, INbtByte
    {
        private new NbtByte Tag => (NbtByte)base.Tag;
        public NbtByteNodeData(NbtByte tag) : base(tag) { }
        public virtual byte Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtShortNodeData : NbtTagNodeData, INbtShort
    {
        private new NbtShort Tag => (NbtShort)base.Tag;
        public NbtShortNodeData(NbtShort tag) : base(tag) { }
        public virtual short Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtIntNodeData : NbtTagNodeData, INbtInt
    {
        private new NbtInt Tag => (NbtInt)base.Tag;
        public NbtIntNodeData(NbtInt tag) : base(tag) { }
        public virtual int Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtLongNodeData : NbtTagNodeData, INbtLong
    {
        private new NbtLong Tag => (NbtLong)base.Tag;
        public NbtLongNodeData(NbtLong tag) : base(tag) { }
        public virtual long Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtFloatNodeData : NbtTagNodeData, INbtFloat
    {
        private new NbtFloat Tag => (NbtFloat)base.Tag;
        public NbtFloatNodeData(NbtFloat tag) : base(tag) { }
        public virtual float Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtDoubleNodeData : NbtTagNodeData, INbtDouble
    {
        private new NbtDouble Tag => (NbtDouble)base.Tag;
        public NbtDoubleNodeData(NbtDouble tag) : base(tag) { }
        public virtual double Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtStringNodeData : NbtTagNodeData, INbtString
    {
        private new NbtString Tag => (NbtString)base.Tag;
        public NbtStringNodeData(NbtString tag) : base(tag) { }
        public virtual string Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtByteArrayNodeData : NbtTagNodeData, INbtByteArray
    {
        private new NbtByteArray Tag => (NbtByteArray)base.Tag;
        public NbtByteArrayNodeData(NbtByteArray tag) : base(tag) { }
        public virtual byte[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtIntArrayNodeData : NbtTagNodeData, INbtIntArray
    {
        private new NbtIntArray Tag => (NbtIntArray)base.Tag;
        public NbtIntArrayNodeData(NbtIntArray tag) : base(tag) { }
        public virtual int[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtLongArrayNodeData : NbtTagNodeData, INbtLongArray
    {
        private new NbtLongArray Tag => (NbtLongArray)base.Tag;
        public NbtLongArrayNodeData(NbtLongArray tag) : base(tag) { }
        public virtual long[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtCompoundNodeData : NbtTagNodeData, INbtCompound
    {
        private NbtCompound Compound => (NbtCompound)base.Tag;
        public NbtCompoundNodeData(NbtCompound compound) : base(compound) { }

        public virtual IEnumerable<INbtTag> Tags => Compound.Tags.Select(x => x.Adapt());
        public virtual int Count => Compound.Count;
        public virtual bool CanAdd(NbtTagType type) => true;
        public virtual void Add(NbtTag tag) => Compound.Add(tag);
        public virtual void Insert(int index, NbtTag tag) => Compound.Insert(index, tag);
        public virtual void AddRange(IEnumerable<NbtTag> tags) => Compound.AddRange(tags);
        public virtual void Clear() => Compound.Clear();
        public virtual bool Contains(NbtTag tag) => Compound.Contains(tag);
        public virtual bool Contains(string name) => Compound.Contains(name);

        public virtual bool Remove(NbtTag tag) => Compound.Remove(tag);
        public virtual bool Remove(string name) => Compound.Remove(name);

        public virtual IEnumerator<INbtTag> GetEnumerator() => Compound.Tags.Select(x => x.Adapt()).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class NbtListNodeData : NbtTagNodeData, INbtList
    {
        private NbtList List => (NbtList)base.Tag;
        public NbtListNodeData(NbtList list) : base(list) { }

        public virtual NbtTagType ListType => List.ListType;
        public virtual int Count => List.Count;
        public virtual bool CanAdd(NbtTagType type) => List.Count == 0 || List.ListType == type;
        public virtual void Add(NbtTag tag) => List.Add(tag);
        public virtual void Insert(int index, NbtTag tag) => List.Insert(index, tag);
        public virtual void AddRange(IEnumerable<NbtTag> tags) => List.AddRange(tags);
        public virtual void Clear() => List.Clear();
        public virtual bool Contains(NbtTag tag) => List.Contains(tag);
        public virtual bool Remove(NbtTag tag) => List.Remove(tag);
        public virtual INbtTag this[int index] => List[index].Adapt();
        public virtual IEnumerator<INbtTag> GetEnumerator() => List.Select(x => x.Adapt()).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
