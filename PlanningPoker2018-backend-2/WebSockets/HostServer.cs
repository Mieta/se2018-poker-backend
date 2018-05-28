using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class HostServer
    {
        private WebSocketServer server;
        private static List<IWebSocketConnection> activeSockets = new List<IWebSocketConnection>();

        public HostServer()
        {
            server = new WebSocketServer("ws://0.0.0.0:3001");
            server.Start(socket =>
            {
                socket.OnOpen = () => handleNewSocket(socket);
                socket.OnClose = () => handleSocketClose(socket);
                socket.OnMessage = message => activeSockets.ForEach(s => s.Send(message));
            });
        }

        private static void handleSocketClose(IWebSocketConnection socket)
        {
            activeSockets.Add(socket);
        }

        private static void handleNewSocket(IWebSocketConnection socket)
        {
            activeSockets.Remove(socket);
        }
    }
}
