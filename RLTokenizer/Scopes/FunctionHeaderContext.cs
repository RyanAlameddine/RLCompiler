﻿using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    class FunctionHeaderContext : Context
    {
        private bool namePresent  = false;
        private bool colonPresent = false;
        private bool arrowPresent = false;

        public AccessModifiers AccessModifier { get; private set; }

        public string Name { get => ((IdentifierContext)Children.First.Value     ).Identifier; }

        public (string Name, string Type)[] InputVariables
        {
            get => 
                Children.Skip(1).SkipLast(2).Cast<VariableDefinitionContext>()
                .Select((x) => (x.Name, x.Type)).ToArray();
        }

        public string ReturnType 
        { 
            get
            {
                if (Children.Count < 3) return "void";

                var last = Children.Last.Previous.Value;
                if(last is VariableOrIdentifierDefinitionContext c)
                {
                    if (!c.IsVariable) return c.Identifier;
                    throw new TokenizationException("Return type is set to a variable declaration");
                }
                return ((IdentifierContext)last).Identifier;
            }
        }

        public FunctionHeaderContext(AccessModifiers accessModifier)
        {
            AccessModifier = accessModifier;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token == "{") return (true, new ScopeBodyContext());

            if (arrowPresent)
            {
                if (token.IsNewlineOrWhitespace()) return (true, this);
                //add support for multiple returns here
            }
            
            if (colonPresent)
            {
                if (Children.Last.Value is VariableOrIdentifierDefinitionContext v && !v.IsVariable)
                {
                    if(!token.IsNewlineOrWhitespace()) throw new TokenizationException("Unexpected character found after return type");
                    return (true, this);
                }

                if (token == "-") 
                    return (false, this);
                if(token == "->")
                {
                    arrowPresent = true;
                    return (true, new IdentifierContext());
                }
                if(previous == ',')
                {
                    return (true, new VariableDefinitionContext(AccessModifiers.Scope, ","));
                }
                if (token.IsWhitespace()) return (true, this);
                throw new TokenizationException("Comma not found after function parameter declaration");
            }

            if(Children.Count == 0)
            {
                if (!namePresent)
                {
                    namePresent = true;
                    return (true, new IdentifierContext());
                }
                if (!token.IsNewlineOrWhitespace()) 
                    throw new TokenizationException("No whitespace found after function declaration");
            }

            if(Children.Count == 1)
            {
                if (token.IsNewlineOrWhitespace()) return (true, this);
                if (token == ":") return (false, this);
                if (token == "::")
                {
                    colonPresent = true;
                    return (true, new VariableOrIdentifierDefinitionContext(AccessModifiers.Scope, ","));
                }
                throw new TokenizationException("No :: found after function declaration");
            }

            throw new TokenizationException("Failed to complete Function declaration");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Function Header (Name:");
            sb.Append(Name);
            sb.Append(", Return Type: ");
            sb.Append(ReturnType);
            sb.Append(", Access: ");
            sb.Append(AccessModifier);
            sb.Append(")");

            return sb.ToString();
        }
    }
}