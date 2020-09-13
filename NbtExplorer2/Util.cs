using fNbt;
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

        public static int? ChildrenCount(object obj)
        {
            if (obj is NbtFile file)
                return file.RootTag.Count;
            if (obj is NbtCompound compound)
                return compound.Count;
            if (obj is NbtList list)
                return list.Count;
            return null;
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

        public static Image TagTypeImage(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Properties.Resources.byte_image;
                case NbtTagType.Short:
                    return Properties.Resources.short_image;
                case NbtTagType.Int:
                    return Properties.Resources.int_image;
                case NbtTagType.Long:
                    return Properties.Resources.long_image;
                case NbtTagType.Float:
                    return Properties.Resources.float_image;
                case NbtTagType.Double:
                    return Properties.Resources.double_image;
                case NbtTagType.ByteArray:
                    return Properties.Resources.byte_array_image;
                case NbtTagType.String:
                    return Properties.Resources.string_image;
                case NbtTagType.List:
                    return Properties.Resources.list_image;
                case NbtTagType.Compound:
                    return Properties.Resources.compound_image;
                case NbtTagType.IntArray:
                    return Properties.Resources.int_array_image;
                case NbtTagType.LongArray:
                    return Properties.Resources.long_array_image;
                default:
                    return null;
            }
        }

        public static Icon TagTypeIcon(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.Byte:
                    return Properties.Resources.byte_icon;
                case NbtTagType.Short:
                    return Properties.Resources.short_icon;
                case NbtTagType.Int:
                    return Properties.Resources.int_icon;
                case NbtTagType.Long:
                    return Properties.Resources.long_icon;
                case NbtTagType.Float:
                    return Properties.Resources.float_icon;
                case NbtTagType.Double:
                    return Properties.Resources.double_icon;
                case NbtTagType.ByteArray:
                    return Properties.Resources.byte_array_icon;
                case NbtTagType.String:
                    return Properties.Resources.string_icon;
                case NbtTagType.List:
                    return Properties.Resources.list_icon;
                case NbtTagType.Compound:
                    return Properties.Resources.compound_icon;
                case NbtTagType.IntArray:
                    return Properties.Resources.int_array_icon;
                case NbtTagType.LongArray:
                    return Properties.Resources.long_array_icon;
                default:
                    return null;
            }
        }
    }
}
