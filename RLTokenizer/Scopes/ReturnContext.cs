namespace RLParser.Scopes
{
    class ReturnContext : Context
    {
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if(Children.Count == 0)
            {
                if (token.IsWhitespace()) return (true, new ExpressionContext());
            }

            if (!token.IsNewlineOrWhitespace()) throw new TokenizationException("Unexpected character found in return statement");
            return (true, Parent);
        }

        public override string ToString() => $"Return Statement";
    }
}