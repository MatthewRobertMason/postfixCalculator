using System;
using System.Runtime.Serialization;

namespace PostfixBoolean
{
    public class InvalidEquationException : Exception
    {
        public InvalidEquationException()
        {
        }

        public InvalidEquationException(string message) : base(message)
        {
        }

        public InvalidEquationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidEquationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
