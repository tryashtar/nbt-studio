using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public class NbtFolder
    {
        public readonly string Path;
        public readonly bool Recursive;
        public bool HasScanned { get; private set; } = false;
        public IReadOnlyCollection<NbtFolder> Subfolders => _Subfolders.AsReadOnly();
        public IReadOnlyCollection<ISaveable> Files => _Files.AsReadOnly();
        private readonly List<NbtFolder> _Subfolders = new List<NbtFolder>();
        private readonly List<ISaveable> _Files = new List<ISaveable>();

        public NbtFolder(string path, bool recursive)
        {
            Path = path;
            Recursive = recursive;
        }

        public void Scan()
        {
            HasScanned = true;
            _Files.Clear();
            _Subfolders.Clear();
            foreach (var item in Directory.GetFiles(Path))
            {
                var file = OpenFile(item);
                if (file != null)
                    _Files.Add(file);
            }
            if (Recursive)
            {
                foreach (var item in Directory.GetDirectories(Path, "*", SearchOption.AllDirectories))
                {
                    _Subfolders.Add(new NbtFolder(item, true));
                }
            }
        }

        public static ISaveable OpenFile(string path)
        {
            return (ISaveable)NbtFile.TryCreate(path) ??
                RegionFile.TryCreate(path);
        }

        public static object OpenFileOrFolder(string path)
        {
            if (Directory.Exists(path))
                return new NbtFolder(path, true);
            return OpenFile(path);
        }
    }
}
