using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class UndoableAction
    {
        public readonly string Description;
        private Action DoAction;
        private Action UndoAction;
        public bool IsDone { get; private set; } = false;
        public UndoableAction(string description, Action action, Action undo)
        {
            Description = description;
            DoAction = action;
            UndoAction = undo;
        }

        public void Undo()
        {
            if (IsDone)
            {
                UndoAction();
                IsDone = false;
            }
#if DEBUG
            else
            {
                Console.WriteLine($"Tried to undo action '{UndoAction}' twice");
            }
#endif
        }

        public void Do()
        {
            if (!IsDone)
            {
                DoAction();
                IsDone = true;
            }
#if DEBUG
            else
            {
                Console.WriteLine($"Tried to do action '{DoAction}' twice");
            }
#endif
        }

        public void Add(UndoableAction other)
        {
            DoAction += other.DoAction;
            UndoAction = other.UndoAction + UndoAction;
        }

        public static UndoableAction Merge(string description, IEnumerable<UndoableAction> actions)
        {
            var first = actions.First();
            var result = new UndoableAction(description, first.DoAction, first.UndoAction);
            result.IsDone = first.IsDone;
            foreach (var action in actions.Skip(1))
            {
                result.Add(action);
                if (action.IsDone != result.IsDone)
                    throw new ArgumentException($"Can't merge actions with mismatching done-ness: [{action.Description}: {action.IsDone}] and [{result.Description}: {action.IsDone}]");
            }
            return result;
        }
    }
}
