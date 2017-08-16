using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACS.ACS_Lexer;
using Microsoft.Win32;

namespace ACS.ACS_Parser.BNF
{
    class BNF
    {
        public static void Match(List<Token> _queue)
        {
            var input = new List<Token>();
            //这里是把语句一句一句扔给匹配的
            foreach (var item in _queue)
            {
                if (item == null) break;
                switch (item.Value.ToString())
                {
                    case "{":
                    case "}":
                    case ";":
                        input.Add(item);
                        var p= MatchRules(input);
                        Console.WriteLine(p.matched);
                        input = new List<Token>();
                        continue;
                    default:
                        break;
                }
                input.Add(item);
            }
        }
        private static Processer MatchRules(List<Token> _input)
        {
            #region OLD
            //var parser = new Parser(_input);
            //parser = parser.rule("if").rule("(").or("identifier",x).or("float",x).or("int",x).or("string",x);
            //parser.is_or_matched = false;
            //parser = parser.or("==").or("<=").or(">=");
            //parser.is_or_matched = false;
            //parser = parser.or("identifier", x).or("float", x).or("int", x).or("string", x).rule(")").rule("{");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("}");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("identifier",x).rule("=").rule("int",x).or("float",x).or("identifier",x).LoopStart().or("+").or("-").or("*").or("/").rule("int", x).or("float", x).or("identifier", x).LoopEnd().rule(";");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("identifier",x).rule("=").rule("int",x).LoopStart().or("+").or("-").or("*").or("/").rule("float",x).LoopEnd().rule(";");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("identifier",x).rule("=").rule("identifier",x).LoopStart().or("+").or("-").or("*").or("/").rule("identifier",x).LoopEnd().rule(";");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("identifier",x).rule("=").rule("int",x).rule(";");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("identifier",x).rule("=").rule("float",x).rule(";");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("print").rule("(").or("identifier",x).or("int",x).or("string",x).or("float",x).rule(")").rule(";");
            //if (parser.is_matched) return parser;
            //parser = new Parser(_input);
            //parser = parser.rule("input").rule("(").rule("identifier",x).rule(")").rule(";");
            //if (parser.is_matched) return parser;
            //parser = new Parser(_input);
            //parser = parser.rule("[").rule("identifier",x).rule("]");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("goto").rule("identifier",x).rule(";");
            //if (parser.is_matched) return parser;

            //parser = new Parser(_input);
            //parser = parser.rule("goto").rule("identifier", x).rule(";");
            //if (parser.is_matched) return parser;


            #endregion
            var Processer = new Processer(_input);
            Processer = Processer.Next(Indentifier).LoopStart().Next(new Element("+")).LoopEnd().Next(END);

            return Processer;
            //在这里进行花式报错
        }


        public static Element Indentifier=new Element("identifier",ElementType.Token);
        public static Element END = new Element(";");

        public static Element LoopStart=new Element("loopstart",ElementType.tag);
        public static Element LoopEnd = new Element("loopend", ElementType.tag);

        public static Expression Math=new Expression(new List<Element>
        {
            LoopStart,
            LoopEnd

        });

    }



    public class Parser
    {
        public List<Token> queue;
        public List<Token> new_queue = new List<Token>();
        int now_count;
        bool in_loop;
        bool in_region;

        public   List<string> command = new List<string>();
        public   List<object> parameter = new List<object>();
        

        Parser parser;
        public bool is_matched = true; //表示整个语句是否匹配成功
        public bool is_or_matched; //表示or语句是否匹配成功
        bool is_region_matched = true;

        public Parser(List<Token> _queue)
        {
            queue = _queue;
        }

        void MatchSuccess()
        {
            new_queue.Add(queue[now_count]);
            now_count++;
            is_matched = true;
        }
        void AddCommand(string c, object s)
        {
            if (in_loop)
            {
                command.Add(c);
                parameter.Add(s);
            }
        }
        void AddCommand(string c)
        {
            if (in_loop)
            {
                command.Add(c);
            }
        }


