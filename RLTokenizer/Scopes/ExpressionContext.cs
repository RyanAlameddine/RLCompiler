using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RLTokenizer.Scopes
{
    class ExpressionContext : Context
    {
        private Regex otherExitCharacters;

        public bool Parenthesis { private get; set; } = false;

        public ExpressionContext() { }
        public ExpressionContext(string identifier)
        {
            AddIdentifierChild(identifier);
        }

        public ExpressionContext(Regex otherExitCharacters)
        {
            this.otherExitCharacters = otherExitCharacters;
        }

        private void AddIdentifierChild(string id)
        {
            var child = new IdentifierContext
            {
                Identifier = id,
                Parent = this
            };
            Children.AddLast(child);
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewline() || (otherExitCharacters != null && otherExitCharacters.IsMatch(token)))
            {
                if (Parent is ExpressionContext)
                {
                    throw new TokenizationException("Incomplete expression statement");
                }
                return (true, Parent);
            }

            if (token == "-" && next == '>') return (false, this);

            if (token == "->")
            {
                if (Parent is ExpressionContext)
                {
                    throw new TokenizationException("Incomplete expression statement");
                }
                return (true, new VariableAssignmentIdentifier());
            }

            if (token[0] == '"')
            {
                if (token[^1] == '"' && token.Length != 1)
                {
                    var child = new StringLiteral(token);
                    child.Parent = this;
                    Children.AddLast(child);
                    return (true, this);
                }
                return (false, this);
            }

            if (token.IsWhitespace()) return (true, this);

            if (token.IsIdentifier())
            {
                if(!(token + next).IsIdentifier())
                {
                    AddIdentifierChild(token);
                    return (true, this);
                }
                return (false, this);
            }

            if (token.IsOperator())
            {
                if(!(token + next).IsOperator())
                {
                    var child = new OperatorIdentifierContext
                    {
                        Identifier = token,
                        Parent = this
                    };
                    Children.AddLast(child);
                    return (true, this);
                }
                return (false, this);
            }

            if (token.IsNumber())
            {
                if (!(token + next).IsNumber())
                {
                    var child = new IntLiteral(int.Parse(token));
                    child.Parent = this;
                    Children.AddLast(child);
                    return (true, this);
                }
                return (false, this);
            }

            if (token == "(")
            {
                var child = new ExpressionContext();
                child.Parenthesis = true;
                return (true, child);
            }

            if(token == ")")
            {
                if (Parenthesis) return (true, Parent);
                throw new TokenizationException("Closing parenthesis found without open parenthesis");
            }

            throw new TokenizationException("Unknown character found in statment");
        }

        public override string ToString() => "Expression Context";
    }
}
