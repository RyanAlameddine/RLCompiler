namespace RLParser.Scopes
{
    class VariableAssignmentIdentifier : IdentifierContext
    {
        public bool IsNewVariable { get; private set; } = false; 
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (IsNewVariable)
            {
                if (!token.IsNewlineOrWhitespace()) throw new TokenizationException($"Unexpected character '{token}' found in variable assignment");
                return (true, Parent);
            }
            if (token == "var")
            {
                IsNewVariable = true;
                return (true, new VariableDefinitionContext(AccessModifiers.Scope));
            }

            return base.Evaluate(previous, token, next);
        }

        public override string ToString() 
        {
            if (IsNewVariable)
            {
                return "Expression value assignment to new variable:";
            }
            return "Expression value assignment to: " + Identifier; 
        }
    }
}