using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
                    await room.HandleSendingMessage(socket, message);
                    if (!(parsedMessage.content["estimates"] is JArray estimates) || estimates.Count <= 1) return;
                    var estimatesList = estimates.ToObject<List<WsEstimate>>()
                        .OrderBy(e => e.estimate)
                        .ToList();
                    var minEstimate = estimatesList.First();
                    var maxEstimate = estimatesList.Last();

                    var startDiscussionMessage =
                        new WebSocketMessage()
                        {
                            roomId = parsedMessage.roomId,
                            type = "start-discussion",
                            socketId = parsedMessage.socketId
                        };
                    var serializedDisussionMessage = JsonConvert.SerializeObject(startDiscussionMessage);

                    var minEstimates = estimatesList.Where(e => e.estimate == minEstimate.estimate).ToList();
                    minEstimates.ForEach(async e =>
                        await room.SendMessageToParticipant(e.socketId, serializedDisussionMessage)
                    );
                    if (minEstimate.estimate != maxEstimate.estimate)
                    {
                        var maxEstimates = estimatesList.Where(e => e.estimate == maxEstimate.estimate).ToList();
                        maxEstimates.ForEach(async e =>
                            await room.SendMessageToParticipant(e.socketId, serializedDisussionMessage)
                        );
                    }
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