using System;
using System.Runtime.Serialization;

namespace RLParser
{
    [Serializable]
    public class CompileException : Exception
    {
        public int Start;
        public int End;

        public CompileException()
        {
        }

        public CompileException(string message) : base(message)
        {
        }

        public CompileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CompileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}