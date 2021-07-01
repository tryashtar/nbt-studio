using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class NbtTagLens<T> where T : NbtTag
    {
        public readonly T Tag;
        public readonly IUndoHistory History;
        public readonly INode Node;
        public NbtTagLens(T tag, IUndoHistory history, INode node)
        {
            Tag = tag;
            History = history;
            Node = node;
        }

        private void PerformAction(DescriptionHolder description, Action action, Action undo)
        {
            action += Changed;
            undo += Changed;
            var undoable = new UndoableAction(description, action, undo);
            undoable.Do();
            History.SaveAction(undoable);
        }

        private void Changed()
        {
            Node.NoticeChange();
        }

        public void SetValue(object value)
        {
            NbtUtil.SetValue(Tag, value);
        }

        public void Sort(IComparer<NbtTag> sorter, bool recursive)
        {
            if (Tag is NbtCompound compound)
            {
                if (recursive)
                {
                    var restore_sort = (NbtCompound)compound.Clone();
                    PerformAction(new DescriptionHolder("Sort {0}", compound),
                         () => compound.Sort(sorter, true),
                         () => compound.UnsortRecursive(restore_sort)
                     );
                }
                else
                {
                    var restore_sort = compound.Tags.ToList();
                    PerformAction(new DescriptionHolder("Sort {0}", compound),
                         () => compound.Sort(sorter, false),
                         () => compound.UnsortRoot(restore_sort)
                     );
                }
            }
        }

        public void Add(NbtTag child)
        {
            if (Tag is NbtContainerTag container)
            {
                PerformAction(new DescriptionHolder("Add {0} to {1}", child, container),
                    () => container.Add(child),
                    () => container.Remove(child)
                );
            }
        }
    }

    public class NbtTagLens : NbtTagLens<NbtTag>
    {
        public NbtTagLens(NbtTag tag, IUndoHistory history, INode node) : base(tag, history, node)
        {
        }
    }
}