        public Parser LoopStart()
        {
            AddCommand("LoopStart");
            in_loop = true;
            return this;
        }

        //这个函数会生成一个和现在这个数值一致的parser实例，并把先前记录的 匹配动作和参数 循环执行，直到没有新的匹配，循环结束，返回带有匹配结果的parser对象
        public Parser LoopEnd()
        {
            AddCommand("LoopEnd");
            parser = new Parser(queue)
            {
                now_count = now_count,
                new_queue = new_queue,
                is_matched = is_matched,
                is_or_matched = is_or_matched,
                in_loop = true,
                in_region = in_region,
                is_region_matched = is_region_matched
            };
            Parser _parser = new Parser(parser.queue)
            {
                now_count = parser.now_count,
                new_queue = parser.new_queue,
                is_matched = parser.is_matched,
                is_or_matched = parser.is_or_matched,
                in_loop = true,
                in_region = in_region,
                is_region_matched = is_region_matched
            };

            while (true)
            {
                if (!parser.is_matched)
                {
                    _parser.in_loop = false;
                    return _parser;
                }
                _parser = new Parser(parser.queue)
                {
                    now_count = parser.now_count,
                    new_queue = parser.new_queue,
                    is_matched = parser.is_matched,
                    is_or_matched = parser.is_or_matched,
                    in_loop = true
                };
                for (var i = 0; i < command.Count; i++)
                {
                    switch (command[i])
                    {
                        case "rule":
                            {
                                parser = parser.rule(parameter[i].ToString());
                                break;
                            }
                        case "or":
                            {
                                parser = parser.or(parameter[i].ToString());
                                break;
                            }
                        case "LoopStart":
                            {
                                parser = parser.LoopStart();
                                break;
                            }
                        case "LoopEnd":
                            {
                                parser = parser.LoopEnd();
                                break;
                            }
                        case "RegionStart":
                            {
                                parser = parser.RegionStart();
                                break;
                            }
                        case "RegionEnd":
                            {
                                parser = parser.RegionEnd();
                                break;
                            }

                        default:
                            break;
                    }
                }
            }
        }

        public Parser RegionStart()
        {
            AddCommand("RegionStart");
            in_region = true;
            is_region_matched = true;
            return this;
        }

        public Parser RegionEnd()
        {
            AddCommand("RegionEnd");
            if (is_matched)
                is_matched = is_region_matched;
            in_region = false;
            is_region_matched = true;
            return this;
        }

