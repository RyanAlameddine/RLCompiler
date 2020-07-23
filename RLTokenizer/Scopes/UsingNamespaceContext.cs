using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLParser.Scopes
{
    public class UsingNamespaceContext : NamespaceContext
    {
        public override string ToString() => "Using " + base.ToString();
    }
}