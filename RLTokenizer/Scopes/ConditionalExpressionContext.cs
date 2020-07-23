using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    public class ConditionalExpressionContext : ExpressionContext
    {
        private readonly bool hasCondition;

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
            if (otherExitCharacters != null && otherExitCharacters.IsMatch(token))
            {
                return Parent.Evaluate(previous, token, next);
            }

            if (!token.IsNewlineOrWhitespace()) 
                throw new CompileException($"No whitespace after {Statement} statement");
            return RegisterChild(new ExpressionContext(otherExitCharacters)).Evaluate(previous, token, next);
        }

        public override string ToString() => $"{Statement} statement header";
    }
}