using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RLTokenizer.Scopes
{
    class ListDeclarationContext : Context
    {
        private bool commaPresent = true;
        private LinkedListNode<Context> lastPackaged = null;
        private readonly bool packageOnExit;

        public ListDeclarationContext(bool packageOnExit)
        {
            this.packageOnExit = packageOnExit;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token == "]")
            {
                if(Children.Count != 0) PackageChildren();
                if (!packageOnExit)
                {
                    if (Parent is ListDeclarationContext) return (true, Parent);
                    return (true, Parent.Parent);
                }
                return PackageToParent();
            }
            if (token.IsNewlineOrWhitespace()) return (true, this);

            if(token == ",")
            {
                if (commaPresent) throw new TokenizationException("Two commas found in a row in list declaration");
                if (Children.Count == 0) throw new TokenizationException("No items found before comma");
                commaPresent = true;

                PackageChildren();

                return (true, this);
            }

            if (token == "." && next == '.') return (false, this);
            if(token == "..")
            {
                PackageChildren();
                var newChild = new OperatorIdentifierContext();
                newChild.Identifier = token;
                newChild.Parent = this;
                Children.AddLast(newChild);

                lastPackaged = lastPackaged.Next;
                return (true, this);
            }

            commaPresent = false;

            return ExpressionContext.CheckExpressions(previous, token, next, this, false, null);
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

        public override string ToString() => "List Declaration";
    }
}