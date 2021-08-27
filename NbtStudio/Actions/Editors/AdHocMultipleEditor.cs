using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;
using NbtStudio.UI;

namespace NbtStudio
{
    public class AdHocMultipleEditor<T> : TypedEditor<T> where T : Node
    {
        public delegate bool EditCheck(IEnumerable<T> nodes);
        public delegate ICommand EditDo(IEnumerable<T> nodes);

        private readonly EditCheck Check;
        private readonly EditDo Do;

        public AdHocMultipleEditor(EditCheck check, EditDo edit)
        {
            Check = check;
            Do = edit;
        }

        public override bool CanEdit(IEnumerable<T> nodes)
        {
            return Check(nodes);
        }

        public override ICommand Edit(IEnumerable<T> nodes)
        {
            return Do(nodes);
        }
    }
}
