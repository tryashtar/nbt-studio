using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public class Chunk
    {
        public readonly int X;
        public readonly int Z;
        public NbtCompound Data;
        private readonly int Offset;
        private readonly int Size;
        private readonly FileStream Stream;
        public bool IsLoaded => Data != null;
        public Chunk(int x, int z, int offset, int size, FileStream stream)
        {
            X = x;
            Z = z;
            Offset = offset;
            Size = size;
            Stream = stream;
        }

        public byte[] RawData()
        {
            Stream.Seek(Offset, SeekOrigin.Begin);
            return Util.ReadBytes(Stream, Size);
        }

        public void Load()
        {
            var data = RawData();
            var file = new fNbt.NbtFile();
            file.LoadFromBuffer(data, 5, data.Length - 5, NbtCompression.AutoDetect);
            Data = file.RootTag;
        }
    }
}
