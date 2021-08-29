using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbtStudio
{
    public class ChildrenChangedReport
    {
        public readonly Node Node;
        public readonly TreePath Path;
        public readonly OrderedDictionary<object, Node> OldChildren;
        public readonly OrderedDictionary<object, Node> CurrentChildren;
        public readonly OrderedDictionary<object, Node> RemovedChildren;
        public readonly OrderedDictionary<object, Node> AddedChildren;
        public ChildrenChangedReport(Node node, IEnumerable<KeyValuePair<object, Node>> old_children, IEnumerable<KeyValuePair<object, Node>> current_children)
        {
            Node = node;
            Path = node.Path;
            OldChildren = new(old_children);
            CurrentChildren = new(current_children);
            RemovedChildren = new(OldChildren.Where(x => !CurrentChildren.ContainsKey(x)));
            AddedChildren = new(CurrentChildren.Where(x => !OldChildren.ContainsKey(x)));
        }

        public TreeModelEventArgs InsertedArgs()
        {
            if (AddedChildren.Count == 0)
                return null;
            var indices = AddedChildren.Select(x => CurrentChildren.IndexOf(x.Key)).ToArray();
            return new TreeModelEventArgs(Path, indices, AddedChildren.Values.ToArray());
        }

        public TreeModelEventArgs RemovedArgs()
        {
            if (RemovedChildren.Count == 0)
                return null;
            var indices = RemovedChildren.Select(x => OldChildren.IndexOf(x.Key)).ToArray();
            return new TreeModelEventArgs(Path, indices, RemovedChildren.Values.ToArray());
        }

        public TreeModelEventArgs StructureChangedArgs()
        {
            return null;
        }

        public TreeModelEventArgs ChangedArgs()
        {
            if (Node.Parent is null)
                return null;
            return new TreeModelEventArgs(Node.Parent.Path, new object[] { Node });
        }
    }
}
