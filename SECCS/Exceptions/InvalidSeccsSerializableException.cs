using System;

namespace SECCS.Exceptions
{
    public class InvalidSeccsSerializableException : Exception
    {
        public InvalidSeccsSerializableException() : base()
        {
        }

        public InvalidSeccsSerializableException(string message) : base(message)
        {
        }

        public InvalidSeccsSerializableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
