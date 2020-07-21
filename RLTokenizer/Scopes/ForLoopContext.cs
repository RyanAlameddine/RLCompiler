using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    class ForLoopContext : Context
    {
        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (previous == '}' && Children.Count == 3)
            {
                return (true, Parent);
            }

            if (Children.Count == 2)
            {
                if (previous == '{') return RegisterChild(new ScopeBodyContext()).Evaluate(previous, token, next);
                if (token == "{") return (true, RegisterChild(new ScopeBodyContext()));
            }

            if (Children.Count == 1)
            {
                if (token == "i") return (false, this);
                if (token == "in") return (true, RegisterChild(new ExpressionContext(new Regex("^{$"))));
            }

            if (!token.IsNewlineOrWhitespace()) 
                throw new CompileException("No space found after for declaration");

            if (Children.Count == 0) 
                return (true, RegisterChild(new VariableDefinitionContext(AccessModifiers.Scope)));

            return (true, this);

        }
    }
}