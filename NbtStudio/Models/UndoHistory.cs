using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface IUndoHistory
    {
        void SaveAction(UndoableAction action);
        void Undo();
        void Redo();
    }

    public class UndoHistory
    {
        public delegate string DescriptionGetter(object obj);
        private readonly DescriptionGetter DescriptionSource;
        private readonly Stack<UndoableAction> UndoStack = new();
        private readonly Stack<UndoableAction> RedoStack = new();
        public event EventHandler Changed;

        public UndoHistory(DescriptionGetter description_generator)
        {
            DescriptionSource = description_generator;
        }

        public string GetDescription(DescriptionHolder holder)
        {
            return holder.Convert(DescriptionSource);
        }

        public void SaveAction(UndoableAction action)
        {
            if (!action.IsDone)
                throw new ArgumentException($"Action {GetDescription(action.Description)} hasn't been done yet, we can't save it to the undo stack");
            RedoStack.Clear();
            if (BatchNumber == 0)
            {
                UndoStack.Push(action);
                Changed?.Invoke(this, EventArgs.Empty);
            }
            else
                UndoBatch.Add(action);
#if DEBUG
            if (BatchNumber == 0)
                Console.WriteLine($"Added action to main stack: \"{GetDescription(action.Description)}\". Undo stack has {UndoStack.Count} items");
            else
                Console.WriteLine($"Added action to batch: \"{GetDescription(action.Description)}\". Batch has {UndoBatch.Count} items");
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
                action.Do();
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
            return UndoStack.Select((v, i) => new KeyValuePair<int, string>(i + 1, GetDescription(v.Description))).ToList();
        }
        public List<KeyValuePair<int, string>> GetRedoHistory()
        {
            return RedoStack.Select((v, i) => new KeyValuePair<int, string>(i + 1, GetDescription(v.Description))).ToList();
        }

        private int BatchNumber = 0;
        private readonly List<UndoableAction> UndoBatch = new List<UndoableAction>();
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

        public void FinishBatchOperation(DescriptionHolder description, bool replace_single)
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
            if (BatchNumber == 0 && UndoBatch.Any())
            {
                UndoableAction merged_action;
                if (replace_single || UndoBatch.Count > 1)
                    merged_action = UndoableAction.Merge(description, UndoBatch);
                else
                    merged_action = UndoBatch.Single();
                UndoStack.Push(merged_action);
#if DEBUG
                Console.WriteLine($"Finished batch of {UndoBatch.Count} actions, merged onto stack as action: \"{GetDescription(description)}\". Stack has {UndoStack.Count} items");
#endif
                UndoBatch.Clear();
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
