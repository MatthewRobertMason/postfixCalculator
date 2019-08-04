using System;
using System.Runtime.Serialization;

namespace postfixBoolean
{
    public class InvalidVariableException :Exception
    {
        public InvalidVariableException()
        {
        }

        public InvalidVariableException(string message) : base(message)
        {
        }

        public InvalidVariableException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidVariableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
