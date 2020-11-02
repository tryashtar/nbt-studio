using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbtStudio;

namespace NbtStudioTests
{
    [TestClass]
    public class DragDrop
    {
        [TestMethod]
        public void MoveEarlier()
        {
            var compound = new NbtCompound
            {
                new NbtByte("a", 0),
                new NbtByte("b", 1),
                new NbtByte("c", 2),
                new NbtByte("d", 3),
                new NbtByte("e", 4),
                new NbtByte("f", 5),
                new NbtByte("g", 6),
            };
            var tags = compound.Tags.ToList();
            var moving = new List<INbtTag> { tags[1], tags[3], tags[5] };
            var correct_order = new List<INbtTag> { /**/ tags[1], tags[3], tags[5], /**/ tags[0], tags[2], tags[4], tags[6] };

            NbtUtil.TransformInsert(moving, compound, 0);

            Assert.IsTrue(compound.Tags.SequenceEqual(correct_order));
        }

        [TestMethod]
        public void MoveBetween()
        {
            var compound = new NbtCompound
            {
                new NbtByte("a", 0),
                new NbtByte("b", 1),
                new NbtByte("c", 2),
                new NbtByte("d", 3),
                new NbtByte("e", 4),
                new NbtByte("f", 5),
                new NbtByte("g", 6),
            };
            var tags = compound.Tags.ToList();
            var moving = new List<INbtTag> { tags[1], tags[3], tags[5] };
            var correct_order = new List<INbtTag> { tags[0], tags[2], /**/ tags[1], tags[3], tags[5], /**/ tags[4], tags[6] };

            NbtUtil.TransformInsert(moving, compound, 3);

            Assert.IsTrue(compound.Tags.SequenceEqual(correct_order));
        }

        [TestMethod]
        public void MoveAfter()
        {
            var compound = new NbtCompound
            {
                new NbtByte("a", 0),
                new NbtByte("b", 1),
                new NbtByte("c", 2),
                new NbtByte("d", 3),
                new NbtByte("e", 4),
                new NbtByte("f", 5),
                new NbtByte("g", 6),
            };
            var tags = compound.Tags.ToList();
            var moving = new List<INbtTag> { tags[1], tags[3], tags[5] };
            var correct_order = new List<INbtTag> { tags[0], tags[2], tags[4], /**/ tags[1], tags[3], tags[5], /**/ tags[6] };

            NbtUtil.TransformInsert(moving, compound, 6);

            Assert.IsTrue(compound.Tags.SequenceEqual(correct_order));
        }
    }
}
