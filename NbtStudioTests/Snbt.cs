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
    public class Snbt
    {
        [TestMethod]
        public void RoundTripNaN()
        {
            var compound1 = new NbtCompound
            {
                new NbtDouble("double", double.NaN),
                new NbtFloat("float", float.NaN),
            };
            string snbt1 = compound1.ToSnbt(SnbtOptions.Default);

            var compound2 = SnbtParser.Parse(snbt1, named: false);
            string snbt2 = compound2.ToSnbt(SnbtOptions.Default);

            Assert.AreEqual(snbt1, snbt2);
        }

        [TestMethod]
        public void RoundTripInfinity()
        {
            var compound1 = new NbtCompound
            {
                new NbtDouble("double_plus", double.PositiveInfinity),
                new NbtDouble("double_minus", double.NegativeInfinity),
                new NbtFloat("float_plus", float.PositiveInfinity),
                new NbtFloat("float_minus", float.NegativeInfinity),
            };
            string snbt1 = compound1.ToSnbt(SnbtOptions.Default);

            var compound2 = SnbtParser.Parse(snbt1, named: false);
            string snbt2 = compound2.ToSnbt(SnbtOptions.Default);

            Assert.AreEqual(snbt1, snbt2);
        }

        [TestMethod]
        public void RoundTripEscapes()
        {
            var compound1 = new NbtCompound
            {
                new NbtString("simple_name", "simple_value"),
                new NbtString("name with spaces", "simple_value"),
                new NbtString("simple_name_2", "value with spaces"),
                new NbtString("name with spaces 2", "value with spaces"),
                new NbtString("name with \"double quotes\"", "value with 'single quotes'"),
                new NbtString("name with 'single quotes'", "value with \"double quotes\""),
                new NbtString("name with\nnewline", "value with\nnewline"),
            };
            string snbt1 = compound1.ToSnbt(SnbtOptions.Default);

            var compound2 = SnbtParser.Parse(snbt1, named: false);
            string snbt2 = compound2.ToSnbt(SnbtOptions.Default);

            Assert.AreEqual(snbt1, snbt2);
        }

        [TestMethod]
        public void SpecialFloatStringsSuffixed()
        {
            var snbt = "{test1:infinityf,test2:-infinityf,test3:+∞f,test4:∞f,test5:-∞f,test6:nanf}";
            var parsed = (NbtCompound)SnbtParser.Parse(snbt, named: false);
            Assert.IsTrue(parsed.Tags.All(x => x.TagType == NbtTagType.Float));
        }

        [TestMethod]
        public void SpecialDoubleStringsSuffixed()
        {
            var snbt = "{test1:infinityd,test2:-infinityd,test3:+∞d,test4:∞d,test5:-∞d,test6:nand}";
            var parsed = (NbtCompound)SnbtParser.Parse(snbt, named: false);
            Assert.IsTrue(parsed.Tags.All(x => x.TagType == NbtTagType.Double));
        }

        [TestMethod]
        public void SpecialDoubleStrings()
        {
            var snbt = "{test1:infinity,test2:-infinity,test3:+∞,test4:∞,test5:-∞,test6:nan}";
            var parsed = (NbtCompound)SnbtParser.Parse(snbt, named: false);
            Assert.IsTrue(parsed.Tags.All(x => x.TagType == NbtTagType.Double));
        }

        [TestMethod]
        public void SpecialByteStrings()
        {
            var snbt = "{test1:true,test2:false}";
            var parsed = (NbtCompound)SnbtParser.Parse(snbt, named: false);
            Assert.IsTrue(parsed.Tags.All(x => x.TagType == NbtTagType.Byte));
        }

        [TestMethod]
        public void NonSpecialStrings()
        {
            var snbt = "{test1:infinitydd,test2:trueb,test3:falseb,test4:--infinity}";
            var parsed = (NbtCompound)SnbtParser.Parse(snbt, named: false);
            Assert.IsTrue(parsed.Tags.All(x => x.TagType == NbtTagType.String));
        }
    }
}
