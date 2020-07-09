/*
class Program
{
    static int AverageTwoNumbers(int a, int b)
    {
        return (a + b) / 2;
    }

    static void Main(string[] args)
    {
        int a = 34;
        int b = 42;

        var name = "Bob";
        var number = AverageTwoNumbers(a, b);
    }
}
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BasicTokenizer
{
    class Program
    {
        static List<KeyValuePair<string, Token>> tokenz = new List<KeyValuePair<string, Token>>();


        static List<(string regex, Token tokenMatch)> tokenMatches = new List<(string, Token)>()
        {
            ("public|private", Token.AccessModifier),
            ("class",          Token.ClassKeyword),
            ("static",         Token.FunctionKeyword),
            ("int|string",     Token.TypeKeyword),
            ("[0-9]+",         Token.Integer),
            (" |\n|\r",        Token.Whitespace),
            (@"\{|\}|\(\)",    Token.Bracket),
            (@"\+|\-|\/",      Token.Operators),
            (";",              Token.Whitespace),
        };

        //static char[] separators = new char[] { ' ', '{', '}', '=', '(', ')' };

        static string code = @"class Program
{
    static int AverageTwoNumbers(int a, int b)
    {
        return (a + b) / 2;
    }

    static void Main(string[] args)
    {
        int a = 34;
        int b = 42;

        var name = nameof(a);
        var number = AverageTwoNumbers(a, b);
    }
}";


        static void Main(string[] args)
        {
            ReadOnlySpan<char> currentToken = new ReadOnlySpan<char>();
            ReadOnlySpan<char> codeSpan = code;

            int tokenStart = 0;

            for(int i = 0; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '{':
                    case '}':
                    case '(':
                    case ')':
                    case ' ':
                    case '\n':
                    case '\r':
                    case ',':
                        tokenStart = i + 1;
                        if (currentToken.Length != 0)
                        {
                            Evaluate(new string(currentToken));
                            currentToken = new ReadOnlySpan<char>();
                        }
                        Evaluate(new string(codeSpan.Slice(tokenStart - 1, 1)));
                        break;
                    default:
                        currentToken = codeSpan.Slice(tokenStart, i - tokenStart + 1);
                        break;
                }
            }

            foreach(var t in tokenz)
            {
                Console.WriteLine(t.Key + ", " + t.Value);
            }
        }

        static void Evaluate(string token)
        {
            Token val = Token.Identifier;
            for(int i = 0; i < tokenMatches.Count; i++)
            {
                if(Regex.Match(token, tokenMatches[i].regex).Success)
                {
                    val = tokenMatches[i].tokenMatch;
                    break;
                }
            }
            //check if two whitespace in a row
            if(!(val == Token.Whitespace && tokenz[tokenz.Count - 1].Value == val))
                tokenz.Add(new KeyValuePair<string, Token>(token, val));
        }

    }

    enum Token
    {
        AccessModifier,
        ClassKeyword,
        ClassNameIdentifier,
        FunctionKeyword,
        TypeKeyword,
        Identifier,
        Integer,
        Whitespace,
        Bracket,
        Operators
    }
}