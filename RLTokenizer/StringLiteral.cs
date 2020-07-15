using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser
{
    class StringLiteral : Context
    {
        public string String { get; set; }

        public StringLiteral(string String)
        {
            this.String = String;
        }

        public override string ToString() => "String literal: " + String;

        public override (bool, Context) Evaluate(char previous, string token, char next) => throw new NotImplementedException();
    }
}
