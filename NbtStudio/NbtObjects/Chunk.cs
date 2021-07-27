using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public class Chunk : IExportable
    {
        public const int BlocksXDimension = 16;
        public const int BlocksZDimension = 16;
        public RegionFile Region { get; internal set; }
        public int X { get; private set; }
        public int Z { get; private set; }
        public readonly NbtCompound Data;
        
        internal Chunk(NbtCompound data, RegionFile region, int x, int z)
        {
            Data = data;
            Region = region;
            X = x;
            Z = z;
        }

        public static Chunk Empty(NbtCompound data, int x = -1, int z = -1)
        {
            return new Chunk(data, null, x, z);
        }

        public void SaveAs(string path)
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            if (Region is not null)
                Region.RemoveChunk(X, Z);
        }

        public void AddTo(RegionFile region)
        {
            region.AddChunk(this);
        }

        public void Move(int x, int z)
        {
            var region = Region;
            if (region is not null)
                region.RemoveChunk(X, Z);
            X = x;
            Z = z;
            if (region is not null)
                region.AddChunk(this);
        }
    }
}
