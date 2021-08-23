using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class RegionFileNode : Node<RegionFile, ChunkEntry>
    {
        public RegionFileNode(Node parent, RegionFile wrapped) : base(parent, wrapped) { }

        protected override IEnumerable<ChunkEntry> GetTypedChildren()
        {
            return WrappedObject.AllChunks;
        }

        protected override Node MakeTypedChild(ChunkEntry obj)
        {
            return new ChunkNode(this, obj);
        }

        public override IconType GetIcon() => IconType.Region;
        public override string PreviewName() => System.IO.Path.GetFileName(WrappedObject.Path);
        public override string PreviewValue() => $"[{StringUtils.Pluralize(WrappedObject.ChunkCount, "chunk")}]";
    }
}
