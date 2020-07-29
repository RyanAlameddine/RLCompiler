using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEmitter
{
    abstract class MethodEvaluatorBase
    {
        public FunctionHeaderContext Header { get; }

        public Dictionary<string, LocalBuilder> variables = new Dictionary<string, LocalBuilder>();
        public readonly ClassEvaluator parent;

        public MethodEvaluatorBase(FunctionHeaderContext header, ClassEvaluator parent)
        {
            Header = header;
            this.parent = parent;
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
                    Type t = parent.parent.FindType(var.Type);
                    var localBuilder = generator.DeclareLocal(t);
                    variables.Add(var.Name, localBuilder);
                }
                if (context is ExpressionContext expression)
                {
                    isRet = ExpressionGenerator.Generate(expression, functionTable, generator, variables, this);
                }
            }
            if (!isRet)
            {
                generator.Emit(OpCodes.Ret);
            }
        }


        public void GenerateVariableLoad(ILGenerator generator, string name)
        {
            var index = variables[name].LocalIndex;
            generator.Emit(OpCodes.Ldloc, index);
        }

        public void GenerateVariableStore(ILGenerator generator, string name)
        {
            generator.Emit(OpCodes.Stloc, variables[name].LocalIndex);
        }
    }
}
