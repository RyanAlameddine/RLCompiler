using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser.Scopes
{
    public class OperatorIdentifierContext : IdentifierContext
    {
        public override string ToString() => $"Operator: {Identifier}";
    }
}
