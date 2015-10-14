using System;
using NUnit.Framework;

namespace Regexp
{
    [TestFixture]
    public class TogetherTest
    {
        private bool IsMatch(string regex, string value){
            var m = Regex.isMatch(regex, value);
            return m!=null ? m.Value : false;
        }
        private bool HasMatch(NFA.State state){
            if (state.c.IsMatch) {
                return true;
            }
            return (state.@out != null && HasMatch (state.@out.Value))
            || (state.@out1 != null && HasMatch (state.@out1.Value));
        }
        private void Should_have_match_state(string regex){
            
            var post = RegexToPostfix.re2post(regex);
            var start = NFA.post2nfa(post);
            if (start == null) {
                throw new Exception ("Failed post2nfa");
            }
            if (!HasMatch (start.Value)) {
                throw new Exception ("Missing match for \n"+States.printnfa (start.Value, 0));
            }
        }

        [Test]
        public void A_or_b(){
            Should_have_match_state ("(a|b)");
            Assert.That (IsMatch ("(a|b)", "a"));
        }

        [Test]
        public void A_followed_by_b(){
            Should_have_match_state ("ab");
            Assert.That (IsMatch ("ab", "ab"));
        }

        [Test]
        public void Just_A(){
            Should_have_match_state ("a");
            Assert.That (IsMatch ("a", "a"));
        }
    }
}

