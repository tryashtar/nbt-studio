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
    public class AdHocSingleEditor<T> : SingleEditor<T> where T : Node
    {
        public delegate bool EditCheck(T node);
        public delegate void EditDo(T node);

        private readonly EditCheck Check;
        private readonly EditDo Do;

        public AdHocSingleEditor(EditCheck check, EditDo edit)
        {
            Check = check;
            Do = edit;
        }

        public override bool CanEdit(T node)
        {
            return Check(node);
        }

        public override void Edit(T node)
        {
            Do(node);
        }
    }
}
