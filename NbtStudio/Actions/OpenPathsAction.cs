using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public class OpenPathsAction
    {
        public UnsavedWarningHandler UnsavedWarningCheck;
        public TreeGetter TreeGetter;
        public PathsGetter PathsGetter;
        public PathsErrorHandler ErrorHandler;

        public IEnumerable<IHavePath> Open() => OpenOrImportPaths(true);
        public IEnumerable<IHavePath> Import() => OpenOrImportPaths(false);

        private IEnumerable<IHavePath> OpenOrImportPaths(bool open)
        {
            if (open && !UnsavedWarningCheck())
                return null;
            IHavePath[] loadable;
            var files = PathsGetter();
            // can be null when the user cancels a file browse, for example
            if (files == null)
                return null;
            var bad = files.FailedPaths.ToArray();
            loadable = files.SucceededValues.ToArray();
            if (bad.Any())
                ErrorHandler(files);
            if (loadable.Any())
            {
                if (open)
                    TreeGetter().Replace(loadable);
                else
                    TreeGetter().Import(loadable);
                return loadable;
            }
            return null;
        }
    }
}
