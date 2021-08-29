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
    public class MultiContextFreeEditor : ContextFreeEditor
    {
        private readonly List<ContextFreeEditor> Editors = new();

        public void Add(ContextFreeEditor editor)
        {
            Editors.Add(editor);
        }

        public override bool CanEdit()
        {
            return Editors.Any(x => x.CanEdit());
        }

        public override ICommand Edit()
        {
            foreach (var editor in Editors)
            {
                if (editor.CanEdit())
                {
                    return editor.Edit();
                    // still exit even if command is null
                    // because otherwise, if the user cancels an action, it will try the next fitting action
                }
            }
            return null;
        }
    }
}
