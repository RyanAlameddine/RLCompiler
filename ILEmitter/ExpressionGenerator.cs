using RLParser;
using RLParser.Scopes;
using RLTypeChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RLCodeGenerator
{
    class ExpressionGenerator
    {
        private Label? ifEndLabel = null;

        public bool Generate(Context expression, SymbolTable functionTable, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent)
        {
            bool isRet = GenerateStatement(expression, functionTable, generator, variables, parent);
            if (isRet)
            {
                generator.Emit(OpCodes.Ret);
            }
            //for (; stackDepth > 0; stackDepth--) generator.Emit(OpCodes.Pop);
            return isRet;
        }

        private bool GenerateStatement(Context expression, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent)
        {
            if (expression.Children.Count == 2 && expression.Children.Last.Value is VariableAssignmentIdentifier v)
            {
                if (v.IsNewVariable)
                {
                    parent.GenerateLocalVariable(v.Identifier, v.NewVarType, generator);
                }
                Action GenerateVar = () => GenerateExpression(expression.Children.First.Value, table, generator, variables, parent);
                parent.GenerateVariableStore(generator, v.Identifier, GenerateVar);
                return false;
            }

            GenerateExpression(expression, table, generator, variables, parent);
            if (expression is ReturnContext) return true;
            return false;
        }

        private void GenerateExpression(Context expression, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent)
        {
            if (expression is ConditionalExpressionContext condition)
            {
                ifEndLabel = ConditionalGenerator.Generate(condition, table, generator, variables, parent, this, ifEndLabel);
                return;
            }
            else if(ifEndLabel != null)
            {
                generator.MarkLabel(ifEndLabel.Value);
            }
            ifEndLabel = null;

            if (expression is StringLiteral str)
            {
                generator.Emit(OpCodes.Ldstr, str.String);
            }
            else if (expression is IntLiteral num)
            {
                generator.Emit(OpCodes.Ldc_I4, num.Number);
            }
            else if (expression is IdentifierContext id)
            {
                var name = id.Identifier;
                if (name == "true" || name == "false")
                {
                    if (name == "true") generator.Emit(OpCodes.Ldc_I4_1);
                    else generator.Emit(OpCodes.Ldc_I4_0);
                }
                else
                {
                    parent.GenerateVariableLoad(generator, name, false);
                }
            }
            else if (expression is ExpressionContext && expression.Children.Count == 0) return;
            else if (expression.Children.Count == 1) GenerateExpression(expression.Children.First.Value, table, generator, variables, parent);
            else if (expression is ExpressionContext e && e.IsFunctionCall) GenerateFunctionCall(e.Children.First.Value as IdentifierContext, e.Children.Skip(1), table, generator, variables, parent);
            else if (expression.Children.Count >= 3)
            {
                var leftChild = expression.Children.First.Value;
                string opID = (expression.Children.First.Next.Value as OperatorIdentifierContext).Identifier;
                var rightChild = expression.Children.Last.Value;

                GenerateExpression(leftChild, table, generator, variables, parent);

                //standard operator
                if (expression.Children.Count == 3)
                {
                    //casting
                    if (opID == ":")
                    {
                        string castType = (rightChild as IdentifierContext).Identifier;
                        //TODO add casting
                    }
                    //general operators
                    else
                    {
                        GenerateExpression(rightChild, table, generator, variables, parent);

                        OperatorGenerator.GenerateOperator(opID, generator);
                    }
                }
                else if(opID == ".")
                {
                    var idContext = expression.Children.First.Next.Next.Value as IdentifierContext;
                    var paramChildren = expression.Children.Skip(3);

                    Type type = parent.parent.parent.FindType(RlTypeChecker.GetExpressionType(leftChild, null, table));
                    GenerateFunctionCall(idContext, paramChildren, table, generator, variables, parent, type);
                }
                else throw new CompileException("Invalid Expression found by RLCodeGenerator");
            }
            else throw new CompileException("Invalid Expression found by RLCodeGenerator");
            
        }

        private void GenerateFunctionCall(IdentifierContext identifierContext, IEnumerable<Context> children, SymbolTable table, ILGenerator generator, Dictionary<string, LocalBuilder> variables, MethodEvaluatorBase parent, Type type = null)
        {
            var isConstructor = parent.parent.TryGetConstructor(identifierContext.Identifier, out var c);
            MethodInfo methodInfo = null;
            if (!isConstructor)
            {
                methodInfo = parent.GetMethodInfo(identifierContext.Identifier, generator, table, type);
            }
            foreach (var child in children)
            {
                GenerateExpression(child, table, generator, variables, parent);
            }

            if (isConstructor)
            {
                generator.Emit(OpCodes.Newobj, parent.parent.GetConstructorInfo(identifierContext.Identifier, table));
            }
            else
            {
                generator.Emit(OpCodes.Call, methodInfo);
            }
        }
    }
}
