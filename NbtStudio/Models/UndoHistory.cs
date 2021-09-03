using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class UndoHistory
    {
        private readonly Stack<ICommand> UndoStack = new();
        private readonly Stack<ICommand> RedoStack = new();
        public event EventHandler Changed;

        public void PerformAction(ICommand command)
        {
            command.Execute();
            RedoStack.Clear();
            UndoStack.Push(command);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Undo(int count = 1)
        {
            for (int i = 0; i < count && UndoStack.Any(); i++)
            {
                var action = UndoStack.Pop();
                RedoStack.Push(action);
                action.Undo();
                DebugLog.WriteLine($"Performed undo of action \"{action.Description}\". Undo stack has {UndoStack.Count} items. Redo stack has {RedoStack.Count} items");
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Redo(int count = 1)
        {
            for (int i = 0; i < count && RedoStack.Any(); i++)
            {
                var action = RedoStack.Pop();
                UndoStack.Push(action);
                action.Execute();
                DebugLog.WriteLine($"Performed redo of action \"{action.Description}\". Redo stack has {RedoStack.Count} items. Undo stack has {UndoStack.Count} items");
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            UndoStack.Clear();
            RedoStack.Clear();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool CanUndo => UndoStack.Any();
        public bool CanRedo => RedoStack.Any();

        public List<KeyValuePair<int, string>> GetUndoHistory()
        {
            return UndoStack.Select((v, i) => new KeyValuePair<int, string>(i + 1, v.Description)).ToList();
        }
        public List<KeyValuePair<int, string>> GetRedoHistory()
        {
            return RedoStack.Select((v, i) => new KeyValuePair<int, string>(i + 1, v.Description)).ToList();
        }
    }
}
