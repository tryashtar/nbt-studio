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
    public class ChunkNode : Node<ChunkEntry, NbtTag>
    {
        public ChunkNode(Node parent, ChunkEntry wrapped) : base(parent, wrapped) { }

        private Chunk GetChunk()
        {
            if (!WrappedObject.IsLoaded)
                WrappedObject.Load();
            return WrappedObject.Chunk;
        }

        protected override IEnumerable<NbtTag> GetTypedChildren()
        {
            return GetChunk().Data;
        }

        public override bool HasChildren => !WrappedObject.IsLoaded || base.HasChildren;

        protected override Node MakeTypedChild(NbtTag obj)
        {
            return new NbtTagNode(this, obj);
        }

        protected override NbtTag GetNbtTag() => GetChunk().Data;

        public override IconType GetIcon() => IconType.Chunk;

        public override string PreviewName()
        {
            string text = $"Chunk [{WrappedObject.X}, {WrappedObject.Z}]";
            var coords = WrappedObject.Region.Coords;
            if (coords is null)
                return text;
            var (x, z) = coords.WorldChunk(WrappedObject);
            return $"{text} in world at ({x}, {z})";
        }

        public override string PreviewValue()
        {
            if (WrappedObject.IsLoaded)
                return NbtUtil.PreviewNbtValue(WrappedObject.Chunk.Data);
            return WrappedObject.Status switch
            {
                ChunkStatus.NotLoaded => "(open to load)",
                ChunkStatus.Corrupt => "(corrupt!)",
                ChunkStatus.External => "(saved externally)",
                _ => throw new ArgumentException(),
            };
        }
    }
}
