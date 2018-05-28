using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Fleck.Interfaces;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class ClientServer : AppWebSocketServer
    {
        
        private static ClientServer _instance;

        public static ClientServer Instance => _instance;

        public static void initialize()
        {
            _instance = new ClientServer();
        }
        
        private readonly ConcurrentDictionary<string, List<IWebSocketConnection>> _activeSockets = new ConcurrentDictionary<string, List<IWebSocketConnection>>();

        public ClientServer() : base("ws://0.0.0.0:3002")
        {
        }

        public override void handleSocketClose(IWebSocketConnection socket)
        {
            var socketsList = _activeSockets.Values.First(list => list.Contains(socket));
            socketsList.Remove(socket);
        }

        public override void handleNewMessage(IWebSocketConnection socket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            if (parsedMessage.type == "init-ws")
            {
                if (!_activeSockets.ContainsKey(parsedMessage.roomId))
                {
                    _activeSockets.TryAdd(parsedMessage.roomId, new List<IWebSocketConnection>());
                }
                _activeSockets[parsedMessage.roomId].Add(socket);
                var confirmationMessage =
                    new WebSocketMessage() {type = "sockets-ready", roomId = parsedMessage.roomId};
                var serializedMessage = JsonConvert.SerializeObject(confirmationMessage);
                socket.Send(serializedMessage);
            }
            else
            {
                HostServer.Instance.sendToHost(parsedMessage.roomId, message);   
            }
        }

        public override void handleNewSocket(IWebSocketConnection socket)
        {
        }

        public void sendToAllInRoom(string roomId, string message)
        {
            _activeSockets[roomId].ForEach(s => s.Send(message));
        }
    }
}