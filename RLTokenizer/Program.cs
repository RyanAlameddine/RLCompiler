using System;
using System.IO;
using System.Threading;

namespace RLTokenizer
{
    class Program
    {
        static void Main(string[] args)
        {

            string path = @$"C:\Users\rhala\Code\SLCompiler\{Console.ReadLine()}.rl";
            while (true)
            {
                string code = File.ReadAllText(path);
                Console.Clear();

                Context program = RlTokenizer.Tokenize(code);
                program.ConsolePrint();


                Thread.Sleep(3000);
            }
        }
    }
}
