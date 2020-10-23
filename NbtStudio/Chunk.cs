using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface IChunk
    {
        IRegion Region { get; }
        int X { get; }
        int Z { get; }
        bool IsLoaded { get; }
        bool IsCorrupt { get; }
        void Load();
        void Remove();
        void AddTo(IRegion region);
        void Move(int x, int z);
    }

    public class Chunk : IChunk
    {
        public IRegion Region { get; internal set; }
        public int X { get; private set; }
        public int Z { get; private set; }
        public NbtCompound Data { get; private set; }
        private readonly int Offset;
        private readonly Stream Stream;
        private NbtCompression Compression;
        public bool IsLoaded => Data != null;
        public bool IsCorrupt { get; private set; } = false;
        internal Chunk(IRegion region, int x, int z, int offset, Stream stream)
        {
            Region = region;
            X = x;
            Z = z;
            Offset = offset;
            Stream = stream;
        }

        public static Chunk EmptyChunk()
        {
            var stream = new MemoryStream();
            var file = new fNbt.NbtFile();
            file.SaveToStream(stream, NbtCompression.None);
            var chunk = new Chunk(null, -1, -1, 0, stream);
            chunk.Data = file.RootTag;
            chunk.Compression = NbtCompression.ZLib;
            return chunk;
        }

        public byte[] SaveBytes()
        {
            if (!IsLoaded)
                Load();
            if (IsCorrupt)
                return new byte[0];
            var file = new fNbt.NbtFile(Data);
            var bytes = file.SaveToBuffer(Compression);
            var with_header = new byte[bytes.Length + 5];
            Array.Copy(bytes, 0, with_header, 5, bytes.Length);
            var length = Util.GetBytes(bytes.Length);
            Array.Copy(length, with_header, 4);
            if (Compression == NbtCompression.GZip)
                with_header[4] = 1;
            else if (Compression == NbtCompression.ZLib)
                with_header[4] = 2;
            return with_header;
        }

        public void Load()
        {
            if (IsCorrupt) return;
            Stream.Seek(Offset + 5, SeekOrigin.Begin);
            var file = new fNbt.NbtFile();
            try
            {
                file.LoadFromStream(Stream, NbtCompression.AutoDetect);
                Compression = file.FileCompression;
                Data = file.RootTag;
            }
            catch
            {
                IsCorrupt = true;
                Remove();
            }
        }

        public void Remove()
        {
            if (Region != null)
                Region.RemoveChunk(X, Z);
        }

        public void AddTo(IRegion region)
        {
            if (Region != null)
                Region.RemoveChunk(X, Z);
            region.AddChunk(this);
        }

        public void Move(int x, int z)
        {
            var region = Region;
            if (region != null)
                Region.RemoveChunk(X, Z);
            X = x;
            Z = z;
            if (region != null)
                region.AddChunk(this);
        }
    }
}
