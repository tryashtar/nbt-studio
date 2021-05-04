using fNbt;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

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
            if (ListUtils.ExactlyOne(nodes)) // exactly one
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
                strings.Add(StringUtils.Pluralize(item.Value, singular, plural));
            }
            if (unknowns > 0)
                strings.Add(StringUtils.Pluralize(unknowns, "unknown node"));
            return String.Join(", ", strings);
        }

        // used like Filter(nodes, x=>x.GetNbtTag()) to get NBT for every node that has it
        public static IEnumerable<T> Filter<T>(this IEnumerable<INode> nodes, Func<INode, T> transformer)
        {
            return nodes.Select(transformer).Where(x => x != null);
        }

        public static T Get<T>(this INode node) where T : class
        {
            if (node is ChunkNode chunk)
                return chunk.Chunk as T;
            if (node is FolderNode folder)
                return folder.Folder as T;
            if (node is NbtFileNode file)
                return file.File as T;
            if (node is NbtTagNode tag)
                return tag.Tag as T;
            if (node is RegionFileNode region)
                return region.Region as T;
            return null;
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
    }

    public enum SearchDirection
    {
        Forward,
        Backward
    }

    public class TreeSearchReport
    {
        public int NodesSearched;
        public int TotalNodes;
        public decimal Percentage => (decimal)NodesSearched / TotalNodes;
    }

    public static class SearchNodeOperations
    {
        public static INode SearchFrom(NbtTreeModel model, INode start, Predicate<INode> predicate, SearchDirection direction, IProgress<TreeSearchReport> progress, CancellationToken token, bool wrap)
        {
            if (direction == SearchDirection.Forward)
            {
                var first = model.Root.Children.First();
                if (start == null)
                    start = first;
                else
                    start = NextNode(start);
                return SearchFromNext(model, start, predicate, NextNode, progress, token, new TreeSearchReport(), wrap ? first : null);
            }
            else
            {
                var last = FinalNode(model.Root);
                if (start == null)
                    start = last;
                else
                    start = PreviousNode(start);
                return SearchFromNext(model, start, predicate, PreviousNode, progress, token, new TreeSearchReport(), wrap ? last : null);
            }
        }

        public static IEnumerable<INode> SearchAll(NbtTreeModel model, Predicate<INode> predicate, IProgress<TreeSearchReport> progress, CancellationToken token)
        {
            var report = new TreeSearchReport();
            report.TotalNodes = model.Root.DescendantsCount;
            var node = model.Root.Children.First();
            while (node != null)
            {
                node = NextNode(node);
                if (node != null && predicate(node))
                    yield return node;
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = model.Root.DescendantsCount;
                    progress.Report(report);
                    token.ThrowIfCancellationRequested();
                }
            }
        }

        private static INode SearchFromNext(NbtTreeModel model, INode node, Predicate<INode> predicate, Func<INode, INode> next, IProgress<TreeSearchReport> progress, CancellationToken token, TreeSearchReport report, INode wrap_start)
        {
            var start = node;
            report.TotalNodes = model.Root.DescendantsCount;
            while (node != null && !predicate(node))
            {
                node = next(node);
                report.NodesSearched++;
                if (report.NodesSearched % 200 == 0)
                {
                    report.TotalNodes = model.Root.DescendantsCount;
                    progress.Report(report);
                    token.ThrowIfCancellationRequested();
                }
            }
            if (node != null && node != model.Root)
                return node;
            if (wrap_start == null)
                return null;

            // search again from new starting point, until reaching original starting point
            node = SearchFromNext(model, wrap_start, x => x == start || predicate(x), next, progress, token, report, null);
            if (node == start)
                return null;
            else
                return node;
        }

        public static INode NextNode(INode node)
        {
            var children = node.Children;
            if (children.Count > 0)
                return children.First();
            INode next = null;
            while (next == null && node != null)
            {
                if (node.Parent == null)
                    return null;
                next = Sibling(node, 1);
                if (next == null)
                    node = node.Parent;
            }
            return next;
        }

        public static INode PreviousNode(INode node)
        {
            var prev = Sibling(node, -1);
            if (prev == null)
                return node.Parent;
            while (prev != null)
            {
                var children = prev.Children;
                if (children.Count == 0)
                    return prev;
                prev = children[children.Count - 1];
            }
            return null;
        }

        private static INode Sibling(INode node, int add)
        {
            if (node.Parent == null)
                return null;
            var children = node.Parent.Children;
            int i;
            for (i = 0; i < children.Count; i++)
            {
                if (children[i] == node)
                    break;
            }
            i += add;
            if (i < 0 || i >= children.Count)
                return null;
            return children[i];
        }

        public static INode FinalNode(INode root)
        {
            var current = root;
            while (true)
            {
                var children = current.Children;
                if (children.Count == 0)
                    return current;
                current = current.Children.Last();
            }
        }
    }

    public static class GenericNodeOperations
    {
        public static IEnumerable<INode> RemoveAncestors(INode destination, IEnumerable<INode> nodes)
        {
            var ancestors = destination.Path.FullPath;
            foreach (var item in nodes)
            {
                if (!ancestors.Contains(item))
                    yield return item;
            }
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
            {
                var name_sorter = new NbtUtil.TagNameSorter();
                var tag_sorter = new NbtUtil.TagTypeSorter();
                var current_sort = FindCurrentSort(compound, name_sorter, tag_sorter);
                if (current_sort == tag_sorter || current_sort == null)
                    compound.Sort(name_sorter, true);
                else
                    compound.Sort(tag_sorter, true);
            }
        }

        private static IComparer<NbtTag> FindCurrentSort(NbtCompound compound, params IComparer<NbtTag>[] candidates)
        {
            var potential = new List<IComparer<NbtTag>>(candidates.Reverse());
            var scanning = compound.GetAllTags().OfType<NbtCompound>().Prepend(compound);
            foreach (var scan in scanning)
            {
                for (int i = potential.Count - 1; i >= 0; i--)
                {
                    if (!IsSortedBy(scan.Tags, potential[i]))
                        potential.RemoveAt(i);
                }
                if (potential.Count == 0)
                    return null;
                if (potential.Count == 1)
                    return potential.Single();
            }
            return null;
        }

        private static bool IsSortedBy<T>(IEnumerable<T> sequence, IComparer<T> sorter)
        {
            T prev = default;
            bool first = true;
            foreach (var item in sequence)
            {
                if (!first && sorter.Compare(prev, item) > 0)
                    return false;
                prev = item;
                first = false;
            }
            return true;
        }

        public static IEnumerable<NbtTag> ParseTags(IDataObject data)
        {
            var text = (string)data.GetData(typeof(string));
            if (text == null)
                yield break;
            var snbts = text.Split('\n');
            foreach (var nbt in snbts)
            {
                if (SnbtParser.ClassicTryParse(nbt, true, out NbtTag tag) || SnbtParser.ClassicTryParse(nbt, false, out tag))
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
