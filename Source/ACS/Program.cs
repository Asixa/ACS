using System;
using ACS.Parser;
using ACS.Variable_Register;

namespace ACS
{
    internal class Program
    {
        private static void Main(string[] args)
         {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();  //开始监视代码运行时间
            //************************
   
             Register.Init();
             var lexer_result = Lexer.Lexer._Main();

             var lexer_timespan = watch.Elapsed.TotalMilliseconds;
             //Console.WriteLine("词法分析器执行时间：{0}(毫秒)", lexer_timespan);

             Parser.Parser.Match(lexer_result);
            // Console.WriteLine("语法法分析器执行时间：{0}(毫秒)", watch.Elapsed.TotalMilliseconds-lexer_timespan);
            //************************
            watch.Stop();
            var timespan = watch.Elapsed;
            Console.WriteLine("-----------------------");
            Console.WriteLine("完全执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
            Console.ReadKey();
        }
    }
}
