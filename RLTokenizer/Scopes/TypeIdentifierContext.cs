using System;
using System.Collections.Generic;
using System.Text;

namespace RLTokenizer.Scopes
{
    class TypeIdentifierContext : IdentifierContext
    {
        public bool isList = false;

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsLineOrWhitespace()) return (true, this);

            if(token == "[")
            {
                isList = true;
                return (true, this);
            }
            if(token[^1] == ']')
            {
                if (isList)
                {
                    Identifier = token[0..^1];
                    return (true, Parent);
                }
            }

            if (!(token + next).IsIdentifier() && next != ']')
            {
                if (token.IsIdentifier())
                {
                    Identifier = token;
                    return (true, Parent);
                }
                throw new TokenizationException("Token is not a valid identifier");
            }

            return (false, this);
        }

        public override string ToString() => $"Type Identifier: {Identifier}" + (isList ? ", list" : "");
    }
}
