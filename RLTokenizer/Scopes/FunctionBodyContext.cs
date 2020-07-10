namespace RLTokenizer.Scopes
{
    class FunctionBodyContext : Context
    {
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token == "}") return (true, Parent.Parent);
            return (true, this);
        }

        public override string ToString() => "Function Body";
    }
}