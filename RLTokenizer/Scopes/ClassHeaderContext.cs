using System.Collections.Generic;

namespace RLParser.Scopes
{
    public class ClassHeaderContext : Context
    {
        public string Name { get => ((IdentifierContext)Children.First.Value).Identifier; }
        public string Base { get => Children.Count < 3 ? null : ((IdentifierContext)Children.First.Next.Value).Identifier; }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if(Children.Count != 0 && Children.Last.Value is ClassBodyContext)
            {
                return Parent.Evaluate(previous, token, next);
            }

            if(Children.Count == 0)
            {
                if (token.IsNewlineOrWhitespace()) return (true, new IdentifierContext());

                throw new TokenizationException("No whitespace after class declaration");
            }

            if(Children.Count == 1)
            {
                if (token == ":") return (true, new TypeIdentifierContext());
            }

            if (token == "{")
            {
                return (true, new ClassBodyContext());
            }

            if (token.IsNewlineOrWhitespace()) return (true, this);

            throw new TokenizationException("Invalid character found in class declaration");
        }

        public override string ToString() => $"Class Header (Name: {Name}" + (Base == null ? ")" : $"{Base})");
    }
}