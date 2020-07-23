namespace RLParser.Scopes
{
    public class ReturnContext : Context
    {
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if(Children.Count == 0)
            {
                if (token.IsWhitespace()) return (true, new ExpressionContext());
            }

            if (!token.IsNewlineOrWhitespace()) throw new CompileException($"Unexpected character '{token}' found in return statement");
            return (true, Parent);
        }

        public override string ToString() => $"Return Statement";
    }
}