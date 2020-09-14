using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2
{
    public static class Controller
    {
        public static IEnumerable<NbtFile> OpenFiles(IEnumerable<string> paths)
        {
            return paths.Select(x => new NbtFile(x));
        }
    }
}
