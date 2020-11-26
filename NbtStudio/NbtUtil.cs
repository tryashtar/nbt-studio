using Aga.Controls.Tree;
using fNbt;
using NbtStudio.SNBT;
using NbtStudio.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public static class NbtUtil
    {
        // everything except End and Unknown
        public static IEnumerable<NbtTagType> NormalTagTypes()
        {
            yield return NbtTagType.Byte;
            yield return NbtTagType.Short;
            yield return NbtTagType.Int;
            yield return NbtTagType.Long;
            yield return NbtTagType.Float;
            yield return NbtTagType.Double;
            yield return NbtTagType.String;
            yield return NbtTagType.ByteArray;
            yield return NbtTagType.IntArray;
            yield return NbtTagType.LongArray;
            yield return NbtTagType.Compound;
            yield return NbtTagType.List;
        }

        // tags with simple primitive values
        // excludes lists, compounds, and arrays
        public static bool IsValueType(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                case NbtTagType.Short:
                case NbtTagType.Int:
                case NbtTagType.Long:
                case NbtTagType.Float:
                case NbtTagType.Double:
                case NbtTagType.String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsArrayType(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.ByteArray:
                case NbtTagType.IntArray:
                case NbtTagType.LongArray:
                    return true;
                default:
                    return false;
            }
        }

        public static object GetValue(NbtTag tag)
        {
            if (tag is NbtByte tag_byte)
                return tag_byte.Value;
            else if (tag is NbtShort tag_short)
                return tag_short.Value;
            else if (tag is NbtInt tag_int)
                return tag_int.Value;
            else if (tag is NbtLong tag_long)
                return tag_long.Value;
            else if (tag is NbtFloat tag_float)
                return tag_float.Value;
            else if (tag is NbtDouble tag_double)
                return tag_double.Value;
            else if (tag is NbtString tag_string)
                return tag_string.Value;
            else if (tag is NbtByteArray tag_ba)
                return tag_ba.Value;
            else if (tag is NbtIntArray tag_ia)
                return tag_ia.Value;
            else if (tag is NbtLongArray tag_la)
                return tag_la.Value;
            else if (tag is NbtCompound tag_compound)
                return tag_compound.Tags;
            else if (tag is NbtList tag_list)
                return tag_list;
            throw new ArgumentException($"Can't get value from {tag.TagType}");
        }

        public static void ResetValue(NbtTag tag)
        {
            if (tag is NbtByte tag_byte)
                tag_byte.Value = 0;
            else if (tag is NbtShort tag_short)
                tag_short.Value = 0;
            else if (tag is NbtInt tag_int)
                tag_int.Value = 0;
            else if (tag is NbtLong tag_long)
                tag_long.Value = 0;
            else if (tag is NbtFloat tag_float)
                tag_float.Value = 0;
            else if (tag is NbtDouble tag_double)
                tag_double.Value = 0;
            else if (tag is NbtString tag_string)
                tag_string.Value = String.Empty;
            else if (tag is NbtByteArray tag_ba)
                tag_ba.Value = new byte[0];
            else if (tag is NbtIntArray tag_ia)
                tag_ia.Value = new int[0];
            else if (tag is NbtLongArray tag_la)
                tag_la.Value = new long[0];
            else if (tag is NbtCompound tag_compound)
                tag_compound.Clear();
            else if (tag is NbtList tag_list)
                tag_list.Clear();
        }

        public static void SetValue(NbtTag tag, object value)
        {
            if (tag is NbtByte tag_byte && value is byte b)
                tag_byte.Value = b;
            else if (tag is NbtShort tag_short && value is short s)
                tag_short.Value = s;
            else if (tag is NbtInt tag_int && value is int i)
                tag_int.Value = i;
            else if (tag is NbtLong tag_long && value is long l)
                tag_long.Value = l;
            else if (tag is NbtFloat tag_float && value is float f)
                tag_float.Value = f;
            else if (tag is NbtDouble tag_double && value is double d)
                tag_double.Value = d;
            else if (tag is NbtString tag_string && value is string str)
                tag_string.Value = str;
            else if (tag is NbtByteArray tag_ba && value is byte[] bytes)
                tag_ba.Value = bytes;
            else if (tag is NbtIntArray tag_ia && value is int[] ints)
                tag_ia.Value = ints;
            else if (tag is NbtLongArray tag_la && value is long[] longs)
                tag_la.Value = longs;
            else if (tag is NbtCompound tag_compound && value is IEnumerable<NbtTag> c_tags)
            {
                var tags = c_tags.ToList();
                tag_compound.Clear();
                foreach (var child in tags)
                {
                    child.AddTo(tag_compound);
                }
            }
            else if (tag is NbtList tag_list && value is IEnumerable<NbtTag> l_tags)
            {
                var tags = l_tags.ToList();
                tag_list.Clear();
                foreach (var child in tags)
                {
                    child.AddTo(tag_list);
                }
            }
        }

        public static object ParseValue(string value, NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return (byte)Util.ParseByte(value);
                case NbtTagType.Short:
                    return short.Parse(value);
                case NbtTagType.Int:
                    return int.Parse(value);
                case NbtTagType.Long:
                    return long.Parse(value);
                case NbtTagType.Float:
                    return Util.ParseFloat(value);
                case NbtTagType.Double:
                    return Util.ParseDouble(value);
                case NbtTagType.String:
                    return value;
                default:
                    return null;
            }
        }

        public static NbtTag CreateTag(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return new NbtByte();
                case NbtTagType.Short:
                    return new NbtShort();
                case NbtTagType.Int:
                    return new NbtInt();
                case NbtTagType.Long:
                    return new NbtLong();
                case NbtTagType.Float:
                    return new NbtFloat();
                case NbtTagType.Double:
                    return new NbtDouble();
                case NbtTagType.String:
                    return new NbtString();
                case NbtTagType.ByteArray:
                    return new NbtByteArray();
                case NbtTagType.IntArray:
                    return new NbtIntArray();
                case NbtTagType.LongArray:
                    return new NbtLongArray();
                case NbtTagType.Compound:
                    return new NbtCompound();
                case NbtTagType.List:
                    return new NbtList();
                default:
                    throw new ArgumentException($"Can't create a tag from {type}");
            }
        }

        public static (string min, string max) MinMaxFor(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return (sbyte.MinValue.ToString(), sbyte.MaxValue.ToString());
                case NbtTagType.Short:
                    return (short.MinValue.ToString(), short.MaxValue.ToString());
                case NbtTagType.Int:
                    return (int.MinValue.ToString(), int.MaxValue.ToString());
                case NbtTagType.Long:
                    return (long.MinValue.ToString(), long.MaxValue.ToString());
                case NbtTagType.Float:
                    return (float.MinValue.ToString(), float.MaxValue.ToString());
                case NbtTagType.Double:
                    return (double.MinValue.ToString(), double.MaxValue.ToString());
                default:
                    throw new ArgumentException($"{type} isn't numeric, has no min and max");
            }
        }

        public static string PreviewNbtValue(NbtTag tag)
        {
            if (tag is NbtCompound compound)
                return $"[{Util.Pluralize(compound.Count, "entry", "entries")}]";
            else if (tag is NbtList list)
            {
                if (list.Count == 0)
                    return $"[0 entries]";
                return $"[{Util.Pluralize(list.Count, TagTypeName(list.ListType).ToLower())}]";
            }
            else if (tag is NbtByteArray byte_array)
                return $"[{Util.Pluralize(byte_array.Value.Length, "byte")}]";
            else if (tag is NbtIntArray int_array)
                return $"[{Util.Pluralize(int_array.Value.Length, "int")}]";
            else if (tag is NbtLongArray long_array)
                return $"[{Util.Pluralize(long_array.Value.Length, "long")}]";
            return tag.ToSnbt(SnbtOptions.Preview);
        }

        public static string TagTypeName(NbtTagType type)
        {
            if (type == NbtTagType.ByteArray)
                return "Byte Array";
            if (type == NbtTagType.IntArray)
                return "Int Array";
            if (type == NbtTagType.LongArray)
                return "Long Array";
            return type.ToString();
        }

        public class TagNameSorter : IComparer<NbtTag>
        {
            public int Compare(NbtTag x, NbtTag y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        public class TagTypeSorter : IComparer<NbtTag>
        {
            private static readonly Dictionary<NbtTagType, int> TypeOrder = new Dictionary<NbtTagType, int>
            {
                {NbtTagType.Compound, 1},
                {NbtTagType.List, 2},
                {NbtTagType.Byte, 3},
                {NbtTagType.Short, 4},
                {NbtTagType.Int, 5},
                {NbtTagType.Long, 6},
                {NbtTagType.Float, 7},
                {NbtTagType.Double, 8},
                {NbtTagType.String, 9},
                {NbtTagType.ByteArray, 9},
                {NbtTagType.IntArray, 10},
                {NbtTagType.LongArray, 11},
            };
            public int Compare(NbtTag x, NbtTag y)
            {
                int compare = TypePriority(x.TagType).CompareTo(TypePriority(y.TagType));
                if (compare != 0)
                    return compare;
                return x.Name.CompareTo(y.Name);
            }
            private int TypePriority(NbtTagType type)
            {
                if (TypeOrder.TryGetValue(type, out int result))
                    return result;
                return int.MaxValue;
            }
        }

        public static void Sort(NbtCompound compound, IComparer<NbtTag> sorter, bool recursive)
        {
            var tags = compound.Tags.OrderBy(x => x, sorter).ToList();
            compound.Clear();
            foreach (var tag in tags)
            {
                if (recursive)
                {
                    if (tag is NbtCompound sub)
                        Sort(sub, sorter, true);
                    else if (tag is NbtList list)
                        SortChildren(list, sorter);
                }
                tag.AddTo(compound);
            }
        }

        private static void SortChildren(NbtList list, IComparer<NbtTag> sorter)
        {
            if (list.ListType == NbtTagType.Compound)
            {
                foreach (NbtCompound item in list)
                {
                    Sort(item, sorter, true);
                }
            }
            else if (list.ListType == NbtTagType.List)
            {
                foreach (NbtList item in list)
                {
                    SortChildren(item, sorter);
                }
            }
        }

        public static void TransformAdd(NbtTag tag, NbtContainerTag destination) => TransformAdd(new[] { tag }, destination);
        public static void TransformAdd(IEnumerable<NbtTag> tags, NbtContainerTag destination) => TransformInsert(tags, destination, destination.Count);
        public static void TransformInsert(NbtTag tag, NbtContainerTag destination, int index) => TransformInsert(new[] { tag }, destination, index);
        public static void TransformInsert(IEnumerable<NbtTag> tags, NbtContainerTag destination, int index)
        {
            var adding = tags.ToList();
            int original_index = index;
            foreach (var tag in tags)
            {
                if (!destination.CanAdd(tag.TagType))
                {
                    adding.Remove(tag);
                    continue;
                }
                if (tag.IsInside(destination) && original_index > tag.GetIndex())
                    index--;
            }
            foreach (var tag in adding)
            {
                tag.Remove();
            }
            foreach (var tag in adding)
            {
                tag.Name = GetAutomaticName(tag, destination);
                tag.InsertInto(destination, index);
                index++;
            }
        }

        public static string GetAutomaticName(NbtTag tag, NbtContainerTag parent)
        {
            if (parent is NbtList)
                return null;
            if (parent is NbtCompound compound)
            {
                if (tag.Name != null && !compound.Contains(tag.Name))
                    return tag.Name;
                string basename = tag.Name ?? TagTypeName(tag.TagType).ToLower().Replace(' ', '_');
                for (int i = 1; i < 999999; i++)
                {
                    string name = basename + i.ToString();
                    if (!compound.Contains(name))
                        return name;
                }
                throw new InvalidOperationException("This compound really contains 999999 similarly named tags?!");
            }
            throw new ArgumentException($"Can't get automatic name for tags inside a {parent.TagType}");
        }


        public static bool CanAddAll(IEnumerable<NbtTag> tags, NbtContainerTag destination)
        {
            // check if you're trying to add items of different types to a list
            if (destination is NbtList list && tags.Select(x => x.TagType).Distinct().Skip(1).Any())
                return false;
            // check if you're trying to add an item to its own descendent
            var ancestors = Ancestors(destination);
            if (tags.Intersect(ancestors).Any())
                return false;
            return tags.All(x => destination.CanAdd(x.TagType));
        }

        public static List<NbtTag> Ancestors(NbtTag tag)
        {
            var ancestors = new List<NbtTag>();
            while (tag != null)
            {
                ancestors.Add(tag);
                tag = tag.Parent;
            }
            return ancestors;
        }

        public static string TagDescription(this NbtTag tag)
        {
            string type = NbtUtil.TagTypeName(tag.TagType).ToLower();
            if (!String.IsNullOrEmpty(tag.Name))
                return $"'{tag.Name}' {type}";
            int index = tag.GetIndex();
            if (index != -1)
            {
                if (!String.IsNullOrEmpty(tag.Parent?.Name))
                    return $"{type} at index {index} in '{tag.Parent.Name}'";
                else if (tag.Parent?.TagType != null)
                    return $"{type} at index {index} in a {NbtUtil.TagTypeName(tag.Parent.TagType).ToLower()}";
            }
            return type;
        }
        public static string TagDescription(IEnumerable<NbtTag> tags)
        {
            if (!tags.Any()) // none
                return "0 tags";
            if (Util.ExactlyOne(tags)) // exactly one
                return TagDescription(tags.Single()); // more than one
            return Util.Pluralize(tags.Count(), "tag");
        }

        public static string ChunkDescription(Chunk chunk)
        {
            if (chunk.Region == null)
                return $"chunk at ({chunk.X}, {chunk.Z})";
            return $"chunk at ({chunk.X}, {chunk.Z}) in '{Path.GetFileName(chunk.Region.Path)}'";
        }

        private class FileExtension
        {
            public readonly string Extension;
            public readonly string Description;
            public readonly bool IsRegion;
            public readonly bool IsBinary;

            public FileExtension(string extension, string description, bool binary = true, bool region = false)
            {
                Extension = extension;
                Description = description;
                IsBinary = binary;
                IsRegion = region;
            }
        }
        private static readonly List<FileExtension> NbtExtensions = new List<FileExtension>
        {
            { new FileExtension("nbt", "NBT files") },
            { new FileExtension("snbt", "SNBT files", binary: false) },
            { new FileExtension("dat", "DAT files") },
            { new FileExtension("mca", "Anvil Region files", region: true) },
            { new FileExtension("mcr", "Legacy Region files", region: true) },
            { new FileExtension("mcc", "External Chunk files") },
            { new FileExtension("mcstructure", "Bedrock Structure files") },
            { new FileExtension("json", "JSON files", binary: false) },
            { new FileExtension("schematic", "MCEdit Schematic files") }
        };
        private static string GetEntry(FileExtension f) => $"{f.Description}|*.{f.Extension}";
        private static string AllFiles => "All Files|*";
        private static string AllNbtFiles(IEnumerable<FileExtension> source) => $"All NBT Files|{String.Join("; ", source.Select(x => "*." + x.Extension))}";
        private static string IndividualNbtFiles(IEnumerable<FileExtension> source) => String.Join("|", source.Select(GetEntry));
        private static string IndividualNbtFiles<T>(IEnumerable<FileExtension> source, Func<FileExtension, T> order) => String.Join("|", source.OrderBy(order).Select(GetEntry));
        public static string SaveFilter(string first_extension)
        {
            var relevant = NbtExtensions.Where(x => !x.IsRegion);
            if (first_extension == null)
                return $"{IndividualNbtFiles(relevant)}|{AllNbtFiles(relevant)}|{AllFiles}";
            var first = NbtExtensions.FirstOrDefault(x => "." + x.Extension == first_extension);
            if (first == null)
                return $"{AllFiles}|{IndividualNbtFiles(relevant)}|{AllNbtFiles(relevant)}";
            string individual = IndividualNbtFiles(relevant, x => x != first); // OrderBy puts false before true
            return $"{individual}|{AllNbtFiles(relevant)}|{AllFiles}";
        }
        public static string OpenFilter()
        {
            return $"{AllFiles}|{AllNbtFiles(NbtExtensions)}|{IndividualNbtFiles(NbtExtensions)}";
        }
        public static bool? BinaryExtension(string extension) => NbtExtensions.FirstOrDefault(x => "." + x.Extension == extension)?.IsBinary;

        public static IconType TagIconType(NbtTagType type)
        {
            if (type == NbtTagType.Byte)
                return IconType.ByteTag;
            if (type == NbtTagType.Short)
                return IconType.ShortTag;
            if (type == NbtTagType.Int)
                return IconType.IntTag;
            if (type == NbtTagType.Long)
                return IconType.LongTag;
            if (type == NbtTagType.Float)
                return IconType.FloatTag;
            if (type == NbtTagType.Double)
                return IconType.DoubleTag;
            if (type == NbtTagType.String)
                return IconType.StringTag;
            if (type == NbtTagType.ByteArray)
                return IconType.ByteArrayTag;
            if (type == NbtTagType.IntArray)
                return IconType.IntArrayTag;
            if (type == NbtTagType.LongArray)
                return IconType.LongArrayTag;
            if (type == NbtTagType.Compound)
                return IconType.CompoundTag;
            if (type == NbtTagType.List)
                return IconType.ListTag;
            throw new ArgumentException($"No icon for {type}");
        }

        public static ImageIcon TagTypeImage(IconSource source, NbtTagType type)
        {
            return source.GetImage(TagIconType(type));
        }

        public static void AddTo(this NbtTag tag, NbtContainerTag container)
        {
            Remove(tag);
            container.Add(tag);
        }

        public static void InsertInto(this NbtTag tag, NbtContainerTag container, int index)
        {
            Remove(tag);
            container.Insert(index, tag);
        }

        public static int GetIndex(this NbtTag tag)
        {
            if (tag.Parent == null)
                return -1;
            return tag.Parent.IndexOf(tag);
        }

        public static bool IsInside(this NbtTag tag, NbtContainerTag container)
        {
            return container.Contains(tag);
        }

        public static void Remove(this NbtTag tag)
        {
            if (tag.Parent != null)
                tag.Parent.Remove(tag);
        }
    }
}
