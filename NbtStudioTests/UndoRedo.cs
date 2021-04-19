using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbtStudio;

namespace NbtStudioTests
{
    [TestClass]
    public class UndoRedo
    {
        [TestMethod]
        public void BasicUndo()
        {
            int before = 10;
            int after = 10;
            var actions = new Stack<UndoableAction>();
            var tag = new NbtInt();
            tag.ActionPerformed += a => actions.Push(a);

            tag.Value = before;
            tag.Value = after;
            actions.Pop().Undo();

            Assert.AreEqual(tag.Value, before);
        }

        [TestMethod]
        public void ModelUndo()
        {
            var compound = new NbtCompound();

            //compound.Add(new NbtByte("test"));
            //Assert.AreEqual(compound.Count, 1);
            //model.UndoHistory.Undo();
            //Assert.AreEqual(compound.Count, 0);

            var sub = new NbtCompound("test");
            var b1 = new NbtByte("b1");
            var b2 = new NbtByte("b2");
            compound.Add(sub);
            sub.Add(b1);
            sub.Add(b2);

            var model = new NbtTreeModel();
            var node = new NbtTagNode(model, null, compound);
            model.Import(node);

            model.UndoHistory.StartBatchOperation();
            node.ReceiveDrop(new[] { node.Children.First() }, 0);
            model.UndoHistory.FinishBatchOperation(new DescriptionHolder(""), true);

            model.UndoHistory.StartBatchOperation();
            node.Children.First().ReceiveDrop(new[] { node.Children.First().Children.First() }, 0);
            model.UndoHistory.FinishBatchOperation(new DescriptionHolder(""), true);

            model.UndoHistory.Undo();
        }
    }
}
