using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class EditorAction
    {
        private readonly List<Editor> Editors = new();
        public IEnumerable<Node> Nodes;
        public UndoHistory UndoHistory;

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
                    var command = editor.Edit(Nodes);
                    if (command is not null)
                        UndoHistory.PerformAction(command);
                    // still break even if command is null
                    // because otherwise, if the user cancels an action, it will try the next fitting action
                    break;
                }
            }
        }
    }
}
