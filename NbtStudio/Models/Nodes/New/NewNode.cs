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
    public abstract class Node
    {
        public Node Parent { get; private set; }
        public TreePath Path
        {
            get
            {
                var path = new Stack<Node>();
                Node item = this;
                while (item is not null)
                {
                    path.Push(item);
                    item = item.Parent;
                }
                return new TreePath(path.ToArray());
            }
        }
        public IReadOnlyList<Node> Children => ChildNodes;

        private List<Node> ChildNodes;
    }
}
