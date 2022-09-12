using System;

namespace DatabaseCopierSingle.DatabaseProviders.Exceptions
{
    public class DatabaseConnectionFailedException : Exception
    {
        public DatabaseConnectionFailedException()
        {
        }

        public DatabaseConnectionFailedException(string message) 
            : base(message)
        {
        }

        public DatabaseConnectionFailedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}