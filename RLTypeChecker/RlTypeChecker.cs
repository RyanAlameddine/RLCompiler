using RLParser;
using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLTypeChecker
{
    public static class RlTypeChecker
    {
        public static SymbolTable TypeCheck(Context root, Action<CompileException, Context> onError)
        {
            return CheckFile(root, onError, new SymbolTable(onError));
        }

        private static SymbolTable CheckFile(Context root, Action<CompileException, Context> onError, SymbolTable table)
        {
            NamespaceLoader.LoadFrom("System", onError, table);
            foreach (var child in root.Children)
            {
                if (child is ClassHeaderContext c)
                {
                    table.RegisterClass(c.Name, c.Base, c);
                    LoadClass(c.Children.Last.Value, c.Name, onError, table.CreateChild(c.Name));
                }
                else if (child is UsingNamespaceContext n)
                {
                    if (n.Namespace == "System") continue;
                    NamespaceLoader.LoadFrom(n.Namespace, onError, table);
                }
            }

            foreach (var child in root.Children)
            {
                if (child is ClassHeaderContext c)
                {
                    var classBody = c.Children.First.Next;
                    if (!(classBody.Value is ClassBodyContext)) classBody = classBody.Next;
                    CheckClass(classBody.Value, onError, table.Children[c.Name].Children["classMembers"]);
                }
            }

            return table;
        }

        /// <summary>
        /// loads the variables and functions from a class before typechecking it
        /// </summary>
        private static void LoadClass(Context root, string name, Action<CompileException, Context> onError, SymbolTable table)
        {
            bool constructorPresent = false;
            SymbolTable privateChild = table.CreateChild("classMembers");
            foreach (var child in root.Children)
            {
                if (child is VariableDefinitionContext v)
                {
                    if (v.AccessModifier == AccessModifiers.Public)
                    {
                        table.RegisterVariable(v.Name, v.Type, v);
                    }
                    else
                    {
                        privateChild.RegisterVariable(v.Name, v.Type, v);
                    }
                }
                else if (child is FunctionHeaderContext f)
                {
                    if (f.Name == name) constructorPresent = true;
                    if (f.AccessModifier == AccessModifiers.Public)
                    {
                        if (f.Name == name)
                        {
                            //constructor
                            table.Parent.RegisterFunction(f.Name, f.Name, f.ParamTypes, f);
                        }
                        else
                        {
                            table.RegisterFunction(f.Name, f.ReturnType, f.ParamTypes, f);
                        }
                    }
                    else
                    {
                        privateChild.RegisterFunction(f.Name, f.ReturnType, f.ParamTypes, f);
                    }
                }
            }

            if (!constructorPresent)
            {
                table.CreateChild(name);
                table.Parent.RegisterFunction(name, name, new List<string>(), root);
            }
        }

        private static void CheckClass(Context root, Action<CompileException, Context> onError, SymbolTable table)
        {
            foreach (var child in root.Children)
            {
                if (child is FunctionHeaderContext f)
                {
                    CheckFunctionHeader(f, f.ReturnType, onError, table.CreateChild(f.Name));
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
            int statement = 0;
            foreach (var child in root.Children)
            {
                if (child is ConditionalExpressionContext c)
                {
                    CheckConditional(c, returnType, onError, table.CreateChild($"statement{statement}"));
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
                    CheckReturn(r, returnType, onError, table.CreateChild($"statement{statement}"));
                }
                statement++;
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
                    type = TableSearcher.GetVar(table, name, expression).type;
                }

                string expressionType = GetExpressionType(expression.Children.First.Value, onError, table);
                if (expressionType != "void" && type != expressionType) 
                    onError(new CompileException($"Type of expression in statement does not match type of assignment"), expression);

                return;
            }

            GetExpressionType(expression, onError, table);
        }

        private static string CheckFunctionCall(ExpressionContext expression, Action<CompileException, Context> onError, SymbolTable table)
        {
            string name = (expression.Children.First.Value as IdentifierContext).Identifier;
            var children = expression.Children.Skip(1);
            var func = TableSearcher.GetFunc(table, name, expression);

            return CheckFunctionCall(children, name, func, onError, table);
        }

        private static string CheckFunctionCall(IEnumerable<Context> children, string name, (string type, List<string> paramTypes, Context context) func, Action<CompileException, Context> onError, SymbolTable table)
        {
            var (type, paramTypes, _) = func;
            //var (type, paramTypes, _) = table.GetFunction(name, expression);

            //check for function calls with unit (for example, Console.Readline () )
            if (children.Count() == 1 && children.First() is ExpressionContext e && e.Children.Count == 0)
            {
                if (!(paramTypes.Count == 0 || (paramTypes.Count == 1 && paramTypes[0] == "void")))
                    onError(new CompileException($"() is not a valid parameter for {name}"), e);
                return type;
            }

            //check for invalid parameter count
            if (children.Count() != paramTypes.Count()) onError(new CompileException($"{name} function call does not have the correct number of parameters"), func.context);

            foreach (var parameter in children.Zip(paramTypes, (x, y) => (x, y)))
            {
                string expressionType = GetExpressionType(parameter.x, onError, table);
                if (expressionType != parameter.y) onError(new CompileException($"{name} function call was provided a parameter of type {expressionType} instead of {parameter.y}"), func.context);
            }

            return type;
        }

        public static string GetExpressionType(Context context, Action<CompileException, Context> onError, SymbolTable table)
        {
            if (context is IntLiteral i) return "int";
            else if (context is StringLiteral s) return "string";
            else if (context is IdentifierContext id)
            {
                var name = id.Identifier;
                if (name == "true" || name == "false") return "bool";
                //TODO DELEGATE: if(table.FunctionsContains(id))
                var (type, _) = TableSearcher.GetVar(table, name, context);
                return type;
            }
            else if (context is ExpressionContext e && e.IsFunctionCall) return CheckFunctionCall(e, onError, table);
            else if (context is ListDeclarationContext l)
            {
                if (l.IsListComprehension) return GetListComprehensionType(l, onError, table.CreateChild($"comprehension{l.Characters.Start}"));
                else return GetListDeclarationType(l, onError, table);
            }

            if (context.Children.Count == 0) return "void";
            if (context.Children.Count == 1) return GetExpressionType(context.Children.First.Value, onError, table);
            //TODO fix this case (-1) -> (0-1)
            if(context.Children.Count > 2)
            {
                string typeL = GetExpressionType(context.Children.First.Value, onError, table);
                string op = (context.Children.First.Next.Value as OperatorIdentifierContext).Identifier;
                if (op == ".")
                {
                    //variable
                    if (context.Children.Count == 3)
                    {
                        var id = context.Children.Last.Value as IdentifierContext;
                        (string type, _) = TableSearcher.LoadVarFromType(table, typeL, id.Identifier, context);
                        return type;
                    }
                    //function
                    else
                    {
                        var id = context.Children.First.Next.Next.Value as IdentifierContext;
                        var func = TableSearcher.LoadFuncFromType(table, typeL, id.Identifier, context);
                        return CheckFunctionCall(context.Children.Skip(3), id.Identifier, func, onError, table);
                    }
                }

                if (context.Children.Count == 3)
                {
                    if (op == ":")
                    {
                        return (context.Children.Last.Value as IdentifierContext).Identifier;
                        //casting
                    }

                    string typeR = GetExpressionType(context.Children.Last.Value, onError, table);

                    if (typeL != typeR)
                    {
                        //TODO check for special cases
                        if (op.IsSpltOperator() && (typeL == "void" || typeR == "void")) return "bool";
                        onError(new CompileException($"Type {typeL} does not have functionality {op} with {typeR}"), context);
                    }
                    if (op.IsSpltOperator()) return "bool";
                    return typeL;
                }
            }

            onError(new CompileException("Typechecker found invalid expression"), context);
            return "void";
        }

        private static string GetListDeclarationType(ListDeclarationContext l, Action<CompileException, Context> onError, SymbolTable table)
        {
            if (l.Children.Count == 0) return "void";

            string type = GetExpressionType(l.Children.First.Value, onError, table);
            foreach(var expression in l.Children.Skip(1))
            {
                string newType = GetExpressionType(expression, onError, table);
                if(type != newType) 
                    onError(new CompileException($"Type {type} and {newType} cannot both be elements of the same list"), l);
            }
            return $"[{type}]";
        }

        private static string GetListComprehensionType(ListDeclarationContext l, Action<CompileException, Context> onError, SymbolTable table)
        {
            string id = (l.Children.First.Next.Value as IdentifierContext).Identifier;
            string listType = GetExpressionType(l.Children.First.Next.Next.Value, onError, table);
            string idType = listType.Substring(1, listType.Length-2);
            table.RegisterVariable(id, idType, l);

            string returnType = GetExpressionType(l.Children.First.Value, onError, table);

            //check if statement
            if (l.Children.Last.Value is ConditionalExpressionContext c && GetExpressionType(c, onError, table) != "bool")
                onError(new CompileException("Conditional expression type in list comprehension is not bool"), c);

            return $"[{returnType}]";
        }
    }
}
