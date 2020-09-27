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
        private NbtCompression Compression;
        public bool IsLoaded => Data != null;
        public Chunk(int x, int z, int offset, int size, FileStream stream)
        {
            X = x;
            Z = z;
            Offset = offset;
            Size = size;
            Stream = stream;
        }

        public byte[] SaveBytes()
        {
            if (!IsLoaded)
                Load();
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

        private byte[] ReadFromStream()
        {
            Stream.Seek(Offset, SeekOrigin.Begin);
            return Util.ReadBytes(Stream, Size);
        }

        public void Load()
        {
            var data = ReadFromStream();
            var file = new fNbt.NbtFile();
            file.LoadFromBuffer(data, 5, data.Length - 5, NbtCompression.AutoDetect);
            Compression = file.FileCompression;
            Data = file.RootTag;
        }
    }
}
