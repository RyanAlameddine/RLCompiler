using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    internal class ConditionalExpressionContext : ExpressionContext
    {
        private readonly bool hasCondition;
        private bool scopeComplete;

        public string Statement { get; }

        public ConditionalExpressionContext(string statement, bool hasCondition, Regex otherExitCharacters = null)
            :base(otherExitCharacters)
        {
            this.hasCondition = hasCondition;
            Statement = statement;
            this.otherExitCharacters = otherExitCharacters;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (scopeComplete || (otherExitCharacters != null && otherExitCharacters.IsMatch(token)))
            {
                return Parent.Evaluate(previous, token, next);
            }

            if (!token.IsWhitespace()) throw new TokenizationException($"No whitespace after {Statement} statement");
            return RegisterChild(new ExpressionContext(otherExitCharacters)).Evaluate(previous, token, next);
        }

        public override string ToString() => $"{Statement} statement header";
    }
}