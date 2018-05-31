using Newtonsoft.Json;
using PlanningPoker2018_backend_2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public class AppWebSocket
    {
        private WebSocket socket;

        public event MessageReceivedEventHandler OnMessageReceived;
        public event ConnectionStateChangedEventHandler OnOpen;
        public event ConnectionStateChangedEventHandler OnClose;

        public delegate void MessageReceivedEventHandler(AppWebSocket sender, string message);
        public delegate void ConnectionStateChangedEventHandler(AppWebSocket sender, string roomId);


        public AppWebSocket(WebSocket webSocket)
        {
            socket = webSocket;
        }

        public async Task Initialize()
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult wsresult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer),
            CancellationToken.None);
            var initialMessage = System.Text.Encoding.Default.GetString(buffer);
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(initialMessage);
            Array.Clear(buffer, 0, buffer.Length);
            while(parsedMessage.type != "init-ws")
            {
                string initErrorMessage = "{'message': 'Web socket was not initialized. Send initialization message first.'}";
                await Send(initErrorMessage);
                wsresult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = System.Text.Encoding.Default.GetString(buffer);
                parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
                Array.Clear(buffer, 0, buffer.Length);
            }
            OnOpen(this, parsedMessage.roomId);
            while (!wsresult.CloseStatus.HasValue)
            {
                wsresult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if(wsresult.MessageType == WebSocketMessageType.Text)
                {
                    var receivedMessage = System.Text.Encoding.Default.GetString(buffer);
                    OnMessageReceived(this, receivedMessage);
                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
            OnClose(this, parsedMessage.roomId);
            await socket.CloseAsync(wsresult.CloseStatus.Value, wsresult.CloseStatusDescription,
   CancellationToken.None);
        }

        public async Task Send(string message)
        {
            var messageBytes = System.Text.Encoding.Default.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(messageBytes, 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
