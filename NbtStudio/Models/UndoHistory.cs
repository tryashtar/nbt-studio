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
        private readonly Func<object, string> DescriptionGenerator;
        private readonly Stack<UndoableAction> UndoStack = new Stack<UndoableAction>();
        private readonly Stack<UndoableAction> RedoStack = new Stack<UndoableAction>();

        public UndoHistory(Func<object, string> description_generator)
        {
            DescriptionGenerator = description_generator;
        }

        public string GetDescription(DescriptionHolder holder)
        {
            return holder.Convert(DescriptionGenerator);
        }

        public void SaveAction(UndoableAction action)
        {
            if (!action.IsDone)
                throw new ArgumentException($"Action {GetDescription(action.Description)} hasn't been done yet, we can't save it to the undo stack");
            RedoStack.Clear();
            if (BatchNumber == 0)
                UndoStack.Push(action);
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
        }

        public void FinishBatchOperation(DescriptionHolder description, bool replace_single)
        {
            if (BatchNumber == 0)
                return;
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
                Console.WriteLine($"Merged {UndoBatch.Count} batch actions onto stack as action: \"{description}\". Stack has {UndoStack.Count} items");
#endif
                UndoBatch.Clear();
            }
        }
    }
}
