using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
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
    }
}
