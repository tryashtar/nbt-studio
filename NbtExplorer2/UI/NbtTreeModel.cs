using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2.UI
{
    public class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public readonly IEnumerable<NbtFile> Files;
        public NbtTreeModel(IEnumerable<NbtFile> files)
        {
            Files = files;
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Files;
            else
                return GetChildren(treePath.LastNode);
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !HasChildren(treePath.LastNode);
        }

        private IEnumerable<object> GetChildren(object obj)
        {
            if (obj is NbtFolder folder)
                return folder.Subfolders.Concat<object>(folder.Files);
            if (obj is NbtFile file)
                return file.RootTag.Tags;
            if (obj is NbtCompound compound)
                return compound.Tags;
            if (obj is NbtList list)
                return list;
            return null;
        }

        private bool HasChildren(object obj)
        {
            var children = GetChildren(obj);
            return children != null && children.Any();
        }
    }
}
