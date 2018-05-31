using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using PlanningPoker2018_backend_2.Entities;
using PlanningPoker2018_backend_2.Fleck.Interfaces;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class ClientServer : AppWebSocketServer
    {
        
        private static ClientServer _instance;

        public static ClientServer Instance => _instance;

        public static void initialize(IApplicationBuilder app, string path)
        {
            _instance = new ClientServer(app, path);
        }
        
        private readonly ConcurrentDictionary<string, List<AppWebSocket>> _activeSockets = new ConcurrentDictionary<string, List<AppWebSocket>>();

        public ClientServer(IApplicationBuilder app, string path) : base(app, path)
        {
        }

        public async override void handleNewMessage(AppWebSocket socket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            var messageType = parsedMessage.type;
            await HostServer.Instance.sendToHost(parsedMessage.roomId, message);
        }

        public async override Task handleNewSocket(AppWebSocket socket)
        {
            socket.OnMessageReceived += handleNewMessage;
            socket.OnOpen += Socket_OnOpen;
            socket.OnClose += Socket_OnClose;
            await socket.Initialize();

        }

        private void Socket_OnClose(AppWebSocket sender, string roomId)
        {
            var socketsList = _activeSockets.Values.First(list => list.Contains(sender));
            socketsList.Remove(sender);
        }

        private async void Socket_OnOpen(AppWebSocket sender, string roomId)
        {
            if(!_activeSockets.ContainsKey(roomId))
            {
                _activeSockets.TryAdd(roomId, new List<AppWebSocket>() { sender });
            } else
            {
                _activeSockets[roomId].Add(sender);
            }
            var socketsReadyMessage = new WebSocketMessage() { type = "sockets-ready", roomId = roomId };
            var serializedMessage = JsonConvert.SerializeObject(socketsReadyMessage);
            await sender.Send(serializedMessage);
        }

        public void sendToAllInRoom(string roomId, string message)
        {
            _activeSockets[roomId].ForEach(async s => await s.Send(message));
        }

        public override void handleSocketClose(AppWebSocket socket)
        {
            
        }
    }
}