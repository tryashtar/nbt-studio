using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class OpenFileAction
    {
        public UnsavedWarningHandler UnsavedWarningCheck;
        public TreeGetter TreeGetter;
        public FilesGetter FilesGetter;
        public ErrorHandler ErrorHandler;

        public OpenFileAction() { }

        public OpenFileAction(IHavePath literal)
        {
            FilesGetter = () => new[] { FailableFactory.Success(literal, null) };
        }

        public IEnumerable<IHavePath> Open() => OpenOrImportFiles(true);
        public IEnumerable<IHavePath> Import() => OpenOrImportFiles(false);

        private IEnumerable<IHavePath> OpenOrImportFiles(bool open)
        {
            if (open && !UnsavedWarningCheck())
                return null;
            IHavePath[] loadable;
            var files = FilesGetter();
            // can be null when the user cancels a file browse, for example
            if (files == null)
                return null;
            var bad = files.Where(x => x.Failed).ToArray();
            loadable = files.Where(x => !x.Failed).Select(x => x.Result).ToArray();
            if (bad.Any())
                ErrorHandler(FailableFactory.Aggregate(bad));
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
