using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class EditorAction
    {
        private readonly List<Editor> Editors = new();
        public IEnumerable<Node> Nodes;
        public void AddEditor(Editor editor)
        {
            Editors.Add(editor);
        }

        public void Edit()
        {
            foreach (var editor in Editors)
            {
                if (editor.CanEdit(Nodes))
                {
                    editor.Edit(Nodes);
                    break;
                }
            }
        }
    }
}
