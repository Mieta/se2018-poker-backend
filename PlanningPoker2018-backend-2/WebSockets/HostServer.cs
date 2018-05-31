using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
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

        public static void initialize(IApplicationBuilder app, string Path)
        {
            _instance = new HostServer(app, Path);
        }


        private readonly ConcurrentDictionary<string, AppWebSocket> _activeSockets =
            new ConcurrentDictionary<string, AppWebSocket>();

        public HostServer(IApplicationBuilder app, string Location) : base(app, Location)
        {
        }

        public override void handleSocketClose(AppWebSocket socket)
        {
            
        }

        public override void handleNewMessage(AppWebSocket socket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            var messageType = parsedMessage.type;
            ClientServer.Instance.sendToAllInRoom(parsedMessage.roomId, message);
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
            var keyToRemove = _activeSockets.First(s => s.Value == sender).Key;
            _activeSockets.Remove(keyToRemove, out var removedSocket);
        }

        private async void Socket_OnOpen(AppWebSocket sender, string roomId)
        {
            _activeSockets.TryAdd(roomId, sender);
            var socketsReadyMessage = new WebSocketMessage() { type = "sockets-ready", roomId = roomId };
            var serializedMessage = JsonConvert.SerializeObject(socketsReadyMessage);
            await sender.Send(serializedMessage);
        }

        public async Task sendToHost(string roomId, string message)
        {
            if(_activeSockets.ContainsKey(roomId))
            {
                await _activeSockets[roomId].Send(message);
            }
        }
    }
}