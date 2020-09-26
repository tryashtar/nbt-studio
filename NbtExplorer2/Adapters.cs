using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
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

    public static class Adapters
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
        public static INbtByte AdaptByte(this NbtByte tag) => new NbtByteAdapter(tag);
        public static INbtShort AdaptShort(this NbtShort tag) => new NbtShortAdapter(tag);
        public static INbtInt AdaptInt(this NbtInt tag) => new NbtIntAdapter(tag);
        public static INbtLong AdaptLong(this NbtLong tag) => new NbtLongAdapter(tag);
        public static INbtFloat AdaptFloat(this NbtFloat tag) => new NbtFloatAdapter(tag);
        public static INbtDouble AdaptDouble(this NbtDouble tag) => new NbtDoubleAdapter(tag);
        public static INbtString AdaptString(this NbtString tag) => new NbtStringAdapter(tag);
        public static INbtByteArray AdaptByteArray(this NbtByteArray tag) => new NbtByteArrayAdapter(tag);
        public static INbtIntArray AdaptIntArray(this NbtIntArray tag) => new NbtIntArrayAdapter(tag);
        public static INbtLongArray AdaptLongArray(this NbtLongArray tag) => new NbtLongArrayAdapter(tag);
        public static INbtCompound AdaptCompound(this NbtCompound compound) => new NbtCompoundAdapter(compound);
        public static INbtList AdaptList(this NbtList list) => new NbtListAdapter(list);
    }

    public abstract class NbtTagAdapter : INbtTag
    {
        protected readonly NbtTag Tag;
        public NbtTagAdapter(NbtTag tag) { Tag = tag; }
        public string Name { get => Tag.Name; set => Tag.Name = value; }
        public NbtTagType TagType => Tag.TagType;
        public INbtContainer Parent => (INbtContainer)Tag.Parent.Adapt();
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
        }
        public void AddTo(INbtContainer container)
        {
            if (Tag.Parent != null)
                Remove();
            container.Add(Tag);
        }
        public void InsertInto(INbtContainer container, int index)
        {
            if (Tag.Parent != null)
                Remove();
            container.Insert(index, Tag);
        }
        public bool IsInside(INbtContainer container) => container.Contains(Tag);
        public override bool Equals(object obj) => obj.Equals(Tag);
        public override int GetHashCode() => Tag.GetHashCode();
    }

    public class NbtByteAdapter : NbtTagAdapter, INbtByte
    {
        private new NbtByte Tag => (NbtByte)base.Tag;
        public NbtByteAdapter(NbtByte tag) : base(tag) { }
        public byte Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtShortAdapter : NbtTagAdapter, INbtShort
    {
        private new NbtShort Tag => (NbtShort)base.Tag;
        public NbtShortAdapter(NbtShort tag) : base(tag) { }
        public short Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtIntAdapter : NbtTagAdapter, INbtInt
    {
        private new NbtInt Tag => (NbtInt)base.Tag;
        public NbtIntAdapter(NbtInt tag) : base(tag) { }
        public int Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtLongAdapter : NbtTagAdapter, INbtLong
    {
        private new NbtLong Tag => (NbtLong)base.Tag;
        public NbtLongAdapter(NbtLong tag) : base(tag) { }
        public long Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtFloatAdapter : NbtTagAdapter, INbtFloat
    {
        private new NbtFloat Tag => (NbtFloat)base.Tag;
        public NbtFloatAdapter(NbtFloat tag) : base(tag) { }
        public float Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtDoubleAdapter : NbtTagAdapter, INbtDouble
    {
        private new NbtDouble Tag => (NbtDouble)base.Tag;
        public NbtDoubleAdapter(NbtDouble tag) : base(tag) { }
        public double Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtStringAdapter : NbtTagAdapter, INbtString
    {
        private new NbtString Tag => (NbtString)base.Tag;
        public NbtStringAdapter(NbtString tag) : base(tag) { }
        public string Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtByteArrayAdapter : NbtTagAdapter, INbtByteArray
    {
        private new NbtByteArray Tag => (NbtByteArray)base.Tag;
        public NbtByteArrayAdapter(NbtByteArray tag) : base(tag) { }
        public byte[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtIntArrayAdapter : NbtTagAdapter, INbtIntArray
    {
        private new NbtIntArray Tag => (NbtIntArray)base.Tag;
        public NbtIntArrayAdapter(NbtIntArray tag) : base(tag) { }
        public int[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtLongArrayAdapter : NbtTagAdapter, INbtLongArray
    {
        private new NbtLongArray Tag => (NbtLongArray)base.Tag;
        public NbtLongArrayAdapter(NbtLongArray tag) : base(tag) { }
        public long[] Value { get => Tag.Value; set => Tag.Value = value; }
    }

    public class NbtCompoundAdapter : NbtTagAdapter, INbtCompound
    {
        private NbtCompound Compound => (NbtCompound)base.Tag;
        public NbtCompoundAdapter(NbtCompound compound) : base(compound) { }

        public IEnumerable<INbtTag> Tags => Compound.Tags.Select(x => x.Adapt());
        public int Count => Compound.Count;
        public bool CanAdd(NbtTagType type) => true;
        public void Add(NbtTag tag) => Compound.Add(tag);
        public void Insert(int index, NbtTag tag) => Compound.Insert(index, tag);
        public void AddRange(IEnumerable<NbtTag> tags) => Compound.AddRange(tags);
        public void Clear() => Compound.Clear();
        public bool Contains(NbtTag tag) => Compound.Contains(tag);
        public bool Contains(string name) => Compound.Contains(name);


        public bool Remove(NbtTag tag) => Compound.Remove(tag);
        public bool Remove(string name) => Compound.Remove(name);

        public IEnumerator<INbtTag> GetEnumerator() => Compound.Tags.Select(x => x.Adapt()).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class NbtListAdapter : NbtTagAdapter, INbtList
    {
        private NbtList List => (NbtList)base.Tag;
        public NbtListAdapter(NbtList list) : base(list) { }

        public NbtTagType ListType => List.ListType;
        public int Count => List.Count;
        public bool CanAdd(NbtTagType type) => List.Count == 0 || List.ListType == type;
        public void Add(NbtTag tag) => List.Add(tag);
        public void Insert(int index, NbtTag tag) => List.Insert(index, tag);
        public void AddRange(IEnumerable<NbtTag> tags) => List.AddRange(tags);
        public void Clear() => List.Clear();
        public bool Contains(NbtTag tag) => List.Contains(tag);
        public bool Remove(NbtTag tag) => List.Remove(tag);
        public IEnumerator<INbtTag> GetEnumerator() => List.Select(x => x.Adapt()).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public INbtTag this[int index] => List[index].Adapt();
    }
}
