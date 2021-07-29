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
    public class ActionContext
    {
        public IEnumerable<Node> SelectedNodes;
        public IEnumerable<IHavePath> ImportingFiles;
        public NbtTag AddingTag;
        public ErrorHandler ErrorHandler;
        public UnsavedWarningHandler UnsavedWarningHandler;
        public DeleteFileWarningHandler DeleteFileWarningHandler;
        public IEnumerable<NbtTag> SelectedNbt()
        {
            return SelectedNodes.Select(x => x.GetNbtTagLens()).Where(x => x != null).Select(x => x.Item);
        }
    }
}
