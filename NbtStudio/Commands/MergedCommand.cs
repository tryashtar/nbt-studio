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
    public class MergedCommand : ICommand
    {
        private readonly List<ICommand> Commands;
        private readonly string Description;
        public MergedCommand(string description, IEnumerable<ICommand> commands)
        {
            Commands = commands.ToList();
            Description = description;
        }
        public MergedCommand(string description, params ICommand[] commands) : this(description, (IEnumerable<ICommand>)commands) { }

        string ICommand.Description => Description;
        public void Execute()
        {
            for (int i = 0; i < Commands.Count; i++)
            {
                Commands[i].Execute();
            }
        }

        public void Undo()
        {
            for (int i = Commands.Count - 1; i >= 0; i--)
            {
                Commands[i].Undo();
            }
        }
    }
}
