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
    public class CapabilityContext
    {
        public readonly IDataObject ClipboardData;
        public readonly IEnumerable<Node> DropNodes;
    }
}
