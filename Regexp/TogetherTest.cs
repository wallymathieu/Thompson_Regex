using System;
using NUnit.Framework;

namespace Regexp
{
    [TestFixture]
    public class TogetherTest
    {
        private bool IsMatch(string regex, string value){
            var post = ThompsonRegex.re2post(regex);
            var rs = new Regex.RegexState ();
            var ls = new Regex.ListState ();
            var start = Regex.post2nfa(rs, post);
            if (start == null) {
                throw new Exception ("Failed post2nfa");
            }
            return (Regex.match_ (rs,ls,start.Value, value));
        }

        [Test]
        public void A_or_b(){
            Assert.That (IsMatch ("(a|b)", "a"));
        }

        [Test]
        public void A_followed_by_b(){
            Assert.That (IsMatch ("ab", "ab"));
        }

        [Test]
        public void Just_A(){
            Assert.That (IsMatch ("a", "a"));
        }
    }
}

