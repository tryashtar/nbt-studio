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

        public static void DeleteNbt(IList objects)
        {
            foreach (var nbt in objects)
            {
                if (nbt is NbtTag tag)
                {
                    var parent = tag.Parent;
                    if (parent is NbtCompound compound)
                        compound.Remove(tag);
                    else if (parent is NbtList list)
                        list.Remove(tag);
                }
            }
        }

        public static NbtTag GetTag(object obj)
        {
            if (obj is NbtFile file)
                return file.RootTag;
            if (obj is NbtTag tag)
                return tag;
            return null;
        }
    }
}
