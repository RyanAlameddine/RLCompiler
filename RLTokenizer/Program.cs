using System;
using System.IO;
using System.Threading;

namespace RLParser
{
    class Program
    {
        static void Main(string[] args)
        {

            //I realize that this is not a real tokenizer
            //its a tokenizer/parser hybrid

            Console.WriteLine("Enter file name (small, SyntaxDefinition, test)");
            string path = @$"C:\Users\rhala\Code\RLCompiler\{Console.ReadLine()}.rl";
            Console.WriteLine("Press 's' to start the refresh. Press any other key to refresh once");
            char c = Console.ReadKey().KeyChar;
            while (true)
            {
                string code = File.ReadAllText(path);
                Console.Clear();

                Context program = RLParser.Parse(code);
                program.ConsolePrint();

                if (Console.KeyAvailable)
                {
                    c = Console.ReadKey().KeyChar;
                }
                if (c == 's')
                {
                    Thread.Sleep(3000);
                }
                else
                {
                    c = Console.ReadKey().KeyChar;
                }
            }
        }
    }
}
