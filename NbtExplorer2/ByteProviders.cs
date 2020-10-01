using Be.Windows.Forms;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public static class ByteProviders
    {
        public static IByteTransformer GetByteProvider(INbtTag tag)
        {
            if (tag is INbtByteArray ba)
                return new ByteArrayByteProvider(ba);
            if (tag is INbtIntArray ia)
                return new IntArrayByteProvider(ia);
            if (tag is INbtLongArray la)
                return new LongArrayByteProvider(la);
            if (tag is INbtList list)
            {
                if (list.ListType == NbtTagType.Byte)
                    return new ByteListByteProvider(list);
                if (list.ListType == NbtTagType.Short)
                    return new ShortListByteProvider(list);
                if (list.ListType == NbtTagType.Int)
                    return new IntListByteProvider(list);
                if (list.ListType == NbtTagType.Long)
                    return new LongListByteProvider(list);
            }
            throw new ArgumentException($"Can't get a byte provider from {tag.TagType}");
        }

        public static bool HasProvider(INbtTag tag)
        {
            if (NbtUtil.IsArrayType(tag.TagType))
                return true;
            if (tag is INbtList list)
            {
                switch (list.ListType)
                {
                    case NbtTagType.Byte:
                    case NbtTagType.Short:
                    case NbtTagType.Int:
                    case NbtTagType.Long:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
    }

    public interface IByteTransformer : IByteProvider
    {
        int BytesPerValue { get; }
    }

    public abstract class NbtByteProvider : IByteTransformer
    {
        protected readonly INbtTag Tag;
        private readonly List<byte> Bytes = new List<byte>();
        private bool HasChanged = false;
        public NbtByteProvider(INbtTag tag)
        {
            Tag = tag;
            Bytes.AddRange(GetBytesFromTag());
        }
        public event EventHandler LengthChanged;
        public event EventHandler Changed;

        protected abstract IEnumerable<byte> GetBytesFromTag();
        protected abstract void SetBytesToTag(List<byte> bytes);
        public abstract int BytesPerValue { get; }

        protected void OnLengthChanged()
        {
            LengthChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void OnChanged()
        {
            HasChanged = true;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public long Length => Bytes.Count;
        public void ApplyChanges()
        {
            SetBytesToTag(Bytes);
            HasChanged = false;
        }

        public void DeleteBytes(long index, long length)
        {
            int internal_index = (int)Math.Max(0, index);
            int internal_length = (int)Math.Min((int)Length, length);
            Bytes.RemoveRange(internal_index, internal_length);
            OnLengthChanged();
            OnChanged();
        }

        public void InsertBytes(long index, byte[] bs)
        {
            Bytes.InsertRange((int)index, bs);
            OnLengthChanged();
            OnChanged();
        }

        public void WriteByte(long index, byte value)
        {
            Bytes[(int)index] = value;
            OnChanged();
        }

        public byte ReadByte(long index) => Bytes[(int)index];

        public bool HasChanges() => HasChanged;
        public bool SupportsDeleteBytes() => true;
        public bool SupportsInsertBytes() => true;
        public bool SupportsWriteByte() => true;
    }

    public class ByteArrayByteProvider : NbtByteProvider
    {
        protected new INbtByteArray Tag => (INbtByteArray)base.Tag;
        public ByteArrayByteProvider(INbtByteArray tag) : base(tag) { }
        public override int BytesPerValue => sizeof(byte);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            return Tag.Value;
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            Tag.Value = bytes.ToArray();
        }
    }

    public class IntArrayByteProvider : NbtByteProvider
    {
        protected new INbtIntArray Tag => (INbtIntArray)base.Tag;
        public IntArrayByteProvider(INbtIntArray tag) : base(tag) { }
        public override int BytesPerValue => sizeof(int);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            return Util.ToByteArray(Tag.Value);
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            Tag.Value = Util.ToIntArray(bytes.ToArray());
        }
    }

    public class LongArrayByteProvider : NbtByteProvider
    {
        protected new INbtLongArray Tag => (INbtLongArray)base.Tag;
        public LongArrayByteProvider(INbtLongArray tag) : base(tag) { }
        public override int BytesPerValue => sizeof(long);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            return Util.ToByteArray(Tag.Value);
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            Tag.Value = Util.ToLongArray(bytes.ToArray());
        }
    }

    public abstract class NbtListByteProvider : NbtByteProvider
    {
        protected new INbtList Tag => (INbtList)base.Tag;
        public NbtListByteProvider(INbtList list) : base(list) { }
    }

    public class ByteListByteProvider : NbtListByteProvider
    {
        public ByteListByteProvider(INbtList list) : base(list) { }
        public override int BytesPerValue => sizeof(byte);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            return Tag.Cast<INbtByte>().Select(x => x.Value);
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            Tag.Clear();
            Tag.AddRange(bytes.Select(x => new NbtByte(x)));
        }
    }

    public class ShortListByteProvider : NbtListByteProvider
    {
        public ShortListByteProvider(INbtList list) : base(list) { }
        public override int BytesPerValue => sizeof(short);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            var shorts = Tag.Cast<INbtShort>().Select(x => x.Value);
            return Util.ToByteArray(shorts.ToArray());
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            var shorts = Util.ToShortArray(bytes.ToArray());
            Tag.Clear();
            Tag.AddRange(shorts.Select(x => new NbtShort(x)));
        }
    }

    public class IntListByteProvider : NbtListByteProvider
    {
        public IntListByteProvider(INbtList list) : base(list) { }
        public override int BytesPerValue => sizeof(int);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            var ints = Tag.Cast<INbtInt>().Select(x => x.Value);
            return Util.ToByteArray(ints.ToArray());
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            var ints = Util.ToIntArray(bytes.ToArray());
            Tag.Clear();
            Tag.AddRange(ints.Select(x => new NbtInt(x)));
        }
    }

    public class LongListByteProvider : NbtListByteProvider
    {
        public LongListByteProvider(INbtList list) : base(list) { }
        public override int BytesPerValue => sizeof(long);

        protected override IEnumerable<byte> GetBytesFromTag()
        {
            var longs = Tag.Cast<INbtLong>().Select(x => x.Value);
            return Util.ToByteArray(longs.ToArray());
        }

        protected override void SetBytesToTag(List<byte> bytes)
        {
            var longs = Util.ToLongArray(bytes.ToArray());
            Tag.Clear();
            Tag.AddRange(longs.Select(x => new NbtLong(x)));
        }
    }
}
