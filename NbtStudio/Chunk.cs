using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class Chunk
    {
        public RegionFile Region { get; internal set; }
        public int X { get; private set; }
        public int Z { get; private set; }
        public NotifyNbtCompound Data { get; private set; }
        private NbtCompound RawData;
        public bool HasUnsavedChanges { get; private set; } = false;
        private readonly int Offset;
        private readonly int Size;
        private NbtCompression Compression;
        public bool IsLoaded => Data != null;
        public event EventHandler OnLoaded;
        public bool IsCorrupt { get; private set; } = false;
        internal Chunk(RegionFile region, int x, int z, int offset, int size)
        {
            Region = region;
            X = x;
            Z = z;
            Offset = offset;
            Size = size;
        }

        public static Chunk EmptyChunk(NbtCompound data, int x = -1, int z = -1)
        {
            var chunk = new Chunk(null, x, z, 0, 0);
            chunk.SetData(data ?? new NbtCompound(""));
            chunk.Compression = NbtCompression.ZLib;
            chunk.HasUnsavedChanges = true;
            return chunk;
        }

        private void SetData(NbtCompound data)
        {
            RawData = data;
            Data = (NotifyNbtCompound)NotifyNbtTag.CreateFrom(data);
            Data.Changed += (s, e) => HasUnsavedChanges = true;
        }

        public byte[] SaveBytes()
        {
            if (!IsLoaded)
            {
                Region.Stream.Seek(Offset, SeekOrigin.Begin);
                byte[] result = new byte[Size];
                Region.Stream.Read(result, 0, Size);
                return result;
            }
            if (IsCorrupt)
                return new byte[0];
            var file = new fNbt.NbtFile(RawData);
            var bytes = file.SaveToBuffer(Compression);
            var with_header = new byte[bytes.Length + 5];
            Array.Copy(bytes, 0, with_header, 5, bytes.Length);
            var length = Util.GetBytes(bytes.Length);
            Array.Copy(length, with_header, 4);
            if (Compression == NbtCompression.GZip)
                with_header[4] = 1;
            else if (Compression == NbtCompression.ZLib)
                with_header[4] = 2;
            HasUnsavedChanges = false;
            return with_header;
        }

        public void Load()
        {
            if (IsCorrupt) return;
            Region.Stream.Seek(Offset + 5, SeekOrigin.Begin);
            var file = new fNbt.NbtFile();
            try
            {
                file.LoadFromStream(Region.Stream, NbtCompression.AutoDetect);
                Compression = file.FileCompression;
                SetData(file.RootTag);
            }
            catch
            {
                IsCorrupt = true;
                Remove();
            }
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void Remove()
        {
            if (Region != null)
                Region.RemoveChunk(X, Z);
        }

        public void AddTo(RegionFile region)
        {
            if (Region != null)
                Region.RemoveChunk(X, Z);
            region.AddChunk(this);
        }

        public void Move(int x, int z)
        {
            var region = Region;
            if (region != null)
                region.RemoveChunk(X, Z);
            X = x;
            Z = z;
            if (region != null)
                region.AddChunk(this);
        }
    }
}
