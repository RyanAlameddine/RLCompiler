using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser
{
    public abstract class Context
    {            
        public Context Parent { get; set; }
        public LinkedList<Context> Children { get; } = new LinkedList<Context>();

        private Range characters;
        public Range Characters { get => getRangeContext().characters; }
        private Range lines;
        public Range Lines { get => getRangeContext().lines; }
        private bool rangesSet;
        private Context getRangeContext()
        {
            if (Parent == this) return this;
            if (!rangesSet) return Parent.getRangeContext();
            return this;
        }

        //public (string regex, Func<IScope> newScope)[] Rules { get; }

        /// <returns>true if the token should be cleared</returns>
        public abstract (bool, Context) Evaluate(char previous, string token, char next);

        public T RegisterChild<T>(T context) where T : Context
        {
            context.Parent = this;
            context.lines = lines;
            context.characters = characters;
            this.Children.AddLast(context);

            return context;
        }

        public void RegisterNewCharReached(int character, int lineNumber)
        {
            if (!rangesSet)
            {
                characters = new Range(character, character);
                lines = new Range(lineNumber, lineNumber);
                rangesSet = true;
            }
            else 
            {
                characters = new Range(Characters.Start, character);
                lines = new Range(Lines.Start, lineNumber);
            }
        }

        public void ConsolePrint()
        {
            ConsolePrint(0);
        }

        private void ConsolePrint(int depth)
        {
            int color = 1;
            Console.Write(Lines.Start.ToString("D3"));
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
