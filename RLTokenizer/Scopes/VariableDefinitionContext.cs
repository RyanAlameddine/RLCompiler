using System.Text.RegularExpressions;

namespace RLTokenizer.Scopes
{
    class VariableDefinitionContext : Context
    {
        private Regex otherExitCharacters = null;

        private bool colonPresent = false;

        public string Name { get => ((IdentifierContext)Children.First.Value     ).Identifier; }
        public string Type { get => ((IdentifierContext)Children.First.Next.Value).Identifier; }

        public AccessModifiers AccessModifier { get; private set; }

        public VariableDefinitionContext(AccessModifiers accessModifier)
        {
            AccessModifier = accessModifier;
        }

        public VariableDefinitionContext(AccessModifiers accessModifier, string otherExitCharacters)
        {
            AccessModifier = accessModifier;
            this.otherExitCharacters = new Regex(otherExitCharacters);
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewline() || otherExitCharacters != null && otherExitCharacters.IsMatch(token))
            {
                if (Children.Count != 2) throw new TokenizationException("Incomplete variable declaration");
                return (true, Parent);
            }

            if (Children.Count == 2 && token.IsWhitespace())
            {
                return (true, Parent);
            }

            if (Children.Count == 0)
            {
                if (token.IsLineOrWhitespace()) return (true, new IdentifierContext());
                if (previous.ToString().IsWhitespace()) 
                {
                    if (!next.ToString().IsIdentifier())
                    {
                        var newContext = new IdentifierContext();
                        newContext.Parent = this;
                        newContext.Identifier = token;
                        Children.AddLast(newContext);
                        return (true, this);
                    }
                    return new IdentifierContext().Evaluate(previous, token, next);
                }

                throw new TokenizationException("No whitespace after var in variable declaration");
            }

            if (token.IsLineOrWhitespace()) return (true, this);

            if (token == ":")
            {
                if (colonPresent) 
                    throw new TokenizationException("Two :'s found in variable declaration");
                colonPresent = true;
                return (true, new TypeIdentifierContext());
            }

            if (colonPresent) return (true, new IdentifierContext());


            throw new TokenizationException("Type identifier found before : in variable declaration");
        }

        public override string ToString() => $"Variable Declaration (Name: {Name}, Type: {Type})";
    }
}