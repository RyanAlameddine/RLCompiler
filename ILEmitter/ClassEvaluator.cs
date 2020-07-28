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
    class ClassEvaluator
    {
        readonly FileEvaluator parent;
        readonly ClassHeaderContext header;

        public TypeBuilder TypeBuilder { get; private set; }
        public Dictionary<string, MethodBuilder> Methods { get; private set; }
        public ConstructorBuilder Constructor { get; private set; }

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

        public void LoadClassMembers(SymbolTable privatememberTable)
        {
            foreach(var child in header.Children)
            {
                if(child is FunctionHeaderContext func)
                {
                    MethodAttributes methodAttributes;
                    if (func.AccessModifier == AccessModifiers.Public)  methodAttributes = MethodAttributes.Public;
                    else methodAttributes = MethodAttributes.Private;
                    var paramTypes = privatememberTable.GetFunction(func.Name, func).paramTypes.Select((x) => parent.FindType(x)).ToArray();
                    //constructor
                    if (func.Name == header.Name)
                    {
                        var constructorBuilder = TypeBuilder.DefineConstructor(methodAttributes, CallingConventions.Standard, paramTypes);
                        Constructor = constructorBuilder;
                    }
                    else 
                    {
                        var methodBuilder = TypeBuilder.DefineMethod(func.Name, methodAttributes, parent.FindType(func.ReturnType), paramTypes);
                        Methods.Add(func.Name, methodBuilder);
                        if(func.Name == "Main")
                        {
                            EntryPoint = methodBuilder;
                        }
                    }
                }
                else if(child is VariableDefinitionContext var)
                {
                    
                }
            }
        }
    }
}
