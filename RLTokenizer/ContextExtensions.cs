using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser.Extensions
{
    public static class ContextExtensions
    {
        public static Context GetFirstAtChar(this Context root, int c)
        {
            if (root.Characters.Start.Value <= c && root.Characters.End.Value >= c) return root;
            foreach(var child in root.Children)
            {
                var result = GetFirstAtChar(child, c);
                if (result == null) continue;
                return result;
            }
            return null;
        }
    }
}
