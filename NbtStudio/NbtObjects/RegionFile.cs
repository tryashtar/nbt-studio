using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class RegionFile : IFile
    {
        public const int ChunkXDimension = 32;
        public const int ChunkZDimension = 32;
        private static readonly Regex CoordsRegex = new Regex(@"^r\.(?<x>-?[0-9]+)\.(?<z>-?[0-9]+)");
        public RegionCoords Coords
        {
            get
            {
                if (Path == null)
                    return null;
                var match = CoordsRegex.Match(System.IO.Path.GetFileNameWithoutExtension(Path));
                if (!match.Success)
                    return null;
                return new RegionCoords(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["z"].Value));
            }
        }
        public int ChunkCount { get; private set; }
        public event EventHandler ChunksChanged;
        public event EventHandler OnSaved;
        public event EventHandler<UndoableAction> ActionPerformed;
        private Chunk[,] Chunks;
        private byte[] Locations;
        private byte[] Timestamps;
        public string Path { get; private set; }
        public bool HasChunkChanges { get; private set; } = false;
        public bool HasUnsavedChanges => HasChunkChanges || AllChunks.Any(x => x != null && x.HasUnsavedChanges);
        public RegionFile(string path)
        {
            Path = path;
            Load();
        }

        private void Load()
        {
            Chunks = new Chunk[ChunkXDimension, ChunkZDimension];
            var stream = GetStream();
            try
            {
                Locations = Util.ReadBytes(stream, 4096);
                Timestamps = Util.ReadBytes(stream, 4096);
                ChunkCount = 0;
                for (int z = 0; z < Chunks.GetLength(1); z++)
                {
                    for (int x = 0; x < Chunks.GetLength(0); x++)
                    {
                        int offset = ChunkOffset(x, z);
                        int size = ChunkSize(x, z);
                        if (offset > 0 && offset < 8192)
                            throw new FormatException($"Invalid region file, thinks there's a chunk at position {offset} but the header tables are there");
                        if (offset > stream.Length)
                            throw new FormatException($"Invalid region file, thinks there's a {size}-long chunk at position {offset} but file is only {stream.Length} long");
                        if (size > 0)
                        {
                            ChunkCount++;
                            Chunks[x, z] = new Chunk(this, x, z, offset, size);
                            if (ChunkCount == 1)
                                Chunks[x, z].Load(); // load the first one to check if this is really a region file
                        }
                    }
                }
                if (ChunkCount == 0)
                    throw new FormatException($"Region doesn't contain any chunks");
            }
            finally
            {
                stream.Dispose();
            }
        }

        private RegionFile()
        {
            Chunks = new Chunk[ChunkXDimension, ChunkZDimension];
            Path = null;
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

        internal FileStream GetStream()
        {
            return File.OpenRead(Path);
        }

        public IEnumerable<Chunk> AllChunks => Chunks.Cast<Chunk>();

        public Chunk GetChunk(int x, int z)
        {
            return Chunks[x, z];
        }

        public IEnumerable<(int x, int z)> GetAvailableCoords(int starting_x = 0, int starting_z = 0)
        {
            for (int x = starting_x; x < ChunkXDimension; x++)
            {
                for (int z = (x == starting_x ? starting_z : 0); z < ChunkZDimension; z++)
                {
                    if (GetChunk(x, z) == null)
                        yield return (x, z);
                }
            }
        }

        private void PerformAction(DescriptionHolder holder, Action action, Action undo)
        {
            var undoable = new UndoableAction(holder, action, undo);
            undoable.Do();
            ActionPerformed?.Invoke(this, undoable);
        }

        public void RemoveChunk(int x, int z)
        {
            if (Chunks[x, z] != null)
            {
                HasChunkChanges = true;
                var chunk = Chunks[x, z];
                PerformAction(new DescriptionHolder("Remove {0} from {1}", chunk, this),
                    () => DoRemoveChunk(x, z),
                    () => DoAddChunk(chunk)
                );
            }
        }

        public void AddChunk(Chunk chunk)
        {
            HasChunkChanges = true;
            PerformAction(new DescriptionHolder("Add {0} to {1}", chunk, this),
                () => DoAddChunk(chunk),
                () => DoRemoveChunk(chunk.X, chunk.Z)
            );
        }

        private void DoRemoveChunk(int x, int z)
        {
            Chunks[x, z].Region = null;
            Chunks[x, z] = null;
            ChunkCount--;
            ChunksChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DoAddChunk(Chunk chunk)
        {
            if (Chunks[chunk.X, chunk.Z] != null)
                throw new InvalidOperationException($"There is already a chunk at coordinates {chunk.X}, {chunk.Z}");
            if (chunk.Region != null)
            {
                if (!chunk.IsLoaded)
                    chunk.Load();
                chunk.Region.RemoveChunk(chunk.X, chunk.Z);
            }
            Chunks[chunk.X, chunk.Z] = chunk;
            chunk.Region = this;
            ChunkCount++;
            HasChunkChanges = true;
            ChunksChanged?.Invoke(this, EventArgs.Empty);
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
        public bool CanRefresh => CanSave;
        public void Save()
        {
            int current_offset = 8192;
            var chunk_writes = new List<Action<FileStream>>();
            for (int z = 0; z < Chunks.GetLength(1); z++)
            {
                for (int x = 0; x < Chunks.GetLength(0); x++)
                {
                    var (new_offset, save_action) = SaveChunkInternal(current_offset, x, z);
                    current_offset = new_offset;
                    if (save_action != null)
                        chunk_writes.Add(save_action);
                }
            }
            using (var writer = File.OpenWrite(Path))
            {
                writer.Write(Locations, 0, Locations.Length);
                writer.Write(Timestamps, 0, Locations.Length);
                foreach (var action in chunk_writes)
                {
                    action(writer);
                }
            }
            HasChunkChanges = false;
            OnSaved?.Invoke(this, EventArgs.Empty);
        }

        private (int new_offset, Action<FileStream> save_action) SaveChunkInternal(int current_offset, int x, int z)
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
            Action<FileStream> result = null;
            if (CanWriteChunk(chunk))
            {
                int write_offset = current_offset;
                result = writer =>
                {
                    writer.Seek(write_offset, SeekOrigin.Begin);
                    writer.Write(data, 0, data.Length);
                };
            }
            current_offset += data.Length;
            current_offset = (int)Math.Ceiling((decimal)current_offset / 4096) * 4096;
            return (current_offset, result);
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

        public void Refresh()
        {
            Load();
            HasChunkChanges = false;
            ChunksChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Move(string path)
        {
            if (Path != null)
            {
                File.Move(Path, path);
                Path = path;
            }
        }
    }

    public class RegionCoords
    {
        public readonly int X;
        public readonly int Z;
        public RegionCoords(int x, int z)
        {
            X = x;
            Z = z;
        }

        public (int x, int z) WorldChunk(Chunk chunk)
        {
            return (RegionFile.ChunkXDimension * X + chunk.X, RegionFile.ChunkZDimension * Z + chunk.Z);
        }

        public (int x_min, int x_max, int z_min, int z_max) WorldBlocks(Chunk chunk)
        {
            var (chunk_x, chunk_z) = WorldChunk(chunk);
            int block_x = Chunk.BlocksXDimension * chunk_x;
            int block_z = Chunk.BlocksZDimension * chunk_z;
            return (block_x, block_x + Chunk.BlocksXDimension - 1, block_z, block_z + Chunk.BlocksZDimension - 1);
        }
    }
}
