using RLParser;
using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLTypeChecker
{
    class RlTypeChecker
    {
        public static SymbolTable TypeCheck(Context root, Action<CompileException, Context> onError)
        {
            return CheckFile(root, onError, new SymbolTable(onError));
        }

        private static SymbolTable CheckFile(Context root, Action<CompileException, Context> onError, SymbolTable table)
        {
            foreach (var child in root.Children)
            {
                if (child is ClassHeaderContext c)
                {
                    table.RegisterClass(c.Name, c.Base, c);
                }
                else if (child is UsingNamespaceContext n)
                {
                    NamespaceLoader.LoadFrom(n.Namespace, onError, table);
                }
            }

            foreach (var child in root.Children)
            {
                if (child is ClassHeaderContext c)
                {
                    CheckClass(c.Children.First.Next.Value, onError, table.CreateChild());
                }
            }

            return table;
        }

        private static void CheckClass(Context root, Action<CompileException, Context> onError, SymbolTable table)
        {
            foreach (var child in root.Children)
            {
                if (child is VariableDefinitionContext v)
                {
                    table.RegisterVariable(v.Name, v.Type, v);
                }
                else if (child is FunctionHeaderContext f)
                {
                    table.RegisterFunction(f.Name, f.ReturnType, f.ParamTypes, f);
                }
            }

            foreach (var child in root.Children)
            {
                if (child is FunctionHeaderContext f)
                {
                    CheckFunctionHeader(f, f.ReturnType, onError, table.CreateChild());
                }
            }
        }

        private static void CheckFunctionHeader(FunctionHeaderContext root, string returnType, Action<CompileException, Context> onError, SymbolTable table)
        {
            for(int i = 1; i < root.Children.Count - 2; i++)
            {
                var v = (VariableDefinitionContext)root.Children.ElementAt(i);

                table.RegisterVariable(v.Name, v.Type, v);
            }

            CheckScopeBody(root.Children.Last.Value, returnType, onError, table);
        }

        private static void CheckScopeBody(Context root, string returnType, Action<CompileException, Context> onError, SymbolTable table)
        {
            foreach (var child in root.Children)
            {
                if (child is ConditionalExpressionContext c)
                {
                    CheckConditional(c, returnType, onError, table.CreateChild());
                }
                else if (child is ExpressionContext e)
                {
                    CheckStatement(e, onError, table);
                }
                else if (child is VariableDefinitionContext v)
                {
                    table.RegisterVariable(v.Name, v.Type, v);
                }
                else if (child is ReturnContext r)
                {
                    CheckReturn(r, returnType, onError, table.CreateChild());
                }
            }
        }

        private static void CheckConditional(ConditionalExpressionContext c, string returnType, Action<CompileException, Context> onError, SymbolTable table)
        {
            string type = GetExpressionType(c.Children.First.Value, onError, table);
            if (type != "bool") 
                onError(new CompileException($"Type of conditional expression in {c.Statement} statement is not bool"), c);
            CheckScopeBody(c.Children.First.Next.Value, returnType, onError, table);
        }

        private static void CheckReturn(ReturnContext r, string returnType, Action<CompileException, Context> onError, SymbolTable table)
        {
            var expressionType = GetExpressionType(r.Children.First.Value, onError, table);
            if (expressionType != returnType) onError(new CompileException($"Return expression does not match return type"), r);
        }

        private static void CheckStatement(Context expression, Action<CompileException, Context> onError, SymbolTable table)
        {
            if (expression.Children.Count == 0) return;

            if (expression.Children.Count == 2 && expression.Children.Last.Value is VariableAssignmentIdentifier v)
            {
                string type;
                if (v.IsNewVariable)
                {
                    var variable = v.Children.First.Value as VariableDefinitionContext;
                    type = variable.Type;
                    table.RegisterVariable(variable.Name, type, variable);
                }
                else
                {
                    string name = v.Identifier;
                    type = table.GetVariable(name, expression).type;
                }

                string expressionType = GetExpressionType(expression.Children.First.Value, onError, table);
                if (type != expressionType) 
                    onError(new CompileException($"Type of expression in statement does not match type of assignment"), expression);

                return;
            }

            GetExpressionType(expression, onError, table);
        }

        private static string CheckFunctionCall(ExpressionContext expression, Action<CompileException, Context> onError, SymbolTable table)
        {
            string name = (expression.Children.First.Value as IdentifierContext).Identifier;
            var children = expression.Children.Skip(1);
            var (type, paramTypes, _) = table.GetFunction(name, expression);
            var parameters = paramTypes;

            if(children.Count() != parameters.Count()) onError(new CompileException($"{name} function call does not have the correct number of parameters"), expression);

            foreach (var parameter in children.Zip(parameters))
            {
                string expressionType = GetExpressionType(parameter.First, onError, table);
                if (expressionType != parameter.Second) onError(new CompileException($"{name} function call was provided a parameter of type {expressionType} instead of {parameter}"), expression);
            }

            return type;
        }

        private static string GetExpressionType(Context expression, Action<CompileException, Context> onError, SymbolTable table)
        {
            if (expression is IntLiteral i) return "int";
            else if (expression is StringLiteral s) return "string";
            else if (expression is IdentifierContext id)
            {
                var name = id.Identifier;
                if (name == "true" || name == "false") return "bool";
                //TODO DELEGATE: if(table.FunctionsContains(id))
                var (type, _) = table.GetVariable(name, expression);
                return type;
            }
            else if (expression is ExpressionContext e && e.IsFunctionCall) return CheckFunctionCall(e, onError, table);

            if (expression.Children.Count == 0) return "void";
            if (expression.Children.Count == 1) return GetExpressionType(expression.Children.First.Value, onError, table);
            //TODO fix this case (-1) -> (0-1)
            if(expression.Children.Count == 3)
            {
                string typeL = GetExpressionType(expression.Children.First.Value, onError, table);
                string op = (expression.Children.First.Next.Value as OperatorIdentifierContext).Identifier;
                if (op == ":")
                {
                    return (expression.Children.Last.Value as IdentifierContext).Identifier;
                    //casting
                }
                
                string typeR = GetExpressionType(expression.Children.Last.Value, onError, table);

                if (op.IsSpltOperator()) return "bool";
                if (typeL != typeR)
                {
                    //TODO check for special cases
                    onError(new CompileException($"Type {typeL} does not have functionality {op} with {typeR}"), expression);
                }
                return typeL;
            }

            onError(new CompileException("Typechecker found invalid expression").SetStartAndEnd(expression), expression);
            return "void";
        }
    }
}
