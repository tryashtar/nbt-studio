using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public static class Util
    {
        public static string Pluralize(int count, string word) => Pluralize(count, word, word + "s");
        public static string Pluralize(int count, string word, string plural)
        {
            if (count == 1)
                return $"{count} {word}";
            return $"{count} {plural}";
        }

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

        public static bool IsValueType(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                case NbtTagType.Double:
                case NbtTagType.Float:
                case NbtTagType.Int:
                case NbtTagType.Long:
                case NbtTagType.Short:
                case NbtTagType.String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSizeType(NbtTagType type)
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

        // value is parsed to the proper type and inserted into the tag
        // value is treated as size for arrays
        public static NbtTag CreateTag(NbtTagType type, string name, string value)
        {
            var tag = CreateTag(type, name);
            if (tag is NbtByte tag_byte)
                tag_byte.Value = (byte)sbyte.Parse(value);
            else if (tag is NbtShort tag_short)
                tag_short.Value = short.Parse(value);
            else if (tag is NbtInt tag_int)
                tag_int.Value = int.Parse(value);
            else if (tag is NbtFloat tag_float)
                tag_float.Value = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            else if (tag is NbtDouble tag_double)
                tag_double.Value = double.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
            else if (tag is NbtString tag_string)
                tag_string.Value = value;
            else if (tag is NbtByteArray tag_byte_array)
                tag_byte_array.Value = new byte[ParseNonNegativeInt(value)];
            else if (tag is NbtIntArray tag_int_array)
                tag_int_array.Value = new int[ParseNonNegativeInt(value)];
            else if (tag is NbtLongArray tag_long_array)
                tag_long_array.Value = new long[ParseNonNegativeInt(value)];
            return tag;
        }

        private static int ParseNonNegativeInt(string value)
        {
            int val = int.Parse(value);
            if (val < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            return val;
        }

        public static NbtTag CreateTag(NbtTagType type, string name)
        {
            var tag = CreateTag(type);
            tag.Name = name;
            return tag;
        }

        public static NbtTag CreateTag(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return new NbtByte();
                case NbtTagType.ByteArray:
                    return new NbtByteArray();
                case NbtTagType.Compound:
                    return new NbtCompound();
                case NbtTagType.Double:
                    return new NbtDouble();
                case NbtTagType.Float:
                    return new NbtFloat();
                case NbtTagType.Int:
                    return new NbtInt();
                case NbtTagType.IntArray:
                    return new NbtIntArray();
                case NbtTagType.List:
                    return new NbtList();
                case NbtTagType.Long:
                    return new NbtLong();
                case NbtTagType.LongArray:
                    return new NbtLongArray();
                case NbtTagType.Short:
                    return new NbtShort();
                case NbtTagType.String:
                    return new NbtString();
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

        public static bool CanAdd(NbtTag tag, NbtTagType type)
        {
            if (tag is NbtCompound)
                return true;
            if (tag is NbtList list)
                return list.Count == 0 || list.ListType == type;
            return false;
        }

        public static string PreviewNbtValue(NbtTag tag)
        {
            if (tag is NbtCompound compound)
                return $"[{Util.Pluralize(compound.Count, "entry", "entries")}]";
            else if (tag is NbtList list)
            {
                if (list.Count == 0)
                    return $"[0 entries]";
                return $"[{Util.Pluralize(list.Count, Util.TagTypeName(list.ListType).ToLower())}]";
            }
            else if (tag is NbtByteArray byte_array)
                return $"[{Util.Pluralize(byte_array.Value.Length, "byte")}]";
            else if (tag is NbtIntArray int_array)
                return $"[{Util.Pluralize(int_array.Value.Length, "int")}]";
            else if (tag is NbtLongArray long_array)
                return $"[{Util.Pluralize(long_array.Value.Length, "long")}]";
            return tag.ToSnbt(multiline: false, delimit: false);
        }

        public static string PreviewNbtValue(NbtFile file) => PreviewNbtValue(file.RootTag);

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
