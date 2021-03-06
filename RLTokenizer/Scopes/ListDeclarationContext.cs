﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RLParser.Scopes
{
    public class ListDeclarationContext : Context
    {
        private LinkedListNode<Context> lastPackaged = null;
        private readonly bool packageOnExit;
        private bool expressionComplete = false;
        private bool forStatement = false;
        public bool IsListComprehension { get; private set; } = false;

        public ListDeclarationContext(bool packageOnExit)
        {
            this.packageOnExit = packageOnExit;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (IsListComprehension)
            {
                if (previous == ']')
                {
                    if (Children.Count < 3) throw new CompileException("Incomplete list comprenehsion");
                    return Parent.Evaluate(previous, token, next);
                }
                if (forStatement)
                {
                    if (token.IsWhitespace()) return (true, this);
                    if (token == "i") return (false, this);
                    forStatement = false;
                    if (token == "in") return (true, RegisterChild(new ExpressionContext(new Regex("^(]|if)$"))));
                    
                    throw new CompileException($"Unexpected character '{token}' found instead of \"in\" in list comprehension");
                }

                if (previous == 'f')
                    return RegisterChild(new ConditionalExpressionContext
                        ("if", true, new Regex("^\\]$"))).Evaluate(previous, token, next);

                if (token.IsWhitespace()) return (true, this);

                throw new CompileException("Incomplete list comprenehsion");
            }

            if (expressionComplete)
            {
                expressionComplete = false;
                if (previous == ',') { }
                else if(previous == ']')
                {
                    return Parent.Evaluate(previous, token, next);
                }
                else if(previous == 'r')
                {
                    IsListComprehension = true;
                    forStatement = true;
                    if (!token.IsWhitespace()) throw new CompileException("No space found after for in list comprehension");
                    return (true, RegisterChild(new IdentifierContext()));
                }
                else
                {
                    throw new CompileException("List declaration incomplete");
                }
            }

            //if (token == "]")
            //{
            //    if(Children.Count != 0) PackageChildren();
            //    if (!packageOnExit)
            //    {
            //        if (Parent is ListDeclarationContext) return (true, Parent);
            //        return (true, Parent.Parent);
            //    }
            //    return PackageToParent();
            //}
            //if (token.IsNewlineOrWhitespace()) return (true, this);

            //if(token == ",")
            //{
            //    if (commaPresent) throw new TokenizationException("Two commas found in a row in list declaration");
            //    if (Children.Count == 0) throw new TokenizationException("No items found before comma");
            //    commaPresent = true;

            //    PackageChildren();

            //    return (true, this);
            //}

            //if (token == "." && next == '.') return (false, this);
            //if(token == "..")
            //{
            //    PackageChildren();
            //    var newChild = new OperatorIdentifierContext();
            //    newChild.Identifier = token;
            //    newChild.Parent = this;
            //    Children.AddLast(newChild);

            //    lastPackaged = lastPackaged.Next;
            //    return (true, this);
            //}

            //commaPresent = false;

            expressionComplete = true;

            var newExpression = new ExpressionContext(new Regex("^(\\]|,|for)$"));
            newExpression.Parent = this;
            Children.AddLast(newExpression);
            
            return newExpression.Evaluate(previous, token, next);
            
            //return ExpressionContext.CheckExpressions(previous, token, next, this, false, new Regex("^\\]$"));
        }

        private void PackageChildren()
        {
            if (lastPackaged == null) PackageChildFrom(Children.First);
            else PackageChildFrom(lastPackaged.Next);
            lastPackaged = Children.Last;
        }

        private void PackageChildFrom(LinkedListNode<Context> current)
        {
            var newChild = new ExpressionContext();
            newChild.Parent = this;
            while(current != null)
            {
                var next = current.Next;
                Children.Remove(current);
                current.Value.Parent = newChild;
                newChild.Children.AddLast(current.Value);

                current = next;
            }
            Children.AddLast(newChild);
        }

        private (bool, Context) PackageToParent()
        {
            var newParent = new ExpressionContext();

            Parent.Children.RemoveLast();
            Parent.Children.AddLast(newParent);

            newParent.Parent = Parent;
            newParent.Children.AddLast(this);

            Parent = newParent;


            return (true, Parent);
        }

        public override string ToString() => IsListComprehension ? "List Comprehension" : "List Declaration";
    }
}