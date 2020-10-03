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
    // provides psuedo-interfaces to NBT tags as methods
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

        public static object GetValue(INbtTag tag)
        {
            if (tag is INbtByte tag_byte)
                return tag_byte.Value;
            else if (tag is INbtShort tag_short)
                return tag_short.Value;
            else if (tag is INbtInt tag_int)
                return tag_int.Value;
            else if (tag is INbtLong tag_long)
                return tag_long.Value;
            else if (tag is INbtFloat tag_float)
                return tag_float.Value;
            else if (tag is INbtDouble tag_double)
                return tag_double.Value;
            else if (tag is INbtString tag_string)
                return tag_string.Value;
            else if (tag is INbtByteArray tag_ba)
                return tag_ba.Value;
            else if (tag is INbtIntArray tag_ia)
                return tag_ia.Value;
            else if (tag is INbtLongArray tag_la)
                return tag_la.Value;
            else if (tag is INbtCompound tag_compound)
                return tag_compound.Tags;
            else if (tag is INbtList tag_list)
                return tag_list;
            throw new ArgumentException($"Can't get value from {tag.TagType}");
        }

        public static void SetValue(INbtTag tag, object value)
        {
            if (tag is INbtByte tag_byte && value is byte b)
                tag_byte.Value = b;
            else if (tag is INbtShort tag_short && value is short s)
                tag_short.Value = s;
            else if (tag is INbtInt tag_int && value is int i)
                tag_int.Value = i;
            else if (tag is INbtLong tag_long && value is long l)
                tag_long.Value = l;
            else if (tag is INbtFloat tag_float && value is float f)
                tag_float.Value = f;
            else if (tag is INbtDouble tag_double && value is double d)
                tag_double.Value = d;
            else if (tag is INbtString tag_string && value is string str)
                tag_string.Value = str;
            else if (tag is INbtByteArray tag_ba && value is byte[] bytes)
                tag_ba.Value = bytes;
            else if (tag is INbtIntArray tag_ia && value is int[] ints)
                tag_ia.Value = ints;
            else if (tag is INbtLongArray tag_la && value is long[] longs)
                tag_la.Value = longs;
            else if (tag is INbtCompound tag_compound && value is IEnumerable<INbtTag> c_tags)
            {
                var tags = c_tags.ToList();
                tag_compound.Clear();
                foreach (var child in tags)
                {
                    child.AddTo(tag_compound);
                }
            }
            else if (tag is INbtList tag_list && value is IEnumerable<INbtTag> l_tags)
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
                    return (byte)sbyte.Parse(value);
                case NbtTagType.Short:
                    return short.Parse(value);
                case NbtTagType.Int:
                    return int.Parse(value);
                case NbtTagType.Long:
                    return long.Parse(value);
                case NbtTagType.Float:
                    return float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                case NbtTagType.Double:
                    return double.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                case NbtTagType.String:
                    return value;
                default:
                    return null;
            }
        }

        // clears any existing data in the tag's array
        public static void SetSize(INbtTag tag, int size)
        {
            if (tag is INbtByteArray tag_byte_array)
                tag_byte_array.Value = new byte[size];
            else if (tag is INbtIntArray tag_int_array)
                tag_int_array.Value = new int[size];
            else if (tag is INbtLongArray tag_long_array)
                tag_long_array.Value = new long[size];
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

        public static Tuple<string, string> MinMaxFor(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Tuple.Create(sbyte.MinValue.ToString(), sbyte.MaxValue.ToString());
                case NbtTagType.Short:
                    return Tuple.Create(short.MinValue.ToString(), short.MaxValue.ToString());
                case NbtTagType.Int:
                    return Tuple.Create(int.MinValue.ToString(), int.MaxValue.ToString());
                case NbtTagType.Long:
                    return Tuple.Create(long.MinValue.ToString(), long.MaxValue.ToString());
                case NbtTagType.Float:
                    return Tuple.Create(float.MinValue.ToString(), float.MaxValue.ToString());
                case NbtTagType.Double:
                    return Tuple.Create(double.MinValue.ToString(), double.MaxValue.ToString());
                default:
                    throw new ArgumentException($"{type} isn't numeric, has no min and max");
            }
        }

        public static string PreviewNbtValue(INbtTag tag)
        {
            if (tag is INbtCompound compound)
                return $"[{Util.Pluralize(compound.Count, "entry", "entries")}]";
            else if (tag is INbtList list)
            {
                if (list.Count == 0)
                    return $"[0 entries]";
                return $"[{Util.Pluralize(list.Count, TagTypeName(list.ListType).ToLower())}]";
            }
            else if (tag is INbtByteArray byte_array)
                return $"[{Util.Pluralize(byte_array.Value.Length, "byte")}]";
            else if (tag is INbtIntArray int_array)
                return $"[{Util.Pluralize(int_array.Value.Length, "int")}]";
            else if (tag is INbtLongArray long_array)
                return $"[{Util.Pluralize(long_array.Value.Length, "long")}]";
            return tag.ToSnbt(expanded: false, delimit: false);
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

        public class TagNameSorter : IComparer<INbtTag>
        {
            public int Compare(INbtTag x, INbtTag y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        public class TagTypeSorter : IComparer<INbtTag>
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
            public int Compare(INbtTag x, INbtTag y)
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

        public static void Sort(INbtCompound compound, IComparer<INbtTag> sorter, bool recursive)
        {
            var tags = compound.Tags.OrderBy(x => x, sorter).ToList();
            compound.Clear();
            foreach (var tag in tags)
            {
                if (recursive)
                {
                    if (tag is INbtCompound sub)
                        Sort(sub, sorter, true);
                    else if (tag is INbtList list)
                        SortChildren(list, sorter);
                }
                tag.AddTo(compound);
            }
        }

        private static void SortChildren(INbtList list, IComparer<INbtTag> sorter)
        {
            if (list.ListType == NbtTagType.Compound)
            {
                foreach (INbtCompound item in list)
                {
                    Sort(item, sorter, true);
                }
            }
            else if (list.ListType == NbtTagType.List)
            {
                foreach (INbtList item in list)
                {
                    SortChildren(item, sorter);
                }
            }
        }

        public static Tuple<INbtContainer, int> GetInsertionLocation(INbtTag target, NodePosition position)
        {
            if (position == NodePosition.Inside)
            {
                var container = target as INbtContainer;
                return Tuple.Create(container, container?.Count ?? 0);
            }
            else
            {
                var parent = target.Parent;
                int index = target.Index;
                if (position == NodePosition.After)
                    index++;
                return Tuple.Create(parent, index);
            }
        }

        public static void TransformInsert(INbtTag tag, INbtContainer destination, int index)
        {
            if (!destination.CanAdd(tag.TagType))
                return;
            if (tag.IsInside(destination) && index > tag.Index)
                index--;
            tag.Remove();
            if (destination is INbtCompound compound)
                tag.Name = GetAutomaticName(tag, compound);
            else if (destination is INbtList)
                tag.Name = null;
            tag.InsertInto(destination, index);
        }

        public static string GetAutomaticName(INbtTag tag, INbtCompound parent)
        {
            if (tag.Name != null && !parent.Contains(tag.Name))
                return tag.Name;
            string basename = tag.Name ?? TagTypeName(tag.TagType).ToLower().Replace(' ', '_');
            for (int i = 1; i < 999999; i++)
            {
                string name = basename + i.ToString();
                if (!parent.Contains(name))
                    return name;
            }
            throw new InvalidOperationException("This compound really contains 999999 similarly named tags?!");
        }

        public static void TransformAdd(INbtTag tag, INbtContainer destination) => TransformInsert(tag, destination, destination.Count);

        public static bool CanAddAll(IEnumerable<INbtTag> tags, INbtContainer destination)
        {
            // check if you're trying to add items of different types to a list
            if (destination is INbtList list && tags.Select(x => x.TagType).Distinct().Skip(1).Any())
                return false;
            // check if you're trying to add an item to its own descendent
            var ancestors = Ancestors(destination);
            if (tags.Intersect(ancestors).Any())
                return false;
            return tags.All(x => destination.CanAdd(x.TagType));
        }

        public static List<INbtTag> Ancestors(INbtTag tag)
        {
            var ancestors = new List<INbtTag>();
            while (tag != null)
            {
                ancestors.Add(tag);
                tag = tag.Parent;
            }
            return ancestors;
        }

        private static readonly string[] NbtExtensions = new[] { "nbt", "snbt", "dat", "mca", "mcr", "mcstructure", "schematic" };
        public static string SaveFilter()
        {
            return "NBT Files|" + String.Join("; ", NbtExtensions.Select(x => "*." + x)) + "|All Files|*";
        }
        public static string OpenFilter()
        {
            return "All Files|*|NBT Files|" + String.Join("; ", NbtExtensions.Select(x => "*." + x));
        }

        public static Image TagTypeImage(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Properties.Resources.tag_byte_image;
                case NbtTagType.Short:
                    return Properties.Resources.tag_short_image;
                case NbtTagType.Int:
                    return Properties.Resources.tag_int_image;
                case NbtTagType.Long:
                    return Properties.Resources.tag_long_image;
                case NbtTagType.Float:
                    return Properties.Resources.tag_float_image;
                case NbtTagType.Double:
                    return Properties.Resources.tag_double_image;
                case NbtTagType.ByteArray:
                    return Properties.Resources.tag_byte_array_image;
                case NbtTagType.String:
                    return Properties.Resources.tag_string_image;
                case NbtTagType.List:
                    return Properties.Resources.tag_list_image;
                case NbtTagType.Compound:
                    return Properties.Resources.tag_compound_image;
                case NbtTagType.IntArray:
                    return Properties.Resources.tag_int_array_image;
                case NbtTagType.LongArray:
                    return Properties.Resources.tag_long_array_image;
                default:
                    return null;
            }
        }

        public static Icon TagTypeIcon(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Properties.Resources.tag_byte_icon;
                case NbtTagType.Short:
                    return Properties.Resources.tag_short_icon;
                case NbtTagType.Int:
                    return Properties.Resources.tag_int_icon;
                case NbtTagType.Long:
                    return Properties.Resources.tag_long_icon;
                case NbtTagType.Float:
                    return Properties.Resources.tag_float_icon;
                case NbtTagType.Double:
                    return Properties.Resources.tag_double_icon;
                case NbtTagType.ByteArray:
                    return Properties.Resources.tag_byte_array_icon;
                case NbtTagType.String:
                    return Properties.Resources.tag_string_icon;
                case NbtTagType.List:
                    return Properties.Resources.tag_list_icon;
                case NbtTagType.Compound:
                    return Properties.Resources.tag_compound_icon;
                case NbtTagType.IntArray:
                    return Properties.Resources.tag_int_array_icon;
                case NbtTagType.LongArray:
                    return Properties.Resources.tag_long_array_icon;
                default:
                    return null;
            }
        }
    }
}
