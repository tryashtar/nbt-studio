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
                return new NotifyNbtFile(file, tree, obj);
            throw new ArgumentException($"Can't notify wrap {obj.GetType()}");
        }

        private static NotifyNbtFile NotifyWrapFile(NbtTreeModel tree, NbtFile file)
        {
            return new NotifyNbtFile(file, tree, file);
        }

        private static INotifyNbt NotifyWrapNbt(NbtTreeModel tree, object original, NbtTag tag)
        {
            if (tag == null)
                return null;
            if (tag is NbtByte b)
                return new NotifyNbtByte(b, tree, original);
            if (tag is NbtShort s)
                return new NotifyNbtShort(s, tree, original);
            if (tag is NbtInt i)
                return new NotifyNbtInt(i, tree, original);
            if (tag is NbtLong l)
                return new NotifyNbtLong(l, tree, original);
            if (tag is NbtFloat f)
                return new NotifyNbtFloat(f, tree, original);
            if (tag is NbtDouble d)
                return new NotifyNbtDouble(d, tree, original);
            if (tag is NbtString str)
                return new NotifyNbtString(str, tree, original);
            if (tag is NbtByteArray ba)
                return new NotifyNbtByteArray(ba, tree, original);
            if (tag is NbtIntArray ia)
                return new NotifyNbtIntArray(ia, tree, original);
            if (tag is NbtLongArray la)
                return new NotifyNbtLongArray(la, tree, original);
            if (tag is NbtCompound compound)
                return new NotifyNbtCompound(compound, tree, original);
            if (tag is NbtList list)
                return new NotifyNbtList(list, tree, original);
            throw new ArgumentException($"Can't notify wrap {tag.GetType()}");
        }

        public interface INotifyNode
        { }

        public interface INotifyNbt : INotifyNode, INbtTag
        { }

        public abstract class NotifyNode : INotifyNode
        {
            protected readonly NbtTreeModel Tree;
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
            protected void Notify(object obj)
            {
                Tree.Notify(obj);
            }
            protected void PushUndo(Action undo)
            {
                Tree.PushUndo(undo);
            }
            protected INotifyNbt Wrap(NbtTag tag) => NotifyWrapNbt(Tree, tag, tag);
        }

        public class NotifyNbtFile : NotifyNode, INbtFile
        {
            private readonly NbtFile File;
            public NotifyNbtFile(NbtFile file, NbtTreeModel tree, object original) : base(tree, original)
            {
                File = file;
            }
            private void NotifySaved()
            {
                Tree.HasUnsavedChanges = false;
            }
            public string Path => File.Path;
            public ExportSettings ExportSettings => File.ExportSettings;
            public bool CanSave => File.CanSave;
            public void Save() { File.Save(); NotifySaved(); }
            public void SaveAs(string path, ExportSettings settings) { File.SaveAs(path, settings); Notify(); NotifySaved(); }
        }

        public abstract class NotifyNbtTag : NotifyNode, INotifyNbt
        {
            protected readonly NbtTag Tag;
            public NotifyNbtTag(NbtTag tag, NbtTreeModel tree, object original) : base(tree, original) { Tag = tag; }
            public string Name
            {
                get => Tag.Name;
                set
                {
                    var current = Tag.Name;
                    PushUndo(() => { Tag.Name = current; Notify(); });
                    Tag.Name = value;
                    Notify();
                }
            }
            public NbtTagType TagType => Tag.TagType;
            public INbtContainer Parent => (INbtContainer)Wrap(Tag.Parent);
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
                var parent = Tag.Parent;
                int index = Index;
                if (parent is NbtCompound c)
                {
                    PushUndo(() => { c.Insert(index, Tag); Notify(parent); });
                    c.Remove(Tag);
                }
                else if (parent is NbtList l)
                {
                    PushUndo(() => { l.Insert(index, Tag); Notify(parent); });
                    l.Remove(Tag);
                }
                Notify(parent);
            }
            public void AddTo(INbtContainer container)
            {
                if (Tag.Parent != null)
                    Remove();
                PushUndo(() => { container.Remove(Tag); Notify(container); });
                container.Add(Tag);
                Notify(container);
            }
            public void InsertInto(INbtContainer container, int index)
            {
                if (Tag.Parent != null)
                    Remove();
                PushUndo(() => { container.Remove(Tag); Notify(container); });
                container.Insert(index, Tag);
                Notify(container);
            }
            public bool IsInside(INbtContainer container) => container.Contains(Tag);
        }

        public class NotifyNbtByte : NotifyNbtTag, INbtByte
        {
            private new NbtByte Tag => (NbtByte)base.Tag;
            public NotifyNbtByte(NbtByte tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public byte Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtShort : NotifyNbtTag, INbtShort
        {
            private new NbtShort Tag => (NbtShort)base.Tag;
            public NotifyNbtShort(NbtShort tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public short Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtInt : NotifyNbtTag, INbtInt
        {
            private new NbtInt Tag => (NbtInt)base.Tag;
            public NotifyNbtInt(NbtInt tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public int Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtLong : NotifyNbtTag, INbtLong
        {
            private new NbtLong Tag => (NbtLong)base.Tag;
            public NotifyNbtLong(NbtLong tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public long Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtFloat : NotifyNbtTag, INbtFloat
        {
            private new NbtFloat Tag => (NbtFloat)base.Tag;
            public NotifyNbtFloat(NbtFloat tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public float Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtDouble : NotifyNbtTag, INbtDouble
        {
            private new NbtDouble Tag => (NbtDouble)base.Tag;
            public NotifyNbtDouble(NbtDouble tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public double Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtString : NotifyNbtTag, INbtString
        {
            private new NbtString Tag => (NbtString)base.Tag;
            public NotifyNbtString(NbtString tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public string Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtByteArray : NotifyNbtTag, INbtByteArray
        {
            private new NbtByteArray Tag => (NbtByteArray)base.Tag;
            public NotifyNbtByteArray(NbtByteArray tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public byte[] Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtIntArray : NotifyNbtTag, INbtIntArray
        {
            private new NbtIntArray Tag => (NbtIntArray)base.Tag;
            public NotifyNbtIntArray(NbtIntArray tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public int[] Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtLongArray : NotifyNbtTag, INbtLongArray
        {
            private new NbtLongArray Tag => (NbtLongArray)base.Tag;
            public NotifyNbtLongArray(NbtLongArray tag, NbtTreeModel tree, object original) : base(tag, tree, original) { }
            public long[] Value
            {
                get => Tag.Value;
                set
                {
                    var current_value = Tag.Value;
                    PushUndo(() => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
                    Notify();
                }
            }
        }

        public class NotifyNbtList : NotifyNbtTag, INbtList
        {
            private NbtList List => (NbtList)base.Tag;
            public NotifyNbtList(NbtList list, NbtTreeModel tree, object original) : base(list, tree, original) { }
            public INbtTag this[int index] => Wrap(List[index]);
            public int Count => List.Count;
            public NbtTagType ListType => List.ListType;
            public bool CanAdd(NbtTagType type) => List.Count == 0 || List.ListType == type;
            public void Add(NbtTag tag)
            {
                PushUndo(() => { List.Remove(tag); Notify(); });
                List.Add(tag);
                Notify();
            }
            public void AddRange(IEnumerable<NbtTag> tags)
            {
                PushUndo(() => { foreach (var tag in tags.ToList()) { List.Remove(tag); } Notify(); });
                List.AddRange(tags);
                Notify();
            }
            public void Insert(int index, NbtTag tag)
            {
                PushUndo(() => { List.Remove(tag); Notify(); });
                List.Insert(index, tag);
                Notify();
            }
            public void Clear()
            {
                var tags = List.ToList();
                PushUndo(() => { List.AddRange(tags); Notify(); });
                List.Clear();
                Notify();
            }
            public bool Contains(NbtTag tag) => List.Contains(tag);
            public bool Remove(NbtTag tag)
            {
                int index = List.IndexOf(tag);
                if (List.Remove(tag))
                {
                    PushUndo(() => { List.Insert(index, tag); Notify(); });
                    Notify();
                    return true;
                }
                return false;
            }
            public IEnumerator<INbtTag> GetEnumerator() => List.Select(Wrap).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class NotifyNbtCompound : NotifyNbtTag, INbtCompound
        {
            private NbtCompound Compound => (NbtCompound)base.Tag;
            public NotifyNbtCompound(NbtCompound compound, NbtTreeModel tree, object original) : base(compound, tree, original) { }
            public int Count => Compound.Count;
            public IEnumerable<INbtTag> Tags => Compound.Tags.Select(Wrap);
            public bool CanAdd(NbtTagType type) => true;
            public void Add(NbtTag tag)
            {
                PushUndo(() => { Compound.Remove(tag); Notify(); });
                Compound.Add(tag);
                Notify();
            }
            public void AddRange(IEnumerable<NbtTag> tags)
            {
                PushUndo(() => { foreach (var tag in tags.ToList()) { Compound.Remove(tag); } Notify(); });
                Compound.AddRange(tags);
                Notify();
            }
            public void Insert(int index, NbtTag tag)
            {
                PushUndo(() => { Compound.Remove(tag); Notify(); });
                Compound.Insert(index, tag);
                Notify();
            }
            public void Clear()
            {
                var tags = Compound.Tags.ToList();
                PushUndo(() => { Compound.AddRange(tags); Notify(); });
                Compound.Clear();
                Notify();
            }
            public bool Contains(NbtTag tag) => Compound.Contains(tag);
            public bool Remove(NbtTag tag)
            {
                int index = Compound.IndexOf(tag);
                if (Compound.Remove(tag))
                {
                    PushUndo(() => { Compound.Insert(index, tag); Notify(); });
                    Notify();
                    return true;
                }
                return false;
            }
            public IEnumerator<INbtTag> GetEnumerator() => Tags.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public bool Contains(string name) => Compound.Contains(name);
            public bool Remove(string name)
            {
                if (Compound.TryGet(name, out var tag))
                {
                    int index = Compound.IndexOf(tag);
                    PushUndo(() => { Compound.Insert(index, tag); Notify(); });
                    Compound.Remove(name);
                    Notify();
                    return true;
                }
                return false;
            }
        }
    }
}
