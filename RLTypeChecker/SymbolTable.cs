using RLParser;
using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RLTypeChecker
{
    public class SymbolTable
    {
        private Action<CompileException, Context> onError;

        public Dictionary<string, (string type, Context context)> Classes   { get; private set; } = new Dictionary<string, (string, Context)>();
        public Dictionary<string, (string type, List<string>, Context context)> Functions { get; private set; } = new Dictionary<string, (string, List<string>, Context)>();
        public Dictionary<string, (string type, Context context)> Variables { get; private set; } = new Dictionary<string, (string, Context)>();

        public Dictionary<string, MethodInfo> MethodInfos = new Dictionary<string, MethodInfo>();

        public SymbolTable Parent { get; } = null;

        public Dictionary<string, SymbolTable> Children { get; } = new Dictionary<string, SymbolTable>();

        public SymbolTable(Action<CompileException, Context> onError) { this.onError = onError; }

        private SymbolTable(string name, SymbolTable parent) : this(parent.onError) 
        { 
            Parent = parent; 
            parent.Children.Add(name, this); 
        }

        public SymbolTable CreateChild(string name) => new SymbolTable(name, this);

        public void RegisterClass(string name, string type, Context context) 
        {
            if (ClassesContains(name)) onError(new CompileException($"Class name {name} already defined in scope"), context);
            Classes.Add(name, (type, context));
        }

        public void RegisterFunction(string name, string type, List<string> typeParams, Context context) 
        { 
            if (FunctionsContains(name)) onError(new CompileException($"Function name {name} already defined in scope"), context);
            if (VariablesContains(name)) onError(new CompileException($"Name {name} already defined in scope"), context);
            Functions.Add(name, (type, typeParams, context));
        }


        public void RegisterVariable(string name, string type, Context context) 
        { 
            if (VariablesContains(name)) onError(new CompileException($"Variable name {name} already defined in scope"), context);
            Variables.Add(name, (type, context));
        }

        public (string type, Context context) GetClass(string name, Context current)
        {
            if (Classes.TryGetValue(name, out var o)) return o;
            if (Parent == null)
            {
                onError(new CompileException($"Class name {name} cannot be found"), current);
                return ("void", current);
            }
            return Parent.GetClass(name, current);
        }

        public void RegisterFunction(string name, string v, List<string> returnTypes, Context p, MethodInfo method)
        {
            MethodInfos.Add(name, method);
            RegisterFunction(name, v, returnTypes, p);
        }

        public (string type, List<string> paramTypes, Context context) GetFunction(string name, Context current)
        {
            if (Functions.TryGetValue(name, out var o)) return o;
            if (Parent == null)
            {
                onError(new CompileException($"Function name {name} cannot be found"), current);
                return ("void", new List<string> { "void" }, current);
            }
            return Parent.GetFunction(name, current);
        }

        public (string type, Context context) GetVariable(string name, Context current)
        {
            if (Variables.TryGetValue(name, out var o)) return o;
            if (Parent == null)
            {
                onError(new CompileException($"Variable name {name} cannot be found"), current);
                return ("void", current);
            }
            return Parent.GetVariable(name, current);
        }


        public bool VariablesContains(string name)
        {
            if (Variables.ContainsKey(name)) return true;
            if (Parent == null) return false;
            else return Parent.VariablesContains(name);
        }

        public bool ClassesContains(string name)
        {
            if (Classes.ContainsKey(name)) return true;
            if (Parent == null) return false;
            else return Parent.ClassesContains(name);
        }

        public bool FunctionsContains(string name)
        {
            if (Functions.ContainsKey(name)) return true;
            if (Parent == null) return false;
            else return Parent.FunctionsContains(name);
        }
    }
}
