using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using fNbt;

namespace NbtExplorer2.SNBT
{
    public static class SnbtMaker
    {
        // convert a tag to its string form
        public static string ToSnbt(this NbtTag tag, bool multiline = false, bool delimit = true)
        {
            switch (tag.TagType)
            {
                case NbtTagType.Byte:
                    return ((NbtByte)tag).ToSnbt(delimit);
                case NbtTagType.Short:
                    return ((NbtShort)tag).ToSnbt(delimit);
                case NbtTagType.Int:
                    return ((NbtInt)tag).ToSnbt();
                case NbtTagType.Long:
                    return ((NbtLong)tag).ToSnbt(delimit);
                case NbtTagType.Float:
                    return ((NbtFloat)tag).ToSnbt(delimit);
                case NbtTagType.Double:
                    return ((NbtDouble)tag).ToSnbt(delimit);
                case NbtTagType.ByteArray:
                    return ((NbtByteArray)tag).ToSnbt(multiline);
                case NbtTagType.String:
                    return ((NbtString)tag).ToSnbt(delimit);
                case NbtTagType.List:
                    return ((NbtList)tag).ToSnbt(multiline);
                case NbtTagType.Compound:
                    return ((NbtCompound)tag).ToSnbt(multiline);
                case NbtTagType.IntArray:
                    return ((NbtIntArray)tag).ToSnbt(multiline);
                case NbtTagType.LongArray:
                    return ((NbtLongArray)tag).ToSnbt(multiline);
                default:
                    return "";
            }
        }

        public static string ToSnbt(this NbtByte tag, bool suffix) => $"{(sbyte)tag.Value}" + (suffix ? "b" : "");
        public static string ToSnbt(this NbtShort tag, bool suffix) => $"{tag.Value}" + (suffix ? "s" : "");
        public static string ToSnbt(this NbtInt tag) => $"{tag.Value}";
        public static string ToSnbt(this NbtLong tag, bool suffix) => $"{tag.Value}" + (suffix ? "L" : "");
        public static string ToSnbt(this NbtFloat tag, bool suffix)
        {
            string result;
            if (tag.Value == float.PositiveInfinity)
                result = "Infinity";
            else if (tag.Value == float.NegativeInfinity)
                result = "-Infinity";
            else if (tag.Value == float.NaN)
                result = "NaN";
            else
                result = tag.Value.ToString("0." + new string('#', 339));
            return result + (suffix ? "f" : "");
        }
        public static string ToSnbt(this NbtDouble tag, bool suffix)
        {
            string result;
            if (tag.Value == double.PositiveInfinity)
                result = "Infinity";
            else if (tag.Value == double.NegativeInfinity)
                result = "-Infinity";
            else if (tag.Value == double.NaN)
                result = "NaN";
            else
                result = tag.Value.ToString("0." + new string('#', 339));
            return result + (suffix ? "d" : "");
        }
        public static string ToSnbt(this NbtString tag, bool quotes = true)
        {
            if (quotes)
                return QuoteAndEscape(tag.Value);
            return tag.Value;
        }

        public static string ToSnbt(this NbtByteArray tag, bool multiline = false)
        {
            return ListToString("B;", x => $"{(sbyte)x}b", tag.Value, multiline);
        }

        public static string ToSnbt(this NbtIntArray tag, bool multiline = false)
        {
            return ListToString("I;", x => x.ToString(), tag.Value, multiline);
        }

        public static string ToSnbt(this NbtLongArray tag, bool multiline = false)
        {
            return ListToString("L;", x => $"{x}L", tag.Value, multiline);
        }

        public static string ToSnbt(this NbtList tag, bool multiline = false)
        {
            if (!multiline)
                return ListToString("", x => x.ToSnbt(false), tag, false);
            var sb = new StringBuilder();
            AddSnbt(tag, sb, "    ", 0, false);
            return sb.ToString();
        }

        public static string ToSnbt(this NbtCompound tag, bool multiline = false)
        {
            var sb = new StringBuilder();
            if (multiline)
                AddSnbt(tag, sb, "    ", 0, false);
            else
            {
                sb.Append("{");
                sb.Append(String.Join(",", tag.Select(x => GetName(x) + ":" + x.ToSnbt(false)).ToArray()));
                sb.Append("}");
            }
            return sb.ToString();
        }

        private static readonly Regex StringRegex;

