using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS_Lexer
{
    public enum Types
    {
        Identifier,
        Number,
        String,
        Bool,
        Float
    }

    class Token
    {
        public static Token EOF = new Token(-1);
        public static string EOL = "\\n";
        private int line_number;
        public Types type;

        private Token(int line)
        {
            line_number = line;
        }
        public int GetLineNumber()
        {
            return line_number;
        }
        public int GetNumber()
        {
            throw new Exception("nut number token");
        }
        public string GetText()
        {
            return "";
        }
        public Types GetTokenType()
        {
            return type;
        }
    }
}
