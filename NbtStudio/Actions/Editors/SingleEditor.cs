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
    public abstract class SingleEditor<T> : TypedEditor<T> where T : Node
    {
        public abstract bool CanEdit(T node);
        public abstract ICommand Edit(T node);
        public sealed override bool CanEdit(IEnumerable<T> nodes) => ListUtils.ExactlyOne(nodes) && CanEdit(nodes.Single());
        public sealed override ICommand Edit(IEnumerable<T> nodes) => Edit(nodes.Single());
    }
}
