using System;
using System.Collections.Generic;
using System.Text;

namespace RLParser
{
    public class Range
    {
        public int Start { get; }
        public int End { get; }

        public Range(int Start, int End)
        {
            this.Start = Start;
            this.End = End;
        }
    }
}
