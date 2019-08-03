using System;
using System.Runtime.Serialization;

namespace postfix_calculator
{
    public class InvalidOperandException : Exception
    {
        public InvalidOperandException()
        {
        }

        public InvalidOperandException(string message) : base(message)
        {
        }

        public InvalidOperandException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidOperandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}