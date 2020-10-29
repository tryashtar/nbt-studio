using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public interface IRegion : ISaveable
    {
        int ChunkCount { get; }
        IEnumerable<IChunk> AllChunks { get; }
        IChunk GetChunk(int x, int z);
        void RemoveChunk(int x, int z);
        void AddChunk(Chunk chunk);
    }
    public class RegionFile : IRegion, IDisposable
    {
        public const int ChunkXDimension = 32;
        public const int ChunkZDimension = 32;
        public int ChunkCount { get; private set; }
        private readonly Chunk[,] Chunks;
        private readonly byte[] Locations;
        private readonly byte[] Timestamps;
        private FileStream Stream;
        public string Path { get; private set; }
        public RegionFile(string path)
        {
            Chunks = new Chunk[ChunkXDimension, ChunkZDimension];
            Path = path;
            Stream = File.OpenRead(path);
            Locations = Util.ReadBytes(Stream, 4096);
            Timestamps = Util.ReadBytes(Stream, 4096);
            ChunkCount = 0;
            for (int z = 0; z < Chunks.GetLength(1); z++)
            {
                for (int x = 0; x < Chunks.GetLength(0); x++)
                {
                    int offset = ChunkOffset(x, z);
                    int size = ChunkSize(x, z);
                    if (offset > 0 && offset < 8192)
                        throw new FormatException($"Invalid region file, thinks there's a chunk at position {offset} but the header tables are there");
                    if (offset > Stream.Length)
                        throw new FormatException($"Invalid region file, thinks there's a {size}-long chunk at position {offset} but file is only {Stream.Length} long");
                    if (size > 0)
                    {
                        ChunkCount++;
                        Chunks[x, z] = new Chunk(this, x, z, offset, size, Stream);
                        if (ChunkCount == 1)
                            Chunks[x, z].Load(); // load the first one to check if this is really a region file
                    }
                }
            }
            if (ChunkCount == 0)
                throw new FormatException($"Region doesn't contain any chunks");
        }

        private RegionFile()
        {
            Chunks = new Chunk[ChunkXDimension, ChunkZDimension];
            Path = null;
            Stream = null;
            Locations = new byte[4096];
            Timestamps = new byte[4096];
            ChunkCount = 0;
        }
        public static RegionFile EmptyRegion()
        {
            return new RegionFile();
        }

        public static RegionFile TryCreate(string path)
        {
            try
            { return new RegionFile(path); }
            catch { return null; }
        }

        public IEnumerable<Chunk> AllChunks => Chunks.Cast<Chunk>();
        IEnumerable<IChunk> IRegion.AllChunks => AllChunks;

        public Chunk GetChunk(int x, int z)
        {
            return Chunks[x, z];
        }
        IChunk IRegion.GetChunk(int x, int z) => GetChunk(x, z);

        public void RemoveChunk(int x, int z)
        {
            if (Chunks[x, z] != null)
            {
                Chunks[x, z].Region = null;
                ChunkCount--;
            }
            Chunks[x, z] = null;
        }

        public void AddChunk(Chunk chunk)
        {
            if (Chunks[chunk.X, chunk.Z] != null)
                throw new InvalidOperationException($"There is already a chunk at coordinates {chunk.X}, {chunk.Z}");
            ChunkCount++;
            Chunks[chunk.X, chunk.Z] = chunk;
            chunk.Region = this;
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }

        private static int ChunkDataLocation(int x, int z)
        {
            return (x % ChunkXDimension + (z % ChunkZDimension) * ChunkZDimension) * 4;
        }

        private int ChunkSize(int x, int z)
        {
            int location = ChunkDataLocation(x, z);
            return 4096 * Locations[location + 3];
        }

        private int ChunkOffset(int x, int z)
        {
            int location = ChunkDataLocation(x, z);
            byte[] four = new byte[4];
            Array.Copy(Locations, location, four, 1, 3);
            return 4096 * Util.ToInt32(four);
        }

        public bool CanSave => Path != null;
        public void Save()
        {
            int current_offset = 8192;
            var chunk_writes = new List<Action<FileStream>>();
            for (int z = 0; z < Chunks.GetLength(1); z++)
            {
                for (int x = 0; x < Chunks.GetLength(0); x++)
                {
                    var chunk = Chunks[x, z];
                    bool update_timestamp = chunk != null && chunk.IsLoaded;
                    int location = ChunkDataLocation(x, z);
                    var data = chunk?.SaveBytes() ?? new byte[0];
                    byte size = (byte)Math.Ceiling((decimal)data.Length / 4096);
                    byte[] offset = CanWriteChunk(chunk) ? Util.GetBytes(current_offset / 4096) : new byte[] { 0, 0, 0, 0 };
                    Locations[location] = offset[1];
                    Locations[location + 1] = offset[2];
                    Locations[location + 2] = offset[3];
                    Locations[location + 3] = size;
                    if (update_timestamp)
                    {
                        int timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        byte[] time = Util.GetBytes(timestamp);
                        Array.Copy(time, 0, Timestamps, location, 4);
                    }
                    if (CanWriteChunk(chunk))
                    {
                        int write_offset = current_offset;
                        chunk_writes.Add(writer =>
                            {
                                writer.Seek(write_offset, SeekOrigin.Begin);
                                writer.Write(data, 0, data.Length);
                            });
                    }
                    current_offset += data.Length;
                    current_offset = (int)Math.Ceiling((decimal)current_offset / 4096) * 4096;
                }
            }
            Stream?.Dispose();
            using (var writer = File.OpenWrite(Path))
            {
                writer.Write(Locations, 0, Locations.Length);
                writer.Write(Timestamps, 0, Locations.Length);
                foreach (var action in chunk_writes)
                {
                    action(writer);
                }
            }
            Stream = File.OpenRead(Path);
        }

        private bool CanWriteChunk(Chunk chunk)
        {
            return chunk != null && !chunk.IsCorrupt;
        }

        public void SaveAs(string path)
        {
            Path = path;
            Save();
        }
    }
}
