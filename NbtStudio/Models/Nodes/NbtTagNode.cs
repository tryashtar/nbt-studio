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
    public class NbtTagNode : ModelNode
    {
        public readonly NbtTag Tag;
        public NbtTagNode(NbtTreeModel tree, INode parent, NbtTag tag) : base(tree, parent)
        {
            Tag = tag;
            Tag.Changed += Tag_Changed;
            Tag.ActionPerformed += Tag_ActionPerformed;
        }

        private void Tag_ActionPerformed(object sender, UndoableAction e)
        {
            NoticeAction(e);
        }

        private void Tag_Changed(object sender, NbtTag e)
        {
            RefreshChildren();
        }

        static NbtTagNode()
        {
            NodeRegistry.Register<NbtTag>((tree, parent, tag) => new NbtTagNode(tree, parent, tag));
        }

        protected override IEnumerable<object> GetChildren()
        {
            if (Tag is NbtContainerTag container)
                return container;
            return Enumerable.Empty<object>();
        }

        public override string Description => Tag.TagDescription();

        public override bool CanCopy => true;
        public override DataObject Copy() => NbtNodeOperations.Copy(Tag);
        public override bool CanDelete => true;
        public override void Delete()
        {
            Tag.Remove();
        }
        public override bool CanEdit => NbtNodeOperations.CanEdit(Tag);
        public override bool CanPaste => NbtNodeOperations.CanPaste(Tag);
        public override bool CanRename => NbtNodeOperations.CanRename(Tag);
        public override bool CanSort => NbtNodeOperations.CanSort(Tag);
        public override void Sort() => NbtNodeOperations.Sort(Tag);
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(Tag, data);
            return FindChildren<NbtTagNode>(tags, x => x.Tag);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode) && NbtNodeOperations.CanReceiveDrop(Tag, nodes.Filter(x => x.GetNbtTag()));
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(Tag, tags, index);
        }
    }
}
