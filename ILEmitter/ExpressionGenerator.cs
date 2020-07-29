using RLParser;
using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ILEmitter
{
    static class ExpressionGenerator
    {
        public static bool Generate(ExpressionContext expression, SymbolTable functionTable, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent)
        {
            int stackDepth = 0;
            bool isRet = GenerateStatement(expression, functionTable, generator, variables, parent, ref stackDepth);
            if (isRet)
            {
                generator.Emit(OpCodes.Ret);
            }
            //for (; stackDepth > 0; stackDepth--) generator.Emit(OpCodes.Pop);
            return isRet;
        }

        private static bool GenerateStatement(Context expression, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent, ref int stackDepth)
        {
            if (expression.Children.Count == 2 && expression.Children.Last.Value is VariableAssignmentIdentifier v)
            {
                if (v.IsNewVariable)
                {
                    //generator.DeclareLocal()
                }
                GenerateExpression(expression.Children.First.Value, table, generator, variables, parent, ref stackDepth);
                parent.GenerateVariableStore(generator, v.Identifier);
                return false;
            }
            GenerateExpression(expression, table, generator, variables, parent, ref stackDepth);
            if (expression is ReturnContext) return true;
            return false;
        }

        private static void GenerateExpression(Context expression, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent, ref int stackDepth)
        {
            if(expression is StringLiteral str)
            {
                generator.Emit(OpCodes.Ldstr, str.String);
                stackDepth++;
            }
            else if (expression is IntLiteral num)
            {
                generator.Emit(OpCodes.Ldc_I4, num.Number);
                stackDepth++;
            }
            else if (expression is IdentifierContext id)
            {
                var name = id.Identifier;
                if (name == "true" || name == "false")
                {
                    if (name == "true") generator.Emit(OpCodes.Ldc_I4_1);
                    else generator.Emit(OpCodes.Ldc_I4_0);
                    stackDepth++;
                }
                else
                {
                    parent.GenerateVariableLoad(generator, name);
                    stackDepth++;
                }
            }
            else if (expression.Children.Count == 1)
            {
                GenerateExpression(expression.Children.First.Value, table, generator, variables, parent, ref stackDepth);
            }
            else if (expression is ExpressionContext e && e.IsFunctionCall) GenerateFunctionCall(e.Children.First.Value as IdentifierContext, e.Children.Skip(1), table, generator, variables, parent, ref stackDepth);
        }

        private static void GenerateFunctionCall(IdentifierContext identifierContext, IEnumerable<Context> children, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent, ref int stackDepth)
        {
            foreach(var child in children)
            {
                GenerateExpression(child, table, generator, variables, parent, ref stackDepth);
            }
            generator.Emit(OpCodes.Call, parent.parent.GetMethodInfo(identifierContext.Identifier, table));
        }
    }
}
