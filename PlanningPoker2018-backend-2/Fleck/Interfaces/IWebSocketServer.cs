using System;

namespace PlanningPoker2018_backend_2.Fleck.Interfaces
{
    public interface IWebSocketServer : IDisposable
    {
        void Start(Action<IWebSocketConnection> config);
    }
}
