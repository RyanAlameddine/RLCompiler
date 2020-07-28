using RLParser;
using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEmitter
{
    class FileEvaluator
    {
        string name;
        FileContext program;
        
        Dictionary<string, ClassEvaluator> classes = new Dictionary<string, ClassEvaluator>();

        public FileEvaluator(string name, FileContext program)
        {
            this.name = name;
            this.program = program;
        }

        public void GenerateAssembly(SymbolTable table)
        {
            var assemblyName = new AssemblyName(name);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            LoadFileContext(table, assemblyBuilder, out MethodInfo entryPoint);

            assemblyBuilder.SetEntryPoint(entryPoint);
            assemblyBuilder.Save("HelloWorld.exe");
        }

        private void LoadFileContext(SymbolTable table, AssemblyBuilder assemblyBuilder, out MethodInfo entryPoint)
        {
            entryPoint = null;
            var nmspace = program.Children.Where((c) => c is NamespaceContext).Cast<NamespaceContext>().First().Namespace;

            var moduleBuilder = assemblyBuilder.DefineDynamicModule(nmspace, name + ".exe");

            //Load classes
            foreach(var child in program.Children)
            {
                if(child is ClassHeaderContext header)
                {
                    var eval = new ClassEvaluator(this, header);
                    eval.LoadClass(table.Children[header.Name], moduleBuilder);

                    classes.Add(header.Name, eval);
                }
            }

            //Load classes
            foreach (var child in classes)
            {
                child.Value.LoadClassMembers(table.Children[child.Key].Children["privatemembers"]);
                if (child.Value.EntryPoint != null) entryPoint = child.Value.EntryPoint;
            }
        }

        public Type FindType(string name)
        {
            if (classes.ContainsKey(name)) return classes[name].TypeBuilder;

            return Assembly.GetExecutingAssembly().GetType(name);
        }
    }
}
