using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using fNbt;

namespace NbtStudio.SNBT
{
    public static class Snbt
    {
        public const char BYTE_SUFFIX = 'b';
        public const char SHORT_SUFFIX = 's';
        public const char LONG_SUFFIX = 'L';
        public const char FLOAT_SUFFIX = 'f';
        public const char DOUBLE_SUFFIX = 'd';
        public const char BYTE_ARRAY_PREFIX = 'B';
        public const char INT_ARRAY_PREFIX = 'I';
        public const char LONG_ARRAY_PREFIX = 'L';
        public const char NAME_VALUE_SEPARATOR = ':';
        public const char VALUE_SEPARATOR = ',';
        public const char ARRAY_DELIMITER = ';';
        public const char LIST_OPEN = '[';
        public const char LIST_CLOSE = ']';
        public const char COMPOUND_OPEN = '{';
        public const char COMPOUND_CLOSE = '}';
        public const char STRING_ESCAPE = '\\';
        public const char STRING_PRIMARY_QUOTE = '"';
        public const char STRING_SECONDARY_QUOTE = '\'';
        public const string VALUE_SPACING = " ";
        public const string INDENTATION = "    ";
        private static readonly Regex StringRegex = new Regex("^[A-Za-z0-9._+-]+$", RegexOptions.Compiled);

        // convert a tag to its string form
        // expanded: for compounds and lists of non-numeric type, creates pretty indented structure. for all tags, causes spaces between values
        // delimit: for numeric types, determines whether the suffix should be included. for strings, determines whether the value should be quoted
        // include_name: whether to put the tag's name (if it has one) in front of its value
        public static string ToSnbt(this INbtTag tag, bool expanded = false, bool delimit = true, bool include_name = false)
        {
            string name = include_name ? GetNameBeforeValue(tag, expanded) : String.Empty;
            if (tag is INbtByte b)
                return name + b.ToSnbt(delimit);
            if (tag is INbtShort s)
                return name + s.ToSnbt(delimit);
            if (tag is INbtInt i)
                return name + i.ToSnbt();
            if (tag is INbtLong l)
                return name + l.ToSnbt(delimit);
            if (tag is INbtFloat f)
                return name + f.ToSnbt(delimit);
            if (tag is INbtDouble d)
                return name + d.ToSnbt(delimit);
            if (tag is INbtString str)
                return name + str.ToSnbt(delimit);
            if (tag is INbtByteArray ba)
                return name + ba.ToSnbt(expanded);
            if (tag is INbtIntArray ia)
                return name + ia.ToSnbt(expanded);
            if (tag is INbtLongArray la)
                return name + la.ToSnbt(expanded);
            if (tag is INbtList list)
                return name + list.ToSnbt(expanded);
            if (tag is INbtCompound compound)
                return name + compound.ToSnbt(expanded);
            throw new ArgumentException($"Can't convert tag of type {tag.TagType} to SNBT");
        }

        private static string OptionalSuffix(bool include, char suffix)
        {
            return include ? suffix.ToString() : String.Empty;
        }

        public static string ToSnbt(this INbtByte tag, bool suffix) => (sbyte)tag.Value + OptionalSuffix(suffix, BYTE_SUFFIX);
        public static string ToSnbt(this INbtShort tag, bool suffix) => tag.Value + OptionalSuffix(suffix, SHORT_SUFFIX);
        public static string ToSnbt(this INbtInt tag) => tag.Value.ToString();
        public static string ToSnbt(this INbtLong tag, bool suffix) => tag.Value + OptionalSuffix(suffix, LONG_SUFFIX);
        public static string ToSnbt(this INbtFloat tag, bool suffix)
        {
            string result;
            if (float.IsPositiveInfinity(tag.Value))
                result = "Infinity";
            else if (float.IsNegativeInfinity(tag.Value))
                result = "-Infinity";
            else if (float.IsNaN(tag.Value))
                result = "NaN";
            else
                result = Util.FloatToString(tag.Value);
            return result + OptionalSuffix(suffix, FLOAT_SUFFIX);
        }
        public static string ToSnbt(this INbtDouble tag, bool suffix)
        {
            string result;
            if (double.IsPositiveInfinity(tag.Value))
                result = "Infinity";
            else if (double.IsNegativeInfinity(tag.Value))
                result = "-Infinity";
            else if (double.IsNaN(tag.Value))
                result = "NaN";
            else
                result = Util.DoubleToString(tag.Value);
            return result + OptionalSuffix(suffix, DOUBLE_SUFFIX);
        }
        public static string ToSnbt(this INbtString tag, bool quotes)
        {
            if (quotes)
                return QuoteAndEscape(tag.Value);
            return tag.Value;
        }

