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
    // basic interface for all items that can be interacted with on the form (tags, files, chunks, etc.)
    // this allows us to create relationships like NBT tags being the direct children of NBT files
    // NbtTreeModel creates nodes for the root objects, and they then create their own children
    // nodes can notify the NbtTreeModel of changes, so it can save undo history and synchronize nodes to the NbtTreeView
    // the ModelNodes test class verifies that we can create an NbtCompound, attach it to an NbtTreeModel, edit it as we please, and it creates nodes on the model and view automatically
    public interface INode
    {
        INode Parent { get; }
        TreePath Path { get; } // path from the root node to this node, can be created by following Parent until null
        IReadOnlyList<INode> Children { get; }
        bool HasChildren { get; } // allows nodes to be expandable without evaluating children yet
        string Description { get; } // appears in undo history
        int DescendantsCount { get; } // total number of descendant nodes, cached
        void NoticeChange();

        // operations to be performed on nodes
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

    // pass in an object and get the best matching node type
    public static class NodeRegistry
    {
        // these used to be included in the static constructor of each class, but they don't run without interacting with the concrete class first
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
