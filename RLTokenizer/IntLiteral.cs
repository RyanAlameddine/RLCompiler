using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser
{
    public class IntLiteral : Context
    {
        public int Number { get; set; }

        public IntLiteral(int number)
        {
            Number = number;
        }

        public override string ToString() => "Int literal: " + Number;

        public override (bool, Context) Evaluate(char previous, string token, char next) => throw new NotImplementedException();
    }
}
