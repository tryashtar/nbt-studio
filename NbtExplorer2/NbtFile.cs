using fNbt;
using NbtExplorer2.SNBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    // represents a loadable and saveable NBT file
    // uses fNbt.NbtFile to do the work reading/writing binary data to disk, but can also read/write SNBT without using one
    public class NbtFile : INbtFile
    {
        public string Path { get; private set; }
        public NbtCompound RootTag { get; private set; }
        public ExportSettings ExportSettings { get; private set; }
        public bool CanSave => Path != null && ExportSettings != null;

        private NbtFile(string path, NbtCompound root, ExportSettings settings)
        {
            Path = path;
            RootTag = root;
            ExportSettings = settings;
        }

        public NbtFile() : this(new NbtCompound(""))
        { }

        public NbtFile(NbtCompound root)
        {
            if (root.Name == null)
                root.Name = "";
            RootTag = root;
            Path = null;
            ExportSettings = null;
        }

        public static NbtFile TryCreate(string path)
        {
            return TryCreateFromSnbt(path)
                   ?? TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: true) // java files
                   ?? TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: false) // bedrock files
                   ?? TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: false, header_size: 8); // bedrock level.dat files
        }

        public static NbtFile TryCreateFromSnbt(string path)
        {
            try
            {
                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    char[] firstchar = new char[1];
                    reader.ReadBlock(firstchar, 0, 1);
                    if (firstchar[0] != '{') // optimization to not load in huge files
                        return null;
                    var text = firstchar[0] + reader.ReadToEnd();
                    var tag = SnbtParser.Parse(text, named: false);
                    if (!(tag is NbtCompound compound))
                        return null;
                    compound.Name = "";
                    var file = new fNbt.NbtFile(compound);
                    return new NbtFile(path, file.RootTag, ExportSettings.AsSnbt(!text.Contains("\n")));
                }
            }
            catch
            {
                return null;
            }
        }

        public static NbtFile TryCreateFromNbt(string path, NbtCompression compression, bool big_endian = true, int header_size = 0)
        {
            try
            {
                var file = new fNbt.NbtFile();
                file.BigEndian = big_endian;
                var header = new byte[header_size];
                using (var reader = File.OpenRead(path))
                {
                    reader.Read(header, 0, header_size);
                    file.LoadFromStream(reader, compression);
                }
                if (file.RootTag == null)
                    return null;

                return new NbtFile(path, file.RootTag, ExportSettings.AsNbt(file.FileCompression, big_endian, header));
            }
            catch
            {
                return null;
            }
        }

        public void Save()
        {
            ExportSettings.Export(Path, RootTag);
        }

        public void SaveAs(string path)
        {
            Path = path;
            Save();
        }

        public void SaveAs(string path, ExportSettings settings)
        {
            Path = path;
            ExportSettings = settings;
            Save();
        }
    }

    public interface ISaveable
    {
        string Path { get; }
        bool CanSave { get; }
        void Save();
        void SaveAs(string path);
    }

    public interface INbtFile : ISaveable
    {
        ExportSettings ExportSettings { get; }
        void SaveAs(string path, ExportSettings settings);
    }
}
