using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACS_Lexer;

namespace ACS_Analyzer.BNF_Engine
{
    public static class BNF
    {
        public static void Match(List<Token> _queue)
        {
            List<Token> input = new List<Token>();
            //这里是把语句一句一句扔给匹配的
            foreach (Token item in _queue)
            { 
                if (item == null) break;
                if (item.GetValue() == ";")
                {
                    input.Add(item);
                    MatchRules(input);
                    input = new List<Token>();
                    continue;
                }
                if (item.GetValue() == "{")
                {
                    input.Add(item);
                    MatchRules(input);
                    input = new List<Token>();
                    continue;
                }
                if (item.GetValue() == "}")
                {
                    input.Add(item);
                    MatchRules(input);
                    input = new List<Token>();
                    continue;
                }
                input.Add(item);
            }
            
        }

        static void MatchRules(List<Token> _input)
        {
            Parser parser = new Parser(_input);

            #region 匹配
            parser = parser.rule("if").rule("(").or(Types.Identifier).or(Types.Float).or(Types.Number).or(Types.String);
            parser.is_or_matched = false;
            parser = parser.or("==").or("<=").or(">=");
            parser.is_or_matched = false;
            parser = parser.or(Types.Identifier).or(Types.Float).or(Types.Number).or(Types.String).rule(")").rule("{");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule("}");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule(Types.Identifier).rule("=").rule(Types.Number).LoopStart().or("+").or("-").or("*").or("/").rule(Types.Number).LoopEnd().rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule(Types.Identifier).rule("=").rule(Types.Number).LoopStart().or("+").or("-").or("*").or("/").rule(Types.Float).LoopEnd().rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule(Types.Identifier).rule("=").rule(Types.Identifier).LoopStart().or("+").or("-").or("*").or("/").rule(Types.Identifier).LoopEnd().rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule(Types.Identifier).rule("=").rule(Types.Number).rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule(Types.Identifier).rule("=").rule(Types.Float).rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule("print").rule("(").or(Types.Identifier).or(Types.Number).or(Types.String).or(Types.Float).rule(")").rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule("input").rule("(").rule(Types.Identifier).rule(")").rule(";");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule("[").rule(Types.Identifier).rule("]");
            if (parser.is_matched) return;
            parser = new Parser(_input);
            parser = parser.rule("goto").rule(Types.Identifier).rule(";");
            if (parser.is_matched) return;
            #endregion

            //在这里进行花式报错
        }
    }

    public class Parser
    {
        public List<Token> queue;
        public List<Token> new_queue = new List<Token>();
        int now_count;
        bool in_loop;
        bool in_region;
        List<string> command = new List<string>();
        List<string> parameter = new List<string>();

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
        void AddCommand(string c, string s)
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
                for (int i = 0; i < command.Count; i++)
                {
                    switch (command[i])
                    {
                        case "rule":
                            {
                                parser = parser.rule(parameter[i]);
                                break;
                            }
                        case "or":
                            {
                                parser = parser.or(parameter[i]);
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
        
        public Parser rule(string s)
        {
            is_or_matched = false;
            if (!is_matched) return this;
            if (queue.Count == now_count) return this;
            AddCommand("rule", s);
            if (queue[now_count].GetValue() == ";" && s != ";")
            {
                is_matched = false;
                return this;
            }
            if (queue[now_count].GetValue() == s)
            {
                MatchSuccess();
                return this;
            }
            else if (queue[now_count].GetTokenType().ToString() == s && in_loop)
            {
                MatchSuccess();
                return this;
            }
            if (in_region) is_region_matched = false;
            is_matched = false;
            return this;
        }
        public Parser rule(Types t)
        {
            is_or_matched = false;
            if (!is_matched) return this;
            if (queue.Count == now_count) return this;
            AddCommand("rule", t.ToString());
            if (queue[now_count].GetValue() == ";")
            {
                is_matched = false;
                return this;
            }
            if (queue[now_count].GetTokenType().ToString() == t.ToString())
            {
                MatchSuccess();
                return this;
            }
            if (in_region) is_region_matched = false;
            is_matched = false;
            return this;
        }
        public Parser or(string s)
        {
            //Console.WriteLine(is_matched);
            AddCommand("or", s);
            if (queue.Count == now_count) return this;
            if (is_or_matched) return this;
            is_matched = false;
            if (queue[now_count].GetValue() == ";")
            {
                is_matched = false;
                return this;
            }
            if (queue[now_count].GetValue() == s)
            {
                MatchSuccess();
                is_or_matched = true;
                return this;
            }
            if (in_region) is_region_matched = false;
            is_matched = false;
            return this;
        }
        public Parser or(Types t)
        {
            AddCommand("or", t.ToString());
            if (queue.Count == now_count) return this;
            if (is_or_matched) return this;
            is_matched = false;
            if (queue[now_count].GetValue() == ";")
            {
                is_matched = false;
                return this;
            }
            if (queue[now_count].GetTokenType().ToString() == t.ToString())
            {
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