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
    public class NbtFileNode : Node<NbtFile, NbtTag>
    {
        public NbtFileNode(Node parent, NbtFile wrapped) : base(parent, wrapped) { }

        protected override IEnumerable<NbtTag> GetTypedChildren()
        {
            if (WrappedObject.RootTag is NbtContainerTag container)
                return container;
            return null;
        }

        protected override Node MakeTypedChild(NbtTag obj)
        {
            return new NbtTagNode(this, obj);
        }

        protected override NbtTag GetNbtTag() => WrappedObject.RootTag;

        public override IconType GetIcon() => IconType.File;
        public override string PreviewName() => System.IO.Path.GetFileName(WrappedObject.Path);
        public override string PreviewValue() => NbtUtil.PreviewNbtValue(WrappedObject.RootTag);
    }
}
