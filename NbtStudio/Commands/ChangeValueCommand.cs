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
    public class ChangeValueCommand : ICommand
    {
        public readonly NbtTag Tag;
        private object OriginalValue;
        public readonly object Value;
        public ChangeValueCommand(NbtTag tag, object value)
        {
            Tag = tag;
            Value = value;
        }

        public void Execute()
        {
            OriginalValue = NbtUtil.GetValue(Tag);
            NbtUtil.SetValue(Tag, Value);
        }

        public void Undo()
        {
            NbtUtil.SetValue(Tag, OriginalValue);
        }
    }
}
