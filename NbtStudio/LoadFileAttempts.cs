using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using NbtStudio.UI;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class LoadFileAttempts<T>
    {
        private readonly List<(string path, IFailable<T> failure)> Attempts = new();
        public IEnumerable<string> FailedPaths => Attempts.Where(x => x.failure.Failed).Select(x => x.path);
        public IEnumerable<string> SucceededPaths => Attempts.Where(x => !x.failure.Failed).Select(x => x.path);
        public IEnumerable<T> SucceededValues => Attempts.Where(x => !x.failure.Failed).Select(x => x.failure.Result);
        public IFailable Failable => FailableFactory.Aggregate(Attempts.Where(x => x.failure.Failed).Select(x => x.failure).ToArray());

        public delegate IFailable<T> Loader(string path);
        public delegate T RawLoader(string path);
        public void AddAttempt(string path, Loader loader)
        {
            var result = loader(path);
            Attempts.Add((path, result));
        }
        public void AddAttempt(string path, RawLoader loader)
        {
            var result = new Failable<T>(() => loader(path), null);
            Attempts.Add((path, result));
        }
        public void AddMany(IEnumerable<string> paths, Loader loader)
        {
            foreach (var item in paths)
            {
                AddAttempt(item, loader);
            }
        }

        public bool AnyFailed() => FailedPaths.Any();
    }

    public class LoadFileAttempts
    {
        private readonly List<(string path, IFailable failure)> Attempts = new();
        public IEnumerable<string> FailedPaths => Attempts.Where(x => x.failure.Failed).Select(x => x.path);
        public IEnumerable<string> SucceededPaths => Attempts.Where(x => !x.failure.Failed).Select(x => x.path);
        public IFailable Failable => FailableFactory.Aggregate(Attempts.Where(x => x.failure.Failed).Select(x => x.failure).ToArray());

        public delegate IFailable Loader(string path);
        public void AddAttempt(string path, Loader loader)
        {
            var result = loader(path);
            Attempts.Add((path, result));
        }

        public bool AnyFailed() => FailedPaths.Any();
    }
}
