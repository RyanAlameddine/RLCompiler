using System;
using System.Runtime.Serialization;

namespace RLParser
{
    [Serializable]
    public class TokenizationException : Exception
    {
        public TokenizationException()
        {
        }

        public TokenizationException(string message) : base(message)
        {
        }

        public TokenizationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenizationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}