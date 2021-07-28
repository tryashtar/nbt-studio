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
    public class NbtTagNode : Node<NbtTag>
    {
        public NbtTagNode(Node parent, NbtTag wrapped) : base(parent, wrapped) { }
    }
}
