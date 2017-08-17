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
        private static bool Debug;
        public static void Analyze(Result result, string debug="")
        {

            if(Debug)Console.WriteLine(debug+"匹配到公式：" + result.name);
            var index=0;
            foreach (var t in result.commands)
            {
                index++;
                Analyze_Single_result(t);
                if (Debug)
                {
                    Console.WriteLine(debug + "第" + index + "个元素 属性:[" + t.type + "] 值:[" + t.value + "]");
                    if (t.type == "expression")
                    {
                        //Console.WriteLine(debug + "--"+ "表达式名称：" + ((Result)t.value).name);
                        Analyze((Result) t.value, debug + "--");
                    }
                }
            }
        }

        private static void Analyze_Single_result(SingleResult r)
        {
            
        }
    }
}
