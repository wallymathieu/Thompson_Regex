using System;
using NUnit.Framework;

namespace Regexp
{
    [TestFixture]
    public class SomeTest
    {
        [Test]
        public void Test(){
            Assert.AreEqual ("abc||", RegexToPostfix.re2post ("a|b|c").Value);
            Assert.AreEqual ("ab.c|", RegexToPostfix.re2post ("ab|c").Value);
        }

        [Test]
        public void Test_2(){
            Assert.AreEqual ("ab|", RegexToPostfix.re2post ("(a|b)").Value);
        }
    }
}

