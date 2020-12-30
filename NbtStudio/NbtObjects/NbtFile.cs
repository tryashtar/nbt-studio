using fNbt;
using NbtStudio.SNBT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NbtStudio
{
    // represents a loadable and saveable NBT file
    // uses fNbt.NbtFile to do the work reading/writing binary data to disk, but can also read/write SNBT without using one
    public class NbtFile : IFile
    {
        public string Path { get; private set; }
        public event EventHandler OnSaved;
        public NbtTag RootTag { get; private set; }
        public T GetRootTag<T>() where T : NbtTag => RootTag as T;
        public ExportSettings ExportSettings { get; private set; }
        public bool CanSave => Path != null && ExportSettings != null;
        public bool CanRefresh => CanSave;
        public bool HasUnsavedChanges { get; private set; } = false;

        private NbtFile(string path, NbtTag root, ExportSettings settings)
        {
            Path = path;
            SetRoot(root);
            ExportSettings = settings;
        }

        public NbtFile() : this(new NbtCompound(""))
        { }

        public NbtFile(NbtTag root)
        {
            if (root.Name == null)
                root.Name = "";
            SetRoot(root);
            Path = null;
            ExportSettings = null;
            HasUnsavedChanges = true;
        }

        private void SetRoot(NbtTag root)
        {
            RootTag = root;
            RootTag.Changed += (s, e) => HasUnsavedChanges = true;
        }

        private static bool LooksSuspicious(string name)
        {
            if (name == null)
                return false;
            foreach (var ch in name)
            {
                if (Char.IsControl(ch))
                    return true;
            }
            return false;
        }

        private static bool LooksSuspicious(NbtTag tag)
        {
            if (LooksSuspicious(tag.Name))
                return true;
            if (tag is NbtContainerTag container && container.Any(x => LooksSuspicious(x.Name)))
                return true;
            return false;
        }

        public static NbtFile TryCreate(string path)
        {
            // try loading the file four different ways
            // if loading fails or looks suspicious, try a different way
            // if all loads are suspicious, choose the first that didn't fail

            // SNBT
            var attempt1 = TryCreateFromSnbt(path);
            if (attempt1 != null && !LooksSuspicious(attempt1.RootTag))
                return attempt1;
            // java files
            var attempt2 = TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: true);
            if (attempt2 != null && !LooksSuspicious(attempt2.RootTag))
                return attempt2;
            // bedrock files
            var attempt3 = TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: false);
            if (attempt3 != null && !LooksSuspicious(attempt3.RootTag))
                return attempt3;
            // bedrock level.dat files
            var attempt4 = TryCreateFromNbt(path, NbtCompression.AutoDetect, big_endian: false, bedrock_header: true);
            if (attempt4 != null && !LooksSuspicious(attempt4.RootTag))
                return attempt4;
            return attempt1 ?? attempt2 ?? attempt3 ?? attempt4;
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
                    return new NbtFile(path, file.RootTag, ExportSettings.AsSnbt(!text.Contains("\n"), System.IO.Path.GetExtension(path) == ".json"));
                }
            }
            catch
            {
                return null;
            }
        }

        public static NbtFile TryCreateFromNbt(string path, NbtCompression compression, bool big_endian = true, bool bedrock_header = false)
        {
            try
            {
                var file = new fNbt.NbtFile();
                file.BigEndian = big_endian;
                using (var reader = File.OpenRead(path))
                {
                    if (bedrock_header)
                    {
                        var header = new byte[8];
                        reader.Read(header, 0, header.Length);
                    }
                    file.LoadFromStream(reader, compression);
                }
                if (file.RootTag == null)
                    return null;

                return new NbtFile(path, file.RootTag, ExportSettings.AsNbt(file.FileCompression, big_endian, bedrock_header));
            }
            catch
            {
                return null;
            }
        }

        public static NbtFile TryCreateFromExportSettings(string path, ExportSettings settings)
        {
            if (settings.Snbt)
                return TryCreateFromSnbt(path);
            else
                return TryCreateFromNbt(path, settings.Compression, settings.BigEndian, settings.BedrockHeader);
        }

        public void Save()
        {
            ExportSettings.Export(Path, RootTag);
            HasUnsavedChanges = false;
            OnSaved?.Invoke(this, EventArgs.Empty);
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

        public void Refresh()
        {
            var current = TryCreateFromExportSettings(Path, ExportSettings).RootTag as NbtContainerTag;
            var self = RootTag as NbtContainerTag;
            var tags = current.ToList();
            self.Clear();
            current.Clear();
            self.AddRange(tags);
            HasUnsavedChanges = false;
        }

        public void Move(string path)
        {
            if (Path != null)
            {
                File.Move(Path, path);
                Path = path;
            }
        }
    }
}
