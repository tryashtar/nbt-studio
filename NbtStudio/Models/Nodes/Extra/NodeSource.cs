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
    public abstract class NodeSource
    {
        public event EventHandler Changed;
        public abstract IEnumerable<Node> GetNodes();

        protected void InvokeChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
