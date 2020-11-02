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

    public static class NodeExtractions
    {
        public static INbtTag GetNbtTag(this INode node)
        {
            if (node is NbtTagNode nbt)
                return nbt.Tag;
            if (node is NbtFileNode file)
                return file.File.RootTag;
            if (node is ChunkNode chunk)
                return chunk.Data;
            return null;
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
        protected void PerformAction(UndoableAction action)
        {
            Tree.PerformAction(action);
        }

        public static NotifyNode Create(NbtTreeModel tree, object obj)
        {
            if (obj is NbtTag tag)
                return new NbtTagNode(tree, tag);
            if (obj is NbtFile file)
                return new NbtFileNode(tree, file);
            if (obj is RegionFile region)
                return new RegionFileNode(tree, region);
            if (obj is Chunk chunk)
                return new ChunkNode(tree, chunk);
            if (obj is NbtFolder folder)
                return new FolderNode(tree, folder);
            throw new ArgumentException($"Can't create node from {obj.GetType()}");
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
        public readonly NotifyNbtTag Tag;
        public NbtTagNode(NbtTreeModel tree, NbtTag tag) : base(tree, tag)
        {
            Tag = NotifyNbtTag.CreateFrom(tag);
            Tag.Changed += Tag_Changed;
            Tag.ActionPrepared += Tag_ActionPrepared;
        }

        private void Tag_ActionPrepared(object sender, UndoableAction action)
        {
            PerformAction(action);
        }

        private void Tag_Changed(object sender, EventArgs e)
        {
            Notify((NotifyNbtTag)sender);
        }
    }
}
