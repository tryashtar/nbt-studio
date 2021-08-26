using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class RegionFile : IFile
    {
        public const int ChunkXDimension = 32;
        public const int ChunkZDimension = 32;
        public static readonly Regex CoordsRegex = new(@"^r\.(?<x>-?[0-9]+)\.(?<z>-?[0-9]+)");
        public RegionCoords Coords
        {
            get
            {
                if (Path is null)
                    return null;
                var match = CoordsRegex.Match(System.IO.Path.GetFileNameWithoutExtension(Path));
                if (!match.Success)
                    return null;
                return new RegionCoords(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["z"].Value));
            }
        }
        public int ChunkCount { get; private set; }
        public event Action OnSaved;
        private ChunkEntry[,] ChunkEntries;
        public string Path { get; private set; }
        private bool UnsavedStructureChanges = false;
        private bool UnsavedChunkChanges = false;
        public bool HasUnsavedChanges => UnsavedStructureChanges || UnsavedChunkChanges;
        public RegionFile(string path)
        {
            Path = path;
            Load();
        }

        private void Load()
        {
            ChunkEntries = new ChunkEntry[ChunkXDimension, ChunkZDimension];
            using var stream = GetStream();
            var locations = IOUtils.ReadBytes(stream, 4096);
            // reading timestamps isn't important
            //var timestamps = IOUtils.ReadBytes(stream, 4096);
            ChunkCount = 0;
            for (int z = 0; z < ChunkEntries.GetLength(1); z++)
            {
                for (int x = 0; x < ChunkEntries.GetLength(0); x++)
                {
                    int offset = ChunkOffset(locations, x, z);
                    int size = ChunkSize(locations, x, z);
                    if (offset > 0 && offset < 8192)
                        throw new FormatException($"Invalid region file, thinks there's a chunk at position {offset} but the header tables are there");
                    if (offset > stream.Length)
                        throw new FormatException($"Invalid region file, thinks there's a {size}-long chunk at position {offset} but file is only {stream.Length} long");
                    if (size > 0)
                    {
                        ChunkCount++;
                        ChunkEntries[x, z] = new ChunkEntry(this, x, z, offset, size);
                        if (ChunkCount == 1)
                            ChunkEntries[x, z].Load(); // load the first one to check if this is really a region file
                    }
                }
            }
            if (ChunkCount == 0)
                throw new FormatException($"Region doesn't contain any chunks");
            UnsavedStructureChanges = false;
        }

        public RegionFile()
        {
            ChunkEntries = new ChunkEntry[ChunkXDimension, ChunkZDimension];
            Path = null;
            ChunkCount = 0;
            UnsavedStructureChanges = true;
        }

        public static Failable<RegionFile> TryCreate(string path)
        {
            return new Failable<RegionFile>(() => new RegionFile(path), "Load as region file");
        }

        internal FileStream GetStream()
        {
            return File.OpenRead(Path);
        }

        public IEnumerable<ChunkEntry> AllChunks => ChunkEntries.Cast<ChunkEntry>().Where(x => x is not null);

        public ChunkEntry GetChunk(int x, int z)
        {
            return ChunkEntries[x, z];
        }

        public IEnumerable<(int x, int z)> GetAvailableCoords(int starting_x = 0, int starting_z = 0)
        {
            for (int x = starting_x; x < ChunkXDimension; x++)
            {
                for (int z = (x == starting_x ? starting_z : 0); z < ChunkZDimension; z++)
                {
                    if (GetChunk(x, z) is null)
                        yield return (x, z);
                }
            }
        }

        private static int ChunkDataLocation(int x, int z)
        {
            return (x % ChunkXDimension + (z % ChunkZDimension) * ChunkZDimension) * 4;
        }

        private static int ChunkSize(byte[] locations, int x, int z)
        {
            int location = ChunkDataLocation(x, z);
            return 4096 * locations[location + 3];
        }

        private static int ChunkOffset(byte[] locations, int x, int z)
        {
            int location = ChunkDataLocation(x, z);
            byte[] four = new byte[4];
            Array.Copy(locations, location, four, 1, 3);
            return 4096 * DataUtils.ToInt32(four);
        }

        public bool CanSave => Path is not null;
        public bool CanRefresh => CanSave;
        public void Save()
        {
            var locations = new byte[4096];
            var timestamps = new byte[4096];
            int current_offset = locations.Length + timestamps.Length;
            var chunk_writes = new List<Action<FileStream>>();
            for (int z = 0; z < ChunkEntries.GetLength(1); z++)
            {
                for (int x = 0; x < ChunkEntries.GetLength(0); x++)
                {
                    var (new_offset, save_action) = SaveChunkInternal(locations, timestamps, current_offset, x, z);
                    current_offset = new_offset;
                    if (save_action is not null)
                        chunk_writes.Add(save_action);
                }
            }
            using (var writer = File.OpenWrite(Path))
            {
                writer.Write(locations, 0, locations.Length);
                writer.Write(timestamps, 0, timestamps.Length);
                foreach (var action in chunk_writes)
                {
                    action(writer);
                }
            }
            UnsavedStructureChanges = false;
            OnSaved?.Invoke();
        }

        private (int new_offset, Action<FileStream> save_action) SaveChunkInternal(byte[] locations, byte[] timestamps, int current_offset, int x, int z)
        {
            var chunk = ChunkEntries[x, z];
            bool update_timestamp = chunk is not null && chunk.IsLoaded;
            int location = ChunkDataLocation(x, z);
            var data = chunk?.Save() ?? Array.Empty<byte>();
            byte size = (byte)Math.Ceiling((decimal)data.Length / 4096);
            byte[] offset = CanWriteChunk(chunk) ? DataUtils.GetBytes(current_offset / 4096) : new byte[] { 0, 0, 0, 0 };
            locations[location] = offset[1];
            locations[location + 1] = offset[2];
            locations[location + 2] = offset[3];
            locations[location + 3] = size;
            if (update_timestamp)
            {
                int timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                byte[] time = DataUtils.GetBytes(timestamp);
                Array.Copy(time, 0, timestamps, location, 4);
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

        private bool CanWriteChunk(ChunkEntry chunk)
        {
            return chunk is not null && chunk.Status != ChunkStatus.Corrupt;
        }

        public void SaveAs(string path)
        {
            Path = path;
            Save();
        }

        public void Refresh()
        {
            Load();
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

        public (int x, int z) WorldChunk(ChunkEntry chunk)
        {
            return (RegionFile.ChunkXDimension * X + chunk.X, RegionFile.ChunkZDimension * Z + chunk.Z);
        }

        public (int x_min, int x_max, int z_min, int z_max) WorldBlocks(ChunkEntry chunk)
        {
            var (chunk_x, chunk_z) = WorldChunk(chunk);
            int block_x = Chunk.BlocksXDimension * chunk_x;
            int block_z = Chunk.BlocksZDimension * chunk_z;
            return (block_x, block_x + Chunk.BlocksXDimension - 1, block_z, block_z + Chunk.BlocksZDimension - 1);
        }
    }
}
