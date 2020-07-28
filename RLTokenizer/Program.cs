using System;
using System.IO;
using System.Threading;

namespace RLParser
{
    public class Program
    {
        static void Main(string[] args)
        {

            //I realize that this is not a real tokenizer
            //its a tokenizer/parser hybrid
            string path = GetPath();
            while (true)
            {
                string code = File.ReadAllText(path);
                Console.Clear();

                Context program = RLParser.Parse(code, (e, l) =>
                {
                    Console.WriteLine(e);
                    Console.WriteLine("On line " + l.Lines.End);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                });
                program.ConsolePrint();

                Console.ReadKey();
            }
        }

        public static string GetPath()
        {
            Console.WriteLine("Enter file name (small, SyntaxDefinition, test, OOtest)");
            return $@"C:\Users\rhala\Code\RLCompiler\{Console.ReadLine()}.rl";
        }
    }
}
