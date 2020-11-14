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
    public class ChunkNode : ModelNode
    {
        public readonly Chunk Chunk;
        private bool HasSetupEvents = false;
        public ChunkNode(NbtTreeModel tree, INode parent, Chunk chunk) : base(tree, parent)
        {
            Chunk = chunk;
            if (Chunk.IsLoaded)
                SetupEvents();
            else
                Chunk.OnLoaded += Chunk_OnLoaded;
        }

        public NbtCompound AccessChunkData()
        {
            if (!Chunk.IsLoaded)
                Chunk.Load();
            return Chunk.Data;
        }

        public override bool HasChildren
        {
            get
            {
                if (Chunk.IsExternal)
                    return false;
                if (!Chunk.IsLoaded)
                    return true;
                return base.HasChildren;
            }
        }

        private void Chunk_OnLoaded(object sender, EventArgs e)
        {
            SetupEvents();
        }

        private void SetupEvents()
        {
            if (!HasSetupEvents)
            {
                Chunk.Data.Changed += Data_Changed;
                Chunk.Data.ActionPerformed += Data_ActionPerformed;
                HasSetupEvents = true;
            }
        }

        private void Data_ActionPerformed(object sender, UndoableAction e)
        {
            NoticeAction(e);
        }

        private void Data_Changed(object sender, NbtTag e)
        {
            RefreshChildren();
        }

        protected override IEnumerable<object> GetChildren()
        {
            if (!Chunk.IsLoaded)
                Chunk.Load();
            return Chunk.Data;
        }

        public override string Description => NbtUtil.ChunkDescription(Chunk);

        public override bool CanCopy => !Chunk.IsExternal;
        public override DataObject Copy() => NbtNodeOperations.Copy(AccessChunkData());
        public override bool CanDelete => !Chunk.IsExternal;
        public override void Delete()
        {
            var region = Chunk.Region;
            if (region != null)
            {
                var action = new UndoableAction(new DescriptionHolder("Remove {0}", Chunk),
                    () => { region.RemoveChunk(Chunk.X, Chunk.Z); },
                    () => { region.AddChunk(Chunk); }
                );
                action.Do();
                NoticeAction(action);
            }
        }
        public override bool CanEdit => !Chunk.IsExternal;
        public override bool CanPaste => !Chunk.IsExternal;
        public override bool CanRename => !Chunk.IsExternal;
        public override bool CanSort => !Chunk.IsExternal;
        public override void Sort() => NbtNodeOperations.Sort(AccessChunkData());
        public override IEnumerable<INode> Paste(IDataObject data)
        {
            var tags = NbtNodeOperations.Paste(AccessChunkData(), data);
            return FindChildren<NbtTagNode>(tags, x => x.Tag);
        }
        public override bool CanReceiveDrop(IEnumerable<INode> nodes) => nodes.All(x => x is NbtTagNode);
        public override void ReceiveDrop(IEnumerable<INode> nodes, int index)
        {
            var tags = nodes.Filter(x => x.GetNbtTag());
            NbtNodeOperations.ReceiveDrop(AccessChunkData(), tags, index);
        }
    }
}
