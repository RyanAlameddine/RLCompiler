using System.Text.RegularExpressions;

namespace RLTokenizer.Scopes
{
    internal class ConditionalExpressionContext : Context
    {
        private readonly bool hasCondition;
        private bool scopeComplete;

        public string Statement { get; }

        public ConditionalExpressionContext(string statement, bool hasCondition)
        {
            this.hasCondition = hasCondition;
            Statement = statement;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (scopeComplete)
            {
                return (true, Parent);
            }

            if (hasCondition)
            {
                if(Children.Count == 0)
                {
                    if (!token.IsWhitespace()) throw new TokenizationException("No whitespace after if statement");
                    return (true, new ExpressionContext(new Regex("^{$")));
                }
            }

            if(token == "{")
            {
                scopeComplete = true;
                return (true, new ScopeBodyContext());
            }
            if (!token.IsNewlineOrWhitespace()) throw new TokenizationException($"No whitespace after {Statement} statement");
            return (true, this);
            
        }

        public override string ToString() => $"{Statement} statement header";
    }
}