using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    public class VariableDefinitionContext : Context
    {
        protected Regex otherExitCharacters = null;

        protected bool colonPresent = false;

        public string Name { get => Children.Count == 0 ? null : ((IdentifierContext)Children.First.Value     ).Identifier; }
        public string Type { get => Children.Count == 0 ? null : ((IdentifierContext)Children.First.Next.Value).Identifier; }

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
                if (Children.Count != 2) throw new CompileException("Incomplete variable declaration");
                if (token.IsNewline()) return Parent.Evaluate(previous, token, next);
                return (true, Parent);
            }

            if (Children.Count == 2)
            {
                if (!token.IsNewlineOrWhitespace()) throw new CompileException($"Unexpected token '{token}' found in variable definition context");
                return (true, Parent);
            }

            if (Children.Count == 0)
            {
                if (token.IsNewlineOrWhitespace()) return (true, new IdentifierContext());
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

                throw new CompileException("No whitespace after var in variable declaration");
            }

            if (token.IsNewline()) return (true, this);

            if (token == ":")
            {
                if (colonPresent) 
                    throw new CompileException("Two :'s found in variable declaration");
                colonPresent = true;
                return (true, new TypeIdentifierContext());
            }

            if (colonPresent) return (true, new IdentifierContext());

            if (token.IsWhitespace()) throw new CompileException("Space found before : in variable declaration");
            throw new CompileException("Type identifier found before : in variable declaration");
        }

        public override string ToString() => $"Variable Declaration (Name: {Name}, Type: {Type}, Access: {AccessModifier})";
    }
}