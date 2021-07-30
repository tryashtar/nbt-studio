using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
{
    public delegate void ErrorHandler(IFailable failure);
    public delegate void AdvancedFileErrorHandler(IEnumerable<(string path, IFailable<IHavePath> file)> failures);
    public delegate bool UnsavedWarningHandler();
    public delegate bool DeleteFileWarningHandler(IEnumerable<IHavePath> files);
    public delegate void TreeSetter(NbtTreeModel new_tree);
    public delegate NbtTreeModel TreeGetter();
    public delegate IFailable<NbtTag> TagGetter();
    public delegate IEnumerable<IFailable<IHavePath>> FilesGetter();
    public delegate IEnumerable<(string path, IFailable<IHavePath> file)> AdvancedFilesGetter();
    public class ActionContext
    {
        public IEnumerable<Node> SelectedNodes;
        public TagGetter TagSource;
        public ErrorHandler TagErrorHandler;
        public UnsavedWarningHandler UnsavedWarningCheck;
        public DeleteFileWarningHandler DeleteFileWarning;
        public TreeSetter TreeSetter;
        public TreeGetter TreeGetter;
        public FilesGetter FilesGetter;
        public AdvancedFilesGetter AdvancedFilesGetter;
        public ErrorHandler FileErrorHandler;
        public AdvancedFileErrorHandler AdvancedFileErrorHandler;
        public IEnumerable<NbtTag> SelectedNbt()
        {
            return SelectedNodes.Select(x => x.GetNbtTagLens()).Where(x => x != null).Select(x => x.Item);
        }
        public static FilesGetter SingleFile(IHavePath file)
        {
            return () => new IFailable<IHavePath>[] { FailableFactory.Success(file, null) };
        }
    }
}
