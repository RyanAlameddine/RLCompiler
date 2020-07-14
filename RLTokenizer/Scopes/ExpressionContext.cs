using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RLTokenizer.Scopes
{
    class ExpressionContext : Context
    {
        private Regex otherExitCharacters;

        public bool Parenthesis { private get; set; } = false;

        public bool IsFunctionCall 
        {
            get => Children.Count > 1
                && Children.First.Value is IdentifierContext function
                && Children.Where((x) => x is OperatorIdentifierContext).Count() == 0;
        }

        public ExpressionContext() { }
        public ExpressionContext(string identifier)
        {
            var child = new IdentifierContext
            {
                Identifier = identifier,
                Parent = this
            };
            Children.AddLast(child);
        }

        public ExpressionContext(Regex otherExitCharacters)
        {
            this.otherExitCharacters = otherExitCharacters;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewline() || (otherExitCharacters != null && otherExitCharacters.IsMatch(token)))
            {
                var ret = Parent;
                while (ret is ExpressionContext) ret = ret.Parent;
                return (true, ret);
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

            return CheckExpressions(previous, token, next, this, Parenthesis, otherExitCharacters);
        }

        public static (bool, Context) CheckExpressions(char previous, string token, char next, Context context, bool parenthesis, Regex otherExitCharacters)
        {
            if (token[0] == '"')
            {
                if (token[^1] == '"' && token.Length != 1)
                {
                    var child = new StringLiteral(token);
                    child.Parent = context;
                    context.Children.AddLast(child);
                    return (true, context);
                }
                return (false, context);
            }

            if (token.IsWhitespace()) return (true, context);

            if (token.IsIdentifier())
            {
                if (!(token + next).IsIdentifier())
                {
                    var child = new IdentifierContext
                    {
                        Identifier = token,
                        Parent = context
                    };
                    context.Children.AddLast(child);
                    return (true, context);
                }
                return (false, context);
            }

            if (token.IsOperator())
            {
                if (!(token + next).IsOperator())
                {
                    var operatorChild = new OperatorIdentifierContext
                    {
                        Identifier = token,
                        Parent = context
                    };
                    if (token.IsSOperator())
                    {
                        var newParent = new ExpressionContext(otherExitCharacters);
                        newParent.Parenthesis = parenthesis;

                        context.Parent.Children.RemoveLast();
                        context.Parent.Children.AddLast(newParent);

                        newParent.Children.AddLast(context);
                        newParent.Parent = context.Parent;
                        context.Parent = newParent;

                        newParent.Children.AddLast(operatorChild);

                        var newChild = new ExpressionContext(otherExitCharacters);
                        newChild.Parent = newParent;
                        newParent.Children.AddLast(newChild);
                        return (true, newChild);
                    }
                    context.Children.AddLast(operatorChild);
                    return (true, new ExpressionContext());
                }
                return (false, context);
            }

            if (token.IsNumber())
            {
                if (!(token + next).IsNumber())
                {
                    var child = new IntLiteral(int.Parse(token));
                    child.Parent = context;
                    context.Children.AddLast(child);
                    return (true, context);
                }
                return (false, context);
            }

            if (token == "(")
            {
                var child = new ExpressionContext();
                child.Parenthesis = true;
                return (true, child);
            }

            if (token == ")")
            {
                if (parenthesis) return (true, context.Parent);
                throw new TokenizationException("Closing parenthesis found without open parenthesis");
            }

            if(token == ".")
            {
                var child = new Dot();
                child.Parent = context;
                context.Children.AddLast(child);
                return (true, context);
            }

            //TODO fix this
            if (token == "[") return (true, new ListDeclarationContext(false));

            throw new TokenizationException("Unknown character found in statment");
        }

        public override string ToString() 
            => "Expression Context" + (IsFunctionCall ? $" (Calling function {(Children.First.Value as IdentifierContext).Identifier})" : "");
    }
}
