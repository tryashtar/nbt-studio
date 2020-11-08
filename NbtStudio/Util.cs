using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public static class Util
    {
        public static readonly Color SelectionColor = Color.FromArgb(181, 215, 243);
        public static string Pluralize(int count, string word) => Pluralize(count, word, word + "s");
        public static string Pluralize(int count, string word, string plural)
        {
            if (count == 1)
                return $"{count} {word}";
            return $"{count} {plural}";
        }

        public static sbyte ParseByte(string value)
        {
            if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return 1;
            if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                return 0;
            return sbyte.Parse(value);
        }

        public static double ParseDouble(string value)
        {
            if (value.Equals("∞", StringComparison.OrdinalIgnoreCase))
                return double.PositiveInfinity;
            if (value.Equals("+∞", StringComparison.OrdinalIgnoreCase))
                return double.PositiveInfinity;
            if (value.Equals("-∞", StringComparison.OrdinalIgnoreCase))
                return double.NegativeInfinity;
            if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase))
                return double.PositiveInfinity;
            if (value.Equals("+Infinity", StringComparison.OrdinalIgnoreCase))
                return double.PositiveInfinity;
            if (value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase))
                return double.NegativeInfinity;
            if (value.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                return double.NaN;
            return double.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static float ParseFloat(string value)
        {
            if (value.Equals("∞", StringComparison.OrdinalIgnoreCase))
                return float.PositiveInfinity;
            if (value.Equals("+∞", StringComparison.OrdinalIgnoreCase))
                return float.PositiveInfinity;
            if (value.Equals("-∞", StringComparison.OrdinalIgnoreCase))
                return float.NegativeInfinity;
            if (value.Equals("Infinity", StringComparison.OrdinalIgnoreCase))
                return float.PositiveInfinity;
            if (value.Equals("+Infinity", StringComparison.OrdinalIgnoreCase))
                return float.PositiveInfinity;
            if (value.Equals("-Infinity", StringComparison.OrdinalIgnoreCase))
                return float.NegativeInfinity;
            if (value.Equals("NaN", StringComparison.OrdinalIgnoreCase))
                return float.NaN;
            return float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string DoubleToString(double d)
        {
            return d.ToString("0." + new string('#', 339));
        }

        public static string FloatToString(float f)
        {
            return f.ToString("0." + new string('#', 339));
        }

        public static byte[] ReadBytes(FileStream stream, int count)
        {
            byte[] bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }

        // always big endian
        public static byte[] GetBytes(int value)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);
            return result;
        }

        public static int ToInt32(params byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] swap = bytes.Take(4).Reverse().ToArray();
                return BitConverter.ToInt32(swap, 0);
            }
            else
                return BitConverter.ToInt32(bytes, 0);
        }

        public static byte[] ToByteArray(params short[] shorts)
        {
            byte[] result = new byte[shorts.Length * sizeof(short)];
            Buffer.BlockCopy(shorts, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] ToByteArray(params int[] ints)
        {
            byte[] result = new byte[ints.Length * sizeof(int)];
            Buffer.BlockCopy(ints, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] ToByteArray(params long[] longs)
        {
            byte[] result = new byte[longs.Length * sizeof(long)];
            Buffer.BlockCopy(longs, 0, result, 0, result.Length);
            return result;
        }

        public static short[] ToShortArray(params byte[] bytes)
        {
            var size = bytes.Length / sizeof(short);
            var shorts = new short[size];
            for (int index = 0; index < size; index++)
            {
                shorts[index] = BitConverter.ToInt16(bytes, index * sizeof(short));
            }
            return shorts;
        }

        public static int[] ToIntArray(params byte[] bytes)
        {
            var size = bytes.Length / sizeof(int);
            var ints = new int[size];
            for (int index = 0; index < size; index++)
            {
                ints[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }
            return ints;
        }

        public static long[] ToLongArray(params byte[] bytes)
        {
            var size = bytes.Length / sizeof(long);
            var longs = new long[size];
            for (int index = 0; index < size; index++)
            {
                longs[index] = BitConverter.ToInt64(bytes, index * sizeof(long));
            }
            return longs;
        }

        public static bool ExactlyOne<T>(IEnumerable<T> items)
        {
            return items.Any() && !items.Skip(1).Any();
        }

        public static DataObject Merge(DataObject obj1, DataObject obj2)
        {
            var result = new DataObject();

            // naive transfer
            // gets (hopefully) everything, but replaces rather than combining
            var formats1 = obj1.GetFormats();
            var formats2 = obj2.GetFormats();
            foreach (var format in formats1)
            {
                var data = obj1.GetData(format);
                result.SetData(format, data);
            }
            foreach (var format in formats2)
            {
                var data = obj2.GetData(format);
                result.SetData(format, data);
            }

            // specific mergeable transfers
            var text1 = obj1.GetText();
            var text2 = obj2.GetText();
            var text_result = Merge(text1, text2);
            if (text_result != null)
                result.SetText(text_result);

            var file1 = obj1.GetFileDropList();
            var file2 = obj2.GetFileDropList();
            var file_result = Merge(file1, file2);
            if (file_result != null)
                result.SetFileDropList(file_result);

            return result;
        }

        private static string Merge(string str1, string str2)
        {
            if (str1 == null)
                return str2;
            if (str2 == null)
                return str1;
            return str1 + "\n" + str2;
        }

        private static StringCollection Merge(StringCollection col1, StringCollection col2)
        {
            if (col1 == null)
                return col2;
            if (col2 == null)
                return col1;
            var result = new StringCollection();
            result.AddRange(col1.Cast<string>().ToArray());
            result.AddRange(col2.Cast<string>().ToArray());
            return result;
        }

        public static string ExceptionMessage(Exception exception)
        {
            string message = exception.Message;
            if (exception is AggregateException aggregate)
                message += "\n" + String.Join("\n", aggregate.InnerExceptions.Select(ExceptionMessage));
            if (exception.InnerException != null)
                message += "\n" + ExceptionMessage(exception.InnerException);
            return message;
        }
    }
}
