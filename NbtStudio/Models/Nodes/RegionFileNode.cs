using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public class RegionFileNode : ModelNode<Chunk>
    {
        public readonly RegionFile Region;
        public RegionFileNode(NbtTreeModel tree, INode parent, RegionFile file) : base(tree, parent)
        {
            Region = file;
            Region.ChunksChanged += Region_ChunksChanged;
            Region.ActionPerformed += Region_ActionPerformed;
        }

        private void Region_ActionPerformed(object sender, UndoableAction e)
        {
            NoticeAction(e);
        }

        private void Region_ChunksChanged(object sender, EventArgs e)
        {
            RefreshChildren();
        }

        protected override IEnumerable<Chunk> GetChildren()
        {
            return Region.AllChunks;
        }

        public override string Description => Region.Path == null ? "unsaved region file" : System.IO.Path.GetFileName(Region.Path);

        public override bool CanCopy => Region.Path != null;
        public override DataObject Copy()
        {
            var data = new DataObject();
            if (Region.Path != null)
                data.SetFileDropList(new StringCollection { Region.Path });
            return data;
        }
        public override bool CanCut => Region.Path != null;
        public override DataObject Cut() => FileNodeOperations.Cut(Region.Path);
        public override bool CanDelete => true;
        public override void Delete()
        {
            FileNodeOperations.DeleteFile(Region.Path);
        }
        public override bool CanEdit => Region.Path != null;
        public override bool CanPaste => true;
        public override bool CanRename => Region.Path != null;
        public override bool CanSort => false;
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.ParseTags(data).OfType<NbtCompound>().ToList();
            var available = Region.GetAvailableCoords();
            var chunks = Enumerable.Zip(available, tags, (slot, tag) => Chunk.EmptyChunk(tag, slot.x, slot.z)).ToList();
            foreach (var chunk in chunks)
            {
                Region.AddChunk(chunk);
            }
            return NodeChildren(chunks);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is ChunkNode);
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var chunks = nodes.Filter(x => x.GetChunk()).ToList();
            foreach (var chunk in chunks)
            {
                if (Region.GetChunk(chunk.X, chunk.Z) == null)
                    Region.AddChunk(chunk);
            }
        }
    }
}
