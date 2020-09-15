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
    public class NbtFile
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

        public NbtFile()
        {
            RootTag = new NbtCompound();
            Path = null;
            ExportSettings = null;
        }

        public static NbtFile TryCreate(string path)
        {
            return TryCreateFromSnbt(path)
                   ?? TryCreateFromNbt(path, NbtCompression.AutoDetect)
                   ?? TryCreateFromNbt(path, NbtCompression.AutoDetect, true);
        }

        public static NbtFile TryCreateFromSnbt(string path)
        {
            try
            {
                var text = File.ReadAllText(path);
                var tag = SnbtParser.Parse(text);
                if (!(tag is NbtCompound compound))
                    return null;
                compound.Name = "";
                var file = new fNbt.NbtFile(compound);
                return new NbtFile(path, file.RootTag, ExportSettings.AsSnbt(path, !text.Contains("\n")));
            }
            catch
            {
                return null;
            }
        }

        public static NbtFile TryCreateFromNbt(string path, NbtCompression compression, bool big_endian = true)
        {
            try
            {
                var file = new fNbt.NbtFile();
                file.BigEndian = big_endian;
                file.LoadFromFile(path, compression, x => true);

                if (file.RootTag == null)
                    return null;

                return new NbtFile(path, file.RootTag, ExportSettings.AsNbt(path, file.FileCompression, big_endian));
            }
            catch
            {
                return null;
            }
        }

        public void Save()
        {
            if (ExportSettings.Snbt)
                File.WriteAllText(Path, RootTag.ToSnbt(multiline: !ExportSettings.Minified));
            else
            {
                var file = new fNbt.NbtFile(Path);
                file.BigEndian = !ExportSettings.BigEndian;
                file.SaveToFile(Path, ExportSettings.Compression);
            }
        }

        public void SaveAs(string path, ExportSettings settings)
        {
            Path = path;
            ExportSettings = settings;
            Save();
        }
    }
}
