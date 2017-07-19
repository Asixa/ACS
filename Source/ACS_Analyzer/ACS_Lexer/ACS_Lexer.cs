using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace ACS_Lexer
{
    class ACS_Lexer
    {
        public static string regex_pat1 = "\\s*";
        public static string regex_pat2 = "(//.*)";
        public static string regex_pat3 = "([0-9]+[.][0-9]+)";
        public static string regex_pat4 = "(\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\")";
        public static string regex_pat5 = "([A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\||[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~])";
        public static string regex_pat6 = "([0-9]+)";

        public static string regex_pat = "((//.*)|([0-9]+[.][0-9]+)|([0-9]+)|(\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\")" + 
            "|[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\||[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~])?";
        //|\\p{Punct} \\s* (//.*)|
        public static string program = "void xxx(){\n" +
            "int i666 = 0;\n" +
            "float f = 666.666;\n" +
            "i++;\n" +
            "i += 1" +
            "}\n" +
            "//fangxm is very 666666666666\n" +
            "==" +
            "<=" +
            ">=" +
            "||" +
            "\"6666666666666\"\n" +
            "\"" +
            "/***/";

        static FileStream file_stream;
        static StreamReader file_reader;
        private static MatchCollection matches;
        static List<Token> queue = new List<Token>();
        static void _Main(string[] args)
        {
            file_stream = new FileStream("ACS_Lexer.cs", FileMode.Open);
            file_reader = new StreamReader(file_stream);
            program = file_reader.ReadToEnd();
            file_reader.Close();
            file_stream.Close();
            matches = Regex.Matches(program, regex_pat);
            
            foreach (Match item in matches)
            {
                AddToken(item.Value);
            }

            /*
            foreach (Match item in matches)
            {
                Console.WriteLine(item.Value);
                //AddToken(item.Value);
            }*/
            //Console.WriteLine("");
            /*
            foreach (Match item in matches)
            {
                Console.Write(item);
            }*/
            Console.WriteLine(queue.Count);
            Console.Read();
            
        }

        
        protected static void AddToken(string s)
        {
            Token token;
            if (InGroup(1, s) || InGroup(2, s)) return;
            if (InGroup(3, s))
            {
                token = new FloatToken(float.Parse(s));
                queue.Add(token);
            }
            else if (InGroup(4, s))
            {
                token = new StringToken(s);
                queue.Add(token);
            }
            else if (InGroup(5, s))
            {
                token = new IdentifierToken(s);
                queue.Add(token);
            }
            else if (InGroup(6, s))
            {
                token = new NumberToken(int.Parse(s));
                queue.Add(token);
            }
            else
            {
                Console.WriteLine(s);
                Console.WriteLine("");
            }
        }
        
        
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
        
        static bool InGroup(int i, string s)
        {
            switch (i)
            {
                case 1:
                    {
                        if (Regex.Match(s, regex_pat1).Value == s) return true;
                        break;
                    }
                case 2:
                    {
                        if (Regex.Match(s, regex_pat2).Value == s) return true;
                        break;
                    }
                case 3:
                    {
                        if (Regex.Match(s, regex_pat3).Value == s) return true;
                        break;
                    }
                case 4:
                    {
                        if (Regex.Match(s, regex_pat4).Value == s) return true;
                        break;
                    }
                case 5:
                    {
                        if (Regex.Match(s, regex_pat5).Value == s) return true;
                        break;
                    }
                case 6:
                    {
                        if (Regex.Match(s, regex_pat6).Value == s) return true;
                        break;
                    }
            }
            return false;
        }
    }

    #region DefinitionTypes
    class IdentifierToken : Token
    {
        private string text;

        public IdentifierToken(string id)
        {
            type = Types.Identifier;
            text = id;
        }
        new public string GetText()
        {
            return text;
        }
    }
    class NumberToken : Token
    {
        private int value;

        public NumberToken(int v)
        {
            type = Types.Number;
            value = v;
        }
        new public int GetNumber()
        {
            return value;
        }
    }
    class StringToken : Token
    {
        private string literal;

        public StringToken(string str)
        {
            type = Types.String;
            literal = str;
        }
        new public string GetText()
        {
            return literal;
        }
    }
    class FloatToken : Token
    {
        private float value;

        public FloatToken(float v)
        {
            type = Types.Float;
            value = v;
        }
        new public float GetFloat()
        {
            return value;
        }
    }
    #endregion
}