        static SnbtMaker()
        {
            StringRegex = new Regex("^[A-Za-z0-9._+-]+$", RegexOptions.Compiled);
        }

        // shared technique for single-line arrays
        // (list, int array, byte array)
        private static string ListToString<T>(string list_prefix, Func<T, string> function, IEnumerable<T> values, bool spaced = false)
        {
            string separator = spaced ? " " : "";
            string prefix_separator = spaced && list_prefix.Length > 0 && values.Any() ? " " : "";
            var s = new StringBuilder("[" + list_prefix + prefix_separator);
            string contents = String.Join("," + separator, values.Select(x => function(x)).ToArray());
            s.Append(contents);
            s.Append("]");
            return s.ToString();
        }

        private static string GetName(NbtTag tag)
        {
            if (StringRegex.IsMatch(tag.Name))
                return tag.Name;
            else
                return QuoteAndEscape(tag.Name);
        }

        // adapted directly from minecraft's (decompiled) source
        private static string QuoteAndEscape(string input)
        {
            var builder = new StringBuilder(" ");
            char preferred_quote = '\0';
            foreach (char c in input)
            {
                if (c == '\\')
                    builder.Append('\\');
                else if (c == '"' || c == '\'')
                {
                    if (preferred_quote == '\0')
                        preferred_quote = (c == '"' ? '\'' : '"');
                    if (c == preferred_quote)
                        builder.Append('\\');
                }
                builder.Append(c);
            }
            if (preferred_quote == '\0')
                preferred_quote = '"';
            builder[0] = preferred_quote;
            builder.Append(preferred_quote);
            return builder.ToString();
        }

        private static void AddIndents(StringBuilder sb, string indentString, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append(indentString);
            }
        }

        // add contents of tag to stringbuilder
        // used for aligning indents for multiline compounds and lists
        private static void AddSnbt(NbtTag tag, StringBuilder sb, string indentString, int indentLevel, bool includeName)
        {
            if (tag.TagType == NbtTagType.Compound)
                AddSnbt((NbtCompound)tag, sb, indentString, indentLevel, includeName);
            else if (tag.TagType == NbtTagType.List)
                AddSnbt((NbtList)tag, sb, indentString, indentLevel, includeName);
            else
            {
                AddIndents(sb, indentString, indentLevel);
                if (includeName)
                    sb.Append(GetName(tag) + ": ");
                sb.Append(tag.ToSnbt(true));
            }
        }

        private static void AddSnbt(NbtCompound tag, StringBuilder sb, string indentString, int indentLevel, bool includeName)
        {
            AddIndents(sb, indentString, indentLevel);
            if (includeName)
                sb.Append(GetName(tag) + ": ");
            sb.Append("{");
            if (tag.Count > 0)
            {
                sb.Append(Environment.NewLine);
                var children = tag.Tags.ToArray();
                for (int i = 0; i < children.Length; i++)
                {
                    AddSnbt(children[i], sb, indentString, indentLevel + 1, true);
                    if (i < children.Length - 1)
                        sb.Append(',');
                    sb.Append(Environment.NewLine);
                }
                AddIndents(sb, indentString, indentLevel);
            }
            sb.Append('}');
        }

        private static void AddSnbt(NbtList tag, StringBuilder sb, string indentString, int indentLevel, bool includeName)
        {
            AddIndents(sb, indentString, indentLevel);
            if (includeName)
                sb.Append(GetName(tag) + ": ");
            bool lines =
                (tag.ListType == NbtTagType.Compound ||
                tag.ListType == NbtTagType.String ||
                tag.ListType == NbtTagType.List ||
                tag.ListType == NbtTagType.IntArray ||
                tag.ListType == NbtTagType.ByteArray ||
                tag.ListType == NbtTagType.LongArray);
            if (!lines)
                sb.Append(ListToString("", x => x.ToSnbt(false), tag, true));
            else
            {
                sb.Append("[");
                if (tag.Count > 0)
                {
                    sb.Append(Environment.NewLine);
                    for (int i = 0; i < tag.Count; i++)
                    {
                        AddSnbt(tag[i], sb, indentString, indentLevel + 1, false);
                        if (i < tag.Count - 1)
                            sb.Append(',');
                        sb.Append(Environment.NewLine);
                    }
                    AddIndents(sb, indentString, indentLevel);

                }
                sb.Append(']');
            }
        }
    }
}
