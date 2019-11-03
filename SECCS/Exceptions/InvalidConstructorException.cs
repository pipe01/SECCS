using System;

namespace SECCS.Exceptions
{
    public sealed class InvalidConstructorException : Exception
    {
        public InvalidConstructorException() : base()
        {
        }

        public InvalidConstructorException(string message) : base(message)
        {
        }

        public InvalidConstructorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
