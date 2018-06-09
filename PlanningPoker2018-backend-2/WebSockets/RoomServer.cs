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
    public class RoomServer : AppWebSocketServer
    {
        private static RoomServer _instance;

        public static RoomServer Instance => _instance;

        public static void Initialize(IApplicationBuilder app, string Path)
        {
            _instance = new RoomServer(app, Path);
        }


        private readonly ConcurrentDictionary<string, WebSocketRoom> _activeRooms =new ConcurrentDictionary<string, WebSocketRoom>();

        private RoomServer(IApplicationBuilder app, string location) : base(app, location)
        {
        }

        public override void handleSocketClose(AppWebSocket socket)
        {
            
        }

        public override async void handleNewMessage(AppWebSocket socket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            //WORKAROUND to fix problem with end of message
            var messageToSend = JsonConvert.SerializeObject(parsedMessage);
            if(_activeRooms.ContainsKey(parsedMessage.roomId))
            {
                try
                {
                    await _activeRooms[parsedMessage.roomId].HandleSendingMessage(socket, messageToSend);
                } 
                catch(WebSocketException ex)
                {
                    await socket.Send("{'message': '" + ex.Message + "', 'type': 'error' }");
                }
            }
            else
            {
                await socket.Send("{'message': 'Room not found', 'type': 'error' }");
            }
        }

        public override async Task handleNewSocket(AppWebSocket socket)
        {
            socket.OnMessageReceived += handleNewMessage;
            socket.OnOpen += Socket_OnOpen;
            socket.OnClose += Socket_OnClose;
            await socket.Initialize();
            
        }

        private void Socket_OnClose(AppWebSocket sender, string roomId)
        {
            _activeRooms[roomId].RemoveSocketFromRoom(sender);
        }

        private async void Socket_OnOpen(AppWebSocket sender, string roomId, bool isClientSocket)
        {
            var socketsReadyMessage = new WebSocketMessage() { type = "sockets-ready", roomId = roomId, socketId = sender.WebSocketId};
            var serializedMessage = JsonConvert.SerializeObject(socketsReadyMessage);
            if (!_activeRooms.ContainsKey(roomId))
            {
                if(isClientSocket)
                {
                    await sender.Send("You have no access to the room");
                    return;
                }
                else
                {
                    _activeRooms.TryAdd(roomId, new WebSocketRoom(roomId, sender));
                }
            }
            else if(isClientSocket)
            {
                await _activeRooms[roomId].AddClientToRoom(sender);
            }
            else
            {
                _activeRooms[roomId].AddHostToRoom(sender);
            }
            await sender.Send(serializedMessage);
        }
    }
}