using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class Chunk : IExportable
    {
        public const int BlocksXDimension = 16;
        public const int BlocksZDimension = 16;
        public RegionFile Region { get; internal set; }
        public int X { get; private set; }
        public int Z { get; private set; }
        public NbtCompound Data { get; private set; }
        public bool HasUnsavedChanges { get; private set; } = false;
        private readonly int Offset;
        private readonly int Size;
        private NbtCompression Compression;
        public bool IsLoaded => Data != null;
        public event EventHandler OnLoaded;
        public bool IsCorrupt { get; private set; } = false;
        public bool IsExternal { get; private set; } = false;
        private byte ExternalCompression;
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
            Data = data;
            Data.Changed += (s, e) => HasUnsavedChanges = true;
        }

        public byte[] SaveBytes()
        {
            if (IsExternal)
            {
                var data = new byte[Size + 5];
                var size = Util.GetBytes(1);
                Array.Copy(size, data, 4);
                data[4] = ExternalCompression;
                return data;
            }
            if (!IsLoaded)
            {
                var stream = Region.GetStream();
                stream.Seek(Offset, SeekOrigin.Begin);
                byte[] result = new byte[Size];
                stream.Read(result, 0, Size);
                stream.Dispose();
                return result;
            }
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
            HasUnsavedChanges = false;
            return with_header;
        }

        public void Load()
        {
            if (IsCorrupt || IsExternal) return;
            var stream = Region.GetStream();
            stream.Seek(Offset + 4, SeekOrigin.Begin);
            int compression = stream.ReadByte();
            if (compression == -1)
            {
                IsCorrupt = true;
                Remove();
            }
            else
            {
                if ((compression & (1 << 7)) != 0)
                {
                    IsExternal = true;
                    ExternalCompression = (byte)compression;
                }
                else
                {
                    var file = new fNbt.NbtFile();
                    try
                    {
                        file.LoadFromStream(stream, NbtCompression.AutoDetect);
                        Compression = file.FileCompression;
                        SetData(file.RootTag);
                    }
                    catch
                    {
                        IsCorrupt = true;
                        Remove();
                    }
                }
            }
            stream.Dispose();
            OnLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void SaveAs(string path)
        {
            File.WriteAllBytes(path, SaveBytes());
        }

        public void Remove()
        {
            if (Region != null)
                Region.RemoveChunk(X, Z);
        }

        public void AddTo(RegionFile region)
        {
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
