using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace SECCS.Exceptions
{
    public class InvalidHeaderException : Exception
    {
        internal InvalidHeaderException()
        {
        }

        internal InvalidHeaderException(string message) : base(message)
        {
        }

        internal InvalidHeaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidHeaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public static Expression Throw(string msg)
            => Expression.Throw(Expression.New(
                typeof(InvalidHeaderException).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null),
                Expression.Constant(msg)));
    }
}
