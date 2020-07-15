using System;

namespace RLParser.Scopes
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

            if (token == "[")
            {
                var newExpression = RegisterChild(new ExpressionContext());
                
                return (true, newExpression.RegisterChild(new ListDeclarationContext(true)));
            }

            if (!(token + next).IsNumber())
            {
                if (token.IsNumber())
                {
                    var newChild = new ExpressionContext();
                    newChild.RegisterChild(new IntLiteral(int.Parse(token)));
                    return (true, RegisterChild(newChild));
                }
            }
            if (token.IsNumber())
            {
                return (false, this);
            }

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
                    return new ReturnContext();
                case "var":
                    return new VariableDefinitionContext(AccessModifiers.Scope);
                case "if":
                case "elif":
                case "while":
                    return new ConditionalExpressionContext(token, true);
                case "else":
                    return new ConditionalExpressionContext(token, false);
                case "for":
                    return new ForLoopContext();
            }
            return new ExpressionContext(token);

        }

        public override string ToString() => "Scope Body";
    }
}