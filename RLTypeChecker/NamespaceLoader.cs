using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RLParser;

namespace RLTypeChecker
{
    public static class NamespaceLoader
    {
        public static void LoadFrom(string Namespace, Action<CompileException, Context> onError, SymbolTable table)
        {
            var classes = AppDomain.CurrentDomain.GetAssemblies()
                       .SelectMany(t => t.GetTypes())
                       .Where(t => t.IsClass || t.IsValueType && t.Namespace == Namespace);

            foreach(var c in classes)
            {
                if (c.ContainsGenericParameters) continue;
                if (!c.IsPublic) continue;
                if (table.ClassesContains(c.Name)) continue;
                table.RegisterClass(c.Name, c.BaseType?.Name, null);
                RegisterInstanceMembers(c, onError, table);
            }
        }

        private static void RegisterInstanceMembers(Type c, Action<CompileException, Context> onError, SymbolTable table)
        {
            var methods = c.GetMethods();
            var fields = c.GetFields();

            SymbolTable classTable = table.CreateChild(c.Name);

            foreach(var method in methods)
            {
                if (!method.IsPublic) continue;
                if (method.IsStatic) RegisterMethod($"{c.Name}.{method.Name}", method, onError, table);
                else RegisterMethod(method.Name, method, onError, classTable);
            }

            foreach (var field in fields)
            {
                if (!field.IsPublic) continue;
                if (field.IsStatic) RegisterField($"{c.Name}.{field.Name}", field, onError, table);
                else RegisterField(field.Name, field, onError, classTable);
            }
        }

        private static void RegisterMethod(string name, MethodInfo method, Action<CompileException, Context> onError, SymbolTable table)
        {
            List<string> returnTypes = new List<string>();
            foreach(var param in method.GetParameters())
            {
                returnTypes.Add(GetTypeName(param.ParameterType));
            }
            if (table.FunctionsContains(name)) return;
            table.RegisterFunction(name, GetTypeName(method.ReturnType), returnTypes, null);
        }

        private static void RegisterField(string name, FieldInfo field, Action<CompileException, Context> onError, SymbolTable table)
        {
            table.RegisterVariable(name, GetTypeName(field.FieldType), null);
        }

        private static string GetTypeName(Type t)
        {
            if (t.Name.ToLower().IsBuiltinType()) return t.Name.ToLower();
            return t.Name;
        }
    }
}
