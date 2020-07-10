namespace RLTokenizer.Scopes
{
    class ClassBodyContext : Context
    {
        private AccessModifiers accessModifier = AccessModifiers.Private;

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token == "}") return (true, Parent.Parent);

            if (next.ToString().IsLineOrWhitespace())
            {
                if (token == "public:")
                {
                    accessModifier = AccessModifiers.Private;
                    return (true, this);
                }
                else if (token == "private:")
                {
                    accessModifier = AccessModifiers.Public;
                    return (true, this);
                }
                else if (token == "var") return (true, new VariableDefinitionContext (accessModifier));
                else if (token == "def") return (true, new FunctionHeaderContext(accessModifier));
            }

            if (token.IsLineOrWhitespace()) return (true, this);
            else if (next.ToString().IsNewline()) throw new TokenizationException("Invalid statement");

            return (false, this);
        }

        public override string ToString() => "Class Body";
    }

    enum AccessModifiers
    {
        Public,
        Private,
        Function,
    }
}