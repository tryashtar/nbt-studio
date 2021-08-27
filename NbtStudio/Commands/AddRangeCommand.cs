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
    public class AddRangeCommand : ICommand
    {
        public readonly NbtContainerTag Tag;
        private readonly List<NbtTag> Adding;

        public string Description => $"Add {StringUtils.Pluralize(Adding.Count, "tag")} to {CommandExtensions.Describe(Tag)}";

        public AddRangeCommand(NbtContainerTag tag, IEnumerable<NbtTag> adding)
        {
            Tag = tag;
            Adding = adding.ToList();
        }

        public void Execute()
        {
            Tag.AddRange(Adding);
        }

        public void Undo()
        {
            foreach (var tag in Adding)
            {
                Tag.Remove(tag);
            }
        }
    }
}
