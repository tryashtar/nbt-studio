using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public interface INode
    {
        INode Parent { get; }
        TreePath Path { get; }
        IEnumerable<INode> Children { get; }
        bool HasChildren { get; }
        string Description { get; }
        bool CanDelete { get; }
        void Delete();
        bool CanSort { get; }
        void Sort();
        bool CanCopy { get; }
        DataObject Copy();
        bool CanCut { get; }
        DataObject Cut();
        bool CanPaste { get; }
        IEnumerable<INode> Paste(IDataObject data);
        bool CanRename { get; }
        bool CanEdit { get; }
        bool CanReceiveDrop(IEnumerable<INode> nodes);
        void ReceiveDrop(IEnumerable<INode> nodes, int index);
    }

    public static class NodeRegistry
    {
        static NodeRegistry()
        {
            Register<NbtTag>((tree, parent, tag) => new NbtTagNode(tree, parent, tag));
            Register<NbtFile>((tree, parent, file) => new NbtFileNode(tree, parent, file));
            Register<RegionFile>((tree, parent, region) => new RegionFileNode(tree, parent, region));
            Register<Chunk>((tree, parent, chunk) => new ChunkNode(tree, parent, chunk));
            Register<NbtFolder>((tree, parent, folder) => new FolderNode(tree, parent, folder));
        }

        private static readonly Dictionary<Type, Func<NbtTreeModel, INode, object, INode>> RegisteredConverters = new Dictionary<Type, Func<NbtTreeModel, INode, object, INode>>();
        public static void Register<T>(Func<NbtTreeModel, INode, T, INode> converter)
        {
            RegisteredConverters[typeof(T)] = (tree, parent, item) => converter(tree, parent, (T)item);
        }

        public static INode CreateNode(NbtTreeModel tree, INode parent, object item)
        {
            foreach (var converter in RegisteredConverters)
            {
                if (converter.Key.IsInstanceOfType(item))
                    return converter.Value(tree, parent, item);
            }
            throw new InvalidOperationException($"No registered converter for {item.GetType()}");
        }

        public static INode CreateRootNode(NbtTreeModel tree, object item) => CreateNode(tree, null, item);
    }
}
