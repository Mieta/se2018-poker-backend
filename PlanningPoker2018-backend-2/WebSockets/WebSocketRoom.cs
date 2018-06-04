using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class WebSocketRoom
    {
        private string id;
        private AppWebSocket hostWebSocket;
        private List<AppWebSocket> clientWebSockets;

        public WebSocketRoom(string id, AppWebSocket host)
        {
            this.id = id;
            this.hostWebSocket = host;
            clientWebSockets = new List<AppWebSocket>();
        }

        public async Task handleSendingMessage(AppWebSocket sender, string message)
        {
            if(sender == hostWebSocket)
            {
                sendMessageToParticipants(message);
            } else
            {
                await sendMessageToHost(message);
            }
        }

        public void addHostToRoom(AppWebSocket hostWebSocket)
        {
            this.hostWebSocket = hostWebSocket;
            sendMessageToParticipants("{'message': 'Host connected to the room', 'type': 'warning'}");
        }

        public void addClientToRoom(AppWebSocket clientWebSocket)
        {
            clientWebSockets.Add(clientWebSocket);
        }

        public void removeSocketFromRoom(AppWebSocket socket)
        {
            if(socket == hostWebSocket)
            {
                hostWebSocket = null;
                sendMessageToParticipants("{'message': 'Host left the room', 'type': 'warning'}");
            }
            else
            {
                clientWebSockets.Remove(socket);
            }
        }

        public void sendMessageToParticipants(string message)
        {
            clientWebSockets.ForEach(async s => await s.Send(message));
        }

        public async Task sendMessageToHost(string message)
        {
            if (hostWebSocket == null)
            {
                throw new WebSocketException("Host web socket was not initialized or it is closed");
            } else
            {
                await hostWebSocket.Send(message);
            }
            
        }
    }
}
