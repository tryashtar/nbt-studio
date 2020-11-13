using fNbt;
using Microsoft.VisualBasic.FileIO;
using NbtStudio;
using NbtStudio.SNBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public interface INode
    {
        object Object { get; }
        string Description { get; }
        bool CanDelete { get; }
        void Delete();
        bool CanSort { get; }
        void Sort();
        bool CanCopy { get; }
        DataObject Copy();
        bool CanCut { get; }
        DataObject Cut();
        bool CanPaste { get; }
        IEnumerable<INode> Paste(IDataObject data);
        bool CanRename { get; }
        bool CanEdit { get; }
        bool CanReceiveDrop(IEnumerable<INode> nodes);
        void ReceiveDrop(IEnumerable<INode> nodes, int index);
    }

    public static class NodeExtractions
    {
        public static Dictionary<Type, (string singular, string plural)> NodeTypes = new Dictionary<Type, (string, string)>
        {
            { typeof(NbtTagNode), ("tag", "tags") },
            { typeof(NbtFileNode), ("file", "files") },
            { typeof(ChunkNode), ("chunk", "chunks") },
            { typeof(RegionFileNode), ("region file", "region files") },
            { typeof(FolderNode), ("folder", "folders") }
        };

        // user-friendly description of multiple nodes
        // uses the node-specific description for single collections (like "byte tag named 'whatever'")
        // otherwise builds a string like "4 tags, 3 files, 1 chunk, 2 unknown nodes"
        public static string Description(IEnumerable<INode> nodes)
        {
            if (!nodes.Any()) // none
                return "0 nodes";
            if (Util.ExactlyOne(nodes)) // exactly one
                return nodes.Single().Description;
            var results = new Dictionary<Type, int>();
            int unknowns = 0;
            foreach (var node in nodes)
            {
                var type = node.GetType();
                if (NodeTypes.ContainsKey(type))
                {
                    if (results.ContainsKey(type))
                        results[type]++;
                    else
                        results[type] = 1;
                }
                else
                    unknowns++;
            }
            var strings = new List<string>();
            foreach (var item in results)
            {
                var (singular, plural) = NodeTypes[item.Key];
                strings.Add(Util.Pluralize(item.Value, singular, plural));
            }
            if (unknowns > 0)
                strings.Add(Util.Pluralize(unknowns, "unknown node"));
            return String.Join(", ", strings);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<INode> nodes, Func<INode, T> transformer)
        {
            return nodes.Select(transformer).Where(x => x != null);
        }

        public static NbtTag GetNbtTag(this INode node)
        {
            if (node is NbtTagNode nbt)
                return nbt.Tag;
            if (node is NbtFileNode file)
                return file.File.RootTag;
            if (node is ChunkNode chunk)
                return chunk.Chunk.Data;
            return null;
        }

        public static ISaveable GetSaveable(this INode node)
        {
            if (node is NbtFileNode file)
                return file.File;
            if (node is RegionFileNode region)
                return region.Region;
            return null;
        }

        public static IHavePath GetHasPath(this INode node)
        {
            if (node is FolderNode folder)
                return folder.Folder;
            return GetSaveable(node);
        }

        public static NbtFile GetNbtFile(this INode node)
        {
            if (node is NbtFileNode file)
                return file.File;
            return null;
        }

        public static RegionFile GetRegionFile(this INode node)
        {
            if (node is RegionFileNode region)
                return region.Region;
            return null;
        }

        public static Chunk GetChunk(this INode node)
        {
            if (node is ChunkNode chunk)
                return chunk.Chunk;
            return null;
        }

        public static NbtFolder GetNbtFolder(this INode node)
        {
            if (node is FolderNode folder)
                return folder.Folder;
            return null;
        }
    }

    public abstract class NotifyNode : INode, IDisposable
    {
        private readonly NbtTreeModel Tree;
        protected readonly object SourceObject;
        public object Object => SourceObject;
        protected NotifyNode(NbtTreeModel tree, object source)
        {
            Tree = tree;
            SourceObject = source;
            Subscribe();
        }

        protected abstract void Subscribe();
        protected abstract void Unsubscribe();

        public virtual void Dispose()
        {
            Unsubscribe();
        }

        protected void Notify()
        {
            Tree.Notify(SourceObject);
        }
        protected void Notify(object obj)
        {
            Tree.Notify(obj);
        }
        protected void RemoveSelf()
        {
            Tree.Remove(SourceObject);
        }
        protected void NoticeAction(UndoableAction action)
        {
            Tree.SaveAction(action);
        }
        protected NotifyNode Wrap(object obj) => Create(Tree, obj);

        public static NotifyNode Create(NbtTreeModel tree, object obj)
        {
            if (obj is NbtTag tag)
                return new NbtTagNode(tree, tag);
            else if (obj is NbtFile file)
                return new NbtFileNode(tree, file);
            else if (obj is RegionFile region)
                return new RegionFileNode(tree, region);
            else if (obj is Chunk chunk)
                return new ChunkNode(tree, chunk);
            else if (obj is NbtFolder folder)
                return new FolderNode(tree, folder);
            throw new ArgumentException($"Can't create node from {obj.GetType()}");
        }

        public virtual string Description => "unknown node";
        public virtual bool CanDelete => false;
        public virtual void Delete()
        {
            RemoveSelf();
        }
        public virtual bool CanSort => false;
        public virtual void Sort() { }
        public virtual bool CanCopy => false;
        public virtual DataObject Copy() => null;
        public virtual bool CanCut => CanDelete && CanCopy;
        public virtual DataObject Cut() { var copy = Copy(); Delete(); return copy; }
        public virtual bool CanPaste => false;
        public virtual IEnumerable<INode> Paste(IDataObject data) => Enumerable.Empty<INode>();
        public virtual bool CanRename => false;
        public virtual bool CanEdit => false;
        public virtual bool CanReceiveDrop(IEnumerable<INode> nodes) => false;
        public virtual void ReceiveDrop(IEnumerable<INode> nodes, int index) { }
    }

    public static class NbtNodeOperations
    {
        public static DataObject Copy(NbtTag tag)
        {
            var data = new DataObject();
            data.SetText(tag.ToSnbt(SnbtOptions.Default, include_name: true));
            return data;
        }

        public static bool CanEdit(NbtTag tag)
        {
            return true;
        }

        public static bool CanPaste(NbtTag tag)
        {
            return tag is NbtContainerTag;
        }

        public static bool CanRename(NbtTag tag)
        {
            return !(tag.Parent is NbtList);
        }

        public static bool CanSort(NbtTag tag)
        {
            return tag is NbtCompound;
        }

        public static void Sort(NbtTag tag)
        {
            if (tag is NbtCompound compound)
                NbtUtil.Sort(compound, new NbtUtil.TagTypeSorter(), true);
        }

        public static IEnumerable<NbtTag> ParseTags(IDataObject data)
        {
            var text = (string)data.GetData(typeof(string));
            if (text == null)
                yield break;
            var snbts = text.Split('\n');
            foreach (var nbt in snbts)
            {
                if (SnbtParser.TryParse(nbt, true, out NbtTag tag) || SnbtParser.TryParse(nbt, false, out tag))
                    yield return tag;
            }
        }

        public static IEnumerable<NbtTag> Paste(NbtTag tag, IDataObject data)
        {
            if (tag is NbtContainerTag container)
            {
                var tags = ParseTags(data).ToList();
                NbtUtil.TransformAdd(tags, container);
                return tags;
            }
            return Enumerable.Empty<NbtTag>();
        }

        public static bool CanReceiveDrop(NbtTag tag, IEnumerable<NbtTag> tags)
        {
            if (!(tag is NbtContainerTag container))
                return false;
            return NbtUtil.CanAddAll(tags, container);
        }

        public static void ReceiveDrop(NbtTag tag, IEnumerable<NbtTag> tags, int index)
        {
            if (tag is NbtContainerTag container)
                NbtUtil.TransformInsert(tags, container, index);
        }
    }

    public static class FileNodeOperations
    {
        public static string Description(string path)
        {
            return Path.GetFileName(path);
        }

        public static bool DeleteFile(string path)
        {
            if (path == null || !File.Exists(path))
                return true;
            try
            {
                FileSystem.DeleteFile(path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch { return false; }
            return true;
        }

        public static bool DeleteFolder(string path)
        {
            if (path == null || !Directory.Exists(path))
                return true;
            try
            {
                FileSystem.DeleteDirectory(path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch { return false; }
            return true;
        }

        public static DataObject Copy(string path)
        {
            var data = new DataObject();
            if (path != null)
            {
                data.SetFileDropList(new StringCollection { path });
                data.SetData("Preferred DropEffect", new MemoryStream(BitConverter.GetBytes((int)DragDropEffects.Copy)));
            }
            return data;
        }

        public static DataObject Cut(string path)
        {
            var data = new DataObject();
            if (path != null)
            {
                data.SetFileDropList(new StringCollection { path });
                data.SetData("Preferred DropEffect", new MemoryStream(BitConverter.GetBytes((int)DragDropEffects.Move)));
            }
            return data;
        }
    }

    public class NbtTagNode : NotifyNode
    {
        public NbtTag Tag => (NbtTag)SourceObject;
        public NbtTagNode(NbtTreeModel tree, NbtTag tag) : base(tree, tag)
        { }

        protected override void Subscribe()
        {
            Tag.Changed += Tag_Changed;
            Tag.ActionPerformed += Tag_ActionPerformed;
        }

        protected override void Unsubscribe()
        {
            Tag.Changed -= Tag_Changed;
            Tag.ActionPerformed -= Tag_ActionPerformed;
        }

        private void Tag_Changed(object sender, NbtTag e)
        {
            Notify(e);
        }

        private void Tag_ActionPerformed(object sender, (NbtTag tag, UndoableAction action) e)
        {
            if (Tag == e.tag)
                NoticeAction(e.action);
        }

        public override string Description => Tag.TagDescription();

        public override bool CanCopy => true;
        public override DataObject Copy() => NbtNodeOperations.Copy(Tag);
        public override bool CanDelete => true;
        public override void Delete()
        {
            Tag.Remove();
            base.Delete();
        }
        public override bool CanEdit => NbtNodeOperations.CanEdit(Tag);
        public override bool CanPaste => NbtNodeOperations.CanPaste(Tag);
        public override bool CanRename => NbtNodeOperations.CanRename(Tag);
        public override bool CanSort => NbtNodeOperations.CanSort(Tag);
        public override void Sort() => NbtNodeOperations.Sort(Tag);
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(Tag, data);
            return tags.Select(Wrap);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode) && NbtNodeOperations.CanReceiveDrop(Tag, nodes.Filter(x => x.GetNbtTag()));
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(Tag, tags, index);
        }
    }

    public class NbtFileNode : NotifyNode
    {
        public NbtFile File => (NbtFile)SourceObject;
        public NbtFileNode(NbtTreeModel tree, NbtFile file) : base(tree, file)
        { }

        protected override void Subscribe()
        {
            File.OnSaved += File_OnSaved;
            File.RootTag.Changed += RootTag_Changed;
            File.RootTag.ActionPerformed += RootTag_ActionPerformed;
        }

        protected override void Unsubscribe()
        {
            File.OnSaved -= File_OnSaved;
            File.RootTag.Changed -= RootTag_Changed;
            File.RootTag.ActionPerformed -= RootTag_ActionPerformed;
        }

        private void RootTag_Changed(object sender, NbtTag e)
        {
            Notify(e);
        }

        private void RootTag_ActionPerformed(object sender, (NbtTag tag, UndoableAction action) e)
        {
            if (File.RootTag == e.tag)
                NoticeAction(e.action);
        }

        private void File_OnSaved(object sender, EventArgs e)
        {
            Notify();
        }

        public override string Description => File.Path == null ? "unsaved file" : Path.GetFileName(File.Path);

        public override bool CanCopy => true;
        public override DataObject Copy()
        {
            var data1 = NbtNodeOperations.Copy(File.RootTag);
            var data2 = FileNodeOperations.Copy(File.Path);
            return Util.Merge(data1, data2);
        }
        public override bool CanCut => true;
        public override DataObject Cut()
        {
            var data1 = NbtNodeOperations.Copy(File.RootTag);
            var data2 = FileNodeOperations.Cut(File.Path);
            return Util.Merge(data1, data2);
        }
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (FileNodeOperations.DeleteFile(File.Path))
                base.Delete();
        }
        public override bool CanEdit => File.Path != null;
        public override bool CanPaste => NbtNodeOperations.CanPaste(File.RootTag);
        public override bool CanRename => File.Path != null;
        public override bool CanSort => NbtNodeOperations.CanSort(File.RootTag);
        public override void Sort() => NbtNodeOperations.Sort(File.RootTag);
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(File.RootTag, data);
            return tags.Select(Wrap);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode) && NbtNodeOperations.CanReceiveDrop(File.RootTag, nodes.Filter(x => x.GetNbtTag()));
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(File.RootTag, tags, index);
        }
    }

    public class ChunkNode : NotifyNode
    {
        public Chunk Chunk => (Chunk)SourceObject;
        private bool HasSetupEvents = false;
        public ChunkNode(NbtTreeModel tree, Chunk chunk) : base(tree, chunk)
        {
            if (Chunk.IsLoaded)
                SetupEvents();
        }

        protected override void Subscribe()
        {
            if (!Chunk.IsLoaded)
                Chunk.OnLoaded += Chunk_OnLoaded;
        }

        private void SetupEvents()
        {
            if (!HasSetupEvents)
            {
                Chunk.Data.Changed += Data_Changed;
                Chunk.Data.ActionPerformed += Data_ActionPerformed;
                HasSetupEvents = true;
            }
        }

        protected override void Unsubscribe()
        {
            Chunk.OnLoaded -= Chunk_OnLoaded;
            Chunk.Data.Changed -= Data_Changed;
            Chunk.Data.ActionPerformed -= Data_ActionPerformed;
            HasSetupEvents = false;
        }

        private void Chunk_OnLoaded(object sender, EventArgs e)
        {
            SetupEvents();
        }

        private void Data_Changed(object sender, NbtTag e)
        {
            Notify(e);
        }

        private void Data_ActionPerformed(object sender, (NbtTag tag, UndoableAction action) e)
        {
            if (Chunk.Data == e.tag)
                NoticeAction(e.action);
        }

        public NbtCompound AccessChunkData()
        {
            if (!Chunk.IsLoaded)
                Chunk.Load();
            return Chunk.Data;
        }

        public override string Description => NbtUtil.ChunkDescription(Chunk);

        public override bool CanCopy => !Chunk.IsExternal;
        public override DataObject Copy() => NbtNodeOperations.Copy(AccessChunkData());
        public override bool CanDelete => !Chunk.IsExternal;
        public override void Delete()
        {
            var region = Chunk.Region;
            if (region != null)
            {
                var action = new UndoableAction(new DescriptionHolder("Remove {0}", Chunk),
                    () => { region.RemoveChunk(Chunk.X, Chunk.Z); base.Delete(); },
                    () => { region.AddChunk(Chunk); }
                );
                action.Do();
                NoticeAction(action);
            }
        }
        public override bool CanEdit => !Chunk.IsExternal;
        public override bool CanPaste => !Chunk.IsExternal;
        public override bool CanRename => !Chunk.IsExternal;
        public override bool CanSort => !Chunk.IsExternal;
        public override void Sort() => NbtNodeOperations.Sort(AccessChunkData());
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(AccessChunkData(), data);
            return tags.Select(Wrap);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode);
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(AccessChunkData(), tags, index);
        }
    }

    public class RegionFileNode : NotifyNode
    {
        public RegionFile Region => (RegionFile)SourceObject;
        public RegionFileNode(NbtTreeModel tree, RegionFile region) : base(tree, region)
        { }

        protected override void Subscribe()
        {
            Region.OnSaved += Region_OnSaved;
            Region.ChunksChanged += Region_ChunksChanged;
        }

        protected override void Unsubscribe()
        {
            Region.OnSaved -= Region_OnSaved;
            Region.ChunksChanged -= Region_ChunksChanged;
        }

        private void Region_OnSaved(object sender, EventArgs e)
        {
            Notify();
        }

        private void Region_ChunksChanged(object sender, EventArgs e)
        {
            Notify(sender);
        }

        public override string Description => Region.Path == null ? "unsaved region file" : Path.GetFileName(Region.Path);

        public override bool CanCopy => Region.Path != null;
        public override DataObject Copy()
        {
            var data = new DataObject();
            if (Region.Path != null)
                data.SetFileDropList(new StringCollection { Region.Path });
            return data;
        }
        public override bool CanCut => Region.Path != null;
        public override DataObject Cut() => FileNodeOperations.Cut(Region.Path);
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (FileNodeOperations.DeleteFile(Region.Path))
                base.Delete();
        }
        public override bool CanEdit => Region.Path != null;
        public override bool CanPaste => true;
        public override bool CanRename => Region.Path != null;
        public override bool CanSort => false;
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.ParseTags(data).OfType<NbtCompound>().ToList();
            var available = Region.GetAvailableCoords();
            var chunks = Enumerable.Zip(available, tags, (slot, tag) => Chunk.EmptyChunk(tag, slot.x, slot.z));
            foreach (var chunk in chunks)
            {
                Region.AddChunk(chunk);
            }
            return chunks.Select(Wrap);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is ChunkNode);
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var chunks = nodes.Filter(x => x.GetChunk());
            foreach (var chunk in chunks)
            {
                Region.AddChunk(chunk);
            }
        }
    }

    public class FolderNode : NotifyNode
    {
        public NbtFolder Folder => (NbtFolder)SourceObject;
        public FolderNode(NbtTreeModel tree, NbtFolder folder) : base(tree, folder)
        {  }

        public override void Dispose()
        {
            base.Dispose();
            Folder.Dispose();
        }

        protected override void Subscribe()
        {
            Folder.ContentsChanged += Folder_ContentsChanged;
        }

        protected override void Unsubscribe()
        {
            Folder.ContentsChanged -= Folder_ContentsChanged;
        }

        private void Folder_ContentsChanged(object sender, EventArgs e)
        {
            Notify();
        }

        public override string Description => Path.GetFileName(Folder.Path);

        public override bool CanCopy => true;
        public override DataObject Copy() => FileNodeOperations.Copy(Folder.Path);
        public override bool CanCut => true;
        public override DataObject Cut() => FileNodeOperations.Cut(Folder.Path);
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (FileNodeOperations.DeleteFolder(Folder.Path))
                base.Delete();
        }
        public override bool CanEdit => true;
        public override bool CanPaste => true;
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var files = (string[])data.GetData("FileDrop");
            var drop_effect = (MemoryStream)data.GetData("Preferred DropEffect");
            if (files == null || drop_effect == null)
                return Enumerable.Empty<INode>();
            var bytes = new byte[4];
            drop_effect.Read(bytes, 0, bytes.Length);
            var drop = (DragDropEffects)BitConverter.ToInt32(bytes, 0);
            bool move = drop.HasFlag(DragDropEffects.Move);
            foreach (var item in files)
            {
                var destination = Util.GetUniqueFilename(Path.Combine(Folder.Path, Path.GetFileName(item)));
                if (move)
                {
                    if (Directory.Exists(item))
                        FileSystem.MoveDirectory(item, destination, UIOption.AllDialogs);
                    else if (File.Exists(item))
                        FileSystem.MoveFile(item, destination, UIOption.AllDialogs);
                }
                else
                {
                    if (Directory.Exists(item))
                        FileSystem.CopyDirectory(item, destination, UIOption.AllDialogs);
                    else if (File.Exists(item))
                        FileSystem.CopyFile(item, destination, UIOption.AllDialogs);
                }
            }
            Folder.Scan();
            return Folder.Files.Where(x => files.Contains(x.Path)).Select(Wrap);
        }
        public override bool CanRename => true;
        public override bool CanSort => false;
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x.GetSaveable() != null || x is FolderNode);
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var files = nodes.Filter(x => x.GetSaveable());
            var folders = nodes.Filter(x => x.GetNbtFolder());
            foreach (var file in files)
            {
                if (file.Path != null)
                {
                    var destination = Path.Combine(Folder.Path, Path.GetFileName(file.Path));
                    FileSystem.MoveFile(file.Path, destination, UIOption.AllDialogs);
                }
            }
            foreach (var folder in folders)
            {
                var destination = Path.Combine(Folder.Path, Path.GetFileName(folder.Path));
                FileSystem.MoveDirectory(folder.Path, destination, UIOption.AllDialogs);
            }
            Folder.Scan();
        }
    }
}
