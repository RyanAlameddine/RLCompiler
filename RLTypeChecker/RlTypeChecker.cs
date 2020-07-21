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
                    table.RegisterFunction(f.Name, f.ReturnType, f);
                }
            }

            foreach (var child in root.Children)
            {
                if (child is FunctionHeaderContext f)
                {
                    CheckFunctionHeader(f, onError, table.CreateChild());
                }
            }
        }

        private static void CheckFunctionHeader(FunctionHeaderContext root, Action<CompileException, Context> onError, SymbolTable table)
        {
            for(int i = 1; i < root.Children.Count - 2; i++)
            {
                var v = (VariableDefinitionContext)root.Children.ElementAt(i);

                table.RegisterVariable(v.Name, v.Type, v);
            }

            CheckScopeBody(root.Children.Last.Value, onError, table);
        }

        private static void CheckScopeBody(Context root, Action<CompileException, Context> onError, SymbolTable table)
        {

        }
    }
}
