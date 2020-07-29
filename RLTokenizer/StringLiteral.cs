using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser
{
    public class StringLiteral : Context
    {
        public string String { get; set; }

        public StringLiteral(string String)
        {
            this.String = String.Substring(1, String.Length - 2);
        }

        public override string ToString() => "String literal: " + String;

        public override (bool, Context) Evaluate(char previous, string token, char next) => throw new NotImplementedException();
    }
}
