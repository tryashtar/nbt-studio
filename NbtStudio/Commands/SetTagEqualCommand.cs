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
    public class SetTagEqualCommand : ICommand
    {
        public readonly NbtTag Tag;
        private NbtTag Original;
        private readonly NbtTag Template;

        public string Description => $"Replace contents of {CommandExtensions.Describe(Tag)}";

        public SetTagEqualCommand(NbtTag tag, NbtTag template)
        {
            Tag = tag;
            Template = template;
        }

        public void Execute()
        {
            Original = (NbtTag)Tag.Clone();
            Tag.SetEqualTo(Template);
        }

        public void Undo()
        {
            Tag.SetEqualTo(Original);
        }
    }
}
