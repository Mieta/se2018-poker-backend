using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class WebSocketRoom
    {
        private readonly string _id;
        private AppWebSocket _hostWebSocket;
        private readonly List<AppWebSocket> _clientWebSockets;

        public string HostId => _hostWebSocket.WebSocketId;

        public WebSocketRoom(string id, AppWebSocket host)
        {
            _id = id;
            _hostWebSocket = host;
            _clientWebSockets = new List<AppWebSocket>();
        }

        public async Task HandleSendingMessage(AppWebSocket sender, string message)
        {
            if (sender == _hostWebSocket)
            {
                SendMessageToParticipants(message);
            }
            else
            {
                await SendMessageToHost(message);
            }
        }

        public void AddHostToRoom(AppWebSocket webSocket)
        {
            _hostWebSocket = webSocket;
            SendMessageToParticipants(new BasicMessage {message = "Host connected to the room", type = "warning"}
                .ToJsonString());
        }

        public async Task AddClientToRoom(AppWebSocket clientWebSocket)
        {
            _clientWebSockets.Add(clientWebSocket);
            var jObject = new JObject
            {
                ["type"] = "new-client",
                ["roomId"] = _id,
                ["clientId"] = clientWebSocket.WebSocketId
            };
            var serializedJson = JsonConvert.SerializeObject(jObject);
            await SendMessageToHost(serializedJson);
        }

        public void RemoveSocketFromRoom(AppWebSocket socket)
        {
            if (socket == _hostWebSocket)
            {
                _hostWebSocket = null;
                SendMessageToParticipants(new BasicMessage {message = "Host left the room", type = "warning"}
                    .ToJsonString());
            }
            else
            {
                _clientWebSockets.Remove(socket);
            }
        }

        public async Task SendMessageToParticipant(string participantId, string message)
        {
            var clientWebSocket = _clientWebSockets.First(ws => ws.WebSocketId == participantId);
            await clientWebSocket.Send(message);
        }

        private void SendMessageToParticipants(string message)
        {
            _clientWebSockets.ForEach(async s => await s.Send(message));
        }

        public async Task SendMessageToOthers(string senderId, string message)
        {
            if (senderId == _hostWebSocket.WebSocketId)
            {
                SendMessageToParticipants(message);
            }
            else
            {
                await _hostWebSocket.Send(message);
                _clientWebSockets.Where(ws => ws.WebSocketId != senderId)
                    .ToList()
                    .ForEach(async ws => await ws.Send(message));
            }
        }

        private async Task SendMessageToHost(string message)
        {
            if (_hostWebSocket == null)
            {
                throw new WebSocketException(new BasicMessage
                {
                    message = "Host web socket was not initialized or it is closed",
                    type =
                        "error"
                }.ToJsonString());
            }
            else
            {
                await _hostWebSocket.Send(message);
            }
        }
    }
}