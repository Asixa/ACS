using System;
using ACS.Parser;
using ACS.Variable_Register;

namespace ACS
{
    internal class Program
    {
        public static bool debug=false;
        private static void Main(string[] args)
         {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

             Register.Init();
             var lexer_result = Lexer.Lexer._Main();
             double lexer_timespan=0;
             if (debug)
             {
                 lexer_timespan = watch.Elapsed.TotalMilliseconds;
                 Console.WriteLine("词法分析器执行时间：{0}(毫秒)", lexer_timespan);
             }
             var GrammerTree=Parser.Parser.Match(lexer_result);
             if (debug)
             {
                 Console.WriteLine("语法法分析器执行时间：{0}(毫秒)", watch.Elapsed.TotalMilliseconds - lexer_timespan);
                 Console.WriteLine();
                 Console.WriteLine("------------下面是程序输出内容-----------");
             }
             Interpreter.Interpreter.Run(GrammerTree);
            if(debug)
             Console.WriteLine("----------------------------------------");
            watch.Stop();
            var timespan = watch.Elapsed;
            
            Console.WriteLine("完全执行时间：{0}(毫秒)", timespan.TotalMilliseconds);

            Console.ReadKey();
        }
    }
}
