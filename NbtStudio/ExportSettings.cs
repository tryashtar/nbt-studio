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
        public readonly bool Json;
        public readonly NbtCompression Compression;
        public readonly bool BigEndian;
        public readonly bool BedrockHeader;

        private ExportSettings(bool snbt, bool minified, bool json, NbtCompression compression, bool big_endian, bool bedrock_header)
        {
            Snbt = snbt;
            Minified = minified;
            Json = json;
            Compression = compression;
            BigEndian = big_endian;
            BedrockHeader = bedrock_header;
        }

        public static ExportSettings AsSnbt(bool minified, bool json)
        {
            return new ExportSettings(true, minified, json, NbtCompression.None, false, false);
        }

        public static ExportSettings AsNbt(NbtCompression compression, bool big_endian, bool bedrock_header)
        {
            return new ExportSettings(false, false, false, compression, big_endian, bedrock_header);
        }

        private SnbtOptions CreateOptions()
        {
            var options = Json ? SnbtOptions.JsonLike : SnbtOptions.Default;
            if (!Minified)
                options = options.Expanded();
            return options;
        }

        public void Export(string path, NbtCompound root)
        {
            if (Snbt)
                File.WriteAllText(path, root.ToSnbt(CreateOptions()));
            else
            {
                var file = new fNbt.NbtFile();
                file.BigEndian = BigEndian;
                file.RootTag = root;
                using (var writer = File.OpenWrite(path))
                {
                    if (BedrockHeader)
                        writer.Seek(8, SeekOrigin.Begin);
                    long size = file.SaveToStream(writer, Compression);
                    if (BedrockHeader)
                    {
                        writer.Seek(0, SeekOrigin.Begin);
                        writer.Write(new byte[] { 8, 0, 0, 0 }, 0, 4);
                        writer.Write(Util.GetBytes((int)size, little_endian: !BigEndian), 0, 4);
                    }
                }
            }
        }
    }
}
