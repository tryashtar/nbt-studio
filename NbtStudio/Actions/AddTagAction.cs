using fNbt;
using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class AddTagAction
    {
        public TagCreator TagSource;
        public ErrorHandler ErrorHandler;
        public IEnumerable<Node> SelectedNodes;

        public NbtTag AddTag()
        {
            var containers = SelectedNodes.Select(x => x.GetNbtTag()).Where(x => x is not null).OfType<NbtContainerTag>();
            if (!containers.Any())
                return null;
            var tag = TagSource(containers.First());
            if (tag.Failed)
            {
                ErrorHandler(tag);
                return null;
            }
            if (tag.Result == null)
                return null;
            bool has_original = true;
            foreach (var container in containers)
            {
                var adding = has_original ? tag.Result : (NbtTag)tag.Result.Clone();
                adding.AddTo(container);
                has_original = false;
            }
            return tag.Result;
        }
    }
}
