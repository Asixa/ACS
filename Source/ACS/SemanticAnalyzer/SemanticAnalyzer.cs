using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACS.Parser;

namespace ACS.SemanticAnalyzer
{
    internal class SemanticAnalyzer
    {
        public static bool Debug => Program.debug;

        public static void Analyze(Result result, string debug="")
        {
            if(!Debug)return;

            if(Debug)Console.WriteLine(debug+"匹配到公式：" + result.name);
            var index=0;
            //Interpreter.Interpreter.Run(result);
            if(Debug) foreach (var t in result.commands)
            {
                index++;
                if (Debug)
                {
                    Console.WriteLine(debug + "<" + index + "> 属性:[" + t.type + "] 值:[" + t.value + "]");

                    if (t.type == "expression")
                    {
                        if(Debug)Console.WriteLine(debug + "--"+ "表达式名称：" + ((Result)t.value).name);
                        Analyze((Result) t.value, debug + "--");
                    }
                    if (t.type == "field")
                    {
                        Analyze((Result)t.value, debug + "--");
                    }
                }
            }
        }

    }
}
