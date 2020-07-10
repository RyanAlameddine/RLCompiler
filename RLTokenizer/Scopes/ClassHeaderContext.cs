using System.Collections.Generic;

namespace RLTokenizer.Scopes
{
    class ClassHeaderContext : Context
    {
        public string Name { get => ((IdentifierContext)Children.First.Value).Identifier; }
        public string Base { get => ((IdentifierContext)Children.First.Next.Value).Identifier; }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            //note to self: change to three when add inheritance
            if(Children.Count == 2)
            {
                return Parent.Evaluate(previous, token, next);
            }

            if(Children.Count == 0)
            {
                if (token.IsLineOrWhitespace()) return (true, new IdentifierContext());

                throw new TokenizationException("No whitespace after class declaration");
            }

            //TODO implement base classes

            if (token == "{")
            {
                return (true, new ClassBodyContext());
            }

            if (token.IsLineOrWhitespace()) return (true, this);

            throw new TokenizationException("Invalid Identifier in class declaration");
        }

        public override string ToString() => $"Class Header (Name: {Name}, Base: TODO)";
    }
}