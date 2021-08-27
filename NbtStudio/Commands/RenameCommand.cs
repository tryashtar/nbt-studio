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
    public class RenameCommand : ICommand
    {
        public readonly NbtTag Tag;
        private readonly string OriginalDescription;
        private string OriginalName;
        public readonly string Name;

        public string Description => $"Rename {OriginalDescription} to {Name}";

        public RenameCommand(NbtTag tag, string name)
        {
            Tag = tag;
            Name = name;
            OriginalDescription = CommandExtensions.Describe(tag);
        }

        public void Execute()
        {
            OriginalName = Tag.Name;
            Tag.Name = Name;
        }

        public void Undo()
        {
            Tag.Name = OriginalName;
        }
    }
}
