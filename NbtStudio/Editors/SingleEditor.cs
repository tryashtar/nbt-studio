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
    public abstract class SingleEditor<T> : TypedEditor<T>
    {
        protected sealed override bool CanEdit(IEnumerable<T> items)
        {
            return ListUtils.ExactlyOne(items);
        }

        protected override ICommand Edit(IEnumerable<T> items)
        {
            return Edit(items.Single());
        }

        protected abstract ICommand Edit(T item);
    }
}
