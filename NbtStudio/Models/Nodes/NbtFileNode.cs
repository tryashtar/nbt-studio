using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Forms;

namespace NbtStudio
{
    public class NbtFileNode : ModelNode<NbtTag>
    {
        public readonly NbtFile File;
        public NbtFileNode(NbtTreeModel tree, INode parent, NbtFile file) : base(tree, parent)
        {
            File = file;
            File.OnSaved += File_OnSaved;
        }

        protected override void SelfDispose()
        {
            File.OnSaved -= File_OnSaved;
        }

        private void File_OnSaved()
        {
            NotifyChanged();
        }

        protected override IEnumerable<NbtTag> GetChildren()
        {
            if (File.RootTag is NbtContainerTag container)
                return container;
            return Enumerable.Empty<NbtTag>();
        }

        public override string Description => File.Path is null ? "unsaved file" : System.IO.Path.GetFileName(File.Path);

        // file nodes copy as both a file and a compound
        // they can then be pasted as text or into explorer, cool!
        public override bool CanCopy => true;
        public override DataObject Copy()
        {
            var data1 = NbtNodeOperations.Copy(File.RootTag);
            var data2 = FileNodeOperations.Copy(File.Path);
            return Utils.Merge(data1, data2);
        }
        public override bool CanCut => true;
        public override DataObject Cut()
        {
            var data1 = NbtNodeOperations.Copy(File.RootTag);
            var data2 = FileNodeOperations.Cut(File.Path);
            return Utils.Merge(data1, data2);
        }
        public override bool CanDelete => true;
        public override void Delete()
        {
            FileNodeOperations.DeleteFile(File.Path);
            base.Delete();
        }
        public override bool CanEdit => File.Path is not null;
        public override bool CanPaste => NbtNodeOperations.CanPaste(File.RootTag);
        public override bool CanRename => File.Path is not null;
        public override bool CanSort => NbtNodeOperations.CanSort(File.RootTag);
        public override void Sort() => NbtNodeOperations.Sort(File.RootTag);
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(File.RootTag, data);
            return NodeChildren(tags);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode) && NbtNodeOperations.CanReceiveDrop(File.RootTag, nodes.Filter(x => x.GetNbtTag()));
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(File.RootTag, tags, index);
        }
    }
}
