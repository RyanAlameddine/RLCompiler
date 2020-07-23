using RLParser;
using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RLTypeChecker
{
    public class SymbolTable
    {
        private Action<CompileException, Context> onError;

        private Dictionary<string, (string type, Context context)> Classes   { get; set; } = new Dictionary<string, (string, Context)>();
        private Dictionary<string, (string type, List<string>, Context context)> Functions { get; set; } = new Dictionary<string, (string, List<string>, Context)>();
        private Dictionary<string, (string type, Context context)> Variables { get; set; } = new Dictionary<string, (string, Context)>();

        public SymbolTable Parent { get; } = null;

        public List<SymbolTable> Children { get; } = new List<SymbolTable>();

        public SymbolTable(Action<CompileException, Context> onError) { this.onError = onError; }

        private SymbolTable(SymbolTable parent) : this(parent.onError) { Parent = parent; parent.Children.Add(this); }


        public SymbolTable CreateChild() => new SymbolTable(this);

        public void RegisterClass(string name, string type, Context context) 
        {
            if (ClassesContains(name)) onError(new CompileException($"Class name {name} already defined in scope"), context);
            Classes.Add(name, (type, context)); 
        }

        public void RegisterFunction(string name, string type, List<string> typeParams, Context context) 
        { 
            if (FunctionsContains(name)) onError(new CompileException($"Function name {name} already defined in scope"), context);
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
            if (Parent == null) return false;
            if (Variables.ContainsKey(name)) return true;
            else return Parent.VariablesContains(name);
        }

        public bool ClassesContains(string name)
        {
            if (Parent == null) return false;
            if (Classes.ContainsKey(name)) return true;
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
