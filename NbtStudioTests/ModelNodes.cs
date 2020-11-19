using System;
using System.Collections.Generic;
using System.Linq;
using Aga.Controls.Tree;
using fNbt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbtStudio;
using NbtStudio.SNBT;
using NbtStudio.UI;

namespace NbtStudioTests
{
    [TestClass]
    public class ModelNodes
    {
        [TestMethod]
        public void Parenting()
        {
            var compound = new NbtCompound("root")
            {
                new NbtByte("sub1"),
                new NbtShort("sub2"),
                new NbtCompound("sub3")
                {
                    new NbtString("grandchild", "")
                }
            };
            var root = new NbtTagNode(new NbtTreeModel(), null, compound);
            AssertChildStructure(root);
        }

        [TestMethod]
        public void ChildrenRefresh()
        {
            var compound = new NbtCompound("root")
            {
                new NbtByte("sub1"),
                new NbtShort("sub2"),
                new NbtCompound("sub3")
                {
                    new NbtString("grandchild", "")
                }
            };
            var root = new NbtTagNode(new NbtTreeModel(), null, compound);
            Assert.AreEqual(root.Children.Count(), 3);
            compound.Add(new NbtInt("more1"));
            compound.Add(new NbtInt("more2"));
            compound.Add(new NbtInt("more3"));
            Assert.AreEqual(root.Children.Count(), 6);
            compound.Remove("more1");
            Assert.AreEqual(root.Children.Count(), 5);
        }

        [TestMethod]
        public void CheckSynchronized()
        {
            var root = new NbtCompound("test");
            var view = new NbtTreeView();
            var model = new NbtTreeModel((object)root);
            view.Model = model;
            Assert.AreEqual(model.Root.Children.Count(), 1);
            Assert.AreEqual(view.Root.Children.Count, 1);
            AssertSynchronized(view, model);
            root.Add(new NbtByte("test1"));
            AssertSynchronized(view, model);
            root.Add(new NbtByte("test2"));
            AssertSynchronized(view, model);
            root.Add(new NbtCompound("test3"));
            AssertSynchronized(view, model);
            root.Get<NbtCompound>("test3").Add(new NbtShort("test4"));
            Assert.AreEqual(view.Root.DescendantsCount, 5);
            AssertSynchronized(view, model);
            root.Remove("test2");
            AssertSynchronized(view, model);
            root.Get<NbtCompound>("test3").Clear();
            Assert.AreEqual(view.Root.DescendantsCount, 3);
            AssertSynchronized(view, model);
            root.Clear();
            AssertSynchronized(view, model);
        }

        private void AssertSynchronized(NbtTreeView view, NbtTreeModel model)
        {
            var view_queue = new Queue<TreeNodeAdv>();
            var model_queue = new Queue<INode>();
            foreach (var root in view.Root.Children)
            {
                view_queue.Enqueue(root);
            }
            foreach (var root in model.Root.Children)
            {
                model_queue.Enqueue(root);
            }
            while (view_queue.Any() || model_queue.Any())
            {
                Assert.AreEqual(view_queue.Count, model_queue.Count);
                var view_item = view_queue.Dequeue();
                var model_item = model_queue.Dequeue();
                foreach (var child in view_item.Children)
                {
                    view_queue.Enqueue(child);
                }
                foreach (var child in model_item.Children)
                {
                    model_queue.Enqueue(child);
                }
                Assert.AreEqual(view_item.Tag, model_item);
            }
        }

        private void AssertChildStructure(INode node)
        {
            var children = node.Children;
            foreach (var child in children)
            {
                Assert.AreEqual(child.Parent, node);
                AssertChildStructure(child);
            }
        }
    }
}
