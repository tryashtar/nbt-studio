using fNbt;
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
    public delegate void ErrorHandler(IFailable failure);
    public delegate bool UnsavedWarningHandler();
    public delegate bool DeleteFileWarningHandler(IEnumerable<IHavePath> files);
    public delegate NbtTreeModel TreeGetter();
    public delegate IFailable<NbtTag> TagGetter();
    public delegate IFailable<NbtTag> TagCreator(NbtContainerTag pending_destination);
    public delegate IEnumerable<IFailable<IHavePath>> FilesGetter();
    public delegate LoadFileAttempts<IHavePath> PathsGetter();
    public delegate void PathsErrorHandler(LoadFileAttempts<IHavePath> attempts);

    public abstract class AbstractAction
    {
        public static ErrorHandler Throw()
        {
            return x => throw x.Exception;
        }

        public static PathsErrorHandler ThrowPaths()
        {
            return x => throw x.Failable.Exception;
        }
    }
}
