using RLParser.Scopes;

namespace RLParser
{
    public class Dot : OperatorIdentifierContext
    {
        public Dot()
        {
            Identifier = ".";
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => "Dot";
    }
}