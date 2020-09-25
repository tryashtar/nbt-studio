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
        public readonly byte[] Header;

        private ExportSettings(string path, bool snbt, bool minified, NbtCompression compression, bool big_endian, byte[] header)
        {
            Path = path;
            Snbt = snbt;
            Minified = minified;
            Compression = compression;
            BigEndian = big_endian;
            Header = header;
        }

        public static ExportSettings AsSnbt(string path, bool minified)
        {
            return new ExportSettings(path, true, minified, NbtCompression.None, false, new byte[0]);
        }

        public static ExportSettings AsNbt(string path, NbtCompression compression, bool big_endian, byte[] header)
        {
            return new ExportSettings(path, false, false, compression, big_endian, header);
        }
    }
}
