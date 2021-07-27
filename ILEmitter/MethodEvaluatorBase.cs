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

namespace RLCodeGenerator
{
    abstract class MethodEvaluatorBase
    {
        public FunctionHeaderContext Header { get; }

        private readonly Dictionary<string, LocalBuilder> localVariables = new Dictionary<string, LocalBuilder>();
        private readonly Dictionary<string, (int index, Type type)> paramVariables = new Dictionary<string, (int index, Type type)>();
        public readonly ClassEvaluator parent;
        private readonly ExpressionGenerator expressionGenerator = new ExpressionGenerator();

        public MethodEvaluatorBase(FunctionHeaderContext header, ClassEvaluator parent)
        {
            Header = header;
            this.parent = parent;
        }

        protected abstract ILGenerator GetGenerator();

        private void RegisterParams()
        {
            paramVariables.Add("this", (0, parent.TypeBuilder));

            int i = 0;
            foreach (var param in Header.ParamNames.Zip(Header.ParamTypes, (name, type) => (name, type)))
            {
                i++;
                paramVariables.Add(param.name, (i, parent.parent.FindType(param.type)));
            }
        }

        public void GenerateIL(SymbolTable functionTable)
        {
            RegisterParams();

            var generator = GetGenerator();

            bool isRet = false;
            foreach (var context in Header.Children.Last.Value.Children)
            {
                isRet = GenerateStatement(context, generator, functionTable);
            }
            if (!isRet)
            {
                generator.Emit(OpCodes.Ret);
            }
        }

        public bool GenerateStatement(Context context, ILGenerator generator, SymbolTable functionTable)
        {
            bool isRet = false;
            if (context is VariableDefinitionContext var)
            {
                GenerateLocalVariable(var.Name, var.Type, generator);
            }
            if (context is ExpressionContext || context is ReturnContext)
            {
                isRet = expressionGenerator.Generate(context, functionTable, generator, localVariables, this);
            }
            return isRet;
        }

        public void GenerateLocalVariable(string name, string type, ILGenerator generator)
        {
            Type t = parent.parent.FindType(type);
            var localBuilder = generator.DeclareLocal(t);
            localVariables.Add(name, localBuilder);
        }

        public Type GenerateVariableLoad(ILGenerator generator, string name, bool loadAddress)
        {
            if (localVariables.ContainsKey(name))
            {
                var var = localVariables[name];
                generator.Emit(loadAddress && var.LocalType.IsValueType ? OpCodes.Ldloca : OpCodes.Ldloc, var.LocalIndex);
                return var.LocalType;
            }
            else if (paramVariables.TryGetValue(name, out var var))
            {
                generator.Emit(loadAddress && var.type.IsValueType ? OpCodes.Ldarga : OpCodes.Ldarg, var.index);
                return var.type;
            }
            else if (parent.Fields.ContainsKey(name))
            {
                var field = parent.Fields[name];
                generator.Emit(OpCodes.Ldarg_0);
                //maybe add a OpCodes.Ldarga
                generator.Emit(OpCodes.Ldfld, field);
                return field.FieldType;
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
            else if (paramVariables.TryGetValue(name, out var param))
            {
                GenerateValue();
                generator.Emit(OpCodes.Starg, param.index);
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

        public MethodInfo GetMethodInfo(string name, ILGenerator generator, SymbolTable table, Type type = null)
        {
            if (type == null)
            {
                //personal method
                if (parent.Methods.ContainsKey(name))
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    return parent.Methods[name].MethodBuilder;
                }
            }
            else
            {
                //method on type

                if(type is TypeBuilder)
                {
                    foreach(var c in parent.parent.Classes)
                    {
                        if(c.Key == type.Name)
                        {
                            return c.Value.Methods[name].MethodBuilder;
                        }
                    }
                }

                var method = type.GetMethods().Where((x) => x.Name == name).FirstOrDefault();
                if (method != null) return method;
            }
            
            var current = table;
            do
            {
                if (current.MethodInfos.ContainsKey(name)) 
                    return current.MethodInfos[name];
                current = current.Parent;
            } while (current != null);

            int dotIndex = name.IndexOf('.');

            if(dotIndex != -1)
            {
                string variable = name.Substring(0, dotIndex);
                string newFunction = name.Substring(dotIndex + 1, name.Length - (dotIndex + 1));
                var t = GenerateVariableLoad(generator, variable, true);
                return GetMethodInfo(newFunction, generator, table, t);
            }

            throw new CompileException($"Could not find function with name {name}");

        }
    }
}
