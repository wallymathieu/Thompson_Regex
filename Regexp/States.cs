using System;
using System.Text;

namespace Regexp
{
    public class States
    {
        static string printSpaces(int num){
            var sb = new StringBuilder();
            for (int i = 0; i < num; ++i)
            {
                sb.Append(" ");
            }
            return sb.ToString ();
        }
        public static string printnfa(NFA.State start, int depth)
        {
            var sb = new StringBuilder();
            if (depth > 100) {
                throw new Exception ("!!");
            }
            sb.Append(printSpaces(depth));
            sb.Append(String.Format("(c={0}, lastlist={1} \n",start.c, start.lastList));
            if (start.@out!=null){
                sb.Append(printSpaces(depth+1));
                sb.Append(",out=\n");
                sb.Append(printnfa(start.@out.Value, depth+2));
            }
            if (start.out1!=null){
                sb.Append(printSpaces(depth+1));
                sb.Append(",out1:\n");
                sb.Append(printnfa(start.out1.Value, depth+2));
            }
            sb.Append(printSpaces(depth));
            sb.Append(")\n");
            return sb.ToString ();
        }
    }
}

