using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLParser.Scopes
{
    /// <summary>
    /// Small scope in the namespace declaration
    /// </summary>
    public class NamespaceContext : Context
    {
        public string Namespace { get; private set; }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewline())
            {
                if (Namespace == null)
                {
                    throw new CompileException("No namespace identifier specified");
                }
                return (true, Parent);
            }

            if (token.IsNewlineOrWhitespace()) return (true, this);

            if (next.ToString().IsNewlineOrWhitespace() || next == '.')
            {
                if (token.IsIdentifier())
                {
                    if (Namespace != null) Namespace += '.';
                    Namespace += token;
                    return (true, this);
                }
                throw new CompileException("Namespace token is not a valid identifier");
            }

            if(token.IsIdentifier())
                return (false, this);

            if (token == ".") return (true, this);

            throw new CompileException("Invalid identifier in namespace declaration");
        }

        public override string ToString() => $"Namespace: {Namespace}";
    }
}