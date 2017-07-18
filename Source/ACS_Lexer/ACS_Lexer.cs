using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ACS_Lexer
{
    class ACS_Lexer
    {
        public static string regex_pat = "((//.*)|([0-9]+)|(\"(\\\\\"|\\\\\\\\|\\n|[^\"])*\")" + "|[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\||[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~])?";
        //|\\p{Punct} \\s*
        public static string program = "void xxx(){\n" +
            "int i = 0;\n" +
            "float f = 666;\n" +
            "i++\n" +
            "}\n";
            
        private static MatchCollection pattern;
        List<Token> queue = new List<Token>();
        static void Main(string[] args)
        {
            pattern = Regex.Matches(program, regex_pat);
            foreach (Match item in pattern)
            {
                Console.Write(item.Value);
            }
            Console.WriteLine("");
            foreach (Match item in pattern)
            {
                Console.Write(item);
            }
            Console.Read();
        }

        protected string ToStringLiteral(string s)
        {
            StringBuilder string_builder = new StringBuilder();
            string_builder.Append(s.ToCharArray()[1]);
            return string_builder.ToString();
        }
    }

    #region DefinitionTypes
    class Identifier : Token
    {
        private string text;

        protected Identifier(int line, string id) : base(line)
        {
            type = Types.Identifier;
            text = id;
        }
        new public string GetText()
        {
            return text;
        }
    }
    class Number : Token
    {
        private int value;

        protected Number(int line, int v) : base(line)
        {
            type = Types.Number;
            value = v;
        }
        new public int GetNumber()
        {
            return value;
        }
    }
    class String : Token
    {
        private string literal;

        protected String(int line, string str) : base(line)
        {
            type = Types.String;
            literal = str;
        }
        new public string GetText()
        {
            return literal;
        }
    }
    class Bool : Token
    {
        private bool value;

        protected Bool(int line, bool b) : base(line)
        {
            type = Types.Bool;
            value = b;
        }
        new public bool GetBool()
        {
            return value;
        }
    }
    class Float : Token
    {
        private float value;

        protected Float(int line, float v) : base(line)
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
