using System;

namespace Regexp
{
    public class States
    {
        static void printSpaces(int num){
            for (int i = 0; i < num; ++i)
            {
                Console.Write(" ");
            }
        }
        public static void printnfa(Regex.State start, int depth)
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
    }
}

