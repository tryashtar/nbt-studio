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
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }

    public static class CommandExtensions
    {
        public static string Describe(NbtTag tag)
        {
            string type = NbtUtil.TagTypeName(tag.TagType).ToLower();
            if (!String.IsNullOrEmpty(tag.Name))
                return $"'{tag.Name}' {type}";
            int index = tag.GetIndex();
            if (index != -1)
            {
                if (!String.IsNullOrEmpty(tag.Parent?.Name))
                    return $"{type} at index {index} in '{tag.Parent.Name}'";
                else if (tag.Parent?.TagType is not null)
                    return $"{type} at index {index} in a {NbtUtil.TagTypeName(tag.Parent.TagType).ToLower()}";
            }
            return type;
        }

        public static ICommand Merge(string description, bool replace_single, params ICommand[] commands)
        {
            ICommand merged_action;
            if (replace_single || commands.Length > 1)
                merged_action = new MergedCommand(description, commands);
            else
                merged_action = commands[0];
            return merged_action;
        }

        public static ICommand MergeNullable(string description, bool replace_single, params ICommand[] commands)
        {
            var not_null = commands.Where(x => x is not null).ToArray();
            if (not_null.Length == 0)
                return null;
            return Merge(description, replace_single, not_null);
        }
    }
}
