using System;
using ACS.ACS_Lexer;
using ACS.ACS_Parser;
using ACS.ACS_Parser.BNF;
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
            _main();
            //************************
            watch.Stop();
            var timespan = watch.Elapsed;
            Console.WriteLine("-----------------------");
            Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
            Console.ReadKey();
        }

        public static void _main()
        {
            var register = new Register();


            //var parser=new ParserEngine(Lexer._Main(), true);
            BNF.Match(ACS_Lexer.Lexer._Main());
            //  var parser = new ACS_Parser.Parser(ACS_Lexer.Lexer._Main());
            //  parser.Start();
            //   BNF.Match(ACS_Lexer.Lexer._Main());
        }


    }
}
