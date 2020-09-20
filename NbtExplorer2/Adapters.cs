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

    public interface INbtCompound : INbtTag
    {
        IEnumerable<INbtTag> Tags { get; }
        int Count { get; }
        void Add(NbtTag tag);
        void AddRange(IEnumerable<NbtTag> tags);
        void Clear();
        bool Contains(NbtTag tag);
        bool Contains(string name);
        bool Remove(NbtTag tag);
        bool Remove(string name);
    }

    public interface INbtList : INbtTag, IReadOnlyList<INbtTag>
    {
        NbtTagType ListType { get; }
        int Count { get; }
        void Add(NbtTag tag);
        void AddRange(IEnumerable<NbtTag> tags);
        void Clear();
        bool Contains(NbtTag tag);
        bool Remove(NbtTag tag);
    }

    public static class Adapters
    {
        public static INbtTag Adapt(this NbtTag tag)
        {
            if (tag is NbtCompound compound)
                return compound.AdaptCompound();
            if (tag is NbtList list)
                return list.AdaptList();
            throw new ArgumentException($"Can't adapt {tag.GetType()}");
        }
        public static INbtCompound AdaptCompound(this NbtCompound compound) => new NbtCompoundAdapter(compound);
        public static INbtList AdaptList(this NbtList list) => new NbtListAdapter(list);
    }

    public class NbtCompoundAdapter : INbtCompound
    {
        private readonly NbtCompound Compound;
        public NbtCompoundAdapter(NbtCompound compound) { Compound = compound; }

        public string Name { get => Compound.Name; set => Compound.Name = value; }
        public NbtTagType TagType => NbtTagType.Compound;
        public IEnumerable<INbtTag> Tags => Compound.Tags.Select(x => x.Adapt());
        public int Count => Compound.Count;
        public void Add(NbtTag tag) => Compound.Add(tag);
        public void AddRange(IEnumerable<NbtTag> tags) => Compound.AddRange(tags);
        public void Clear() => Compound.Clear();
        public bool Contains(NbtTag tag) => Compound.Contains(tag);
        public bool Contains(string name) => Compound.Contains(name);
        public bool Remove(NbtTag tag) => Compound.Remove(tag);
        public bool Remove(string name) => Compound.Remove(name);
    }

    public class NbtListAdapter : INbtList
    {
        private readonly NbtList List;
        public NbtListAdapter(NbtList list) { List = list; }

        public string Name { get => List.Name; set => List.Name = value; }
        public NbtTagType TagType => NbtTagType.Compound;
        public NbtTagType ListType => List.ListType;
        public int Count => List.Count;
        public void Add(NbtTag tag) => List.Add(tag);
        public void AddRange(IEnumerable<NbtTag> tags) => List.AddRange(tags);
        public void Clear() => List.Clear();
        public bool Contains(NbtTag tag) => List.Contains(tag);
        public bool Remove(NbtTag tag) => List.Remove(tag);
        public IEnumerator<INbtTag> GetEnumerator() => List.Select(x => x.Adapt()).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public INbtTag this[int index] => List[index].Adapt();
    }
}
