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
        public static string regex_pat_float = "([0-9]+[.][0-9]+)";
        public static string regex_pat_number = "([0-9]+)";
        public static string regex_pat_identifier = "([A-Z_a-z][A-Z_a-z0-9]*)";
        public static string regex_pat_punct = "(==|<=|>=|&&|\\|\\|)|[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~]";
        public static string regex_pat_string = "[\"].*[\"]";
        public static string regex_pat_comment = "([//].+)|[/*][\\s\\S]*[*/]";

        public static string regex_pat = "(([0-9]+[.][0-9]+)|([0-9]+)|(\"(\\\\\"|\\\\\\\\|\\n|[^\"])*\")" + 
            "|[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\||[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~])?";
        //|\\p{Punct} \\s* (//.*)|
        public static string program = "void xxx(){\n" +
            "int _i = 0;\n" +
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
        List<Token> queue = new List<Token>();
        private static bool in_string = false;
        static void Main(string[] args)
        {
            file_stream = new FileStream("ACS_Lexer.cs", FileMode.Open);
            file_reader = new StreamReader(file_stream);
            program = file_reader.ReadToEnd();
            file_reader.Close();
            file_stream.Close();
            matches = Regex.Matches(RemoveComment(program), regex_pat);
            foreach (Match item in matches)
            {
                if(item.Value == "\"")
                {
                    in_string = !in_string;
                    return;
                }
                if (in_string)
                {
                    AddToken(item.Value, true);
                }
                AddToken(item.Value, false);
            }

            /*
            foreach (Match item in matches)
            {
                Console.WriteLine(item.Value);
                //AddToken(item.Value);
            }
            //Console.WriteLine("");
            /*
            foreach (Match item in matches)
            {
                Console.Write(item);
            }*/
            Console.Read();
            
        }

        protected string ToStringLiteral(string s)
        {
            StringBuilder string_builder = new StringBuilder();
            int length = s.Length - 1;
            string_builder.Append(s.ToCharArray()[1]);
            return string_builder.ToString();
        }

        protected static void AddToken(string s, bool is_string)
        {
            Token token;
            if (string.IsNullOrWhiteSpace(s)) return;
            if (is_string)
            {
                token = new StringToken(s);
            }
            else if (Regex.Match(s, regex_pat_identifier).Value == s)
            {
                if (s.ToLower() == "true") token = new BoolToken(true);
                if (s.ToLower() == "false") token = new BoolToken(false);
                token = new IdentifierToken(s);
            }
            else if(Regex.Match(s, regex_pat_float).Value == s)
            {
                token = new FloatToken(float.Parse(s));
            }
            else if(Regex.Match(s, regex_pat_number).Value == s)
            {
                token = new NumberToken(int.Parse(s));
            }
            else if (Regex.Match(s, regex_pat_punct).Value == s)
            {
                token = new IdentifierToken(s);
            }
            else
            {
                Console.WriteLine(s);
            }
        }
        
        protected static string RemoveComment(string s)
        {
            string result = s;
            foreach (Match item in Regex.Matches(s, regex_pat_comment))
            {
                result.Replace(item.Value, "");
            }
            return result;
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
    class BoolToken : Token
    {
        private bool value;

        public BoolToken(bool b)
        {
            type = Types.Bool;
            value = b;
        }
        new public bool GetBool()
        {
            return value;
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
