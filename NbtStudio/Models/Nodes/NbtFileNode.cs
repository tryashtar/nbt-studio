using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public class NbtFileNode : FileNode<NbtFile, NbtTag>
    {
        public NbtFileNode(NbtFile wrapped) : base(wrapped)
        {
        }

        protected override IEnumerable<NbtTag> GetTypedChildren()
        {
            if (WrappedObject.RootTag is NbtContainerTag container)
                return container.Tags;
            return null;
        }

        protected override Node MakeTypedChild(NbtTag obj)
        {
            return new NbtTagNode(obj);
        }

        public override NbtTag GetNbtTag() => WrappedObject.RootTag;

        public override IconType GetIcon() => IconType.File;
        public override string PreviewValue() => NbtUtil.PreviewNbtValue(WrappedObject.RootTag);
    }
}
