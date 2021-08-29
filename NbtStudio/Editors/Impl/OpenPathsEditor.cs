using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class OpenPathsEditor : ContextFreeEditor
    {
        public UnsavedWarningHandler UnsavedWarningCheck;
        public TreeGetter TreeGetter;
        public PathsGetter PathsGetter;
        public PathsErrorHandler ErrorHandler;
        public OpenMode Mode;

        public override bool CanEdit()
        {
            return true;
        }

        public override ICommand Edit()
        {
            if (Mode == OpenMode.Open && !UnsavedWarningCheck())
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
                Properties.Settings.Default.RecentFiles.AddMany(files.SucceededPaths);
                if (Mode == OpenMode.Open)
                    TreeGetter().Replace(loadable);
                else
                    TreeGetter().Import(loadable);
            }
            return null;
        }
    }
}
