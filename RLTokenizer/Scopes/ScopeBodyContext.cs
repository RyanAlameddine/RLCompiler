using System;

namespace RLTokenizer.Scopes
{
    class ScopeBodyContext : Context
    {
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if(token == "}")
            {
                //if (!isClean) throw new TokenizationException("Inclomplete statement in function body");
                return (true, Parent.Parent);
            }

            if (token.IsNewlineOrWhitespace()) return (true, this);

            if (token == "[") throw new NotImplementedException("Lists not yet implemented");

            //Identifier stage
            if (!(token + next).IsIdentifier())
            {
                if (token.IsIdentifier())
                {
                    return (true, CheckStatementRules(token));
                }
                throw new TokenizationException("Token is not a valid identifier");
            }

            return (false, this);
        }

        private Context CheckStatementRules(string token)
        {
            switch (token)
            {
                case "ret":
                    return new ReturnExpressionContext();
                case "if":
                case "elif":
                case "while":
                    return new ConditionalExpressionContext(token, true);
                case "else":
                    return new ConditionalExpressionContext(token, false);
            }
            return new ExpressionContext(token);

        }

        public override string ToString() => "Scope Body";
    }
}