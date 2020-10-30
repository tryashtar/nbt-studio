using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface INbtTag
    {
        string Name { get; set; }
        NbtTagType TagType { get; }
        INbtContainer Parent { get; }
        int Index { get; }
        void Remove();
        void AddTo(INbtContainer container);
        void InsertInto(INbtContainer container, int index);
        bool IsInside(INbtContainer container);
    }

    public interface INbtContainer : INbtTag, IReadOnlyCollection<INbtTag>
    {
        bool CanAdd(NbtTagType type);
        void Add(NbtTag tag);
        void AddRange(IEnumerable<NbtTag> tags);
        void Insert(int index, NbtTag tag);
        void Clear();
        bool Contains(NbtTag tag);
        bool Remove(NbtTag tag);
    }

    public interface INbtByte : INbtTag
    {
        byte Value { get; set; }
    }

    public interface INbtShort : INbtTag
    {
        short Value { get; set; }
    }

    public interface INbtInt : INbtTag
    {
        int Value { get; set; }
    }

    public interface INbtLong : INbtTag
    {
        long Value { get; set; }
    }

    public interface INbtFloat : INbtTag
    {
        float Value { get; set; }
    }

    public interface INbtDouble : INbtTag
    {
        double Value { get; set; }
    }

    public interface INbtString : INbtTag
    {
        string Value { get; set; }
    }

    public interface INbtByteArray : INbtTag
    {
        byte[] Value { get; set; }
    }

    public interface INbtIntArray : INbtTag
    {
        int[] Value { get; set; }
    }

    public interface INbtLongArray : INbtTag
    {
        long[] Value { get; set; }
    }

    public interface INbtCompound : INbtContainer
    {
        IEnumerable<INbtTag> Tags { get; }
        bool Contains(string name);
        bool Remove(string name);
    }

    public interface INbtList : INbtContainer, IReadOnlyList<INbtTag>
    {
        NbtTagType ListType { get; }
    }

    public static class Wrappers
    {
        public static INbtTag Adapt(this NbtTag tag)
        {
            if (tag == null)
                return null;
            if (tag is NbtByte b)
                return b.AdaptByte();
            if (tag is NbtShort s)
                return s.AdaptShort();
            if (tag is NbtInt i)
                return i.AdaptInt();
            if (tag is NbtLong l)
                return l.AdaptLong();
            if (tag is NbtFloat f)
                return f.AdaptFloat();
            if (tag is NbtDouble d)
                return d.AdaptDouble();
            if (tag is NbtString str)
                return str.AdaptString();
            if (tag is NbtByteArray ba)
                return ba.AdaptByteArray();
            if (tag is NbtIntArray ia)
                return ia.AdaptIntArray();
            if (tag is NbtLongArray la)
                return la.AdaptLongArray();
            if (tag is NbtCompound compound)
                return compound.AdaptCompound();
            if (tag is NbtList list)
                return list.AdaptList();
            throw new ArgumentException($"Can't adapt {tag.GetType()}");
        }
        public static INbtByte AdaptByte(this NbtByte tag) => new NbtByteWrapper(tag);
        public static INbtShort AdaptShort(this NbtShort tag) => new NbtShortWrapper(tag);
        public static INbtInt AdaptInt(this NbtInt tag) => new NbtIntWrapper(tag);
        public static INbtLong AdaptLong(this NbtLong tag) => new NbtLongWrapper(tag);
        public static INbtFloat AdaptFloat(this NbtFloat tag) => new NbtFloatWrapper(tag);
        public static INbtDouble AdaptDouble(this NbtDouble tag) => new NbtDoubleWrapper(tag);
        public static INbtString AdaptString(this NbtString tag) => new NbtStringWrapper(tag);
        public static INbtByteArray AdaptByteArray(this NbtByteArray tag) => new NbtByteArrayWrapper(tag);
        public static INbtIntArray AdaptIntArray(this NbtIntArray tag) => new NbtIntArrayWrapper(tag);
        public static INbtLongArray AdaptLongArray(this NbtLongArray tag) => new NbtLongArrayWrapper(tag);
        public static INbtCompound AdaptCompound(this NbtCompound compound) => new NbtCompoundWrapper(compound);
        public static INbtList AdaptList(this NbtList list) => new NbtListWrapper(list);
    }

    public abstract class NbtTagWrapper : INbtTag
    {
        protected readonly NbtTag Tag;
        public NbtTagWrapper(NbtTag tag) { Tag = tag; }
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

    public class NbtByteWrapper : NbtTagWrapper, INbtByte
    {
        private new NbtByte Tag => (NbtByte)base.Tag;
        public NbtByteWrapper(NbtByte tag) : base(tag) { }
        public virtual byte Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtShortWrapper : NbtTagWrapper, INbtShort
    {
        private new NbtShort Tag => (NbtShort)base.Tag;
        public NbtShortWrapper(NbtShort tag) : base(tag) { }
        public virtual short Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtIntWrapper : NbtTagWrapper, INbtInt
    {
        private new NbtInt Tag => (NbtInt)base.Tag;
        public NbtIntWrapper(NbtInt tag) : base(tag) { }
        public virtual int Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtLongWrapper : NbtTagWrapper, INbtLong
    {
        private new NbtLong Tag => (NbtLong)base.Tag;
        public NbtLongWrapper(NbtLong tag) : base(tag) { }
        public virtual long Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtFloatWrapper : NbtTagWrapper, INbtFloat
    {
        private new NbtFloat Tag => (NbtFloat)base.Tag;
        public NbtFloatWrapper(NbtFloat tag) : base(tag) { }
        public virtual float Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtDoubleWrapper : NbtTagWrapper, INbtDouble
    {
        private new NbtDouble Tag => (NbtDouble)base.Tag;
        public NbtDoubleWrapper(NbtDouble tag) : base(tag) { }
        public virtual double Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtStringWrapper : NbtTagWrapper, INbtString
    {
        private new NbtString Tag => (NbtString)base.Tag;
        public NbtStringWrapper(NbtString tag) : base(tag) { }
        public virtual string Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtByteArrayWrapper : NbtTagWrapper, INbtByteArray
    {
        private new NbtByteArray Tag => (NbtByteArray)base.Tag;
        public NbtByteArrayWrapper(NbtByteArray tag) : base(tag) { }
        public virtual byte[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtIntArrayWrapper : NbtTagWrapper, INbtIntArray
    {
        private new NbtIntArray Tag => (NbtIntArray)base.Tag;
        public NbtIntArrayWrapper(NbtIntArray tag) : base(tag) { }
        public virtual int[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtLongArrayWrapper : NbtTagWrapper, INbtLongArray
    {
        private new NbtLongArray Tag => (NbtLongArray)base.Tag;
        public NbtLongArrayWrapper(NbtLongArray tag) : base(tag) { }
        public virtual long[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtCompoundWrapper : NbtTagWrapper, INbtCompound
    {
        private NbtCompound Compound => (NbtCompound)base.Tag;
        public NbtCompoundWrapper(NbtCompound compound) : base(compound) { }

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

    public class NbtListWrapper : NbtTagWrapper, INbtList
    {
        private NbtList List => (NbtList)base.Tag;
        public NbtListWrapper(NbtList list) : base(list) { }

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
