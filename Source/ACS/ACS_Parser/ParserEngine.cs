using System.Collections.Generic;
using ACS.ACS_Lexer;

namespace ACS.ACS_Parser
{
    internal class ParserEngine
    {
        public List<Token> q;
        private List<Grammar> Grammars;

        #region Base

        public ParserEngine(List<Token> q, bool run)
        {
            this.q = q;
            if (run) Begin();
        }

        public void Begin()
        {
            foreach (var t in q)
            {
                Match_grammar(t);
            }
        }

        #endregion

        private void Match_grammar(Token t)
        {
            List<List<Token>> fields;
            foreach (var g in Grammars)
            {
                if (t.Value.ToString() != g.head) continue;
                if (q[t.seq + 1].Value.ToString() == g.field_marks[0].left)
                {
                        fields=new List<List<Token>>();
                }
            }
        }
    }

    #region Classes

    public class BinaryExpression
    {
        public BinaryExpression left, right;
        public string Operator;
    }

    public class Grammar
    {
        public string head;
        public List<FieldMark> field_marks;

        public Grammar(string head, List<FieldMark> field_marks)
        {
            this.head = head;
            this.field_marks = field_marks;
        }

        private static readonly FieldMark[] Marks =
        {
            new FieldMark("(",")"),
            new FieldMark("[","]"),
            new FieldMark("<",">"),
            new FieldMark("{","}"),
        };
    }

    public class FieldMark
    {
        public string left, right;
        public FieldMark(string l, string r)
        {
            left = l;
            right = r;
        }
    }

    #endregion

}
