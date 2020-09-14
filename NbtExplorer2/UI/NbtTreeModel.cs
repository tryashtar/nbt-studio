using Aga.Controls.Tree;
using fNbt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtExplorer2.UI
{
    public class NbtTreeModel : ITreeModel
    {
        public event EventHandler<TreeModelEventArgs> NodesChanged;
        public event EventHandler<TreeModelEventArgs> NodesInserted;
        public event EventHandler<TreeModelEventArgs> NodesRemoved;
        public event EventHandler<TreePathEventArgs> StructureChanged;

        public readonly IEnumerable<object> Roots;
        private readonly Dictionary<object, object> ParentDict = new Dictionary<object, object>();
        public NbtTreeModel(IEnumerable<object> roots)
        {
            Roots = roots;
        }

        public void Remove(object obj)
        {
            INbt.Delete(obj);
            NodesRemoved?.Invoke(this, new TreeModelEventArgs(GetPath(ParentDict[obj]), new[] { obj }));
        }

        private TreePath GetPath(object item)
        {
            Stack<object> stack = new Stack<object>();
            while (!Roots.Contains(item))
            {
                stack.Push(item);
                item = ParentDict[item];
            }
            return new TreePath(stack.ToArray());
        }

        public IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return Roots;
            else
                return GetChildren(treePath.LastNode, true);
        }

        public bool IsLeaf(TreePath treePath)
        {
            return !HasChildren(treePath.LastNode);
        }

        private IEnumerable<object> GetChildren(object obj, bool remember_parent)
        {
            var children = INbt.GetChildren(obj);
            if (children != null && remember_parent)
            {
                foreach (var child in children)
                {
                    ParentDict[child] = obj;
                }
            }
            return children;
        }

        private bool HasChildren(object obj)
        {
            var children = GetChildren(obj, false);
            return children != null && children.Any();
        }
    }
}
