using System;

namespace WFBind.Exceptions
{
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
