using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACS_Lexer
{
    class ACS_Lexer
    {
        static void Main(string[] args)
        {
            
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
