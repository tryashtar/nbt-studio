using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Nbt;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    // settings an NBT file can be saved with, and the implementation for saving as such
    public class ExportSettings
    {
        public readonly bool Snbt;
        public readonly SnbtOptions SnbtOptions;
        public readonly NbtCompression Compression;
        public readonly bool BigEndian;
        public readonly bool BedrockHeader;

        // keep this private and only expose the static constructors below so callers don't have to include irrelevant information
        private ExportSettings(bool snbt, SnbtOptions options, NbtCompression compression, bool big_endian, bool bedrock_header)
        {
            Snbt = snbt;
            SnbtOptions = options;
            Compression = compression;
            BigEndian = big_endian;
            BedrockHeader = bedrock_header;
        }

        public static ExportSettings AsSnbt(SnbtOptions options)
        {
            return new ExportSettings(true, options, NbtCompression.None, false, false);
        }

        public static ExportSettings AsNbt(NbtCompression compression, bool big_endian, bool bedrock_header)
        {
            return new ExportSettings(false, null, compression, big_endian, bedrock_header);
        }

        public void Export(string path, NbtTag root)
        {
            if (Snbt)
                File.WriteAllText(path, root.ToSnbt(SnbtOptions));
            else
            {
                var file = new fNbt.NbtFile();
                file.BigEndian = BigEndian;
                file.RootTag = root;
                using var writer = File.Create(path);
                if (BedrockHeader)
                    writer.Seek(8, SeekOrigin.Begin);
                long size = file.SaveToStream(writer, Compression);
                if (BedrockHeader)
                {
                    // bedrock level.dat files start with a header containing a magic number and then the little-endian size of the data
                    writer.Seek(0, SeekOrigin.Begin);
                    writer.Write(new byte[] { 8, 0, 0, 0 }, 0, 4);
                    writer.Write(DataUtils.GetBytes((int)size, little_endian: !BigEndian), 0, 4);
                }
            }
        }
    }
}
