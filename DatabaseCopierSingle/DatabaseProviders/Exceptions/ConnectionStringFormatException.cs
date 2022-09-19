using System;

namespace DatabaseCopierSingle.DatabaseProviders.Exceptions
{
    public class ConnectionStringFormatException : ArgumentException
    {
        public ConnectionStringFormatException()
        {
        }

        public ConnectionStringFormatException(string message)
            : base(message)
        {
        }

        public ConnectionStringFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}