using fNbt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class ChunkEntry
    {
        public readonly RegionFile Region;
        public readonly int X;
        public readonly int Z;
        public readonly int Offset;
        public readonly int Size;
        public bool IsLoaded => Status == ChunkStatus.Loaded || Status == ChunkStatus.External;
        public ChunkStatus Status { get; private set; } = ChunkStatus.NotLoaded;
        public Chunk Chunk
        {
            get
            {
                if (!IsLoaded || LoadedChunk == null)
                    throw new InvalidOperationException($"Current chunk entry status: {Status}");
                return LoadedChunk;
            }
        }

        private byte ExternalCompression;
        private NbtCompression LocalCompression;
        private Chunk LoadedChunk;

        public ChunkEntry(RegionFile region, int x, int z, int offset, int size)
        {
            Region = region;
            X = x;
            Z = z;
            Offset = offset;
            Size = size;
        }

        public ChunkEntry(RegionFile region, int x, int z, Chunk loaded)
        {
            Region = region;
            X = x;
            Z = z;
            LoadedChunk = loaded;
            Status = ChunkStatus.Loaded;
            LocalCompression = NbtCompression.ZLib;
        }

        public void Load()
        {
#if DEBUG
            if (IsLoaded)
                Debug.WriteLine($"Loading chunk entry ({X},{Z}) that's already loaded");
#endif
            using var stream = Region.GetStream();
            stream.Seek(Offset + 4, SeekOrigin.Begin);
            int compression = stream.ReadByte();

            if (compression == -1)
            {
                Status = ChunkStatus.Corrupt;
                return;
            }

            if ((compression & (1 << 7)) != 0)
            {
                Status = ChunkStatus.External;
                ExternalCompression = (byte)compression;
                return;
            }

            var file = new fNbt.NbtFile();
            try
            {
                file.LoadFromStream(stream, NbtCompression.AutoDetect);
                LocalCompression = file.FileCompression;
                LoadedChunk = new Chunk(file.GetRootTag<NbtCompound>());
                Status = ChunkStatus.Loaded;
            }
            catch
            {
                Status = ChunkStatus.Corrupt;
            }
        }

        public byte[] Save()
        {
            if (Status == ChunkStatus.External)
            {
                throw new NotImplementedException();
            }

            if (Status == ChunkStatus.NotLoaded)
            {
                using var stream = Region.GetStream();
                stream.Seek(Offset, SeekOrigin.Begin);
                byte[] result = new byte[Size];
                stream.Read(result, 0, Size);
                return result;
            }

            if (Status == ChunkStatus.Corrupt)
                return new byte[0];

            var file = new fNbt.NbtFile(Chunk.Data);
            var bytes = file.SaveToBuffer(LocalCompression);
            var with_header = new byte[bytes.Length + 5];
            Array.Copy(bytes, 0, with_header, 5, bytes.Length);
            var length = DataUtils.GetBytes(bytes.Length);
            Array.Copy(length, with_header, 4);
            if (LocalCompression == NbtCompression.GZip)
                with_header[4] = 1;
            else if (LocalCompression == NbtCompression.ZLib)
                with_header[4] = 2;
            return with_header;
        }
    }

    public enum ChunkStatus
    {
        NotLoaded,
        Loaded,
        Corrupt,
        External
    }
}
