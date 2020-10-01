using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class UndoableAction
    {
        private Action DoAction;
        private Action UndoAction;
        public UndoableAction(Action action, Action undo)
        {
            DoAction = action;
            UndoAction = undo;
        }

        public void Undo()
        {
            UndoAction();
        }

        public void Do()
        {
            DoAction();
        }

        public void Add(UndoableAction other)
        {
            DoAction += other.DoAction;
            UndoAction = other.UndoAction + UndoAction;
        }

        public static UndoableAction Merge(IEnumerable<UndoableAction> actions)
        {
            var first = actions.First();
            var result = new UndoableAction(first.DoAction, first.UndoAction);
            foreach (var action in actions.Skip(1))
            {
                result.Add(action);
            }
            return result;
        }
    }
}
