using System;
using System.Collections.Generic;
using System.Text;

namespace RLTokenizer.Scopes
{
    class OperatorIdentifierContext : IdentifierContext
    {
        public override string ToString() => $"Operator: {Identifier}";
    }
}
