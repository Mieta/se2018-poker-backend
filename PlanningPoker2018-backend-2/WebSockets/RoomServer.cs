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

        public static void initialize(IApplicationBuilder app, string Path)
        {
            _instance = new RoomServer(app, Path);
        }


        private readonly ConcurrentDictionary<string, WebSocketRoom> _activeRooms =new ConcurrentDictionary<string, WebSocketRoom>();

        public RoomServer(IApplicationBuilder app, string Location) : base(app, Location)
        {
        }

        public override void handleSocketClose(AppWebSocket socket)
        {
            
        }

        public async override void handleNewMessage(AppWebSocket socket, string message)
        {
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
            var messageType = parsedMessage.type;
            //WORKAROUND to fix problem with end of message
            var messageToSend = JsonConvert.SerializeObject(parsedMessage);
            if(_activeRooms.ContainsKey(parsedMessage.roomId))
            {
                try
                {
                    await _activeRooms[parsedMessage.roomId].handleSendingMessage(socket, messageToSend);
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

        public async override Task handleNewSocket(AppWebSocket socket)
        {
            socket.OnMessageReceived += handleNewMessage;
            socket.OnOpen += Socket_OnOpen;
            socket.OnClose += Socket_OnClose;
            await socket.Initialize();
            
        }

        private void Socket_OnClose(AppWebSocket sender, string roomId)
        {
            _activeRooms[roomId].removeSocketFromRoom(sender);
        }

        private async void Socket_OnOpen(AppWebSocket sender, string roomId, bool isClientSocket)
        {
            var socketsReadyMessage = new WebSocketMessage() { type = "sockets-ready", roomId = roomId };
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
                _activeRooms[roomId].addClientToRoom(sender);
            }
            else
            {
                _activeRooms[roomId].addHostToRoom(sender);
            }
            await sender.Send(serializedMessage);
        }
    }
}