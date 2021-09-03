using fNbt;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class ExtraOperationCommand : ICommand
    {
        private readonly ICommand BaseCommand;
        private readonly Action ExtraDo;
        private readonly Action ExtraUndo;

        public string Description => BaseCommand.Description;

        public ExtraOperationCommand(ICommand wrapped, Action extra_do, Action extra_undo)
        {
            BaseCommand = wrapped;
            ExtraDo = extra_do;
            ExtraUndo = extra_undo;
        }

        public void Execute()
        {
            BaseCommand.Execute();
            ExtraDo();
        }

        public void Undo()
        {
            BaseCommand.Undo();
            ExtraUndo();
        }
    }
}
