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
        Float,
        Operator
    }

    public abstract class Token
    {
        public static readonly Token EOF = null;
        public static string EOL = "\\n";
        public Types type;
        public int line_number;
        public int seq;

        public string text;
        public int int_value;
        public float float_value;

        public Token(int line=0)
        {
            line_number = line;
        }
        public int GetLineNumber()
        {
            return line_number;
        }
        public virtual int GetNumber()
        {
            throw new Exception("not number token");
        }
        public virtual float GetFloat()
        {
            throw new Exception("not float token");
        }
        public virtual string GetText()
        {
            return "";
        }
        public Types GetTokenType()
        {
            return type;
        }
        public string GetValue()
        {
            switch (type)
            {
                case Types.Float:
                    {
                        return float_value.ToString();
                    }
                case Types.Identifier:
                    {
                        return text;
                    }
                case Types.Number:
                    {
                        return int_value.ToString();
                    }
                case Types.String:
                    {
                        return text;
                    }
                case Types.Operator:
                    {
                        return text;
                    }
            }
            return "";
        }
    }

    #region DefinitionTypes
    class IdentifierToken : Token
    {

        public IdentifierToken(int line, int _id, string id) : base(line)
        {
            type = Types.Identifier;
            text = id;
            seq = _id;
        }
        public override string GetText()
        {
            return text;
        }
    }
    class NumberToken : Token
    {

        public NumberToken(int line,int id, int v) : base(line)
        {
            type = Types.Number;
            int_value = v;
            seq = id;
        }
        public override int GetNumber()
        {
            return int_value;
        }
    }
    class StringToken : Token
    {

        public StringToken(int line, int id, string str) : base(line)
        {
            type = Types.String;
            text = str;
            seq = id;
        }
        public override string GetText()
        {
            return text;
        }
    }
    class FloatToken : Token
    {
        public FloatToken(int line, int id, float v) : base(line)
        {
            type = Types.Float;
            float_value = v;
           
            seq = id;
        }
        public override float GetFloat()
        {
            return float_value;
        }
    }

    class OperatorToken : Token
    {
        public OperatorToken(int line, int id, string str) : base(line)
        {
            type = Types.Operator;
            text = str;
            seq = id;
        }

        public override string GetText()
        {
            return text;
        }
    }
    #endregion
}
