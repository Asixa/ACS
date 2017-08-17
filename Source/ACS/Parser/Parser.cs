using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using ACS.Lexer;

namespace ACS.Parser
{
    #region 没必要动

    public enum ElementType //这个枚举类型表示 元素的类型
    {
        String,//普通的字符串
        Token, //一类token
        Expression, //表达式
        Looptag//循环标记 
    }

    public class Element //这个类 代表一个单独的元素
    {
        public ElementType type;  //这个元素的类型
        public object value; //这个元素的值
        public bool can_be_removed;
        public bool self_loop;

        public Element(object value, ElementType type = ElementType.String,bool can_be_removed=false,bool self_loop=false) //构造函数
        {
            this.type = type;
            this.value = value;
            this.can_be_removed = can_be_removed;
            this.self_loop = self_loop;
        }
    }

    public class ElementsGroup //这个类 用来存储若干个 元素，来作为可替换元素库
    {
        public List<Element> group; //这是元素数组

        public Element Element => group[0];// 取默认元素，就是元素数组里的第一个元素

        public ElementsGroup(List<Element> e) //构造函数
        {
            group = e;
        }
        public ElementsGroup(Element e) //构造函数
        {
            group=new List<Element>(); group.Add(e);
        }

        public ElementsGroup(object value, ElementType type = ElementType.String) //构造函数 重载，此重载可以直接声明默认元素
        {
            var e = new Element(value, type);
            group = new List<Element> { e };
        }

