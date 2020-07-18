using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser
{
    public abstract class Context
    {            
        public Context Parent { get; set; }
        public LinkedList<Context> Children { get; } = new LinkedList<Context>();

        public Range Characters { get; set; }
        public Range Lines { get; set; }
        private bool rangesSet;

        //public (string regex, Func<IScope> newScope)[] Rules { get; }

        /// <returns>true if the token should be cleared</returns>
        public abstract (bool, Context) Evaluate(char previous, string token, char next);

        public T RegisterChild<T>(T context) where T : Context
        {
            context.Parent = this;
            context.Lines = Lines;
            context.Characters = Characters;
            this.Children.AddLast(context);

            return context;
        }

        public void RegisterNewCharReached(int character, int lineNumber)
        {
            if (!rangesSet)
            {
                Characters = new Range(character, character);
                Lines = new Range(lineNumber, lineNumber);
                rangesSet = true;
            }
            else 
            {
                Characters = new Range(Characters.Start, character);
                Lines = new Range(Lines.Start, lineNumber);
            }
        }

        public void ConsolePrint()
        {
            ConsolePrint(0);
        }

        private void ConsolePrint(int depth)
        {
            int color = 1;
            Console.Write(Lines.Start.Value.ToString("D3"));
            for (int i = 0; i < depth; i++)
            {
                if (color > 15) color -= 15;
                Console.ForegroundColor = ((ConsoleColor) (color));
                Console.Write('|');
                color += 2;
            }
            Console.ResetColor();
            Console.WriteLine(ToString());
            if(Parent == null)
            {
                Console.WriteLine("this shouldnt work");
            }
            foreach(var child in Children)
            {
                child.ConsolePrint(depth + 1);
            }
        }
    }
}
