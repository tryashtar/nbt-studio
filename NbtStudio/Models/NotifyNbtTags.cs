using fNbt;
using NbtStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    // Unwrap is not good, let's at least keep it here where we know it's "safe"
    public static class NbtUnwrapExtensions
    {
        public static void AddTo(this INbtTag tag, INbtContainer container)
        {
            container.Add(tag.Unwrap());
        }

        public static void InsertInto(this INbtTag tag, INbtContainer container, int index)
        {
            container.Insert(index, tag.Unwrap());
        }

        public static int GetIndex(this INbtTag tag)
        {
            if (tag.Parent == null)
                return -1;
            return tag.Parent.IndexOf(tag.Unwrap());
        }

        public static bool IsInside(this INbtTag tag, INbtContainer container)
        {
            return container.Contains(tag.Unwrap());
        }

        public static void Remove(this INbtTag tag)
        {
            if (tag.Parent != null)
                tag.Parent.Remove(tag.Unwrap());
        }
    }

    public abstract class NotifyNbtTag : INbtTag
    {
        protected readonly NbtTag Tag;
        public event EventHandler<UndoableAction> ActionPrepared;
        public event EventHandler Changed;
        public NotifyNbtTag(NbtTag tag)
        {
            Tag = tag;
        }

        public NbtTag Unwrap() => Tag;

        protected void RaiseChanged(INbtTag tag) => Changed?.Invoke(tag, EventArgs.Empty);
        protected void RaiseChanged() => RaiseChanged(Tag);
        protected void PrepareAction(INbtTag tag, UndoableAction action)
        {
            ActionPrepared?.Invoke(tag, action);
        }
        protected void PrepareAction(INbtTag tag, string description, Action action, Action undo)
        {
            PrepareAction(tag, new UndoableAction(description, action, undo));
        }
        protected void PrepareAction(string description, Action action, Action undo) => PrepareAction(Tag, description, action, undo);

        public string Name
        {
            get => Tag.Name;
            set
            {
                var current = Tag.Name;
                if (current == value) return;
                PrepareAction($"Rename {Tag.TagDescription()} to '{value}'",
                    () => { Tag.Name = value; RaiseChanged(); },
                    () => { Tag.Name = current; RaiseChanged(); });
            }
        }
        public NbtTagType TagType => Tag.TagType;
        public INbtContainer Parent => (INbtContainer)Wrap(Tag.Parent);

        public static NotifyNbtTag CreateFrom(NbtTag tag)
        {
            if (tag is NbtByte b)
                return new NotifyNbtByte(b);
            if (tag is NbtShort s)
                return new NotifyNbtShort(s);
            if (tag is NbtInt i)
                return new NotifyNbtInt(i);
            if (tag is NbtLong l)
                return new NotifyNbtLong(l);
            if (tag is NbtFloat f)
                return new NotifyNbtFloat(f);
            if (tag is NbtDouble d)
                return new NotifyNbtDouble(d);
            if (tag is NbtString str)
                return new NotifyNbtString(str);
            if (tag is NbtByteArray ba)
                return new NotifyNbtByteArray(ba);
            if (tag is NbtIntArray ia)
                return new NotifyNbtIntArray(ia);
            if (tag is NbtLongArray la)
                return new NotifyNbtLongArray(la);
            if (tag is NbtCompound compound)
                return new NotifyNbtCompound(compound);
            if (tag is NbtList list)
                return new NotifyNbtList(list);
            throw new ArgumentException($"Can't wrap {tag.TagType}");
        }

        public NotifyNbtTag Wrap(NbtTag tag)
        {
            var result = CreateFrom(tag);
            result.ActionPrepared += (_, e) => PrepareAction(result, e);
            result.Changed += (_, e) => RaiseChanged(result);
            return result;
        }
    }

    public class NotifyNbtByte : NotifyNbtTag, INbtByte
    {
        private new NbtByte Tag => (NbtByte)base.Tag;
        public NotifyNbtByte(NbtByte tag) : base(tag) { }
        public byte Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtShort : NotifyNbtTag, INbtShort
    {
        private new NbtShort Tag => (NbtShort)base.Tag;
        public NotifyNbtShort(NbtShort tag) : base(tag) { }
        public short Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtInt : NotifyNbtTag, INbtInt
    {
        private new NbtInt Tag => (NbtInt)base.Tag;
        public NotifyNbtInt(NbtInt tag) : base(tag) { }
        public int Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtLong : NotifyNbtTag, INbtLong
    {
        private new NbtLong Tag => (NbtLong)base.Tag;
        public NotifyNbtLong(NbtLong tag) : base(tag) { }
        public long Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtFloat : NotifyNbtTag, INbtFloat
    {
        private new NbtFloat Tag => (NbtFloat)base.Tag;
        public NotifyNbtFloat(NbtFloat tag) : base(tag) { }
        public float Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtDouble : NotifyNbtTag, INbtDouble
    {
        private new NbtDouble Tag => (NbtDouble)base.Tag;
        public NotifyNbtDouble(NbtDouble tag) : base(tag) { }
        public double Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtString : NotifyNbtTag, INbtString
    {
        private new NbtString Tag => (NbtString)base.Tag;
        public NotifyNbtString(NbtString tag) : base(tag) { }
        public String Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtByteArray : NotifyNbtTag, INbtByteArray
    {
        private new NbtByteArray Tag => (NbtByteArray)base.Tag;
        public NotifyNbtByteArray(NbtByteArray tag) : base(tag) { }
        public byte[] Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtIntArray : NotifyNbtTag, INbtIntArray
    {
        private new NbtIntArray Tag => (NbtIntArray)base.Tag;
        public NotifyNbtIntArray(NbtIntArray tag) : base(tag) { }
        public int[] Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtLongArray : NotifyNbtTag, INbtLongArray
    {
        private new NbtLongArray Tag => (NbtLongArray)base.Tag;
        public NotifyNbtLongArray(NbtLongArray tag) : base(tag) { }
        public long[] Value
        {
            get => Tag.Value;
            set
            {
                var current_value = Tag.Value;
                if (current_value == value) return;
                PrepareAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                    () => { Tag.Value = value; RaiseChanged(); },
                    () => { Tag.Value = current_value; RaiseChanged(); });
            }
        }
    }

    public class NotifyNbtList : NotifyNbtTag, INbtList
    {
        private new NbtList Tag => (NbtList)base.Tag;
        public NotifyNbtList(NbtList tag) : base(tag) { }

        // read-only
        public NbtTagType ListType => Tag.ListType;
        public int Count => Tag.Count;
        public bool CanAdd(NbtTagType type) => Tag.CanAdd(type);
        public bool Contains(NbtTag tag) => Tag.Contains(tag);
        public int IndexOf(NbtTag tag) => Tag.IndexOf(tag);

        // tag wrapping
        public INbtTag this[int index] => Wrap(Tag[index]);
        public IEnumerator<INbtTag> GetEnumerator() => ((IEnumerable<NbtTag>)Tag).Select(Wrap).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // modifications
        public void Add(NbtTag tag)
        {
            PrepareAction($"Add {tag.TagDescription()} to {Tag.TagDescription()}",
                () => { Tag.Add(tag); RaiseChanged(); },
                () => { Tag.Remove(tag); RaiseChanged(); });
        }
        public void AddRange(IEnumerable<NbtTag> tags)
        {
            PrepareAction($"Add {NbtUtil.TagDescription(tags)} to {Tag.TagDescription()}",
                () => { Tag.AddRange(tags); RaiseChanged(); },
                () => { foreach (var tag in tags.ToList()) { Tag.Remove(tag); } RaiseChanged(); });
        }
        public void Insert(int index, NbtTag tag)
        {
            PrepareAction($"Insert {tag.TagDescription()} into {Tag.TagDescription()} at position {index}",
                () => { Tag.Insert(index, tag); RaiseChanged(); },
                () => { Tag.Remove(tag); RaiseChanged(); });
        }
        public void Clear()
        {
            var tags = ((IEnumerable<NbtTag>)Tag).ToList();
            PrepareAction($"Clear all tags from {Tag.TagDescription()}",
                () => { Tag.Clear(); RaiseChanged(); },
                () => { Tag.AddRange(tags); RaiseChanged(); });
        }
        public bool Remove(NbtTag tag)
        {
            int index = Tag.IndexOf(tag);
            if (index != -1)
            {
                PrepareAction($"Remove {tag.TagDescription()} from {Tag.TagDescription()}",
                    () => { Tag.Remove(tag); RaiseChanged(); },
                    () => { Tag.Insert(index, tag); RaiseChanged(); });
                return true;
            }
            return false;
        }
    }

    public class NotifyNbtCompound : NotifyNbtTag, INbtCompound
    {
        private new NbtCompound Tag => (NbtCompound)base.Tag;
        public NotifyNbtCompound(NbtCompound tag) : base(tag) { }

        // read-only
        public int Count => Tag.Count;
        public bool CanAdd(NbtTagType type) => Tag.CanAdd(type);
        public bool Contains(NbtTag tag) => Tag.Contains(tag);
        public bool Contains(string name) => Tag.Contains(name);
        public int IndexOf(NbtTag tag) => Tag.IndexOf(tag);
        public int IndexOf(string name) => Tag.IndexOf(name);

        // tag wrapping
        public INbtTag this[int index] => Wrap(Tag[index]);
        public IEnumerable<INbtTag> Tags => ((IEnumerable<NbtTag>)Tag).Select(Wrap);
        public IEnumerator<INbtTag> GetEnumerator() => Tags.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // modifications
        public void Add(NbtTag tag)
        {
            PrepareAction($"Add {tag.TagDescription()} to {Tag.TagDescription()}",
                () => { Tag.Add(tag); RaiseChanged(); },
                () => { Tag.Remove(tag); RaiseChanged(); });
        }
        public void AddRange(IEnumerable<NbtTag> tags)
        {
            PrepareAction($"Add {NbtUtil.TagDescription(tags)} to {Tag.TagDescription()}",
                () => { Tag.AddRange(tags); RaiseChanged(); },
                () => { foreach (var tag in tags.ToList()) { Tag.Remove(tag); } RaiseChanged(); });
        }
        public void Insert(int index, NbtTag tag)
        {
            PrepareAction($"Insert {tag.TagDescription()} into {Tag.TagDescription()} at position {index}",
                () => { Tag.Insert(index, tag); RaiseChanged(); },
                () => { Tag.Remove(tag); RaiseChanged(); });
        }
        public void Clear()
        {
            var tags = ((IEnumerable<NbtTag>)Tag).ToList();
            PrepareAction($"Clear all tags from {Tag.TagDescription()}",
                () => { Tag.Clear(); RaiseChanged(); },
                () => { Tag.AddRange(tags); RaiseChanged(); });
        }
        public bool Remove(NbtTag tag)
        {
            int index = Tag.IndexOf(tag);
            if (index != -1)
            {
                PrepareAction($"Remove {tag.TagDescription()} from {Tag.TagDescription()}",
                    () => { Tag.Remove(tag); RaiseChanged(); },
                    () => { Tag.Insert(index, tag); RaiseChanged(); });
                return true;
            }
            return false;
        }
        public bool Remove(string name)
        {
            if (Tag.TryGet(name, out var tag))
                return Remove(tag);
            else
                return false;
        }
    }
}
