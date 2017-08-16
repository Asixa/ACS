using System;
using System.Collections.Generic;
using System.Linq;
using ACS.ACS_Lexer;


namespace ACS.ACS_Parser.Parser
{
    internal class ParserEngine
    {
        public List<Token> q;
        #region Base

        public ParserEngine(List<Token> q, bool run)
        {
            this.q = q;
            if (run) Begin();
        }

        public void Begin()
        {
            Console.WriteLine(DataBase.c.Match(this.q).ToString());
        }

        #endregion

        private void Match_grammar(Token t)
        {
            var LoopStart=new Element("tag","loopstart");
            var LoopEnd=new Element("tag","loopend");

            var MathValue=new Element("int",null,new List<Element>
            {
                new Element("float"),
                new Element("Identifier")
            });

            var Operator = new Element("string","+",new List<Element>
            {
                new Element("string","-"),
                new Element("string","*"),
                new Element("string","/"),
                new Element("string","=="),
                new Element("string","<"),
                new Element("string",">"),
                new Element("string",">="),
                new Element("string","<="),
                new Element("string","!=")
            });

            var Arithmetic=new Expression(new List<Element>
            {
                LoopStart,
                MathValue,
                Operator,
                LoopEnd,
                MathValue
            });
            var CodeField = new Expression(new List<Element>());
 
            var _if=new Expression(new List<Element>
           {
               new Element("string","if"),
               new Element("string","("),
               new Element("expression",Arithmetic),
               new Element("string",")"),
               new Element("string","{"),
               new Element("expression",CodeField),
               new Element("string","}"),
           });
        }
    }

    #region Classes

    public class BinaryExpression
    {
        public BinaryExpression left, right;
        public string Operator;
    }

    public class Element
    {
        public string type;
        public object value;

        public Element(string type,object value=null, List<Element> e=null)
        {
            this.type = type;
            this.value = value;
        }
    }

    public class Expression
    {
        public List<Element> elements;
        public List<Token> q;
        public Expression(List<Element> e)
        {
            elements = e;
        }

        public bool Match(List<Token> Q) //输入一句话 大概是 int a = 1 + 2 + 3;
        {                                                
            this.q = Q;
            
            for (var i = 0; i < q.Count; i++) //调整 token的seq
            {
                this.q[i].seq = i;
            }
            var now_token=0;
            for (var i = 0; i < elements.Count; i++)
            {
                if (elements[i].type == "tag")
                {
                    if (elements[i].value.ToString() == "loopstart")
                    {

                    }
                    if (elements[i].value.ToString() == "loopend")
                    {

                    }
                }
                else
                {
                    if (!Match_token_Element(q[now_token], elements[i]))
                    {
                        return false;
                    }
                    now_token++;
                }
            }
            return true;
            ;
        }

        private bool Match_token_Element(Token t, Element e)
        {
           // seq = t.seq;
            if (e.type != "tag")
            {
                if (e.type != "expression") //如果元素不是表达式
                {
                    if (t.type == e.type)
                    {
                        return true; //如果匹配到本值则返回真
                    }
                    else //如果不与本值匹配
                    {
                        //if (e.replaces != null) //如果有替换元素
                        //{
                        //    if (e.replaces.Any(t1 => Match_token_Element(t, t1)))
                        //    {
                        //        return true; //如果匹配到替换值则返回真
                        //    }
                        //}
                    }
                }
                else
                {
                    var exp = (Expression) e.value;
                    
                    return exp.Match(new List<Token>(SplitTokenArray(q.ToArray(), t.seq, q.Count)));

                }
            }
            else
            {
                if (e.value.ToString() == "loopstart")
                {
                    
                }
                if (e.value.ToString() == "loopend")
                {

                }
            }

            return false;//出错
        }

        public static Token[] SplitTokenArray(Token[] Source, int StartIndex, int EndIndex)
        {
            try
            {
                var result = new Token[EndIndex - StartIndex + 1];
                for (var i = 0; i <= EndIndex - StartIndex; i++) result[i] = Source[i + StartIndex];
                return result;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    #endregion


    #region  表达式定义
 
    public class DataBase
    {
        #region 元素
        public static Element LoopStart = new Element("tag", "loopstart");
        public static Element LoopEnd = new Element("tag", "loopend");

        public static Element MathValue = new Element("int", null, new List<Element>
        {
            new Element("float"),
            new Element("identifier")
        });

        public static Element Operator = new Element("operator", "+", new List<Element>
        {
            new Element("operator","-"),
            new Element("operator","*"),
            new Element("operator","/"),
            new Element("operator","=="),
            new Element("operator","<"),
            new Element("operator",">"),
            new Element("operator",">="),
            new Element("operator","<="),
            new Element("operator","!=")
        });

        #endregion


        public static Expression Arithmetic = new Expression(new List<Element>
        {
            //LoopStart,
            //MathValue,
            //Operator,
            //LoopEnd,
            //MathValue

            MathValue,
            Operator,
            MathValue,
            Operator,
            MathValue
        });

        public static Expression c=new Expression(new List<Element>()
        {
            new Element("expression",Arithmetic),
            new Element("operator",";")
        });

        public static Expression CodeField = new Expression(new List<Element>());

        public static Expression _if = new Expression(new List<Element>
        {
            new Element("string","if"),
            new Element("string","("),
            new Element("expression",Arithmetic),
            new Element("string",")"),
            new Element("string","{"),
            new Element("expression",CodeField),
            new Element("string","}"),
        });
    }
    

    #endregion
}
