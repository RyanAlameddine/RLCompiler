using RLParser.Scopes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RLParser
{
    public static class RLParser
    {
        public static Context Parse(string code, Action<TokenizationException, Context> onError)
        {
            Context root = new FileContext();

            //The current tokenization scope
            Context scope = root;

            //Used to check with the scope is changed
            Context previousScope = scope;
            ReadOnlySpan<char> codeSpan = code;
            bool reset = true;

            int tokenStart = 0;
            int lineNumber = 1;
            for (int i = 0; i < codeSpan.Length; i++)
            {
                char previous = i == 0 ? ' ' : codeSpan[i - 1];
                char next = i + 1 == codeSpan.Length ? ' ' : codeSpan[i + 1];
                string token = codeSpan.Slice(tokenStart, i - tokenStart + 1).ToString();

                if (codeSpan[i] == '\n')
                {
                    lineNumber++;
                }
                if (CommentCheck(token, ref reset))
                {
                    scope.RegisterNewCharReached(i, lineNumber);
                    try
                    {
                        (reset, scope) = scope.Evaluate(previous, token, next);
                    }
                    catch (TokenizationException e)
                    {
                        e.Start = tokenStart;
                        e.End = tokenStart + token.Length;
                        onError(e, scope);
                        
                        while (!codeSpan[i].ToString().IsNewline() || i == codeSpan.Length)
                        {
                            i++;
                            if (i >= codeSpan.Length - 1) break;
                        }
                        while(scope is ExpressionContext || scope is ListDeclarationContext || scope is VariableDefinitionContext || scope is VariableOrIdentifierDefinitionContext)
                            scope = scope.Parent;
                        tokenStart = i;
                    }
                }

                if (scope != previousScope && scope.Parent == null)
                {
                    scope.Parent = previousScope;
                    previousScope.Children.AddLast(scope);
                }
                previousScope = scope;

                if (reset) tokenStart = i + 1;
            }

            if (tokenStart < codeSpan.Length - 1 || scope != root)
            {
                onError(new TokenizationException("Invalid or incomplete namespace/class declaration"), scope);
            }
            return root;
        }

        private static bool CommentCheck(string token, ref bool reset)
        {
            if(token[0] == '#')
            {
                reset = false;
                if (token[^1] == '\n')
                {
                    reset = true;
                }
                return false;
            }
            return true;
        }
    }
}
