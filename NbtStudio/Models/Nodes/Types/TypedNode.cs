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
    public abstract class Node<WrappedType, ChildType> : Node where WrappedType : class where ChildType : class
    {
        protected readonly WrappedType WrappedObject;
        public Node(Node parent, WrappedType wrapped) : base(parent)
        {
            WrappedObject = wrapped;
        }

        public void ModifyObject(Action<WrappedType> action)
        {
            action(WrappedObject);
            MarkDirty();
        }

        protected abstract Node MakeTypedChild(ChildType obj);
        protected abstract IEnumerable<ChildType> GetTypedChildren();

        protected sealed override Node MakeChild(object item) => MakeTypedChild((ChildType)item);
        protected sealed override IEnumerable<object> GetChildren() => GetTypedChildren() ?? Array.Empty<ChildType>();
    }
}
