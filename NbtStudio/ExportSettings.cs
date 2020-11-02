using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class ExportSettings
    {
        public readonly bool Snbt;
        public readonly bool Minified;
        public readonly NbtCompression Compression;
        public readonly bool BigEndian;
        public readonly byte[] Header;

        private ExportSettings(bool snbt, bool minified, NbtCompression compression, bool big_endian, byte[] header)
        {
            Snbt = snbt;
            Minified = minified;
            Compression = compression;
            BigEndian = big_endian;
            Header = header;
        }

        public static ExportSettings AsSnbt(bool minified)
        {
            return new ExportSettings(true, minified, NbtCompression.None, false, new byte[0]);
        }

        public static ExportSettings AsNbt(NbtCompression compression, bool big_endian, byte[] header)
        {
            return new ExportSettings(false, false, compression, big_endian, header);
        }

        public void Export(string path, NbtCompound root)
        {
            if (Snbt)
                File.WriteAllText(path, root.ToSnbt(expanded: !Minified));
            else
            {
                var file = new fNbt.NbtFile();
                file.BigEndian = BigEndian;
                file.RootTag = root;
                using (var writer = File.OpenWrite(path))
                {
                    writer.Write(Header, 0, Header.Length);
                    file.SaveToStream(writer, Compression);
                }
            }
        }
    }
}
