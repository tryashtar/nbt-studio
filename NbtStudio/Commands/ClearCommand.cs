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
    public class ClearCommand : ICommand
    {
        public readonly NbtContainerTag Tag;
        private List<NbtTag> OriginalTags;

        public string Description => $"Clear {CommandExtensions.Describe(Tag)}";

        public ClearCommand(NbtContainerTag tag)
        {
            Tag = tag;
        }

        public void Execute()
        {
            OriginalTags = Tag.Tags.ToList();
            Tag.Clear();
        }

        public void Undo()
        {
            Tag.AddRange(OriginalTags);
        }
    }
}
