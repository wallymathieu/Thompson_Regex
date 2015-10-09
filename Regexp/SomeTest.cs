using System;
using NUnit.Framework;

namespace Regexp
{
    [TestFixture]
    public class SomeTest
    {
        [Test]
        public void Test(){
            Assert.AreEqual ("abc||", ThompsonRegex.re2post ("a|b|c"));
            Assert.AreEqual ("ab.c|", ThompsonRegex.re2post ("ab|c"));
        }
    }
}

