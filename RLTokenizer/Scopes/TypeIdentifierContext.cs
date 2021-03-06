﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser.Scopes
{
    class TypeIdentifierContext : IdentifierContext
    {
        public int ListCount { get; private set; } = 0;
        private int listClosedCount = 0;

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewlineOrWhitespace()) return (true, this);

            if(token == "[")
            {
                ListCount++;
                return (true, this);
            }
            if(token[token.Length - 1] == ']')
            {
                listClosedCount++;
                if (listClosedCount == ListCount)
                {
                    Identifier = token;
                    for (int i = 0; i < listClosedCount; i++) Identifier = '[' + Identifier;
                    return (true, Parent);
                }
                if (listClosedCount > ListCount) throw new CompileException("Closing bracket found with no opening bracket in type declaration");
            }

            if (!(token + next).IsIdentifier() && next != ']')
            {
                if (token.IsIdentifier())
                {
                    Identifier = token;
                    return (true, Parent);
                }
                throw new CompileException("Token is not a valid identifier");
            }

            return (false, this);
        }

        public override string ToString() => $"Type Identifier: {Identifier}" + (ListCount != 0 ? ", list*" + ListCount : "");
    }
}
