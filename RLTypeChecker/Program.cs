using RLParser;
using System;
using System.IO;

namespace RLTypeChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file name (small, SyntaxDefinition, test)");
            string path = @$"C:\Users\rhala\Code\RLCompiler\{Console.ReadLine()}.rl";
            while (true)
            {
                string code = File.ReadAllText(path);
                Console.Clear();


                Action<CompileException, Context> onError = (e, l) =>
                {
                    Console.WriteLine(e);
                    Console.WriteLine("On line " + l.Lines.End.Value);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                };
                Context program = RLParser.RLParser.Parse(code, onError);
                SymbolTable table = RlTypeChecker.TypeCheck(program, onError);
                program.ConsolePrint();

                Console.ReadKey();
            }
        }
    }
}
