using System;
using System.Diagnostics.CodeAnalysis;

namespace WFBind.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidBindingException : Exception
    {
        public InvalidBindingException()
        {
        }

        public InvalidBindingException(string message) : base(message)
        {
        }

        public InvalidBindingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
