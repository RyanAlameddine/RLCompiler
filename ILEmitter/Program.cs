using RLParser;
using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEmitter
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = RLParser.Program.GetPath();
            while (true)
            {
                string code = File.ReadAllText(path);
                Console.Clear();


                Action<CompileException, Context> onError = (e, l) =>
                {
                    Console.WriteLine(e);
                    Console.WriteLine("On line " + l.Lines.End);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                };
                Context program = RLParser.RLParser.Parse(code, onError);
                SymbolTable table = RlTypeChecker.TypeCheck(program, onError);

                var fileGenerator = new FileEvaluator(Path.GetFileNameWithoutExtension(path), program as FileContext);
                fileGenerator.GenerateAssembly(table);

                program.ConsolePrint();

                Console.ReadKey();
            }


            //string aName = "Practice";

            //var assemblyName = new AssemblyName(aName);
            //var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            //var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule", "HelloWorld.exe");
            //var typeBuilder = moduleBuilder.DefineType("Program");
            //var methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Static);
            //var ilGenerator = methodBuilder.GetILGenerator();

            //ilGenerator.Emit(OpCodes.Ldstr, "Hello, World");
            //ilGenerator.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string[]) }));
            //ilGenerator.Emit(OpCodes.Ret);
            //typeBuilder.CreateType();

            //assemblyBuilder.SetEntryPoint(methodBuilder);
            //assemblyBuilder.Save("HelloWorld.exe");
        }
    }
}
