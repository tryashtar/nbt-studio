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

        protected override void SelfDispose()
        { }

        public void Add(object obj)
        {
            ChildObjects.Add(obj);
            RefreshChildren();
        }

        public void AddRange(IEnumerable<object> objects)
        {
            ChildObjects.AddRange(objects);
            RefreshChildren();
        }

        public void Remove(INode child)
        {
            RemoveNodes(new[] { child });
        }

        public void RemoveNodes(IEnumerable<INode> nodes)
        {
            var children = NodeChildrenMap(ChildObjects);
            foreach (var sibling in children)
            {
                if (nodes.Contains(sibling.Value))
                    ChildObjects.Remove(sibling.Key);
            }
            RefreshChildren();
        }

        protected override IEnumerable<object> GetChildren()
        {
            return ChildObjects;
        }
    }
}
