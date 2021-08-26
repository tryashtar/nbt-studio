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
    public class RegionFileNode : FileNode<RegionFile, ChunkEntry>
    {
        public RegionFileNode(RegionFile file) : base(file) { }

        protected override IEnumerable<ChunkEntry> GetTypedChildren()
        {
            return WrappedObject.AllChunks;
        }

        protected override Node MakeTypedChild(ChunkEntry obj)
        {
            return new ChunkNode(obj);
        }

        public override IconType GetIcon() => IconType.Region;
        public override string PreviewValue() => $"[{StringUtils.Pluralize(WrappedObject.ChunkCount, "chunk")}]";
    }
}
