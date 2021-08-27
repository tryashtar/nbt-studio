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
        private int BatchNumber = 0;
        private readonly List<ICommand> UndoBatch = new();

        public void PerformAction(ICommand command)
        {
            command.Execute();
            RedoStack.Clear();
            if (BatchNumber == 0)
            {
                UndoStack.Push(command);
                Changed?.Invoke(this, EventArgs.Empty);
            }
            else
                UndoBatch.Add(command);
#if DEBUG
            if (BatchNumber == 0)
                Console.WriteLine($"Added action to main stack: \"{command.Description}\". Undo stack has {UndoStack.Count} items");
            else
                Console.WriteLine($"Added action to batch: \"{command.Description}\". Batch has {UndoBatch.Count} items");
#endif
        }

        public void Undo(int count = 1)
        {
            for (int i = 0; i < count && UndoStack.Any(); i++)
            {
                var action = UndoStack.Pop();
                RedoStack.Push(action);
                action.Undo();
#if DEBUG
                Console.WriteLine($"Performed undo of action \"{action.Description}\". Undo stack has {UndoStack.Count} items. Redo stack has {RedoStack.Count} items");
#endif
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
#if DEBUG
                Console.WriteLine($"Performed redo of action \"{action.Description}\". Redo stack has {RedoStack.Count} items. Undo stack has {UndoStack.Count} items");
#endif
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

        // call this and then do things that signal undos, then call FinishBatchOperation to merge all those undos into one
        public void StartBatchOperation()
        {
            BatchNumber++;
#if DEBUG
            Console.WriteLine($"Starting a batch operation");
            if (BatchNumber > 1)
                Console.WriteLine($"It's nested (batch {BatchNumber}), that's a bit unusual");
#endif
        }

        public void FinishBatchOperation(string description, bool replace_single)
        {
            if (BatchNumber == 0)
            {
#if DEBUG
                Console.WriteLine($"Told to finish a batch operation but we aren't currently doing one?");
#endif
                return;
            }
#if DEBUG
            if (!UndoBatch.Any())
                Console.WriteLine($"Finished a batch that didn't have any actions in it?");
            if (BatchNumber > 1)
                Console.WriteLine($"Finished nested batch {BatchNumber}, continuing to batch");
#endif
            BatchNumber--;
            if (BatchNumber == 0 && UndoBatch.Count > 0)
            {
                var merged_action = CommandExtensions.Merge(description, replace_single, UndoBatch.ToArray());
                UndoStack.Push(merged_action);
#if DEBUG
                Console.WriteLine($"Finished batch of {UndoBatch.Count} actions, merged onto stack as action: \"{description}\". Stack has {UndoStack.Count} items");
#endif
                UndoBatch.Clear();
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
