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
    public abstract class TypedEditor<T> : Editor
    {
        public sealed override bool Filter(Node node)
        {
            return Extract(node) is not null;
        }
        protected sealed override ICommand FilteredEdit(IEnumerable<Node> nodes)
        {
            return Edit(nodes.Select(Extract));
        }
        protected sealed override bool FilteredCanEdit(IEnumerable<Node> nodes)
        {
            return CanEdit(nodes.Select(Extract));
        }

        // return null if the node is invalid for any reason
        protected abstract T Extract(Node node);
        protected abstract ICommand Edit(IEnumerable<T> items);
        protected abstract bool CanEdit(IEnumerable<T> items);
    }
}
