namespace RLParser.Scopes
{
    public class VariableAssignmentIdentifier : IdentifierContext
    {
        public bool IsNewVariable { get; private set; } = false;
        public string NewVarType { get; private set; }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (IsNewVariable)
            {
                if (!token.IsNewlineOrWhitespace()) throw new CompileException($"Unexpected character '{token}' found in variable assignment");

                var child = Children.First.Value as VariableDefinitionContext;
                NewVarType = child.Type;
                Identifier = child.Name;

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