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
    public class ResetValueCommand : ICommand
    {
        public readonly NbtTag Tag;
        private object OriginalValue;

        public string Description => $"Reset value of {CommandExtensions.Describe(Tag)}";

        public ResetValueCommand(NbtTag tag)
        {
            Tag = tag;
        }

        public void Execute()
        {
            OriginalValue = NbtUtil.GetValue(Tag);
            NbtUtil.ResetValue(Tag);
        }

        public void Undo()
        {
            NbtUtil.SetValue(Tag, OriginalValue);
        }
    }
}
