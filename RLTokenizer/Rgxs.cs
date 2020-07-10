using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RLTokenizer
{
    static class Rgxs
    {
        public static bool IsWhitespace      (this string t) => new Regex("^( |\t)+$"      ).IsMatch(t);
        public static bool IsLineOrWhitespace(this string t) => new Regex("^( |\n|\r|\t)+$").IsMatch(t);
        public static bool IsNewline         (this string t) => new Regex("^(\n|\r)+$"     ).IsMatch(t);
        public static bool IsIdentifier      (this string t) => new Regex("^[a-zA-Z_]\\w*$").IsMatch(t);
    }
}
