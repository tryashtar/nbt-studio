using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TryashtarUtils.Utility;

namespace NbtStudio
{
    public abstract class FileNode<WrappedType, ChildType> : Node<WrappedType, ChildType> where WrappedType : class, IFile where ChildType : class
    {
        public FileNode(WrappedType file) : base(file) { }

        public override string PreviewName()
        {
            string name;
            if (WrappedObject.Path == null)
                name = "";
            else
                name = System.IO.Path.GetFileName(WrappedObject.Path);
            if (WrappedObject.HasUnsavedChanges)
            {
                if (name == "")
                    name = "*";
                else
                    name = "* " + name;
            }
            return name;
        }
    }
}
