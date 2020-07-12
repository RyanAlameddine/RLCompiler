using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLTokenizer.Scopes
{
    /// <summary>
    /// Small scope in the namespace declaration
    /// </summary>
    internal class NamespaceContext : Context
    {
        public LinkedList<string> Namespaces { get; private set; } = new LinkedList<string>();

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewline())
            {
                if (Namespaces.Count == 0)
                {
                    throw new TokenizationException("No namespace identifier specified");
                }
                return (true, Parent);
            }

            if (token.IsNewlineOrWhitespace()) return (true, this);

            if (next.ToString().IsNewlineOrWhitespace() || next == '.')
            {
                if (token.Length > 0 && token[0] == '.') token = token.Substring(1, token.Length - 1);

                if (token.IsIdentifier())
                {
                    Namespaces.AddLast(token);
                    return (true, this);
                }
                throw new TokenizationException("Namespace token is not a valid identifier");
            }

            return (false, this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Namespace: ");
            foreach(var ns in Namespaces)
            {
                sb.Append(ns);
                sb.Append('.');
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}