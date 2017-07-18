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

    public abstract class Token
    {
        public static readonly Token EOF = null;
        public static string EOL = "\\n";
        public Types type;
        int line_number;

        protected Token(int line)
        {
            line_number = line;
        }
        public int GetLineNumber()
        {
            return line_number;
        }
        public int GetNumber()
        {
            throw new Exception("not number token");
        }
        public float GetFloat()
        {
            throw new Exception("not float token");
        }
        public bool GetBool()
        {
            throw new Exception("not bool token");
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