        public static string ToSnbt(this INbtByteArray tag, bool spaced = false)
        {
            return ListToString("" + BYTE_ARRAY_PREFIX + ARRAY_DELIMITER, x => ((sbyte)x).ToString() + BYTE_SUFFIX, tag.Value, spaced);
        }

        public static string ToSnbt(this INbtIntArray tag, bool spaced = false)
        {
            return ListToString("" + INT_ARRAY_PREFIX + ARRAY_DELIMITER, x => x.ToString(), tag.Value, spaced);
        }

        public static string ToSnbt(this INbtLongArray tag, bool spaced = false)
        {
            return ListToString("" + LONG_ARRAY_PREFIX + ARRAY_DELIMITER, x => x.ToString() + LONG_SUFFIX, tag.Value, spaced);
        }

        public static string ToSnbt(this INbtList tag, bool expanded = false)
        {
            if (expanded)
            {
                var sb = new StringBuilder();
                AddSnbtList(tag, sb, INDENTATION, 0, false);
                return sb.ToString();
            }
            else
                return ListToString("", x => x.ToSnbt(expanded: false, delimit: true, include_name: false), tag, spaced: false);
        }

        public static string ToSnbt(this INbtCompound tag, bool expanded = false)
        {
            var sb = new StringBuilder();
            if (expanded)
                AddSnbtCompound(tag, sb, INDENTATION, 0, false);
            else
            {
                sb.Append(COMPOUND_OPEN);
                sb.Append(String.Join(VALUE_SEPARATOR.ToString(), tag.Select(x => x.ToSnbt(expanded: false, include_name: true)).ToArray()));
                sb.Append(COMPOUND_CLOSE);
            }
            return sb.ToString();
        }

        // shared technique for single-line arrays
        // (list, int array, byte array)
        private static string ListToString<T>(string list_prefix, Func<T, string> function, IEnumerable<T> values, bool spaced = false)
        {
            // spacing between values
            string spacing = spaced ? VALUE_SPACING : String.Empty;
            // spacing between list prefix and first value
            string prefix_separator = spaced && list_prefix.Length > 0 && values.Any() ? VALUE_SPACING : String.Empty;
            var s = new StringBuilder(LIST_OPEN + list_prefix + prefix_separator);
            string contents = String.Join(VALUE_SEPARATOR + spacing, values.Select(x => function(x)));
            s.Append(contents);
            s.Append(LIST_CLOSE);
            return s.ToString();
        }

        private static string GetName(INbtTag tag)
        {
            if (StringRegex.IsMatch(tag.Name))
                return tag.Name;
            else
                return QuoteAndEscape(tag.Name);
        }

        private static string GetNameBeforeValue(INbtTag tag, bool spaced)
        {
            if (tag.Name == null)
                return String.Empty;
            return GetName(tag) + NAME_VALUE_SEPARATOR + (spaced ? VALUE_SPACING : String.Empty);
        }

