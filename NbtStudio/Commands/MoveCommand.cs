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
    public class MoveCommand : ICommand
    {
        public readonly NbtTag Tag;
        private NbtContainerTag OriginalParent;
        private int OriginalIndex;
        public readonly NbtContainerTag Parent;

        public string Description => $"Add {CommandExtensions.Describe(Tag)} to {CommandExtensions.Describe(Parent)}";

        public MoveCommand(NbtTag tag, NbtContainerTag parent)
        {
            Tag = tag;
            Parent = parent;
        }

        public void Execute()
        {
            OriginalParent = Tag.Parent;
            if (OriginalParent is not null)
            {
                OriginalIndex = OriginalParent.IndexOf(Tag);
                OriginalParent.RemoveAt(OriginalIndex);
            }
            Parent.Add(Tag);
        }

        public void Undo()
        {
            Parent.Remove(Tag);
            if (OriginalParent is not null)
                OriginalParent.Insert(OriginalIndex, Tag);
        }
    }
}
