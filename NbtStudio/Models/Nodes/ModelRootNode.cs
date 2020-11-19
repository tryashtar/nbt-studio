using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class ModelRootNode : ModelNode<object>
    {
        private readonly List<object> ChildObjects;
        public ModelRootNode(NbtTreeModel tree, IEnumerable<object> children) : base(tree, null)
        {
            ChildObjects = children.ToList();
        }

        protected override IEnumerable<object> GetChildren()
        {
            return ChildObjects;
        }

        public void Remove(INode child)
        {
            var children = NodeChildrenMap(ChildObjects);
            foreach (var sibling in children)
            {
                if (sibling.Value == child)
                {
                    ChildObjects.Remove(sibling.Key);
                    RefreshChildren();
                    break;
                }
            }
        }
    }
}
