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
            printnfa (start.Value, 0);
            return (Regex.match_ (rs,ls,start.Value, value));
        }
        private void PrintNfa(State state, int depth){
            
        }
        void printSpaces(int num){
            for (int i = 0; i < num; ++i)
            {
                Console.Write(" ");
            }
        }
        void printnfa(Regex.State start, int depth)
        {
            if (depth > 100) {
                throw new Exception ("!!");
            }
            printSpaces(depth);
            Console.Write(String.Format("(c={0}, lastlist={1} \n",start.c, start.lastList));
            if (start.@out!=null){
                printSpaces(depth+1);
                Console.Write(",out=\n");
                printnfa(start.@out.Value, depth+2);
            }
            if (start.out1!=null){
                printSpaces(depth+1);
                Console.Write(",out1:\n");
                printnfa(start.out1.Value, depth+2);
            }
            printSpaces(depth);
            Console.Write(")\n");
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