        public Parser rule(string v, bool is_string = false)
        {
            if (!is_string)
            {
                is_or_matched = false;
                if (!is_matched) return this;
                if (queue.Count == now_count) return this;
                AddCommand("rule", v);
                
                if (queue[now_count].Value.ToString() == ";" && v != ";")
                {
              
                    is_matched = false;
                    return this;
                }
                if (queue[now_count].Value.ToString() == v)
                {
                    Console.WriteLine("rule-文字-值: " + queue[now_count].Value + "     " + queue[now_count].Value + "匹配到" + v);
                    MatchSuccess();
                    return this;
                }
                else if (queue[now_count].type == v && in_loop)
                {
                    Console.WriteLine("rule-文字-类型: " + queue[now_count].Value + "     " + queue[now_count].type + "匹配到" + v);
                    MatchSuccess();
                    return this;
                }
                if (in_region) is_region_matched = false;
                is_matched = false;
                return this;
            }
            else
            {
                is_or_matched = false;
                if (!is_matched) return this;
                if (queue.Count == now_count) return this;
                AddCommand("rule",v);
                if (queue[now_count].Value.ToString() == ";")
                {
                    is_matched = false;
                    return this;
                }
                if (queue[now_count].type == v)
                {
                    Console.WriteLine("rule-非文字-类型: "+ queue[now_count].Value+"     " + queue[now_count].type + "匹配到" + v);
                    MatchSuccess();
                    return this;
                }
                if (in_region) is_region_matched = false;
                is_matched = false;
                return this;
            }
        }
        public Parser or(string v,bool is_string=false)
        {
            if (!is_string)
            {
                //Console.WriteLine(is_matched);
                AddCommand("or", v);
                if (queue.Count == now_count) return this;
                if (is_or_matched) return this;
                is_matched = false;

                if (queue[now_count].Value.ToString() == ";")
                {
                    is_matched = false;
                    return this;
                }
                if (queue[now_count].Value.ToString() == v)
                {
                    Console.WriteLine("or-非文字-值: " + queue[now_count].Value + "     " + queue[now_count].type + "匹配到" + v);
                    MatchSuccess();
                    is_or_matched = true;
                    return this;
                }
                if (in_region) is_region_matched = false;
                is_matched = false;
                return this;
            }
            else
            {
                AddCommand("or", v);
                if (queue.Count == now_count) return this;
                if (is_or_matched) return this;
                is_matched = false;
               
                if (queue[now_count].Value.ToString() == ";")
                {
                    is_matched = false;
                    return this;
                }
                if (queue[now_count].type== v)
                {
                    Console.WriteLine("or-非文字-类型: " + queue[now_count].Value + "     " + queue[now_count].type + "匹配到" + v);
                    MatchSuccess();
                    is_or_matched = true;
                    return this;
                }
                if (in_region) is_region_matched = false;
                is_matched = false;
                return this;
            }
        }
    }
    

    public class Processer
    {
        private List<Token> q; //这个是当前的 标记列表，应是一条代码
        public int now_id;      //当前 处理的 标记 的序号
        public bool matched = false;    //当前模块是否匹配成功
        public Processer(List<Token> queue){q = queue;} //构造函数,用于声明初始模块时 赋值标记列表

        public Processer Next(Element e)//这个函数是设置下一个新元素
        {
            switch (e.type)
            {
                case ElementType.String:
                    if (q[now_id].Value.ToString() == e.value.ToString()) //如果 标记 的值等于 声明的内容
                    {
                        
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
                        MatchSuccessed();   //匹配成功
                    }
                    else
                    {
                        MatchFailed();      //匹配失败
                    }
                    break;
                case ElementType.Expression:
                    {
                
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return this;        //返回
        }

        public Processer Or(string v, bool is_type) //这个函数是设置在现在的元素没有匹配到的时候，提供可替换的 元素
        {   
            if (matched) return this; //如果上一个提供的元素已经匹配到，则不执行
            if (!is_type)       //如果声明的不是[元素类型]
            {
                if (q[now_id].Value.ToString() == v) //如果 标记 的值等于 声明的内容
                {
                    MatchSuccessed();   //匹配成功
                }
                else
                {
                    MatchFailed();      //匹配失败
                }
            }
            else                //如果声明的是[元素类型]
            {
                if (q[now_id].type == v)            //如果 标记 的类型等于 声明的内容
                {
                    MatchSuccessed();   //匹配成功
                }
                else
                {
                    MatchFailed();      //匹配失败
                }
            }
            return this;    //返回
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

        public Processer LoopStart()
        {
            return this;
        }

        public Processer LoopEnd()
        {
            return this;
        }
    }

    public enum ElementType //这个枚举类型表示 元素的类型
    {
        String,//普通的字符串
        Token, //一类token
        Expression, //表达式
        tag
    }

    public class Element //这个类 代表一个单独的元素
    {
        public ElementType type;  //这个元素的类型
        public object value; //这个元素的值

        public Element(object value, ElementType type = ElementType.String) //构造函数
        {
            this.type = type;
            this.value = value;
        }
    }

    public class Expression
    {
        public List<Element> elements;
        public Expression(List<Element> e)
        {
            elements = e;
        }
    }
}