        // adapted directly from minecraft's (decompiled) source
        private static string QuoteAndEscape(string input)
        {
            const char PLACEHOLDER_QUOTE = '\0';
            var builder = new StringBuilder(PLACEHOLDER_QUOTE.ToString()); // dummy value to be replaced at end
            char preferred_quote = PLACEHOLDER_QUOTE; // dummy value when we're not sure which quote type to use yet
            foreach (char c in input)
            {
                if (c == STRING_ESCAPE)
                    builder.Append(STRING_ESCAPE);
                else if (c == STRING_PRIMARY_QUOTE || c == STRING_SECONDARY_QUOTE)
                {
                    // if we find one of the quotes in the actual string text, use the other one for quoting
                    if (preferred_quote == PLACEHOLDER_QUOTE)
                        preferred_quote = (c == STRING_PRIMARY_QUOTE ? STRING_SECONDARY_QUOTE : STRING_PRIMARY_QUOTE);
                    if (c == preferred_quote)
                        builder.Append(STRING_ESCAPE);
                }
                if (c == '\n')
                    builder.Append("\\n");
                else
                    builder.Append(c);
            }
            if (preferred_quote == PLACEHOLDER_QUOTE)
                preferred_quote = STRING_PRIMARY_QUOTE;
            builder[0] = preferred_quote;
            builder.Append(preferred_quote);
            return builder.ToString();
        }

        private static void AddIndents(StringBuilder sb, string indent_string, int indent_level)
        {
            for (int i = 0; i < indent_level; i++)
            {
                sb.Append(indent_string);
            }
        }

        // add contents of tag to stringbuilder
        // used for aligning indents for multiline compounds and lists
        private static void AddSnbt(INbtTag tag, StringBuilder sb, string indent_string, int indent_level, bool include_name)
        {
            if (tag is INbtCompound compound)
                AddSnbtCompound(compound, sb, indent_string, indent_level, include_name);
            else if (tag is INbtList list)
                AddSnbtList(list, sb, indent_string, indent_level, include_name);
            else
            {
                AddIndents(sb, indent_string, indent_level);
                sb.Append(tag.ToSnbt(expanded: true, include_name: include_name));
            }
        }

        private static void AddSnbtCompound(INbtCompound tag, StringBuilder sb, string indent_string, int indent_level, bool include_name)
        {
            AddIndents(sb, indent_string, indent_level);
            if (include_name)
                sb.Append(GetNameBeforeValue(tag, spaced: true));
            sb.Append(COMPOUND_OPEN);
            if (tag.Count > 0)
            {
                sb.Append(Environment.NewLine);
                var children = tag.Tags.ToArray();
                for (int i = 0; i < children.Length; i++)
                {
                    AddSnbt(children[i], sb, indent_string, indent_level + 1, true);
                    if (i < children.Length - 1)
                        sb.Append(VALUE_SEPARATOR);
                    sb.Append(Environment.NewLine);
                }
                AddIndents(sb, indent_string, indent_level);
            }
            sb.Append('}');
        }

        private static void AddSnbtList(INbtList tag, StringBuilder sb, string indent_string, int indent_level, bool include_name)
        {
            AddIndents(sb, indent_string, indent_level);
            if (include_name)
                sb.Append(GetNameBeforeValue(tag, spaced: true));
            bool compressed = ShouldCompressListOf(tag.ListType);
            if (compressed)
                sb.Append(ListToString("", x => x.ToSnbt(false), tag, true));
            else
            {
                sb.Append(LIST_OPEN);
                if (tag.Count > 0)
                {
                    sb.Append(Environment.NewLine);
                    for (int i = 0; i < tag.Count; i++)
                    {
                        AddSnbt(tag[i], sb, indent_string, indent_level + 1, false);
                        if (i < tag.Count - 1)
                            sb.Append(VALUE_SEPARATOR);
                        sb.Append(Environment.NewLine);
                    }
                    AddIndents(sb, indent_string, indent_level);

                }
                sb.Append(LIST_CLOSE);
            }
        }

        // when a multiline list contains this type, should it keep all the values on one line anyway?
        private static bool ShouldCompressListOf(NbtTagType type)
        {
            switch (type)
            {
                case NbtTagType.String:
                case NbtTagType.List:
                case NbtTagType.Compound:
                case NbtTagType.IntArray:
                case NbtTagType.ByteArray:
                case NbtTagType.LongArray:
                    return false;
                default:
                    return true;
            }
        }
    }
}