        //FIX ME 写的是如果匹配是表达式的话，会throw
        public bool Match(List<Token>q, int Token_id,out SingleResult Matched_type) //匹配，返回一个布尔变量 来表示 token 是否与元素组的其中之一匹配
        {
            var t = q[Token_id];
            Matched_type = new SingleResult();
            foreach (var t1 in group)
            {
                switch (t1.type)
                {
                    case ElementType.String:
                        if (t.Value.ToString() == t1.value.ToString())
                        {
                            Matched_type=new SingleResult("operator", t1.value.ToString());
                            return true;
                        }
                        break;
                    case ElementType.Token:
                        if (t.type == t1.value.ToString())
                        {
                            Matched_type = new SingleResult(t.type, t.Value);
                            return true;
                        }
                        break;
                    case ElementType.Expression:
                        throw new ArgumentOutOfRangeException();
                        var exp = (Expression) (t1.value);
                        var c=0;
                        Result r;
                        if (exp.Match(q, Token_id,out c,out r))
                        {
                            Matched_type = new SingleResult("expression", r);
                            return true;
                        }
                        break;
                        //throw new ArgumentOutOfRangeException();
                    case ElementType.Looptag:
                        throw new ArgumentOutOfRangeException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }

    }

    public class Expression //这个类表示一个表达式
    {
        public List<ElementsGroup> elements;
        public string name;
        public Expression(string name, List<ElementsGroup> e)
        {
            this.name = name;
            elements = e;
        }

        public bool Match(List<Token> q, int seq, out int count, out Result R)
        {
            R = new Result {name = name};
            count = 0;
            int elements_id = 0, token_id = seq;

            while (true) //Group 内循环
            {
                SingleResult match_result;
                if ((elements_id == elements.Count))//||(token_id==seq))
                {
                    count = token_id;
                    return true;
                }
                if (elements[elements_id].Element.type != ElementType.Looptag) //如果不是循环标记
                {
   
                    if (!elements[elements_id].Match(q, token_id, out match_result)) // 匹配元素组
                        {
                            
                            if (elements[elements_id].Element.can_be_removed)
                            {
                                elements_id++;
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                            
                        }
                    if (elements[elements_id].Element.self_loop)
                    {
                        SingleResult match_result2;
                        if (elements[elements_id].Match(q, token_id + 1, out match_result2))
                        {
                            elements_id--;
                        }
                    }
                        R.Add(match_result);
                }
                else //如果是循环标记
                {
                    var back_pos = (int)elements[elements_id].Element.value;
                    var loop_length = elements_id - back_pos;

                    var loop_is_ready = true;

                    int e_id=-1, t_id=-1;
                    for (var i = 0; i < loop_length; i++) //检测 循环段 和后面的 标记
                    {
                        e_id++;
                        t_id++;
                        if (elements[back_pos + e_id].Match(q, token_id + t_id, out match_result))
                        {
                            continue;
                        }
                        if (elements[back_pos + e_id].Element.can_be_removed)
                        {
                            t_id--;
                            continue;
                        }
                       
                        //跳出loop循环
                        loop_is_ready = false;
                        break;
                    }
                   // ADK.Print(loop_is_ready.ToString());

                    if (loop_is_ready)       //开始loop循环
                    {
                        elements_id = back_pos - 1;
                    }
                    else
                    {
                        if (elements_id == elements.Count - 1)
                        {
                            count = token_id;
                            return true;
                        }
                    }
                    token_id--;
                }

                //停止组循环检测


                
                elements_id++;
                token_id++;

            }
        }
    }

    public class Processer
    {
        public List<Token> q; //这个是当前的 标记列表，应是一条代码
        public int now_id;      //当前 处理的 标记 的序号
        public bool matched;    //当前模块是否匹配成功
        public bool error=false;
        public Result result=new Result();

        public Processer(List<Token> queue,string name){
            q = queue;
            result.name = name;
        } //构造函数,用于声明初始模块时 赋值标记列表
        public Processer()
        {
            error = true;
        } //构造函数,用于报错

        public Processer Next(Element e) => NextBase(e); //Next重载1
        public Processer Next(object value, ElementType type = ElementType.String) => NextBase(new Element(value,type));//Next重载2

        public Processer NextBase(Element e)//这个函数是设置下一个新元素
        {
            switch (e.type)
            {
                case ElementType.String:
                    if (q[now_id].Value.ToString() == e.value.ToString()) //如果 标记 的值等于 声明的内容
                    {
                        //result.Add("string", q[now_id].Value.ToString());
                        MatchSuccessed();   //匹配成功
                    }
                    else
                    {
                        MatchFailed();      //匹配失败
                    }
                    break;
                case ElementType.Token:
                    if (q[now_id].type ==e.value.ToString())            //如果 标记 的类型等于 声明的内容
                    {
                        result.Add(e.value.ToString(), q[now_id].Value);
                        MatchSuccessed();   //匹配成功
                    }
                    else
                    {
                        MatchFailed();      //匹配失败
                    }
                    break;
                case ElementType.Expression:
                    var exp = (Expression) e.value;
                    int count;
                    Result Exp_out;
                    if (exp.Match(q,now_id,out count,out Exp_out)) //q[now_id].type == e.value.ToString())            //如果 标记 的类型等于 声明的内容
                    {   
                        now_id = count - 1;
                        result.Add("expression", Exp_out);
                        MatchSuccessed();   //匹配成功
                    }
                    else
                    {
                        MatchFailed();      //匹配失败
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return this;        //返回
        }

        public Processer OrNextBase(Element e)//这个函数是设置下一个新元素
        {
            switch (e.type)
            {
                case ElementType.String:
                    if (q[now_id].Value.ToString() == e.value.ToString()) //如果 标记 的值等于 声明的内容
                    {
                        //result.Add("string", q[now_id].Value.ToString());
                        now_id++;        
                        matched = true;
                    }
                    else
                    {
                        matched = true; //匹配失败
                    }
                    break;
                case ElementType.Token:
                    if (q[now_id].type == e.value.ToString())            //如果 标记 的类型等于 声明的内容
                    {
                        result.Add(e.value.ToString(), q[now_id].Value);
                        now_id++;
                        matched = true;  //匹配成功
                    }
                    else
                    {
                        matched = true;     //匹配失败
                    }
                    break;
                case ElementType.Expression:
                    var exp = (Expression)e.value;
                    int count;
                    Result Exp_out;
                    if (exp.Match(q, now_id, out count, out Exp_out)) //q[now_id].type == e.value.ToString())            //如果 标记 的类型等于 声明的内容
                    {
                        now_id = count - 1;
                        result.Add("expression", Exp_out);
                        now_id++;
                        matched = true;//匹配成功
                    }
                    else
                    {
                        matched = true;     //匹配失败
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return this;        //返回
        }
        private void MatchSuccessed()   //这个函数是在单独标记匹配成功元素的时候调用的
        {
            now_id++;        //把当前序号更改为下一个
            matched = true;  //当前模块匹配成功
        }

        private void MatchFailed()      //这个函数是在单独标记没有匹配成功元素的时候调用的
        {
            matched = false; //当前模块匹配失败
        }

    } //处理器

    public class Result
    {
        public string name;
        public List<SingleResult> commands=new List<SingleResult>();

        public void Add(string type, object value)
        {
            commands.Add(new SingleResult(type,value));
        }

        public void Add(SingleResult r)
        {
            if(r==null)return;
            
            commands.Add(r);
        }
    }

    public class SingleResult
    {
        public string type;
        public object value;
        public SingleResult(){}

        public SingleResult(string type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }

    #endregion

    #region 垃圾桶
    //来自 Processer类
    //public Processer Or(string v, bool is_type) //这个函数是设置在现在的元素没有匹配到的时候，提供可替换的 元素
    //{
    //    if (matched) return this; //如果上一个提供的元素已经匹配到，则不执行
    //    if (!is_type)       //如果声明的不是[元素类型]
    //    {
    //        if (q[now_id].Value.ToString() == v) //如果 标记 的值等于 声明的内容
    //        {
    //            MatchSuccessed();   //匹配成功
    //        }
    //        else
    //        {
    //            MatchFailed();      //匹配失败
    //        }
    //    }
    //    else                //如果声明的是[元素类型]
    //    {
    //        if (q[now_id].type == v)            //如果 标记 的类型等于 声明的内容
    //        {
    //            MatchSuccessed();   //匹配成功
    //        }
    //        else
    //        {
    //            MatchFailed();      //匹配失败
    //        }
    //    }
    //    return this;    //返回
    //}
    #endregion

    internal class Parser : DataBase
    {
        public static void Match(List<Token> queue)
        {
            var input = new List<Token>();
            //这里是把语句一句一句扔给匹配的

            var lineCount = 0;

            foreach (var item in queue)
            {
                if (item == null) break;
                switch (item.Value.ToString())
                {
                    case "{":
                    case "}":
                    case ";":
                        lineCount++;
                        input.Add(item);
                        Handle(MatchRules(input),lineCount);
                        input = new List<Token>();
                        continue;
                    default:
                        break;
                }
                input.Add(item);
            }
        }

        public static void Handle(Processer p,int lineCount)
        {
            if (!p.error)
            {
                Console.WriteLine("第" + lineCount + "条匹配成功");
                SemanticAnalyzer.SemanticAnalyzer.Analyze(p.result, "--");
           //     Console.WriteLine();
            }
            else
            {
                ADK.Print("没有匹配到公式");
                Console.WriteLine();
            }
        }

    } //主类

    public class DataBase //数据库，主类的基类，存放所有元素，元素组，表达式声明
    {
        #region 元素
        public static Element Indentifier = new Element("identifier", ElementType.Token);
        public static Element Int = new Element("int", ElementType.Token);
        public static Element Float = new Element("float", ElementType.Token);
        public static Element END = new Element(";");
        
        #endregion

        #region 元素组
        public static ElementsGroup LoopEnd = new ElementsGroup("loopend", ElementType.Looptag);
        public static ElementsGroup MathElement = new ElementsGroup(new List<Element>
        {
            Indentifier,Int,Float
        });

        public static ElementsGroup MathOperator = new ElementsGroup(new List<Element>
        {
            new Element("="),
            new Element("+"),
            new Element("-"),
            new Element("*"),
            new Element("/"),
        });
        #endregion

        #region 表达式

        public static Expression Math = new Expression("Math",new List<ElementsGroup>
        {
            MathElement, 
            MathOperator, 
            new ElementsGroup(new Element("(",ElementType.String,true,true)),
            MathElement, 
            new ElementsGroup(new Element(")",ElementType.String,true,true)),
            new ElementsGroup(1, ElementType.Looptag),
        });


        #endregion

        public static Processer MatchRules(List<Token> _input)
        {
            Processer processer;
            processer = new Processer(_input, "math").Next(Math, ElementType.Expression).Next(END);
            if (processer.matched) return processer;

            processer = new Processer(_input, "print").Next("print").Next("(").Next(Math, ElementType.Expression).Next(")").Next(END); ;
            if (processer.matched) return processer;

            processer = new Processer(_input, "怕死了").Next("怕死了").Next("(").Next(Math, ElementType.Expression).Next(")").Next("<").Next("identifier", ElementType.Token).Next(">").Next(END); ;
            if (processer.matched) return processer;

            return new Processer();
            //在这里进行报错 没有匹配搭配对应语法
        }
    } 

}