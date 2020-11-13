using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbtStudio;
using NbtStudio.SNBT;

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
            var root = new NbtTagNode(null, null, compound);
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
            var root = new NbtTagNode(null, null, compound);
            Assert.AreEqual(root.Children.Count(), 3);
            compound.Add(new NbtInt("more1"));
            compound.Add(new NbtInt("more2"));
            compound.Add(new NbtInt("more3"));
            Assert.AreEqual(root.Children.Count(), 6);
            compound.Remove("more1");
            Assert.AreEqual(root.Children.Count(), 5);
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
