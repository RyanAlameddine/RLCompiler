using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    class ExpressionContext : Context
    {
        protected Regex otherExitCharacters;

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

        public ExpressionContext(Regex otherExitCharacters, bool Parenthesis)
        {
            this.otherExitCharacters = otherExitCharacters;
            this.Parenthesis = Parenthesis;
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
                Context ret = this;
                while (ret.Parent is ExpressionContext) ret = ret.Parent;

                var newParent = new ExpressionContext(otherExitCharacters, Parenthesis);
                ret.Parent.Children.RemoveLast();
                ret.Parent.Children.AddLast(newParent);
                newParent.Parent = ret.Parent;

                ret.Parent = newParent;
                newParent.Children.AddLast(ret);

                return (true, newParent.RegisterChild(new VariableAssignmentIdentifier()));
            }

            if (token[0] == '"')
            {
                if (token[^1] == '"' && token.Length != 1)
                {
                    var child = new StringLiteral(token);

                    var e = CheckEndOp(child);
                    if (e != default) return e;

                    child.Parent = this;
                    Children.AddLast(child);
                    return (true, this);
                }
                return (false, this);
            }

            if (token.IsWhitespace()) return (true, this);

            if (token.IsIdentifier())
            {
                if (!(token + next).IsIdentifier())
                {
                    var child = new IdentifierContext
                    {
                        Identifier = token,
                        Parent = this
                    };
                    var e = CheckEndOp(child);
                    if (e != default) return e;

                    RegisterChild(new ExpressionContext().RegisterChild(child));
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

                    var e = CheckEndOp(child);
                    if (e != default) return e;

                    child.Parent = this;
                    this.Children.AddLast(child);
                    return (true, this);
                }
                return (false, this);
            }

            if (token.IsOperator())
            {
                if (!(token + next).IsOperator())
                {
                    var operatorChild = new OperatorIdentifierContext
                    {
                        Identifier = token,
                        Parent = this
                    };
                    if (token.IsSpltOperator())
                    {
                        var topParent = Parent;
                        while (topParent.GetType() == typeof(ExpressionContext)) topParent = topParent.Parent;

                        var oldTop = topParent.Children.Last.Value;
                        var newTop = new ExpressionContext(otherExitCharacters, Parenthesis);
                        topParent.Children.RemoveLast();
                        topParent.Children.AddLast(newTop);
                        newTop.Parent = topParent;

                        newTop.Children.AddLast(oldTop);
                        oldTop.Parent = newTop;

                        newTop.Children.AddLast(operatorChild);

                        var newChild = new ExpressionContext(otherExitCharacters, Parenthesis);

                        newTop.Children.AddLast(newChild);
                        newChild.Parent = newTop;

                        return (true, newChild);
                    }

                    if(Children.Count == 0 && (token == "+" || token == "-"))
                    {
                        RegisterChild(new IntLiteral(0));
                    }

                    Children.AddLast(operatorChild);

                    if (token.IsEndOperator())
                    {
                        return (true, this);
                    }
                    return (true, RegisterChild(new ExpressionContext(otherExitCharacters, Parenthesis)));
                }
                return (false, this);
            }

            if (token == "(")
            {
                var child = RegisterChild(new ExpressionContext(null, true));
                return (true, child);
            }

            if (token == ")")
            {
                if (Parenthesis) return (true, Parent);
                throw new CompileException("Closing parenthesis found without open parenthesis");
            }

            if (token == ".")
            {
                if (next == '.') return (false, this);
                var child = new Dot();
                child.Parent = this;
                Children.AddLast(child);
                return (true, this);
            }
            if(token == "..")
            {
                var dotdot = RegisterChild(new OperatorIdentifierContext());
                dotdot.Identifier = token;
                return (true, this);
            }

            //TODO fix this
            if (token == "[")
            {
                return (true, RegisterChild(new ListDeclarationContext(false)));
            }

            throw new CompileException("Unknown character found in statment");
        }

        private (bool, Context) CheckEndOp(Context child)
        {
            if (!IsFunctionCall
                        && Children.Count == 2
                        && (Children.First.Next.Value is OperatorIdentifierContext o)
                        && o.Identifier.IsEndOperator())
            {
                var newParent = new ExpressionContext(otherExitCharacters, Parenthesis);
                newParent.Parenthesis = Parenthesis;

                Parent.Children.RemoveLast();
                Parent.Children.AddLast(newParent);

                newParent.Children.AddLast(this);
                newParent.Parent = Parent;
                Parent = newParent;

                Children.AddLast(child);

                return (true, newParent);
            }
            return default;
        }

        public override string ToString() 
            => "Expression Context" + (IsFunctionCall ? $" (Calling function {(Children.First.Value as IdentifierContext).Identifier})" : "");
    }
}
