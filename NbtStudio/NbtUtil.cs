using Aga.Controls.Tree;
using fNbt;
using NbtStudio.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

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
            return type switch
            {
                NbtTagType.Byte or
                NbtTagType.Short or
                NbtTagType.Int or
                NbtTagType.Long or
                NbtTagType.Float or
                NbtTagType.Double or
                NbtTagType.String => true,
                _ => false,
            };
        }

        public static bool IsNumericType(NbtTagType type)
        {
            return type switch
            {
                NbtTagType.Byte or
                NbtTagType.Short or
                NbtTagType.Int or
                NbtTagType.Long or
                NbtTagType.Float or
                NbtTagType.Double => true,
                _ => false,
            };
        }

        public static bool IsArrayType(NbtTagType type)
        {
            return type switch
            {
                NbtTagType.ByteArray or
                NbtTagType.IntArray or
                NbtTagType.LongArray => true,
                _ => false,
            };
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
                return tag_list.Tags;
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
                tag_ba.Value = Array.Empty<byte>();
            else if (tag is NbtIntArray tag_ia)
                tag_ia.Value = Array.Empty<int>();
            else if (tag is NbtLongArray tag_la)
                tag_la.Value = Array.Empty<long>();
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
            else
                throw new ArgumentException($"Cant assign {value} ({value.GetType()}) to a {tag.TagType}");
        }

        public static object ParseValue(string value, NbtTagType type)
        {
            return type switch
            {
                NbtTagType.Byte => (byte)SnbtParser.ParseByte(value),
                NbtTagType.Short => short.Parse(value),
                NbtTagType.Int => int.Parse(value),
                NbtTagType.Long => long.Parse(value),
                NbtTagType.Float => DataUtils.ParseFloat(value),
                NbtTagType.Double => DataUtils.ParseDouble(value),
                NbtTagType.String => value,
                _ => null,
            };
        }

        public static NbtTag CreateTag(NbtTagType type)
        {
            return type switch
            {
                NbtTagType.Byte => new NbtByte(),
                NbtTagType.Short => new NbtShort(),
                NbtTagType.Int => new NbtInt(),
                NbtTagType.Long => new NbtLong(),
                NbtTagType.Float => new NbtFloat(),
                NbtTagType.Double => new NbtDouble(),
                NbtTagType.String => new NbtString(),
                NbtTagType.ByteArray => new NbtByteArray(),
                NbtTagType.IntArray => new NbtIntArray(),
                NbtTagType.LongArray => new NbtLongArray(),
                NbtTagType.Compound => new NbtCompound(),
                NbtTagType.List => new NbtList(),
                _ => throw new ArgumentException($"Can't create a tag from {type}"),
            };
        }

        public static (string min, string max) MinMaxFor(NbtTagType type)
        {
            return type switch
            {
                NbtTagType.Byte => (sbyte.MinValue.ToString(), sbyte.MaxValue.ToString()),
                NbtTagType.Short => (short.MinValue.ToString(), short.MaxValue.ToString()),
                NbtTagType.Int => (int.MinValue.ToString(), int.MaxValue.ToString()),
                NbtTagType.Long => (long.MinValue.ToString(), long.MaxValue.ToString()),
                NbtTagType.Float => (float.MinValue.ToString(), float.MaxValue.ToString()),
                NbtTagType.Double => (double.MinValue.ToString(), double.MaxValue.ToString()),
                _ => throw new ArgumentException($"{type} isn't numeric, has no min and max"),
            };
        }

        public static string PreviewNbtValue(NbtTag tag)
        {
            if (tag is NbtCompound compound)
                return $"[{StringUtils.Pluralize(compound.Count, "entry", "entries")}]";
            else if (tag is NbtList list)
            {
                if (list.Count == 0)
                    return $"[0 entries]";
                return $"[{StringUtils.Pluralize(list.Count, TagTypeName(list.ListType).ToLower())}]";
            }
            else if (tag is NbtByteArray byte_array)
            {
                if (byte_array.Value.Length is < 20 and > 0)
                    return $"[{String.Join(", ", byte_array.Value)}]";
                return $"[{StringUtils.Pluralize(byte_array.Value.Length, "byte")}]";
            }
            else if (tag is NbtIntArray int_array)
            {
                if (int_array.Value.Length is < 20 and > 0)
                    return $"[{String.Join(", ", int_array.Value)}]";
                return $"[{StringUtils.Pluralize(int_array.Value.Length, "int")}]";
            }
            else if (tag is NbtLongArray long_array)
            {
                if (long_array.Value.Length is < 20 and > 0)
                    return $"[{String.Join(", ", long_array.Value)}]";
                return $"[{StringUtils.Pluralize(long_array.Value.Length, "long")}]";
            }
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

        public class ExistingCompoundSorter : IComparer<NbtTag>
        {
            private readonly NbtCompound Source;
            public ExistingCompoundSorter(NbtCompound source)
            {
                Source = source;
            }

            public int Compare(NbtTag x, NbtTag y)
            {
                return Source.IndexOf(x.Name).CompareTo(Source.IndexOf(y.Name));
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

        public static void SetEqualTo(this NbtTag destination, NbtTag source)
        {
            if (destination.TagType != source.TagType)
                throw new InvalidOperationException($"Tag types must match: {destination.TagType} != {source.TagType}");
            if (source.TagType == NbtTagType.Compound)
                ((NbtCompound)destination).SetCompoundEqualTo((NbtCompound)source);
            else if (source.TagType == NbtTagType.List)
                ((NbtList)destination).SetListEqualTo((NbtList)source);
            else
                SetValue(destination, GetValue(source));
        }

        private static bool PsuedoContains(NbtCompound compound, NbtTag tag)
        {
            if (compound.TryGet(tag.Name, out var existing))
                return existing.TagType == tag.TagType;
            return false;
        }

        public static void SetCompoundEqualTo(this NbtCompound destination, NbtCompound source)
        {
            var add_children = source.Tags.Where(x => !PsuedoContains(destination, x));
            var remove_children = destination.Tags.Where(x => !PsuedoContains(source, x));
            var update_children = destination.Tags.Except(remove_children);
            foreach (var child in remove_children)
            {
                destination.Remove(child);
            }
            foreach (var child in add_children)
            {
                destination.Add((NbtTag)child.Clone());
            }
            foreach (var child in update_children)
            {
                child.SetEqualTo(source[child.Name]);
            }
            destination.Sort(new ExistingCompoundSorter(source), false);
        }

        public static void SetListEqualTo(this NbtList destination, NbtList source)
        {
            if (destination.ListType != source.ListType)
                destination.Clear();
            while (destination.Count > source.Count)
            {
                destination.RemoveAt(destination.Count - 1);
            }
            int needs_updating = destination.Count;
            if (source.Count > destination.Count)
                destination.AddRange(source.Tags.Skip(destination.Count).Select(x => (NbtTag)x.Clone()));
            for (int i = 0; i < needs_updating; i++)
            {
                destination[i].SetEqualTo(source[i]);
            }
        }

        public static void TransformAdd(NbtTag tag, NbtContainerTag destination) => TransformAdd(new[] { tag }, destination);
        public static void TransformAdd(IEnumerable<NbtTag> tags, NbtContainerTag destination) => TransformInsert(tags, destination, destination.Count);
        public static void TransformInsert(NbtTag tag, NbtContainerTag destination, int index) => TransformInsert(new[] { tag }, destination, index);
        public static void TransformInsert(IEnumerable<NbtTag> tags, NbtContainerTag destination, int index)
        {
            //var adding = tags.ToList();
            //int original_index = index;
            //foreach (var tag in tags)
            //{
            //    if (!destination.CanAdd(tag.TagType))
            //    {
            //        adding.Remove(tag);
            //        continue;
            //    }
            //    if (tag.IsInside(destination) && original_index > tag.GetIndex())
            //        index--;
            //}
            //foreach (var tag in adding)
            //{
            //    tag.Remove();
            //}
            //foreach (var tag in adding)
            //{
            //    tag.Name = GetAutomaticName(tag, destination);
            //    tag.InsertInto(destination, index);
            //    index++;
            //}
        }

        public static string GetAutomaticName(NbtTag tag, NbtContainerTag parent)
        {
            if (parent is NbtList)
                return null;
            if (parent is NbtCompound compound)
            {
                if (tag.Name is not null && !compound.Contains(tag.Name))
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

        private class FileExtension
        {
            public readonly string Extension;
            public readonly string Description;
            public readonly NbtFileType Type;

            public FileExtension(string extension, string description, NbtFileType type)
            {
                Extension = extension;
                Description = description;
                Type = type;
            }
        }
        private static readonly List<FileExtension> NbtExtensions = new()
        {
            { new FileExtension("nbt", "NBT files", NbtFileType.BinaryNbt) },
            { new FileExtension("snbt", "SNBT files", NbtFileType.Snbt) },
            { new FileExtension("dat", "DAT files", NbtFileType.BinaryNbt) },
            { new FileExtension("mca", "Anvil Region files", NbtFileType.Region) },
            { new FileExtension("mcr", "Legacy Region files", NbtFileType.Region) },
            { new FileExtension("mcc", "External Chunk files", NbtFileType.Chunk) },
            { new FileExtension("mcstructure", "Bedrock Structure files", NbtFileType.BinaryNbt) },
            { new FileExtension("json", "JSON files", NbtFileType.Snbt) },
            { new FileExtension("schematic", "MCEdit Schematic files", NbtFileType.BinaryNbt) }
        };
        private static string GetEntry(FileExtension f) => $"{f.Description}|*.{f.Extension}";
        private static string AllFiles => "All Files|*";
        private static string AllNbtFiles(IEnumerable<FileExtension> source) => $"All NBT Files|{String.Join("; ", source.Select(x => "*." + x.Extension))}";
        private static string IndividualNbtFiles(IEnumerable<FileExtension> source) => String.Join("|", source.Select(GetEntry));
        public static string SaveFilter(string path, NbtFileType type)
        {
            var relevant = NbtExtensions.Where(x => x.Type == type);
            string all_relevant;
            if (ListUtils.ExactlyOne(relevant))
                all_relevant = "";
            else
                all_relevant = "|" + AllNbtFiles(relevant);
            if (path is null)
                return $"{IndividualNbtFiles(relevant)}{all_relevant}|{AllFiles}";
            else
            {
                string extension = Path.GetExtension(path);
                if (!relevant.Any(x => "." + x.Extension == extension))
                    return $"{AllFiles}|{IndividualNbtFiles(relevant)}{all_relevant}";
                relevant = relevant.OrderByDescending(x => "." + x.Extension == extension);
            }
            return $"{IndividualNbtFiles(relevant)}{all_relevant}|{AllFiles}";
        }
        public static string OpenFilter()
        {
            return $"{AllNbtFiles(NbtExtensions)}|{AllFiles}|{IndividualNbtFiles(NbtExtensions)}";
        }
        public static bool? BinaryExtension(string extension)
        {
            var ext = NbtExtensions.FirstOrDefault(x => "." + x.Extension == extension);
            if (ext is null)
                return null;
            return ext.Type != NbtFileType.Snbt;
        }
        public static NbtFileType GetFileType(IExportable saveable)
        {
            if (saveable is Chunk)
                return NbtFileType.Chunk;
            else if (saveable is RegionFile)
                return NbtFileType.Region;
            else
                return NbtFileType.BinaryNbt;
        }

        public enum NbtFileType
        {
            Snbt,
            BinaryNbt,
            Region,
            Chunk
        }

        public static IconType TagIconType(NbtTagType type)
        {
            return type switch
            {
                NbtTagType.Byte => IconType.ByteTag,
                NbtTagType.Short => IconType.ShortTag,
                NbtTagType.Int => IconType.IntTag,
                NbtTagType.Long => IconType.LongTag,
                NbtTagType.Float => IconType.FloatTag,
                NbtTagType.Double => IconType.DoubleTag,
                NbtTagType.String => IconType.StringTag,
                NbtTagType.ByteArray => IconType.ByteArrayTag,
                NbtTagType.IntArray => IconType.IntArrayTag,
                NbtTagType.LongArray => IconType.LongArrayTag,
                NbtTagType.Compound => IconType.CompoundTag,
                NbtTagType.List => IconType.ListTag,
                _ => throw new ArgumentException($"No icon for {type}")
            };
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
            if (tag.Parent is null)
                return -1;
            return tag.Parent.IndexOf(tag);
        }

        public static bool IsInside(this NbtTag tag, NbtContainerTag container)
        {
            return container.Contains(tag);
        }

        public static void Remove(this NbtTag tag)
        {
            if (tag.Parent is not null)
                tag.Parent.Remove(tag);
        }
    }
}
