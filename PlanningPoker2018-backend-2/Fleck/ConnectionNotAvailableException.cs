using System;

namespace PlanningPoker2018_backend_2.Fleck
{
    public class ConnectionNotAvailableException : Exception
    {
        public ConnectionNotAvailableException() : base()
        {
        }

        public ConnectionNotAvailableException(string message) : base(message)
        {
        }

        public ConnectionNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
