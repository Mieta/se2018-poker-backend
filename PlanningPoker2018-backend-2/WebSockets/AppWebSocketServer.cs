using Microsoft.AspNetCore.Builder;
using PlanningPoker2018_backend_2.Fleck;
using PlanningPoker2018_backend_2.Fleck.Interfaces;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace PlanningPoker2018_backend_2.WebSockets
{
    public abstract class AppWebSocketServer : IWebSocketHandler
    {
        protected WebSocketServer server;

        public AppWebSocketServer(IApplicationBuilder app, string Location)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == Location)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        AppWebSocket appWebSocket = new AppWebSocket(webSocket);
                        await handleNewSocket(appWebSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        public abstract Task handleNewSocket(AppWebSocket socket);
        public abstract void handleSocketClose(AppWebSocket socket);
        public abstract void handleNewMessage(AppWebSocket socket, string message);
    }
}