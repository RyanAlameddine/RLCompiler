using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace RLParser
{
    static class Rgxs
    {
        public static bool IsNewlineOrWhitespace(this string t) => t.IsWhitespace() || t.IsNewline();

        public static bool IsWhitespace  (this string t) => new Regex("^( |\t|#)+$").IsMatch(t);
        public static bool IsNewline     (this string t) => new Regex("^(\n|\r|#)+$").IsMatch(t);
        public static bool IsIdentifier  (this string t) => new Regex("^[a-zA-Z_][a-zA-Z\\.]*$").IsMatch(t);
        
        public static bool IsOperator    (this string t) => new Regex("^(\\*|\\+|\\-|!|!=|/|=|==|\\||\\|\\||&|&&|:|\\<|\\>||\\>=|\\<=)$").IsMatch(t);
        public static bool IsEndOperator (this string t) => new Regex("^(\\*|\\/|:)$").IsMatch(t);
        public static bool IsSpltOperator(this string t) => new Regex("^(==|!=|\\|\\||&&|\\<|\\>||\\>=|\\<=)$").IsMatch(t);
        
        //public static bool IsPrsnOperator(this string t) => new Regex("^:$").IsMatch(t);
        public static bool IsNumber      (this string t) => new Regex("^[0-9]+$").IsMatch(t);
        public static bool IsString      (this string t) => new Regex("^\"([^\"])*\"$").IsMatch(t);
    }
}
