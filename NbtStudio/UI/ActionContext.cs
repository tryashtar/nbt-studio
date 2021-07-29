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
    public delegate void ErrorHandler(Exception exception);
    public delegate bool UnsavedWarningHandler();
    public delegate bool DeleteFileWarningHandler(IEnumerable<IHavePath> files);
    public delegate void TreeSetter(NbtTreeModel new_tree);
    public delegate NbtTreeModel TreeGetter();
    public delegate NbtTag TagGetter();
    public delegate IEnumerable<IFile> FilesGetter();
    public class ActionContext
    {
        public IEnumerable<Node> SelectedNodes;
        public TagGetter TagSource;
        public ErrorHandler ErrorHandler;
        public UnsavedWarningHandler UnsavedWarningCheck;
        public DeleteFileWarningHandler DeleteFileWarning;
        public TreeSetter TreeSetter;
        public TreeGetter TreeGetter;
        public FilesGetter FilesGetter;
        public IEnumerable<NbtTag> SelectedNbt()
        {
            return SelectedNodes.Select(x => x.GetNbtTagLens()).Where(x => x != null).Select(x => x.Item);
        }
    }
}
