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
    public abstract class TypedEditor<T> : Editor where T : Node
    {
        public abstract bool CanEdit(IEnumerable<T> nodes);
        public abstract ICommand Edit(IEnumerable<T> nodes);
        public override sealed bool CanEdit(IEnumerable<Node> nodes) => nodes.All(x => x is T) && CanEdit(nodes.Cast<T>());
        public override sealed ICommand Edit(IEnumerable<Node> nodes) => Edit(nodes.Cast<T>());
    }
}
