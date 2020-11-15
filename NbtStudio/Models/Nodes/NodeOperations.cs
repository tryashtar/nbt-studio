using fNbt;
using Microsoft.VisualBasic.FileIO;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public static class ExtractNodeOperations
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

        // used like Filter(nodes, x=>x.GetNbtTag()) to get NBT for every node that has it
        public static IEnumerable<T> Filter<T>(this IEnumerable<INode> nodes, Func<INode, T> transformer)
        {
            return nodes.Select(transformer).Where(x => x != null);
        }

        // technically these could go in INode but I'd prefer to keep it clean of project-specific stuff
        // I could make like an INbtStudioNode but I don't want to type that a lot
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

    // shared implementations of operations like copy, paste, drag and drop for NBT tags
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

    // shared implementations of operations like copy, paste, drag and drop for file-like things
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

        // default cut is copy then delete
        // but for files we wait until pasting to delete
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
}
