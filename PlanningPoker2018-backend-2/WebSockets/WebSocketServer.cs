using Fleck;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public abstract class AppWebSocketServer : IWebSocketHandler
    {
        protected WebSocketServer server;

        public AppWebSocketServer(string Location)
        {
            server = new WebSocketServer(Location);
            server.Start(socket =>
            {
                socket.OnOpen = () => handleNewSocket(socket);
                socket.OnClose = () => handleSocketClose(socket);
                socket.OnMessage = handleNewMessage;
            });
        }

        public abstract void handleNewSocket(IWebSocketConnection socket);
        public abstract void handleSocketClose(IWebSocketConnection socket);
        public abstract void handleNewMessage(string message);
    }
}