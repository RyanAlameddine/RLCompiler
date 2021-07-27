using RLParser;
using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RLCodeGenerator
{
    class ClassEvaluator
    {
        public readonly FileEvaluator parent;
        readonly ClassHeaderContext header;

        public TypeBuilder TypeBuilder { get; private set; }
        public Dictionary<string, FieldBuilder> Fields { get; private set; } = new Dictionary<string, FieldBuilder>();
        public Dictionary<string, MethodEvaluator> Methods { get; private set; } = new Dictionary<string, MethodEvaluator>();
        public ConstructorEvaluator Constructor { get; private set; }

        public MethodBuilder EntryPoint { get; private set; } = null;

        public ClassEvaluator(FileEvaluator parent, ClassHeaderContext header)
        {
            this.parent = parent;
            this.header = header;
        }

        public void LoadClass(SymbolTable classTable, ModuleBuilder moduleBuilder)
        {

            //var base = Assembly.GetExecutingAssembly().GetType(header.Base);
            //TODO SUPPORT INHERITANCE

            TypeBuilder = moduleBuilder.DefineType(header.Name);
        }

        public void LoadClassMembers(SymbolTable classMemberTable)
        {
            foreach(var child in header.Children.First.Next.Value.Children)
            {
                if (child is VariableDefinitionContext var)
                {
                    FieldAttributes fieldAttributes;
                    if (var.AccessModifier == AccessModifiers.Public) fieldAttributes = FieldAttributes.Public;
                    else fieldAttributes = FieldAttributes.Private;
                    var fieldBuilder = TypeBuilder.DefineField(var.Name, parent.FindType(var.Type), fieldAttributes);
                    Fields.Add(var.Name, fieldBuilder);
                }
                else if (child is FunctionHeaderContext func)
                {
                    MethodAttributes methodAttributes;
                    if (func.AccessModifier == AccessModifiers.Public)  methodAttributes = MethodAttributes.Public;
                    else methodAttributes = MethodAttributes.Private;
                    var paramTypes = classMemberTable.GetFunction(func.Name, func).paramTypes
                        .Where((x) => x != "void")
                        .Select((x) => parent.FindType(x)).ToArray();

                    //constructor
                    if (func.Name == header.Name)
                    {
                        var constructorBuilder = TypeBuilder.DefineConstructor(methodAttributes, CallingConventions.Standard, paramTypes);
                        Constructor = new ConstructorEvaluator(constructorBuilder, func, this);
                        parent.Constructors.Add(header.Name, Constructor);
                    }
                    else 
                    {
                        if (func.Name == "Main")
                        {
                            //entrypoint
                            var methodBuilder = TypeBuilder.DefineMethod(func.Name, methodAttributes, parent.FindType(func.ReturnType), paramTypes);
                            Methods.Add(func.Name, new MethodEvaluator(methodBuilder, func, this));
                            EntryPoint = methodBuilder;
                        }
                        else
                        {
                            var methodBuilder = TypeBuilder.DefineMethod(func.Name, methodAttributes, parent.FindType(func.ReturnType), paramTypes);
                            Methods.Add(func.Name, new MethodEvaluator(methodBuilder, func, this));
                        }
                    }
                }
            }
            if (Constructor == null)
            {
                Constructor = ConstructorEvaluator.GenerateConstructor(TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[0]), this, header.Name);
                parent.Constructors.Add(header.Name, Constructor);
            }
        }

        public void CreateClass(SymbolTable classMemberTable)
        {
            //public constructor
            if (classMemberTable.Parent.Children.ContainsKey(Constructor.Header.Name))
                Constructor.GenerateIL(classMemberTable.Parent.Children[Constructor.Header.Name]);
            //private constructor
            else
                Constructor.GenerateIL(classMemberTable.Children[Constructor.Header.Name]);


            foreach (var method in Methods.Values)
            {
                method.GenerateIL(classMemberTable.Children[method.Header.Name]);
            }

            TypeBuilder.CreateType();
        }

        public bool TryGetConstructor(string name, out ConstructorInfo constructor)
        {
            constructor = null;

            if (parent.Constructors.ContainsKey(name))
            {
                constructor = parent.Constructors[name].ConstructorBuilder;
                return true;
            }
            return false;
        }

        public ConstructorInfo GetConstructorInfo(string identifier, SymbolTable table)
        {
            foreach(var constructor in parent.Constructors)
            {
                if (constructor.Key == identifier) return constructor.Value.ConstructorBuilder;
            }
            throw new CompileException($"Cannot find constructor for {identifier}");
        }
    }
}
