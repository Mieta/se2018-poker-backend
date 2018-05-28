using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Fleck.Interfaces;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class HostServer : AppWebSocketServer
    {
        private static HostServer _instance;

        public static HostServer Instance => _instance;

        public static void initialize()
        {
            _instance = new HostServer();
        }


        private readonly ConcurrentDictionary<string, IWebSocketConnection> _activeSockets =
            new ConcurrentDictionary<string, IWebSocketConnection>();

        public HostServer() : base("ws://0.0.0.0:3001")
        {
        }

        public override void handleSocketClose(IWebSocketConnection socket)
        {
            var keyToRemove = _activeSockets.First(s => s.Value == socket).Key;
            _activeSockets.Remove(keyToRemove, out var removedSocket);
            removedSocket.Close();
        }

        public override void handleNewMessage(IWebSocketConnection socket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            var messageType = parsedMessage.type;
            if (messageType == "init-ws")
            {
                _activeSockets.TryAdd(parsedMessage.roomId, socket);
                var confirmationMessage =
                    new WebSocketMessage() {type = "sockets-ready", roomId = parsedMessage.roomId};
                var serializedMessage = JsonConvert.SerializeObject(confirmationMessage);
                socket.Send(serializedMessage);
            }
            else
            {
                ClientServer.Instance.sendToAllInRoom(parsedMessage.roomId, message);
            }
        }

        public override void handleNewSocket(IWebSocketConnection socket)
        {
            
        }

        public void sendToHost(string roomId, string message)
        {
            _activeSockets[roomId].Send(message);
        }
    }
}