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
    abstract class MethodEvaluatorBase
    {
        public FunctionHeaderContext Header { get; }

        private readonly Dictionary<string, LocalBuilder> localVariables = new Dictionary<string, LocalBuilder>();
        private readonly List<string> paramVariables = new List<string>();
        public readonly ClassEvaluator parent;

        public MethodEvaluatorBase(FunctionHeaderContext header, ClassEvaluator parent)
        {
            Header = header;
            this.parent = parent;

            paramVariables.Add("this");
            paramVariables.AddRange(header.ParamNames);
        }

        protected abstract ILGenerator GetGenerator();

        public void GenerateIL(SymbolTable functionTable)
        {
            var generator = GetGenerator();

            bool isRet = false;
            foreach (var context in Header.Children.Last.Value.Children)
            {
                if (context is VariableDefinitionContext var)
                {
                    GenerateLocalVariable(var.Name, var.Type, generator);
                }
                if (context is ExpressionContext || context is ReturnContext)
                {
                    isRet = ExpressionGenerator.Generate(context, functionTable, generator, localVariables, this);
                }
            }
            if (!isRet)
            {
                generator.Emit(OpCodes.Ret);
            }
        }

        public void GenerateLocalVariable(string name, string type, ILGenerator generator)
        {
            Type t = parent.parent.FindType(type);
            var localBuilder = generator.DeclareLocal(t);
            localVariables.Add(name, localBuilder);
        }

        public void GenerateVariableLoad(ILGenerator generator, string name)
        {
            if (localVariables.ContainsKey(name))
            {
                var index = localVariables[name].LocalIndex;
                generator.Emit(OpCodes.Ldloc, index);
            }
            else if (paramVariables.Contains(name))
            {
                generator.Emit(OpCodes.Ldarg, paramVariables.IndexOf(name));
            }
            else if (parent.Fields.ContainsKey(name))
            {
                var field = parent.Fields[name];
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
            }
            else
            {
                throw new CompileException("Failed to find variable with name " + name);
            }
        }

        public void GenerateVariableStore(ILGenerator generator, string name, Action GenerateValue)
        {
            if (localVariables.ContainsKey(name))
            {
                GenerateValue();
                generator.Emit(OpCodes.Stloc, localVariables[name].LocalIndex);
            }
            else if (paramVariables.Contains(name))
            {
                GenerateValue();
                generator.Emit(OpCodes.Starg, paramVariables.IndexOf(name));
            }
            else if (parent.Fields.ContainsKey(name))
            {
                var field = parent.Fields[name];
                generator.Emit(OpCodes.Ldarg_0);
                GenerateValue();
                generator.Emit(OpCodes.Stfld, field);
            }
            else
            {
                throw new CompileException("Failed to find variable with name " + name);
            }
        }

        public MethodInfo GetMethodInfo(string name, SymbolTable table)
        {
            if (parent.Methods.ContainsKey(name)) return parent.Methods[name].MethodBuilder;
            
            var current = table;
            do
            {
                if (current.MethodInfos.ContainsKey(name)) 
                    return current.MethodInfos[name];
                current = current.Parent;
            } while (current != null);

            throw new CompileException($"Could not find function with name {name}");

        }
    }
}
