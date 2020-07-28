using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RLParser.Extensions
{
    public static class ContextExtensions
    {
        public static Context GetFirstAtChar(this Context root, int c)
        {
            foreach(var child in root.Children)
            {
                var result = GetFirstAtChar(child, c);
                if (result == null) continue;
                return result;
            }
            if (root.Characters.Start.Value <= c && root.Characters.End.Value >= c)
            {
                return root;
            }
            return null;
        }
    }
}
