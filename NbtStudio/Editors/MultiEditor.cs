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
    public class MultiEditor : Editor
    {
        private readonly List<Editor> Editors = new();

        public void Add(Editor editor)
        {
            Editors.Add(editor);
        }

        public override bool Filter(Node node)
        {
            return true;
        }

        protected override bool FilteredCanEdit(IEnumerable<Node> nodes)
        {
            return Editors.Any(x => x.CanEdit(nodes));
        }

        protected override ICommand FilteredEdit(IEnumerable<Node> nodes)
        {
            foreach (var editor in Editors)
            {
                if (editor.CanEdit(nodes))
                {
                    return editor.Edit(nodes);
                    // still exit even if command is null
                    // because otherwise, if the user cancels an action, it will try the next fitting action
                }
            }
            return null;
        }
    }
}
