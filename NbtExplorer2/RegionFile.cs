using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public class RegionFile : ISaveable, IDisposable
    {
        public readonly int ChunkCount;
        private readonly Chunk[,] Chunks = new Chunk[32, 32];
        private readonly byte[] Locations;
        private readonly byte[] Timestamps;
        private readonly FileStream Stream;
        public string Path { get; private set; }
        public RegionFile(string path)
        {
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
                    if (offset + size > Stream.Length)
                        throw new FormatException($"Invalid region file, thinks there a {size}-long chunk at position {offset} but file is only {Stream.Length} long");
                    if (size > 0)
                    {
                        ChunkCount++;
                        Chunks[x, z] = new Chunk(x, z, offset, size, Stream);
                        if (ChunkCount == 1)
                            Chunks[x, z].Load(); // load the first one to check if this is really a region file
                    }
                }
            }
            if (ChunkCount == 0)
                throw new FormatException($"Region doesn't contain any chunks");
        }

        public IEnumerable<Chunk> AllChunks => Chunks.Cast<Chunk>();

        public Chunk GetChunk(int x, int z)
        {
            if (!Chunks[x, z].IsLoaded)
                Chunks[x, z].Load();
            return Chunks[x, z];
        }

        public void Dispose()
        {
            Stream.Dispose();
        }

        private static int ChunkDataLocation(int x, int z)
        {
            return (x % 32 + (z % 32) * 32) * 4;
        }

        private int ChunkSize(int x, int z)
        {
            int location = ChunkDataLocation(x, z);
            return 4096 * Locations[location + 3];
        }

        private int ChunkOffset(int x, int z)
        {
            int location = ChunkDataLocation(x, z);
            return 4096 * Combine(Locations[location], Locations[location + 1], Locations[location + 2]);
        }

        private static int Combine(byte b1, byte b2, byte b3)
        {
            return b1 << 16 & 0xFF0000 | b2 << 8 & 0xFF00 | b3 & 0xFF;
        }

        public bool CanSave => true;
        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string path)
        {
            Path = path;
            Save();
        }
    }
}
