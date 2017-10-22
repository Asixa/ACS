using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ACS.Lexer
{
    internal class Lexer
    {
        public static List<TokenDefinition> definitions = new List<TokenDefinition>
        {
            new TokenDefinition(MyType.NULL,""),
            new TokenDefinition(MyType.NULL,"(\\v*)"),
            new TokenDefinition(MyType.NULL,"(//.*)"),
            new TokenDefinition(MyType.Float,"([-]?[0-9]+[.][0-9]+)","Float"),
            new TokenDefinition(MyType.String,"(\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\")"),
            new TokenDefinition(MyType.Identifier,"([A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\|)"),
            new TokenDefinition(MyType.Int,"([-]?[0-9]+)","Int"),
            new TokenDefinition(MyType.Operator,"[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~]"),
        };

        public static string regex_pat
            =
            "("+
            "(//.*)" +
            "|([-]?[0-9]+[.][0-9]+)" +
            "|([0-9]+)" +
            "|(\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\")" + 
            "|[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\\|\\|" +
            "|[!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~]"+
            ")?";

        public static string line;
        public static FileStream file_stream;
        public static StreamReader stream_reader;
        private static MatchCollection _matches;
        private static int _line_number;
        static List<Token> queue = new List<Token>();

        public static List<Token> _Main()
        {
            
            //regex_pat = "("+definitions[2].regex;
            //for (var i = 3; i < definitions.Count; i++)
            //{
            //    regex_pat += "|" + definitions[i].regex;
            //}
            //regex_pat += ")?";

            file_stream = new FileStream(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Data/index.acs", FileMode.Open);
            stream_reader = new StreamReader(file_stream);
            while (ReadLine()){}
            queue.Add(null);
            //print_result(); //这里输出分析结果
            return queue;
        }

        private static void print_result()
        {
            foreach (var t in queue)
            {
                if (t != null)
                    Console.WriteLine("  "+t.Value+new string(" ".ToCharArray()[0],10-t.Value.ToString().Length)+"[" + t.type + "]");
            }
        }

        protected static bool ReadLine()
        {
            _line_number++;
            if (stream_reader.EndOfStream) return false;
            line = stream_reader.ReadLine();

            _matches = Regex.Matches(line, regex_pat);

            foreach (Match item in _matches)
            {
                AddToken(_line_number, queue.Count, item.Value);
            }
            return true;
        }

        protected static void AddToken(int line_number,int id, string v)
        {
            var token=new Token(0,0,new object(),"");
            for (var i = 1; i < definitions.Count; i++)
            {
                if (InGroup(i, v))
                {
                    if(i==1||i==2)return;

                    switch (definitions[i].value_type)
                    {
                        case "Float":
                            token = new Token(_line_number, queue.Count, float.Parse(v), definitions[i].name);
                            break;
                        case "Int":
                            token = new Token(_line_number, queue.Count, int.Parse(v), definitions[i].name);
                            break;
                        default:
                            token = new Token(_line_number, queue.Count, v, definitions[i].name);
                            break;
                    }
                }
                else
                {
                    //报错
                }
            }
            queue.Add(token);
        }
        
        private static bool InGroup(int i, string s)=>(Regex.Match(s, definitions[i].regex).Value == s);
    }

    public class TokenDefinition
    {
        public int name;
        public string regex,value_type;
        public TokenDefinition(int name,string regex,string value_type="String")
        {
            this.name = name;
            this.regex = regex;
            this.value_type = value_type;
        }
    }



}
