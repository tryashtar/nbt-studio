using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public class ExportSettings
    {
        public readonly string Path;
        public readonly bool Snbt;
        public readonly bool Minified;
        public readonly NbtCompression Compression;
        public readonly bool BigEndian;

        private ExportSettings(string path, bool snbt, bool minified, NbtCompression compression, bool big_endian)
        {
            Path = path;
            Snbt = snbt;
            Minified = minified;
            Compression = compression;
            BigEndian = big_endian;
        }

        public static ExportSettings AsSnbt(string path, bool minified)
        {
            return new ExportSettings(path, true, minified, NbtCompression.None, false);
        }

        public static ExportSettings AsNbt(string path, NbtCompression compression, bool little_endian)
        {
            return new ExportSettings(path, false, false, compression, little_endian);
        }
    }
}
