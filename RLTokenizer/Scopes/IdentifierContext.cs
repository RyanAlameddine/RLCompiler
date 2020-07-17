using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser.Scopes
{
    class IdentifierContext : Context
    {
        private string identifier = null;
        public string Identifier 
        { 
            get
            {
                return identifier;
            }
            set
            {
                if (value != null && value.IsKeyword()) throw new TokenizationException("Identifier is a keyword");
                identifier = value;
            }
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewlineOrWhitespace()) return (true, this);

            if (!(token + next).IsIdentifier())
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

        public override string ToString() => $"Identifier: {Identifier}";
    }
}
