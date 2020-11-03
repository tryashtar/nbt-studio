using fNbt;
using Microsoft.VisualBasic.FileIO;
using NbtStudio;
using NbtStudio.SNBT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        string Copy();
        bool CanPaste { get; }
        IEnumerable<INode> Paste(string data);
        bool CanRename { get; }
        bool CanEdit { get; }
        bool CanReceiveDrop(IEnumerable<INode> nodes);
        void ReceiveDrop(IEnumerable<INode> nodes, int index);
    }

    public static class NodeExtractions
    {
        public static Dictionary<Type, Tuple<string, string>> NodeTypes = new Dictionary<Type, Tuple<string, string>>
        {
            { typeof(NbtTagNode), Tuple.Create("tag", "tags") },
            { typeof(NbtFileNode), Tuple.Create("file", "files") },
            { typeof(ChunkNode), Tuple.Create("chunk", "chunks") },
            { typeof(RegionFileNode), Tuple.Create("region file", "region files") },
            { typeof(FolderNode), Tuple.Create("folder", "folders") }
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
                var desc = NodeTypes[item.Key];
                strings.Add(Util.Pluralize(item.Value, desc.Item1, desc.Item2));
            }
            if (unknowns > 0)
                strings.Add(Util.Pluralize(unknowns, "unknown node"));
            return String.Join(", ", strings);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<INode> nodes, Func<INode, T> transformer)
        {
            return nodes.Select(transformer).Where(x => x != null);
        }

        public static INbtTag GetNbtTag(this INode node)
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

    public abstract class NotifyNode : INode
    {
        protected readonly NbtTreeModel Tree;
        protected readonly object SourceObject;
        public object Object => SourceObject;
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
        protected void RemoveSelf()
        {
            Tree.Remove(SourceObject);
        }
        protected void PerformAction(UndoableAction action)
        {
            Tree.PerformAction(action);
        }

        private static readonly Dictionary<object, NotifyNode> ObjectCache = new Dictionary<object, NotifyNode>();
        public static NotifyNode Create(NbtTreeModel tree, object obj)
        {
            if (ObjectCache.TryGetValue(obj, out var cached))
                return cached;
            NotifyNode result = null;
            if (obj is INbtTag tag)
                result = new NbtTagNode(tree, tag.Unwrap());
            else if (obj is NbtFile file)
                result = new NbtFileNode(tree, file);
            else if (obj is RegionFile region)
                result = new RegionFileNode(tree, region);
            else if (obj is Chunk chunk)
                result = new ChunkNode(tree, chunk);
            else if (obj is NbtFolder folder)
                result = new FolderNode(tree, folder);
            if (result != null)
            {
                ObjectCache[obj] = result;
                return result;
            }
            else
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
        public virtual string Copy() => null;
        public virtual bool CanPaste => false;
        public virtual IEnumerable<INode> Paste(string data) => Enumerable.Empty<INode>();
        public virtual bool CanRename => false;
        public virtual bool CanEdit => false;
        public virtual bool CanReceiveDrop(IEnumerable<INode> nodes) => false;
        public virtual void ReceiveDrop(IEnumerable<INode> nodes, int index) { }
    }

    public static class NbtNodeOperations
    {
        public static string Copy(INbtTag tag)
        {
            return tag.ToSnbt(include_name: true);
        }

        public static bool CanEdit(INbtTag tag)
        {
            return true;
        }

        public static bool CanPaste(INbtTag tag)
        {
            return tag is INbtContainer;
        }

        public static bool CanRename(INbtTag tag)
        {
            return !(tag.Parent is INbtList);
        }

        public static bool CanSort(INbtTag tag)
        {
            return tag is INbtCompound;
        }

        public static void Sort(INbtTag tag)
        {
            if (tag is INbtCompound compound)
                NbtUtil.Sort(compound, new NbtUtil.TagTypeSorter(), true);
        }

        public static IEnumerable<NbtTag> ParseTags(string text)
        {
            var snbts = text.Split('\n');
            foreach (var nbt in snbts)
            {
                if (SnbtParser.TryParse(nbt, true, out NbtTag tag) || SnbtParser.TryParse(nbt, false, out tag))
                    yield return tag;
            }
        }

        public static IEnumerable<INbtTag> Paste(INbtTag tag, string text)
        {
            if (tag is INbtContainer container)
            {
                var tags = ParseTags(text).ToList();
                NbtUtil.TransformAdd(tags, container);
                return tags;
            }
            return Enumerable.Empty<INbtTag>();
        }

        public static bool CanReceiveDrop(INbtTag tag, IEnumerable<INbtTag> tags)
        {
            if (!(tag is INbtContainer container))
                return false;
            return NbtUtil.CanAddAll(tags, container);
        }

        public static void ReceiveDrop(INbtTag tag, IEnumerable<INbtTag> tags, int index)
        {
            if (tag is INbtContainer container)
                NbtUtil.TransformInsert(tags, container, index);
        }
    }

    public static class FileNodeOperations
    {
        public static bool DeleteFile(IHavePath item)
        {
            if (item.Path == null)
                return true;
            try
            {
                FileSystem.DeleteFile(item.Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch { return false; }
            return true;
        }

        public static bool DeleteFolder(IHavePath item)
        {
            if (item.Path == null)
                return true;
            try
            {
                FileSystem.DeleteDirectory(item.Path, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);
            }
            catch { return false; }
            return true;
        }
    }

    public class NbtTagNode : NotifyNode
    {
        public NotifyNbtTag Tag => (NotifyNbtTag)SourceObject;
        public NbtTagNode(NbtTreeModel tree, NbtTag tag) : base(tree, NotifyNbtTag.CreateFrom(tag))
        {
            Tag.Changed += Tag_Changed;
            Tag.ActionPrepared += Tag_ActionPrepared;
        }

        private void Tag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }

        private void Tag_ActionPrepared(object sender, UndoableAction action)
        {
            if (sender == Tag)
                PerformAction(action);
        }

        public override string Description => Tag.TagDescription();

        public override bool CanCopy => true;
        public override string Copy() => NbtNodeOperations.Copy(Tag);
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
        public override IEnumerable<INode> Paste(string data)
        {
            var tags = NbtNodeOperations.Paste(Tag, data);
            return tags.Select(x => Create(Tree, x));
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
        {
            File.RootTag.Changed += RootTag_Changed;
            File.RootTag.ActionPrepared += RootTag_ActionPrepared;
        }

        private void RootTag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }

        private void RootTag_ActionPrepared(object sender, UndoableAction action)
        {
            if (sender == File.RootTag)
                PerformAction(action);
        }

        public override string Description => Path.GetFileName(File.Path);

        public override bool CanCopy => true;
        public override string Copy() => NbtNodeOperations.Copy(File.RootTag);
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (FileNodeOperations.DeleteFile(File))
                base.Delete();
        }
        public override bool CanEdit => false;
        public override bool CanPaste => NbtNodeOperations.CanPaste(File.RootTag);
        public override bool CanRename => false;
        public override bool CanSort => NbtNodeOperations.CanSort(File.RootTag);
        public override void Sort() => NbtNodeOperations.Sort(File.RootTag);
        public override IEnumerable<INode> Paste(string data)
        {
            var tags = NbtNodeOperations.Paste(File.RootTag, data);
            return tags.Select(x => Create(Tree, x));
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
            else
                Chunk.OnLoaded += (s, e) => SetupEvents();
        }

        private void SetupEvents()
        {
            if (!HasSetupEvents)
            {
                Chunk.Data.Changed += RootTag_Changed;
                Chunk.Data.ActionPrepared += RootTag_ActionPrepared;
                HasSetupEvents = true;
            }
        }

        public NotifyNbtCompound AccessChunkData()
        {
            if (!Chunk.IsLoaded)
                Chunk.Load();
            return Chunk.Data;
        }

        private void RootTag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }

        private void RootTag_ActionPrepared(object sender, UndoableAction action)
        {
            if (sender == Chunk.Data)
                PerformAction(action);
        }

        public override string Description => NbtUtil.ChunkDescription(Chunk);

        public override bool CanCopy => true;
        public override string Copy() => NbtNodeOperations.Copy(AccessChunkData());
        public override bool CanDelete => true;
        public override void Delete()
        {
            var region = Chunk.Region;
            if (region != null)
                PerformAction(new UndoableAction($"Remove {NbtUtil.ChunkDescription(Chunk)}",
                    () => { region.RemoveChunk(Chunk.X, Chunk.Z); base.Delete(); },
                    () => { region.AddChunk(Chunk); }
                ));
        }
        public override bool CanEdit => true;
        public override bool CanPaste => true;
        public override bool CanRename => true;
        public override bool CanSort => true;
        public override void Sort() => NbtNodeOperations.Sort(AccessChunkData());
        public override IEnumerable<INode> Paste(string data)
        {
            var tags = NbtNodeOperations.Paste(AccessChunkData(), data);
            return tags.Select(x => Create(Tree, x));
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
        public readonly RegionFile Region;
        public RegionFileNode(NbtTreeModel tree, RegionFile region) : base(tree, region)
        {
            Region = region;
            Region.ChunksChanged += Region_ChunksChanged;
        }

        private void Region_ChunksChanged(object sender, EventArgs e)
        {
            Notify(sender);
        }

        public override string Description => Path.GetFileName(Region.Path);

        public override bool CanCopy => false;
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (Region.Path != null)
                Region.Dispose();
            if (FileNodeOperations.DeleteFile(Region))
                base.Delete();
        }
        public override bool CanEdit => false;
        public override bool CanPaste => true;
        public override bool CanRename => false;
        public override bool CanSort => false;
        public override IEnumerable<INode> Paste(string data)
        {
            var tags = NbtNodeOperations.ParseTags(data).OfType<NbtCompound>().ToList();
            var available = NbtUtil.GetAvailableCoords(Region);
            var chunks = Enumerable.Zip(available, tags, (slot, tag) => Chunk.EmptyChunk(tag, slot.Item1, slot.Item2));
            foreach (var chunk in chunks)
            {
                Region.AddChunk(chunk);
            }
            return chunks.Select(x => Create(Tree, x));
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
        public readonly NbtFolder Folder;
        public FolderNode(NbtTreeModel tree, NbtFolder folder) : base(tree, folder)
        {
            Folder = folder;
        }

        public override string Description => Path.GetFileName(Folder.Path);

        public override bool CanCopy => true;
        public override bool CanDelete => true;
        public override void Delete()
        {
            if (FileNodeOperations.DeleteFolder(Folder))
                base.Delete();
        }
        public override bool CanEdit => false;
        public override bool CanPaste => true;
        public override bool CanRename => false;
        public override bool CanSort => false;
    }
}
