using fNbt;
using System.Collections.Generic;
using System.Linq;
using TryashtarUtils.Utility;

namespace NbtStudio.UI
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
            foreach (var node in SelectedNbt().OfType<NbtContainerTag>())
            {
                var adding = has_original ? tag.Result : (NbtTag)tag.Result.Clone();
                adding.AddTo(node);
                has_original = false;
            }
            return tag.Result;
        }

        private IEnumerable<NbtTag> SelectedNbt()
        {
            return SelectedNodes.Select(x => x.GetNbtTagLens()).Where(x => x != null).Select(x => x.Item);
        }
    }
}
