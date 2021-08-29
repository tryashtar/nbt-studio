using fNbt;
using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class AddTagEditor : Editor
    {
        public TagCreator TagSource;
        public ErrorHandler ErrorHandler;
        public readonly NbtTagType Type;

        public AddTagEditor(NbtTagType type)
        {
            Type = type;
        }

        public override bool Filter(Node node)
        {
            var tag = node.GetNbtTag();
            return tag is not null &&
                tag is NbtContainerTag container &&
                container.CanAddType(Type);
        }

        protected override ICommand FilteredEdit(IEnumerable<Node> nodes)
        {
            var containers = nodes.Select(x => (NbtContainerTag)x.GetNbtTag());
            var tag = TagSource(containers.First());
            if (tag.Failed)
            {
                ErrorHandler(tag);
                return null;
            }
            if (tag.Result == null)
                return null;
            bool has_original = true;
            var commands = new List<ICommand>();
            foreach (var container in containers)
            {
                var adding = has_original ? tag.Result : (NbtTag)tag.Result.Clone();
                commands.Add(new MoveCommand(adding, container));
                has_original = false;
            }
            return CommandExtensions.Merge($"Add {CommandExtensions.Describe(tag.Result)} to {StringUtils.Pluralize(containers.Count(), "tag")}", false, commands.ToArray());
        }
    }
}
