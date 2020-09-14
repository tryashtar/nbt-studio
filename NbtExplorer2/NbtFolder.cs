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
        public readonly string FileName;
        public IReadOnlyCollection<NbtFolder> Subfolders => _Subfolders.AsReadOnly();
        public IReadOnlyCollection<NbtFile> Files => _Files.AsReadOnly();
        private readonly List<NbtFolder> _Subfolders = new List<NbtFolder>();
        private readonly List<NbtFile> _Files = new List<NbtFile>();

        public NbtFolder(string path, bool recursive)
        {
            FileName = path;
            foreach (var item in Directory.GetFiles(path))
            {
                _Files.Add(new NbtFile(item));
            }
            if (recursive)
            {
                foreach (var item in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                {
                    _Subfolders.Add(new NbtFolder(item, true));
                }
            }
        }
    }
}
