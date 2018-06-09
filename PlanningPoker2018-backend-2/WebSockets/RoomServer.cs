using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlanningPoker2018_backend_2.Entities;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class RoomServer : AppWebSocketServer
    {
        private static RoomServer _instance;

        public static RoomServer Instance => _instance;

        public static void Initialize(IApplicationBuilder app, string path)
        {
            _instance = new RoomServer(app, path);
        }


        private readonly ConcurrentDictionary<string, WebSocketRoom> _activeRooms =
            new ConcurrentDictionary<string, WebSocketRoom>();

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
            if (_activeRooms.ContainsKey(parsedMessage.roomId))
            {
                var room = _activeRooms[parsedMessage.roomId];
                if (parsedMessage.type == "discussion")
                {
                    if (parsedMessage.content["estimates"] is JArray estimates)
                    {
                        foreach (var estimationData in estimates)
                        {
                            var estimationValue = estimationData["estimate"];
                            var estimatorId = estimationData["socketId"];
                            //TODO check max and min values
                        }
                    }

                    await room.HandleSendingMessage(socket, message);
                }
                else
                {
                    try
                    {
                        await _activeRooms[parsedMessage.roomId].HandleSendingMessage(socket, messageToSend);
                    }
                    catch (WebSocketException ex)
                    {
                        await socket.Send(new BasicMessage {message = ex.Message, type = "error"}.ToJsonString());
                    }
                }
            }
            else
            {
                await socket.Send(new BasicMessage {message = "Room not found", type = "error"}.ToJsonString());
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
            var socketsReadyMessage =
                new WebSocketMessage() {type = "sockets-ready", roomId = roomId, socketId = sender.WebSocketId};
            var serializedMessage = JsonConvert.SerializeObject(socketsReadyMessage);
            if (!_activeRooms.ContainsKey(roomId))
            {
                if (isClientSocket)
                {
                    await sender.Send(new BasicMessage {message = "You have no access to the room", type = "error"}
                        .ToJsonString());
                    return;
                }
                else
                {
                    _activeRooms.TryAdd(roomId, new WebSocketRoom(roomId, sender));
                }
            }
            else if (isClientSocket)
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