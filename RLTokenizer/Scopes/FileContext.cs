﻿using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RLParser
{
    class FileContext : Context
    {
        public FileContext()
        {
            Parent = this;
        }

        public override (bool, Context) Evaluate(char previous, string token, char next)
        {
            if (token.IsNewlineOrWhitespace()) return (true, this);
                
            if(token == "namespace")
            {
                return (true, new NamespaceContext());
            }
            if(token == "class")
            {
                return (true, new ClassHeaderContext());
            }

            return (false, this);
        }

        public override string ToString() => "File";
    }
}