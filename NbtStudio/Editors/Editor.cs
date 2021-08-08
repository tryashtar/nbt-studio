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
    public abstract class Editor
    {
        public abstract bool CanEdit(IEnumerable<Node> nodes);
        public abstract void Edit(IEnumerable<Node> nodes);
    }
}
