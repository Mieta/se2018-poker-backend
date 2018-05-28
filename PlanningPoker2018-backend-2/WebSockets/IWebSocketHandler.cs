using Fleck;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public interface IWebSocketHandler
    {
        void handleNewSocket(IWebSocketConnection socket);
        void handleSocketClose(IWebSocketConnection socket);
        void handleNewMessage(string message);

    }
}