using RLParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLTypeChecker
{
    static class TableSearcher
    {
        public static SymbolTable GetTypeTable(SymbolTable current, string type, Context context)
        {
            SymbolTable root = current;
            while (root.Parent != null) root = root.Parent;

            return GetTypeChild(root, type);
        }

        private static SymbolTable GetTypeChild(SymbolTable root, string type)
        {
            switch (type)
            {
                case "int":
                    return root.Children["Int32"];
                case "bool":
                    return root.Children["Boolean"];
                case "string":
                    return root.Children["String"];
            }
            return root.Children[type];
        }

        public static (string type, List<string> paramTypes, Context context) GetFunc(SymbolTable table, string name, Context current)
        {
            if (table.FunctionsContains(name)) return table.GetFunction(name, current);

            string[] strings = name.Split('.');
            var currentVar = table.GetVariable(strings.First(), current);
            foreach (var identifier in strings.Skip(1).SkipLast(1))
            {
                currentVar = LoadVarFromType(table, currentVar.type, identifier, current);
            }

            return LoadFuncFromType(table, currentVar.type, strings.Last(), current);
        }

        public static (string type, Context context) GetVar(SymbolTable table, string name, Context current)
        {
            if (table.VariablesContains(name)) return table.GetVariable(name, current);

            string[] strings = name.Split('.');
            var currentVar = table.GetVariable(strings.First(), current);
            foreach (var identifier in strings.Skip(1))
            {
                currentVar = LoadVarFromType(table, currentVar.type, identifier, current);
            }

            return currentVar;
        }

        public static (string type, List<string> paramTypes, Context context) LoadFuncFromType(SymbolTable table, string type, string tail, Context current)
        {
            SymbolTable root = GetTypeTable(table, type, current).Parent;

            while (root.Children.ContainsKey(type) && !root.Children[type].FunctionsContains(tail))
            {
                //search base class
                type = root.GetClass(type, current).type;

                //this will throw an error
                if (type == null) return table.GetFunction(tail, current);
            }

            return GetTypeChild(root, type).GetFunction(tail, current);
        }

        public static (string type, Context context) LoadVarFromType(SymbolTable table, string type, string tail, Context current)
        {
            SymbolTable root = GetTypeTable(table, type, current).Parent;
            while (root.Children.ContainsKey(type) && !root.Children[type].VariablesContains(tail))
            {
                //search base class
                type = root.GetClass(type, current).type;

                //this will throw an error
                if (type == null) return table.GetVariable(tail, current);
            }

            return GetTypeChild(root, type).GetVariable(tail, current);
        }
    }
}
