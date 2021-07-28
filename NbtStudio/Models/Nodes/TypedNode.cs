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
    public abstract class Node<T> : Node
    {
        protected readonly T WrappedObject;
        public Node(Node parent, T wrapped) : base(parent)
        {
            WrappedObject = wrapped;
        }

        public Lens<T> GetLens()
        {
            return new Lens<T>(WrappedObject, MarkDirty);
        }
    }
}
