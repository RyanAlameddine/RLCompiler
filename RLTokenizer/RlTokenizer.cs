using System;
using System.Collections.Generic;
using System.Text;

namespace RLTokenizer
{
    public static class RlTokenizer
    {
        public static Context Tokenize(string code)
        {
            Context root = new FileContext();

            //The current tokenization scope
            Context scope = root;

            //Used to check with the scope is changed
            Context previousScope = scope;
            ReadOnlySpan<char> codeSpan = code;
            bool reset = true;

            int lineNumber = 0;
            int tokenStart = 0;
            for(int i = 0; i < codeSpan.Length; i++)
            {
                char previous = i == 0 ? ' ' : codeSpan[i - 1];
                char next = i + 1 == codeSpan.Length ? ' ' : codeSpan[i + 1];

                try
                {
                    (reset, scope) = scope.Evaluate(previous, codeSpan.Slice(tokenStart, i - tokenStart + 1).ToString(), next);
                }
                catch (TokenizationException e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("On line " + lineNumber);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    return root;
                }

                if (codeSpan[i].ToString().IsNewline())
                {
                    lineNumber++;
                }


                if (scope != previousScope && scope.Parent == null)
                {
                    scope.Parent = previousScope;
                    previousScope.Children.AddLast(scope);
                }
                previousScope = scope;

                if (reset) tokenStart = i + 1;
            }

            if (tokenStart < codeSpan.Length - 1 || scope != root) throw new TokenizationException("Invalid or incomplete namespace/class declaration");
            return root;
        }
    }
}
