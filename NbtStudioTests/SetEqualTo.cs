using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NbtStudio;

namespace NbtStudioTests
{
    [TestClass]
    public class SetEqualTo
    {
        [TestMethod]
        public void KeepSameNames()
        {
            var changing = new NbtCompound
            {
                new NbtByte("a", 0),
                new NbtByte("b", 1),
                new NbtByte("c", 2),
            };
            var template = new NbtCompound
            {
                new NbtByte("a", 10),
                new NbtByte("b", 11),
                new NbtByte("c", 12),
            };
            var tags = changing.Tags.ToList();

            changing.SetEqualTo(template);

            var new_tags = changing.Tags.ToList();
            Assert.IsTrue(tags.SequenceEqual(new_tags));
            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void SameNameDifferentType()
        {
            var changing = new NbtCompound
            {
                new NbtInt("test", 3)
            };
            var template = new NbtCompound
            {
                new NbtString("test", "value")
            };

            changing.SetEqualTo(template);

            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void NestedChanging()
        {
            var changing = new NbtCompound
            {
                new NbtCompound("test")
                {
                    new NbtInt("int", 3),
                    new NbtShort("short", 4)
                }
            };
            var template = new NbtCompound
            {
                new NbtDouble("double", 3)
            };

            changing.SetEqualTo(template);

            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void NestedTemplate()
        {
            var changing = new NbtCompound
            {
                new NbtDouble("double", 3)
            };
            var template = new NbtCompound
            {
                new NbtCompound("test")
                {
                    new NbtInt("int", 3),
                    new NbtShort("short", 4)
                }
            };

            changing.SetEqualTo(template);

            AssertIdentical(template, changing);
        }

        [TestMethod]
        public void NewOrder()
        {
            var changing = new NbtCompound
            {
                new NbtByte("a", 1),
                new NbtByte("b", 2),
                new NbtByte("c", 3),
            };
            var template = new NbtCompound
            {
                new NbtByte("b", 2),
                new NbtByte("c", 3),
                new NbtByte("a", 1),
            };
            var tags = changing.Tags.ToList();

            changing.SetEqualTo(template);

            var new_tags = changing.Tags.ToList();
            Assert.IsTrue(tags.ToHashSet().SetEquals(new_tags));
            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void NestedOrderPreservingNameConflictMegaTest()
        {
            var changing = new NbtCompound
            {
                new NbtByte("root_byte", 3),
                new NbtCompound("root_child")
                {
                    new NbtByte("child_byte", 1),
                    new NbtDouble("child_double", 2),
                    new NbtCompound("child_child")
                    {
                        new NbtString("grandchild_string", "hello"),
                        new NbtLong("grandchild_long", 53),
                    },
                    new NbtShort("child_short", 2)
                },
                new NbtShort("root_short", 2)
            };
            var template = new NbtCompound
            {
                new NbtShort("root_short", 10),
                new NbtCompound("root_child")
                {
                    new NbtLong("child_byte", 1),
                    new NbtLong("child_double", 2),
                    new NbtShort("child_short", 2),
                    new NbtCompound("child_child")
                    {
                        new NbtString("grandchild_string", "hello"),
                    },
                },
                new NbtCompound("root_child2")
                {
                    new NbtByte("child_byte", 3)
                }
            };
            var root_child = changing["root_child"];
            var root_short = changing["root_short"];
            var child_short = changing["root_child"]["child_short"];
            var child_child = changing["root_child"]["child_child"];
            var grandchild_string = changing["root_child"]["child_child"]["grandchild_string"];

            changing.SetEqualTo(template);

            AssertIdentical(changing, template);
            Assert.AreEqual(root_child, changing["root_child"]);
            Assert.AreEqual(root_short, changing["root_short"]);
            Assert.AreEqual(child_short, changing["root_child"]["child_short"]);
            Assert.AreEqual(child_child, changing["root_child"]["child_child"]);
            Assert.AreEqual(grandchild_string, changing["root_child"]["child_child"]["grandchild_string"]);
        }

        [TestMethod]
        public void NumericLists()
        {
            var changing = new NbtList
            {
                new NbtShort(1),
                new NbtShort(2),
                new NbtShort(3),
                new NbtShort(4),
                new NbtShort(5),
            };
            var template = new NbtList
            {
                new NbtShort(10),
                new NbtShort(20),
                new NbtShort(30),
                new NbtShort(40),
                new NbtShort(50),
            };

            var tags = changing.ToList();

            changing.SetEqualTo(template);

            var new_tags = changing.ToList();
            Assert.IsTrue(tags.SequenceEqual(new_tags));
            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void LongerList()
        {
            var changing = new NbtList
            {
                new NbtShort(1),
                new NbtShort(2),
            };
            var template = new NbtList
            {
                new NbtShort(1),
                new NbtShort(2),
                new NbtShort(3),
                new NbtShort(4),
                new NbtShort(5),
            };

            var tags = changing.ToList();

            changing.SetEqualTo(template);

            var new_tags = changing.Take(2).ToList();
            Assert.IsTrue(tags.SequenceEqual(new_tags));
            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void ShorterList()
        {
            var changing = new NbtList
            {
                new NbtShort(1),
                new NbtShort(2),
                new NbtShort(3),
                new NbtShort(4),
                new NbtShort(5),
            };
            var template = new NbtList
            {
                new NbtShort(1),
                new NbtShort(2),
            };

            var tags = changing.Take(2).ToList();

            changing.SetEqualTo(template);

            var new_tags = changing.ToList();
            Assert.IsTrue(tags.SequenceEqual(new_tags));
            AssertIdentical(changing, template);
        }

        [TestMethod]
        public void ListOfCompounds()
        {
            var changing = new NbtList
            {
                new NbtCompound {new NbtByte("test",3)},
                new NbtCompound {},
            };
            var template = new NbtList
            {
                new NbtCompound {new NbtByte("test",10)},
                new NbtCompound {new NbtByte("test",10)},
                new NbtCompound {new NbtByte("test",10)},
            };
            var compound = changing[0];
            var value = changing[0]["test"];

            changing.SetEqualTo(template);
            AssertIdentical(changing, template);
            Assert.AreEqual(compound, changing[0]);
            Assert.AreEqual(value, changing[0]["test"]);
        }

        [TestMethod]
        public void ListOfLists()
        {
            var changing = new NbtList
            {
                new NbtList {new NbtByte(3)},
                new NbtList {},
            };
            var template = new NbtList
            {
                new NbtList {new NbtByte(10)},
                new NbtList {new NbtByte(10)},
                new NbtList {new NbtByte(10)},
            };
            var compound = changing[0];
            var value = changing[0][0];

            changing.SetEqualTo(template);
            AssertIdentical(changing, template);
            Assert.AreEqual(compound, changing[0]);
            Assert.AreEqual(value, changing[0][0]);
        }

        [TestMethod]
        public void CompoundOfLists()
        {
            var changing = new NbtCompound
            {
                new NbtList("a") { new NbtByte(1), new NbtByte(2), new NbtByte(3), },
                new NbtList("b") { new NbtShort(1), new NbtShort(2), new NbtShort(3), },
                new NbtList("c") { new NbtInt(1), new NbtInt(2), new NbtInt(3), },
            };
            var template = new NbtCompound
            {
                new NbtList("a") { new NbtByte(1), new NbtByte(2), new NbtByte(3), new NbtByte(4), },
                new NbtList("b") { },
                new NbtList("c") { new NbtLong(1) },
            };
            var tags = changing.Tags.ToList();
            var bytes = changing.Get<NbtList>("a").Take(3).ToList();

            changing.SetEqualTo(template);

            var new_tags = changing.Tags.ToList();
            var new_bytes = changing.Get<NbtList>("a").Take(3).ToList();
            Assert.IsTrue(tags.SequenceEqual(new_tags));
            Assert.IsTrue(bytes.SequenceEqual(new_bytes));
            AssertIdentical(changing, template);
        }

        private void AssertIdentical(NbtTag c1, NbtTag c2)
        {
            var snbt1 = c1.ToString();
            var snbt2 = c2.ToString();
            Assert.AreEqual(snbt1, snbt2);
        }
    }
}
