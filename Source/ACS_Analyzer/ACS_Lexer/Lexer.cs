using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace ACS_Lexer
{
    class Lexer
    {
       
        public static string[] regex_pats =
        {
             //按照这个顺序进行判断
            "",
            "\\s*",
            "(//.*)",
            "([0-9]+[.][0-9]+)",
            "(\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\")",
            "([A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\|)",//|[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~]
            "([-]?[0-9]+)",
            "[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~]"
        };

        public static string regex_pat =
            "((//.*)|([-]?[0-9]+[.][0-9]+)|([0-9]+)|(\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\")" + 
            "|[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\||[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~])?";

        public static string program;

        public static FileStream file_stream;
        public static StreamReader stream_reader;
        private static MatchCollection matches;
        private static int line_number;
        static List<Token> queue = new List<Token>();
        public static List<Token> _Main()
        {
            //后面变成从外
            file_stream = new FileStream(Environment.CurrentDirectory + "/Script.acs", FileMode.Open);
            stream_reader = new StreamReader(file_stream);

            while (ReadLine()) ;
            queue.Add(Token.EOF);

            //Console.WriteLine(queue.Count);

            //for (int i = 0; i < queue.Count; i++)
            //{
            //    if (queue[i] != null)
            //        Console.WriteLine("[" + queue[i].type + "]>>     " + queue[i].GetValue());
            //}
            //Console.Read();

            return queue;
        }

        protected static bool ReadLine()
        {
            line_number++;
            if (stream_reader.EndOfStream) return false;
            program = stream_reader.ReadLine();
            matches = Regex.Matches(program, regex_pat);

            foreach (Match item in matches)
            {
                AddToken(line_number,queue.Count, item.Value);
            }
            return true;
        }

        protected static void AddToken(int line,int id, string s)
        {
            Token token;
            if (InGroup(1, s) || InGroup(2, s)) return;
            if (InGroup(3, s))
            {
                token = new FloatToken(line,  id, float.Parse(s));
                queue.Add(token);
            }
            else if (InGroup(4, s))
            {
                token = new StringToken(line,  id, s);
                queue.Add(token);
            }
            else if (InGroup(5, s))
            {
                token = new IdentifierToken(line,  id, s);
                queue.Add(token);
            }
            else if (InGroup(6, s))
            {
                token = new NumberToken(line, id, int.Parse(s));
                queue.Add(token);
            }
            else if (InGroup(7, s))
            {
                token = new OperatorToken(line, id, s);
                queue.Add(token);
            }
            else
            {
                Console.WriteLine(s);
                Console.WriteLine("");
                //此处后面加上报错
            }
        }
        
        /// <summary>
        /// 进行字符串处理
        /// </summary>
        static string ToStringLiteral(string s)
        {
            StringBuilder sb = new StringBuilder();
            int length = s.Length - 1;
            for (int i = 1; i < length; i++)
            {
                char c = s.ToCharArray()[i];
                if (c == '\\' && i + 1 < length)
                {
                    int c2 = s.ToCharArray()[++i];
                    if (c2 == '"' || c2 == '\\')
                        c = s.ToCharArray()[++i];
                    else if(c2 == 'n')
                    {
                        ++i;
                        c = '\n';
                    }                    
                }
                sb.Append(c);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 判断被获取的字符串是在正则的哪一组匹配的。
        /// c#不自带这个功能还得自己写，还容易出错
        /// </summary>
        static bool InGroup(int i, string s) {if (Regex.Match(s, regex_pats[i]).Value == s) return true;return false;}
    }

   
}
