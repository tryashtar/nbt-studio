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
    }
}
