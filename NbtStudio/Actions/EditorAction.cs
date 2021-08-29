using System;
using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class EditorAction
    {
        private readonly List<Editor> Editors = new();
        public Func<IEnumerable<Node>> NodeSource;
        public Func<UndoHistory> UndoHistorySource;

        public void AddEditor(Editor editor)
        {
            Editors.Add(editor);
        }

        public bool CanEdit()
        {
            var nodes = NodeSource();
            return Editors.Any(x => x.CanEdit(nodes));
        }

        public void Edit()
        {
            var nodes = NodeSource();
            var history = UndoHistorySource();
            foreach (var editor in Editors)
            {
                if (editor.CanEdit(nodes))
                {
                    var command = editor.Edit(nodes);
                    if (command is not null)
                        history.PerformAction(command);
                    // still break even if command is null
                    // because otherwise, if the user cancels an action, it will try the next fitting action
                    break;
                }
            }
        }
    }
}
