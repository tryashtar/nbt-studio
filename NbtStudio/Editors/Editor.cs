using System;
using System.Collections.Generic;
using System.Linq;

namespace NbtStudio
{
    public abstract class Editor
    {
        public ICommand Edit(IEnumerable<Node> nodes)
        {
            nodes = nodes.Where(Filter);
            if (!FilteredCanEdit(nodes))
                return null;
            return FilteredEdit(nodes);
        }
        public bool CanEdit(IEnumerable<Node> nodes)
        {
            nodes = nodes.Where(Filter);
            return FilteredCanEdit(nodes);
        }

        public abstract bool Filter(Node node);
        protected abstract ICommand FilteredEdit(IEnumerable<Node> nodes);
        protected virtual bool FilteredCanEdit(IEnumerable<Node> nodes)
        {
            return nodes.Any();
        }
    }
}
