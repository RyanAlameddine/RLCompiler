namespace RLTokenizer
{
    internal class Dot : Context
    {
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => "Dot";
    }
}