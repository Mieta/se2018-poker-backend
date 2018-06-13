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
        public string WebSocketId { get; private set; }

        public event MessageReceivedEventHandler OnMessageReceived;
        public event ConnectionOpenedEventHandler OnOpen;
        public event ConnectionClosedEventHandler OnClose;

        public delegate void MessageReceivedEventHandler(AppWebSocket sender, string message);
        public delegate void ConnectionOpenedEventHandler(AppWebSocket sender, string roomId, Boolean isClientSocket);
        public delegate void ConnectionClosedEventHandler(AppWebSocket sender, string roomId);


        public AppWebSocket(WebSocket webSocket)
        {
            socket = webSocket;
            WebSocketId = System.Guid.NewGuid().ToString();
        }

        public async Task Initialize()
        {
            var buffer = new byte[1024 * 8];
            WebSocketReceiveResult wsresult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer),
            CancellationToken.None);
            var initialMessage = System.Text.Encoding.Default.GetString(buffer);
            var parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(initialMessage);
            Array.Clear(buffer, 0, buffer.Length);
            while(parsedMessage.type != "init-host" && parsedMessage.type != "init-client")
            {
                var errorMessage = new BasicMessage() { message = "Web socket was not initialized. Send initialization message first.", type = "error" };
                string serializedErrorMessage = JsonConvert.SerializeObject(errorMessage);
                await Send(serializedErrorMessage);
                wsresult = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = System.Text.Encoding.Default.GetString(buffer);
                parsedMessage = JsonConvert.DeserializeObject<WebSocketMessage>(message);
                Array.Clear(buffer, 0, buffer.Length);
            }
            bool isClientSocket = parsedMessage.type == "init-client";
            OnOpen(this, parsedMessage.roomId, isClientSocket);
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
