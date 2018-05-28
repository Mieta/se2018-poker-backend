using System.Collections.Generic;
using Fleck;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class HostServer : AppWebSocketServer

    {
        private static readonly List<IWebSocketConnection> _activeSockets = new List<IWebSocketConnection>();

        public HostServer() : base("ws://localhost:3001")
        {
        }

        public override void handleSocketClose(IWebSocketConnection socket)
        {
            _activeSockets.Add(socket);
        }

        public override void handleNewMessage(string message)
        {
            _activeSockets.ForEach(s => s.Send(message));
        }

        public override void handleNewSocket(IWebSocketConnection socket)
        {
            _activeSockets.Remove(socket);
        }
    }
}