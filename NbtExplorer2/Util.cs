using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public static class Util
    {
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
    }
}
