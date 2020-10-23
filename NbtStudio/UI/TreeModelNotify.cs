using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio.UI
{
    public partial class NbtTreeModel
    {
        private static INotifyNode NotifyWrap(NbtTreeModel tree, object obj)
        {
            if (obj is NbtTag tag)
                return NotifyWrapNbt(tree, obj, tag);
            if (obj is ISaveable saveable)
                return NotifyWrapSaveable(tree, obj, saveable);
            if (obj is Chunk chunk)
                return NotifyWrapChunk(tree, obj, chunk);
            throw new ArgumentException($"Can't notify wrap {obj.GetType()}");
        }

        private static NotifyChunk NotifyWrapChunk(NbtTreeModel tree, object original, Chunk chunk)
        {
            return new NotifyChunk(chunk, tree, original);
        }

        private static SaveableNotifyNode NotifyWrapSaveable(NbtTreeModel tree, object original, ISaveable saveable)
        {
            if (saveable is NbtFile file)
                return new NotifyNbtFile(file, tree, original);
            if (saveable is RegionFile region)
                return new NotifyRegionFile(region, tree, original);
            throw new ArgumentException($"Can't notify wrap {saveable.GetType()}");
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
            protected void PerformAction(string description, Action action, Action undo)
            {
                Tree.PushUndo(new UndoableAction(description, action, undo));
                action();
            }
            protected INotifyNbt Wrap(NbtTag tag) => NotifyWrapNbt(Tree, tag, tag);
        }

        public abstract class SaveableNotifyNode : NotifyNode, ISaveable
        {
            protected SaveableNotifyNode(NbtTreeModel tree, object source) : base(tree, source) { }
            public abstract string Path { get; }
            public abstract bool CanSave { get; }
            public abstract void Save();
            public abstract void SaveAs(string path);
            protected abstract void NotifySaved();
        }

        public class NotifyNbtFile : SaveableNotifyNode, INbtFile
        {
            private readonly NbtFile File;
            public NotifyNbtFile(NbtFile file, NbtTreeModel tree, object original) : base(tree, original)
            {
                File = file;
            }
            protected override void NotifySaved()
            {
                Tree.NotifySaved(File);
            }
            public override string Path => File.Path;
            public ExportSettings ExportSettings => File.ExportSettings;
            public override bool CanSave => File.CanSave;
            public override void Save() { File.Save(); NotifySaved(); }
            public override void SaveAs(string path) { File.SaveAs(path); Notify(); NotifySaved(); }
            public void SaveAs(string path, ExportSettings settings) { File.SaveAs(path, settings); Notify(); NotifySaved(); }
            public override bool Equals(object obj)
            {
                return obj.Equals(File);
            }
            public override int GetHashCode()
            {
                return File.GetHashCode();
            }
        }

        public class NotifyChunk : NotifyNode, IChunk
        {
            private readonly Chunk Chunk;
            public NotifyChunk(Chunk chunk, NbtTreeModel tree, object original) : base(tree, original)
            {
                Chunk = chunk;
            }
            public IRegion Region => Chunk.Region;
            public int X => Chunk.X;
            public int Z => Chunk.Z;
            public bool IsLoaded => Chunk.IsLoaded;
            public bool IsCorrupt => Chunk.IsCorrupt;
            public void Load() => Chunk.Load();
            public void Remove()
            {
                var region = Chunk.Region;
                int x = Chunk.X;
                int z = Chunk.Z;
                PerformAction($"Remove {NbtUtil.ChunkDescription(Chunk)}",
                    () => { Chunk.Remove(); Notify(region); },
                    () => { region.AddChunk(Chunk); Notify(region); });
            }
            public void AddTo(IRegion region)
            {
                if (Chunk.Region != null)
                    Remove();
                if (region is NotifyRegionFile)
                    region.AddChunk(Chunk);
                else
                    PerformAction($"Add {NbtUtil.ChunkDescription(Chunk)} to {Path.GetFileName(region.Path)}",
                    () => { region.AddChunk(Chunk); Notify(region); },
                    () => { region.RemoveChunk(Chunk.X, Chunk.Z); Notify(region); });
            }
            public void Move(int x, int z)
            {
                int current_x = Chunk.X;
                int current_z = Chunk.Z;
                PerformAction($"Move {NbtUtil.ChunkDescription(Chunk)} to ({x}, {z})",
                    () => { Chunk.Move(x, z); Notify(); },
                    () => { Chunk.Move(current_x, current_z); Notify(); });
            }
        }

        public class NotifyRegionFile : SaveableNotifyNode, IRegion
        {
            private readonly RegionFile File;
            public NotifyRegionFile(RegionFile file, NbtTreeModel tree, object original) : base(tree, original)
            {
                File = file;
            }
            protected override void NotifySaved()
            {
                Tree.NotifySaved(File);
            }
            public int ChunkCount => File.ChunkCount;
            public IEnumerable<IChunk> AllChunks => File.AllChunks; // to do: notify wrap these
            public IChunk GetChunk(int x, int z) => File.GetChunk(x, z); // to do: notify wrap these
            public void RemoveChunk(int x, int z)
            {
                var chunk = File.GetChunk(x, z);
                if (chunk == null)
                    return;
                PerformAction($"Remove {NbtUtil.ChunkDescription(chunk)}",
                    () => { File.RemoveChunk(x, z); Notify(); },
                    () => { chunk.AddTo(File); Notify(); });
            }
            public void AddChunk(Chunk chunk)
            {
                int x = chunk.X;
                int z = chunk.Z;
                PerformAction($"Add {NbtUtil.ChunkDescription(chunk)} to {System.IO.Path.GetFileName(File.Path)}",
                    () => { File.AddChunk(chunk); Notify(); },
                    () => { File.RemoveChunk(x, z); Notify(); });
            }

            public override string Path => File.Path;
            public override bool CanSave => File.CanSave;
            public override void Save() { File.Save(); NotifySaved(); }
            public override void SaveAs(string path) { File.SaveAs(path); Notify(); NotifySaved(); }
            public override bool Equals(object obj)
            {
                return obj.Equals(File);
            }
            public override int GetHashCode()
            {
                return File.GetHashCode();
            }
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
                    if (current == value) return;
                    PerformAction($"Rename {Tag.TagDescription()} to '{value}'",
                        () => { Tag.Name = value; Notify(); },
                        () => { Tag.Name = current; Notify(); });
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
                    PerformAction($"Remove {Tag.TagDescription()} from {c.TagDescription()}",
                        () => { c.Remove(Tag); Notify(parent); },
                        () => { c.Insert(index, Tag); Notify(parent); });
                }
                else if (parent is NbtList l)
                {
                    PerformAction($"Remove {Tag.TagDescription()} from {l.TagDescription()}",
                        () => { l.Remove(Tag); Notify(parent); },
                        () => { l.Insert(index, Tag); Notify(parent); });
                }
            }
            public void AddTo(INbtContainer container)
            {
                if (Tag.Parent != null)
                    Remove();
                if (container is INotifyNbt)
                    container.Add(Tag);
                else
                    PerformAction($"Add {Tag.TagDescription()} to {container.TagDescription()}",
                    () => { container.Add(Tag); Notify(container); },
                    () => { container.Remove(Tag); Notify(container); });
            }
            public void InsertInto(INbtContainer container, int index)
            {
                if (Tag.Parent != null)
                    Remove();
                if (container is INotifyNbt)
                    container.Insert(index, Tag);
                else
                    PerformAction($"Insert {Tag.TagDescription()} into {container.TagDescription()} at position {index}",
                        () => { container.Insert(index, Tag); Notify(container); },
                        () => { container.Remove(Tag); Notify(container); });
            }
            public bool IsInside(INbtContainer container) => container.Contains(Tag);
            public override bool Equals(object obj) => obj.Equals(Tag);
            public override int GetHashCode() => Tag.GetHashCode();
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {current_value} to {value}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value == value) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from '{current_value}' to '{value}'",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value.SequenceEqual(value)) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {Util.Pluralize(current_value.Length, "byte")} to {Util.Pluralize(value.Length, "byte")}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value.SequenceEqual(value)) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {Util.Pluralize(current_value.Length, "int")} to {Util.Pluralize(value.Length, "int")}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                    if (current_value.SequenceEqual(value)) return;
                    PerformAction($"Change value of {Tag.TagDescription()} from {Util.Pluralize(current_value.Length, "long")} to {Util.Pluralize(value.Length, "long")}",
                        () => { Tag.Value = value; Notify(); },
                        () => { Tag.Value = current_value; Notify(); });
                    Tag.Value = value;
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
                PerformAction($"Add {tag.TagDescription()} to {List.TagDescription()}",
                    () => { List.Add(tag); Notify(); },
                    () => { List.Remove(tag); Notify(); });
            }
            public void AddRange(IEnumerable<NbtTag> tags)
            {
                PerformAction($"Add {NbtUtil.TagDescription(tags)} to {List.TagDescription()}",
                    () => { List.AddRange(tags); Notify(); },
                    () => { foreach (var tag in tags.ToList()) { List.Remove(tag); } Notify(); });
            }
            public void Insert(int index, NbtTag tag)
            {
                PerformAction($"Insert {tag.TagDescription()} into {List.TagDescription()} at position {index}",
                    () => { List.Insert(index, tag); Notify(); },
                    () => { List.Remove(tag); Notify(); });
            }
            public void Clear()
            {
                var tags = List.ToList();
                PerformAction($"Clear all tags from {List.TagDescription()}",
                    () => { List.Clear(); Notify(); },
                    () => { List.AddRange(tags); Notify(); });
            }
            public bool Contains(NbtTag tag) => List.Contains(tag);
            public bool Remove(NbtTag tag)
            {
                int index = List.IndexOf(tag);
                if (index != -1)
                {
                    PerformAction($"Remove {tag.TagDescription()} from {List.TagDescription()}",
                        () => { List.Remove(tag); Notify(); },
                        () => { List.Insert(index, tag); Notify(); });
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
                PerformAction($"Add {tag.TagDescription()} to {Compound.TagDescription()}",
                    () => { Compound.Add(tag); Notify(); },
                    () => { Compound.Remove(tag); Notify(); });
            }
            public void AddRange(IEnumerable<NbtTag> tags)
            {
                PerformAction($"Add {NbtUtil.TagDescription(tags)} to {Compound.TagDescription()}",
                    () => { Compound.AddRange(tags); Notify(); },
                    () => { foreach (var tag in tags.ToList()) { Compound.Remove(tag); } Notify(); });
            }
            public void Insert(int index, NbtTag tag)
            {
                PerformAction($"Insert {tag.TagDescription()} into {Compound.TagDescription()} at position {index}",
                    () => { Compound.Insert(index, tag); Notify(); },
                    () => { Compound.Remove(tag); Notify(); });
            }
            public void Clear()
            {
                var tags = Compound.Tags.ToList();
                PerformAction($"Clear all tags from {Compound.TagDescription()}",
                    () => { Compound.Clear(); Notify(); },
                    () => { Compound.AddRange(tags); Notify(); });
            }
            public bool Contains(NbtTag tag) => Compound.Contains(tag);
            public bool Remove(NbtTag tag)
            {
                int index = Compound.IndexOf(tag);
                if (index != -1)
                {
                    PerformAction($"Remove {tag.TagDescription()} from {Compound.TagDescription()}",
                        () => { Compound.Remove(tag); Notify(); },
                        () => { Compound.Insert(index, tag); Notify(); });
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
                    if (index != -1)
                    {
                        PerformAction($"Remove {tag.TagDescription()} from {Compound.TagDescription()}",
                            () => { Compound.Remove(name); Notify(); },
                            () => { Compound.Insert(index, tag); Notify(); });
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
