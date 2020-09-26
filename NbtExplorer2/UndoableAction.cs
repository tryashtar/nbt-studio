using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
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
            UndoAction += other.UndoAction;
        }
    }
}
