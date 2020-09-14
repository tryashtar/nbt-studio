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

        public static void SetValue(NbtTag tag, object value)
        {
            if (tag is NbtByte tag_byte && value is byte b)
                tag_byte.Value = b;
            else if (tag is NbtShort tag_short && value is short s)
                tag_short.Value = s;
            else if (tag is NbtInt tag_int && value is int i)
                tag_int.Value = i;
            else if (tag is NbtFloat tag_float && value is float f)
                tag_float.Value = f;
            else if (tag is NbtDouble tag_double && value is double d)
                tag_double.Value = d;
            else if (tag is NbtString tag_string && value is string str)
                tag_string.Value = str;
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
        public static void SetSize(NbtTag tag, int size)
        {
            if (tag is NbtByteArray tag_byte_array)
                tag_byte_array.Value = new byte[size];
            else if (tag is NbtIntArray tag_int_array)
                tag_int_array.Value = new int[size];
            else if (tag is NbtLongArray tag_long_array)
                tag_long_array.Value = new long[size];
        }

        public static int ParseNonNegativeInt(string value)
        {
            int val = int.Parse(value);
            if (val < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            return val;
        }

        public static string DoubleToString(double d)
        {
            return d.ToString("0." + new string('#', 339));
        }

        public static string FloatToString(float f)
        {
            return f.ToString("0." + new string('#', 339));
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
                    return Tuple.Create(FloatToString(float.MinValue), FloatToString(float.MaxValue));
                case NbtTagType.Double:
                    return Tuple.Create(DoubleToString(double.MinValue), DoubleToString(double.MaxValue));
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

        public static void Add(NbtTag parent, NbtTag child)
        {
            if (parent is NbtCompound compound)
                compound.Add(child);
            if (parent is NbtList list)
                list.Add(child);
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
