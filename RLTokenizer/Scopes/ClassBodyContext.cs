namespace RLParser.Scopes
{
    public class ClassBodyContext : Context
    {
        private AccessModifiers accessModifier = AccessModifiers.Private;

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token == "}") return (true, Parent.Parent);

            if (next.ToString().IsNewlineOrWhitespace())
            {
                if (token == "public:")
                {
                    accessModifier = AccessModifiers.Public;
                    return (true, this);
                }
                else if (token == "private:")
                {
                    accessModifier = AccessModifiers.Private;
                    return (true, this);
                }
                else if (token == "internal:")
                {
                    accessModifier = AccessModifiers.Internal;
                    return (true, this);
                }
                else if (token == "var") return (true, new VariableDefinitionContext(accessModifier));
                else if (token == "def") return (true, new FunctionHeaderContext(accessModifier));
            }

            if (token.IsNewlineOrWhitespace()) return (true, this);
            else if (next.ToString().IsNewline()) throw new CompileException("Invalid statement");

            return (false, this);
        }

        public override string ToString() => "Class Body";
    }

    public enum AccessModifiers
    {
        Public,
        Private,
        Internal,
        Scope,
    }
}