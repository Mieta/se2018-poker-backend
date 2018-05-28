using System;

namespace PlanningPoker2018_backend_2.Fleck
{
    public class SubProtocolNegotiationFailureException : Exception
    {
        public SubProtocolNegotiationFailureException() : base() { }
        
        public SubProtocolNegotiationFailureException(string message) : base(message) {}
        
        public SubProtocolNegotiationFailureException(string message, Exception innerException) : base(message, innerException) {}
    }
}

