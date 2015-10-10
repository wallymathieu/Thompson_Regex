using System;
using NUnit.Framework;

namespace Regexp
{
    [TestFixture]
    public class TogetherTest
    {
        [Test]
        public void Test(){
            //var post = "a";
            var post = ThompsonRegex.re2post("a|b");
            var rs = new Regex.RegexState ();
            var ls = new Regex.ListState ();
            var start = Regex.post2nfa(rs, post).Value;
            Assert.That (Regex.match_ (rs,ls,start, "a"));
        }
    }
}

