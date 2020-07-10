using System;
using System.Collections.Generic;
using System.Text;

namespace RLTokenizer
{
    public abstract class Context
    {            
        public Context Parent { get; set; }
        public LinkedList<Context> Children { get; } = new LinkedList<Context>();
        //public (string regex, Func<IScope> newScope)[] Rules { get; }

        /// <returns>true if the token should be cleared</returns>
        public abstract (bool, Context) Evaluate(char previous, string token, char next);

        public void ConsolePrint()
        {
            ConsolePrint(0);
        }

        private void ConsolePrint(int depth)
        {
            for (int i = 0; i < depth; i++) Console.Write(' ');
            Console.WriteLine(ToString());
            foreach(var child in Children)
            {
                child.ConsolePrint(depth + 1);
            }
        }
    }
}
