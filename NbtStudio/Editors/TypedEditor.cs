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
            var item = Extract(node);
            return item is not null && CanEdit(item);
        }
        protected sealed override ICommand FilteredEdit(IEnumerable<Node> nodes)
        {
            return Edit(nodes.Select(Extract));
        }
        protected sealed override bool FilteredCanEdit(IEnumerable<Node> nodes)
        {
            return CanEdit(nodes.Select(Extract));
        }

        protected abstract T Extract(Node node);
        protected abstract bool CanEdit(T item);
        // all items being passed here passed CanEdit(), so no need to check again
        // but you can for example implement an editor that requires exactly two items
        protected abstract ICommand Edit(IEnumerable<T> items);
        protected virtual bool CanEdit(IEnumerable<T> items)
        {
            return items.Any();
        }
    }
}
