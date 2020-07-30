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
        public readonly IEnumerable<Type> allTypes;
        public Dictionary<string, ConstructorEvaluator> Constructors { get; private set; } = new Dictionary<string, ConstructorEvaluator>();


        public FileEvaluator(string name, FileContext program)
        {
            this.name = name;
            this.program = program;
            allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes());
        }

        public void GenerateAssembly(SymbolTable table)
        {
            var assemblyName = new AssemblyName(name);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

            var nmspace = program.Children.Where((c) => c.GetType() == typeof(NamespaceContext)).Cast<NamespaceContext>().First().Namespace;
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(nmspace, name + ".exe");

            LoadFileContext(table, moduleBuilder, out MethodInfo ep, out ConstructorInfo entryConstructor);


            if (ep == null) throw new CompileException("No entrypoint Main found!");
            var entryPoint = EntryPointGenerator.Generate(moduleBuilder, entryConstructor, ep);
            assemblyBuilder.SetEntryPoint(entryPoint);

            assemblyBuilder.Save(name + ".exe");
        }

        private void LoadFileContext(SymbolTable table, ModuleBuilder moduleBuilder, out MethodInfo entryPoint, out ConstructorInfo entryConstructor)
        {
            entryPoint = null;
            entryConstructor = null;
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
                child.Value.LoadClassMembers(table.Children[child.Key].Children["classMembers"]);
                if (child.Value.EntryPoint != null)
                {
                    entryPoint = child.Value.EntryPoint;
                    entryConstructor = child.Value.Constructor.ConstructorBuilder;
                }
            }

            //Create method bodies and construct final Types
            foreach (var child in classes)
            {
                child.Value.CreateClass(table.Children[child.Key].Children["classMembers"]);
            }
        }

        public Type FindType(string name)
        {
            if (classes.ContainsKey(name)) return classes[name].TypeBuilder;


            //TODO HANDLE LISTS
            //while(name[0] == '[')

            switch (name)
            {
                case "int":
                    name = "Int32";
                    break;
                case "bool":
                    name = "Boolean";
                    break;
                case "string":
                    name = "String";
                    break;
                case "void":
                    name = "Void";
                    break;
            }

            return allTypes.Where((t) => t.Name == name).FirstOrDefault();
        }
    }
}
