using fNbt;
using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class AddTagAction
    {
        public TagGetter TagSource;
        public ErrorHandler ErrorHandler;
        public IEnumerable<Node> SelectedNodes;

        public NbtTag AddTag()
        {
            var tag = TagSource();
            if (tag.Failed)
            {
                ErrorHandler(tag);
                return null;
            }
            bool has_original = true;
            foreach (var node in SelectedNodes)
            {
                node.ModifyNbt(x =>
                {
                    if (x is NbtContainerTag container)
                    {
                        var adding = has_original ? tag.Result : (NbtTag)tag.Result.Clone();
                        adding.AddTo(container);
                        has_original = false;
                    }
                });
            }
            return tag.Result;
        }
    }
}
