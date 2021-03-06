﻿using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    public class VariableOrIdentifierDefinitionContext : VariableDefinitionContext
    {
        public bool IsVariable { get; private set; } = true;
        public string Identifier { get; private set; }

        public VariableOrIdentifierDefinitionContext(AccessModifiers accessModifier) : base(accessModifier)
        {
        }

        public VariableOrIdentifierDefinitionContext(AccessModifiers accessModifier, string otherExitCharacters) : base(accessModifier, otherExitCharacters)
        {
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if(Children.Count == 1 && token.IsNewlineOrWhitespace())
            {
                IsVariable = false;
                Identifier = ((IdentifierContext)Children.First.Value).Identifier;
                Children.Clear();
                return (true, Parent);
            }
            return base.Evaluate(previous, token, next);
        }

        public override string ToString()
        {
            if (IsVariable) return base.ToString();
            return $"VIdentifier: {Identifier}";
        }
    
    }
}