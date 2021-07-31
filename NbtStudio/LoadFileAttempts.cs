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
    // same class basically copy/pasted since I can't figure out how to express it otherwise
    public class LoadFileAttempts
    {
        private readonly List<(string path, IFailable failure)> Attempts = new();
        public IEnumerable<string> FailedPaths => Attempts.Where(x => x.failure.Failed).Select(x => x.path);
        public IEnumerable<string> SucceededPaths => Attempts.Where(x => !x.failure.Failed).Select(x => x.path);
        public IFailable Failable => FailableFactory.Aggregate(Attempts.Where(x=>x.failure.Failed).Select(x => x.failure).ToArray());

        public void AddAttempt(string path, IFailable result)
        {
            Attempts.Add((path, result));
        }

        public bool AnyFailed() => FailedPaths.Any();
    }

    public class LoadFileAttempts<T>
    {
        private readonly List<(string path, IFailable<T> failure)> Attempts = new();
        public IEnumerable<string> FailedPaths => Attempts.Where(x => x.failure.Failed).Select(x => x.path);
        public IEnumerable<string> SucceededPaths => Attempts.Where(x => !x.failure.Failed).Select(x => x.path);
        public IEnumerable<T> SucceededValues => Attempts.Where(x => !x.failure.Failed).Select(x => x.failure.Result);
        public IFailable Failable => FailableFactory.Aggregate(Attempts.Where(x => x.failure.Failed).Select(x => x.failure).ToArray());

        public void AddAttempt(string path, IFailable<T> result)
        {
            Attempts.Add((path, result));
        }

        public bool AnyFailed() => FailedPaths.Any();
    }
}
