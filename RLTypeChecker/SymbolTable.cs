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

        private Dictionary<string, (string, Context)> Classes   { get; set; } = new Dictionary<string, (string, Context)>();
        private Dictionary<string, (string, Context)> Functions { get; set; } = new Dictionary<string, (string, Context)>();
        private Dictionary<string, (string, Context)> Variables { get; set; } = new Dictionary<string, (string, Context)>();

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

        public void RegisterFunction(string name, string type, Context context) 
        { 
            if (FunctionsContains(name)) onError(new CompileException($"Function name {name} already defined in scope"), context);
            Functions.Add(name, (type, context));
        }

        public void RegisterVariable(string name, string type, Context context) 
        { 
            if (VariablesContains(name)) onError(new CompileException($"Variable name {name} already defined in scope"), context);
            Variables.Add(name, (type, context));
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
            if (Parent == null) return false;
            if (Functions.ContainsKey(name)) return true;
            else return Parent.FunctionsContains(name);
        }
    }
}
