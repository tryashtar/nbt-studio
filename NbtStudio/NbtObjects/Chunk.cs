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
        public readonly NbtCompound Data;
        
        public Chunk(NbtCompound data)
        {
            Data = data;
        }

        public static Chunk Empty()
        {
            return new Chunk(new NbtCompound());
        }

        public void SaveAs(string path)
        {
            throw new NotImplementedException();
        }
    }
}
