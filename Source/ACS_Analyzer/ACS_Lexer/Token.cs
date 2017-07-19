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
        Float
    }

    public abstract class Token
    {
        public static readonly Token EOF = null;
        public static string EOL = "\\n";
        public Types type;

        public int GetNumber()
        {
            throw new Exception("not number token");
        }
        public float GetFloat()
        {
            throw new Exception("not float token");
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
