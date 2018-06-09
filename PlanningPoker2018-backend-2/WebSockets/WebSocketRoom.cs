using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class WebSocketRoom
    {
        private string id;
        private AppWebSocket _hostWebSocket;
        private readonly List<AppWebSocket> _clientWebSockets;

        public WebSocketRoom(string id, AppWebSocket host)
        {
            this.id = id;
            _hostWebSocket = host;
            _clientWebSockets = new List<AppWebSocket>();
        }

        public async Task HandleSendingMessage(AppWebSocket sender, string message)
        {
            if(sender == _hostWebSocket)
            {
                SendMessageToParticipants(message);
            } else
            {
                await SendMessageToHost(message);
            }
        }

        public void AddHostToRoom(AppWebSocket webSocket)
        {
            _hostWebSocket = webSocket;
            SendMessageToParticipants("{'message': 'Host connected to the room', 'type': 'warning'}");
        }

        public async Task AddClientToRoom(AppWebSocket clientWebSocket)
        {
            _clientWebSockets.Add(clientWebSocket);
            var jObject = new JObject
            {
                ["type"] = "new-client",
                ["roomId"] = id,
                ["clientId"] = clientWebSocket.WebSocketId
            };
            var serializedJson = JsonConvert.SerializeObject(jObject);
            await SendMessageToHost(serializedJson);
        }

        public void RemoveSocketFromRoom(AppWebSocket socket)
        {
            if(socket == _hostWebSocket)
            {
                _hostWebSocket = null;
                SendMessageToParticipants("{'message': 'Host left the room', 'type': 'warning'}");
            }
            else
            {
                _clientWebSockets.Remove(socket);
            }
        }

        private void SendMessageToParticipants(string message)
        {
            _clientWebSockets.ForEach(async s => await s.Send(message));
        }

        private async Task SendMessageToHost(string message)
        {
            if (_hostWebSocket == null)
            {
                throw new WebSocketException("Host web socket was not initialized or it is closed");
            } else
            {
                await _hostWebSocket.Send(message);
            }
            
        }
    }
}
