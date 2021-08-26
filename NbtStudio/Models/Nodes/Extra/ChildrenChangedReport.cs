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
        public readonly OrderedDictionary<object, Node> OldChildren;
        public readonly OrderedDictionary<object, Node> CurrentChildren;
        public readonly OrderedDictionary<object, Node> RemovedChildren;
        public ChildrenChangedReport(Node node, IEnumerable<KeyValuePair<object, Node>> old_children, IEnumerable<KeyValuePair<object, Node>> current_children)
        {
            Node = node;
            OldChildren = new(old_children);
            CurrentChildren = new(current_children);
            RemovedChildren = new(OldChildren.Where(x => !CurrentChildren.ContainsKey(x)));
        }
    }
}
