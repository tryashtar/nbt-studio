using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbtStudio;

namespace NbtStudioTests
{
    [TestClass]
    public class Sorting
    {
        [TestMethod]
        public void RestoreSort()
        {
            var compound = new NbtCompound
            {
                new NbtLong("ccc", 3),
                new NbtShort("aaa", 3),
                new NbtCompound("bbb")
                {
                    new NbtInt("ccc", 3),
                    new NbtInt("bbb", 3),
                    new NbtInt("aaa", 3),
                }
            };
            var original = (NbtCompound)compound.Clone();
            UndoableAction action = null;
            compound.ActionPerformed += (s, e) => action = e;

            compound.Sort(new AlphabeticalSorter(), true);

            Assert.AreEqual(compound[0].Name, "aaa");
            Assert.AreEqual(compound[1].Name, "bbb");
            Assert.AreEqual(compound[2].Name, "ccc");
            Assert.AreEqual(compound["bbb"][0].Name, "aaa");
            Assert.AreEqual(compound["bbb"][1].Name, "bbb");
            Assert.AreEqual(compound["bbb"][2].Name, "ccc");

            action.Undo();

            AssertIdentical(compound, original);
        }

        private class AlphabeticalSorter : IComparer<NbtTag>
        {
            public int Compare(NbtTag x, NbtTag y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        private void AssertIdentical(NbtTag c1, NbtTag c2)
        {
            var snbt1 = c1.ToString();
            var snbt2 = c2.ToString();
            Assert.AreEqual(snbt1, snbt2);
        }
    }
}
