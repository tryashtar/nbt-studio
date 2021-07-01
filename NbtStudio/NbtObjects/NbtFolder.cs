using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class NbtFolder : IHavePath, IRefreshable, IDisposable
    {
        public string Path { get; private set; }
        public readonly bool Recursive;
        public bool HasScanned { get; private set; } = false;
        public event EventHandler ContentsChanged;
        public event EventHandler<IEnumerable<(string path, Failable<IFile> file)>> FilesFailed;
        public IReadOnlyCollection<NbtFolder> Subfolders => SubfolderDict.Values;
        public IEnumerable<NbtFolder> GetAllSubfolders() => Subfolders.Concat(Subfolders.SelectMany(x => x.GetAllSubfolders()));
        public IReadOnlyCollection<IFile> Files => FileDict.Values;
        public IEnumerable<IFile> GetAllFiles() => Files.Concat(Subfolders.SelectMany(x => x.GetAllFiles()));
        public IEnumerable<(string path, Failable<IFile> file)> FailedFiles => FailedFileDict.Select(x => (x.Key, x.Value));
        private readonly Dictionary<string, NbtFolder> SubfolderDict = new();
        private readonly Dictionary<string, IFile> FileDict = new();
        private readonly Dictionary<string, Failable<IFile>> FailedFileDict = new();
        public bool CanRefresh => true;
        public void Refresh() => Scan();

        public NbtFolder(string path, bool recursive)
        {
            Path = path;
            Recursive = recursive;
        }

        public void Scan()
        {
            HasScanned = true;
            string[] files;
            if (Directory.Exists(Path))
                files = Directory.GetFiles(Path);
            else
                files = new string[0];
            var newly_failed = new List<(string path, Failable<IFile> file)>();
            foreach (var path in files)
            {
                if (!FileDict.ContainsKey(path))
                {
                    var file = OpenFile(path);
                    if (file.Failed)
                        newly_failed.Add((path, file));
                    else
                        FileDict.Add(path, file.Result);
                }
            }
            foreach (var key in FileDict.Keys.ToList())
            {
                if (!files.Contains(key))
                    FileDict.Remove(key);
            }
            if (Recursive)
            {
                string[] folders;
                if (Directory.Exists(Path))
                    folders = Directory.GetDirectories(Path, "*", SearchOption.TopDirectoryOnly);
                else
                    folders = new string[0];
                foreach (var path in folders)
                {
                    if (!SubfolderDict.ContainsKey(path))
                        SubfolderDict.Add(path, new NbtFolder(path, true));
                }
                foreach (var key in SubfolderDict.Keys.ToList())
                {
                    if (!folders.Contains(key))
                        SubfolderDict.Remove(key);
                }
            }
            ContentsChanged?.Invoke(this, EventArgs.Empty);
            if (newly_failed.Any())
            {
                FilesFailed?.Invoke(this, newly_failed);
                foreach (var item in newly_failed)
                {
                    FailedFileDict[item.path] = item.file;
                }
            }
        }

        public static Failable<IFile> OpenFile(string path)
        {
            var attempt1 = NbtFile.TryCreate(path).Cast<IFile>();
            if (!attempt1.Failed)
                return attempt1;
            var attempt2 = RegionFile.TryCreate(path).Cast<IFile>();
            if (!attempt2.Failed)
                return attempt2;
            return Failable<IFile>.Aggregate(attempt1, attempt2);
        }

        public static Failable<IHavePath> OpenFileOrFolder(string path)
        {
            if (Directory.Exists(path))
                return new Failable<IHavePath>(() => new NbtFolder(path, true), "Load as folder");
            return OpenFile(path).Cast<IHavePath>();
        }

        public void Move(string path)
        {
            Directory.Move(Path, path);
            Path = path;
        }

        public void Dispose()
        {

        }
    }
}
